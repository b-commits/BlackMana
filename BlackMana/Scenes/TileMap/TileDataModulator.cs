using Godot;

namespace BlackMana.Scenes.TileMap;

internal class TileDataModulator
{
    private Vector2I? _highlightedCell;
    private readonly TileMapLayer _tileMapLayer;

    public TileDataModulator(TileMapLayer tileMapLayer)
    {
        _tileMapLayer = tileMapLayer;
    }

    public void HighlightCell(Vector2I mapCoords)
    {
        _highlightedCell = mapCoords;
        _tileMapLayer.NotifyRuntimeTileDataUpdate();
    }

    private bool IsHighlighted(Vector2I coords)
    {
        return _highlightedCell.HasValue && _highlightedCell.Value == coords;
    }

    public void ApplyHighlight(Vector2I coords, TileData tileData)
    {
        if (IsHighlighted(coords))
        {
            tileData.Modulate = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
}