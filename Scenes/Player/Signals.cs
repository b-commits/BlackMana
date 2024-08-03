using Godot;

namespace Sandbox.Scenes.Player;

public class PlayerMoveSignal : GodotObject
{
    public Vector2I CurrentPosition { get; init; }
    public Vector2I PreviousPosition { get; init; }
}