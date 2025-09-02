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

    [Export]
    private int stage = 0;
    private double deltaAccumulate;

    private Action<Random, double, int>[] BulletPatterns = new Action<Random, double, int>[4];

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
                bullet.vChange *= -1;
            }

            bullet.p = p;
            bullet.bulletSpeed = BulletSpeed;
            bullet.Velocity = new Vector2(0.0f, (float)BulletSpeed);
            bullet.Viewport = Viewport;
            GetTree().CurrentScene.AddChild(bullet);
        };

        BulletPatterns[1] = (Random rng, double speed, int d) =>
        {
            var bullet = new BossTwoBulletTwo();

            double StartPlace = rng.NextDouble();
            if (StartPlace < 0.25)
                bullet.Position = new Vector2((float)(rng.NextDouble() * Viewport.X), 0.0f);
            else if (StartPlace < 0.5)
                bullet.Position = new Vector2((float)(rng.NextDouble() * Viewport.X), Viewport.Y);
            else if (StartPlace < 0.75)
                bullet.Position = new Vector2(0.0f, (float)(rng.NextDouble() * Viewport.Y));
            else if (StartPlace < 1.0)
                bullet.Position = new Vector2(Viewport.X, (float)(rng.NextDouble() * Viewport.Y));
            GD.Print(bullet.Position);

            double ArchTanAngle = Mathf.Atan2(
                p.Position.Y - bullet.Position.Y,
                p.Position.X - bullet.Position.X
            );

            bullet.Velocity = new Vector2(
                (float)(Mathf.Cos(ArchTanAngle) * BulletSpeed),
                (float)(Mathf.Sin(ArchTanAngle) * BulletSpeed)
            );

            bullet.Viewport = Viewport;
            bullet.bulletSpeed = BulletSpeed;
            bullet.p = p;
            GetTree().CurrentScene.AddChild(bullet);
        };

        BulletPatterns[2] = (Random rng, double speed, int d) =>
        {
            var bullet = new BossTwoBulletOne();
            GetTree().CurrentScene.AddChild(bullet);
        };

        BulletPatterns[3] = (Random rng, double speed, int d) =>
        {
            Vector2 center = new Vector2(650 / 2, Viewport.Y / 2);

            float radius = 300f;

            for (float angle = 0; angle < 360; angle += 5f)
            {
                float normalizedAngle = angle % 360;

                if (normalizedAngle >= 270 || normalizedAngle <= 90)
                    continue;

                var bullet = new BossTwoBulletThree();

                Vector2 spawnPos = new Vector2(
                    center.X + Mathf.Cos(Mathf.DegToRad(angle)) * radius,
                    center.Y + Mathf.Sin(Mathf.DegToRad(angle)) * radius
                );

                bullet.Position = spawnPos;

                Vector2 dirToCenter = (center - spawnPos).Normalized();
                bullet.Velocity = dirToCenter * (float)speed;

                bullet.Target = center;

                bullet.p = p;

                GetTree().CurrentScene.AddChild(bullet);
            }
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
