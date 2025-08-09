using Godot;

public partial class Bullet : BaseEntity
{
    [Export]
    private int Damage;

    public override void _Process(double delta)
    {
        Position += Velocity * (float)delta;
    }

    public override void _PhysicsProcess(double delta)
    {
        var collide = MoveAndCollide(Velocity * (float)delta);

        if (collide != null)
        {
            if (collide.GetCollider().HasMethod("AlterHealth"))
            {
                collide.GetCollider().Call("AlterHealth", Damage);
            }
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        //        if(body is Player p)
        //        {
        //            p.Health -= Damage;
        //            QueueFree();
        //        }
    }

    private void OnVisibilityNotifier2DScreenExited()
    {
        QueueFree();
    }
}
