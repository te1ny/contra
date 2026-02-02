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
	[Export] public bool Enabled
	{
		get;
		set
		{
			if (value == field)
				return;
			field = value;
			SetPhysicsProcess(field);
		}
	}

	[Export] public Vector2 LastDirection
	{
		get;
		private set;
	}

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

	[Export] private float JumpHeight
	{
		get;
		set => field = Mathf.Clamp(value, 0, 100_000);
	}

	[Export] private float TimeToPeak
	{
		get;
		set => field = Mathf.Clamp(value, 0, 100_000);
	}

	private float VerticalSpeed
	{
		get => -(2.0f * JumpHeight / TimeToPeak);
	}

	// Ideally, there should not be here
	private float Gravity
	{
		get => 2.0f * JumpHeight / Mathf.Pow(TimeToPeak, 2);
	}

	public Vector2 Velocity
	{
		get
		{
			float delta = (float)GetPhysicsProcessDeltaTime();

			LastDirection = _logic.GetDirection();
			if (LastDirection.LengthSquared() > 1)
				LastDirection = LastDirection.Normalized();
			if (_target.IsOnFloor())
			{
				// X
				field.X = LastDirection.X * HorizontalSpeed;

				// Y
				if (Mathf.IsZeroApprox(LastDirection.Y))
				{
					field.Y = 0;
				}
				else
				{
					field.Y = -Mathf.Sign(LastDirection.Y) * VerticalSpeed;
				}
			}
			else
			{
				// X
				if (Mathf.IsZeroApprox(LastDirection.X))
				{
					field.X -= Mathf.Sign(field.X) * AirResistance * delta;
				}
				else
				{
					field.X += HorizontalSpeed * LastDirection.X * delta;
					field.X = Mathf.Clamp(field.X, -HorizontalSpeed, HorizontalSpeed);
				}

				// Y
				if (_target.IsOnCeiling())
				{
					field.Y = Gravity * delta;
				}
				else
				{
					field.Y += Gravity * delta;
				}
			}

			if (Mathf.Abs(field.X) < 1.0f) field.X = 0; // jitter fix

			// Sprite flip
			if (Mathf.IsZeroApprox(field.X))
			{
				if (!Mathf.IsZeroApprox(LastDirection.X))
				{
					_sprite.FlipH = LastDirection.X < 0.0f;
				}
			}
			else
			{
				_sprite.FlipH = field.X < 0.0f;
			}

			return field;
		}
		set;
	}

// VARIABLES
	[Export] private Entity2D         _target;
	[Export] private MoveLogic2D      _logic;
	[Export] private AnimatedSprite2D _sprite;
}
