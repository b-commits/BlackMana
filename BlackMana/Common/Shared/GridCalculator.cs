// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
using Godot;

namespace BlackMana.Common.Shared;

internal static class GridCalculator
{
    internal static Vector2 CartesianToIsometric(Vector2 vector) 
        => new(vector.X - vector.Y, (vector.X + vector.Y) / 2);
}