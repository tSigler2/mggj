using System;
using System.Collections.Generic;
using Godot;

public partial class BossOne : Sprite2D
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

    [Export]
    private Player p;

    private float xPos;

    [Export]
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
            var bullet = new BossOneBullet();
            GetTree().CurrentScene.AddChild(bullet);
            bullet.Position = new Vector2((float)(rng.NextDouble() * Viewport.X), Viewport.Y - 50);
            bullet.Velocity = new Vector2(0.0f, (float)-bulletSpeed);
            bullet.p = p;
            bullet.Show();
        };

        BulletPatterns[1] = (Random rng, double bulletSpeed) =>
        {
            var bullet = new BossOneBullet();
            bullet.Position = new Vector2((float)(rng.NextDouble() * Viewport.X), Viewport.Y);

            double ArchTanAngle = Mathf.Atan2(
                p.Position.Y - bullet.Position.Y,
                p.Position.X - bullet.Position.X
            );

            bullet.Velocity = new Vector2(
                (float)(Mathf.Cos(ArchTanAngle) * bulletSpeed),
                (float)(Mathf.Sin(ArchTanAngle) * bulletSpeed)
            );
            bullet.p = p;
            GetTree().CurrentScene.AddChild(bullet);
        };

        BulletPatterns[2] = (Random rng, double bulletSpeed) =>
        {
            float EndAngle = (float)rng.NextDouble() * 360.0f;
            float AngleCurrent = (float)rng.NextDouble() * EndAngle;

            while (AngleCurrent <= EndAngle)
            {
                var bullet = new BossOneBullet();
                bullet.Position = new Vector2(Viewport.X / 2, Viewport.Y / 2);
                bullet.Velocity = new Vector2(
                    (float)(Mathf.Cos(AngleCurrent) * bulletSpeed),
                    (float)(Mathf.Sin(AngleCurrent) * bulletSpeed)
                );

                AngleCurrent += 20.0f;
                bullet.p = p;
                GetTree().CurrentScene.AddChild(bullet);
            }
        };
    }

    public override void _Process(double delta)
    {
        deltaAccumulate += delta;
        GlobalDeltaAccumulate += delta;

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
            BulletPatterns[PatternCount](rng, BulletSpeed);

            if (BulletCount == 7)
            {
                LongIntervalActive = true;
            }
            else
            {
                LongIntervalActive = false;
            }
            deltaAccumulate = 0.0;
        }
    }
}
