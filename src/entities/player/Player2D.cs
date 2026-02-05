using Godot;
using GodotUtilities.Logic;

[GlobalClass]
public partial class Player2D : Entity2D
{
	[Export] private HealthComponent   healthComponent;
	[Export] private MoveComponent2D   moveComponent2D;
	[Export] private HitboxComponent2D hitboxComponent2D;
	[Export] private AnimatedSprite2D  animatedSprite2D;
	[Export] private double            damage;

	private Vector2 dashDirection;
	private bool    lastFlip         = false;
	private DelegateStateMachine dsm = new();

	public override void _Ready()
	{
		healthComponent.HealthChanged += OnHealthChanged;

		animatedSprite2D.AnimationChanged  += OnAnimationChanged;
		animatedSprite2D.FrameChanged      += OnFrameChanged;
		animatedSprite2D.AnimationFinished += OnAnimationFinished;

		moveComponent2D.Enabled = true;

		dsm.AddStates(StateWalk,         null,                   null);
		dsm.AddStates(StateDash,         EnterStateDash,         LeaveStateDash);
		dsm.AddStates(StateBackwardDash, EnterStateBackwardDash, LeaveStateBackwardDash);
		dsm.AddStates(StateAttack,       null,                   LeaveStateAttack);
		dsm.AddStates(StateHurt,         EnterStateHurt,         LeaveStateHurt);
		dsm.AddStates(StateDeath,        null,                   null);

		dsm.SetInitialState(StateWalk);
		animatedSprite2D.Play("walk");
	}

	public override void _PhysicsProcess(double delta)
	{
		dsm.Update();
	}

// WALK
	private void StateWalk()
	{
		float horizontalDirection = Input.GetAxis("left", "right");
		moveComponent2D.SetHorizontalDirection(horizontalDirection);
		if (Mathf.IsZeroApprox(horizontalDirection))
			animatedSprite2D.FlipH = lastFlip;
		else
		{
			animatedSprite2D.FlipH = horizontalDirection < 0.0f;
			lastFlip = animatedSprite2D.FlipH;
		}

		if (Input.IsActionPressed("space"))
			moveComponent2D.Jump();

		if (Input.IsActionJustPressed("shift"))
		{
			animatedSprite2D.Play("dash");
			return;
		}

		if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			animatedSprite2D.Play("attack");
		}
	}

// DASH
	private void EnterStateDash()
	{
		moveComponent2D.Enabled = false;
		float glowPower = 3.0f; 
		animatedSprite2D.SelfModulate = new Color(1.0f * glowPower, 0.9f * glowPower, 0.9f * glowPower, 1.0f);

		float horizontalDirection = Input.GetAxis("left", "right");
		float upDirection         = Input.IsActionPressed("space") ? -1.0f : 0.0f;
		dashDirection = new(horizontalDirection, upDirection);
	}

	private void StateDash()
	{
	}

	private void LeaveStateDash()
	{
		animatedSprite2D.Play("dash_backward");
	}

// BACKWARD DASH
	private void EnterStateBackwardDash()
	{
		var test = new PhysicsTestMotionParameters2D
		{
			From   = GlobalTransform,
			Motion = 200.0f * dashDirection
		};

		var result = new PhysicsTestMotionResult2D();
		PhysicsServer2D.BodyTestMotion(GetRid(), test, result);
		GlobalPosition += result.GetTravel();
	}

	private void StateBackwardDash()
	{
	}

	private void LeaveStateBackwardDash()
	{
		moveComponent2D.Enabled = true;
		animatedSprite2D.SelfModulate = new Color(1, 1, 1, 1);
		animatedSprite2D.Play("walk");
	}

// ATTACK
	private void StateAttack()
	{
		float horizontalDirection = Input.GetAxis("left", "right");
		moveComponent2D.SetHorizontalDirection(horizontalDirection);

		if (Input.IsActionPressed("space"))
			moveComponent2D.Jump();
	}

	private void LeaveStateAttack()
	{
		animatedSprite2D.Play("walk");
	}

// HURT
	private void EnterStateHurt()
	{
		float glowPower = 1.5f; 
		animatedSprite2D.SelfModulate = new Color(1.0f * glowPower, 0.9f * glowPower, 0.9f * glowPower, 1.0f);
	}

	private void StateHurt()
	{
		float horizontalDirection = Input.GetAxis("left", "right");
		moveComponent2D.SetHorizontalDirection(horizontalDirection);

		if (Input.IsActionPressed("space"))
			moveComponent2D.Jump();
	}

	private void LeaveStateHurt()
	{
		animatedSprite2D.SelfModulate = new Color(1, 1, 1, 1);
		animatedSprite2D.Play("walk");
	}

// DEATH
	private void StateDeath()
	{
	}

	private void LeaveStateDeath()
	{
		QueueFree();
	}

// OTHER
	private void OnHealthChanged(HealthChangedInfo info)
	{
		if (info.New == 0.0)
		{
			animatedSprite2D.Play("death");
		}
		else if (info.Delta < 0.0)
		{
			animatedSprite2D.Play("hurt");
		}
	}

	private void OnAnimationChanged()
	{
		var animation = animatedSprite2D.Animation;

		if (animation == "walk")
		{
			dsm.ChangeState(StateWalk);
		}
		else if (animation == "dash")
		{
			dsm.ChangeState(StateDash);
		}
		else if (animation == "dash_backward")
		{
			dsm.ChangeState(StateBackwardDash);
		}
		else if (animation == "attack")
		{
			dsm.ChangeState(StateAttack);
		}
		else if (animation == "hurt")
		{
			dsm.ChangeState(StateHurt);
		}
		else if (animation == "death")
		{
			dsm.ChangeState(StateDeath);
		}
	}

	private void OnAnimationFinished()
	{
		dsm.ChangeState(StateWalk);
	}

	private void OnFrameChanged()
	{
		var animation = animatedSprite2D.Animation;
		if (animation == "attack" && animatedSprite2D.Frame == 5)
		{
			// cast projectile
		}
	}
}
