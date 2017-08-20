using MSCLoader;
using UnityEngine;

namespace ExampleMod
{
    public class ExampleMod : Mod
	{
		// The ID of the mod - Should be unique
		public override string ID => "ExampleMod";

		// The name of the mod that is displayed
		public override string Name => "Example mod";
		
		// The name of the author
		public override string Author => "Your username";

		// The version of the mod - whatever you want.
		public override string Version => "1.0";

		// Keybinds
		private Keybind testKey = new Keybind("KeyID", "Key name", KeyCode.L, KeyCode.LeftControl);

		// Called when the mod is loaded
		public override void OnLoad()
		{
			Keybind.Add(this, testKey); //register keybind for this class
			ConsoleCommand.Add(new ExampleCommand()); //Add command console from ExampleCommand class

			ModConsole.Print("ExampleMod has been loaded!"); //print debug information
		}

		// Called once every frame
		public override void Update()
		{
			if (testKey.IsDown())
			{
				ModConsole.Print("Key is pressed!");
			}

			if (testKey.IsPressed())
			{
				ModConsole.Print("Key is held!");
			}
		}
	}
}
