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

    public float Speed = 150f;

    public int ShiftDirection = 1;
    public float ShiftStrength = 200f;

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
        Vector2 toCenter = (Target - Position).Normalized();

        Vector2 tangent = toCenter.Rotated(ShiftDirection * Mathf.Pi / 2);

        Vector2 finalVel = toCenter * Speed + tangent * ShiftStrength;

        Position += finalVel * (float)delta;

        Rotation = finalVel.Angle() + Mathf.Pi;

        if ((Target - Position).Length() < 5f)
            QueueFree();
    }
}
