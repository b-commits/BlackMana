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
        var selectable = _selectables.SingleOrDefault(x => x.MapPosition == mapCoords);
        return selectable is null ? null : Select(selectable);
    }

    private ISelectable SelectNext()
    {
        var currentSelectable = GetActive();
        var currentIndex = _selectables.IndexOf(currentSelectable);

        return currentIndex + 1 < _selectables.Count 
            ? SelectByIndex(currentIndex + 1) 
            : Select(_selectables[0]);
    }

    public bool IsAnySelectableMoving()
    {
        var players = GetAll();
        return players.OfType<Player.Player>().Any(player => player.IsMoving);
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

    private ISelectable Select(ISelectable selectable)
    {
        if (GetActive() == selectable)
            return selectable;
        
        DeselectCurrentSelectable();
        selectable.Select();
        return selectable;
    }
    
    public IEnumerable<ISelectable> GetAll() => _selectables;
    
    public IEnumerable<ISelectable> GetInactive()
        => _selectables.Where(x => !x.Selected).ToList();
    
    public bool HasActive() => _selectables.Exists(x => x.Selected);

    public ISelectable GetActive() => _selectables.SingleOrDefault(x => x.Selected);

    private void DeselectCurrentSelectable() => GetActive().Deselect();
}