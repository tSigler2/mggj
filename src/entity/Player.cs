using System;
using Godot;

public partial class Player : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 400;
    public int Health { get; set; } = 10;

    public Vector2 ScreenSize;
    public int cooldown = 0;

    [Export]
    public CollisionShape2D Collision;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;

        SetCollisionLayerValue(1, true);
        SetCollisionMaskValue(2, true);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (cooldown > 0)
            cooldown--;

        var velocity = Vector2.Zero;

        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 1;
        }

        if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= 1;
        }

        if (Input.IsActionPressed("move_down"))
        {
            velocity.Y += 1;
        }

        if (Input.IsActionPressed("move_up"))
        {
            velocity.Y -= 1;
        }

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
        }

        Velocity = velocity;
        MoveAndSlide();

        // Clamp position to screen bounds
        var currentScene = GetTree().CurrentScene;
        if (currentScene.Name != "TestBossScene" && currentScene.Name != "TestBossTwo")
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