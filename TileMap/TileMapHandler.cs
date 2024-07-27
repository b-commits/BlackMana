using System.Linq;
using Godot;

namespace Sandbox.TileMap;

internal sealed partial class TileMapHandler : Godot.TileMap
{
	private Tile currentTile;
	private Tile previousTile;
	
	public override void _Ready() => GetAllTiles();
	
	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventMouseButton)  
			return;
		
		PrintMouseDebugInformation();
		
		if (@event.IsActionPressed(ActionProvider.LEFT_MOUSE_BUTTON))
			SelectCell();
		
		if (@event.IsActionPressed(ActionProvider.RIGHT_MOUSE_BUTTON))
			RemoveTile();

		if (@event.IsAction(ActionProvider.MIDDLE_MOUSE_BUTTON))
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

		var astarGrid = new AStarGrid2D();
		astarGrid.Region = GetUsedRect();
		astarGrid.CellSize = new Vector2(32, 16);
		astarGrid.DiagonalMode = AStarGrid2D.DiagonalModeEnum.OnlyIfNoObstacles;
		astarGrid.Update();

		var from = currentTile.Position;
		var to = LocalToMap(GetLocalMousePosition());

		var idPath = astarGrid.GetIdPath(from, to);
		
		foreach (var vector2I in idPath)
		{
			SetCell(0, vector2I, TileProvider.ManaStarTile.TileTexture.SourceId, 
				TileProvider.ManaStarTile.TileTexture.AtlasCoords);
		}
		
		
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
	
}