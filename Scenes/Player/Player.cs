using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Sandbox.Common.Signals;

namespace Sandbox.Scenes.Player;

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

internal sealed partial class Player : CharacterBody2D, IMovable, ISelectable
{
	private List<Vector2I> path;

	public override void _Process(double delta)
	{
		if (path.Any())
		{
			MoveByPath();
		}
	}

	public void Move(Vector2I mapCoords)
	{
		Position = mapCoords;
	}
	
	private void MoveByPath()
	{
		var previousPosition = Position;
		Position = path[0];
		path.RemoveAt(0);
		EmitSignal(Signals.PLAYER_MOVED, new PlayerMoveSignal 
			{ CurrentPosition = previousPosition, PreviousPosition = Position});
	}

	public class PlayerMoveSignal : GodotObject
	{
		public Vector2I CurrentPosition { get; init; }
		public Vector2I PreviousPosition { get; init; }
	}
	
	public void SetPath(List<Vector2I> mapPath)
		=> path = mapPath;
	

	public void OnSelect(Action action)
	{
		throw new NotImplementedException();
	}

	public bool Selected { get; set; }
	public Vector2I Position { get; set; }
}
