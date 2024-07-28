using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Sandbox.TileMap;

internal sealed partial class TileMapHandler : Godot.TileMap
{
	private Tile currentTile;
	private Tile previousTile;
	private List<Vector2I> path;
	private AStarGrid2D aStarGrid = new();
	
	public override void _Ready() => GetAllTiles();

	public override void _Process(double delta)
	{
		MovePlayer();
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventMouseButton)  
			return;
		
		PrintMouseDebugInformation();

		if (@event.IsActionPressed(ActionProvider.LEFT_MOUSE_BUTTON))
			SelectCell();
		
		if (@event.IsActionPressed(ActionProvider.RIGHT_MOUSE_BUTTON))
			RemoveTile();

		if (@event.IsActionPressed(ActionProvider.MIDDLE_MOUSE_BUTTON))
			FindPath();
	}

	private void MovePlayer()
	{
		if (currentTile == null)
			return;

		if (path?.Any() == true)
		{
			if (path.Count > 1)
				path.RemoveAt(0);
    
			currentTile.Position = path[0];
		}

		if (previousTile != null)
			SetCell(0, previousTile.Position, previousTile.TileTexture.SourceId, previousTile.TileTexture.AtlasCoords);

		previousTile = new Tile(currentTile.Position, GetTileTexture(currentTile.Position));

		SetCell(0, currentTile.Position, TileProvider.ManaStarTile.TileTexture.SourceId, 
			TileProvider.ManaStarTile.TileTexture.AtlasCoords);
		
		var player = GetNode<Player>("Player");
		
		
		player.Position = MapToLocal(currentTile.Position);
	}

	private void SelectCell()
	{
		path?.Clear();
		var mapClickCoords = LocalToMap(GetLocalMousePosition());
		currentTile = new Tile(mapClickCoords, GetTileTexture(mapClickCoords));
	}
	
	private void FindPath()
	{
		aStarGrid.Region = GetUsedRect();
		aStarGrid.CellSize = new Vector2(TileSet.TileSize.X, TileSet.TileSize.Y);
		aStarGrid.Update();

		var from = currentTile.Position;
		var to = LocalToMap(GetLocalMousePosition());

		path = aStarGrid.GetIdPath(from, to).ToList();
		
		PrintPathDebugInformation();
	}

	private TileTexture GetTileTexture(Vector2I mapCoords)
	{
		return new TileTexture
		{
			AtlasCoords = GetCellAtlasCoords(0, mapCoords),
			SourceId = GetCellSourceId(0, mapCoords)
		};
	}

	private void RemoveTile()
	{
		var mapCoords = LocalToMap(GetLocalMousePosition());
		SetCell(0, mapCoords);
	}
		
	private void GetAllTiles()
	{
		var layers = Enumerable.Range(0, GetLayersCount()).ToList();
		layers.ForEach(layer => GD.Print(GetUsedCells(layer)));
	}

	private void PrintMouseDebugInformation()
	{
		GD.Print("Global " + (Vector2I)GetGlobalMousePosition());
		GD.Print("Local: " + (Vector2I)GetLocalMousePosition());
		GD.Print("Map: " + LocalToMap(GetLocalMousePosition()));
	}

	private void PrintPathDebugInformation()
	{
		foreach (var vector2I in path)
		{
			GD.PrintRaw($"{vector2I} ");
		}
	}
	
}