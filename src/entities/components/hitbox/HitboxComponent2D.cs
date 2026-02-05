using Godot;

[GlobalClass]
public partial class HitboxComponent2D : Area2D
{
    private HealthComponent healthComponent;

    [Export]
    public HealthComponent HealthComponent
    {
        get => healthComponent;
        set => healthComponent = value;
    }

    public void TakeDamage(double damage)
    {
        healthComponent.Health -= damage;
    }
}
