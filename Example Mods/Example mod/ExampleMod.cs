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
		private Keybind testKey;

		public override void ModSetup()
		{
			SetupFunction(Setup.OnLoad, Mod_OnLoad);
			SetupFunction(Setup.Update, Mod_Update);
		}

		// Called when the mod is loaded
		void Mod_OnLoad()
		{
			testKey = Keybind.Add(this, "KeyID", "Key name", KeyCode.L, KeyCode.LeftControl); //register keybind for this class
			ConsoleCommand.Add(new ExampleCommand()); //Add command console from ExampleCommand class

			ModConsole.Print("ExampleMod has been loaded!"); //print debug information
		}

		// Called once every frame
		void Mod_Update()
		{
			if (testKey.GetKeybindDown())
			{
				ModConsole.Print("Key is pressed!");
			}

			if (testKey.GetKeybindUp())
			{
				ModConsole.Print("Key is released!");
			}
		}
	}
}
