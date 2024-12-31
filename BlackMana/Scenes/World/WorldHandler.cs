using Godot;
using BlackMana.Scenes.TileMap;

namespace BlackMana.Scenes.World;

public partial class WorldHandler : Node2D
{
	private TileMapLayerHandler _tileMapLayer;
	private Scenes.Player.Player player;
	
	public override void _Ready()
    {
        // Method intentionally left empty.
    }

    public override void _Process(double delta)
    {
        // Method intentionally left empty.
    }
}