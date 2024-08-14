using Godot;
using System;

public partial class Tree : Node2D
{
	Sprite2D sprite;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		string which = GD.Randf() > 0.5f ? "1" : "2";
		sprite.Texture = GD.Load<Texture2D>("res://assets/sprites/environment/tree" + which + ".png");
		sprite.Scale = new Vector2(3.5f - 1.5f * GD.Randf(), 3.5f - 1.5f * GD.Randf());
		GetNode<LightOccluder2D>("LightOccluder2D").Position = new Vector2(
			GetNode<LightOccluder2D>("LightOccluder2D").Position.X,
			GetNode<LightOccluder2D>("LightOccluder2D").Position.Y + 35.0f * (sprite.Scale.Y - 2.75f)
		);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
