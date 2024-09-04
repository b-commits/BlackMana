using System.Collections.Generic;
using System.Linq;
using BlackMana.Common.Actions;
using Godot;
using BlackMana.Common.Interfaces;

namespace BlackMana.Common.SelectableManager;

internal interface ISelectableManager<T> where T : class, ISelectable 
{
    T SelectByCoords(Vector2I mapCoords);
    T GetActive();
    T SelectByIndex(int index);
    List<T> GetAll();
    List<T> GetInactive();
    bool HasActive();
}

internal sealed partial class SelectableManager<T> : Node2D, ISelectableManager<T>
    where T : Node2D, ISelectable
{
    private readonly List<T> selectables;

    public SelectableManager(List<T> selectables)
    {
        this.selectables = selectables;
    }
    
    public SelectableManager() { }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(ActionProvider.KeyR))
        {
            GD.Print("Pressed R");
            SelectNext();
        }
    }

    public T SelectByCoords(Vector2I mapCoords)
    {
        var selectable = selectables.SingleOrDefault(x => x.MapPosition == mapCoords);
        return selectable is null ? null : Select(selectable);
    }

    public T SelectNext()
    {
        var currentSelectable = GetActive();
        var currentIndex = selectables.IndexOf(currentSelectable);

        return currentIndex + 1 < selectables.Count ? SelectByIndex(currentIndex + 1) : Select(selectables[0]);
    }

    public T SelectByIndex(int index)
    {
        var selectable = selectables[index];
        return Select(selectable);
    }
    
    public T Select(T selectable)
    {
        if (GetActive() == selectable)
            return selectable;

        DeselectCurrentSelectable();
        selectable.Select();
        return selectable;
    }
    
    public List<T> GetAll() => selectables;
    
    public List<T> GetInactive()
        => selectables.Where(x => !x.Selected).ToList();
    
    public bool HasActive() => selectables.Exists(x => x.Selected);

    public T GetActive() => selectables.SingleOrDefault(x => x.Selected);

    private void DeselectCurrentSelectable() => GetActive().Deselect();
}