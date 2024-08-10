using Godot;

namespace Sandbox.Common.Shared;

internal static class Helpers
{
    private static Vector2 CartesianToIsometric(Vector2 vector) 
        => new(vector.X - vector.Y, (vector.X + vector.Y) / 2);
}