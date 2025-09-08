using Godot;

public partial class BossBattle : PausableScene
{
    [Export]
    public PackedScene BossScene;

    [Export]
    public PackedScene WinScene;

    [Export]
    public PackedScene LoseScene;

    private BossBase boss;
    private Player player;

    public override void _Ready()
    {
        base._Ready();

        // Instantiate boss
        if (BossScene != null)
        {
            boss = BossScene.Instantiate<BossBase>();
            AddChild(boss);
            boss.BossDefeated += OnBossDefeated;
            boss.BossHealthChanged += OnBossHealthChanged;
        }

        // Get or create player
        player = GetNodeOrNull<Player>("Player");
        if (player == null)
        {
            var playerScene = GD.Load<PackedScene>("res://scenes/player.tscn");
            player = playerScene.Instantiate<Player>();
            AddChild(player);
        }

        // Set up Bullet Hell HUD
        GetNode<Ui>("/root/UI")
            .EnterBulletHellMode();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GetNode<Ui>("/root/UI").ExitBulletHellMode();
    }

    private void OnBossDefeated()
    {
        // Transition to win scene
        if (WinScene != null)
        {
            GetTree().ChangeSceneToPacked(WinScene);
        }
    }

    private void OnBossHealthChanged(float healthPercentage)
    {
        // Update boss health bar in HUD
        GetNode<Ui>("/root/UI")
            .UpdateBossHealth(healthPercentage);
    }

    protected override void OnQuitButtonPressed()
    {
        // Special handling for boss battles - maybe go back to boarding school
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("res://scenes/boarding_school.tscn");
    }
}
