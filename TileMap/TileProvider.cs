using Godot;

namespace Sandbox;

public class TileTexture
{
    public Vector2I AtlasCoords { get; set; }
    public int SourceId { get; set; }
}

public class TileAsset
{
    public TileAssetName Name { get; set; }
    public TileTexture TileTexture { get; set; }
}

public enum TileAssetName
{
    MANA_TILE,
    MANA_STAR_TILE
}

public static class TileProvider
{
    public static readonly TileAsset ManaTile = new()
    {
        Name = TileAssetName.MANA_TILE,
        TileTexture = new TileTexture
        {
            AtlasCoords = new Vector2I(0, 0),
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