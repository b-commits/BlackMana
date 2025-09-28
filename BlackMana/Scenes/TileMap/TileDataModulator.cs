using System.Collections.Generic;
using Godot;

namespace BlackMana.Scenes.TileMap;

internal class TileDataModulator(TileMapLayer tileMapLayer)
{
    private readonly Dictionary<Vector2I, TileData> _updatedCells = new();
    private KeyValuePair<Vector2I, TileData>? _lastHoveredOver;

    public void NotifyCellsUpdated(Vector2I mapCoords, TileData tileData)
    {
        _updatedCells.TryAdd(mapCoords, tileData);
        _lastHoveredOver = new KeyValuePair<Vector2I, TileData>(mapCoords, tileData);
        tileMapLayer.NotifyRuntimeTileDataUpdate();
    }

    public bool ShouldModulate(Vector2I coords)
    {
        return _updatedCells.ContainsKey(coords);
    }

    public void ApplyHighlight(Vector2I coords, TileData tileData)
    {
        if (_lastHoveredOver.HasValue && coords == _lastHoveredOver.Value.Key)
        {
            tileData.Modulate = tileData.Modulate.Darkened(0.15f);
        }
        else
        {
            tileData.Modulate = _updatedCells.TryGetValue(coords, out var value)
                ? value.Modulate
                : default;
        }
    }
}