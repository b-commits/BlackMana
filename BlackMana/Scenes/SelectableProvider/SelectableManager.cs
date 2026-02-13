using System.Collections.Generic;
using System.Linq;
using BlackMana.AutoLoads;
using BlackMana.Common.Actions;
using BlackMana.Common.Interfaces;
using Godot;

namespace BlackMana.Scenes.SelectableProvider;

internal sealed partial class SelectableManager : Node2D
{
    private List<ISelectable> _selectables;
    private ICustomSignals _customSignals;
    private SfxManager _sfxManager;

    public override void _Ready()
    {
        _customSignals = GetNode<ICustomSignals>(CustomSignals.ScenePath);
        _sfxManager = GetNode<SfxManager>(SfxManager.ScenePath);
    }

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
        RegisterDebug();
    }

    private void RegisterDebug()
    {
        var overlay = GetNode<DebugOverlay>(DebugOverlay.ScenePath);
        overlay.Register(nameof(SelectableManager), () =>
        {
            var active = GetActive();
            return new Dictionary<string, string>
            {
                [nameof(active)] = active is Node node ? node.Name : "none",
                [nameof(_selectables.Count)] = _selectables?.Count.ToString() ?? "0",
                [nameof(IsAnySelectableMoving)] = IsAnySelectableMoving().ToString()
            };
        });
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

    private ISelectable Activate(ISelectable selectable)
    {
        selectable.Select();
        _customSignals.EmitSelectionChanged(selectable.HealthPoints, true);
        _sfxManager.PlaySelect();
        return selectable;
    }

    private ISelectable DeselectCurrent()
    {
        DeselectCurrentSelectable();
        _customSignals.EmitSelectionChanged(0, false);
        _sfxManager.PlayDeselect();
        return null;
    }

    private ISelectable SwitchActiveSelectable(ISelectable newSelectable)
    {
        GetActive().Deselect();
        newSelectable.Select();
        _customSignals.EmitSelectionChanged(newSelectable.HealthPoints, true);
        _sfxManager.PlaySelect();
        return newSelectable;
    }
    
    public IEnumerable<ISelectable> GetInactive()
        => _selectables.Where(x => !x.Selected).ToList();
    
    public bool HasActive() 
        => _selectables.Exists(x => x.Selected);

    public ISelectable GetActive() 
        => _selectables.SingleOrDefault(x => x.Selected);


}