using Godot;

public partial class BoardingSchool : PausableScene
{
    [Export]
    public Node2D PlayerStartPosition;

    private Player player;

    public override void _Ready()
    {
        base._Ready();

        // Load player if not already in scene
        player = GetNodeOrNull<Player>("Player");
        if (player == null)
        {
            var playerScene = GD.Load<PackedScene>("res://scenes/player.tscn");
            player = playerScene.Instantiate<Player>();
            AddChild(player);

            if (PlayerStartPosition != null)
            {
                player.GlobalPosition = PlayerStartPosition.GlobalPosition;
            }
        }

        // Connect to doors or other interactables
        SetupInteractables();
    }

    private void SetupInteractables()
    {
        // Find all doors and connect their signals
        foreach (var door in GetTree().GetNodesInGroup("doors"))
        {
            if (door is InteractableEntity interactable)
            {
                interactable.Interaction += OnDoorInteracted;
            }
        }
    }

    private void OnDoorInteracted(InteractableEntity interactable)
    {
        if (interactable.Name == "FriendOneDoor")
        {
            GetNode<Ui>("/root/UI").TransitionToScene("res://scenes/friend_one_bedroom.tscn");
        }
        else if (interactable.Name == "FriendTwoDoor")
        {
            GetNode<Ui>("/root/UI").TransitionToScene("res://scenes/friend_two_bedroom.tscn");
        }
        else if (interactable.Name == "FriendThreeDoor")
        {
            GetNode<Ui>("/root/UI").TransitionToScene("res://scenes/friend_three_bedroom.tscn");
        }
    }
}
