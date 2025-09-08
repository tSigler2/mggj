using Godot;
using DialogueManagerRuntime;

public partial class NPC : InteractableEntity
{
	[Export]
	public string TargetBattleScenePath;

	[Export]
	Resource dialogue = GD.Load<Resource>("");

	[Export]
	public string dialogueStart = "start";

	/*[Export]
	Resource dialogueWin = GD.Load<Resource>("");

	[Export]
	public string dialogueWinStart = "start";*/

	[Export]
	Player player;

	private bool _hasInteracted = false;

	public override void _Ready()
	{
		base._Ready();
		OnInteract += OnNPCInteract;
	}

	private void OnNPCInteract()
	{
		if (_hasInteracted)
		{
			return;
		}
		// TODO: Implement dialogue system
		// For now, we'll just transition to the battle scene after a short delay

		_hasInteracted = true;
		//player.CanMove = false;
		GD.Print("Starting dialogue with NPC");
		DialogueManager.ShowDialogueBalloon(dialogue, dialogueStart);
		DialogueManager.DialogueEnded += (Resource dialogueResource) =>
		{
			if (!string.IsNullOrEmpty(TargetBattleScenePath))
			{
				GD.Print($"Loading battle scene: {TargetBattleScenePath}");
				//player.CanMove = true;
				SceneManager.Instance.ChangeScene(TargetBattleScenePath);
			}
		};

		/*GetTree().CreateTimer(1.0f).Timeout += () =>
		{
			if (!string.IsNullOrEmpty(TargetBattleScenePath))
			{
				GD.Print($"Loading battle scene: {TargetBattleScenePath}");
				SceneManager.Instance.ChangeScene(TargetBattleScenePath);
			}
		};*/
	}
}
