using System;
using Godot;

public partial class Player : Area2D
{
    [Export]
    public int Speed { get; set; } = 400;
    public int Health { get; set; } = 10;

    public Vector2 ScreenSize;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;

        // Connect the area entered signal for bullet collisions
        AreaEntered += OnAreaEntered;
    }

    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero; // The player's movement vector.
        GD.Print("Running Process");

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

        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            animatedSprite2D.Play();
        }
        else
        {
            animatedSprite2D.Stop();
        }

        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area.GetParent() is Player player)
        {
            // This shouldn't happen normally
            GD.Print("Player collided with itself?");
            return;
        }

        // Check if the area is a bullet
        if (area is Bullet bullet)
        {
            Health -= bullet.Damage;
            GD.Print($"Player hit! Health: {Health}");
            
            if (Health <= 0)
            {
                GD.Print("Player died!");
                // Handle player death
            }
        }
        else if (area is BossOneBullet bossBullet)
        {
            Health -= bossBullet.Damage;
            GD.Print($"Player hit by boss bullet! Health: {Health}");
            
            if (Health <= 0)
            {
                GD.Print("Player died!");
                // Handle player death
            }
        }
    }
}
