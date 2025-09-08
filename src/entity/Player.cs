using System;
using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = 400;
	public int Health { get; set; } = 10;

	public Vector2 ScreenSize;
	public int cooldown = 0;
	public bool CanMove = true;
	public bool inOverworld = true;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback animState;

	[Export]
	public CollisionShape2D Collision;

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;

		SetCollisionLayerValue(1, true);
		SetCollisionMaskValue(2, true);

		// Add to player group for interaction system
		AddToGroup("player");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		animationTree.Active = true;

		animState = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!CanMove)
			return;

		if (cooldown > 0)
			cooldown--;

		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
			if (inOverworld) animState.Travel("move_right");
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
			if (inOverworld) animState.Travel("move_left");
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
			if (inOverworld) animState.Travel("move_down");
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
			if (inOverworld) animState.Travel("move_up");
		}

		if (inOverworld) animState.Travel("idle");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
		}

		Velocity = velocity;
		MoveAndSlide();

		// Get the current scene name safely
		string currentSceneName = GetTree().CurrentScene?.Name ?? "";

		// Clamp position to screen bounds
		if (currentSceneName != "TestBossScene" && currentSceneName != "TestBossTwo")
		{
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
				y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
			);
		}
		else
		{
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 32, 642.0f),
				y: Mathf.Clamp(Position.Y, 16, ScreenSize.Y - 16)
			);
		}

		if (this.Health <= 0)
			QueueFree();
	}

	public void HealthChange()
	{
		GD.Print("Change Health");
		Health -= 1;
	}
}
