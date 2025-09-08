using System;
using System.Collections.Generic;
using Godot;

public partial class BossThree : Sprite2D
{
    private Random rng = new Random();

    [Export]
    private Player p;

    private float xPos;

    [Export]
    public double BulletSpeed;

    [Export]
    public double BulletRate;

    private Vector2 Viewport;
    private int SideChoice = 1;

    [Export]
    private int stage = 0;
    private double stageTimer;
    private double deltaAccumulate;

    private Action<Random, double, int>[] BulletPatterns = new Action<Random, double, int>[6];

    private int randomNum = 0;
    private int lastPattern = -1;

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
            Vector2 center = new Vector2(650 / 2, Viewport.Y / 2);
            var random = new RandomNumberGenerator();

            do
            {
                randomNum = random.RandiRange(1, 4);
            } while (randomNum == lastPattern);

            lastPattern = randomNum;

            float radius = 300f;
            int ringShiftDir = rng.Next(0, 2) == 0 ? -1 : 1;

            for (float angle = 0; angle < 360; angle += 5f)
            {
                float normalizedAngle = angle % 360;

                if (
                    randomNum == 1
                    && (
                        (normalizedAngle >= 270 && normalizedAngle <= 360)
                        || (normalizedAngle >= 0 && normalizedAngle <= 90)
                    )
                )
                    continue; // Remove Right Portion
                if (randomNum == 2 && (normalizedAngle >= 180 && normalizedAngle <= 360))
                    continue; // Remove Bottom Portion
                if (randomNum == 3 && (normalizedAngle >= 90 && normalizedAngle <= 270))
                    continue; // Remove Left Portion
                if (randomNum == 4 && (normalizedAngle >= 0 && normalizedAngle <= 180))
                    continue; // Remove Top Portion

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

                bullet.bulletSpeed = BulletSpeed;

                GetTree().CurrentScene.AddChild(bullet);

                bullet.ShiftDirection = ringShiftDir;
            }
        };

        BulletPatterns[3] = (Random rng, double speed, int d) =>
        {
            var bullet = new BossOneBullet();
            GetTree().CurrentScene.AddChild(bullet);
            bullet.Position = new Vector2((float)(rng.NextDouble() * 650), Viewport.Y - 50);
            bullet.Velocity = new Vector2(0.0f, (float)-speed);
            bullet.p = p;
            bullet.Show();
        };

        BulletPatterns[4] = (Random rng, double speed, int d) =>
        {
            var bullet = new BossOneBullet();
            bullet.Position = new Vector2((float)(rng.NextDouble() * 650), Viewport.Y);

            double ArchTanAngle = Mathf.Atan2(
                p.Position.Y - bullet.Position.Y,
                p.Position.X - bullet.Position.X
            );

            bullet.Velocity = new Vector2(
                (float)(Mathf.Cos(ArchTanAngle) * speed),
                (float)(Mathf.Sin(ArchTanAngle) * speed)
            );
            bullet.p = p;
            GetTree().CurrentScene.AddChild(bullet);
        };

        BulletPatterns[5] = (Random rng, double speed, int d) =>
        {
            float EndAngle = (float)rng.NextDouble() * 360.0f;
            float AngleCurrent = (float)rng.NextDouble() * EndAngle;

            while (AngleCurrent <= EndAngle)
            {
                var bullet = new BossOneBullet();
                bullet.Position = new Vector2(650 / 2, Viewport.Y / 2);
                bullet.Velocity = new Vector2(
                    (float)(Mathf.Cos(AngleCurrent) * speed),
                    (float)(Mathf.Sin(AngleCurrent) * speed)
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
        stageTimer += delta;

        if (stageTimer >= 20.0)
        {
            stage++;
            stageTimer = 0.0;

            if (stage >= BulletPatterns.Length)
            {
                SceneManager.Instance.ChangeScene("res://scenes/win_splash.tscn");
                QueueFree();
            }
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
