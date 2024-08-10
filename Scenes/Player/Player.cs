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
	[Export] public float Speed { get; set; } = 100.0F;
	
	private List<Vector2I> mapPath = new();
	private TileMapHandler tileMap;
	private Tween MyTween { get; set; }

	public override void _Ready()
	{
		tileMap = GetParent<TileMapHandler>();
		MapPosition = tileMap.LocalToMap(Position);
	}
	
	public override void _Process(double delta)
	{
		if (mapPath.Any())
			MoveByPath(); 
	}
	
	private void MoveByPath()
	{
		if (MyTween is not null && MyTween.IsRunning())
			return;
		
		var nextPosition = tileMap.MapToLocal(mapPath[0]);

		TweenPosition(nextPosition);

		MapPosition = mapPath[0];
		mapPath.RemoveAt(0);
		
		if (mapPath.Count == 0)
			OnSelect();
	}

	private void TweenPosition(Vector2 nextPosition)
	{
		var duration = Position.DistanceTo(nextPosition) / Speed;
		ResolveAnimation(nextPosition);
		var tween = CreateTween();
		MyTween = tween;
		tween.TweenProperty(this, "position", nextPosition, duration);
	}

	private void ResolveAnimation(Vector2 nextMapPosition)
	{
		if (nextMapPosition.Y < MapPosition.Y && nextMapPosition.X == MapPosition.X)
		{
			PlayWalkN();
		}
		
		if (nextMapPosition.Y > MapPosition.Y && nextMapPosition.X == MapPosition.X)
		{
			PlayWalkS();
		}
		
		if (nextMapPosition.Y == MapPosition.Y && nextMapPosition.X < MapPosition.X)
		{
			PlayWalkW();
		}
		
		if (nextMapPosition.Y == MapPosition.Y && nextMapPosition.X > MapPosition.X)
		{
			PlayWalkE();
		}
		
		if (nextMapPosition.Y < MapPosition.Y && nextMapPosition.X > MapPosition.X)
		{
			PlayWalkNE();
		}
		
		if (nextMapPosition.Y < MapPosition.Y && nextMapPosition.X < MapPosition.X)
		{
			PlayWalkNW();
		}
		
		if (nextMapPosition.Y > MapPosition.Y && nextMapPosition.X > MapPosition.X)
		{
			PlayWalkSE();
		}
		
		if (nextMapPosition.Y < MapPosition.Y && nextMapPosition.X < MapPosition.X)
		{
			PlayWalkSW();
		}
	}
	
	public void SetPath(List<Vector2I> path) => mapPath = path;

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
