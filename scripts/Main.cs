using Godot;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

public partial class Main : Node2D
{
	private PackedScene level_loader;
	public Level level;
	public int[,] level_flags;// Used to initialize levels 
	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
		LoadScene("Overworld");
		level.GetNode<Area2D>("DoorToHouse").Connect("ChangeRoom", new Callable(this, MethodName.OnChangeRoom));
        level_flags = new int[2, 2] { {0, 0}, {0, 0} };
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
		if(Input.IsActionJustPressed("ui_cancel"))
		{
			GetTree().Quit();
		}
		Render(delta);
	}

	public void Render(double delta)
	{
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

	public async void OnChangeRoom(string room)
	{
		this.GetNode<Transition>("Transition").RoomTransition("shrink");
		await Task.Delay(700);
		GetChild(GetChildCount()-1).QueueFree();
		CallDeferred("LoadScene", room);
		CallDeferred("ConnectDoors");
		this.GetNode<Transition>("Transition").RoomTransition("grow");
		await Task.Delay(700);
	}
}
