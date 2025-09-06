using Godot;

public partial class SceneManager : Node
{
    public static SceneManager Instance { get; private set; }

    private Node currentScene;

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            // Keep this node running when changing scenes
            ProcessMode = ProcessModeEnum.Always;
        }
        else
        {
            QueueFree();
        }
    }

    public void ChangeScene(string path)
    {
        CallDeferred(nameof(DeferredChangeScene), path);
    }

    private void DeferredChangeScene(string path)
    {
        // Get the current scene from the tree
        var oldScene = GetTree().CurrentScene;

        // Load the new scene
        var nextScene = GD.Load<PackedScene>(path);
        if (nextScene == null)
        {
            GD.PrintErr($"Failed to load scene: {path}");
            return;
        }

        currentScene = nextScene.Instantiate();

        // Add the new scene to the root
        GetTree().Root.AddChild(currentScene);

        // Set it as the current scene
        GetTree().CurrentScene = currentScene;

        // Remove the old scene if it exists
        if (oldScene != null)
        {
            oldScene.QueueFree();
        }

        GD.Print($"Scene changed to: {path}");
    }
}
