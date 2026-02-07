using Godot;
using GodotUtilities.Logic;
using Godot.Collections;

[GlobalClass]
public partial class Druid2D : Entity2D
{
	[Export] private HealthComponent   healthComponent;
	[Export] private MoveComponent2D   moveComponent2D;
	[Export] private HitboxComponent2D hitboxComponent2D;
	[Export] private Area2D            attackTriggerArea2D;
	[Export] private Area2D            walkTriggerArea2D;
	[Export] private AnimatedSprite2D  animatedSprite2D;
	[Export] private PackedScene       currentProjectile;
	
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
		if (attackTarget is not null && !CurrentAnimationIsAsync())
		{
			animatedSprite2D.Play("walk");
		}
	}

// WALK
	private void StateWalk()
	{
		if (attackTarget is null && !CurrentAnimationIsAsync())
		{
            moveComponent2D.SetHorizontalDirection(0);
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

		if (inAttackArea && !CurrentAnimationIsAsync())
		{
			animatedSprite2D.Play("attack");
		}
	}

// ATTACK
	private void EnterStateAttack()
	{
		moveComponent2D.HorizontalSpeed = 0;
	}

	private void StateAttack()
	{
	}

	private void LeaveStateAttack()
	{
        moveComponent2D.HorizontalSpeed = baseSpeed;
        if (!CurrentAnimationIsAsync())
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
        dsm.ChangeState(StateIdle);
	}

	private void OnFrameChanged()
	{
		var animation = animatedSprite2D.Animation;
		if (animation == "attack" && animatedSprite2D.Frame == 7)
		{
			CreateAndCastProjectile(animatedSprite2D.FlipH ? Vector2.Left : Vector2.Right);
		}
	}

	private void CreateAndCastProjectile(Vector2 direction)
	{
        System.Action<int> attack = null;
        int count = 3;
        attack = async (n) => {
            if (n == 0)
                return;

            Projectile2D projectile = currentProjectile.Instantiate<Projectile2D>();

            Vector2 projectileResultPos;
            projectileResultPos.X = GlobalPosition.X + Mathf.Sign(direction.X) * projectile.Position.X;
            projectileResultPos.Y = GlobalPosition.Y + projectile.Position.Y;
            projectileResultPos.X += Mathf.Sign(direction.X) * (50 * Mathf.Abs(count - n) + 30);

            if (direction.X < 0)
            {
                projectile.Scale = new Vector2(-1, 1);
            }

            projectile.GlobalPosition = projectileResultPos;
            projectile.Direction      = direction;
            projectile.Static         = false;
            projectile.Lifetime       = 1.0f;

            GetTree().Root.AddChild(projectile);
            await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);

            attack(n - 1);
        };
        attack(count);
	}

    private bool CurrentAnimationIsAsync()
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
