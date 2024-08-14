using Godot;
using System;

public partial class SignalBus : Node
{
	[Signal]
	public delegate void DisplayDialogueEventHandler(string text_key);

	public static SignalBus Instance { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		this.ProcessMode = Node.ProcessModeEnum.Always;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
