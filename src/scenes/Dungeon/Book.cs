using Contra.Src.Entities;
using Godot;

public partial class Book : Sprite2D
{
	public override void _Ready()
	{
		_self.BodyEntered += OnBodyEntered;
		_initialY = Position.Y;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is not Entity2D entity)
			return;

		if (entity == _target)
		{
			_isCollecting = true;
			_particles2D.Emitting = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_isCollecting)
		{
			_time += (float)delta;
			Position = Position with { Y = _initialY + Mathf.Sin(_time * _speed) * _amplitude };
		}
		else
		{
			Color color = Modulate;
			color.A = Mathf.MoveToward(color.A, 0.0f, (float)delta * 2);
			Modulate = color;
			_pointLight2D.Energy = Mathf.MoveToward(_pointLight2D.Energy, 0.0f, (float)delta * 2);
			if (color.A <= 0)
			{
				QueueFree();
			}
		}
	}

// VARIABLES
	[Export] private Area2D _self;
	[Export] private Entity2D _target;
	[Export] private float _amplitude;
	[Export] private float _speed;
	[Export] private GpuParticles2D _particles2D;
	[Export] private PointLight2D _pointLight2D;
			 private float _time = 0.0f;
			 private float _initialY;
			 private bool _isCollecting = false;
}
