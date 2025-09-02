using Godot;

public partial class Bullet : BaseEntity
{
    [Export]
    private int Damage;

    [Export]
    public float Height;

    [Export]
    public float HeightDelta = 1.0f;

    [Export]
    public string SpritePath = "";

    public Sprite2D sprite;
    private RectangleShape2D ColliderShape;
    private CollisionShape2D Collision;
    public Vector2 Velocity;

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

        Height -= HeightDelta;
        Position -= Velocity * (float)delta;

        ColliderShape.Size = new Vector2(ColliderShape.Size.X, Height);

        sprite.Position = Position;
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
