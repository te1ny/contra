namespace Contra.Src.Components.Move;

using Godot;
using Contra.Src.Entities;

[GlobalClass]
public partial class MoveComponent2D : Node2D
{
// FUNCTIONS
    public override void _PhysicsProcess(double delta)
	{
		_target.Velocity = Velocity;
		_target.MoveAndSlide();
	}

// PROPERTIES
	[Export] private float HorizontalSpeed
	{
		get;
		set => field = Mathf.Clamp(value, 0, 100_000);
	}

	// Ideally, there should not be here
	[Export] private float AirResistance
	{
		get;
		set => field = Mathf.Clamp(value, 0, 100_000);
	}

	[Export] private float VerticalSpeed
	{
		get;
		set => field = Mathf.Clamp(value, 0, 100_000);
	}

	// Ideally, there should not be here
	[Export] private float Gravity
	{
		get;
		set => field = Mathf.Clamp(value, 0, 100_000);
	}

	public Vector2 Velocity
	{
		get
		{
			float delta = (float)GetPhysicsProcessDeltaTime();

			Vector2 direction = _logic.GetDirection();
			if (direction.LengthSquared() > 1)
				direction = direction.Normalized();

			if (_target.IsOnFloor())
			{
				// X
				field.X = direction.X * HorizontalSpeed;

				// Y
				if (Mathf.IsZeroApprox(direction.Y))
				{
					field.Y = 0;
				}
				else
				{
					field.Y = -Mathf.Sign(direction.Y) * VerticalSpeed;
				}
			}
			else
			{
				// X
				if (Mathf.IsZeroApprox(direction.X))
				{
					field.X -= Mathf.Sign(field.X) * AirResistance * delta;
				}
				else
				{
					field.X += HorizontalSpeed * direction.X * delta;
					field.X = Mathf.Clamp(field.X, -HorizontalSpeed, HorizontalSpeed);
				}

				// Y
				field.Y += Gravity * delta;
			}

			return field;
		}
	}

// VARIABLES
	[Export] private Entity2D    _target;
	[Export] private MoveLogic2D _logic;
}
