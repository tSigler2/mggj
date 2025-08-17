using System;
using Godot;

public partial class Enemy : BaseEntity
{
    protected Action<double, double, double, double, double> BulletGenerator;

    [Export]
    private double deltaAccumulate;

    [Export]
    private double GenerateInterval;

    [Export]
    protected double GenerateSpaceInterval;

    [Export]
    private double Radius;

    [Export]
    private double BulletSpeed;

    [Export]
    private int Damage;

    public override void _Ready()
    {
        Velocity = new Vector2(3, 4);

        BulletGenerator = (
            double Radius,
            double GenerateSpaceInterval,
            double x,
            double y,
            double BulletSpeed
        ) =>
        {
            double spaceAccumulate = 0.0;
            while (spaceAccumulate < 360.0)
            {
                var bullet = new Bullet();
                bullet.Position = new Vector2(
                    (float)Mathf.Cos(spaceAccumulate * Math.PI / 180) * (float)Radius,
                    (float)Mathf.Sin(spaceAccumulate * Math.PI / 180) * (float)Radius
                );
                bullet.Velocity = new Vector2(
                    (float)(Mathf.Cos(spaceAccumulate * Math.PI / 180) * BulletSpeed),
                    (float)(Mathf.Sin(spaceAccumulate * Math.PI / 180) * BulletSpeed)
                );
                AddChild(bullet);
                spaceAccumulate += GenerateSpaceInterval;
            }
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
        var collide = MoveAndCollide(Velocity * (float)delta);

        if (collide != null)
        {
            if (collide.GetCollider() is Player) { }
        }
    }

    public override void _Process(double delta)
    {
        deltaAccumulate += delta;

        if (deltaAccumulate >= GenerateInterval)
        {
            BulletGenerator(Radius, GenerateSpaceInterval, Position.X, Position.Y, BulletSpeed);
            deltaAccumulate = 0.0;
        }

        if (Health <= 0)
        {
            QueueFree();
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player p)
        {
            p.Health -= Damage;
        }
    }
}
