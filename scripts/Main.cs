using Godot;
using System;

public partial class Main : Node2D
{
	private PackedScene level_loader;
	private Level level;
	public override void _Ready()
	{
		LoadScene("Overworld");
		ConnectDoors();
	}

	public void LoadScene(string level_name)
	{
		PackedScene level_loader = GD.Load<PackedScene>("res://scenes/Overworld.tscn");
		level = (Level)level_loader.Instantiate();
		AddChild(level);
	}

	public void ConnectDoors()
	{
		for(int i=0; i<level.GetChildCount(); i++)
		{
			if(level.GetChild(i) is Door)
			{
				level.GetChild<Door>(i).ChangeRoom += (room_id) => OnChangeRoom(room_id);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnChangeRoom(string room)
	{
		LoadScene(room);
		ConnectDoors();
	}
}
