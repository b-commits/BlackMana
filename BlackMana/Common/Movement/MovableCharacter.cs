using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackMana.AutoLoads;
using BlackMana.Common.Interfaces;
using Godot;

namespace BlackMana.Common.Movement;

internal abstract partial class MovableCharacter : CharacterBody2D, IMovable, ISelectable
{
    [Export] public bool Selected { get; set; }
    [Export] public Vector2I MapPosition { get; set; }
    [Export] public float Speed { get; set; } = 75.0F;
    [Export] public int HealthPoints { get; set; } = 100;
    [Export] public float AnimationTimeOffset { get; set; } = 0.5F;

    public bool IsMoving { get; private set; }
    private List<Vector2I> MapPath { get; set; }
    private Tween _movementTween;
    protected ICustomSignals CustomSignals { get; private set; }
    protected ICharacterAnimationController AnimationController { get; private set; }

    protected abstract ICharacterAnimationController CreateAnimationController();

    public override void _Ready()
    {
        CustomSignals = GetNode<ICustomSignals>(AutoLoads.CustomSignals.ScenePath);
        AnimationController = CreateAnimationController();
        RegisterDebug();
    }

    private void RegisterDebug()
    {
        var overlay = GetNode<DebugOverlay>(DebugOverlay.ScenePath);
        overlay.Register(Name, () => new Dictionary<string, string>
        {
            ["MapPosition"] = MapPosition.ToString(),
            ["Selected"] = Selected.ToString(),
            ["IsMoving"] = IsMoving.ToString(),
            ["HP"] = HealthPoints.ToString(),
            ["Path"] = MapPath is { Count: > 0 }
                ? string.Join(" → ", MapPath.Select(p => p.ToString()))
                : "none"
        });
    }

    public override void _Process(double delta)
    {
        if (MapPath is not null && MapPath.Count != 0 && Selected)
            MoveByPath();
        else
            IsMoving = false;
    }

    public void SetPath(List<Vector2I> path) => MapPath = path;

    private void MoveByPath()
    {
        if (_movementTween is not null && _movementTween.IsRunning())
            return;

        IsMoving = true;
        var requestMoveEvent = new RequestMoveEvent { CurrentMapPosition = MapPosition, NextMapPosition = MapPath[0] };
        CustomSignals.EmitRequestMove(requestMoveEvent);
    }

    public async Task Move(Vector2 position)
    {
        if (MapPath.Count == 0)
            return;

        MapPosition = MapPath[0];
        TweenPosition(position);
        MapPath.RemoveAt(0);
        await OffsetAnimationChange();

        if (MapPath.Count == 0 && Selected)
            OnSelect();

        if (MapPath.Count == 0 && !Selected)
            OnDeselect();
    }

    private void TweenPosition(Vector2 nextPosition)
    {
        var duration = Position.DistanceTo(nextPosition) / Speed;
        ResolveAnimation(nextPosition);
        _movementTween = CreateTween();
        _movementTween.TweenProperty(this, nameof(Position).ToLower(), nextPosition, duration);
    }

    private void ResolveAnimation(Vector2 nextMapPosition)
    {
        var direction = (nextMapPosition - Position).Normalized();
        AnimationController.PlayWalk(direction);
    }

    private async Task OffsetAnimationChange()
        => await ToSignal(GetTree().CreateTimer(AnimationTimeOffset), SceneTreeTimer.SignalName.Timeout);

    public void OnSelect() => AnimationController.OnSelect();
    public void OnDeselect() => AnimationController.OnDeselect();

    protected AnimatedSprite2D GetAnimatedSprite() => GetNode<AnimatedSprite2D>(nameof(AnimatedSprite2D));
}