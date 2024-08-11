using Godot;

namespace Sandbox.AutoLoads;

internal interface IMouseDeviceController
{
    bool IsMouseClick(InputEvent inputEvent);
    void PrintMouseDebugInformation();
}

internal sealed partial class MouseController : Node2D, IMouseDeviceController
{
    public const string ScenePath = "root/MouseController";
    
    private readonly ICustomSignals _customSignals;

    public MouseController()
    {
        _customSignals = GetNode<ICustomSignals>(CustomSignals.ScenePath);
    }
    
    public bool IsMouseClick(InputEvent inputEvent)
    {
        return inputEvent is InputEventMouseButton && inputEvent.IsPressed();
    }

    public void PrintMouseDebugInformation()
    {
        GD.Print("Global " + GetGlobalMousePosition());
        GD.Print("Local: " + GetLocalMousePosition());
        _customSignals.EmitPrintMapPosition(GetLocalMousePosition());
    }
}