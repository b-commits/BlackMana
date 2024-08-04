using System.Collections.Generic;
using System.Linq;
using Godot;
using Sandbox.Common.Interfaces;

namespace Sandbox.Common.SelectableManager;

internal interface ISelectableManager<T> where T : class, ISelectable
{
    T SelectByCoords(Vector2I mapCoords);
    T GetActive();
    T Select(T selectable);
    List<T> GetAll();
    bool HasActive();
}

internal sealed class SelectableManager<T> : ISelectableManager<T>
    where T : Node2D, ISelectable
{
    private readonly List<T> selectables;

    public SelectableManager(List<T> selectables)
    {
        this.selectables = selectables;
    }

    public T SelectByCoords(Vector2I mapCoords)
    {
        var selectable= selectables.SingleOrDefault(x => x.MapPosition == mapCoords);
        return selectable is null ? null : Select(selectable);
    }

    public T Select(T selectable)
    {
        DeselectCurrentSelectable();
        selectable.Selected = true;
        selectable.OnSelect();
        return selectable;
    }
    
    public List<T> GetAll() => selectables;

    public bool HasActive() => selectables.Exists(x => x.Selected);

    public T GetActive() => selectables.SingleOrDefault(x => x.Selected);

    private void DeselectCurrentSelectable()
    {
        selectables.Where(x => x.Selected).ToList().ForEach(x =>
        {
            x.Selected = !x.Selected;
            x.OnDeselect();
        });
    }
    
}