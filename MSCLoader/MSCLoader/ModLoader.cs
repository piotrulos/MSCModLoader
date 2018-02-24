using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    /// <summary>
    /// This is main Mod Loader class.
    /// </summary>
    public class ModLoader : MonoBehaviour
	{
        /// <summary>
        /// When true, it prints all errors from Update() and OnGUI() class.
        /// </summary>
        public static bool LogAllErrors = false;
 
        /// <summary>
        /// When true, modLoader is ready.
        /// </summary>
        public static bool IsDoneLoading = false;

        /// <summary>
        /// When true, all mods is loaded.
        /// </summary>
        public static bool IsModsDoneLoading = false;
   
        /// <summary>
        /// When true, game scene is fully loaded.
        /// </summary>
        public static bool fullyLoaded = false;

        /// <summary>
        /// A list of all loaded mods.
        /// </summary>
        public static List<Mod> LoadedMods;
        static string mods;
        static string mods_ver;
        /// <summary>
        /// A list of invalid mod files (like random dll in Mods Folder that isn't a mod).
        /// </summary>
        public static List<string> InvalidMods;
        /// <summary>
        /// The instance of ModLoader.
        /// </summary>
        public static ModLoader Instance;

        /// <summary>
        /// The instance of MSCUnloader.
        /// </summary>
        public static MSCUnloader MSCUnloaderInstance;

        /// <summary>
        /// The instance of LoadAssets.
        /// </summary>
        public static LoadAssets loadAssets;

        /// <summary>
        /// The current version of the ModLoader.
        /// </summary>
        public static readonly string Version = "0.3.5";


        /// <summary>
        /// non-public field, please use <c>GetModConfigFolder</c> or <c>GetModAssetsFolder</c> instead, to keep mods folder clean.
        /// </summary>
        static string ModsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
        static string ConfigFolder = Path.Combine(ModsFolder, @"Config\");
        static string AssetsFolder = Path.Combine(ModsFolder, @"Assets\");

        static bool experimental = true; //Is this build is experimental
        static bool modStats = false;
        static GameObject mainMenuInfo;
        static Animator menuInfoAnim;
        
        static int numOfUpdates;
        static bool isModUpdates = false;
        /// <summary>
        /// Get user steamID (read-only)
        /// </summary>
        public static string steamID { get; private set; }

        /// <summary>
        /// Mod config folder, use this if you want save something. 
        /// </summary>
        /// <returns>Path to your mod config folder</returns>
        /// <param name="mod">Your mod Class.</param>
        /// <example>Example Code in Mod subclass.
        /// <code source="Examples.cs" region="GetModConfigFolder" lang="C#" />
        /// Example from other than Mod subclass.
        /// <code source="Examples.cs" region="GetModConfigFolder2" lang="C#" />
        /// </example>
        public static string GetModConfigFolder(Mod mod)
        {
            return Path.Combine(ConfigFolder, mod.ID);
        }

        /// <summary>
        /// Mod assets folder, use this if you want load custom content. 
        /// </summary>
        /// <returns>Path to your mod assets folder</returns>
        /// <param name="mod">Your mod Class.</param>
        /// <example>Example Code in Mod subclass.
        /// <code source="Examples.cs" region="GetModAssetsFolder" lang="C#" />
        /// Example from other than Mod subclass.
        /// <code source="Examples.cs" region="GetModAssetsFolder2" lang="C#" />
        /// </example> 
        public static string GetModAssetsFolder(Mod mod)
        {
            if (mod.UseAssetsFolder == false)
                ModConsole.Error(string.Format("<b>{0}:</b> Please set variable <b>UseAssetsFolder</b> to <b>true</b>",mod.ID));
            return Path.Combine(AssetsFolder, mod.ID);
        }
        /// <summary>
        /// Initialize ModLoader with Mods folder in My Documents (like it was in 0.1)
        /// </summary>
        public static void Init_MD()
        {
            ModsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
            Init(); //Main init
        }
        /// <summary>
        /// Initialize ModLoader with Mods folder in Game Folder (near game's exe)
        /// </summary>
        public static void Init_GF()
        {
            ModsFolder = Path.GetFullPath(Path.Combine("Mods", ""));
            Init(); //Main init
        }
        /// <summary>
        /// Initialize ModLoader with Mods folder in AppData/LocalLow (near game's save)
        /// </summary>
        public static void Init_AD()
        {
            ModsFolder = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"..\LocalLow\Amistech\My Summer Car\Mods"));
            Init(); //Main init
        }

        /// <summary>
        /// GUISkin for OnGUI()
        /// </summary>
        public static GUISkin guiskin;


        /// <summary>
        /// Main function to initialize the ModLoader
        /// </summary>
        public static void Init()
        {
            //Set config and Assets folder in selected mods folder
            ConfigFolder = Path.Combine(ModsFolder, @"Config\");
            AssetsFolder = Path.Combine(ModsFolder, @"Assets\");
            //if mods not loaded and game is loaded.
            if (GameObject.Find("MSCUnloader") == null)
            {
                GameObject go = new GameObject();
                go.name = "MSCUnloader";
                go.AddComponent<MSCUnloader>();
                MSCUnloaderInstance = go.GetComponent<MSCUnloader>();
                DontDestroyOnLoad(go);
            }
            if (IsModsDoneLoading && Application.loadedLevelName == "MainMenu")
            {
                MSCUnloaderInstance.reset = false;
                MSCUnloaderInstance.MSCLoaderReset();
            }
            if (!IsModsDoneLoading && Application.loadedLevelName == "GAME" && fullyLoaded)
            {
                // Load all mods
                ModConsole.Print("Loading mods...");
                Stopwatch s = new Stopwatch();
                s.Start();
                LoadMods();
                ModSettings.LoadBinds();
                IsModsDoneLoading = true;
                s.Stop();
                if (s.ElapsedMilliseconds < 1000)
                    ModConsole.Print(string.Format("Loading mods completed in {0}ms!", s.ElapsedMilliseconds));
                else
                    ModConsole.Print(string.Format("Loading mods completed in {0} sec(s)!", s.Elapsed.Seconds));
            }

            if (IsDoneLoading && Application.loadedLevelName == "MainMenu" && GameObject.Find("MSCLoader Info") == null)
            {
                MainMenuInfo();
            }

            if (IsDoneLoading || Instance)
            {
                if (Application.loadedLevelName != "MainMenu")
                    menuInfoAnim.SetBool("isHidden", true);
                //if (Application.loadedLevelName != "GAME")
                //    ModConsole.Print("<color=#505050ff>MSCLoader is already loaded!</color>");//debug
            }
            else
            {
                // Create game object and attach self
                GameObject go = new GameObject();
                go.name = "MSCModLoader";
                go.AddComponent<ModLoader>();
                go.AddComponent<LoadAssets>();
                Instance = go.GetComponent<ModLoader>();
                loadAssets = go.GetComponent<LoadAssets>();
                DontDestroyOnLoad(go);

                // Init variables
                ModUI.CreateCanvas();
                IsDoneLoading = false;
                IsModsDoneLoading = false;
                LoadedMods = new List<Mod>();
                InvalidMods = new List<string>();

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

                if (!Directory.Exists(AssetsFolder))
                {
                    //if config folder not exists, create it.
                    Directory.CreateDirectory(AssetsFolder);
                }
                // Loading internal tools (console and settings)
                LoadMod(new ModConsole(), Version);
                LoadMod(new ModSettings(), Version);
                LoadCoreAssets();
                IsDoneLoading = true;
                ModConsole.Print(string.Format("<color=green>ModLoader <b>v{0}</b> ready</color>", Version));
                LoadReferences();
                PreLoadMods();
                ModConsole.Print(string.Format("<color=orange>Found <color=green><b>{0}</b></color> mods!</color>", LoadedMods.Count - 2));
                try
                {
                    Steamworks.SteamAPI.Init();
                    steamID = Steamworks.SteamUser.GetSteamID().ToString();
                    ModConsole.Print(string.Format("<color=orange>Hello <color=green><b>{0}</b></color>!</color>", Steamworks.SteamFriends.GetPersonaName()));
                    if (!modStats)
                    {
                        ModStats();
                        modStats = true;
                    }

                    string Name;
                    bool ret = Steamworks.SteamApps.GetCurrentBetaName(out Name, 128);
                    if (ret)
                    {
                        ModUI.ShowMessage(string.Format("<color=orange><b>Warning:</b></color>{1}You are using beta build: <color=orange><b>{0}</b></color>{1}{1}Remember that some mods may not work correctly on beta branches.", Name, Environment.NewLine),"Experimental build warning");
                    }
                }
                catch (Exception e)
                {
                    ModConsole.Error("Steam not detected, only steam version is supported.");
                }
                
            }
        }

        static void LoadReferences()
        {
            if (Directory.Exists(Path.Combine(ModsFolder, "References")))
            {
                string[] files = Directory.GetFiles(Path.Combine(ModsFolder, "References"), "*.dll");
                foreach (var file in files)
                {
                    Assembly.LoadFrom(file);
                }
            }
        }
        static void LoadCoreAssets()
        {
            ModConsole.Print("Loading core assets...");
            AssetBundle ab = LoadAssets.LoadBundle(new ModCore(), "core.unity3d");
            guiskin = ab.LoadAsset("MSCLoader.guiskin") as GUISkin;
            ModUI.messageBox = ab.LoadAsset("MSCLoader MB.prefab") as GameObject;
            mainMenuInfo = ab.LoadAsset("MSCLoader Info.prefab") as GameObject;
            ModConsole.Print("Loading core assets completed!");
            MainMenuInfo(); //show info in main menu.
            ab.Unload(false); //freeup memory
        }

        /// <summary>
		/// Prints information about ModLoader in MainMenu scene.
		/// </summary>
        private static void MainMenuInfo()
        {
            Text info, mf, modUpdates;
            mainMenuInfo = Instantiate(mainMenuInfo);
            mainMenuInfo.name = "MSCLoader Info";
            menuInfoAnim = mainMenuInfo.GetComponent<Animator>();
            menuInfoAnim.SetBool("isHidden", false);
            info = mainMenuInfo.transform.GetChild(0).gameObject.GetComponent<Text>();
            mf = mainMenuInfo.transform.GetChild(1).gameObject.GetComponent<Text>();
            modUpdates = mainMenuInfo.transform.GetChild(2).gameObject.GetComponent<Text>();

            //check if new version is available
            if (!experimental)
            {
                try
                {
                    string version;
                    using (WebClient client = new WebClient())
                    {
                        client.QueryString.Add("core", "stable");
                        version = client.DownloadString("http://my-summer-car.ml/ver.php");                     
                    }
                    if (version.Trim().Length > 8)
                        throw new Exception("Parse Error, please report that problem!");
                    int i = Version.CompareTo(version.Trim());
                    if (i != 0)
                        info.text = string.Format("Mod Loader MSCLoader v{0} is ready! (<color=orange>New version available: <b>v{1}</b></color>)", Version, version.Trim());
                    else if (i == 0)
                        info.text = string.Format("Mod Loader MSCLoader v{0} is ready! (<color=lime>Up to date</color>)", Version);
                }
                catch (Exception e)
                {
                    ModConsole.Error(string.Format("Check for new version failed with error: {0}", e.Message));
                    info.text = string.Format("Mod Loader MSCLoader v{0} is ready!", Version);
                }
            }
            else
            {
                info.text = string.Format("Mod Loader MSCLoader v{0} is ready! (<color=magenta>Experimental</color>)", Version);
            }
            mf.text = string.Format("Mods folder: {0}", ModsFolder);
            if (isModUpdates)
                modUpdates.text = string.Format("<color=lime><b>{0}</b></color> <color=orange>mods has a new version available!</color>", numOfUpdates);
            else
                modUpdates.text = string.Empty;
            mainMenuInfo.transform.SetParent(GameObject.Find("MSCLoader Canvas").transform, false);
        }

        /// <summary>
        /// Load mods, this will call OnLoad() for all loaded mods.
        /// </summary>
        private static void LoadMods()
        {
            ModConsole.Print("<color=#505050ff>");
            // Load Mods
            foreach (Mod mod in LoadedMods)
            {
                try
                {
                    if (!mod.isDisabled)
                    {
                        mod.OnLoad();
                        FsmHook.FsmInject(GameObject.Find("ITEMS"), "Save game", mod.OnSave);
                    }
                }
                catch (Exception e)
                {
                    var st = new StackTrace(e, true);
                    var frame = st.GetFrame(0);

                    string errorDetails = string.Format("{2}<b>Details: </b>{0} in <b>{1}</b>", e.Message, frame.GetMethod(), Environment.NewLine);
                    ModConsole.Error(string.Format("Mod <b>{0}</b> throw an error!{1}", mod.ID, errorDetails));
                }
            }
            ModConsole.Print("</color>");
        }

        /// <summary>
        /// Load all mods from "mods" folder, but don't call OnLoad()
        /// </summary>
        private static void PreLoadMods()
		{
			// Load .dll files
			foreach (string file in Directory.GetFiles(ModsFolder))
			{
				if (file.EndsWith(".dll"))
				{
					LoadDLL(file);
				}
			}

            foreach (string dir in Directory.GetDirectories(ConfigFolder))
            {
                if (!LoadedMods.Exists(x => x.ID == new DirectoryInfo(dir).Name))
                    Directory.Delete(dir, true);
            }
            foreach (string dir in Directory.GetDirectories(AssetsFolder))
            {
                if (!LoadedMods.Exists(x => x.ID == new DirectoryInfo(dir).Name) && new DirectoryInfo(dir).Name != "MSCLoader_Core")
                    Directory.Delete(dir, true);
            }

        }

        static void ModStats()
        {
            if (LoadedMods.Count - 2 > 0)
            {
                numOfUpdates = 0;
                mods = string.Join(",", LoadedMods.Select(s => s.ID).Where(x => !x.StartsWith("MSCLoader_")).ToArray());
                mods_ver = string.Join(",", LoadedMods.Where(x => !x.ID.StartsWith("MSCLoader_")).Select(s => s.Version).ToArray());
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.QueryString.Add("sid", steamID);
                    webClient.QueryString.Add("mods", mods);
                    webClient.QueryString.Add("ver", Version);
                    webClient.QueryString.Add("mods_ver", mods_ver);
                    string result = webClient.DownloadString("http://my-summer-car.ml/mody.php");
                    //string result = webClient.DownloadString("http://localhost/msc/mody.php");
                    if (result != string.Empty)
                    {
                        if (result == "error")
                            throw new Exception("Error");
                        string[] lines = result.Split('\n');
                        for (int i = 0; i < lines.Length; i++)
                        {
                            string[] values = lines[i].Trim().Split(',');
                            foreach (Mod mod in LoadedMods)
                            {
                                if (mod.ID == values[0])
                                {
                                    int v = mod.Version.CompareTo(values[1].Trim());
                                    if (v != 0)
                                    {
                                        mod.hasUpdate = true;
                                        isModUpdates = true;
                                        numOfUpdates++;
                                    }
                                    if (values[2] == "1")
                                    {
                                        mod.isDisabled = true;
                                        ModConsole.Error(string.Format("Mod <b>{0}</b> has been disabled. Reason: {1}", mod.Name, values[3]));
                                    }
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    ModConsole.Error(string.Format("Connection to server failed: {0}", e.Message));
                }
            }
        }

        private static void LoadDLL(string file)
        {
            try
            {
                Assembly asm = null;

                //STILL TESTING - Loading for proper debug
                asm = Assembly.LoadFrom(file);               
                bool isMod = false;

                AssemblyName[] list = asm.GetReferencedAssemblies();
      
                // Look through all public classes                
                foreach (Type type in asm.GetTypes())
                {
                    string msVer = null;
                    // Check if class inherits Mod
                    //if (type.IsSubclassOf(typeof(Mod)))
                    if (typeof(Mod).IsAssignableFrom(type))
                    {
                        for (int i = 0; i < list.Length; i++)
                        {
                            if (list[i].Name == "MSCLoader")
                            {
                                string[] verparse = list[i].Version.ToString().Split('.');
                                if (list[i].Version.ToString() == "1.0.0.0")
                                    msVer = "0.1";
                                else
                                {
                                    if (verparse[2] == "0")
                                        msVer = string.Format("{0}.{1}", verparse[0], verparse[1]);
                                    else
                                        msVer = string.Format("{0}.{1}.{2}", verparse[0], verparse[1], verparse[2]);
                                }
                                break;
                            }
                        }
                        isMod = true;
                        LoadMod((Mod)Activator.CreateInstance(type), msVer);
                        break;
                    }
                    else
                    {
                        isMod = false;
                    }
                }
                if (!isMod)
                {
                    ModConsole.Error(string.Format("<b>{0}</b> - doesn't look like a mod or missing Mod subclass!", Path.GetFileName(file)));
                    InvalidMods.Add(Path.GetFileName(file));
                }
            }
            catch (Exception e)
            {
                ModConsole.Error(string.Format("<b>{0}</b> - doesn't look like a mod, remove this file from mods folder!", Path.GetFileName(file)));
                //ModConsole.Error(e.ToString());
                InvalidMods.Add(Path.GetFileName(file));
            }

        }

        private static void LoadMod(Mod mod, string msver)
        {
            // Check if mod already exists
            if (!LoadedMods.Contains(mod))
            {
                // Create config folder
                if (!Directory.Exists(ConfigFolder + mod.ID))
                {
                    Directory.CreateDirectory(ConfigFolder + mod.ID);
                }
                if (mod.UseAssetsFolder)
                {
                    if (!Directory.Exists(AssetsFolder + mod.ID))
                    {
                        Directory.CreateDirectory(AssetsFolder + mod.ID);
                    }
                }
                if (mod.LoadInMenu)
                {
                    mod.OnMenuLoad();
                    ModSettings.LoadBinds();
                }
                mod.compiledVersion = msver;
                LoadedMods.Add(mod);
            }
            else
            {
                ModConsole.Print(string.Format("<color=orange><b>Mod already loaded (or duplicated ID):</b></color><color=red><b>{0}</b></color>", mod.ID));
            }
        }

		/// <summary>
		/// Call Unity OnGUI() function, for each loaded mods.
		/// </summary>
		private void OnGUI()
		{
            GUI.skin = guiskin;
			// Call OnGUI for loaded mods
			foreach (Mod mod in LoadedMods)
			{
                try
                {
                    if (mod.LoadInMenu)
                        mod.OnGUI();
                    else if (Application.loadedLevelName == "GAME" && !mod.isDisabled)
                        mod.OnGUI();
                }
                catch (Exception e)
                {
                    if (LogAllErrors)
                    {
                        var st = new StackTrace(e, true);
                        var frame = st.GetFrame(0);

                        string errorDetails = string.Format("{2}<b>Details: </b>{0} in <b>{1}</b>", e.Message, frame.GetMethod(), Environment.NewLine);
                        ModConsole.Error(string.Format("Mod <b>{0}</b> throw an error!{1}", mod.ID, errorDetails));
                    }
                }
            }
		}

        /// <summary>
        /// Call Unity Update() function, for each loaded mods.
        /// </summary>
        private void Update()
        {
            if (!fullyLoaded)
            {
                //check if camera is active.
                if (GameObject.Find("PLAYER/Pivot/Camera/FPSCamera") != null)
                {
                    //load mods
                    fullyLoaded = true;
                    Init();
                }
            }

            if (Input.GetKeyDown(KeyCode.F10)) //debug
            {

            }

            // Call update for loaded mods
            foreach (Mod mod in LoadedMods)
			{
                
                try
                {
                    if(mod.LoadInMenu)
                        mod.Update();
                    else if(Application.loadedLevelName == "GAME" && !mod.isDisabled)
                        mod.Update();
                }
                catch (Exception e)
                {
                    if (LogAllErrors)
                    {
                        var st = new StackTrace(e, true);
                        var frame = st.GetFrame(0);

                        string errorDetails = string.Format("{2}<b>Details: </b>{0} in <b>{1}</b>", e.Message, frame.GetMethod(), Environment.NewLine);
                        ModConsole.Error(string.Format("Mod <b>{0}</b> throw an error!{1}", mod.ID, errorDetails));
                    }
                }
            }
		}
	}
}
