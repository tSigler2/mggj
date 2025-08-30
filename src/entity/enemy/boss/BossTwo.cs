using System;
using System.Collections.Generic;
using Godot;

public partial class BossTwo : Sprite2D
{
    private Random rng = new Random();

    [Export]
    private Player p;

    [Export]
    public double BulletSpeed;

    [Export]
    public double BulletRate;

    private Vector2 Viewport;
    private int SideChoice = 1;

    private int stage = 0;
    private double deltaAccumulate;

    private Action<Random, double, int>[] BulletPatterns = new Action<Random, double, int>[3];

    public override void _Ready()
    {
        Viewport = GetViewportRect().Size;

        BulletPatterns[0] = (Random rng, double speed, int d) =>
        {
            var bullet = new BossTwoBulletOne();

            if (d == 1)
            {
                bullet.Position = new Vector2(0.0f, 0.0f);
                bullet.Direction = true;
            }
            else if (d == -1)
            {
                bullet.Position = new Vector2(Viewport.X, 0.0f);
                bullet.Direction = false;
            }

            bullet.p = p;
            bullet.Velocity = new Vector2(0.0f, (float)BulletSpeed);
            GetTree().CurrentScene.AddChild(bullet);
        };

        BulletPatterns[1] = (Random rng, double speed, int d) =>
        {
            var bullet = new BossTwoBulletOne();
            GetTree().CurrentScene.AddChild(bullet);
        };

        BulletPatterns[2] = (Random rng, double speed, int d) =>
        {
            var bullet = new BossTwoBulletOne();
            GetTree().CurrentScene.AddChild(bullet);
        };
    }

    public override void _Process(double delta)
    {
        deltaAccumulate += delta;
        if (stage == 0 && deltaAccumulate >= 60.0)
        {
            stage++;
        }
        else if (stage == 1 && deltaAccumulate >= 180.0)
        {
            stage++;
        }
        else if (deltaAccumulate >= 300.0)
        {
            QueueFree();
        }

        if (deltaAccumulate >= BulletRate)
        {
            BulletPatterns[stage](rng, BulletSpeed, SideChoice);
            deltaAccumulate = 0.0;
            if (stage == 0)
                SideChoice *= -1;
        }
    }
}
