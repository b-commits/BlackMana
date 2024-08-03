using System.Collections.Generic;
using System.Linq;
using Godot;
using Sandbox.Player;

namespace Sandbox.Common;

internal interface ISelectableManager<T> where T : class, ISelectable
{
    T SelectByCoords(Vector2I mapCoords);
    T GetActive();
    List<T> GetAll();
    bool HasActive();
}

internal sealed class SelectableManager<T> : ISelectableManager<T>
    where T : class, ISelectable
{
    private readonly List<T> selectables;

    public SelectableManager(List<T> selectables)
    {
        this.selectables = selectables;
    }

    public T SelectByCoords(Vector2I mapCoords)
        => selectables.SingleOrDefault(x => x.Position == mapCoords);

    public T GetActive()
        => selectables.SingleOrDefault(x => x.Selected);

    public List<T> GetAll()
        => selectables;

    public bool HasActive()
        => selectables.Exists(x => x.Selected);
}