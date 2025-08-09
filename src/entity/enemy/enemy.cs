using Godot;
using System;

public partial class Enemy : BaseEntity
{
    private Action<double, double, double, double, double> BulletGenerator;

    [Export]
    private double deltaAccumulate;

    [Export]
    private double GenerateInterval;

    [Export]
    private double GenerateSpaceInterval;

    [Export]
    private double Radius;

    [Export]
    private double BulletSpeed;

    public override void _Ready()
    {
        Velocity = new Vector2(3, 4);

        BulletGenerator = (double Radius, double GenerateSpaceInterval, double x, double y, double BulletSpeed) =>
        {
            double spaceAccumulate = 0.0;
            GD.Print("Callback Working!");

            while (spaceAccumulate <= 360.0)
            {
                var bullet = new Bullet();
                //bullet.Texture = GD.Load<Texture2D>("assets/art/test/asteroid.png");
                bullet.Position = new Vector2((float)Math.Cos((float)spaceAccumulate) * (float)Radius, (float)Math.Sin((float)spaceAccumulate) * (float)Radius);
                bullet.Velocity = new Vector2((float)((bullet.Position.X - x) * BulletSpeed), (float)((bullet.Position.Y - y) * BulletSpeed));
                AddChild(bullet);
                spaceAccumulate += GenerateSpaceInterval;
            }
        };


    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
        //var collide = MoveAndCollide(Velocity * (float)delta);

        //if (collide != null)
        //{
        //if (collide.GetCollider() is PlayerBullet)
        //{
        //    Health -= collide.GetCollider().Damage;
        //}
        //}
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
        //        if(body is PlayerBullet bullet)
        //        {
        //            Health -= bullet.Damage;
        //        }
    }
}
