namespace BlackMana.Common.Movement;

using Godot;

public interface ICharacterAnimationController
{
    void PlayWalk(Vector2 direction);
    void OnSelect();
    void OnDeselect();
}

