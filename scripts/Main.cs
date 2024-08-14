using Godot;
using System;

public partial class Main : Node2D
{
	private PackedScene level_loader;
	private Level level;
	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
		LoadScene("Overworld");
		level.GetNode<Area2D>("DoorToHouse").Connect("ChangeRoom", new Callable(this, MethodName.OnChangeRoom));
	}

	public void LoadScene(string level_name)
	{
		PackedScene level_loader = GD.Load<PackedScene>("res://scenes/" + level_name +".tscn");
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
		Render(delta);
		if(Input.IsActionJustPressed("ui_cancel"))
		{
			GetTree().Quit();
		}
	}

	public void Render(double delta)
	{
		for (int i=0; i<GetChildCount(); i++)
		{
			if(GetChild(i) is not TextureRect)
				GetChild<Node2D>(i).ZIndex = (int)GetChild<Node2D>(i).Position.Y;
		}
		if(level.Name == "Overworld")
		{
			level.GetNode<Camera2D>("Camera").Transform = level.GetNode<CharacterBody2D>("Beebop").Transform;
		}
		if(level.Name == "Inside")
		{
			PointLight2D light = level.GetNode<TextureRect>("Background").GetNode<PointLight2D>("Firelight");
			light.TextureScale = (float)Mathf.Cos(delta * 0.08f) * 0.8f + 1.8f;
			light.Energy = (float)Mathf.Sin(delta * 0.08f) * 0.9f + 0.8f;
		}
	}

	public void OnChangeRoom(string room)
	{
		GD.Print("Caught!");
		GetChild(GetChildCount()-1).QueueFree();
		CallDeferred("LoadScene", room);
		CallDeferred("ConnectDoors");
	}
}
