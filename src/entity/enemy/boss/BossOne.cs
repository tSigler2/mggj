using System;
using System.Collections.Generic;
using Godot;

public partial class BossOne : Sprite2D
{
    private Random rng;

    private Action<Random, double> BulletGenerator;

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

    [Export]
    private Player p;

    private float xPos;
    private int PatternCount = 0;
    private double GlobalDeltaAccumulate;
    private double deltaAccumulate;

    private Vector2 Viewport;

    private Action<Random, double>[] BulletPatterns = new Action<Random, double>[3];

    public override void _Ready()
    {
        Viewport = GetViewportRect().Size;
        rng = new Random();
        GlobalDeltaAccumulate = 0;

        BulletPatterns[0] = (Random rng, double bulletSpeed) =>
        {
            var bullet = new Bullet();
            bullet.Position = new Vector2((float)(rng.NextDouble() * Viewport.X), Viewport.Y);
            bullet.Velocity = new Vector2(0.0f, (float)-bulletSpeed);
        };

        BulletPatterns[1] = (Random rng, double bulletSpeed) =>
        {
            var bullet = new Bullet();
            bullet.Position = new Vector2(xPos, Viewport.Y);

            double ArchTanAngle = Mathf.Atan2(p.Position.Y - Position.Y, p.Position.X - Position.X);

            bullet.Velocity = new Vector2(
                (float)(Mathf.Cos(ArchTanAngle) * bulletSpeed),
                (float)(Mathf.Sin(ArchTanAngle) * bulletSpeed)
            );
        };

        BulletPatterns[2] = (Random rng, double bulletSpeed) =>
        {
            float AngleDelta = 20.0f;
            float AngleCurrent = 0.0f;

            while (AngleCurrent <= 360.0f)
            {
                var bullet = new BossOneBullet();
                bullet.Position = new Vector2(Position.X, Position.Y);

                AngleCurrent += 20.0f;
            }
        };

        BulletGenerator = BulletPatterns[0];
    }

    public override void _Process(double delta)
    {
        GD.Print("Started Process");
        deltaAccumulate += delta;
        GlobalDeltaAccumulate += delta;
        GD.Print(delta);
        GD.Print(GlobalDeltaAccumulate);

        if (GlobalDeltaAccumulate >= 60.0 && GlobalDeltaAccumulate < 120.0 && PatternCount == 0)
        {
            PatternCount++;
        }
        else if (
            GlobalDeltaAccumulate >= 120.0
            && GlobalDeltaAccumulate < 180.0
            && PatternCount == 1
        )
        {
            PatternCount++;
        }

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
        GD.Print("Ended Process");
    }
}
