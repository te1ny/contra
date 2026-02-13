using Godot;

public partial class RestartTo : Node2D
{
    [Export] private Button _restartButton;
	[Export] private string _targetScene;
    [Export] private Button _exitButton;

    public override void _Ready()
    {
        _restartButton.Pressed += OnRestart;
        _exitButton.Pressed    += OnExit;
    }

    private void OnRestart()
    {
        GetTree().ChangeSceneToFile(_targetScene);
    }

    private void OnExit()
    {
        GetTree().Quit();
    }
}
