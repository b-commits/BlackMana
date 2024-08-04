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
	private readonly IPathfinder _aStarGridProvider;
	private ISelectableManager<Player.Player> _selectableManager;

	public TileMapHandler()
	{
		_aStarGridProvider = new AStarGridPathfinder(GetUsedRect(), TileSet.TileSize);
	}

	public override void _Ready()
	{
		var player1 = GetNode<Player.Player>("Player");
		var player2 = GetNode<Player.Player>("Player2");
		player1.Selected = true;
		var characters = new List<Player.Player> { player1, player2 };
		_selectableManager = new SelectableManager<Player.Player>(characters);
	}
	
	public override void _Input(InputEvent @event)
	{
		if (!MouseDeviceController.IsMouseClick(@event))  
			return;

		if (@event.IsActionPressed(ActionProvider.LEFT_MOUSE_BUTTON))
			SelectCell(LocalToMap(GetLocalMousePosition()));
	}

	private void SelectCell(Vector2I mapCoords)
	{
		if (CellHasSelectable(mapCoords))
			_selectableManager.SelectByCoords(mapCoords);

		if (!CellHasSelectable(mapCoords) && _selectableManager.HasActive())
		{
			var mapPath = _aStarGridProvider.GetPath(LocalToMap(_selectableManager.GetActive().Position), mapCoords);
			_selectableManager.GetActive().SetPath(mapPath);
		}
	}

	private bool CellHasSelectable(Vector2I mapCoords)
		=>_selectableManager.SelectByCoords(mapCoords) is not null;
	
	private void RemoveTile()
		=> SetCell(0, LocalToMap(GetLocalMousePosition()));
	
		
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