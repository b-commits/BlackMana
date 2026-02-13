using System;
using System.Collections.Generic;
using Godot;

namespace BlackMana.Scenes.TileMap;

internal class TileDataModulator(TileMapLayer tileMapLayer)
{
    private const float BaseDarkenAmount = 0.15f;
    private const float DestinationExtraDarken = 0.10f;
    private const float BlinkAmplitude = 0.08f;
    private const float BlinkFrequency = 1.5f;
    private const float HoverDarkenAmount = 0.15f;

    private HashSet<Vector2I> _pathCells = [];
    private HashSet<Vector2I> _cellsToUpdate = [];
    private Vector2I? _destinationCell;
    private bool _isBlinking;
    private double _blinkTime;
    private Vector2I? _hoverCell;

    public void SetBlinkingPath(List<Vector2I> path)
    {
        var previousPathCells = _pathCells;
        _pathCells = [];
        _destinationCell = null;

        if (path is { Count: > 0 })
        {
            _destinationCell = path[^1];
            foreach (var cell in path)
                _pathCells.Add(cell);
        }

        _cellsToUpdate = new HashSet<Vector2I>(previousPathCells);
        _cellsToUpdate.UnionWith(_pathCells);

        _isBlinking = _pathCells.Count > 0;
        _blinkTime = 0;

        tileMapLayer.NotifyRuntimeTileDataUpdate();
    }

    public void ClearPath()
    {
        _cellsToUpdate = new HashSet<Vector2I>(_pathCells);
        _pathCells = [];
        _destinationCell = null;
        _isBlinking = false;
        _blinkTime = 0;

        tileMapLayer.NotifyRuntimeTileDataUpdate();
    }

    public void UpdateBlink(double delta)
    {
        if (!_isBlinking)
            return;

        _blinkTime += delta;
        tileMapLayer.NotifyRuntimeTileDataUpdate();
    }

    public void SetHoverCell(Vector2I coords)
    {
        if (_hoverCell.HasValue)
            _cellsToUpdate.Add(_hoverCell.Value);

        _hoverCell = coords;
        _cellsToUpdate.Add(coords);

        tileMapLayer.NotifyRuntimeTileDataUpdate();
    }

    public void ClearHoverCell()
    {
        if (!_hoverCell.HasValue)
            return;

        _cellsToUpdate.Add(_hoverCell.Value);
        _hoverCell = null;

        tileMapLayer.NotifyRuntimeTileDataUpdate();
    }

    public bool ShouldModulate(Vector2I coords)
        => _cellsToUpdate.Contains(coords);

    public void ApplyHighlight(Vector2I coords, TileData tileData)
    {
        if (_pathCells.Contains(coords))
        {
            var blinkOffset = _isBlinking
                ? (float)Math.Sin(_blinkTime * BlinkFrequency * Math.Tau) * BlinkAmplitude
                : 0f;

            var darken = BaseDarkenAmount + blinkOffset;

            if (_destinationCell.HasValue && coords == _destinationCell.Value)
                darken += DestinationExtraDarken;

            tileData.Modulate = tileData.Modulate.Darkened(darken);
        }
        else if (_hoverCell.HasValue && coords == _hoverCell.Value)
        {
            tileData.Modulate = tileData.Modulate.Darkened(HoverDarkenAmount);
        }
    }
}