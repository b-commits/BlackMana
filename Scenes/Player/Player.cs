using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Sandbox.Common.Interfaces;

namespace Sandbox.Scenes.Player;

internal sealed partial class Player : AnimatedSprite2D, IMovable, ISelectable
{
	private List<Vector2I> path = new();

	public override void _Process(double delta)
	{
		if (path.Any()) MoveByPath();
	}

	public void Move(Vector2I mapCoords)
	{
		Position = MovablePosition;
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

	public bool Selected { get; set; }
	public Vector2I MovablePosition { get; set; }
}
