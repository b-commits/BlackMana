using Godot;

namespace Sandbox.Common.Shared;

internal static class Helpers
{
    internal static Vector2 CartesianToIsometric(Vector2 vector) 
        => new(vector.X - vector.Y, (vector.X + vector.Y) / 2);
}