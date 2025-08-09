using Godot;

public partial class BaseEntity : CharacterBody2D
{
    [Export]
    protected int Health { get; set; }
}
