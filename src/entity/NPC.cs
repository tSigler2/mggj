using Godot;

public partial class NPC : InteractableEntity
{
    [Export]
    public string TargetBattleScenePath;

    [Export]
    public string[] DialogueLines;

    private bool _hasInteracted = false;

    public override void _Ready()
    {
        base._Ready();
        OnInteract += OnNPCInteract;
    }

    private void OnNPCInteract()
    {
        if (_hasInteracted)
            return;

        GD.Print("Starting dialogue with NPC");
        // TODO: Implement dialogue system
        // For now, we'll just transition to the battle scene after a short delay
        _hasInteracted = true;

        GetTree().CreateTimer(1.0f).Timeout += () =>
        {
            if (!string.IsNullOrEmpty(TargetBattleScenePath))
            {
                GD.Print($"Loading battle scene: {TargetBattleScenePath}");
                SceneManager.Instance.ChangeScene(TargetBattleScenePath);
            }
        };
    }
}
