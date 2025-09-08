using Godot;

partial class FightLabel : Label
{
	[Export]
	public Player p;

	[Export]
	public BossOne b1;
	[Export]
	Label b1Label;
	
	[Export]
	public BossTwo b2;
	[Export]
	Label b2Label;

	[Export]
	public BossThree b3;
	[Export]
	Label b3Label;

	public override void _Process(double delta)
	{
		Text = $"Health: {p.Health}";
		if (b1 != null)
		{
			GD.Print(b1.stageTimer);
			b1Label.Text = $"Phase Ends in: {20 - (int)b1.stageTimer} ";
		}else if (b2 != null)
		{
			GD.Print(b2.stageTimer);
			b2Label.Text = $"Phase Ends in: {20 - (int)b2.stageTimer} ";
		}else if (b3 != null)
		{
			GD.Print(b3.stageTimer);
			b3Label.Text = $"Phase Ends in: {20 - (int)b3.stageTimer} ";
		}
	}
}
