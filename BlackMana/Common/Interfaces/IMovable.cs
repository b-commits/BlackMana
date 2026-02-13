using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace BlackMana.Common.Interfaces;

internal interface IMovable
{
    public bool IsMoving { get; }
    void SetPath(List<Vector2I> path);
    Task Move(Vector2 position);
}