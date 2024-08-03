using System.Collections.Generic;
using System.Linq;
using Godot;
using Sandbox.Common.Actions;
using Sandbox.Common.AStarGridProvider;
using Sandbox.Common.MouseDeviceController;
using Sandbox.Common.SelectableManager;

namespace Sandbox.Scenes.TileMap;

internal sealed partial class TileMapHandler : Godot.TileMap
{
	private Tile currentTile;
	private Tile previousTile;
	private List<Vector2I> path;

	private readonly IPathfinder _aStarGridProvider;
	private readonly ISelectableManager<Scenes.Player.Player> _selectableManager;

	public TileMapHandler(List<Scenes.Player.Player> players)
	{
		_selectableManager = new SelectableManager<Scenes.Player.Player>(players);
		_aStarGridProvider = new AStarGridPathfinder(GetUsedRect(), TileSet.TileSize);
	}
	
	public TileMapHandler() { }

	public override void _Ready()
	{
		currentTile = new Tile(default, default);
		GetAllTiles();
	}

	public override void _Process(double delta)
	{
		MovePlayer();
	}
	
	public override void _Input(InputEvent @event)
	{
		if (MouseDeviceController.IsMouseClick(@event))  
			return;
		
		PrintMouseDebugInformation();
		
		// if (@event.IsActionPressed(ActionProvider.LEFT_MOUSE_BUTTON))
		// 	SelectCell();
		//
		// if (@event.IsActionPressed(ActionProvider.RIGHT_MOUSE_BUTTON))
		// 	RemoveTile();

		if (@event.IsActionPressed(ActionProvider.LEFT_MOUSE_BUTTON))
			SelectCell(LocalToMap(GetLocalMousePosition()));
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
		
		var player = GetNode<Scenes.Player.Player>("Player");

		player.Move((Vector2I)MapToLocal(currentTile.Position));
	}

	private void ResetCellTexture()
	{
		var selectablePosition = _selectableManager.GetActive().Position;
		var cellTileData = GetTileTexture(selectablePosition);
		
	}

	private void SelectCell(Vector2I mapCoords)
	{
		if (CellHasSelectable(mapCoords) && !_selectableManager.HasActive())
			_selectableManager.SelectByCoords(mapCoords);

		if (!CellHasSelectable(mapCoords) && _selectableManager.HasActive()) 
			_selectableManager.GetActive().SetPath(_aStarGridProvider.GetPath(currentTile.Position, mapCoords));
	}

	private bool CellHasSelectable(Vector2I mapCoords)
		=> _selectableManager.SelectByCoords(mapCoords) is not null;
	
	private TileTexture GetTileTexture(Vector2I mapCoords) 
		=> new() 
		{
			AtlasCoords = GetCellAtlasCoords(0, mapCoords),
			SourceId = GetCellSourceId(0, mapCoords)
		};

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