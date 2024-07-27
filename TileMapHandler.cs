using System.Linq;
using Godot;

namespace Sandbox;

internal sealed class Tile
{
	public Vector2I Position { get; }
	public TileTexture TileTexture { get; }

	public Tile(Vector2I position, TileTexture tileTexture)
	{
		Position = position;
		TileTexture = tileTexture;
	}
}

internal sealed partial class TileMapHandler : TileMap
{
	private Tile currentTile;
	private Tile previousTile;
	
	public override void _Ready()
		=> GetAllTiles();
	
	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventMouse || !@event.IsActionPressed(ActionProvider.LEFT_MOUSE_BUTTON)) 
			return;
		
		var mapCoords = LocalToMap(GetGlobalMousePosition());
		SelectCell(mapCoords);
	}

	private void SelectCell(Vector2I mapClickCoords)
	{
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

	private TileTexture GetTileTexture(Vector2I mapCoords)
	{
		return new TileTexture
		{
			AtlasCoords = GetCellAtlasCoords(0, mapCoords),
			SourceId = GetCellSourceId(0, mapCoords)
		};
	} 

	private void RemoveTile(Vector2I vector) 
		=> SetCell(0, vector, 3, new Vector2I(1, 0));

	private void GetAllTiles()
	{
		var layers = Enumerable.Range(0, GetLayersCount()).ToList();
		layers.ForEach(layer => GD.Print(GetUsedCells(layer)));
	}
	
}