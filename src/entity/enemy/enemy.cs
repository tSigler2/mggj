using Godot;
using System;

public partial class Enemy : Area2D
{
    private Action<double, double, double, double, double> BulletGenerator;
    private double deltaAccumulate;

    [Export]
    private double GenerateInterval;

    [Export]
    private double GenerateSpaceInterval;

    [Export]
    public int Health;

    [Export]
    private double Radius;

    [Export]
    private double BulletSpeed;

    [Export]
    public double x;

    [Export]
    public double y;

    [Export]
    public double dx;

    [Export]
    public double dy;


    public override void _Ready()
    {
        BulletGenerator = (double Radius, double GenerateSpaceInterval, double x, double y, double BulletSpeed) =>
        {
            double spaceAccumulate = 0.0;

            while (spaceAccumulate <= 360.0)
            {
                var bullet = new Bullet();
                bullet.Texture = GD.Load<Texture2D>("assets/art");
                bullet.Position = new Vector2((float)Math.Cos((float)spaceAccumulate) * (float)Radius, (float)Math.Sin((float)spaceAccumulate) * (float)Radius);
                bullet.Velocity = new Vector2((float)((bullet.Position.X - x) * BulletSpeed), (float)((bullet.Position.Y - y) * BulletSpeed));
                AddChild(bullet);
                spaceAccumulate += GenerateSpaceInterval;
            }
        };


    }

    public override void _Process(double delta)
    {
        deltaAccumulate += delta;

        if (deltaAccumulate >= GenerateInterval)
        {
            BulletGenerator(Radius, GenerateSpaceInterval, x, y, BulletSpeed);
            deltaAccumulate = 0.0;
        }

        x += dx * (float)delta;
        y += dy * (float)delta;

        if (Health <= 0)
        {
            QueueFree();
        }
    }

    private void OnBodyEntered(Node body)
    {
        //        if(body is PlayerBullet bullet)
        //        {
        //            Health -= bullet.Damage;
        //        }
    }
}
