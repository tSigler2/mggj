using Godot;

public partial class BossOneBullet : BaseEntity
{
    [Export]
    public int Damage;

    [Export]
    public string SpritePath;

    public double Height;

    public Sprite2D sprite;
    private RectangleShape2D ColliderShape;
    private CollisionShape2D Collision;

    public override void _Ready()
    {
        sprite = new Sprite2D();
        sprite.Texture = (Texture2D)ResourceLoader.Load(SpritePath);
        AddChild(sprite);

        Collision = new CollisionShape2D();

        ColliderShape = new RectangleShape2D();
        ColliderShape.Size = new Vector2(0.0f, 20.0f);
        Collision.Shape = ColliderShape;
        Height = Viewport.Y;
        Show();
    }

    public override void _Process(double delta)
    {
        if (
            Position.X <= 0
            || Position.Y <= 0
            || Position.X >= Viewport.X
            || Position.Y >= Viewport.Y
        )
        {
            QueueFree();
            return;
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
