using BlackMana.AutoLoads;
using BlackMana.Common.Movement;
using Godot;

namespace BlackMana.Scenes.Player;

internal sealed partial class Player : MovableCharacter
{
    protected override ICharacterAnimationController CreateAnimationController()
        => new CharacterAnimationController(GetAnimatedSprite(), PlayerAnimations.AnimationSet);

    public override void _Ready()
    {
        base._Ready();
        CustomSignals.EmitRequestMove(new RequestMoveEvent { CurrentMapPosition = MapPosition });
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
            AnimationController.PlayWalk(inputDirection);

        MoveAndSlide();
    }
}