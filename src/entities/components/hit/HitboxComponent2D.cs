namespace Contra.Src.Components.Hit;

using Components.Health;
using Godot;

[GlobalClass]
public partial class HitboxComponent2D : Area2D
{
// VARIABLES
	public void TakeDamage(double damage)
	{
		_healthComponent.DecreaseHealth(damage);       
	}

// VARIABLES
	[Export] private HealthComponent _healthComponent;
}
