using Godot;
using System;

public partial class MainMenu : MarginContainer
{
    [Export] public Button StartButton;
    [Export] public Button SettingsButton;
    [Export] public Button QuitButton;
    
    // Events for button presses
    public event Action StartButtonPressed;
    public event Action SettingsButtonPressed;
    public event Action QuitButtonPressed;
    
    public override void _Ready()
    {
        // Connect button signals
        if (StartButton != null)
        {
            StartButton.Pressed += OnStartButtonPressed;
        }
        
        if (SettingsButton != null)
        {
            SettingsButton.Pressed += OnSettingsButtonPressed;
        }
        
        if (QuitButton != null)
        {
            QuitButton.Pressed += OnQuitButtonPressed;
        }
    }
    
    private void OnStartButtonPressed()
    {
        StartButtonPressed?.Invoke();
    }
    
    private void OnSettingsButtonPressed()
    {
        SettingsButtonPressed?.Invoke();
    }
    
    private void OnQuitButtonPressed()
    {
        QuitButtonPressed?.Invoke();
    }
}