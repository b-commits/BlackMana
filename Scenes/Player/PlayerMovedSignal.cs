using Godot;
using Sandbox.Scenes.TileMap;

namespace Sandbox.Scenes.Player;

internal sealed partial class PlayerMovedSignal : GodotObject
{
    public Tile PreviousTile { get; init; }
}