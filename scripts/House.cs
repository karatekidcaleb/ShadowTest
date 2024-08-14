using Godot;
using System;

public partial class House : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnAreaEntered(Area2D area)
	{
		if(area.GetParent().Name == "Beebop")
		{
			this.GetNode<AnimatedSprite2D>("Sprite2D").Animation = "open";
			this.GetNode<AudioStreamPlayer>("OpenDoor").Play();
		}
	}

	public void OnAreaExited(Area2D area)
	{
		if(area.GetParent().Name == "Beebop")
		{
			this.GetNode<AnimatedSprite2D>("Sprite2D").Animation = "closed";
			this.GetNode<AudioStreamPlayer>("CloseDoor").Play();
		}
	}
}
