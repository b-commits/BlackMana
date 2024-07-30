using Godot;

namespace Sandbox.Common;

public static class Helpers
{
    public static bool IsMouseClick(InputEvent inputEvent)  =>
        inputEvent is InputEventMouseButton && inputEvent.IsPressed();
}