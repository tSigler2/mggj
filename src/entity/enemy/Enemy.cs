using System;
using Godot;

public partial class Enemy : BaseEntity
{
    protected Action<Random, double> BulletGenerator;

    [Export]
    protected double deltaAccumulate;

    [Export]
    public double GenerateInterval;

    [Export]
    protected double GenerateSpaceInterval;

    [Export]
    private double Radius;

    [Export]
    private double BulletSpeed;

    [Export]
    private int Damage;

    private Vector2 Velocity;

    public override void _Ready()
    {
        base._Ready();
        Velocity = new Vector2(3, 4);
    }

    public override void _PhysicsProcess(double delta)
    {
        //MoveAndSlide();
        //var collide = MoveAndCollide(Velocity * (float)delta);
        /*
                if (collide != null)
                {
                    if (collide.GetCollider() is Player) { }
                }
                */
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player p)
        {
            p.Health -= Damage;
        }
    }
}
