using Godot;

public partial class Congratulations : Node2D
{
	public override void _Ready()
	{
		GetTree().CreateTimer(3.0f).Timeout += () => GetTree().Quit();
	}
}
