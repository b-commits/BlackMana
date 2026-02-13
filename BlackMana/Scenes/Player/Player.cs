using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using BlackMana.AutoLoads;
using BlackMana.Common.Interfaces;
using BlackMana.Common.Movement;

namespace BlackMana.Scenes.Player;

internal sealed partial class Player : CharacterBody2D, IMovable, ISelectable
{
    [Export] public bool Selected { get; set; }
    [Export] public Vector2I MapPosition { get; set; }
    [Export] public float Speed { get; set; } = 75.0F;
    [Export] public int HealthPoints { get; set; } = 100;
    [Export] public float AnimationTimeOffset { get; set; } = 0.5F;

    public List<Vector2I> MapPath { get; set; }
    public bool IsMoving { get; set; }
    private Tween MovementTween { get; set; }
    private ICustomSignals _customSignals;
    private CharacterAnimationController _animationController;

    public override void _Ready()
    {
        _customSignals = GetNode<ICustomSignals>(CustomSignals.ScenePath);
        _animationController = new CharacterAnimationController(GetAnimatedSprite());
        _customSignals.EmitRequestMove(new RequestMoveEvent { CurrentMapPosition = MapPosition });
    }

    public override void _Process(double delta)
    {
        if (MapPath is not null && MapPath.Count != 0 && Selected)
            MoveByPath();
        else
            IsMoving = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Selected) MoveWithPhysics();
    }

    private void MoveWithPhysics()
    {
        var inputDirection = new Vector2(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        );
        Speed = 50.0F;
        Velocity = inputDirection * Speed;

        if (inputDirection != Vector2.Zero)
            _animationController.PlayWalk(inputDirection);

        MoveAndSlide();
    }

    public void MoveByPath()
    {
        if (MovementTween is not null && MovementTween.IsRunning())
            return;

        IsMoving = true;
        var requestMoveEvent = new RequestMoveEvent { CurrentMapPosition = MapPosition, NextMapPosition = MapPath[0] };
        _customSignals.EmitRequestMove(requestMoveEvent);
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

    public void TweenPosition(Vector2 nextPosition)
    {
        var duration = Position.DistanceTo(nextPosition) / Speed;
        ResolveAnimation(nextPosition);
        MovementTween = CreateTween();
        MovementTween.TweenProperty(this, nameof(Position).ToLower(), nextPosition, duration);
    }

    private async Task OffsetAnimationChange()
        => await ToSignal(GetTree().CreateTimer(AnimationTimeOffset), SceneTreeTimer.SignalName.Timeout);

    public void ResolveAnimation(Vector2 nextMapPosition)
    {
        var direction = (nextMapPosition - Position).Normalized();
        _animationController.PlayWalk(direction);
    }
    
    public Action GetAnimation(Vector2 nextMapPosition)
    {
        var direction = (nextMapPosition - Position).Normalized();
        return () => _animationController.PlayWalk(direction);
    }

    public void SetPath(List<Vector2I> path) => MapPath = path;

    private AnimatedSprite2D GetAnimatedSprite() => GetNode<AnimatedSprite2D>(nameof(AnimatedSprite2D));

    public void OnSelect() => _animationController.OnSelect();
    public void OnDeselect() => _animationController.OnDeselect();
}