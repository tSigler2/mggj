using System;
using Godot;

public partial class BossOne : Enemy
{
    private Random rng;

    [Export]
    private double LongInterval = 360;

    [Export]
    private bool longIntervalActive = true;

    [Export]
    private double longIntervalCount = 0;

    [Export]
    private double BulletSpeed = 20;

    public override void _Ready()
    {
        base._Ready();
        rng = new Random();

        BulletGenerator = (Random rng, double bulletSpeed) =>
        {
            float xPos = (float)rng.NextDouble() * Viewport.X;
            var bullet = new Bullet();
            bullet.Position = new Vector2(xPos, Viewport.Y);
            bullet.Velocity = new Vector2(0.0f, (float)-bulletSpeed);
        };
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        deltaAccumulate += delta;

        if (
            deltaAccumulate >= GenerateInterval
            && (deltaAccumulate >= LongInterval || !longIntervalActive)
        )
        {
            if (longIntervalActive)
                longIntervalActive = false;
            BulletGenerator(rng, BulletSpeed);
            longIntervalCount++;

            if (longIntervalCount == 10)
            {
                longIntervalCount = 0;
                longIntervalActive = true;
            }
        }
    }
}
