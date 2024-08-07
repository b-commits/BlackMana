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
	
	private Vector2 CartesianToIsometric(Vector2 vector)
	{
		return new Vector2(vector.X - vector.Y, (vector.X + vector.Y) / 2);
	}

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
		
		var tween = CreateTween();
		MyTween = tween;
		tween.TweenProperty(this, "position", nextPosition, duration);

		MapPosition = mapPath[0];
		mapPath.RemoveAt(0);
	}
	
	public void SetPath(List<Vector2I> path)
		=> mapPath = path;
	

	public void OnSelect()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "SelectedFrame";
	}

	public void OnDeselect()
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "DeselectedFrame";
	}
}
