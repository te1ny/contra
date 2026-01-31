namespace Contra.Src.Entities;

using Godot;
using Components.Health;
using Components.Hit;
using Components.Move;

[GlobalClass]
public partial class Entity2D : CharacterBody2D
{
// VARIABLES
    [Export] protected HealthComponent   _healthComponent;
    [Export] protected MoveComponent2D   _moveComponent;
    [Export] protected HitboxComponent2D _hitboxComponent;
}
