using Godot;

public partial class Projectile2D : Node2D
{
	[Export]
	public bool Static
	{
		get;
		set
		{
			if (field == value)
				return;
			field = value;
			SetPhysicsProcess(!field);
		}
	} = false;

	[Export] public AttackComponent2D AttackComponent { get; set; } = null;
	[Export] public float             Speed           { get; set; } = 0.0f;
	[Export] public float             Lifetime        { get; set; } = 0.0f;
			 public Vector2           Direction       { get; set => field = value.Normalized(); } = Vector2.Zero;

	public override void _Ready()
	{
		// emitted when collide with hitbox
		AttackComponent.AreaEntered += (_) => 
		{ 
			AttackComponent?.Attack(); QueueFree(); 
		};

		// emitted when collide with walls
		AttackComponent.BodyEntered += (_) => {
            QueueFree();
        };

		GetTree().CreateTimer(Lifetime).Timeout += QueueFree; 
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += Direction * Speed * (float)delta;
	}
}
