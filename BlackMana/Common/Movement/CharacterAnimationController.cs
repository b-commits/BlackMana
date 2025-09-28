using Godot;
using BlackMana.Scenes.Player;

namespace BlackMana.Common.Movement;

internal sealed class CharacterAnimationController(AnimatedSprite2D sprite) : ICharacterAnimationController
{
    public void PlayWalk(Vector2 direction)
    {
        var animation = (direction.X, direction.Y) switch
        {
            (0, < 0) => PlayerAnimations.WalkSelectedNorth,
            (0, > 0) => PlayerAnimations.WalkSelectedSouth,
            (< 0, 0) => PlayerAnimations.WalkSelectedWest,
            (> 0, 0) => PlayerAnimations.WalkSelectedEast,
            (> 0, < 0) => PlayerAnimations.WalkSelectedNorthEast,
            (< 0, < 0) => PlayerAnimations.WalkSelectedNorthWest,
            (> 0, > 0) => PlayerAnimations.WalkSelectedSouthEast,
            (< 0, > 0) => PlayerAnimations.WalkSelectedSouthWest,
            _ => PlayerAnimations.IdleFrame
        };
        sprite.Animation = animation;
    }

    public void OnSelect() => sprite.Animation = PlayerAnimations.IdleSelectedFrame;
    public void OnDeselect() => sprite.Animation = PlayerAnimations.IdleFrame;
}