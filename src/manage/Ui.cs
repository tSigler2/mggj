// src/manage/UI.cs
using Godot;
using System;
using System.Threading.Tasks;

public partial class UI : CanvasLayer
{
    public Vector2 ScreenSize = new Vector2(480.0f, 640.0f);

    // Menu references
    private MarginContainer _mainMenu;
    private MarginContainer _settingsMenu;
    private MarginContainer _pauseMenu;
    private MarginContainer _winScreen;
    private MarginContainer _loseScreen;

    [Export]
    public MarginContainer HUD;

    [Export]
    public Control DialogueContainer; // Container for dialogue UI

    // Scene management
    private const string MainMenuScene = "res://scenes/main_menu.tscn";
    private const string SchoolScene = "res://scenes/school_world.tscn";
    private const string BossScenePrefix = "res://scenes/boss_battles/";

    private bool _inDialogue = false;
    private bool _inBulletHell = false;

    // Audio
    private AudioStreamPlayer _buttonClickPlayer;

    public override void _Ready()
    {
        // Set initial volume
        ApplyAudioSettings(LoadVolumeSetting());

        // Setup audio player
        _buttonClickPlayer = new AudioStreamPlayer();
        AddChild(_buttonClickPlayer);
        _buttonClickPlayer.Stream = GD.Load<AudioStream>("res://assets/sound/ui/button_click.ogg");
        _buttonClickPlayer.Bus = "Master";
        _buttonClickPlayer.VolumeDb = 5;

        // Get menu references
        _mainMenu = GetNode<MarginContainer>("MainMenu");
        _settingsMenu = GetNode<MarginContainer>("SettingsMenu");
        _pauseMenu = GetNode<MarginContainer>("PauseMenu");
        _winScreen = GetNode<MarginContainer>("WinScreen");
        _loseScreen = GetNode<MarginContainer>("LoseScreen");

        // Initial state
        HUD?.Hide();
        DialogueContainer?.Hide();

        // Connect to game events
        EventBus.Instance.Connect(
            EventBus.SignalName.DialogueStateChanged,
            new Callable(this, nameof(OnDialogueStateChanged))
        );
        EventBus.Instance.Connect(
            EventBus.SignalName.BossBattleStarted,
            new Callable(this, nameof(OnBossBattleStarted))
        );

        // Initial fade
        FadeInFromBlack();
    }

    private async void FadeInFromBlack(float duration = 1.0f)
    {
        ColorRect fadeOverlay = new ColorRect
        {
            Name = "FadeOverlay",
            AnchorRight = 1,
            AnchorBottom = 1,
            Color = Colors.Black
        };
        AddChild(fadeOverlay);

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

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel") && !_inDialogue)
        {
            if (_inBulletHell && !_pauseMenu.Visible)
            {
                ShowPauseMenu();
            }
            else if (_pauseMenu.Visible)
            {
                ResumeGame();
            }
        }
    }

    // ===== Scene Management =====
    public async void LoadMainMenu()
    {
        await FadeToBlack();
        GetTree().ChangeSceneToFile(MainMenuScene);
        ShowMainMenu();
        FadeInFromBlack();
    }

    public async void LoadSchoolScene()
    {
        await FadeToBlack();
        GetTree().ChangeSceneToFile(SchoolScene);
        HUD?.Show();
        _inBulletHell = false;
        FadeInFromBlack();
    }

    public async void LoadBossScene(string bossName)
    {
        await FadeToBlack();
        GetTree().ChangeSceneToFile($"{BossScenePrefix}{bossName}.tscn");
        HUD?.Show();
        _inBulletHell = true;
        FadeInFromBlack();
    }

    private async Task FadeToBlack(float duration = 0.5f)
    {
        ColorRect fadeOverlay = new ColorRect
        {
            Name = "FadeOverlay",
            AnchorRight = 1,
            AnchorBottom = 1,
            Color = new Color(0, 0, 0, 0)
        };
        AddChild(fadeOverlay);

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += (float)GetProcessDeltaTime();
            float alpha = Mathf.Lerp(0, 1, elapsed / duration);
            fadeOverlay.Color = new Color(0, 0, 0, alpha);
            await ToSignal(GetTree(), "process_frame");
        }
    }

    // ===== Event Handlers =====
    private void OnDialogueStateChanged(bool isActive)
    {
        _inDialogue = isActive;
        DialogueContainer?.SetVisible(isActive);
    }

    private void OnBossBattleStarted(string bossName)
    {
        LoadBossScene(bossName);
    }

    // ===== Menu Functions =====
    private void ShowMainMenu()
    {
        _mainMenu.Show();
        _settingsMenu.Hide();
        _pauseMenu.Hide();
        _winScreen.Hide();
        _loseScreen.Hide();
        HUD?.Hide();
    }

    private void ShowSettingsMenu()
    {
        _settingsMenu.Show();
        _mainMenu.Hide();
        _pauseMenu.Hide();
    }

    private void ShowPauseMenu()
    {
        _pauseMenu.Show();
        GetTree().Paused = true;
    }

    public void ShowWinScreen()
    {
        _winScreen.Show();
        GetTree().Paused = true;
    }

    public void ShowLoseScreen()
    {
        _loseScreen.Show();
        GetTree().Paused = true;
    }

    private void ResumeGame()
    {
        _pauseMenu.Hide();
        GetTree().Paused = false;
    }

    // ===== Button Handlers =====
    private void OnStartButtonPressed()
    {
        PlayButtonClick();
        LoadSchoolScene();
    }

    private void OnSettingsButtonPressed()
    {
        PlayButtonClick();
        ShowSettingsMenu();
    }

    private void OnResumeButtonPressed()
    {
        PlayButtonClick();
        ResumeGame();
    }

    private void OnQuitButtonPressed()
    {
        PlayButtonClick();
        GetTree().Quit();
    }

    private void PlayButtonClick()
    {
        _buttonClickPlayer.Play();
    }

    // ===== Audio Management =====
    private void ApplyAudioSettings(float volume)
    {
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
        if (config.Load("user://settings.cfg") == Error.Ok)
        {
            return (float)config.GetValue("audio", "volume", 80f);
        }
        return 80f;
    }
}

// ===== Event Bus =====
public partial class EventBus : Node
{
    public static EventBus Instance { get; private set; }

    [Signal]
    public delegate void DialogueStateChangedEventHandler(bool isActive);

    [Signal]
    public delegate void BossBattleStartedEventHandler(string bossName);

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
            ProcessMode = ProcessModeEnum.Always;
        }
        else
        {
            QueueFree();
        }
    }
}
