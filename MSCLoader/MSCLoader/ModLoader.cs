using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MSCLoader
{
	/// <summary>
	/// Main ModLoader class.
	/// </summary>
	public class ModLoader : MonoBehaviour
	{
		/// <summary>
		/// If the ModLoader is done loading or not.
		/// </summary>
		public static bool IsDoneLoading { get; private set; }
		
		/// <summary>
		/// A list of all currently loaded mods.
		/// </summary>
		public static List<Mod> LoadedMods { get; private set; }

		/// <summary>
		/// The instance of ModLoader.
		/// </summary>
		public static ModLoader Instance { get; private set; }


		/// <summary>
		/// The current version of the ModLoader.
		/// </summary>
		public static string Version = "0.1";

		/// <summary>
		/// The folder where all Mods are stored.
		/// </summary>
		public static string ModsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MySummerCar\Mods\";

        /// <summary>
        /// The folder where the config files for Mods are stored.
        /// </summary>
        public static string ConfigFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MySummerCar\Mods\Config\";


		/// <summary>
		/// Initialize the ModLoader
		/// </summary>
		public static void Init()
		{
			if (IsDoneLoading || Instance)
			{
				ModConsole.Error("MSCLoader is already loaded!");
				return;
			}

            // Create game object and attach self
            GameObject go = new GameObject();
			go.AddComponent<ModLoader>();      
            Instance = go.GetComponent<ModLoader>();
			GameObject.DontDestroyOnLoad(go);

			// Init variables
			ModConsole.Print("Loading...");
            IsDoneLoading = false;
			LoadedMods = new List<Mod>();

			// Init mod loader settings
			if (!Directory.Exists(ModsFolder))
			{
				Directory.CreateDirectory(ModsFolder);
			}

			if (!Directory.Exists(ConfigFolder))
			{
				Directory.CreateDirectory(ConfigFolder);
			}

			// Loading internal mods
			ModConsole.Print("Loading internal mods...");
			LoadMod(new ModConsole(), true);
			LoadMod(new ModSettings(), true);

			// Load all mods
			ModConsole.Print("Loading mods...");
			LoadMods();
			ModSettings.LoadBinds();

			// Finished loading
			IsDoneLoading = true;
			ModConsole.Print("Done loading");
		}

		/// <summary>
		/// Load all mods in the "mods" folder.
		/// </summary>
		private static void LoadMods()
		{
			// Load loose .dll files
			foreach (string file in Directory.GetFiles(ModsFolder))
			{
				if (file.EndsWith(".dll"))
				{
					LoadDLL(file);
				}
			}

			// Load subdirectories
			foreach (string dir in Directory.GetDirectories(ModsFolder))
			{
				foreach (string file in Directory.GetFiles(dir))
				{
					// Only load .dll files
					if (file.EndsWith(".dll"))
					{
						LoadDLL(file);
					}
				}
			}
		}

		/// <summary>
		/// Load mod from DLL file.
		/// </summary>
		/// <param name="file">The path of the DLL file to load the Mod from.</param>
		private static void LoadDLL(string file)
		{
			Assembly asm = Assembly.LoadFrom(file);

			// Look through all public classes
			foreach (Type type in asm.GetTypes())
			{
				// Check if class inherits Mod
				if (type.IsSubclassOf(typeof(Mod)))
				{
					LoadMod((Mod)Activator.CreateInstance(type));
				}
			}
		}

		/// <summary>
		/// Load a Mod.
		/// </summary>
		/// <param name="mod">The instance of the Mod to load.</param>
		/// <param name="isInternal">If the Mod is internal or not.</param>
		private static void LoadMod(Mod mod, bool isInternal = false)
		{
			// Check if mod already exists
			if (!LoadedMods.Contains(mod))
			{
				// Generate config files
				if (!Directory.Exists(ConfigFolder + mod.ID))
				{
					Directory.CreateDirectory(ConfigFolder + mod.ID);
				}

				// Load
				mod.OnLoad();
				LoadedMods.Add(mod);

				if (!isInternal)
				{
					ModConsole.Print("Loaded mod: " + mod.ID);
				}
				else
				{
					ModConsole.Print("Loaded internal mod: " + mod.ID);
				}
			}
			else
			{
				ModConsole.Print("Mod already loaded: " + mod.ID);
			}
		}

		/// <summary>
		/// Draw GUI.
		/// </summary>
		private void OnGUI()
		{			
			// Call OnGUI for loaded mods
			foreach (Mod mod in LoadedMods)
			{
				mod.OnGUI();
			}
		}

		/// <summary>
		/// Call Update for all loaded Mods.
		/// </summary>
		private void Update()
		{
			// Call update for loaded mods
			foreach (Mod mod in LoadedMods)
			{
				mod.Update();
			}
		}
	}
}
