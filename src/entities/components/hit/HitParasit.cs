namespace Contra.Src.Components.Hit;

using Godot;

public partial class HitParasit(HitEmitter owner) : Node
{
    public override void _Ready()
    {
        owner.Hit          += GetParent<HitboxComponent2D>().TakeDamage;
        owner.KillYourself += OnKillYourself;
    }

    private void OnKillYourself()
    {
        owner.Hit          -= GetParent<HitboxComponent2D>().TakeDamage;
        owner.KillYourself -= OnKillYourself;
        QueueFree();
    }
}
