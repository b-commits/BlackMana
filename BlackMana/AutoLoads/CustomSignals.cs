using Godot;

namespace BlackMana.AutoLoads;

internal interface ICustomSignals
{
    void EmitRequestMove(RequestMoveEvent requestMoveEvent);
    void EmitPrintMapPosition(Vector2 localPosition);
}

internal sealed partial class CustomSignals : Node, ICustomSignals
{
    internal const string ScenePath = $"{ScenePaths.RootPath}/CustomSignals";

    [Signal] public delegate void RequestMoveEventHandler(RequestMoveEvent requestMoveEvent);
    [Signal] public delegate void PrintMapPositionEventHandler(Vector2 localPosition);

    public void EmitRequestMove(RequestMoveEvent requestMoveEvent)
        => EmitSignal(SignalName.RequestMove, requestMoveEvent);

    public void EmitPrintMapPosition(Vector2 localPosition)
        => EmitSignal(SignalName.PrintMapPosition, localPosition);
}

internal sealed partial class RequestMoveEvent : GodotObject
{
    public Vector2I CurrentMapPosition { get; init; }
    public Vector2I NextMapPosition { get; init; }
}