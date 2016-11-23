using System;
using System.Linq;
using UnityEngine;
using MSCLoader.Commands;

namespace MSCLoader
{
    /// <summary>
    /// The console for MSCLoader.
    /// </summary>
    public class ModConsole : Mod
	{
		/// <summary>
		/// ID of the mod.
		/// </summary>
		public override string ID { get { return "MSCLoader_Console"; } }

		/// <summary>
		/// Display name of the mod.
		/// </summary>
		public override string Name { get { return "Console"; } }

		/// <summary>
		/// Version of the mod.
		/// </summary>
		public override string Version { get { return ModLoader.Version; } }

		/// <summary>
		/// Author.
		/// </summary>
		public override string Author { get { return "sfoy"; } }


		/// <summary>
		/// If the console is open or not
		/// </summary>
		public static bool IsOpen { get; private set; }

		private static string consoleText = "";
		private static string commandText = "";

		private Rect windowRect = new Rect(40, Screen.height - 500, 680, 480);
		private Vector2 scrollPosition = new Vector2(0, 0);

		private Keybind consoleKey = new Keybind("Open", "Open console", KeyCode.BackQuote);

		/// <summary>
		/// Append a message to console.
		/// </summary>
		/// <param name="obj">Text or object to append to console.</param>
		public static void Print(object obj)
		{
			consoleText += "<color=white>" + obj.ToString() + "</color>" + Environment.NewLine;
		}

		/// <summary>
		/// Append an error to the console.
		/// </summary>
		/// <param name="obj">Text or object to append to error log.</param>
		public static void Error(object obj)
		{
			Print("<color=red>[Error] " + consoleText.ToString() + "</color>");
			IsOpen = true;
		}

		/// <summary>
		/// Clear the console.
		/// </summary>
		public static void Clear()
		{
			consoleText = "";
		}

		/// <summary>
		/// Toggle if the console is open or not.
		/// </summary>
		public static void Toggle()
		{
			IsOpen = !IsOpen;
			commandText = "";
		}

		// Run command
		private void RunCommand()
		{
			string[] parts = commandText.Split(' ');

			// Empty command
			if (string.IsNullOrEmpty(commandText)) { return; }

			// Invalid command
			if (parts.Length < 1)
			{
				Print("<color=red>Invalid command</color>");
				return;
			}

			// Parse command
			foreach (ConsoleCommand cmd in ConsoleCommand.Commands)
			{
				if (cmd.Name == parts[0])
				{
					if (parts.Length >= 2)
					{
						string[] args = (string[])parts.Skip(1);
						cmd.Run(args);
					}
					else
					{
						cmd.Run(new string[] { });
					}

					commandText = "";
					return;
				}
			}

			commandText = "";
			Print("<color=red>Command does not exist</color>");
		}

		// Manage windows
		private void windowManager(int id)
		{
			if (id == 0)
			{
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(600), GUILayout.Height(400));
				GUILayout.TextArea(consoleText, new GUIStyle() { richText = true });
				GUILayout.EndScrollView();

				commandText = GUI.TextField(new Rect(12, windowRect.height - 40, 600, 20), commandText);

				if (GUI.Button(new Rect(625, 30, 40, 40), "X"))
				{
					IsOpen = false;
				}

				if (GUI.Button(new Rect(625, windowRect.height - 50, 40, 40), ">"))
				{
					RunCommand();
					scrollPosition.y = int.MaxValue;
				}

				GUI.DragWindow();
			}
		}

		/// <summary>
		/// Load built-in console commands.
		/// </summary>
		public override void OnLoad()
		{
			IsOpen = false;

			Keybind.Add(this, consoleKey);

			ConsoleCommand.Add(new CommandClear());
			ConsoleCommand.Add(new CommandHelp());
		}

		/// <summary>
		/// Draw console.
		/// </summary>
		public override void OnGUI()
		{
			if (IsOpen)
			{
				windowRect = GUI.Window(0, windowRect, windowManager, "ModLoader Console");
			}
		}

		/// <summary>
		/// Listen for Keybind press.
		/// </summary>
		public override void Update()
		{
			if (consoleKey.IsDown())
			{
				Toggle();
			}
		}
	}
}
