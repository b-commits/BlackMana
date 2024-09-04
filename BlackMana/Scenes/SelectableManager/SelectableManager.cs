using System.Collections.Generic;
using System.Linq;
using BlackMana.Common.Actions;
using BlackMana.Common.Interfaces;
using Godot;

namespace BlackMana.Scenes.SelectableManager;

internal interface ISelectableManager
{
    ISelectable SelectByCoords(Vector2I mapCoords);
    ISelectable GetActive();
    
    ISelectable SelectByIndex(int index);
    List<ISelectable> GetAll();
    IEnumerable<ISelectable> GetInactive();
    bool HasActive();
    bool IsAnySelectableMoving();
}

internal sealed partial class SelectableManager : Node2D, ISelectableManager
{
    private readonly List<ISelectable> selectables;

    public SelectableManager(List<ISelectable> selectables)
    {
        this.selectables = selectables;
    }
    
    public SelectableManager() { }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed(ActionProvider.KeyR)) 
            return;

        SelectNext();
    }

    public ISelectable SelectByCoords(Vector2I mapCoords)
    {
        var selectable = selectables.SingleOrDefault(x => x.MapPosition == mapCoords);
        return selectable is null ? null : Select(selectable);
    }

    private ISelectable SelectNext()
    {
        var currentSelectable = GetActive();
        var currentIndex = selectables.IndexOf(currentSelectable);

        return currentIndex + 1 < selectables.Count 
            ? SelectByIndex(currentIndex + 1) 
            : Select(selectables[0]);
    }

    public bool IsAnySelectableMoving()
    {
        var players = GetAll();
        return players.OfType<Player.Player>().Any(player => player.IsMoving);
    }

    public ISelectable SelectByIndex(int index)
    {
        var selectable = selectables[index];
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
    
    public List<ISelectable> GetAll() => selectables;
    
    public IEnumerable<ISelectable> GetInactive()
        => selectables.Where(x => !x.Selected).ToList();
    
    public bool HasActive() => selectables.Exists(x => x.Selected);

    public ISelectable GetActive() => selectables.SingleOrDefault(x => x.Selected);

    private void DeselectCurrentSelectable() => GetActive().Deselect();
}