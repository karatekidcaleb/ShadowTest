using Godot;
using System;

public partial class Door : Area2D
{
	[Signal]
	public delegate void ChangeRoomEventHandler(string room);

	[Export]
	public string connected_room;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void OnBodyEntered(Node body)
	{
		GD.Print("hi!");
		if(body.Name == "Beebop")
		{
			this.SetDeferred("Monitoring", false);
			EmitSignal("ChangeRoom", connected_room);
		}
	}
}
