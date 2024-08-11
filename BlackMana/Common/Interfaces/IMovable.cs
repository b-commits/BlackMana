using System;
using System.Collections.Generic;
using Godot;

namespace BlackMana.Common.Interfaces;

internal interface IMovable
{
    public List<Vector2I> MapPath { get; set; }
    void SetPath(List<Vector2I> path);
    void MoveByPath();
    void ResolveAnimation(Vector2 nextMapPosition);
    Action GetAnimation(Vector2 nextMapPosition);
    void Move(Vector2 position);
    void TweenPosition(Vector2 nextPosition);
}
