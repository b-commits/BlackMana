using Godot;

namespace BlackMana.Common.Interfaces;

internal interface ISelectable
{
    public bool Selected { get; set; }
    public Vector2I MapPosition { get; set; }
    public void OnSelect();
    public void OnDeselect();

    public void Select()
    {
        Selected = true;
        OnSelect();
    }

    public void Deselect()
    {
        Selected = false;
        OnDeselect();
    }
}