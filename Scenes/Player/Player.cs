using System.Collections.Generic;
using System.Linq;
using Godot;
using Sandbox.Common.Interfaces;
using Sandbox.Scenes.TileMap;

namespace Sandbox.Scenes.Player;

internal sealed partial class Player : Node2D, IMovable, ISelectable
{
	[Export] public bool Selected { get; set; }
	[Export] public Vector2I MapPosition { get; set; }
	[Export] public float Speed { get; set; } = 50.0F;
	
	private List<Vector2I> mapPath = new();
	private TileMapHandler tileMap;
	private Tween MyTween { get; set; }
	
	private static Vector2 CartesianToIsometric(Vector2 vector) 
		=> new(vector.X - vector.Y, (vector.X + vector.Y) / 2);

	public override void _Ready()
	{
		tileMap = GetParent<TileMapHandler>();
		MapPosition = tileMap.LocalToMap(Position);
	}
	
	public override void _Process(double delta)
	{
		if (mapPath.Any())
		{
			MoveByPath(); 
		}
	}

	public void Move(Vector2I mapCoords)
	{
		MapPosition = mapCoords;
		Position = tileMap.MapToLocal(mapCoords);
	}
	
	private void MoveByPath()
	{
		if (MyTween is not null && MyTween.IsRunning())
			return;
		
		var nextPosition = tileMap.MapToLocal(mapPath[0]);
		var duration = Position.DistanceTo(nextPosition) / Speed;

		if (nextPosition.Y < Position.Y && nextPosition.X == Position.X)
		{
			PlayWalkN();
		}
		
		if (nextPosition.Y > Position.Y && nextPosition.X == Position.X)
		{
			PlayWalkS();
		}
		
		if (nextPosition.Y == Position.Y && nextPosition.X < Position.X)
		{
			PlayWalkW();
		}
		
		if (nextPosition.Y == Position.Y && nextPosition.X > Position.X)
		{
			PlayWalkE();
		}
		
		if (nextPosition.Y < Position.Y && nextPosition.X > Position.X)
		{
			PlayWalkNE();
		}
		
		if (nextPosition.Y < Position.Y && nextPosition.X < Position.X)
		{
			PlayWalkNW();
		}
		
		if (nextPosition.Y > Position.Y && nextPosition.X > Position.X)
		{
			PlayWalkSE();
		}
		
		if (nextPosition.Y < Position.Y && nextPosition.X < Position.X)
		{
			PlayWalkSW();
		}
		
		var tween = CreateTween();
		MyTween = tween;
		tween.TweenProperty(this, "position", nextPosition, duration);

		MapPosition = mapPath[0];
		mapPath.RemoveAt(0);

		if (mapPath.Count == 0)
		{
			OnSelect();
		}
	}
	
	public void SetPath(List<Vector2I> path)
		=> mapPath = path;

	private void PlayWalkSE()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "WalkSESelected";
	}

	private void PlayWalkSW()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "WalkSWSelected";
	}

	private void PlayWalkS()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "WalkSSelected";
	}

	private void PlayWalkNE()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "WalkNESelected";
	}

	private void PlayWalkNW()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "WalkNWSelected";
	}
	
	private void PlayWalkW()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "WalkWSelected";
	}
	
	private void PlayWalkE()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "WalkESelected";
	}

	private void PlayWalkN()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "WalkNSelected";
	}

	public void OnSelect()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "IdleSelectedFrame";
	}

	public void OnDeselect()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "IdleFrame";
	}
}
