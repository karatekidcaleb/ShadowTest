using Godot;
using System;

public partial class Level : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		switch(this.Name)
		{
			case "Overworld":
				GD.Print((GetParent() as Main).level_flags);
				break;
			default:
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		for (int i=0; i<GetChildCount(); i++)
		{
			if (GetChild(i) is not TextureRect && GetChild(i) is not CanvasLayer)
			{
				try
				{
					GetChild<Node2D>(i).ZIndex = (int)GetChild<Node2D>(i).Position.Y; 
					// Reorder all sprites in the scene based on their physical positions (height) 
					// i..e player should be able to walk in front of the house and be rendered behind 
					// it when physically walking behind it
				}
				catch(InvalidCastException){	} 
				// One day I'll find a better way to ignore UI items int he scene... but today is not that day
			}
		}
		if(Input.IsActionJustPressed("lighten_shit"))
		{
			GetNode<CanvasModulate>("Darkness").Color = GetNode<CanvasModulate>("Darkness").Color * 1.1f;
		}
	}
}
