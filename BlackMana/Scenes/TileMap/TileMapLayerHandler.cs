using System.Collections.Generic;
using System.Linq;
using Godot;
using BlackMana.AutoLoads;
using BlackMana.Common.AStarGridProvider;
using BlackMana.Common.Interfaces;
using BlackMana.Scenes.SelectableProvider;

namespace BlackMana.Scenes.TileMap;

internal sealed partial class TileMapLayerHandler : TileMapLayer
{
	private IPathfinder _pathFinder;
	private SelectableManager _selectableManager;
	private IMouseController _mouseController;
	private CustomSignals _customSignals;
	private TileDataModulator _tileDataModulator;
	
	public override void _Ready()
	{
		_tileDataModulator = new TileDataModulator(this);
		_pathFinder = new AStarGridPathfinder(GetUsedRect(), TileSet.TileSize);
		_mouseController = GetNode<IMouseController>(MouseController.ScenePath);
		_customSignals = GetNode<CustomSignals>(CustomSignals.ScenePath);
		_selectableManager = GetNode<SelectableManager>($"%{nameof(SelectableManager)}");
		_selectableManager.SetSelectables(GetSeededPlayers());
		RegisterEventHandlers();
	}

	private void RegisterEventHandlers()
	{
		_customSignals.RequestMove += OnMoveRequested;
		_customSignals.PrintMapPosition += OnPrintMapPosition;
	}

	private void OnMoveRequested(RequestMoveEvent requestMoveEvent)
	{
		var activeSelectable = (Player.Player)_selectableManager.GetActive();  
		activeSelectable.Move(MapToLocal(requestMoveEvent.NextMapPosition));
	}

	private void OnPrintMapPosition(Vector2 localPosition)
		=> GD.Print(LocalToMap(localPosition));

	public override void _Input(InputEvent @event)
	{
		if (!(_mouseController.IsMouseClick(@event) || _mouseController.IsMouseHover(@event)))
			return;
		
		var mouseMapPosition = LocalToMap(GetLocalMousePosition());

		if (_mouseController.IsMouseLeftClick(@event))
			SelectCell(mouseMapPosition);
		
		var tileData = GetCellTileData(mouseMapPosition);

		if (_mouseController.IsMouseHover(@event))
			_tileDataModulator.NotifyCellsUpdated(mouseMapPosition, tileData);
	}
	
	public override void _TileDataRuntimeUpdate(Vector2I coords, TileData tileData)
	{
		if (!GetOccupiedCells().Contains(coords) && _selectableManager.HasActive())
		{
			_tileDataModulator.ApplyHighlight(coords, tileData);
		} 
	}

	public override bool _UseTileDataRuntimeUpdate(Vector2I coords)
	{
		return _tileDataModulator.ShouldModulate(coords);
	}
	
	private void SelectCell(Vector2I mapCoords)
	{
		_selectableManager.SelectByCoords(mapCoords);
		if (_selectableManager.GetActive() is null)
			return;
		
		var mapPath = _pathFinder.GetPathWithDisabledNodes(
			_selectableManager.GetActive().MapPosition, mapCoords, GetOccupiedCells());
		var activeSelectable = (IMovable)_selectableManager.GetActive();
		activeSelectable.SetPath(mapPath);
	}

	private IEnumerable<Vector2I> GetOccupiedCells()
		=> _selectableManager.GetInactive().Select(x => x.MapPosition);

	private List<ISelectable> GetSeededPlayers()
	{
		var player = GetNode<ISelectable>($"{nameof(Player)}");
		var companion = GetNode<ISelectable>($"{nameof(Player)}2");
		player.MapPosition = new Vector2I(0, 1);
		companion.MapPosition = new Vector2I(3, 1);
		player.Select();
		
		return new List<ISelectable> { player, companion };
	}
}
