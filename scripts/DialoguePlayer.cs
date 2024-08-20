using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

/******************************************************************************
This class is a C# rework of the very helpful example made by "StevePixelFace"
from his "Dialog System in 10 minutes! Godot Tutorial" on Youtube.

Link: https://www.youtube.com/watch?v=Ur9j3c5_of0

Some modifications made in structure of json file and functionality, as follows:
	• Speech displays one character at a time, utilizing a variable per-character delay
	• Random audio files are triggered for every X character displayed in the box
		- Control node "AudioMixer" must exist with any quantity of AudioStreamPlayer children
		- All children of AudioMixer represent segments of the "voice" which are randomly indexed
	• .NET's Json deserializer refers to a custom class' getters and seters for population, doesn't play nice
******************************************************************************/


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
		private bool player_skip_initiated = false;
		[Export]
		public int char_delay = 30;
		[Export]
		public int voice_delay = 3; // shifts multiplicatively with char delay irrespective of this setting!!!

		public override void _Ready()
		{
			background = GetNode<TextureRect>("Background"); 
			label = GetNode<Label>("TextLabel");
			audio_mixer = GetNode<Control>("AudioMixer");

			background.Visible = false; // Starts hidden in the scene
			scene_text = LoadSceneText(); 
			// Get contents of the json file containing all relevant scene dialogue... may rework to dynamically 
			// choose dialogue available per scene in seperate json files / superstructure within json file

			SignalBus.Instance.Connect("DisplayDialogue", new Callable(this, MethodName.OnDisplayDialogue));
			// Must use .Instance in order to utilize any methods within C# singletons on autoloads. 
			// Not sure if this is specific to Signals or if all methods require this "ambiguous" instantiation
			// I also don't care
		}

		public async void ShowText()
		{
			label.Text = "";
			text_scrolling = true;
			string full_text = selected_text.Pop();
			int current_voice_tick = 3;
			foreach (char c in full_text)
			{
				if(current_voice_tick == voice_delay)
				{
					int randomChild = (int) Mathf.Floor(audio_mixer.GetChildCount() * GD.Randf());
					audio_mixer.GetChild<AudioStreamPlayer>(randomChild).Play();
					current_voice_tick = 0;
				}
				else
				{
					current_voice_tick++;
				}
				label.Text += c;
				await Task.Delay(char_delay);
				if(player_skip_initiated)
				{
					label.Text = full_text;
					player_skip_initiated = false;
					break;
				}
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
			GetParent().ProcessMode = ProcessModeEnum.Inherit; // Unpause the scene
		}
		public void OnDisplayDialogue(string text_key)
		{
			if(!text_scrolling) // Is the text actively scrolling? If so, player clearly wants to skip over the text scroll.
			{
				if(in_progress)
					NextLine();
				else
				{
					GetParent().ProcessMode = ProcessModeEnum.Disabled;
					background.Visible = true;
					in_progress = true;
					selected_text = new Stack<string>((string[])scene_text[text_key].Clone()); 
					// Text blocks currently in use get put on a stack from the json and popped
					ShowText();
				}
			}
			else
			{
				player_skip_initiated = true;
			}
		}
	
		public Dictionary<string,string[]> LoadSceneText()
		{
			// Fixing file path to be relative to .NET standard scope (excluding first six characters "res://")
			// WARNING: Changing export config / culling superfluous  could interfere with scope of resource location
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
