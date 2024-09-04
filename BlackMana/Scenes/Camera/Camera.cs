using BlackMana.AutoLoads;
using Godot;

namespace BlackMana.Scenes.TileMap;

internal sealed partial class Camera : Camera2D
{
    [Export] public float ScrollSpeed { get; set; } = 1.0F;
    [Export] public Vector2 MaxZoomIn { get; set; } = new(10, 10);
    [Export] public Vector2 MaxZoomOut { get; set; } = new(2, 2);
    
    private IMouseController _mouseController;
    
    public override void _Ready()
    {
        _mouseController = GetNode<IMouseController>(MouseController.ScenePath);
    }

    public override void _Input(InputEvent @event)
    {
        if (!_mouseController.IsMouseClick(@event))
            return;
        
        HandleZoom((InputEventMouseButton)@event);
    }

    private void HandleZoom(InputEventMouseButton mouseButtonEvent)
    {
        if (mouseButtonEvent.ButtonIndex != MouseButton.WheelDown &&
            mouseButtonEvent.ButtonIndex != MouseButton.WheelUp)
            return;

        var zoomDelta = new Vector2(mouseButtonEvent.Factor, mouseButtonEvent.Factor); 
        
        if ((Zoom + zoomDelta > MaxZoomIn && mouseButtonEvent.ButtonIndex == MouseButton.WheelUp)
            || Zoom - zoomDelta < MaxZoomOut  && mouseButtonEvent.ButtonIndex == MouseButton.WheelDown)
            return;
        
        var zoomIncrement = mouseButtonEvent.Factor * ScrollSpeed;
        
        Zoom += mouseButtonEvent.ButtonIndex == MouseButton.WheelUp 
            ? new Vector2(zoomIncrement, zoomIncrement) 
            : new Vector2(-zoomIncrement, -zoomIncrement);
    }
}