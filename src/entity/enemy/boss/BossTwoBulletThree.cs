using System;
using System;
using Godot;
using Godot;

public partial class BossTwoBulletThree : BaseEntity
{
    [Export]
    public int Damage;

    [Export]
    public string SpritePath = "./assets/art/projectile/PhoebeBullet.png";

    public Sprite2D sprite;
    public Vector2 Velocity;
    public Vector2 Target;
    public Player p;

    public override void _Ready()
    {
        base._Ready();

        sprite = new Sprite2D();
        sprite.Texture = (Texture2D)ResourceLoader.Load(SpritePath);
        AddChild(sprite);
        ProcessMode = Node.ProcessModeEnum.Always;

        var collision = new CollisionShape2D();
        collision.Shape = new RectangleShape2D { Size = sprite.Texture.GetSize() };
        AddChild(collision);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 toTarget = Target - Position;

        if (Velocity.Length() * (float)delta >= toTarget.Length())
        {
            Position = Target;
            QueueFree();
            return;
        }

        Position += Velocity * (float)delta;
    }
}
