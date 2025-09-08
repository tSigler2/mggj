using Godot;

public partial class PausableScene : Node2D
{
    [Export]
    public PackedScene PauseMenuScene;

    protected PauseMenu pauseMenu;
    protected bool isPaused = false;

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel") && !@event.IsEcho())
        {
            TogglePause();
            GetViewport().SetInputAsHandled();
        }
    }

    protected virtual void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    protected virtual void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;
        GetTree().Paused = true;

        // Instantiate pause menu if not already exists
        if (pauseMenu == null && PauseMenuScene != null)
        {
            pauseMenu = PauseMenuScene.Instantiate<PauseMenu>();
            AddChild(pauseMenu);

            // Connect events
            pauseMenu.ResumeButtonPressed += OnResumeButtonPressed;
            pauseMenu.RestartButtonPressed += OnRestartButtonPressed;
            pauseMenu.QuitButtonPressed += OnQuitButtonPressed;
            pauseMenu.VolumeChanged += OnVolumeChanged;

            // Set initial volume
            pauseMenu.SetVolume(GetNode<Ui>("/root/UI").LoadVolumeSetting());
        }

        if (pauseMenu != null)
        {
            pauseMenu.Visible = true;
        }
    }

    protected virtual void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;
        GetTree().Paused = false;

        if (pauseMenu != null)
        {
            pauseMenu.Visible = false;
        }
    }

    protected virtual void OnResumeButtonPressed()
    {
        ResumeGame();
    }

    protected virtual void OnRestartButtonPressed()
    {
        GetTree().ReloadCurrentScene();
        ResumeGame();
    }

    protected virtual void OnQuitButtonPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToPacked(GetNode<Ui>("/root/UI").MainMenuScene);
    }

    protected virtual void OnVolumeChanged(float volume)
    {
        GetNode<Ui>("/root/UI").ApplyAudioSettings(volume);
        GetNode<Ui>("/root/UI").SaveVolumeSetting(volume);
    }
}
