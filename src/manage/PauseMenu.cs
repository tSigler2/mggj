using Godot;
using System;

public partial class PauseMenu : MarginContainer
{
    [Export]
    public HSlider VolumeSlider;

    [Export]
    public Button ResumeButton;

    [Export]
    public Button RestartButton;

    [Export]
    public Button QuitButton;

    // Events for button presses
    public event Action<float> VolumeChanged;
    public event Action ResumeButtonPressed;
    public event Action RestartButtonPressed;
    public event Action QuitButtonPressed;

    public override void _Ready()
    {
        // Connect UI element signals
        if (VolumeSlider != null)
        {
            VolumeSlider.ValueChanged += OnVolumeSliderValueChanged;
        }

        if (ResumeButton != null)
        {
            ResumeButton.Pressed += OnResumeButtonPressed;
        }

        if (RestartButton != null)
        {
            RestartButton.Pressed += OnRestartButtonPressed;
        }

        if (QuitButton != null)
        {
            QuitButton.Pressed += OnQuitButtonPressed;
        }
    }

    private void OnVolumeSliderValueChanged(double value)
    {
        VolumeChanged?.Invoke((float)value);
    }

    private void OnResumeButtonPressed()
    {
        ResumeButtonPressed?.Invoke();
    }

    private void OnRestartButtonPressed()
    {
        RestartButtonPressed?.Invoke();
    }

    private void OnQuitButtonPressed()
    {
        QuitButtonPressed?.Invoke();
    }

    public void SetVolume(float volume)
    {
        if (VolumeSlider != null)
        {
            VolumeSlider.Value = volume;
        }
    }
}
