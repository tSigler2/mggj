using Godot;

public partial class InteractableEntity : Area2D
{
    [Signal]
    public delegate void OnInteractEventHandler();

    [Export]
    public string InteractionText = "Press E to interact";

    private Label _interactionLabel;
    private bool _isPlayerInRange = false;

    public override void _Ready()
    {
        // Create interaction label
        _interactionLabel = new Label();
        _interactionLabel.Text = InteractionText;
        _interactionLabel.Position = new Vector2(0, -20); // Position above the entity
        _interactionLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _interactionLabel.Visible = false;
        AddChild(_interactionLabel);

        // Connect signals
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("interact") && _isPlayerInRange)
        {
            GD.Print("Interacting with entity");
            EmitSignal(SignalName.OnInteract);
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Player)
        {
            _isPlayerInRange = true;
            _interactionLabel.Visible = true;
            GD.Print("Player entered interaction range");
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body is Player)
        {
            _isPlayerInRange = false;
            _interactionLabel.Visible = false;
            GD.Print("Player exited interaction range");
        }
    }
}
