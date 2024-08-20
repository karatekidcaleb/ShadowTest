using Godot;
using System;
using System.Threading.Tasks;

public partial class Transition : AnimatedSprite2D
{
	private bool growing = false;
	private bool timer_signal_received = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		Scale = new Vector2(10, 10);
		this.ZIndex = 1000;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public async void RoomTransition(string which)
	{
		timer_signal_received = false;
		this.GetNode<Timer>("Timer").Start();
		if(which == "shrink")
		{
			this.GetNode<AudioStreamPlayer>("Enter").Play();
			this.Animation = "default";
			Visible = true;
			Scale = new Vector2(20.0f, 20.0f);
			this.Position = GetParent<Main>().level.GetNode<Player>("Beebop").Position;
			this.Position = new Vector2(this.Position.X, this.Position.Y);
			GD.Print("shrinkin now" + this.Scale.Length() + " : " + timer_signal_received);
			while(!timer_signal_received)
			{
				if(Scale.X > 1.5f)
					Scale = new Vector2(Scale.X*0.97f, Scale.Y*0.97f);
				else
					this.Animation = "black";
				await Task.Delay(3);
			}
			timer_signal_received = false;
		}
		else
		{
			this.GetNode<AudioStreamPlayer>("Exit").Play();
			timer_signal_received = false;
			GD.Print("growing now" + this.Scale.Length() + " : " + timer_signal_received);
			this.Animation = "default";
			while(!timer_signal_received)
			{
				this.Position = GetParent<Main>().level.GetNode<Player>("Beebop").Position;
				this.Position = new Vector2(this.Position.X, this.Position.Y);
				if(Scale.X < 80.0f)
					Scale = new Vector2(Scale.X*1.04f, Scale.Y*1.04f);
				else
				{
					this.Visible = false;
				}
				await Task.Delay(3);
			}
			Visible = false;
			timer_signal_received = false;
		}
		
	}

	public void OnTimerTimeout()
	{
		GD.Print("timer timed out");
		this.timer_signal_received = true;
	}
}
