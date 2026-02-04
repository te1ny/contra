using Contra.Src.Entities.Player;
using Godot;

public partial class ToSwampForest : Area2D
{
	private ColorRect _fadeRect = new();
	private string _targetScene = "res://src/scenes/SwampForest/SwampForest.tscn";

	public override void _Ready()
	{
		_fadeRect.Color = new Color(0, 0, 0, 0);
		_fadeRect.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
		
		CanvasLayer canvas = new();
		canvas.AddChild(_fadeRect);
		AddChild(canvas);

		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player2D)
			StartTransition();
	}

	private void StartTransition()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(_fadeRect, "color", new Color(0, 0, 0, 1), 1.5f);
		tween.Finished += () => {
			GetTree().ChangeSceneToFile(_targetScene);
		};
	}
}
