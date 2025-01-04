using System.Collections.Generic;
using BlackMana.Common.Interfaces;
using Godot;

namespace BlackMana.Scenes.SelectableProvider;

internal interface ISelectableManager
{
    ISelectable SelectByCoords(Vector2I mapCoords);
    ISelectable SelectByIndex(int index);
    void SetSelectables(List<ISelectable> selectables);
    ISelectable GetActive();
    IEnumerable<ISelectable> GetInactive();
    bool HasActive();
    bool IsAnySelectableMoving();

}
