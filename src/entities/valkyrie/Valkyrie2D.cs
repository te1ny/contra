using Godot;
using GodotUtilities.Logic;
using Godot.Collections;

[GlobalClass]
public partial class Valkyrie2D : Entity2D
{
	[Export] private HealthComponent   healthComponent;
	[Export] private MoveComponent2D   moveComponent2D;
	[Export] private HitboxComponent2D hitboxComponent2D;
	[Export] private AttackComponent2D attackComponent2D;
	[Export] private Area2D            attackTriggerArea2D;
	[Export] private Area2D            walkTriggerArea2D;
	[Export] private AnimatedSprite2D  animatedSprite2D;
	
	private float                baseSpeed;
	private bool                 lastFlip     = false;
	private Player2D             attackTarget = null;
	private bool                 inAttackArea = false;
	private DelegateStateMachine dsm = new();

	public override void _Ready()
	{
		healthComponent.HealthChanged      += OnHealthChanged;

		attackTriggerArea2D.BodyEntered    += OnAttackTriggerBodyEntered;
		attackTriggerArea2D.BodyExited     += OnAttackTriggerBodyExited;

		walkTriggerArea2D.BodyEntered      += OnWalkTriggerBodyEntered;
		walkTriggerArea2D.BodyExited       += OnWalkTriggerBodyExited;

		animatedSprite2D.AnimationChanged  += OnAnimationChanged;
		animatedSprite2D.FrameChanged      += OnFrameChanged;
		animatedSprite2D.AnimationFinished += OnAnimationFinished;

		GD.Randomize();

		moveComponent2D.Enabled = true;
		baseSpeed = moveComponent2D.HorizontalSpeed;

		dsm.AddStates(StateIdle);
		dsm.AddStates(StateWalk);
		dsm.AddStates(StateAttack,      EnterStateAttack, LeaveStateAttack);
		dsm.AddStates(StateAfterAttack, null,             LeaveStateAfterAttack);
		dsm.AddStates(StateHurt,        EnterStateHurt,   LeaveStateHurt);
		dsm.AddStates(StateDeath,       EnterStateDeath,  LeaveStateDeath);

		dsm.SetInitialState(StateIdle);
		animatedSprite2D.Play("idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		dsm.Update();
	}

// IDLE
	private void StateIdle()
	{
		// busy wait - bad
		if (attackTarget is not null)
		{
			animatedSprite2D.Play("walk");
		}
	}

// WALK
	private void StateWalk()
	{
		if (attackTarget is null)
		{
			animatedSprite2D.Play("idle");
			return;
		}

		float direction = Mathf.Sign(attackTarget.GlobalPosition.X - GlobalPosition.X);
		moveComponent2D.SetHorizontalDirection(direction);
		if (Mathf.IsZeroApprox(direction))
			animatedSprite2D.FlipH = lastFlip;
		else
		{
			animatedSprite2D.FlipH = direction < 0.0f;
			lastFlip = animatedSprite2D.FlipH;
		}

		if (inAttackArea)
		{
			animatedSprite2D.Play("attack");
			return;
		}
	}

// ATTACK
	private void EnterStateAttack()
	{
		moveComponent2D.HorizontalSpeed = baseSpeed / 3;
	}

	private void StateAttack()
	{
	}

	private void LeaveStateAttack()
	{
		uint value = GD.Randi() % 100;
		if (value >= 0 && value < 75)
		{
			moveComponent2D.HorizontalSpeed = 0;
			if (animatedSprite2D.Animation != "hurt")
				animatedSprite2D.Play("after_attack");
		}
		else
		{
			moveComponent2D.HorizontalSpeed = baseSpeed;
			if (animatedSprite2D.Animation != "hurt")
				animatedSprite2D.Play("idle");
		}
	}

// AFTER ATTACK

	private void StateAfterAttack()
	{
		float direction = 0.0f;
		if (attackTarget is not null)
			direction = Mathf.Sign(attackTarget.GlobalPosition.X - GlobalPosition.X);

		if (Mathf.IsZeroApprox(direction))
			animatedSprite2D.FlipH = lastFlip;
		else
		{
			animatedSprite2D.FlipH = direction < 0.0f;
			lastFlip = animatedSprite2D.FlipH;
		}
	}

	private void LeaveStateAfterAttack()
	{
		moveComponent2D.HorizontalSpeed = baseSpeed;
		if (animatedSprite2D.Animation != "hurt")
			animatedSprite2D.Play("idle");
	}

// HURT
	private void EnterStateHurt()
	{
		float glowPower = 1.5f; 
		animatedSprite2D.SelfModulate = new Color(1.0f * glowPower, 0.9f * glowPower, 0.9f * glowPower, 1.0f);
		moveComponent2D.HorizontalSpeed = 0;
	}

	private void StateHurt()
	{
	}

	private void LeaveStateHurt()
	{
		moveComponent2D.HorizontalSpeed = baseSpeed;
		animatedSprite2D.SelfModulate = new Color(1, 1, 1, 1);
		animatedSprite2D.Play("idle");
	}

// DEATH
	private void EnterStateDeath()
	{
		attackTriggerArea2D.Monitoring = false;
		walkTriggerArea2D.Monitoring   = false;
		attackComponent2D.Monitoring   = false;
		hitboxComponent2D.Monitorable  = false;
		moveComponent2D.Enabled        = false;
	}

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

	private void OnWalkTriggerBodyEntered(Node2D body)
	{
		if (body is Player2D player)
			attackTarget = player;
	}
	
	private void OnWalkTriggerBodyExited(Node2D body)
	{
		if (body == attackTarget)
			attackTarget = null;
	}
	private void OnAttackTriggerBodyEntered(Node2D body)
	{
		if (body == attackTarget)
			inAttackArea = true;
	}

	private void OnAttackTriggerBodyExited(Node2D body)
	{
		if (body == attackTarget) 
			inAttackArea = false;
	}

	private void OnAnimationChanged()
	{
		var animation = animatedSprite2D.Animation;

		if (animation == "idle")
		{
			dsm.ChangeState(StateIdle);
		}
		else if (animation == "walk")
		{
			dsm.ChangeState(StateWalk);
		}
		else if (animation == "attack")
		{
			dsm.ChangeState(StateAttack);
		}
		else if (animation == "after_attack")
		{
			dsm.ChangeState(StateAfterAttack);
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
		var animation = animatedSprite2D.Animation;

		if (animation == "attack")
		{
			dsm.ChangeState(StateIdle);
		}
		else if (animation == "after_attack")
		{
			dsm.ChangeState(StateIdle);
		}
		else if (animation == "hurt")
		{
			dsm.ChangeState(StateIdle);
		}
		else if (animation == "death")
		{
			dsm.ChangeState(StateIdle);
		}
	}

	private void OnFrameChanged()
	{
		var animation = animatedSprite2D.Animation;

		if (animation == "attack" && animatedSprite2D.Frame == 5)
		{
			float glowPower = 2.0f; 
			animatedSprite2D.SelfModulate = new Color(1.0f * glowPower, 0.9f * glowPower, 0.9f * glowPower, 1.0f);
			attackComponent2D.Attack();
		}
		if (animation == "attack" && animatedSprite2D.Frame == 8)
		{
			animatedSprite2D.SelfModulate = new Color(1, 1, 1, 1);
		}
	}

	private bool AnimationIsAsync()
	{
		var asyncAnimations = new Array<StringName> 
		{ 
			"hurt", "death"
		};

		foreach (StringName asyncAnimation in asyncAnimations)
		{
			if (animatedSprite2D.Animation == asyncAnimation)
				return true;
		}

		return false;
	}
}
