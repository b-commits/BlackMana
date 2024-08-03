using Godot;

namespace Sandbox.Common.MouseDeviceController;

internal static class MouseDeviceController
{
    public static bool IsMouseClick(InputEvent inputEvent)  =>
        inputEvent is InputEventMouseButton && inputEvent.IsPressed();
}