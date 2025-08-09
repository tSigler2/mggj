using Godot;

public partial class Bullet : Sprite2D
{
    public Vector2 Velocity;

    public override void _Process(double delta)
    {
        Position += Velocity * (float)delta;
    }
}
