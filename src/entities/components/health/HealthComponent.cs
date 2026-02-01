namespace Contra.Src.Components.Health;

using Godot;

[GlobalClass, Tool]
public partial class HealthComponent : Node2D
{
// SIGNALS
	[Signal] public delegate void HealthChangedEventHandler(HealthChangedInfo info);
	[Signal] public delegate void MaxHealthChangedEventHandler(HealthChangedInfo info);

// FUNCTIONS
	public void SetMaxHealth(double value)
	{
		double old = MaxHealth;
		MaxHealth = value;
		EmitHealthSignal(SignalName.MaxHealthChanged, old, MaxHealth);
	}

	public void SetHealth(double value)
	{
		double old = Health;
		Health = value;
		EmitHealthSignal(SignalName.HealthChanged, old, Health);
	}

	public void IncreaseHealth(double value)
	{
		if (value == 0)
			return;

		double old = Health;
		Health += value;
		EmitHealthSignal(SignalName.HealthChanged, old, Health);
	}

	public void DecreaseHealth(double value)
	{
		if (value == 0)
			return;

		double old = Health;
		Health -= value;
		EmitHealthSignal(SignalName.HealthChanged, old, Health);
	}

	private void EmitHealthSignal(StringName signalName, double oldValue, double newValue)
	{
		HealthChangedInfo info = new()
		{
			Old = oldValue,
			New = newValue,
		};
		EmitSignal(signalName, info);
	}

// PROPERTIES
	[Export] public double MaxHealth
	{ 
		get; 
		private set
		{
			field = Mathf.Max(value, 0);
		}
	}

	[Export] public double Health
	{ 
		get; 
		private set
		{
			field = Mathf.Clamp(value, 0, MaxHealth);
		}
	}
}

public partial class HealthChangedInfo : RefCounted
{
// PROPERTIES
	public double Old { get; set; }
	public double New { get; set; }
}
