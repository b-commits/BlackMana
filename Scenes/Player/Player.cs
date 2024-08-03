using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Sandbox.Common.Interfaces;

namespace Sandbox.Scenes.Player;

internal sealed partial class Player : AnimatedSprite2D, IMovable, ISelectable
{
	private List<Vector2I> path = new();
	[Export] public bool Selected { get; set; }
	public Vector2I Coords { get; set; }

	public override void _Process(double delta)
	{
		if (path.Any()) MoveByPath();
	}

	public void Move(Vector2I mapCoords)
	{
		Position = Coords;
	}
	
	private void MoveByPath()
	{
		Position = path[0];
		path.RemoveAt(0);
	}
	
	public void SetPath(List<Vector2I> mapPath)
		=> path = mapPath;
	

	public void OnSelect(Action action)
	{
		throw new NotImplementedException();
	}
}
