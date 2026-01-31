namespace Contra.Src.Components.Hit;

using Godot;

[GlobalClass]
public partial class HitComponent2D : Area2D
{
// FUNCTIONS
    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;

        if (!_isClonableEmitter)
            AddChild(_emitter);
    }
    
    private void OnAreaEntered(Area2D area)
    {
        if (area is not HitboxComponent2D hitbox)
            return;

        if (_isClonableEmitter)
        {
            var copy = (HitEmitter)_emitter.Duplicate();
            var parasit = new HitParasit(copy);
            parasit.AddChild(copy);
            hitbox.AddChild(parasit);
        }
        else
        {
            var parasit = new HitParasit(_emitter);
            hitbox.AddChild(parasit);
        }
    }

// VARIABLES
    [Export] private HitEmitter _emitter;
    [Export] private bool       _isClonableEmitter;
}
