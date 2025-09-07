using Godot;


public partial class Dialogue : InteractableEntity
{
	// Export as Resource because C# cannot directly export GDScript types
	[Export] public Resource DialogueResourceObject;
	[Export] public string DialogueStart = "start";

	private bool _isPlayerInRange = false;

	public override void _Ready()
	{
		base._Ready();
		OnInteract += Action;
	}

	// Call this to trigger dialogue
	public void Action()
	{
		if (DialogueResourceObject == null)
		{
			GD.PrintErr("⚠ DialogueResourceObject is not assigned!");
			return;
		}

		GD.Print($"Action called on: {this}");
		GD.Print($"Dialogue resource: {DialogueResourceObject}");

		// Call the GDScript DialogueManager singleton
		var manager = Engine.GetSingleton("DialogueManager");
		if (manager != null)
		{
			manager.Call("show_dialogue_balloon", DialogueResourceObject, DialogueStart);
		}
		else
		{
			GD.PrintErr("⚠ DialogueManager singleton not found!");
		}
	}
}
