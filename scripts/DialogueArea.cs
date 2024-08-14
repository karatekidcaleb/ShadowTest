using Godot;
using System;

public partial class DialogueArea : Area2D
{
	[Export]
	string dialogue_key;
	bool area_active = false;

    public override void _Process(double delta)
    {
        if(area_active && Input.IsActionJustPressed("ui_accept"))
		{
			SignalBus.Instance.EmitSignal("DisplayDialogue", dialogue_key);
		}
    }

    public void OnAreaEntered(Area2D area)
	{
		if(area.Name == "DialogueInteractionArea")
			area_active = true;
	}

	public void OnAreaExited(Area2D area)
	{
		if(area.Name == "DialogueInteractionArea")
			area_active = false;
	}
}
