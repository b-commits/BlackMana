using Godot;

namespace BlackMana.Common.Movement;

internal sealed class CharacterAnimationController(AnimatedSprite2D sprite, CharacterAnimationSet animations)
    : ICharacterAnimationController
{
    public void PlayWalk(Vector2 direction)
    {
        var animation = (direction.X, direction.Y) switch
        {
            (0, < 0) => animations.WalkN,
            (0, > 0) => animations.WalkS,
            (< 0, 0) => animations.WalkW,
            (> 0, 0) => animations.WalkE,
            (> 0, < 0) => animations.WalkNE,
            (< 0, < 0) => animations.WalkNW,
            (> 0, > 0) => animations.WalkSE,
            (< 0, > 0) => animations.WalkSW,
            _ => animations.IdleFrame
        };
        sprite.Animation = animation;
    }

    public void OnSelect() => sprite.Animation = animations.IdleSelectedFrame;
    public void OnDeselect() => sprite.Animation = animations.IdleFrame;
}