using Godot;

namespace BlackMana.Scenes.TileMap;

internal sealed class TileTexture
{
    public Vector2I AtlasCoords { get; init; }
    public int SourceId { get; init; }
}

internal sealed class TileAsset
{
    public TileAssetName Name { get; init; }
    public TileTexture TileTexture { get; init; }
}

internal enum TileAssetName
{
    MANA_TILE,
    MANA_STAR_TILE
}

internal static class TileProvider
{
    internal static readonly TileAsset ManaTile = new()
    {
        Name = TileAssetName.MANA_TILE,
        TileTexture = new TileTexture
        {
            AtlasCoords = new Vector2I(1, 0),
            SourceId = 3
        }
    };

    public static readonly TileAsset ManaStarTile = new()
    {
        Name = TileAssetName.MANA_STAR_TILE,
        TileTexture = new TileTexture
        {
            AtlasCoords = new Vector2I(0, 0),
            SourceId = 1
        }
    };
}