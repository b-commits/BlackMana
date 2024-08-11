using Godot;

namespace Sandbox.AutoLoads;

internal interface ICustomSignals
{
    void EmitRequestMove(Vector2I mapPosition);
}

public partial class CustomSignals : Node, ICustomSignals
{
    internal const string ScenePath = "/root/CustomSignals";
    
    [Signal] public delegate void RequestMoveEventHandler(Vector2I mapPosition);
    public void EmitRequestMove(Vector2I mapPosition)
        => EmitSignal(SignalName.RequestMove, mapPosition);
}