using Godot;

[GlobalClass]
public partial class MoveComponent2D : Node2D
{
    private Entity2D target;
    private float    horizontalDirection;
    private float    horizontalSpeed;
    private float    jumpSpeed;
    private float    gravity;
    private bool     lastJump;

    public bool Enabled
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

    [Export]
    public Entity2D Target
    {
        get => target;
        set => target = value;
    }

    [Export]
	public float HorizontalSpeed
	{
		get => horizontalSpeed;
		set => horizontalSpeed = Mathf.Max(value, 0.0f);
	}
    
    [Export]
	public float JumpSpeed
	{
		get => jumpSpeed;
		set => jumpSpeed = value;
	}

	[Export]
    public float Gravity
	{
		get => gravity;
		set => gravity = Mathf.Max(value, 0.0f);
	}

    public override void _PhysicsProcess(double delta)
    {
        if (!Enabled)
            return;

        if (target is null)
            return;
        
        // bitmask better
        bool onFloor   = target.IsOnFloor();
        bool onCeiling = target.IsOnCeiling();
        
        if (onFloor && onCeiling)
        {
            target.Velocity = target.Velocity with { Y = 0 };
        }
        else if (onFloor && !onCeiling)
        {
            if (lastJump)
            {
                target.Velocity = target.Velocity with { Y = -JumpSpeed };
            }
            else
            {
                target.Velocity = target.Velocity with { Y = 0 };
            }
        }
        else if (!onFloor && onCeiling)
        {
            // bug: infinity acceleration when ceiling
            target.Velocity = target.Velocity with { Y = 1 };
        }
        else if (!onFloor && !onCeiling)
        {
            float newY = target.Velocity.Y + gravity * (float)delta;
            target.Velocity = target.Velocity with { Y = newY };
        }

        lastJump = false;
        
        target.Velocity = target.Velocity with { X = horizontalSpeed * horizontalDirection };

        target.MoveAndSlide();
    }

    public void SetHorizontalDirection(float direction)
    {
        horizontalDirection = direction;
    }

    public void Jump()
    {
        if (target is not null && target.IsOnFloor())
        {
            lastJump = true;
        }
    }
}
