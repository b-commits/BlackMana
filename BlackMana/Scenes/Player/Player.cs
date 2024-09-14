using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using BlackMana.AutoLoads;
using BlackMana.Common.Interfaces;

namespace BlackMana.Scenes.Player;

internal sealed partial class Player
    : CharacterBody2D, IMovable, ISelectable
{
    [Export] public bool Selected { get; set; }
    [Export] public Vector2I MapPosition { get; set; }
    [Export] public float Speed { get; set; } = 75.0F;
    [Export] public float AnimationTimeOffset { get; set; } = 0.5F;

    public List<Vector2I> MapPath { get; set; }
    public bool IsMoving { get; set; }
    private Tween MovementTween { get; set; }
    private ICustomSignals _customSignals;

    public override void _Ready()
    {
        _customSignals = GetNode<ICustomSignals>(CustomSignals.ScenePath);
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
        
        var walkActions = new Dictionary<Vector2, Action>
        {
            { new Vector2(0, 1), PlayWalkS },
            { new Vector2(-1, 0), PlayWalkW },
            { new Vector2(1, 0), PlayWalkE },
            { new Vector2(0, -1), PlayWalkN },
            { new Vector2(-1, -1), PlayWalkNW },
            { new Vector2(1, -1), PlayWalkNE },
            { new Vector2(-1, 1), PlayWalkSW },
            { new Vector2(1, 1), PlayWalkSE }
        };
        
        if (walkActions.ContainsKey(inputDirection))
        {
            walkActions[inputDirection].Invoke();
        }

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

    public async void Move(Vector2 position)
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
        var animation = GetAnimation(nextMapPosition);
        animation();
    }
    
    public Action GetAnimation(Vector2 nextMapPosition)
    {
        var deltaX = nextMapPosition.X - Position.X;
        var deltaY = nextMapPosition.Y - Position.Y;

        return (deltaX, deltaY) switch
        {
            (0, < 0) => PlayWalkN, (0, > 0) => PlayWalkS, (< 0, 0) => PlayWalkW,
            (> 0, 0) => PlayWalkE, (> 0, < 0) => PlayWalkNE, (< 0, < 0) => PlayWalkNW,
            (> 0, > 0) => PlayWalkSE, (< 0, > 0) => PlayWalkSW, _ => () => { }
        };
    }

    public void SetPath(List<Vector2I> path) => MapPath = path;

    private AnimatedSprite2D GetAnimatedSprite() => GetNode<AnimatedSprite2D>(nameof(AnimatedSprite2D));

    #region Animations

    private void PlayWalkSE() => GetAnimatedSprite().Animation = PlayerAnimations.WalkSESelected;
    private void PlayWalkSW() => GetAnimatedSprite().Animation = PlayerAnimations.WalkSWSelected;
    private void PlayWalkS() => GetAnimatedSprite().Animation = PlayerAnimations.WalkSSelected;
    private void PlayWalkNE() => GetAnimatedSprite().Animation = PlayerAnimations.WalkNESelected;
    private void PlayWalkNW() => GetAnimatedSprite().Animation = PlayerAnimations.WalkNWSelected;
    private void PlayWalkW() => GetAnimatedSprite().Animation = PlayerAnimations.WalkWSelected;
    private void PlayWalkE() => GetAnimatedSprite().Animation = PlayerAnimations.WalkESelected;
    private void PlayWalkN() => GetAnimatedSprite().Animation = PlayerAnimations.WalkNSelected;
    public void OnSelect() => GetAnimatedSprite().Animation = PlayerAnimations.IdleSelectedFrame;
    public void OnDeselect() => GetAnimatedSprite().Animation = PlayerAnimations.IdleFrame;

    #endregion
}