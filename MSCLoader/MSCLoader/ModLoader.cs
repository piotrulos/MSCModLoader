using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

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
        public static bool IsModsDoneLoading { get; private set; }
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
		public static string Version = "0.2";

		/// <summary>
		/// The folder where all Mods are stored.
		/// </summary>
		public static string ModsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
   
        /// <summary>
        /// The folder where the config files for Mods are stored.
        /// </summary>
        public static string ConfigFolder = Path.Combine(ModsFolder, @"Config\");

        /// <summary>
        /// Initialize with Mods folder in My Documents (like in 0.1)
        /// </summary>
        public static void Init_MD()
        {
            ModsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
            Init(); //Main init
        }
        /// <summary>
        /// Initialize with Mods folder in Game Folder (near game's exe)
        /// </summary>
        public static void Init_GF()
        {
            ModsFolder = Path.GetFullPath(Path.Combine("Mods", ""));
            Init(); //Main init
        }
        /// <summary>
        /// Initialize with Mods folder in AppData/LocalLow (near game's save)
        /// </summary>
        public static void Init_AD()
        {
            ModsFolder = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"..\LocalLow\Amistech\My Summer Car\Mods"));
            Init(); //Main init
        }

        /// <summary>
        /// Initialize the ModLoader
        /// </summary>
        public static void Init()
		{
            //Set config folder in mods folder
            ConfigFolder = Path.Combine(ModsFolder, @"Config\");
            //if mods not loaded and game is loaded.
            if (!IsModsDoneLoading && Application.loadedLevelName == "GAME")
            {
                // Load all mods
                ModConsole.Print("Loading mods...");
                try
                {
                    LoadMods();
                    ModSettings.LoadBinds();
                    IsModsDoneLoading = true;
                    ModConsole.Print("Loading mods complete!");
                }
                catch (Exception e)
                {
                    IsModsDoneLoading = true; //do not load failed mod forever
                    ModConsole.Error("Loading mods failed:");
                    ModConsole.Error(e.Message);
                }
            }

            if(IsDoneLoading && Application.loadedLevelName == "MainMenu")
            {
                MainMenuInfo();
            }

            if (IsDoneLoading || Instance)
            {   if(Application.loadedLevelName != "MainMenu")
                    Destroy(GameObject.Find("MSCLoader Info")); //remove top left info in game.
                if (Application.loadedLevelName != "GAME")
                    ModConsole.Print("MSCLoader is already loaded!");
            }
            else
            {
                // Create game object and attach self
                GameObject go = new GameObject();
                go.name = "MSCModLoader";
                go.AddComponent<ModLoader>();
                Instance = go.GetComponent<ModLoader>();
                DontDestroyOnLoad(go);

                // Init variables
                ModUI.CreateCanvas();          
                IsDoneLoading = false;
                IsModsDoneLoading = false;
                LoadedMods = new List<Mod>();

                // Init mod loader settings
                if (!Directory.Exists(ModsFolder))
                {
                    //if mods folder not exists, create it.
                    Directory.CreateDirectory(ModsFolder);
                }

                if (!Directory.Exists(ConfigFolder))
                {
                    //if config folder not exists, create it.
                    Directory.CreateDirectory(ConfigFolder);
                }

                // Loading internal tools (console and settings)
                LoadMod(new ModConsole(), true);
                LoadMod(new ModSettings(), true);
                ModSettings.LoadBinds();
                MainMenuInfo(); //show info in main menu.
                IsDoneLoading = true;
                ModConsole.Print("Loading internal tools complete!");
                ModConsole.Print(string.Format("<color=green>ModLoader <b>v{0}</b> ready</color>", Version));
                
            }
		}

        /// <summary>
		/// Print information about ModLoader in MainMenu scene.
		/// </summary>
        private static void MainMenuInfo(bool destroy = false)
        {
                //Create parent gameobject in canvas for layout and text information.
                GameObject modInfo = ModUI.CreateUIBase("MSCLoader Info", GameObject.Find("MSCLoader Canvas"));
                modInfo.AddComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
                modInfo.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                modInfo.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                modInfo.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

                ModUI.CreateTextBlock("MSCLoader Info Text", string.Format("Mod Loader MCSLoader v{0} is ready!", Version), modInfo, TextAnchor.MiddleLeft, Color.white, true).GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;              
                ModUI.CreateTextBlock("MSCLoader Mod location Text", string.Format("Mods folder: {0}", ModsFolder), modInfo, TextAnchor.MiddleLeft, Color.white, true).GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        }

        /// <summary>
        /// Load all mods from "mods" folder.
        /// </summary>
        private static void LoadMods()
		{
			// Load .dll files
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
					ModConsole.Print(string.Format("<color=lime><b>Mod Loaded:</b></color><color=orange><b>{0}</b></color>", mod.ID));
				}
				else
				{
					//ModConsole.Print("Loaded internal mod: " + mod.ID); //debug
				}
			}
			else
			{
				ModConsole.Print(string.Format("<color=orange><b>Mod already loaded (or duplicated ID):</b></color><color=red><b>{0}</b></color>", mod.ID));
			}
		}

		/// <summary>
		/// Draw obsolete OnGUI.
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
