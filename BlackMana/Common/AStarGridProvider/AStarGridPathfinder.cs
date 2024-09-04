using System.Collections.Generic;
using System.Linq;
using Godot;

namespace BlackMana.Common.AStarGridProvider;

internal interface IPathfinder
{
    List<Vector2I> GetPath(Vector2I from, Vector2I to);
    List<Vector2I> GetPathWithDisabledNodes(Vector2I from, Vector2I to,
        IEnumerable<Vector2I> disabledPoints);
}

internal sealed partial class AStarGridPathfinder : Node2D, IPathfinder
{
    private readonly AStarGrid2D aStarGrid = new();
    private List<Vector2I> path;
    [Export] private AStarGrid2D.DiagonalModeEnum _diagonalModeEnum;

    public AStarGridPathfinder(Rect2I rect, Vector2I tileSize)
    {
        aStarGrid.Region = rect;
        aStarGrid.DiagonalMode = _diagonalModeEnum;
        aStarGrid.CellSize = new Vector2(tileSize.X, tileSize.Y);
        aStarGrid.Update();
    }

    public AStarGridPathfinder() { }

    public List<Vector2I> GetPath(Vector2I from, Vector2I to)
    {
        path = aStarGrid.GetIdPath(from, to).ToList();
        return path;
    }

    public List<Vector2I> GetPathWithDisabledNodes(Vector2I from, Vector2I to,
        IEnumerable<Vector2I> disabledPoints)
    {
        disabledPoints.ToList().ForEach(x => aStarGrid.SetPointSolid(x));
        path = aStarGrid.GetIdPath(from, to).ToList();
        
        aStarGrid.Update();
        return path;
    }

    private void PrintPathDebugInformation()
        => path?.ForEach(vector => GD.PrintRaw($"{vector}"));
}