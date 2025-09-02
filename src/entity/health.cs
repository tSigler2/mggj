using System;
using Godot;

public partial class health : Node2D
{
    [Export]
    public float maxHealth = 5f;
    float currentHealth;

    public override void _Ready()
    {
        currentHealth = maxHealth;
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            GetParent().QueueFree(); // Deletes parent of this script.
        }
    }
}
