using MSCLoader;
using UnityEngine;

namespace ExampleMod
{
    public class ExampleMod : Mod
	{
		// The ID of the mod - Should be unique
		public override string ID { get { return "ExampleMod"; } }

		// The name of the mod that is displayed
		public override string Name { get { return "Example mod"; } }
		
		// The name of the author
		public override string Author { get { return "Djoe45"; } }

		// The version of the mod
		public override string Version { get { return "1.0.0"; } }

		// Keybinds
		private Keybind testKey = new Keybind("KeyID", "Key name", KeyCode.L, KeyCode.LeftControl);

		// Called when the mod is loaded
		public override void OnLoad()
		{
			// Do your initialization here

			Keybind.Add(this, testKey);
			ConsoleCommand.Add(new ExampleCommand());

			ModConsole.Print("ExampleMod has been loaded!");
		}

		// Called to draw the GUI
		public override void OnGUI()
		{
			// Draw your GUI here
		}

		// Called every tick
		public override void Update()
		{
			// Do your updating here

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
