using Godot;
using System;

public partial class Firelight : PointLight2D
{
	private Vector2 starting_position;
	private double flicker_timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		starting_position = this.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.TextureScale = 1.8f + 0.2f * Mathf.Cos(Time.GetTicksMsec() * 0.0058f) + 0.1f * Mathf.Sin(Time.GetTicksMsec() * 0.0031f);
		this.Energy = 0.8f + 0.4f * Mathf.Cos(Time.GetTicksMsec() * 0.0047f) + 0.03f * Mathf.Sin(Time.GetTicksMsec() * 0.0072f);
	}
}
