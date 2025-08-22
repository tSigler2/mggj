using Godot;
using System;

public partial class BulletHellHUD : MarginContainer
{
    [Export] public Label ScoreLabel;
    [Export] public Label HealthLabel;
    [Export] public Label AbilityLabel;
    
    public void UpdateScore(int score)
    {
        if (ScoreLabel != null)
        {
            ScoreLabel.Text = $"Score: {score}";
        }
    }
    
    public void UpdateHealth(byte health)
    {
        if (HealthLabel != null)
        {
            HealthLabel.Text = $"Health: {health}";
        }
    }
    
    public void UpdateAbility(string ability)
    {
        if (AbilityLabel != null)
        {
            AbilityLabel.Text = $"Ability: {ability}";
        }
    }
}