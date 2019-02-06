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
        /// A list of all loaded mods.
        /// </summary>
        public static List<Mod> LoadedMods;

        /// <summary>
        /// A list of invalid mod files 
        /// (like random dll in Mods Folder that isn't a mod).
        /// </summary>
        public static List<string> InvalidMods;

        /// <summary>
        /// The instance of ModLoader.
        /// </summary>
        public static ModLoader Instance;

        /// <summary>
        /// The current version of the ModLoader.
        /// </summary>
        public static readonly string Version = "0.4.7";
        
        /// <summary>
        /// Is this version of ModLoader experimental (this is NOT game experimental branch)
        /// </summary>
        public static readonly bool experimental = true;

        /// <summary>
        /// Is DevMode active
        /// </summary>
#if DevMode
        public static readonly bool devMode = true;
#else
        public static readonly bool devMode = false;
#endif

        private string expBuild = Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString();
        private MSCUnloader mscUnloader;

        private static string steamID;
        private static bool loaderPrepared = false;
        private static string ModsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
        private static string ConfigFolder = Path.Combine(ModsFolder, @"Config\");
        private static string AssetsFolder = Path.Combine(ModsFolder, @"Assets\");

        private GameObject mainMenuInfo;
        private GameObject loading;
        private Animator menuInfoAnim;
        private GUISkin guiskin;

        private string serverURL = "http://localhost/msc"; //localhost for testing only

        private bool IsDoneLoading = false;
        private bool IsModsLoading = false;
        private bool IsModsDoneLoading = false;
        private bool fullyLoaded = false;
        private bool allModsLoaded = false;
        private bool IsModsResetting = false;
        private bool IsModsDoneResetting = false;
        private bool introCheck;

        public static bool unloader = false;

        /// <summary>
        /// Check if steam is present
        /// </summary>
        /// <returns>Valid steam detected.</returns>
        public static bool CheckSteam()
        {
            if (steamID != null && steamID != string.Empty && steamID != "0")
                return true;
            else
                return false;
        }

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
                ModConsole.Error(string.Format("<b>{0}:</b> Please set variable <b>UseAssetsFolder</b> to <b>true</b>", mod.ID));
            return Path.Combine(AssetsFolder, mod.ID);
        }

        /// <summary>
        /// Initialize ModLoader with Mods folder in My Documents (like it was in 0.1)
        /// </summary>
        public static void Init_MD()
        {
            if (unloader) return;
            ModsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
            PrepareModLoader();
        }

        /// <summary>
        /// Initialize ModLoader with Mods folder in Game Folder (near game's exe)
        /// </summary>
        public static void Init_GF()
        {
            if (unloader) return;
            ModsFolder = Path.GetFullPath(Path.Combine("Mods", ""));
            PrepareModLoader();
        }

        /// <summary>
        /// Initialize ModLoader with Mods folder in AppData/LocalLow (near game's save)
        /// </summary>
        public static void Init_AD()
        {
            if (unloader) return;
            ModsFolder = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"..\LocalLow\Amistech\My Summer Car\Mods"));
            PrepareModLoader();
        }

        private static void PrepareModLoader()
        {
            if (!loaderPrepared)
            {
                loaderPrepared = true;
                GameObject go = new GameObject("MSCLoader", typeof(ModLoader));
                Instance = go.GetComponent<ModLoader>();
                DontDestroyOnLoad(go);
                Instance.Init();
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            if (Application.loadedLevelName == "MainMenu")
            {
                QualitySettings.vSyncCount = 1; //vsync in menu (test)

                if (IsDoneLoading && GameObject.Find("MSCLoader Info") == null)
                {
                    MainMenuInfo();
                }
                if (IsModsDoneLoading)
                {
                    loaderPrepared = false;
                    mscUnloader.MSCLoaderReset();
                    unloader = true;
                    return;
                }
            }
            else if (Application.loadedLevelName == "Intro")
            {
                if (!IsModsDoneResetting && !IsModsResetting)
                {
                    IsModsResetting = true;
                    StartCoroutine(NewGameMods());
                }
            }
            else if (Application.loadedLevelName == "GAME")
            {
                QualitySettings.vSyncCount = 0;

                if (IsDoneLoading)
                {
                    menuInfoAnim.SetBool("isHidden", true);
                }
            }
        }

        private void StartLoadingMods()
        {
            menuInfoAnim.SetBool("isHidden", true);
            if (!IsModsDoneLoading && !IsModsLoading)
            {
                introCheck = true;
                IsModsLoading = true;
                StartCoroutine(LoadMods());
            }
        }
        private void Init()
        {
            //Set config and Assets folder in selected mods folder
            ConfigFolder = Path.Combine(ModsFolder, @"Config\");
            AssetsFolder = Path.Combine(ModsFolder, @"Assets\");

            if (GameObject.Find("MSCUnloader") == null)
            {
                GameObject go = new GameObject();
                go.name = "MSCUnloader";
                go.AddComponent<MSCUnloader>();
                mscUnloader = go.GetComponent<MSCUnloader>();
                DontDestroyOnLoad(go);
            }
            else
            {
                mscUnloader = GameObject.Find("MSCUnloader").GetComponent<MSCUnloader>();
            }
            if (IsDoneLoading) //Remove this.
            {
             
                if (Application.loadedLevelName != "MainMenu")
                    menuInfoAnim.SetBool("isHidden", true);
            }
            else
            {

                // Init variables
                ModUI.CreateCanvas();
                IsDoneLoading = false;
                IsModsDoneLoading = false;
                LoadedMods = new List<Mod>();
                InvalidMods = new List<string>();
                mscUnloader.reset = false;
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
                LoadedMods[0].ModSettings();
                LoadMod(new ModSettings_menu(), Version);
                LoadedMods[1].ModSettings();
                ModSettings_menu.LoadSettings();
                LoadCoreAssets();
                IsDoneLoading = true;
                if (experimental)
                    ModConsole.Print(string.Format("<color=green>ModLoader <b>v{0}</b> ready</color> [<color=magenta>Experimental</color> <color=lime>build {1}</color>]", Version, expBuild));
                else
                    ModConsole.Print(string.Format("<color=green>ModLoader <b>v{0}</b> ready</color>", Version));
                LoadReferences();
                PreLoadMods();
                ModConsole.Print(string.Format("<color=orange>Found <color=green><b>{0}</b></color> mods!</color>", LoadedMods.Count - 2));
                try
                {
                    if (File.Exists(Path.GetFullPath(Path.Combine("LAUNCHER.exe", ""))) || File.Exists(Path.GetFullPath(Path.Combine("SmartSteamEmu64.dll", ""))) || File.Exists(Path.GetFullPath(Path.Combine("SmartSteamEmu.dll", ""))))
                    {
                        ModConsole.Print(string.Format("<color=orange>Hello <color=green><b>{0}</b></color>!</color>", "Murzyn!"));
                        throw new Exception("[EMULATOR] Do What You Want, Cause A Pirate Is Free... You Are A Pirate!");
                        //exclude emulators
                    }
                    Steamworks.SteamAPI.Init();
                    steamID = Steamworks.SteamUser.GetSteamID().ToString();
                    ModConsole.Print(string.Format("<color=orange>Hello <color=green><b>{0}</b></color>!</color>", Steamworks.SteamFriends.GetPersonaName()));
                    WebClient webClient = new WebClient();
                    webClient.QueryString.Add("sid", steamID);
                    string result = webClient.DownloadString(string.Format("{0}/mody-old.php", serverURL));
                    if (result != string.Empty)
                    {
                        if (result == "error")
                            throw new Exception("Holy shish are you cereal?");
                    }
                    bool ret = Steamworks.SteamApps.GetCurrentBetaName(out string Name, 128);
                    if (ret && !(bool)ModSettings_menu.expWarning.GetValue())
                    {
                        if (Name != "default_32bit") //32bit is NOT experimental branch
                            ModUI.ShowMessage(string.Format("<color=orange><b>Warning:</b></color>{1}You are using beta build: <color=orange><b>{0}</b></color>{1}{1}Remember that some mods may not work correctly on beta branches.", Name, Environment.NewLine), "Experimental build warning");
                    }
                    UnityEngine.Debug.Log(string.Format("MSC buildID: <b>{0}</b>", Steamworks.SteamApps.GetAppBuildId()));
                }
                catch (Exception e)
                {
                    //Make more sense in errors
                    steamID = null;
                    ModConsole.Error("Steam not detected, only steam version is supported.");
                    if (devMode)
                        ModConsole.Error(e.ToString());
                    UnityEngine.Debug.Log(e);
                }
                MainMenuInfo();
                LoadModsSettings();
                if (devMode)
                    ModConsole.Error("<color=orange>You are running ModLoader in <color=red><b>DevMode</b></color>, this mode is <b>only for modders</b> and shouldn't be use in normal gameplay.</color>");
            }
        }

        private void LoadReferences()
        {
            if (Directory.Exists(Path.Combine(ModsFolder, "References")))
            {
                string[] files = Directory.GetFiles(Path.Combine(ModsFolder, "References"), "*.dll");
                foreach (var file in files)
                {
                    Assembly.LoadFrom(file);
                }
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(ModsFolder, "References"));
            }
        }
        private void LoadCoreAssets()
        {
            ModConsole.Print("Loading core assets...");
            AssetBundle ab = LoadAssets.LoadBundle(new ModCore(), "core.unity3d");
            guiskin = ab.LoadAsset<GUISkin>("MSCLoader.guiskin");
            ModUI.messageBox = ab.LoadAsset<GameObject>("MSCLoader MB.prefab");
            mainMenuInfo = ab.LoadAsset<GameObject>("MSCLoader Info.prefab");
            loading = ab.LoadAsset<GameObject>("LoadingMods.prefab");
            loading.SetActive(false);
            loading = GameObject.Instantiate(loading);
            loading.transform.SetParent(GameObject.Find("MSCLoader Canvas").transform, false);
            ModConsole.Print("Loading core assets completed!");
            ab.Unload(false); //freeup memory
        }

        private void MainMenuInfo()
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
                        version = client.DownloadString(string.Format("{0}/ver.php",serverURL));
                    }
                    if (version.Trim().Length > 8)
                        throw new Exception("Parse Error, please report that problem!");
                    int i = Version.CompareTo(version.Trim());
                    if (i != 0)
                        info.text = string.Format("Mod Loader MSCLoader v{0} is ready! (<color=orange>New version available: <b>v{1}</b></color>)", Version, version.Trim());
                    else if (i == 0)
                        info.text = string.Format("Mod Loader MSCLoader v{0} is ready! (<color=lime>Up to date</color>)", Version);
                    if (devMode)
                        info.text = info.text + " [<color=red><b>Dev Mode!</b></color>]";
                }
                catch (Exception e)
                {
                    ModConsole.Error(string.Format("Check for new version failed with error: {0}", e.Message));
                    if (devMode)
                        ModConsole.Error(e.ToString());
                    UnityEngine.Debug.Log(e);
                    info.text = string.Format("Mod Loader MSCLoader v{0} is ready!", Version);
                }
            }
            else
            {
                try
                {
                    string newBuild;
                    using (WebClient client = new WebClient())
                    {
                        client.QueryString.Add("core", "exp_build");
                        newBuild = client.DownloadString(string.Format("{0}/ver.php",serverURL));
                    }
                    if (newBuild.Trim().Length > 8)
                        throw new Exception("Parse Error, please report that problem!");
                    int i = expBuild.CompareTo(newBuild.Trim());
                    if (i != 0)
                        info.text = string.Format("Mod Loader MSCLoader v{0} is ready! [<color=magenta>Experimental</color> <color=lime>build {1}</color>] (<color=orange>New build available: <b>{2}</b></color>)", Version, expBuild, newBuild);
                    else if (i == 0)
                        info.text = string.Format("Mod Loader MSCLoader v{0} is ready! [<color=magenta>Experimental</color> <color=lime>build {1}</color>]", Version, expBuild);

                }
                catch (Exception e)
                {
                    ModConsole.Error(string.Format("Check for new build failed with error: {0}", e.Message));
                    if (devMode)
                        ModConsole.Error(e.ToString());
                    UnityEngine.Debug.Log(e);
                    info.text = string.Format("Mod Loader MSCLoader v{0} is ready! [<color=magenta>Experimental</color> <color=lime>build {1}</color>]", Version, expBuild);
                }
            }
            mf.text = string.Format("Mods folder: {0}", ModsFolder);
            modUpdates.text = string.Empty;
            mainMenuInfo.transform.SetParent(GameObject.Find("MSCLoader Canvas").transform, false);
        }

        IEnumerator NewGameMods()
        {
            loading.transform.GetChild(2).GetComponent<Text>().text = string.Format("MSCLoader <color=green>v{0}</color>", Version);
            ModConsole.Print("Resetting mods...");
            loading.SetActive(true);
            loading.transform.GetChild(3).GetComponent<Slider>().minValue = 1;
            loading.transform.GetChild(3).GetComponent<Slider>().maxValue = LoadedMods.Count - 2;

            int i = 1;
            foreach (Mod mod in LoadedMods)
            {

                loading.transform.GetChild(0).GetComponent<Text>().text = string.Format("<color=red>Resetting mods: <color=orange><b>{0}</b></color> of <color=orange><b>{1}</b></color>. <b>Do not skip intro yet!...</b></color>", i, LoadedMods.Count - 2);
                loading.transform.GetChild(3).GetComponent<Slider>().value = i;
                loading.transform.GetChild(3).GetChild(1).GetChild(0).GetComponent<Image>().color = Color.red;
                if (mod.ID.StartsWith("MSCLoader_"))
                    continue;
                i++;
                loading.transform.GetChild(1).GetComponent<Text>().text = mod.Name;
                yield return new WaitForSeconds(.4f);
                try
                {
                    mod.OnNewGame();
                }
                catch (Exception e)
                {
                    var st = new StackTrace(e, true);
                    var frame = st.GetFrame(0);

                    string errorDetails = string.Format("{2}<b>Details: </b>{0} in <b>{1}</b>", e.Message, frame.GetMethod(), Environment.NewLine);
                    ModConsole.Error(string.Format("Mod <b>{0}</b> throw an error!{1}", mod.ID, errorDetails));
                    if (devMode)
                        ModConsole.Error(e.ToString());
                    UnityEngine.Debug.Log(e);
                }

            }
            loading.transform.GetChild(0).GetComponent<Text>().text = string.Format("Resetting Done! You can skip intro now!");
            yield return new WaitForSeconds(2f);
            loading.SetActive(false);
            IsModsDoneResetting = true;
            ModConsole.Print("Resetting done!");
            IsModsResetting = false;

        }
        IEnumerator LoadMods()
        {
            loading.transform.GetChild(2).GetComponent<Text>().text = string.Format("MSCLoader <color=green>v{0}</color>", Version);
            ModConsole.Print("Loading mods...");
            Stopwatch s = new Stopwatch();
            s.Start();
            ModConsole.Print("<color=#505050ff>");
            // Load Mods
            loading.SetActive(true);
            loading.transform.GetChild(3).GetComponent<Slider>().minValue = 1;
            loading.transform.GetChild(3).GetComponent<Slider>().maxValue = LoadedMods.Count-2;

            int i = 1;
            foreach (Mod mod in LoadedMods)
            {
                
                loading.transform.GetChild(0).GetComponent<Text>().text = string.Format("Loading mods: <color=orage><b>{0}</b></color> of <color=orage><b>{1}</b></color>. Please wait...", i, LoadedMods.Count-2);
                loading.transform.GetChild(3).GetComponent<Slider>().value = i;
                loading.transform.GetChild(3).GetChild(1).GetChild(0).GetComponent<Image>().color = new Color32(0, 113, 0, 255);
                if (mod.ID.StartsWith("MSCLoader_"))
                    continue;
                i++;
                if (!mod.isDisabled)
                    loading.transform.GetChild(1).GetComponent<Text>().text = mod.Name;
                yield return new WaitForSeconds(.05f);
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
                    if (devMode)
                        ModConsole.Error(e.ToString());
                    UnityEngine.Debug.Log(e);
                }

            }
            loading.SetActive(false);
            ModConsole.Print("</color>");
            allModsLoaded = true;
            ModSettings_menu.LoadBinds();
            IsModsDoneLoading = true;
            s.Stop();
            if (s.ElapsedMilliseconds < 1000)
                ModConsole.Print(string.Format("Loading mods completed in {0}ms!", s.ElapsedMilliseconds));
            else
                ModConsole.Print(string.Format("Loading mods completed in {0} sec(s)!", s.Elapsed.Seconds));
        }

        private void PreLoadMods()
        {
            // Load .dll files
            foreach (string file in Directory.GetFiles(ModsFolder))
            {
                if (file.EndsWith(".dll"))
                {
                    LoadDLL(file);
                }
            }

            //cleanup files if not in dev mode
            if (!devMode)
            {
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

        }

        private void LoadModsSettings()
        {
            foreach (Mod mod in LoadedMods)
            {
                if (mod.ID.StartsWith("MSCLoader_"))
                    continue;
                try
                {
                    mod.ModSettings();                
                }
                catch (Exception e)
                {
                    ModConsole.Error(string.Format("Settings error for mod <b>{0}</b>{2}<b>Details:</b> {1}", mod.ID, e.Message, Environment.NewLine));
                    if (devMode)
                        ModConsole.Error(e.ToString());
                    UnityEngine.Debug.Log(e);
                }
            }
            ModSettings_menu.LoadSettings();
        }

        private void LoadGarage()
        {
            //Access garage reference here...

        }

        //Remove this shit below

        /*static void ModStats()
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
                    string result = webClient.DownloadString("http://my-summer-car.ml/mody-old.php");
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
                    if (devMode)
                        ModConsole.Error(e.ToString());
                    UnityEngine.Debug.Log(e);
                }
            }
        }*/

        private void LoadDLL(string file)
        {
            try
            {
                Assembly asm = null;
                asm = Assembly.LoadFrom(file);
                bool isMod = false;

                AssemblyName[] list = asm.GetReferencedAssemblies();

                // Look through all public classes                
                foreach (Type type in asm.GetTypes())
                {
                    string msVer = null;
                    if (typeof(Mod).IsAssignableFrom(type))
                    {
                        for (int i = 0; i < list.Length; i++)
                        {
                            if (list[i].Name == "Assembly-CSharp-firstpass")
                            {
                                throw new Exception("Targeting forbidden reference");
                            }
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
                if (devMode)
                    ModConsole.Error(e.ToString());
                UnityEngine.Debug.Log(e);
                InvalidMods.Add(Path.GetFileName(file));
            }

        }

        private void LoadMod(Mod mod, string msver)
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
                    ModSettings_menu.LoadBinds();
                }
                mod.compiledVersion = msver;
                LoadedMods.Add(mod);
            }
            else
            {
                ModConsole.Print(string.Format("<color=orange><b>Mod already loaded (or duplicated ID):</b></color><color=red><b>{0}</b></color>", mod.ID));
            }
        }

        private void OnGUI()
        {
            GUI.skin = guiskin;
            foreach (Mod mod in LoadedMods)
            {
                try
                {
                    if (mod.LoadInMenu)
                        mod.OnGUI();
                    else if (Application.loadedLevelName == "GAME" && !mod.isDisabled && allModsLoaded)
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
                    UnityEngine.Debug.Log(e);
                    if (allModsLoaded && fullyLoaded)
                        mod.modErrors++;
                    if (devMode)
                    {
                        if (mod.modErrors == 30)
                        {
                            ModConsole.Error(string.Format("Mod <b>{0}</b> thrown <b>too many errors</b>!", mod.ID));
                            ModConsole.Error(e.ToString());
                        }

                    }
                    else
                    {
                        if (mod.modErrors > 30)
                        {
                            mod.isDisabled = true;
                            ModConsole.Error(string.Format("Mod <b>{0}</b> has been <b>disabled!</b> Because it thrown too many errors!{1}Report this problem to mod author.", mod.ID, Environment.NewLine));
                        }
                    }
                    
                }
            }
        }

        private void Update()
        {
            if (!fullyLoaded)
            {
                //check if camera is active.
                if (GameObject.Find("PLAYER/Pivot/Camera/FPSCamera") != null)
                {
                    //load mods
                    allModsLoaded = false;
                    fullyLoaded = true;
                    StartLoadingMods();
                }
            }

            if(!introCheck)
            {
                if(Application.loadedLevelName == "Intro")
                {
                    introCheck = true;
                    Init();
                }
            }

            foreach (Mod mod in LoadedMods)
            {

                try
                {
                    if (mod.LoadInMenu)
                        mod.Update();
                    else if (Application.loadedLevelName == "GAME" && !mod.isDisabled && allModsLoaded)
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
                    UnityEngine.Debug.Log(e);
                    if (allModsLoaded && fullyLoaded)
                        mod.modErrors++;
                    if (devMode)
                    {
                        if (mod.modErrors == 30)
                        {
                            ModConsole.Error(string.Format("Mod <b>{0}</b> thrown <b>too many errors</b>!", mod.ID));
                            ModConsole.Error(e.ToString());
                        }

                    }
                    else
                    {
                        if (mod.modErrors > 30)
                        {
                            mod.isDisabled = true;
                            ModConsole.Error(string.Format("Mod <b>{0}</b> has been <b>disabled!</b> Because it thrown too many errors!{1}Report this problem to mod author.", mod.ID, Environment.NewLine));
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            foreach (Mod mod in LoadedMods)
            {

                try
                {
                    if (mod.LoadInMenu)
                        mod.FixedUpdate();
                    else if (Application.loadedLevelName == "GAME" && !mod.isDisabled && allModsLoaded)
                        mod.FixedUpdate();
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
                    UnityEngine.Debug.Log(e);
                    if (allModsLoaded && fullyLoaded)
                        mod.modErrors++;
                    if (devMode)
                    {
                        if (mod.modErrors == 30)
                        {
                            ModConsole.Error(string.Format("Mod <b>{0}</b> thrown <b>too many errors</b>!", mod.ID));
                            ModConsole.Error(e.ToString());
                        }

                    }
                    else
                    {
                        if (mod.modErrors > 30)
                        {
                            mod.isDisabled = true;
                            ModConsole.Error(string.Format("Mod <b>{0}</b> has been <b>disabled!</b> Because it thrown too many errors!{1}Report this problem to mod author.", mod.ID, Environment.NewLine));
                        }
                    }
                }
            }
        }
    }
}
