using Godot;

public partial class CompilationError : Node
{
    public override void _Ready()
    {
        // this should cause a compilation error
        GD.NonExistentMethod("This isn't a real method :)");
    }
}
