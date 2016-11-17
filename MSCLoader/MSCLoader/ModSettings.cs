using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using UnityEngine;

namespace MSCLoader
{
	/// <summary>
	/// Handles the settings for all Mods.
	/// </summary>
	public class ModSettings : Mod
	{
		/// <summary>
		/// Mod ID.
		/// </summary>
		public override string ID { get { return "PBLoader_Settings"; } }

		/// <summary>
		/// Display name.
		/// </summary>
		public override string Name { get { return "Settings"; } }

		/// <summary>
		/// Version.
		/// </summary>
		public override string Version { get { return ModLoader.Version; } }

		/// <summary>
		/// Author
		/// </summary>
		public override string Author { get { return "sfoy"; } }

		private static bool menuOpen = false;
		private static bool awaitingInput = false;
		private static int awaitingKeyID = -1;
		private static int menuState = 0;
		private static Mod selectedMod = null;
		private static Vector2 scrollPos = new Vector2(0, 0);
		private static Keybind awaitingKey = null;

		private Keybind menuKey = new Keybind("Open", "Open menu", KeyCode.M, KeyCode.LeftControl);

		/// <summary>
		/// Toggles if the settings menu is open or not.
		/// </summary>
		public static void Toggle()
		{
			menuOpen = !menuOpen;
			menuState = 0;
			selectedMod = null;
			scrollPos = Vector2.zero;
		}

		// Manage windows
		private void windowManager(int id)
		{
			if (id == 0)
			{
				if (menuState == 0)
				{
					// Main menu

					scrollPos = GUILayout.BeginScrollView(scrollPos);

					GUILayout.Label("Loaded mods");

					GUILayout.Space(20);

					foreach (Mod mod in ModLoader.LoadedMods)
					{
						if (GUILayout.Button(mod.Name, GUILayout.Height(30)))
						{
							menuState = 1;
							selectedMod = mod;
						}
					}

					GUILayout.EndScrollView();

					GUILayout.Space(20);

					if (GUILayout.Button("Close", GUILayout.Height(30)))
					{
						menuOpen = false;
					}
				}
				else if (menuState == 1)
				{
					// Mod info

					scrollPos = GUILayout.BeginScrollView(scrollPos);

					GUILayout.Space(20);

					if (selectedMod != null)
					{
						GUILayout.Label("ID: " + selectedMod.ID);
						GUILayout.Label("Name: " + selectedMod.Name);
						GUILayout.Label("Version: " + selectedMod.Version);
						GUILayout.Label("Author: " + selectedMod.Author);

						GUILayout.Space(20);

						if (GUILayout.Button("Keybinds", GUILayout.Height(30)))
						{
							menuState = 2;
							scrollPos = Vector2.zero;
						}
					}
					else
					{
						GUILayout.Label("Invalid mod");
					}

					GUILayout.EndScrollView();

					GUILayout.Space(20);

					if (GUILayout.Button("Back", GUILayout.Height(30)))
					{
						menuState = 0;
						selectedMod = null;
						scrollPos = Vector2.zero;
					}
				}
				else if (menuState == 2)
				{
					// Edit keybinds

					GUILayout.Label("Keybinds");
					GUILayout.Space(20);

					scrollPos = GUILayout.BeginScrollView(scrollPos);

					if (selectedMod != null)
					{
						bool none = true;

						foreach (Keybind key in Keybind.Keybinds)
						{
							if (key.Mod == selectedMod)
							{
								GUILayout.Label(key.Name);

								string modStr = key.Modifier.ToString();
								string keyStr = key.Key.ToString();

								if (awaitingInput)
								{
									if (awaitingKeyID == 0)
									{
										modStr = "Press any key";
									}
									else if (awaitingKeyID == 1)
									{
										keyStr = "Press any key";
									}
								}

								if (GUILayout.Button("Modifier - " + modStr, GUILayout.Height(30)))
								{
									if (!awaitingInput)
									{
										awaitingInput = true;
										awaitingKeyID = 0;
										awaitingKey = key;
									}
								}

								if (GUILayout.Button("Key - " + keyStr, GUILayout.Height(30)))
								{
									if (!awaitingInput)
									{
										awaitingInput = true;
										awaitingKeyID = 1;
										awaitingKey = key;
									}
								}

								GUILayout.Space(10);

								none = false;
							}
						}

						if (none)
						{
							GUILayout.Label("This mod has no keybinds");
						}
					}
					else
					{
						GUILayout.Label("Invalid mod");
					}

					GUILayout.EndScrollView();

					GUILayout.Space(20);

					if (GUILayout.Button("Reset", GUILayout.Height(30)))
					{
						resetBinds();
					}

					if (GUILayout.Button("Back", GUILayout.Height(30)))
					{
						menuState = 1;
						scrollPos = Vector2.zero;
					}
				}
			}
		}

		// Reset keybinds
		private void resetBinds()
		{
			if (selectedMod != null)
			{
				// Delete file
				string path = ModLoader.ConfigFolder + selectedMod.ID + "\\keybinds.xml";
				File.WriteAllText(path, "");

				// Revert binds
				foreach (Keybind bind in Keybind.Get(selectedMod))
				{
					Keybind original = Keybind.DefaultKeybinds.Find(x => x.Mod == selectedMod && x.ID == bind.ID);

					if (original != null)
					{
						bind.Key = original.Key;
						bind.Modifier = original.Modifier;

						ModConsole.Print(original.Key.ToString() + " -> " + bind.Key.ToString());
					}
				}

				// Save binds
				SaveModBinds(selectedMod);
			}
		}
		
		/// <summary>
		/// Save all keybinds to config files.
		/// </summary>
		public static void SaveAllBinds()
		{
			foreach (Mod mod in ModLoader.LoadedMods)
			{
				SaveModBinds(mod);
			}
		}

		/// <summary>
		/// Save keybind for a single mod to config file.
		/// </summary>
		/// <param name="mod">The mod to save the config for.</param>
		public static void SaveModBinds(Mod mod)
		{
			string path = ModLoader.ConfigFolder + mod.ID + "\\keybinds.xml";

			// Clear file
			File.WriteAllText(path, "");

			// Write XML
			XmlDocument doc = new XmlDocument();
			XmlElement keybinds = doc.CreateElement(string.Empty, "Keybinds", string.Empty);

			foreach (Keybind bind in Keybind.Get(mod))
			{
				XmlElement keybind = doc.CreateElement(string.Empty, "Keybind", string.Empty);

				XmlElement name = doc.CreateElement(string.Empty, "ID", string.Empty);
				name.AppendChild(doc.CreateTextNode(bind.ID));
				keybind.AppendChild(name);

				XmlElement key = doc.CreateElement(string.Empty, "Key", string.Empty);
				key.AppendChild(doc.CreateTextNode(bind.Key.ToString()));
				keybind.AppendChild(key);

				XmlElement modifier = doc.CreateElement(string.Empty, "Modifier", string.Empty);
				modifier.AppendChild(doc.CreateTextNode(bind.Modifier.ToString()));
				keybind.AppendChild(modifier);

				keybinds.AppendChild(keybind);
			}

			doc.AppendChild(keybinds);
			doc.Save(path);
		}

		/// <summary>
		/// Load all keybinds.
		/// </summary>
		public static void LoadBinds()
		{
			foreach (Mod mod in ModLoader.LoadedMods)
			{
				// Check if there are custom keybinds
				string path = ModLoader.ConfigFolder + mod.ID + "\\keybinds.xml";

				if (!File.Exists(path))
				{
					SaveModBinds(mod);
					continue;
				}
				
				// Load XML
				XmlDocument doc = new XmlDocument();
				doc.Load(path);

				foreach (XmlNode keybind in doc.GetElementsByTagName("Keybind"))
				{
					XmlNode id = keybind.SelectSingleNode("ID");
					XmlNode key = keybind.SelectSingleNode("Key");
					XmlNode modifier = keybind.SelectSingleNode("Modifier");

					// Check if its valid and fetch
					if (id == null || key == null || modifier == null)
					{
						continue;
					}

					Keybind bind = Keybind.Keybinds.Find(x => x.Mod == mod && x.ID == id.InnerText);

					if (bind == null)
					{
						continue;
					}

					// Set bind
					try
					{
						KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), key.InnerText);
						bind.Key = code;
					}
					catch (Exception e)
					{
						bind.Key = KeyCode.None;
					}

					try
					{
						KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), modifier.InnerText);
						bind.Modifier = code;
					}
					catch (Exception e)
					{
						bind.Modifier = KeyCode.None;
					}
				}
			}
		}

		/// <summary>
		/// Load the keybinds.
		/// </summary>
		public override void OnLoad()
		{
			Keybind.Add(this, menuKey);
		}

		/// <summary>
		/// Draw menu.
		/// </summary>
		public override void OnGUI()
		{
			if (menuOpen)
			{
				GUI.Window(0, new Rect(Screen.width / 2 - 200, Screen.height / 2 - 300, 400, 600), windowManager, "ModLoader Settings");
			}
		}

		/// <summary>
		/// Open menu if the key is pressed.
		/// </summary>
		public override void Update()
		{
			// Open menu
			if (menuKey.IsDown())
			{
				Toggle();
			}

			// Rebinds
			if (awaitingInput)
			{
				// Cancel rebind
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					if (awaitingKeyID == 0)
					{
						awaitingKey.Modifier = KeyCode.None;
					}
					else if (awaitingKeyID == 1)
					{
						awaitingKey.Key = KeyCode.None;
					}

					awaitingInput = false;
					awaitingKeyID = -1;
					awaitingKey = null;
				}

				if (Input.anyKeyDown)
				{
					if (awaitingKeyID == 0)
					{
						if (Event.current.keyCode != KeyCode.None)
						{
							awaitingKey.Modifier = Event.current.keyCode;
						}
						else
						{
							if (Event.current.shift)
							{
								awaitingKey.Modifier = KeyCode.LeftShift;
							}
						}
					}
					else if (awaitingKeyID == 1)
					{
						string input = Input.inputString.ToUpper();
						KeyCode code = KeyCode.None;

						try
						{
							code = (KeyCode)Enum.Parse(typeof(KeyCode), input);
						}
						catch (Exception e)
						{
							if (input == "`")
							{
								code = KeyCode.BackQuote;
							}
							else
							{
								ModConsole.Error("Invalid key");
							}
						}

						awaitingKey.Key = code;
					}

					awaitingInput = false;
					awaitingKey = null;
					awaitingKeyID = -1;
				}
			}
		}
	}
}
