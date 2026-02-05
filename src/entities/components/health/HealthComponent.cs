using Godot;

[GlobalClass]
public partial class HealthComponent : Node2D
{
	[Signal] public delegate void HealthChangedEventHandler(HealthChangedInfo info);
	[Signal] public delegate void MaxHealthChangedEventHandler(HealthChangedInfo info);

	private double maxHealth = 100.0;
	private double health    = 100.0;

	[Export]
	public double MaxHealth
	{
		get => maxHealth;
		set
		{
			double old = maxHealth;
			maxHealth = Mathf.Max(value, 0.0);
			EmitSignal(SignalName.MaxHealthChanged, new HealthChangedInfo{ Old = old, New = maxHealth });
		}
	}

	[Export]
	public double Health
	{
		get => health;
		set
		{
			double old = health;
			health = Mathf.Max(value, 0.0);
			EmitSignal(SignalName.HealthChanged, new HealthChangedInfo{ Old = old, New = health });
		}
	}
}

public partial class HealthChangedInfo : RefCounted
{
	public double Old { get; set; }
	public double New { get; set; }
	public double Delta => New - Old;
}
