using Godot;

public partial class BaseEntity : CharacterBody2D
{
    [Export]
    protected int Health { get; set; }

    public Vector2 Viewport { get; set; }

    public override void _Ready()
    {
        Viewport = GetViewportRect().Size;
    }
}
