using Godot;

namespace BlackMana.AutoLoads;

internal interface IMouseController
{
    bool IsMouseClick(InputEvent inputEvent);
    bool IsMouseHover(InputEvent inputEvent);
    void PrintMouseDebugInformation();
}

internal sealed partial class MouseController : Node2D, IMouseController
{
    public const string ScenePath = $"{ScenePaths.Root}/{nameof(MouseController)}";

    private ICustomSignals _customSignals;

    public override void _Ready()
    {
        _customSignals = GetNode<ICustomSignals>(CustomSignals.ScenePath);
    }
    
    public bool IsMouseClick(InputEvent inputEvent)
        => inputEvent is InputEventMouseButton && inputEvent.IsPressed();
    
    public bool IsMouseHover(InputEvent inputEvent)
        => inputEvent is InputEventMouseMotion;
    
    public void PrintMouseDebugInformation()
    {
        GD.Print("Global " + GetGlobalMousePosition());
        GD.Print("Local: " + GetLocalMousePosition());
        _customSignals.EmitPrintMapPosition(GetLocalMousePosition());
    }
}