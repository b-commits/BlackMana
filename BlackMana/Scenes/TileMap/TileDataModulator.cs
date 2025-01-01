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

    public bool ShouldModulate(Vector2I coords)
    {
        return _highlightedCell.HasValue && _highlightedCell.Value == coords;
    }

    public void ApplyHighlight(Vector2I coords, TileData tileData)
    {
        if (ShouldModulate(coords))
            tileData.Modulate = tileData.Modulate.Darkened(0.15f);
    }
}