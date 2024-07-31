using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Sandbox.Common.AStarGridProvider;

internal interface IPathFinder
{
    List<Vector2I> GetPath(Vector2I from, Vector2I to);
}

internal sealed partial class AStarGridProvider : Node2D, IPathFinder
{
    private readonly AStarGrid2D aStarGrid = new();
    private List<Vector2I> path;
    [Export] private AStarGrid2D.DiagonalModeEnum _diagonalModeEnum;
    
    public AStarGridProvider(Rect2I rect, Vector2I tileSize)
    {
        aStarGrid.Region = rect;
        aStarGrid.DiagonalMode = _diagonalModeEnum;
        aStarGrid.CellSize = new Vector2(tileSize.X, tileSize.Y);
        aStarGrid.Update();
    }

    public AStarGridProvider() { }

    public List<Vector2I> GetPath(Vector2I from, Vector2I to)
    {
        GD.Print($"Getting path from {from} to {to}.");
        path = aStarGrid.GetIdPath(from, to).ToList();
        return path;
    }
}