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

    public override void _Process(double delta)
    {
        if (cooldown > 0)
            cooldown--;
        var velocity = Vector2.Zero; // The player's movement vector.

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

        //var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            //animatedSprite2D.Play();
        }
        else
        {
            //animatedSprite2D.Stop();
        }

        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );
        if (this.Health <= 0)
        {
            QueueFree();
        }
    }

    public void HealthChange()
    {
        GD.Print("Change Health");
        Health -= 1;
    }
}
