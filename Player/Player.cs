using Godot;

namespace Sandbox.Player;

internal interface ISelectable
{
	
}

internal interface IMovable
{
	void Move(Vector2I mapCoords);
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
}
