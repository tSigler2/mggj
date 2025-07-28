using Godot;

public partial class TestFailure : Node
{
    public override void _Ready()
    {
        GD.Print(  "This is how not to make a line of code"        )    ;
 
        // This line is intentionally too long to trigger formatting issues 99999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999
    }
}
