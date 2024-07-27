using Godot;

namespace Sandbox.TileMap;

internal sealed class Tile
{
    public Vector2I Position { get; }
    public TileTexture TileTexture { get; }

    public Tile(Vector2I position, TileTexture tileTexture)
    {
        Position = position;
        TileTexture = tileTexture;
    }
}