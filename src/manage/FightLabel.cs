using Godot;

partial class FightLabel : Label
{
    [Export]
    public Player p;

    public override void _Process(double delta)
    {
        Text = $"Health: {p.Health}";
    }
}
