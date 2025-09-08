using Godot;

public partial class PauseMenu : Control
{
    [Signal]
    public delegate void ResumeButtonPressedEventHandler();

    [Signal]
    public delegate void RestartButtonPressedEventHandler();

    [Signal]
    public delegate void QuitButtonPressedEventHandler();

    [Signal]
    public delegate void VolumeChangedEventHandler(float volume);

    [Export]
    public HSlider VolumeSlider;

    private Button _resumeButton;
    private Button _restartButton;
    private Button _quitButton;

    public override void _Ready()
    {
        _resumeButton = GetNode<Button>("ResumeButton");
        _restartButton = GetNode<Button>("RestartButton");
        _quitButton = GetNode<Button>("QuitButton");

        _resumeButton.Pressed += OnResumePressed;
        _restartButton.Pressed += OnRestartPressed;
        _quitButton.Pressed += OnQuitPressed;

        if (VolumeSlider != null)
        {
            VolumeSlider.ValueChanged += OnVolumeChanged;
        }
    }

    private void OnResumePressed()
    {
        EmitSignal(SignalName.ResumeButtonPressed);
    }

    private void OnRestartPressed()
    {
        EmitSignal(SignalName.RestartButtonPressed);
    }

    private void OnQuitPressed()
    {
        EmitSignal(SignalName.QuitButtonPressed);
    }

    private void OnVolumeChanged(double value)
    {
        EmitSignal(SignalName.VolumeChanged, (float)value);
    }
}
