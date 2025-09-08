using Godot;

public partial class Dialogue : Node
{
	[Signal]
	public delegate void DialogueFinishedEventHandler();

	[Export]
	public string[] DialogueLines;

	private bool _isActive = false;

	public void StartDialogue(string[] lines)
	{
		DialogueLines = lines;
		_isActive = true;
		GD.Print("Dialogue started (placeholder implementation)");

		// Simulate dialogue completion after a short delay
		GetTree().CreateTimer(2.0f).Timeout += () =>
		{
			_isActive = false;
			EmitSignal(SignalName.DialogueFinished);
			GD.Print("Dialogue finished (placeholder implementation)");
		};
	}

	public bool IsActive()
	{
		return _isActive;
	}
}
