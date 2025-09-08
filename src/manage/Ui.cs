using System;
using Godot;

public partial class Ui : CanvasLayer
{
	public Vector2 ScreenSize = new Vector2(480.0f, 640.0f);

	// Export references to menu scenes
	[Export]
	public PackedScene MainMenuScene;

	[Export]
	public PackedScene SettingsMenuScene;

	[Export]
	public PackedScene PauseMenuScene;

	[Export]
	public PackedScene BulletHellHUDScene;

	// Button click sound
	[Export]
	public AudioStream ButtonClickSound;

	// Menu instances
	private MainMenu mainMenu;
	private SettingsMenu settingsMenu;
	private PauseMenu pauseMenu;
	private BulletHellHUD bulletHellHUD;

	private bool gameStarted = false;
	private bool inBulletHell = false;

	private AudioStreamPlayer buttonClickPlayer;
	private ColorRect sceneTransitionOverlay;

	public override void _Ready()
	{
		// Set initial volume from saved settings
		ApplyAudioSettings(LoadVolumeSetting());

		// Button click sound player
		buttonClickPlayer = new AudioStreamPlayer();
		AddChild(buttonClickPlayer);

		if (ButtonClickSound != null)
		{
			buttonClickPlayer.Stream = ButtonClickSound;
		}
		/*
		else
		{
			// Fallback to the hardcoded path if not assigned in editor
			buttonClickPlayer.Stream = GD.Load<AudioStream>(
				"res://assets/sound/ui/button_click.ogg"
			);
			GD.Print("ButtonClickSound not assigned in editor, using fallback");
		}
		*/

		buttonClickPlayer.Bus = "UI";
		buttonClickPlayer.VolumeDb = -10;

		// Create initial fade overlay
		ColorRect fadeOverlay = new ColorRect();
		fadeOverlay.Name = "FadeOverlay";
		fadeOverlay.AnchorRight = 1;
		fadeOverlay.AnchorBottom = 1;
		fadeOverlay.Color = new Color(0, 0, 0, 1);
		AddChild(fadeOverlay);

		// Create scene transition overlay but don't add it yet
		sceneTransitionOverlay = new ColorRect();
		sceneTransitionOverlay.Name = "SceneTransitionOverlay";
		sceneTransitionOverlay.AnchorRight = 1;
		sceneTransitionOverlay.AnchorBottom = 1;
		sceneTransitionOverlay.Color = new Color(0, 0, 0, 0);
		sceneTransitionOverlay.Visible = false;

		// Instantiate menu scenes
		InstantiateMenus();

		// Start fade animation
		FadeInFromBlack();

		ProcessMode = ProcessModeEnum.Always;
		GD.Print(ProcessMode);

		ShowMainMenu();
	}

	private void InstantiateMenus()
	{
		// Instantiate MainMenu (existing code)
		if (MainMenuScene != null)
		{
			mainMenu = MainMenuScene.Instantiate<MainMenu>();
			AddChild(mainMenu);

			// Connect to button events
			mainMenu.StartButtonPressed += OnStartButtonPressed;
			mainMenu.SettingsButtonPressed += OnSettingsButtonPressed;
			mainMenu.QuitButtonPressed += OnQuitButtonPressed;
		}

		// Instantiate SettingsMenu
		if (SettingsMenuScene != null)
		{
			settingsMenu = SettingsMenuScene.Instantiate<SettingsMenu>();
			AddChild(settingsMenu);
			settingsMenu.Visible = false;
			ConnectSettingsMenuEvents();
		}

		// Instantiate PauseMenu
		if (PauseMenuScene != null)
		{
			pauseMenu = PauseMenuScene.Instantiate<PauseMenu>();
			AddChild(pauseMenu);
			pauseMenu.Visible = false;
			ConnectPauseMenuEvents();
		}

		// Instantiate BulletHellHUD (existing code)
		if (BulletHellHUDScene != null)
		{
			bulletHellHUD = BulletHellHUDScene.Instantiate<BulletHellHUD>();
			AddChild(bulletHellHUD);
			bulletHellHUD.Visible = false;
		}
	}

	private void ApplyResolution(string resolution)
	{
		// Parse resolution string (e.g., "1920x1080")
		var parts = resolution.Split('x');
		if (
			parts.Length == 2
			&& int.TryParse(parts[0], out int width)
			&& int.TryParse(parts[1], out int height)
		)
		{
			GetWindow().Size = new Vector2I(width, height);
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

	private void ShowMainMenu()
	{
		if (mainMenu != null)
			mainMenu.Visible = true;
		if (settingsMenu != null)
			settingsMenu.Visible = false;
		if (pauseMenu != null)
			pauseMenu.Visible = false;
		gameStarted = false;
		GetTree().Paused = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		if (bulletHellHUD != null)
			bulletHellHUD.Visible = false;
	}

	private void ShowSettingsMenu()
	{
		if (settingsMenu != null)
			settingsMenu.Visible = true;
		if (mainMenu != null)
			mainMenu.Visible = false;
		if (pauseMenu != null)
			pauseMenu.Visible = false;
		if (bulletHellHUD != null)
			bulletHellHUD.Visible = false;
	}

	public async void TransitionToScene(string scenePath)
	{
		// Add the transition overlay if it hasn't been added yet
		if (sceneTransitionOverlay.GetParent() == null)
		{
			AddChild(sceneTransitionOverlay);
		}

		sceneTransitionOverlay.Visible = true;

		// Fade out
		float elapsed = 0;
		float duration = 0.5f;

		while (elapsed < duration)
		{
			elapsed += (float)GetProcessDeltaTime();
			float alpha = Mathf.Lerp(0, 1, elapsed / duration);
			sceneTransitionOverlay.Color = new Color(0, 0, 0, alpha);
			await ToSignal(GetTree(), "process_frame");
		}

		// Change scene
		GetTree().ChangeSceneToFile(scenePath);

		// Fade in
		elapsed = 0;
		while (elapsed < duration)
		{
			elapsed += (float)GetProcessDeltaTime();
			float alpha = Mathf.Lerp(1, 0, elapsed / duration);
			sceneTransitionOverlay.Color = new Color(0, 0, 0, alpha);
			await ToSignal(GetTree(), "process_frame");
		}

		sceneTransitionOverlay.Visible = false;
	}

	private void PlayButtonClick()
	{
		if (buttonClickPlayer != null && buttonClickPlayer.Stream != null)
		{
			buttonClickPlayer.Play();
		}
	}

	// Button event handlers
	private void OnStartButtonPressed()
	{
		PlayButtonClick();
		if (mainMenu != null)
			mainMenu.Visible = false;
		if (settingsMenu != null)
			settingsMenu.Visible = false;
		if (pauseMenu != null)
			pauseMenu.Visible = false;
		gameStarted = true;
		GetTree().Paused = false;

		// Start the game - transition to the first scene (boarding school)
		TransitionToScene("res://scenes/boarding_school.tscn");
	}

	private void OnSettingsButtonPressed()
	{
		PlayButtonClick();
		ShowSettingsMenu();
	}

	private void OnQuitButtonPressed()
	{
		PlayButtonClick();
		GetTree().Quit();
	}

	public void UpdateScore(int score)
	{
		if (bulletHellHUD != null)
		{
			bulletHellHUD.UpdateScore(score);
		}
	}

	public void UpdateHealth(byte health)
	{
		if (bulletHellHUD != null)
		{
			bulletHellHUD.UpdateHealth(health);
		}
	}

	public void UpdateAbility(string ability)
	{
		if (bulletHellHUD != null)
		{
			bulletHellHUD.UpdateAbility(ability);
		}
	}

	public void EnterBulletHellMode()
	{
		inBulletHell = true;
		if (bulletHellHUD != null)
		{
			bulletHellHUD.Visible = true;
		}
	}

	public void ExitBulletHellMode()
	{
		inBulletHell = false;
		if (bulletHellHUD != null)
		{
			bulletHellHUD.Visible = false;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			// If game is started and no menu is visible, show pause menu
			if (
				gameStarted
				&& (mainMenu == null || !mainMenu.Visible)
				&& (settingsMenu == null || !settingsMenu.Visible)
				&& (pauseMenu == null || !pauseMenu.Visible)
			)
			{
				ShowPauseMenu();
				return;
			}

			// If pause menu is visible, resume game
			if (pauseMenu != null && pauseMenu.Visible)
			{
				ResumeGame();
				return;
			}

			// If settings menu is open, go back to previous menu
			if (settingsMenu != null && settingsMenu.Visible)
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
		}
	}

	private void ShowPauseMenu()
	{
		if (pauseMenu != null)
			pauseMenu.Visible = true;
		if (bulletHellHUD != null)
			bulletHellHUD.Visible = false;
		GetTree().Paused = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	private void ResumeGame()
	{
		PlayButtonClick();
		if (pauseMenu != null)
			pauseMenu.Visible = false;
		if (inBulletHell && bulletHellHUD != null)
		{
			bulletHellHUD.Visible = true;
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

	private void ApplyAudioSettings(float volume)
	{
		// Convert slider value (0-100) to decibels (-50dB to 0dB)
		float db = volume > 0 ? Mathf.LinearToDb(volume / 100f) : -50;
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), db);
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

	private void ConnectSettingsMenuEvents()
	{
		if (settingsMenu != null)
		{
			settingsMenu.VolumeChanged += OnSettingsVolumeChanged;
			settingsMenu.ResolutionChanged += OnSettingsResolutionChanged;
			settingsMenu.ApplyButtonPressed += OnSettingsApplyButtonPressed;
			settingsMenu.BackButtonPressed += OnSettingsBackButtonPressed;
		}
	}

	private void ConnectPauseMenuEvents()
	{
		if (pauseMenu != null)
		{
			pauseMenu.VolumeChanged += OnPauseVolumeChanged;
			pauseMenu.ResumeButtonPressed += OnPauseResumeButtonPressed;
			pauseMenu.RestartButtonPressed += OnPauseRestartButtonPressed;
			pauseMenu.QuitButtonPressed += OnPauseQuitButtonPressed;
		}
	}

	// Settings menu event handlers
	private void OnSettingsVolumeChanged(float volume)
	{
		// Handle volume change from settings menu
		ApplyAudioSettings(volume);
	}

	private void OnSettingsResolutionChanged(string resolution)
	{
		// Handle resolution change from settings menu
		ApplyResolution(resolution);
	}

	private void OnSettingsApplyButtonPressed()
	{
		// Save settings when apply button is pressed
		if (settingsMenu != null)
		{
			SaveVolumeSetting((float)settingsMenu.VolumeSlider.Value);
		}
		PlayButtonClick();
	}

	private void OnSettingsBackButtonPressed()
	{
		// Go back to previous menu
		PlayButtonClick();
		if (gameStarted)
		{
			ShowPauseMenu();
		}
		else
		{
			ShowMainMenu();
		}
	}

	// Pause menu event handlers
	private void OnPauseVolumeChanged(float volume)
	{
		// Handle volume change from pause menu
		ApplyAudioSettings(volume);
		SaveVolumeSetting(volume);
	}

	private void OnPauseResumeButtonPressed()
	{
		// Resume game
		PlayButtonClick();
		ResumeGame();
	}

	private void OnPauseRestartButtonPressed()
	{
		// Restart current scene
		PlayButtonClick();
		GetTree().ReloadCurrentScene();
	}

	private void OnPauseQuitButtonPressed()
	{
		// Quit to main menu
		PlayButtonClick();
		ShowMainMenu();
	}
}
