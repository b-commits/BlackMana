using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Sandbox.TileMap;

internal sealed partial class TileMapHandler : Godot.TileMap
{
	private Tile currentTile;
	private Tile previousTile;
	private List<Vector2I> path;
	
	public override void _Ready() => GetAllTiles();

	public override void _Process(double delta)
	{
		MoveDownPath();
		
		// SetCell powinien odbywac sie tutaj na podstawie currentTile?
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

	private void SelectCell()
	{
		var mapClickCoords = LocalToMap(GetLocalMousePosition());
		if (previousTile is null)
		{
			previousTile = new Tile(mapClickCoords, GetTileTexture(mapClickCoords));
			currentTile = new Tile(mapClickCoords, TileProvider.ManaStarTile.TileTexture);
			
			SetCell(0, mapClickCoords, currentTile.TileTexture.SourceId, 
				currentTile.TileTexture.AtlasCoords);
		}
		else
		{
			SetCell(0, previousTile.Position, previousTile.TileTexture.SourceId, 
				previousTile.TileTexture.AtlasCoords);

			previousTile = new Tile(mapClickCoords, GetTileTexture(mapClickCoords));
			currentTile = new Tile(mapClickCoords, TileProvider.ManaStarTile.TileTexture);
			
			SetCell(0, currentTile.Position, currentTile.TileTexture.SourceId, 
				currentTile.TileTexture.AtlasCoords);
		}
	}

	private void FindPath()
	{
		if (currentTile is null)
		{
			GD.PushWarning("Current tile is not selected. Cannot find path.");
			return;
		}

		var aStarGrid = new AStarGrid2D();
		aStarGrid.Region = GetUsedRect();
		aStarGrid.CellSize = new Vector2(32, 16);
		aStarGrid.DiagonalMode = AStarGrid2D.DiagonalModeEnum.OnlyIfNoObstacles;
		aStarGrid.Update();

		var from = currentTile.Position;
		var to = LocalToMap(GetLocalMousePosition());

		path = aStarGrid.GetIdPath(from, to).ToList();
		PrintPathDebugInformation();
	}

	private void MoveDownPath()
	{
		if (path is null || currentTile is null || !path.Any())
			return;

		if (path.Count > 1)
		{
			path.RemoveAt(0);
		}

		if (path.Count == 0) 
			return;
		
		currentTile.Position = path[0];
		SetCell(0, currentTile.Position, TileProvider.ManaStarTile.TileTexture.SourceId,
			TileProvider.ManaStarTile.TileTexture.AtlasCoords);
		GD.Print($"Tile after iteration: {currentTile.Position}");
		
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