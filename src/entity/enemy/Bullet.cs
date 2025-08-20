using Godot;

public partial class Bullet : BaseEntity
{
    [Export]
    private int Damage;

    public Sprite2D sprite;
    private CollisionShape2D Collider;

    public override void _Ready()
    {
        sprite = new Sprite2D();
        sprite.Texture = (Texture2D)ResourceLoader.Load("assets/art/test/asteroid.png");
        AddChild(sprite);

        Collider = new CollisionShape2D();
        Collider.Shape = new RectangleShape2D();
        AddChild(Collider);
        Show();
    }

    public override void _Process(double delta)
    {
        Position += Velocity * (float)delta;
        //Collider.Shape = (Vector2)sprite.Texture.GetSize();
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
        if (body is Player p)
        {
            p.Health -= Damage;
            QueueFree();
        }
    }

    private void OnVisibilityNotifier2DScreenExited()
    {
        QueueFree();
    }
}
