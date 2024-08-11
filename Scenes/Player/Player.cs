using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Sandbox.AutoLoads;
using Sandbox.Common.Interfaces;

namespace Sandbox.Scenes.Player;

internal sealed partial class Player : Node2D, IMovable, ISelectable
{
	[Export] public bool Selected { get; set; }
	[Export] public Vector2I MapPosition { get; set; }
	[Export] public float Speed { get; set; } = 100.0F;
	
	private List<Vector2I> mapPath = new();
	private Tween MyTween { get; set; }

	private ICustomSignals _customSignals;

	public override void _Ready()
	{
		_customSignals = GetNode<ICustomSignals>("/root/CustomSignals");
		_customSignals.EmitRequestMove(MapPosition);
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
		
		_customSignals.EmitRequestMove(mapPath[0]);
	}

	private void TweenPosition(Vector2 nextPosition)
	{
		var duration = Position.DistanceTo(nextPosition) / Speed;
		ResolveAnimation(nextPosition);
		MyTween = CreateTween();
		MyTween.TweenProperty(this, "position", nextPosition, duration);
	}

	internal void Move(Vector2 position)
	{
		TweenPosition(position);
		MapPosition = mapPath[0];
		mapPath.RemoveAt(0);
		
		if (mapPath.Count == 0)
			OnSelect();
	}
	
	// TODO Move all this to IMovable
	private void ResolveAnimation(Vector2 nextMapPosition)
	{
		var animation = GetAnimation(nextMapPosition);
		animation();
	}
	
	private Action GetAnimation(Vector2 nextMapPosition)
	{
		var deltaX = nextMapPosition.X - MapPosition.X;
		var deltaY = nextMapPosition.Y - MapPosition.Y;

		return (deltaX, deltaY) switch
		{
			(0, < 0) => PlayWalkN,
			(0, > 0) => PlayWalkS,
			(< 0, 0) => PlayWalkW,
			(> 0, 0) => PlayWalkE,
			(> 0, < 0) => PlayWalkNE,
			(< 0, < 0) => PlayWalkNW,
			(> 0, > 0) => PlayWalkSE,
			(< 0, > 0) => PlayWalkSW,
			_ => () => {}
		};
	}

	public List<Vector2I> MapPath { get; set; }
	
	public void SetPath(List<Vector2I> path) => mapPath = path;

	private AnimatedSprite2D GetAnimatedSprite() => GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	
	private void PlayWalkSE() => GetAnimatedSprite().Animation = "WalkSESelected";
	private void PlayWalkSW() => GetAnimatedSprite().Animation = "WalkSWSelected";
	private void PlayWalkS() => GetAnimatedSprite().Animation = "WalkS";
	private void PlayWalkNE() => GetAnimatedSprite().Animation = "WalkNESelected";
	private void PlayWalkNW() => GetAnimatedSprite().Animation = "WalkNWSelected";
	private void PlayWalkW() => GetAnimatedSprite().Animation = "WalkWSelected";
	private void PlayWalkE() => GetAnimatedSprite().Animation = "WalkESelected";
	private void PlayWalkN() => GetAnimatedSprite().Animation = "WalkNSelected";

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
