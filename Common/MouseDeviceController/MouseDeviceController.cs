using Godot;

namespace Sandbox.Common.MouseDeviceController;

internal static class MouseDeviceController
{
    public static bool IsMouseClick(InputEvent inputEvent) =>
        inputEvent is InputEventMouseButton && inputEvent.IsPressed();
    
    // public static void PrintMouseDebugInformation()
    // {
    //     GD.Print("Global " + (Vector2I)GetLocalMousePosition());
    //     GD.Print("Local: " + (Vector2I)GetLocalMousePosition());
    //     GD.Print("Map: " + LocalToMap(GetLocalMousePosition()));
    // }
}