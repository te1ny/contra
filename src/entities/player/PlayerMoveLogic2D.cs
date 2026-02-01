namespace Contra.Src.Entities.Player;

using Godot;
using Contra.Src.Components.Move;

[GlobalClass]
public partial class PlayerMoveLogic2D : MoveLogic2D
{
	public override Vector2 GetDirection()
	{
		return Input.GetVector("left", "right", "down", "up");
	}
}
