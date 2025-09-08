using Godot;

public partial class FriendBedroom : PausableScene
{
    [Export]
    public string FriendName;

    [Export]
    public PackedScene BossBattleScene;

    [Export]
    public AudioStream BossMusic;

    private bool conversationCompleted = false;
    private NPC friendNPC;

    public override void _Ready()
    {
        base._Ready();

        // Get reference to friend NPC
        friendNPC = GetNode<NPC>(FriendName + "NPC");
        if (friendNPC != null)
        {
            friendNPC.Interaction += OnFriendInteraction;
        }
    }

    private void OnFriendInteraction(InteractableEntity interactable)
    {
        if (!conversationCompleted)
        {
            StartConversation();
        }
        else
        {
            StartBossBattle();
        }
    }

    private async void StartConversation()
    {
        // Disable player movement
        var player = GetNode<Player>("Player");
        player.SetProcessInput(false);

        // Show dialogue
        var dialogue = GetNode<Dialogue>("/root/Dialogue");
        await dialogue.ShowDialogue(FriendName + "Conversation");

        // Transformation sequence
        await friendNPC.TransformToMoth();

        conversationCompleted = true;

        // Re-enable player movement
        player.SetProcessInput(true);
    }

    private void StartBossBattle()
    {
        // Transition to boss battle
        if (BossBattleScene != null)
        {
            GetTree().ChangeSceneToPacked(BossBattleScene);

            // Change music if needed
            if (BossMusic != null)
            {
                // Implement your music change logic here
            }
        }
    }
}
