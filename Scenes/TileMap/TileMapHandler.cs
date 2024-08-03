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
		var characters = new List<Node2D> { GetNode<Node2D>("Player") };
		_selectableManager = new SelectableManager<Player.Player>(null);
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
		if (CellHasSelectable(mapCoords) && !_selectableManager.HasActive())
			_selectableManager.SelectByCoords(mapCoords);

		if (!CellHasSelectable(mapCoords) && _selectableManager.HasActive())
		{
			var activeSelectable = _selectableManager.GetActive();
			activeSelectable.SetPath(_aStarGridProvider.GetPath(activeSelectable.Position, mapCoords));
		}
	}

	private bool CellHasSelectable(Vector2I mapCoords)
		=> _selectableManager.SelectByCoords(mapCoords) is not null;
	
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