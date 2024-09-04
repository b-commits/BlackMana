using System.Collections.Generic;
using System.Linq;
using Godot;
using BlackMana.AutoLoads;
using BlackMana.Common.Actions;
using BlackMana.Common.AStarGridProvider;
using BlackMana.Common.Interfaces;
using BlackMana.Scenes.SelectableManager;

namespace BlackMana.Scenes.TileMap;

internal sealed partial class TileMapHandler
    : Godot.TileMap
{
    private IPathfinder _aStarGridProvider;
    private ISelectableManager _selectableManager;
    private IMouseController _mouseController;
    private CustomSignals _customSignals;

    public override void _Ready()
    {
        _aStarGridProvider = new AStarGridPathfinder(GetUsedRect(), TileSet.TileSize);
        _mouseController = GetNode<IMouseController>(MouseController.ScenePath);
        _customSignals = GetNode<CustomSignals>(CustomSignals.ScenePath);
        _selectableManager = new SelectableManager.SelectableManager(GetSeededPlayers());
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
        if (!_mouseController.IsMouseClick(@event))
            return;

        if (@event.IsActionPressed(ActionProvider.LeftMouseButton))
            SelectCell(LocalToMap(GetLocalMousePosition()));
    }

    private void SelectCell(Vector2I mapCoords)
    {
        if (_selectableManager.IsAnySelectableMoving())
            return;
        
        var selectableAtCoords = _selectableManager.SelectByCoords(mapCoords);
        if (selectableAtCoords is not null || !_selectableManager.HasActive())
            return;
        
        var mapPath = _aStarGridProvider.GetPathWithDisabledNodes(
            _selectableManager.GetActive().MapPosition, mapCoords, GetOccupiedCells());
        var activeSelectable = (Player.Player)_selectableManager.GetActive();
        activeSelectable.SetPath(mapPath);
    }

    private IEnumerable<Vector2I> GetOccupiedCells()
        => _selectableManager.GetInactive().Select(x => x.MapPosition);

    private List<ISelectable> GetSeededPlayers()
    {
        var player = GetNode<ISelectable>("Player");
        var companion = GetNode<ISelectable>("Player2");
        player.MapPosition = new Vector2I(0, 1);
        companion.MapPosition = new Vector2I(3, 1);
        player.OnSelect();

        player.Selected = true;
        return new List<ISelectable> { player, companion };
    }

    private void RemoveTile()
        => SetCell(0, LocalToMap(GetLocalMousePosition()));

    private void GetAllTiles()
    {
        var layers = Enumerable.Range(0, GetLayersCount()).ToList();
        layers.ForEach(layer => GD.Print(GetUsedCells(layer)));
    }
}