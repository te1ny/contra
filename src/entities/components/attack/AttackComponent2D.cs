using Godot;

[GlobalClass]
public partial class AttackComponent2D : Area2D
{
	private double damage;

	[Export]
	public double Damage
	{
		get => damage;
		set
		{
			damage = Mathf.Max(value, 0.0);
		}
	}

	public void Attack()
	{
		foreach (Area2D area in GetOverlappingAreas())
		{
			if (area is HitboxComponent2D hitbox)
			{
				hitbox.TakeDamage(damage);
			}
		}
	}
}
