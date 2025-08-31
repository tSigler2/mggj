using Godot;

public partial class BossOneBullet : BaseEntity
{
    [Export]
    public int Damage;

    [Export]
    public string SpritePath = "./assets/art/projectile/AliceBullet.png";

    public double Height;

    public Sprite2D sprite;
    private CollisionShape2D Collision;
    public Player p;
    public Vector2 Velocity;

    public override void _Ready()
    {
        sprite = new Sprite2D();
        sprite.Texture = (Texture2D)ResourceLoader.Load(SpritePath);
        AddChild(sprite);
        ProcessMode = Node.ProcessModeEnum.Always;

        Collision = new CollisionShape2D();
        Collision.Shape = new RectangleShape2D { Size = sprite.Texture.GetSize() };
        AddChild(Collision);
        Height = Viewport.Y;
        BodyEntered += OnBodyEntered;
        Collision.ProcessMode = Node.ProcessModeEnum.Always;

        SetCollisionLayerValue(1, false);
        SetCollisionLayerValue(2, true);
        SetCollisionMaskValue(1, true);
        SetCollisionMaskValue(2, false);

        Show();
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += Velocity * (float)delta;
        Rotation = Velocity.Angle() + Mathf.Pi;

        if (this.p.cooldown == 0)
            CheckPlayerCollision();
        if (Position.Y <= 0 || Position.X <= 0)
            QueueFree();
    }

    private void OnBodyEntered(Node body)
    {
        GD.Print("Bullet Encountered");
        if (body is Player b)
        {
            b.Health -= Damage;
        }
    }

    private void CheckPlayerCollision()
    {
        var bx = Position.X;
        var by = Position.Y;
        var bex = Collision.Shape.GetRect().Size.X;
        var bey = Collision.Shape.GetRect().Size.Y;

        var px = this.p.Position.X;
        var py = this.p.Position.Y;
        var pex = this.p.Collision.Shape.GetRect().Size.X;
        var pey = this.p.Collision.Shape.GetRect().Size.Y;

        bool overlapX = bx <= px + pex && bx + bex >= px;
        bool overlapY = by <= py + pey && by + bey >= py;

        if (overlapX && overlapY)
        {
            this.p.Health -= 1;
            this.p.cooldown = 30;
            GD.Print("Collision Detected");
        }
    }
}
