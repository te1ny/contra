namespace Contra.Src.Components.Hit;

using Godot;

public abstract partial class HitEmitter : Node
{
// SIGNALS
    [Signal] public delegate void HitEventHandler(double damage);
    [Signal] public delegate void KillYourselfEventHandler();

// FUNCTIONS
    public abstract override void _PhysicsProcess(double delta);
}
