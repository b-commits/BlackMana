using Godot;

namespace Sandbox.Scenes.TileMap;

internal sealed class Tile
{
    public Vector2I Position { get; set; }
    public TileTexture TileTexture { get; }

    public Tile(Vector2I position, TileTexture tileTexture)
    {
        Position = position;
        TileTexture = tileTexture;
    }
}