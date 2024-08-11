using System.Collections.Generic;
using Godot;

namespace Sandbox.Common.Interfaces;

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

internal interface IMovable
{
    public List<Vector2I> MapPath { get; set; }
    void SetPath(List<Vector2I> path);
}
