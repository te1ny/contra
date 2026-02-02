namespace Contra.Src.Entities.Player;

using Contra.Src.Components.Move;
using Godot;

[GlobalClass]
public partial class DashAbility2D : Node2D
{
// FUNCTIONS
    public override void _PhysicsProcess(double delta)
    {
        if (!Input.IsActionJustPressed("shift"))
            return;

        if (Mathf.IsZeroApprox(_target.Velocity.X))
            return;

        //if (_target.Velocity.Y > 0)
        //    return;

        // now we have horizontal velocity and press shift
        // its dash condition

        _sprite.AnimationFinished += OnAnimationFinished;
        _sprite.AnimationChanged  += OnAnimationChanged; 

        Vector2 direction = _moveComponent.LastDirection;
        _flip = direction.X < 0;

        var test = new PhysicsTestMotionParameters2D
        {
            From   = _target.GlobalTransform,
            Motion = _dashDistance * direction
        };

        var result = new PhysicsTestMotionResult2D();
        PhysicsServer2D.BodyTestMotion(_target.GetRid(), test, result);
        _lastPosition = _target.GlobalPosition + result.GetTravel();

        _isBackward = false;
        _sprite.Play(_dashAnimation, _dashAnimationSpeed, _isBackward);

        SetPhysicsProcess(false);
    }

    public async void OnAnimationChanged()
    {
        if (_sprite.Animation == _dashAnimation)
        {
            _moveComponent.Enabled = false;
            _moveComponent.Velocity = new(0, 0);
        }
        if (_sprite.Animation == _animationAfterDash)
        {
            _moveComponent.Enabled = true;
            _sprite.AnimationFinished -= OnAnimationFinished;
            _sprite.AnimationChanged  -= OnAnimationChanged; 
        }
    }

    public async void OnAnimationFinished()
    {
        if (_sprite.Animation != _dashAnimation)
            return;

        if (_isBackward)
        {
            _sprite.Play(_animationAfterDash);
            SetPhysicsProcess(true);
        }
        else
        {
            _sprite.FlipH = _flip;
            _target.GlobalPosition = _lastPosition;
            _isBackward = true;
            _sprite.Play(_dashAnimation, -_dashAnimationSpeed, _isBackward);
        }
    }

// VARIABLES
    [Export] private float            _dashDistance = 300;

    [Export] private AnimatedSprite2D _sprite;
    [Export] private StringName       _dashAnimation;
    [Export] private float            _dashAnimationSpeed;
    [Export] private StringName       _animationAfterDash;
             private bool             _isBackward   = false;
             private bool             _flip         = false;

    [Export] private Entity2D         _target;
    [Export] private MoveComponent2D  _moveComponent;
             private Vector2          _lastPosition = new();

}
