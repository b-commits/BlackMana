using System.Collections.Generic;
using System.Linq;
using Godot;

namespace BlackMana.Common.AStarGridProvider;

internal interface IPathfinder
{
    List<Vector2I> GetPathWithDisabledNodes(Vector2I from, Vector2I to,
        IEnumerable<Vector2I> disabledPoints);
}

internal sealed partial class AStarGridPathfinder : Node2D, IPathfinder
{
    private readonly AStarGrid2D _aStarGrid = new();
    private List<Vector2I> _path;
    [Export] private AStarGrid2D.DiagonalModeEnum _diagonalModeEnum;

    public AStarGridPathfinder(Rect2I rect, Vector2I tileSize)
    {
        _aStarGrid.Region = rect;
        _aStarGrid.DiagonalMode = _diagonalModeEnum;
        _aStarGrid.CellSize = new Vector2(tileSize.X, tileSize.Y);
        _aStarGrid.Update();
    }

    public AStarGridPathfinder() { }

    private List<Vector2I> GetPath(Vector2I from, Vector2I to)
    {
        _path = _aStarGrid.GetIdPath(from, to).ToList();
        return _path;
    }

    public List<Vector2I> GetPathWithDisabledNodes(Vector2I from, Vector2I to,
        IEnumerable<Vector2I> disabledPoints)
    {
        disabledPoints.ToList().ForEach(x => _aStarGrid.SetPointSolid(x));
        _path = GetPath(from, to).ToList();
        
        _aStarGrid.Update();
        return _path;
    }

    private void PrintPathDebugInformation()
        => _path?.ForEach(vector => GD.PrintRaw($"{vector}"));
}