using Godot;
using System;
using System.Collections.Generic;

public partial class UI : CanvasLayer
{
    public Vector2 ScreenSize = new Vector2(480.0f, 640.0f);
    
    // Menu containers
    private MarginContainer mainMenu;
    private MarginContainer settingsMenu;
    private MarginContainer pauseMenu;
    private MarginContainer dialogueContainer;
    private MarginContainer bulletHellHUD;
    
    [Export] public Label ScoreLabel;
    [Export] public Label HealthLabel;
    [Export] public Label AbilityLabel;
    
    private bool gameStarted = false;
    private bool inDialogue = false;
    private bool inBulletHell = false;
    
    private AudioStreamPlayer buttonClickPlayer;
    private SceneManager sceneManager;
    
    public override void _Ready()
    {
        // Set initial volume from saved settings
        ApplyAudioSettings(LoadVolumeSetting());
        
        // Button click sound player
        buttonClickPlayer = new AudioStreamPlayer();
        AddChild(buttonClickPlayer);
        buttonClickPlayer.Stream = GD.Load<AudioStream>("res://assets/sound/ui/button_click.ogg");
        buttonClickPlayer.Bus = "UI";
        buttonClickPlayer.VolumeDb = -10;
        
        // Create fade overlay
        ColorRect fadeOverlay = new ColorRect();
        fadeOverlay.Name = "FadeOverlay";
        fadeOverlay.AnchorRight = 1;
        fadeOverlay.AnchorBottom = 1;
        fadeOverlay.Color = new Color(0, 0, 0, 1);
        AddChild(fadeOverlay);
    
        // Start fade animation
        FadeInFromBlack();
        
        ProcessMode = ProcessModeEnum.Always;
        
        // Get menu references
        mainMenu = GetNode<MarginContainer>("MainMenu");
        settingsMenu = GetNode<MarginContainer>("SettingsMenu");
        pauseMenu = GetNode<MarginContainer>("PauseMenu");
        dialogueContainer = GetNode<MarginContainer>("DialogueContainer");
        bulletHellHUD = GetNode<MarginContainer>("BulletHellHUD");
        
        // Initialize HUD elements
        if (bulletHellHUD != null)
        {
            bulletHellHUD.Hide();
        }
        
        dialogueContainer.Hide();
        
        // Get scene manager
        sceneManager = GetNode<SceneManager>("/root/SceneManager");
        
        ShowMainMenu();
    }
    
    private void PlayButtonClick()
    {
        if (buttonClickPlayer != null && buttonClickPlayer.Stream != null)
        {
            buttonClickPlayer.Play();
        }
    }

    public void UpdateScore(int score)
    {
        if (ScoreLabel != null)
        {
            ScoreLabel.Text = $"Score: {score}";
        }
    }
    
    public void UpdateHealth(byte health)
    {
        if (HealthLabel != null)
        {
            HealthLabel.Text = $"Health: {health}";
        }
    }
    
    public void UpdateAbility(string ability)
    {
        if (AbilityLabel != null)
        {
            AbilityLabel.Text = $"Ability: {ability}";
        }
    }
    
    private async void FadeInFromBlack(float duration = 1.0f)
    {
        ColorRect fadeOverlay = GetNode<ColorRect>("FadeOverlay");
        float elapsed = 0;
    
        while (elapsed < duration)
        {
            elapsed += (float)GetProcessDeltaTime();
            float alpha = Mathf.Lerp(1, 0, elapsed / duration);
            fadeOverlay.Color = new Color(0, 0, 0, alpha);
            await ToSignal(GetTree(), "process_frame");
        }
    
        fadeOverlay.QueueFree();
    }
    
    public async void TransitionToScene(string scenePath)
    {
        // Create new fade overlay
        ColorRect fadeOverlay = new ColorRect();
        fadeOverlay.Name = "SceneTransitionOverlay";
        fadeOverlay.AnchorRight = 1;
        fadeOverlay.AnchorBottom = 1;
        fadeOverlay.Color = new Color(0, 0, 0, 0);
        AddChild(fadeOverlay);
        
        // Fade out
        float elapsed = 0;
        float duration = 0.5f;
        
        while (elapsed < duration)
        {
            elapsed += (float)GetProcessDeltaTime();
            float alpha = Mathf.Lerp(0, 1, elapsed / duration);
            fadeOverlay.Color = new Color(0, 0, 0, alpha);
            await ToSignal(GetTree(), "process_frame");
        }
        
        // Change scene
        sceneManager.ChangeScene(scenePath);
        
        // Fade in
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += (float)GetProcessDeltaTime();
            float alpha = Mathf.Lerp(1, 0, elapsed / duration);
            fadeOverlay.Color = new Color(0, 0, 0, alpha);
            await ToSignal(GetTree(), "process_frame");
        }
        
        fadeOverlay.QueueFree();
    }
    
    public void ShowDialogue(string[] dialogueLines)
    {
        inDialogue = true;
        dialogueContainer.Show();
        bulletHellHUD.Hide();
        
        // Start displaying dialogue (you'll need to implement this based on your dialogue system)
        dialogueContainer.Call("start_dialogue", dialogueLines);
    }
    
    public void EndDialogue()
    {
        inDialogue = false;
        dialogueContainer.Hide();
        
        // Transition to bullet hell mode
        EnterBulletHellMode();
    }
    
    public void EnterBulletHellMode()
    {
        inBulletHell = true;
        bulletHellHUD.Show();
        dialogueContainer.Hide();
        
        // You might want to trigger the bullet hell scene transition here
        // TransitionToScene("res://scenes/bullet_hell.tscn");
    }
    
    public void ExitBulletHellMode()
    {
        inBulletHell = false;
        bulletHellHUD.Hide();
        
        // Return to RPG mode or move to next friend's room
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            // If game is started and no menu is visible, show pause menu
            if (gameStarted && !mainMenu.Visible && !settingsMenu.Visible && !pauseMenu.Visible && !inDialogue)
            {
                ShowPauseMenu();
                return;
            }
            
            // If pause menu is visible, resume game
            if (pauseMenu.Visible)
            {
                ResumeGame();
                return;
            }
            
            // If settings menu is open, go back to previous menu
            if (settingsMenu.Visible)
            {
                if (gameStarted)
                {
                    ShowPauseMenu();
                }
                else
                {
                    ShowMainMenu();
                }
                return;
            }
            
            // If in dialogue, maybe skip dialogue or bring up options
            if (inDialogue)
            {
                dialogueContainer.Call("advance_dialogue");
            }
        }
    }

    private void ShowMainMenu()
    {
        mainMenu.Show();
        settingsMenu.Hide();
        pauseMenu.Hide();
        gameStarted = false;
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        if (bulletHellHUD != null) bulletHellHUD.Hide();
        dialogueContainer.Hide();
    }

    private void ShowSettingsMenu()
    {
        settingsMenu.Show();
        mainMenu.Hide();
        pauseMenu.Hide();
        if (bulletHellHUD != null) bulletHellHUD.Hide();
        dialogueContainer.Hide();
    }
    
    private void ShowPauseMenu()
    {
        pauseMenu.Show();
        if (bulletHellHUD != null) bulletHellHUD.Hide();
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    
        // Update volume slider in pause menu
        HSlider pauseVolumeSlider = pauseMenu.GetNodeOrNull<HSlider>("VBoxContainer/VolumeSlider");
        if (pauseVolumeSlider != null)
        {
            pauseVolumeSlider.Value = LoadVolumeSetting();
        }
    }
    
    private void ResumeGame()
    {
        PlayButtonClick();
        pauseMenu.Hide();
        if (inBulletHell && bulletHellHUD != null) 
        {
            bulletHellHUD.Show();
        }
        GetTree().Paused = false;
        
        if (inBulletHell)
        {
            Input.MouseMode = Input.MouseModeEnum.Hidden;
        }
        else
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

    private void _on_start_button_pressed()
    {
        PlayButtonClick();
        mainMenu.Hide();
        settingsMenu.Hide();
        pauseMenu.Hide();
        gameStarted = true;
        GetTree().Paused = false;
        
        // Start the game - transition to the first scene (boarding school)
        TransitionToScene("res://scenes/boarding_school.tscn");
    }

    private void _on_settings_button_pressed()
    {
        PlayButtonClick();
        ShowSettingsMenu();
    }

    private void _on_resume_button_pressed()
    {
        PlayButtonClick();
        ResumeGame();
    }

    private void _on_restart_button_pressed()
    {
        PlayButtonClick();
        GetTree().ReloadCurrentScene();
    }

    private void _on_quit_button_pressed()
    {
        PlayButtonClick();
        GetTree().Quit();
    }

    private void _on_apply_button_pressed()
    {
        PlayButtonClick();
        HSlider settingsVolumeSlider = settingsMenu.GetNode<HSlider>("VBoxContainer/VolumeSlider");
        float volume = settingsVolumeSlider != null ? (float)settingsVolumeSlider.Value : 80f;
        
        ApplyAudioSettings(volume);
        SaveVolumeSetting(volume);
    }

    private void _on_back_button_pressed()
    {
        PlayButtonClick();
        ShowMainMenu();
    }

    private void _on_pause_volume_slider_value_changed(float value)
    {
        ApplyAudioSettings(value);
        SaveVolumeSetting(value);
    }
    
    private void ApplyAudioSettings(float volume)
    {
        // Convert slider value (0-100) to decibels (-50dB to 0dB)
        float db = volume > 0 ? Mathf.LinearToDb(volume / 100f) : -50;
        AudioServer.SetBusVolumeDb(
            AudioServer.GetBusIndex("Master"), 
            db
        );
    }

    private void SaveVolumeSetting(float volume)
    {
        var config = new ConfigFile();
        config.SetValue("audio", "volume", volume);
        config.Save("user://settings.cfg");
    }

    private float LoadVolumeSetting()
    {
        var config = new ConfigFile();
        var err = config.Load("user://settings.cfg");
        if (err == Error.Ok)
        {
            return (float)config.GetValue("audio", "volume", 80f);
        }
        return 80f; // Default value
    }
}