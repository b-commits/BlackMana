using System.Collections.Generic;
using BlackMana.Common.Interfaces;
using Godot;

namespace BlackMana.Scenes.SelectableProvider;

internal interface ISelectableManager
{
    ISelectable SelectByCoords(Vector2I mapCoords);
    ISelectable GetActive();
    ISelectable SelectByIndex(int index);
    IEnumerable<ISelectable> GetAll();
    IEnumerable<ISelectable> GetInactive();
    bool HasActive();
    bool IsAnySelectableMoving();
    void SetSelectables(List<ISelectable> selectables);
}
