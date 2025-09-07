using Godot;

public partial class InteractableEntity : Area2D
{
    [Signal]
    public delegate void OnInteractEventHandler();

    [Export]
    public string InteractionText = "Press E to interact";

    [Export]
    public Vector2 LabelOffset = new Vector2(0, -20);

    [Export]
    public Font Font;

    [Export(PropertyHint.Range, "8,72,1")]
    public int FontSize = 16;

    [Export]
    public Color FontColor = Colors.White;

    [Export]
    public Color OutlineColor = Colors.Black;

    [Export(PropertyHint.Range, "0,10,1")]
    public int OutlineSize = 2;

    [Export]
    public HorizontalAlignment TextAlignment = HorizontalAlignment.Center;

    private Label _interactionLabel;
    private bool _isPlayerInRange = false;

    public override void _Ready()
    {
        // Create interaction label
        _interactionLabel = new Label();
        _interactionLabel.Text = InteractionText;
        _interactionLabel.Position = LabelOffset;
        _interactionLabel.HorizontalAlignment = TextAlignment;
        _interactionLabel.Visible = false;

        // Apply font settings
        if (Font != null)
        {
            _interactionLabel.AddThemeFontOverride("font", Font);
        }

        _interactionLabel.AddThemeFontSizeOverride("font_size", FontSize);
        _interactionLabel.AddThemeColorOverride("font_color", FontColor);
        _interactionLabel.AddThemeColorOverride("font_outline_color", OutlineColor);
        _interactionLabel.AddThemeConstantOverride("outline_size", OutlineSize);

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

    // Public methods to update label properties at runtime
    public void SetInteractionText(string newText)
    {
        InteractionText = newText;
        _interactionLabel.Text = newText;
    }

    public void SetFont(Font newFont)
    {
        Font = newFont;
        _interactionLabel.AddThemeFontOverride("font", newFont);
    }

    public void SetFontSize(int newSize)
    {
        FontSize = newSize;
        _interactionLabel.AddThemeFontSizeOverride("font_size", newSize);
    }

    public void SetFontColor(Color newColor)
    {
        FontColor = newColor;
        _interactionLabel.AddThemeColorOverride("font_color", newColor);
    }

    public void SetOutlineColor(Color newColor)
    {
        OutlineColor = newColor;
        _interactionLabel.AddThemeColorOverride("font_outline_color", newColor);
    }

    public void SetOutlineSize(int newSize)
    {
        OutlineSize = newSize;
        _interactionLabel.AddThemeConstantOverride("outline_size", newSize);
    }

    public void SetLabelOffset(Vector2 newOffset)
    {
        LabelOffset = newOffset;
        _interactionLabel.Position = newOffset;
    }

    public void SetTextAlignment(HorizontalAlignment alignment)
    {
        TextAlignment = alignment;
        _interactionLabel.HorizontalAlignment = alignment;
    }
}
