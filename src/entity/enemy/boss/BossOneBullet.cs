using Godot;

public partial class BossOneBullet : Area2D
{
    [Export]
    public int Damage { get; set; } = 2;

    [Export]
    public string SpritePath;

    [Export]
    public Vector2 Velocity { get; set; }

    public Vector2 Viewport;
    public double Height;

    public Sprite2D sprite;
    private RectangleShape2D ColliderShape;
    private CollisionShape2D Collision;

    public override void _Ready()
    {
        Viewport = GetViewportRect().Size;
        sprite = new Sprite2D();
        sprite.Texture = (Texture2D)ResourceLoader.Load(SpritePath);
        AddChild(sprite);

        Collision = new CollisionShape2D();
        AddChild(Collision);

        ColliderShape = new RectangleShape2D();
        ColliderShape.Size = new Vector2(0.0f, 20.0f);
        Collision.Shape = ColliderShape;
        Height = Viewport.Y;

        // Connect the area entered signal
        AreaEntered += OnAreaEntered;
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

        Position += Velocity * (float)delta;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Player)
        {
            // Damage is handled in the Player script
            QueueFree();
        }
    }
}
