using Godot;
using Sandbox.TileMap;

namespace Sandbox.World;

public partial class WorldHandler : Node2D
{
	private TileMapHandler TileMap;
	private Player.Player player;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}