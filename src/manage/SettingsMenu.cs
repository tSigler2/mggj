using Godot;
using System;

public partial class SettingsMenu : MarginContainer
{
    [Export] public HSlider VolumeSlider;
    [Export] public OptionButton ResolutionOption;
    [Export] public Button ApplyButton;
    [Export] public Button BackButton;
    
    // Events for button presses
    public event Action<float> VolumeChanged;
    public event Action<string> ResolutionChanged;
    public event Action ApplyButtonPressed;
    public event Action BackButtonPressed;
    
    public override void _Ready()
    {
        // Connect UI element signals
        if (VolumeSlider != null)
        {
            VolumeSlider.ValueChanged += OnVolumeSliderValueChanged;
        }
        
        if (ResolutionOption != null)
        {
            ResolutionOption.ItemSelected += OnResolutionOptionItemSelected;
        }
        
        if (ApplyButton != null)
        {
            ApplyButton.Pressed += OnApplyButtonPressed;
        }
        
        if (BackButton != null)
        {
            BackButton.Pressed += OnBackButtonPressed;
        }
    }
    
    private void OnVolumeSliderValueChanged(double value)
    {
        VolumeChanged?.Invoke((float)value);
    }
    
    private void OnResolutionOptionItemSelected(long index)
    {
        if (ResolutionOption != null)
        {
            ResolutionChanged?.Invoke(ResolutionOption.GetItemText((int)index));
        }
    }
    
    private void OnApplyButtonPressed()
    {
        ApplyButtonPressed?.Invoke();
    }
    
    private void OnBackButtonPressed()
    {
        BackButtonPressed?.Invoke();
    }
    
    public void SetVolume(float volume)
    {
        if (VolumeSlider != null)
        {
            VolumeSlider.Value = volume;
        }
    }
    
    public void SetResolution(string resolution)
    {
        if (ResolutionOption != null)
        {
            for (int i = 0; i < ResolutionOption.ItemCount; i++)
            {
                if (ResolutionOption.GetItemText(i) == resolution)
                {
                    ResolutionOption.Select(i);
                    break;
                }
            }
        }
    }
}