using System.Collections.Generic;
using Godot;

namespace Sandbox.Player;

internal interface ISelectable
{
	public bool Selected { get; set; }
	public Vector2I Position { get; set; }
}

internal interface IMovable
{
	void Move(Vector2I mapCoords);
	void Move(List<Vector2I> path);
}

internal sealed partial class Player : CharacterBody2D, IMovable, ISelectable
{
	public override void _Ready()
	{
		GD.PrintRaw("Ready");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public void Move(Vector2I mapCoords)
	{
		Position = mapCoords;
	}

	public void Move(List<Vector2I> path)
	{
		throw new System.NotImplementedException();
	}

	public bool Selected { get; set; }
	public Vector2I Position { get; set; }
}
