namespace Contra.Src.Components.Move;

using Godot;

public abstract partial class MoveLogic2D : Node2D
{
    public abstract Vector2 GetDirection();
}
