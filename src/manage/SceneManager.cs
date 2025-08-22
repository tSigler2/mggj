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
        // Remove the current scene
        if (currentScene != null)
        {
            currentScene.QueueFree();
        }

        // Load the new scene
        var nextScene = GD.Load<PackedScene>(path);
        currentScene = nextScene.Instantiate();

        // Add it to the scene tree
        GetTree().Root.AddChild(currentScene);

        // Optionally, set it as the current scene
        // GetTree().CurrentScene = currentScene;
    }
}
