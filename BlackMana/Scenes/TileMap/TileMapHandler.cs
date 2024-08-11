using System.Collections.Generic;
using System.Linq;
using Godot;
using BlackMana.AutoLoads;
using BlackMana.Common.Actions;
using BlackMana.Common.AStarGridProvider;
using BlackMana.Common.SelectableManager;

namespace BlackMana.Scenes.TileMap;

internal sealed partial class TileMapHandler 
	: Godot.TileMap
{
	private IPathfinder _aStarGridProvider;
	private ISelectableManager<Player.Player> _selectableManager;
	private IMouseController _mouseController;
	private CustomSignals _customSignals;
	
	public override void _Ready()
	{
		_aStarGridProvider = new AStarGridPathfinder(GetUsedRect(), TileSet.TileSize);
		_mouseController = GetNode<IMouseController>(MouseController.ScenePath);
		_customSignals = GetNode<CustomSignals>(CustomSignals.ScenePath); 
		_selectableManager = new SelectableManager<Player.Player>(SeedPlayers());
		RegisterEventHandlers();		
	}

	private void RegisterEventHandlers()
	{
		_customSignals.RequestMove += OnMoveRequested;
		_customSignals.PrintMapPosition += OnPrintMapPosition;
	}

	private void OnMoveRequested(RequestMoveEvent requestMoveEvent)
	{
		if (requestMoveEvent.NextMapPosition != requestMoveEvent.CurrentMapPosition &&
		    _selectableManager.SelectByCoords(requestMoveEvent.NextMapPosition) is not null)
			return;
		
		_selectableManager.GetActive().Move(MapToLocal(requestMoveEvent.NextMapPosition));
	}

	private void OnPrintMapPosition(Vector2 localPosition)
		=> GD.Print(LocalToMap(localPosition));
	
	public override void _Input(InputEvent @event)
	{
		if (!_mouseController.IsMouseClick(@event))  
			return;
		
		if (@event.IsActionPressed(ActionProvider.LEFT_MOUSE_BUTTON))
			SelectCell(LocalToMap(GetLocalMousePosition()));
	}

	private void SelectCell(Vector2I mapCoords)
	{
		var coordsSelectable = _selectableManager.SelectByCoords(mapCoords);

		if (coordsSelectable is not null || !_selectableManager.HasActive()) 
			return;
		
		var mapPath = _aStarGridProvider.GetPath(_selectableManager.GetActive().MapPosition, 
			mapCoords);
		_selectableManager.GetActive().SetPath(mapPath);
	}

	private List<Player.Player> SeedPlayers()
	{
		var player = GetNode<Player.Player>("Player");
		var companion = GetNode<Player.Player>("Player2");
		player.MapPosition = new Vector2I(0, 1);
		companion.MapPosition = new Vector2I(3, 1);
		
		companion.Selected = true;
		return new List<Player.Player> { player, companion };
	}
	
	private void RemoveTile()
		=> SetCell(0, LocalToMap(GetLocalMousePosition()));
	
	private void GetAllTiles()
	{
		var layers = Enumerable.Range(0, GetLayersCount()).ToList();
		layers.ForEach(layer => GD.Print(GetUsedCells(layer)));
	}
}