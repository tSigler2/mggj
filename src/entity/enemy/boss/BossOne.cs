using System;
using Godot;

public partial class BossOne : Enemy
{
    private Random rng;

    [Export]
    private double LongInterval = 360.0;

    [Export]
    private double ShortInterval = 60.0;

    [Export]
    private bool LongIntervalActive = true;

    [Export]
    private double BulletSpeed = 20.0;

    [Export]
    private int BulletCount = 7;

    private float xPos;

    public override void _Ready()
    {
        base._Ready();
        rng = new Random();

        BulletGenerator = (Random rng, double bulletSpeed) =>
        {
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
            (LongIntervalActive && deltaAccumulate >= LongInterval)
            || (!LongIntervalActive && deltaAccumulate >= ShortInterval)
        )
        {
            if (LongIntervalActive && deltaAccumulate >= LongInterval)
            {
                xPos = (float)rng.NextDouble() * Viewport.X;
            }
            BulletGenerator(rng, BulletSpeed);

            if (BulletCount == 7)
            {
                LongIntervalActive = true;
            }
            deltaAccumulate = 0.0;
        }
    }
}
