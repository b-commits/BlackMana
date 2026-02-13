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
	private List<Vector2I> _pendingPath;
	private Vector2I? _pendingDestination;

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

	public override void _Process(double delta)
	{
		_tileDataModulator.UpdateBlink(delta);
	}

	private void RegisterEventHandlers()
	{
		_customSignals.RequestMove += OnMoveRequested;
		_customSignals.PrintMapPosition += OnPrintMapPosition;
	}

	private void OnMoveRequested(RequestMoveEvent requestMoveEvent)
	{
		var activeSelectable = (IMovable)_selectableManager.GetActive();
		activeSelectable.Move(MapToLocal(requestMoveEvent.NextMapPosition));
	}

	private void OnPrintMapPosition(Vector2 localPosition)
		=> GD.Print(LocalToMap(localPosition));

	public override void _Input(InputEvent @event)
	{
		if (!(_mouseController.IsMouseLeftClick(@event) || _mouseController.IsMouseHover(@event)))
			return;

		var mouseMapPosition = LocalToMap(GetLocalMousePosition());

		if (_mouseController.IsMouseLeftClick(@event))
			HandleClick(mouseMapPosition);

		if (_mouseController.IsMouseHover(@event) && _selectableManager.HasActive())
			_tileDataModulator.SetHoverCell(mouseMapPosition);
	}

	public override void _TileDataRuntimeUpdate(Vector2I coords, TileData tileData)
	{
		if (!GetOccupiedCells().Contains(coords) && _selectableManager.HasActive())
		{
			_tileDataModulator.ApplyHighlight(coords, tileData);
		}
	}

	public override bool _UseTileDataRuntimeUpdate(Vector2I coords)
		=> _tileDataModulator.ShouldModulate(coords);

	private void HandleClick(Vector2I mapCoords)
	{
		if (_selectableManager.IsAnySelectableMoving())
			return;

		var selectableAtCoords = _selectableManager.SelectByCoords(mapCoords);
		if (selectableAtCoords is not null || !_selectableManager.HasActive())
		{
			ClearPendingPath();
			return;
		}

		if (_pendingDestination == mapCoords)
		{
			ConfirmMovement();
			return;
		}

		ShowPendingPath(mapCoords);
	}

	private void ShowPendingPath(Vector2I destination)
	{
		_pendingPath = _pathFinder.GetPathWithDisabledNodes(
			_selectableManager.GetActive().MapPosition, destination, GetOccupiedCells());
		_pendingDestination = destination;
		_tileDataModulator.SetBlinkingPath(_pendingPath);
	}

	private void ConfirmMovement()
	{
		if (_pendingPath is null)
			return;

		var activeSelectable = (IMovable)_selectableManager.GetActive();
		activeSelectable.SetPath(_pendingPath);
		ClearPendingPath();
	}

	private void ClearPendingPath()
	{
		_pendingPath = null;
		_pendingDestination = null;
		_tileDataModulator.ClearPath();
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

		return [player, companion];
	}
}