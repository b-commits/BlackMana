using Godot;

namespace Sandbox.Common.MouseDeviceController;

public static class MouseDeviceController
{
    public static bool IsMouseClick(InputEvent inputEvent)  =>
        inputEvent is InputEventMouseButton && inputEvent.IsPressed();
}