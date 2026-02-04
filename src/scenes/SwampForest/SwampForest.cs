using Godot;

public partial class SwampForest : Node2D
{
	public override void _Ready()
	{
		StartFadeIn();
	}

	private void StartFadeIn()
	{
		Tween tween = CreateTween();
		tween.TweenInterval(0.1f);
		tween.TweenProperty(_fadeRect, "color", new Color(0, 0, 0, 0), 1.5f);
		tween.Finished += _canvas.QueueFree;
	}

	[Export] private CanvasLayer _canvas;
	[Export] private ColorRect _fadeRect;
}
