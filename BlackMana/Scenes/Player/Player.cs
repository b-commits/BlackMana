using System;
using System.Collections.Generic;
using Godot;
using BlackMana.AutoLoads;
using BlackMana.Common.Interfaces;

namespace BlackMana.Scenes.Player;

internal sealed partial class Player
	: Node2D, IMovable, ISelectable
{
	[Export] public bool Selected { get; set; }
	[Export] public Vector2I MapPosition { get; set; }
	[Export] public float Speed { get; set; } = 100.0F;
	
	public List<Vector2I> MapPath { get; set; }
	private Tween MyTween { get; set; }

	private ICustomSignals _customSignals;

	public override void _Ready()
	{
		_customSignals = GetNode<ICustomSignals>(CustomSignals.ScenePath);
		_customSignals.EmitRequestMove(new RequestMoveEvent { CurrentMapPosition = MapPosition });
}
	
	public override void _Process(double delta)
	{
		if (MapPath is not null && MapPath.Count != 0)
			MoveByPath(); 
	}
	
	public void MoveByPath()
	{
		if (MyTween is not null && MyTween.IsRunning())
			return;
		
		var requestMoveEvent = new RequestMoveEvent { CurrentMapPosition = MapPosition, NextMapPosition = MapPath[0] };
		_customSignals.EmitRequestMove(requestMoveEvent);
	}

	public void TweenPosition(Vector2 nextPosition)
	{
		var duration = Position.DistanceTo(nextPosition) / Speed;
		ResolveAnimation(nextPosition);
		MyTween = CreateTween();
		MyTween.TweenProperty(this, "position", nextPosition, duration);
	}

	public void Move(Vector2 position)
	{
		MapPosition = MapPath[0];
		TweenPosition(position);
		MapPath.RemoveAt(0);

		if (MapPath.Count == 0)
			OnSelect();
	}
	
	public void ResolveAnimation(Vector2 nextMapPosition)
	{
		var animation = GetAnimation(nextMapPosition);
		animation();
	}
	
	public Action GetAnimation(Vector2 nextMapPosition)
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
	
	public void SetPath(List<Vector2I> path) => MapPath = path;

	private AnimatedSprite2D GetAnimatedSprite() => GetNode<AnimatedSprite2D>(nameof(AnimatedSprite2D));

	private void PlayWalkSE() => GetAnimatedSprite().Animation = PlayerAnimations.WalkSESelected;
	private void PlayWalkSW() => GetAnimatedSprite().Animation = PlayerAnimations.WalkSWSelected;
	private void PlayWalkS() => GetAnimatedSprite().Animation = PlayerAnimations.WalkSSelected;
	private void PlayWalkNE() => GetAnimatedSprite().Animation = PlayerAnimations.WalkNESelected;
	private void PlayWalkNW() => GetAnimatedSprite().Animation = PlayerAnimations.WalkNWSelected;
	private void PlayWalkW() => GetAnimatedSprite().Animation = PlayerAnimations.WalkWSelected;
	private void PlayWalkE() => GetAnimatedSprite().Animation = PlayerAnimations.WalkESelected;
	private void PlayWalkN() => GetAnimatedSprite().Animation = PlayerAnimations.WalkNSelected;
	public void OnSelect() => GetAnimatedSprite().Animation = PlayerAnimations.IdleSelectedFrame;
	public void OnDeselect() => GetAnimatedSprite().Animation = PlayerAnimations.IdleFrame;
}
