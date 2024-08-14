using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Parsing
{
	public partial class DialoguePlayer : CanvasLayer
	{
		[Export(PropertyHint.GlobalFile, "*.json")]
		public string scene_text_file { get; set; }

		private Dictionary<string, string[]> scene_text;
		private Stack<string> selected_text;
		private bool in_progress = false;
		private TextureRect background;
		private Label label;
		private Control audio_mixer;
		private bool text_scrolling = false;

		public override void _Ready()
		{
			background = GetNode<TextureRect>("Background"); 
			label = GetNode<Label>("TextLabel");
			audio_mixer = GetNode<Control>("AudioMixer");

			background.Visible = false;
			scene_text = LoadSceneText();
			SignalBus.Instance.Connect("DisplayDialogue", new Callable(this, MethodName.OnDisplayDialogue));
		}

		public async void ShowText()
		{
			label.Text = "";
			text_scrolling = true;
			string full_text = selected_text.Pop();
			int voice_delay = 3;
			audio_mixer.GetNode<AudioStreamPlayer>("Snippet1").Play();
			foreach (char c in full_text)
			{
				if(voice_delay == 3)
				{
					audio_mixer.GetChild<AudioStreamPlayer>((int)Mathf.Floor(audio_mixer.GetChildCount()* GD.Randf())).Play();
					voice_delay = 0;
					GD.Print((int)Mathf.Floor((audio_mixer.GetChildCount() + 1)* GD.Randf()));
				}
				else
				{
					voice_delay++;
				}
				label.Text += c;
				GD.Print(label.Text);
				await Task.Delay(30);
			}
			text_scrolling = false;
		}

		public void NextLine()
		{
			if(selected_text.Count > 0)
				ShowText();
			else	
				Finish();
		}

		public void Finish()
		{
			label.Text = "";
			background.Visible = false;
			in_progress = false;
			GetParent().ProcessMode = ProcessModeEnum.Inherit;
		}
		public void OnDisplayDialogue(string text_key)
		{
			if(!text_scrolling)
			{
				GD.Print("yep");
				if(in_progress)
					NextLine();
				else
				{
					GetParent().ProcessMode = ProcessModeEnum.Disabled;
					background.Visible = true;
					in_progress = true;
					selected_text = new Stack<string>((string[])scene_text[text_key].Clone());
					ShowText();
				}
			}
		}
	
		public Dictionary<string,string[]> LoadSceneText()
		{
			//Fixing file path to be relative to .NET standard scope (excluding "res://")
			string jsonString = File.ReadAllText(scene_text_file.Substr(6, scene_text_file.Length));
        	List<DialogueObject> dialogueOptions = JsonSerializer.Deserialize<List<DialogueObject>>(jsonString);
			Dictionary<string, string[]> all_scenes_text = new Dictionary<string,string[]>();
			foreach (var option in dialogueOptions)
			{
				all_scenes_text[option.Key]	= option.Data;
			}
			return all_scenes_text;
		}
	}

	public class DialogueObject
	{
		public string Key { get; set; }
    	public string[] Data { get; set; }
	}
}
