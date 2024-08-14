using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public int Health;
	public float Speed = 200.0f;
	public const float JumpVelocity = -400.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		HandlePlayerMovement();
		MoveAndSlide();
	}

    public override void _Ready()
    {
        this.Health = 100;
    }

    public override void _Process(double delta)
    {
		if(Health <=0)
		{
			Die();
		}
    }

	public void Die()
	{
		GD.Print("ded");
	}

	private void HandlePlayerMovement()
	{
		Vector2 velocity = Velocity;

		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		AnimatedSprite2D anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		anim.Play();
		anim.FlipH = false;
		if (velocity.Length() == 0)
			anim.Animation = "idle";
		else 
		{
			if(velocity.X != 0)
			{
				anim.Animation = Input.IsActionPressed("run") ? "run" : "walk";
				if(velocity.X < 0)
				{
					anim.FlipH = true;
				}
			}
			else 
			{
				if(velocity.Y > 0)
				{
					anim.Animation = Input.IsActionPressed("run") ? "down_run" : "down";
				}
				else if(velocity.Y < 0)
				{
					anim.Animation = Input.IsActionPressed("run") ? "up_run" : "up";
				}
			}
		}
		Speed = Input.IsActionPressed("run") ? 300 : 200;
		velocity = direction * Speed;
		Velocity = velocity;
	}
}
