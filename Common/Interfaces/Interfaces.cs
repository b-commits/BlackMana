using System;
using System.Collections.Generic;
using Godot;

namespace Sandbox.Common.Interfaces;

internal interface ISelectable
{
    void OnSelect(Action action);
    public bool Selected { get; set; }
    public Vector2I Position { get; set; }
}

internal interface IMovable
{
    void Move(Vector2I mapCoords);
    void SetPath(List<Vector2I> mapPath);
}
