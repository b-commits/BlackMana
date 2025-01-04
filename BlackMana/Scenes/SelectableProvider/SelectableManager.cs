using System.Collections.Generic;
using System.Linq;
using BlackMana.Common.Actions;
using BlackMana.Common.Interfaces;
using Godot;

namespace BlackMana.Scenes.SelectableProvider;

internal sealed partial class SelectableManager : Node2D, ISelectableManager
{
    private List<ISelectable> _selectables;

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed(ActionProvider.KeyR)) 
            return;

        SelectNext();
    }

    public ISelectable SelectByCoords(Vector2I mapCoords)
    {
        var selectableAtCoords = _selectables.SingleOrDefault(x => x.MapPosition == mapCoords);
        return selectableAtCoords is null ? null : Select(selectableAtCoords);
    }

    private void SelectNext()
    {
        var currentSelectableIndex = _selectables.IndexOf(GetActive());

        if (currentSelectableIndex + 1 < _selectables.Count)
            SelectByIndex(currentSelectableIndex + 1);
        else
            SelectByIndex(0);
    }

    public bool IsAnySelectableMoving()
    {
        return _selectables.OfType<IMovable>().Any(x => x.IsMoving);
    }

    public void SetSelectables(List<ISelectable> selectables)
    {
        _selectables = selectables;
    }

    public ISelectable SelectByIndex(int index)
    {
        var selectable = _selectables[index];
        return Select(selectable);
    }

    private void DeselectCurrentSelectable()
    {
        GetActive().Deselect();  
    } 

    private ISelectable Select(ISelectable selectable)
    {
        if (IsAnySelectableMoving())
            return null;
        
        if (!HasActive())
            return Activate(selectable);

        return GetActive() == selectable 
            ? DeselectCurrent() 
            : SwitchActiveSelectable(selectable);
    }

    private static ISelectable Activate(ISelectable selectable)
    {
        selectable.Select();
        return selectable;
    }

    private ISelectable DeselectCurrent()
    {
        DeselectCurrentSelectable();
        return null;
    }

    private ISelectable SwitchActiveSelectable(ISelectable newSelectable)
    {
        GetActive().Deselect();
        newSelectable.Select();
        return newSelectable;
    }
    
    public IEnumerable<ISelectable> GetInactive()
        => _selectables.Where(x => !x.Selected).ToList();
    
    public bool HasActive() 
        => _selectables.Exists(x => x.Selected);

    public ISelectable GetActive() 
        => _selectables.SingleOrDefault(x => x.Selected);


}