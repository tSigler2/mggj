using Godot;

public partial class Door : InteractableEntity
{
    [Export]
    public string TargetScenePath;

    [Export]
    public Vector2 PlayerSpawnPosition;

    public override void _Ready()
    {
        base._Ready();
        OnInteract += OnDoorInteract;
    }

    private void OnDoorInteract()
    {
        if (!string.IsNullOrEmpty(TargetScenePath))
        {
            GD.Print($"Loading scene: {TargetScenePath}");
            SceneManager.Instance.ChangeScene(TargetScenePath);
        }
    }
}
