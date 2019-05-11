using Ionic.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{

    /// <summary>
    /// List of possible scenes
    /// </summary>
    public enum CurrentScene
    {
        /// <summary>
        /// Main Menu
        /// </summary>
        MainMenu,
        /// <summary>
        /// Game Scene
        /// </summary>
        Game,
        /// <summary>
        /// Intro for new game
        /// </summary>
        NewGameIntro,
    }

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
        internal static ModLoader Instance;

        /// <summary>
        /// The current version of the ModLoader.
        /// </summary>
        public static readonly string Version = "1.0.1";

        /// <summary>
        /// Is this version of ModLoader experimental (this is NOT game experimental branch)
        /// </summary>
        public static readonly bool experimental = false;

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
        private static string authKey;
        private static bool loaderPrepared = false;
        private static string ModsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
        private static string ConfigFolder = Path.Combine(ModsFolder, @"Config\");
        private static string AssetsFolder = Path.Combine(ModsFolder, @"Assets\");

        private GameObject mainMenuInfo;
        private GameObject loading;
        private Animator menuInfoAnim;
        private GUISkin guiskin;
        private ModCore modCore;

        private string serverURL = "http://my-summer-car.ml"; //localhost for testing only

        private bool IsDoneLoading = false;
        private bool IsModsLoading = false;
        private bool IsModsDoneLoading = false;
        private bool fullyLoaded = false;
        private bool allModsLoaded = false;
        private bool IsModsResetting = false;
        private bool IsModsDoneResetting = false;
        private static CurrentScene CurrentGameScene;

        internal static bool unloader = false;

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
        /// Get Current Game Scene
        /// </summary>
        /// <returns>CurrentScene enum</returns>
        public static CurrentScene GetCurrentScene()
        {
            return CurrentGameScene;
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
                CurrentGameScene = CurrentScene.MainMenu;
                if ((bool)ModSettings_menu.forceMenuVsync.GetValue())
                    QualitySettings.vSyncCount = 1; //vsync in menu
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
                CurrentGameScene = CurrentScene.NewGameIntro;

                if (!IsModsDoneResetting && !IsModsResetting)
                {
                    IsModsResetting = true;
                    StartCoroutine(NewGameMods());
                }
            }
            else if (Application.loadedLevelName == "GAME")
            {
                CurrentGameScene = CurrentScene.Game;
                if ((bool)ModSettings_menu.forceMenuVsync.GetValue())
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
                //introCheck = true;
                IsModsLoading = true;
                StartCoroutine(LoadMods());
            }
        }

        /// <summary>
        /// Set Auth key (not used)
        /// </summary>
        /// <param name="ak">auth key</param>
        public static void SetAuthKey(string ak) => authKey = ak;

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
                ModUI.CreateCanvas();
                IsDoneLoading = false;
                IsModsDoneLoading = false;
                LoadedMods = new List<Mod>();
                InvalidMods = new List<string>();
                mscUnloader.reset = false;
                if (!Directory.Exists(ModsFolder))
                    Directory.CreateDirectory(ModsFolder);
                if (!Directory.Exists(ConfigFolder))
                    Directory.CreateDirectory(ConfigFolder);
                if (!Directory.Exists(AssetsFolder))
                    Directory.CreateDirectory(AssetsFolder);

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
                    //  webClient.Proxy = new WebProxy("127.0.0.1:8888"); //ONLY FOR TESTING
                    webClient.DownloadStringCompleted += sAuthCheckCompleted;
                    webClient.DownloadStringAsync(new Uri(string.Format("{0}/sauth.php?sid={1}", serverURL, steamID)));
                    //return;//Temporary 
                   /* if ((bool)ModSettings_menu.enGarage.GetValue())
                    {
                        webClient.DownloadStringCompleted += AuthCheck;                      
                        webClient.DownloadStringAsync(new Uri(string.Format("{0}/auth.php?sid={1}&auth={2}", serverURL, steamID, authKey)));
                    }
                    else
                    {
                        webClient.DownloadStringCompleted += sAuthCheckCompleted;
                        webClient.DownloadStringAsync(new Uri(string.Format("{0}/sauth.php?sid={1}", serverURL, steamID)));
                    }*/
                }
                catch (Exception e)
                {
                    steamID = null;
                    ModConsole.Error("Steam client doesn't exists.");
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


        [Serializable]
        class SaveOtk
        {
            public string k1;
            public string k2;
        }

        private void AuthCheck(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                    throw new Exception(e.Error.Message);

                string result = e.Result;

                if (result != string.Empty)
                {
                    string[] ed = result.Split('|');
                    if (ed[0] == "error")
                    {
                        switch (ed[1])
                        {
                            case "0":
                                throw new Exception("SteamID failed.");
                            case "1":
                                throw new Exception("Auth-key is missing.");
                            case "2":
                                throw new Exception("Auth-key is invalid.");
                            case "3":
                                throw new Exception("Database connection error.");
                            default:
                                throw new Exception("Unknown error.");
                        }
                    }
                    else if (ed[0] == "ok")
                    {
                        SaveOtk s = new SaveOtk();
                        s.k1 = ed[1];
                        s.k2 = ed[2];
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        string sp = Path.Combine(ConfigFolder, @"MSCLoader_Settings\otk.bin");
                        FileStream st = new FileStream(sp, FileMode.Create);
                        f.Serialize(st, s);
                        st.Close();
                        LoadGarage();
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Unknown: " + ed[0]);
                        throw new Exception("Unknown server response.");
                    }
                }
                bool ret = Steamworks.SteamApps.GetCurrentBetaName(out string Name, 128);
                if (ret && (bool)ModSettings_menu.expWarning.GetValue())
                {
                    if (Name != "default_32bit") //32bit is NOT experimental branch
                        ModUI.ShowMessage(string.Format("<color=orange><b>Warning:</b></color>{1}You are using beta build: <color=orange><b>{0}</b></color>{1}{1}Remember that some mods may not work correctly on beta branches.", Name, Environment.NewLine), "Experimental build warning");
                }
                UnityEngine.Debug.Log(string.Format("MSC buildID: <b>{0}</b>", Steamworks.SteamApps.GetAppBuildId()));
            }
            catch (Exception ex)
            {
                string sp = Path.Combine(ConfigFolder, @"MSCLoader_Settings\otk.bin");
                if (e.Error != null)
                {
                    if (File.Exists(sp))
                    {
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        FileStream st = new FileStream(sp, FileMode.Open);
                        SaveOtk s = f.Deserialize(st) as SaveOtk;
                        st.Close();
                        string murzyn = "otk_" + MurzynskaMatematyka(string.Format("{0}{1}", steamID, s.k1));
                        if (s.k2.CompareTo(murzyn) != 0)
                        {
                            File.Delete(sp);
                            steamID = null;
                            ModConsole.Error("SteamAPI failed with error: " + ex.Message);
                        }
                    }
                    else
                    {
                        steamID = null;
                        ModConsole.Error("SteamAPI failed with error: " + ex.Message);
                    }
                }
                else
                {
                    if (File.Exists(sp))
                        File.Delete(sp);
                    steamID = null;
                    ModConsole.Error("SteamAPI failed with error: " + ex.Message);
                    if (devMode)
                        ModConsole.Error(ex.ToString());
                }

                UnityEngine.Debug.Log(ex);
            }
        }

        private void sAuthCheckCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                    throw new Exception(e.Error.Message);

                string result = e.Result;

                if (result != string.Empty)
                {
                    string[] ed = result.Split('|');
                    if (ed[0] == "error")
                    {
                        switch (ed[1])
                        {
                            case "0":
                                throw new Exception("Getting steamID failed.");
                            case "1":
                                throw new Exception("steamID rejected.");
                            default:
                                throw new Exception("Unknown error.");
                        }
                    }
                    else if (ed[0] == "ok")
                    {
                        SaveOtk s = new SaveOtk();
                        s.k1 = ed[1];
                        s.k2 = ed[2];
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        string sp = Path.Combine(ConfigFolder, @"MSCLoader_Settings\otk.bin");
                        FileStream st = new FileStream(sp, FileMode.Create);
                        f.Serialize(st, s);
                        st.Close();
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Unknown: " + ed[0]);
                        throw new Exception("Unknown server response.");
                    }
                }
                bool ret = Steamworks.SteamApps.GetCurrentBetaName(out string Name, 128);
                if (ret && (bool)ModSettings_menu.expWarning.GetValue())
                {
                    if (Name != "default_32bit") //32bit is NOT experimental branch
                        ModUI.ShowMessage(string.Format("<color=orange><b>Warning:</b></color>{1}You are using beta build: <color=orange><b>{0}</b></color>{1}{1}Remember that some mods may not work correctly on beta branches.", Name, Environment.NewLine), "Experimental build warning");
                }
                UnityEngine.Debug.Log(string.Format("MSC buildID: <b>{0}</b>", Steamworks.SteamApps.GetAppBuildId()));
            }
            catch (Exception ex)
            {
                string sp = Path.Combine(ConfigFolder, @"MSCLoader_Settings\otk.bin");
                if (e.Error != null)
                {
                    if (File.Exists(sp))
                    {
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        FileStream st = new FileStream(sp, FileMode.Open);
                        SaveOtk s = f.Deserialize(st) as SaveOtk;
                        st.Close();
                        string murzyn = "otk_" + MurzynskaMatematyka(string.Format("{0}{1}", steamID, s.k1));
                        if(s.k2.CompareTo(murzyn) != 0)
                        {
                            File.Delete(sp);
                            steamID = null;
                            ModConsole.Error("SteamAPI failed with error: " + ex.Message);
                        }
                        else
                        {
                            UnityEngine.Debug.Log("offline");
                        }
                    }
                    else
                    {
                        steamID = null;
                        ModConsole.Error("SteamAPI failed with error: " + ex.Message);
                    }
                }
                else
                {
                    if (File.Exists(sp))
                        File.Delete(sp);
                    steamID = null;
                    ModConsole.Error("SteamAPI failed with error: " + ex.Message);
                    if (devMode)
                        ModConsole.Error(ex.ToString());
                }

                UnityEngine.Debug.Log(ex);
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
            modCore = new ModCore();
            ModConsole.Print("Loading core assets...");
            AssetBundle ab = LoadAssets.LoadBundle(modCore, "core.unity3d");
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

        /// <summary>
        /// Toggle main menu path via settings
        /// </summary>
        public static void MainMenuPath()
        {
            Instance.mainMenuInfo.transform.GetChild(1).gameObject.SetActive((bool)ModSettings_menu.modPath.GetValue());
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
            info.text = string.Format("Mod Loader MSCLoader <color=cyan>v{0}</color> is ready! (<color=orange>Checking for updates...</color>)", Version);
            WebClient client = new WebClient();
            //client.Proxy = new WebProxy("127.0.0.1:8888"); //ONLY FOR TESTING
            client.DownloadStringCompleted += VersionCheckCompleted;
            string branch = "unknown";
            if (experimental)
                branch = "exp_build";
            else
                branch = "stable";
            client.DownloadStringAsync(new Uri(string.Format("{0}/ver.php?core={1}", serverURL, branch)));

            mf.text = string.Format("<color=orange>Mods folder:</color> {0}", ModsFolder);
            MainMenuPath();
            modUpdates.text = string.Empty;
            mainMenuInfo.transform.SetParent(GameObject.Find("MSCLoader Canvas").transform, false);
        }

        private void VersionCheckCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Text info = mainMenuInfo.transform.GetChild(0).gameObject.GetComponent<Text>();
            try
            {
                if (e.Error != null)
                    throw new Exception(e.Error.Message);

                string[] result = e.Result.Split('|');
                if (result[0] == "error")
                {
                    switch (result[1])
                    {
                        case "0":
                            throw new Exception("Unknown branch");
                        case "1":
                            throw new Exception("Database connection error");
                        default:
                            throw new Exception("Unknown error");
                    }

                }
                else if (result[0] == "ok")
                {
                    if (result[1].Trim().Length > 8)
                        throw new Exception("Parse Error, please report that problem!");
                    int i;
                    if (experimental)
                        i = expBuild.CompareTo(result[1].Trim());
                    else
                        i = Version.CompareTo(result[1].Trim());
                    if (i != 0)
                        if (experimental)
                            info.text = string.Format("MSCLoader <color=cyan>v{0}</color> is ready! [<color=magenta>Experimental</color> <color=lime>build {1}</color>] (<color=orange>New build available: <b>{2}</b></color>)", Version, expBuild, result[1]);
                        else
                            info.text = string.Format("MSCLoader <color=cyan>v{0}</color> is ready! (<color=orange>New version available: <b>v{1}</b></color>)", Version, result[1].Trim());
                    else if (i == 0)
                        if (experimental)
                            info.text = string.Format("MSCLoader <color=cyan>v{0}</color> is ready! [<color=magenta>Experimental</color> <color=lime>build {1}</color>]", Version, expBuild);
                        else
                            info.text = string.Format("MSCLoader <color=cyan>v{0}</color> is ready! (<color=lime>Up to date</color>)", Version);
                }
                else
                {
                    UnityEngine.Debug.Log("Unknown: " + result[0]);
                    throw new Exception("Unknown server response.");
                }
            }
            catch (Exception ex)
            {
                ModConsole.Error(string.Format("Check for new build failed with error: {0}", ex.Message));
                if (devMode)
                    ModConsole.Error(ex.ToString());
                UnityEngine.Debug.Log(ex);
                if (experimental)
                    info.text = string.Format("MSCLoader <color=cyan>v{0}</color> is ready! [<color=magenta>Experimental</color> <color=lime>build {1}</color>]", Version, expBuild);
                else
                    info.text = string.Format("MSCLoader <color=cyan>v{0}</color> is ready!", Version);

            }
            if (devMode)
                info.text += " [<color=red><b>Dev Mode!</b></color>]";
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
                    StackFrame frame = new StackTrace(e, true).GetFrame(0);

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
            loading.SetActive(true);
            loading.transform.GetChild(3).GetComponent<Slider>().minValue = 1;
            loading.transform.GetChild(3).GetComponent<Slider>().maxValue = LoadedMods.Count - 2;

            int i = 1;
            foreach (Mod mod in LoadedMods)
            {

                loading.transform.GetChild(0).GetComponent<Text>().text = string.Format("Loading mods: <color=orage><b>{0}</b></color> of <color=orage><b>{1}</b></color>. Please wait...", i, LoadedMods.Count - 2);
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
                    StackFrame frame = new StackTrace(e, true).GetFrame(0);

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
                    {
                        try
                        {
                            Directory.Delete(dir, true);
                        }
                        catch (Exception ex)
                        {
                            ModConsole.Error(string.Format("{0} (corrupted file?)", ex.Message));
                        }
                    }
                }
                foreach (string dir in Directory.GetDirectories(AssetsFolder))
                {
                    if (!LoadedMods.Exists(x => x.ID == new DirectoryInfo(dir).Name) && new DirectoryInfo(dir).Name != "MSCLoader_Core")
                    {
                        try
                        {
                            Directory.Delete(dir, true);
                        }
                        catch (Exception ex)
                        {
                            ModConsole.Error(string.Format("{0} (corrupted file?)", ex.Message));
                        }
                    }
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
        int pbar = 0;
        bool dwnlFinished = false;
        IEnumerator DownloadProgress()
        {
            while (!dwnlFinished)
            {
                loading.transform.GetChild(0).GetComponent<Text>().text = string.Format("<color=green>Downloading MSCGarage! Please wait...</color>");
                loading.transform.GetChild(3).GetComponent<Slider>().value = pbar;
                loading.transform.GetChild(1).GetComponent<Text>().text = string.Format("Download progress: {0}%", pbar);
                yield return new WaitForEndOfFrame();
            }
        }
        private void LoadGarage()
        {
            return; //Delayed
            if (!File.Exists(Path.Combine(Application.dataPath, @"Managed\MSCGarage.dll")))
            {
                WebClient client = new WebClient();
                //client.Proxy = new WebProxy("127.0.0.1:8888"); //ONLY FOR TESTING
                client.DownloadFileCompleted += DownloadGarageCompleted;
                client.DownloadProgressChanged += DownloadGarageProgress;
                client.DownloadFileAsync(new Uri(string.Format("{0}/Garage/test.zip",serverURL)), Path.Combine(Application.dataPath, @"Managed\test.zip"),loading);
                loading.SetActive(true);
                loading.transform.GetChild(3).GetComponent<Slider>().minValue = 0;
                loading.transform.GetChild(3).GetComponent<Slider>().maxValue = 100;
                loading.transform.GetChild(2).GetComponent<Text>().text = string.Format("MSCLoader <color=green>v{0}</color>", Version);
                ModConsole.Print("Downloading garage...");
                loading.transform.GetChild(0).GetComponent<Text>().text = string.Format("<color=green>Downloading MSCGarage! Please wait...</color>");
                loading.transform.GetChild(1).GetComponent<Text>().text = string.Format("Waiting for response...");
                StartCoroutine(DownloadProgress());
                return;
            }
        }

        private void DownloadGarageProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            pbar = e.ProgressPercentage;
        }

        private void DownloadGarageCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            dwnlFinished = true;
            if (e.Error != null)
            {
                loading.SetActive(false);
                ModConsole.Error(string.Format("Download MSCGarage failed with error: {0}", e.Error.Message));
                return;
            }
            else
            {
                loading.transform.GetChild(1).GetComponent<Text>().text = string.Format("Unpacking...");
                ModConsole.Print("Unpacking garage...");
                try
                {
                    string zip = Path.Combine(Application.dataPath, @"Managed\test.zip");
                    if (!ZipFile.IsZipFile(zip))
                    {
                        loading.SetActive(false);
                        ModConsole.Error(string.Format("Failed to unpack file"));
                    }
                    ZipFile zip1 = ZipFile.Read(zip);
                    foreach (ZipEntry zz in zip1)
                    {
                        ModConsole.Print(zz.FileName);
                        loading.transform.GetChild(1).GetComponent<Text>().text = "Unpacking garage... " + zz.FileName;
                        zz.Extract(Path.Combine(Application.dataPath, @"Managed"), ExtractExistingFileAction.OverwriteSilently);
                    }
                    ModConsole.Print("Done!");
                    loading.SetActive(false);
                }
                catch (Exception ex)
                {
                    ModConsole.Error(ex.Message);
                    UnityEngine.Debug.Log(ex);
                }
            }
        }

        private void LoadDLL(string file)
        {
            try
            {
                Assembly asm = Assembly.LoadFrom(file);
                bool isMod = false;

                AssemblyName[] list = asm.GetReferencedAssemblies();
                if (File.ReadAllText(file).Contains("RegistryKey"))
                    throw new FileLoadException();
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
                mod.compiledVersion = msver;
                LoadedMods.Add(mod);
                try
                {
                    if (mod.LoadInMenu)
                    {
                        mod.OnMenuLoad();
                        ModSettings_menu.LoadBinds();
                    }
                }
                catch (Exception e)
                {
                    StackFrame frame = new StackTrace(e, true).GetFrame(0);

                    string errorDetails = string.Format("{2}<b>Details: </b>{0} in <b>{1}</b>", e.Message, frame.GetMethod(), Environment.NewLine);
                    ModConsole.Error(string.Format("Mod <b>{0}</b> throw an error!{1}", mod.ID, errorDetails));
                    if (devMode)
                        ModConsole.Error(e.ToString());
                    UnityEngine.Debug.Log(e);
                }
            }
            else
            {
                ModConsole.Error(string.Format("<color=orange><b>Mod already loaded (or duplicated ID):</b></color><color=red><b>{0}</b></color>", mod.ID));
            }
        }

        private void OnGUI()
        {
            GUI.skin = guiskin;
            for (int i = 0; i < LoadedMods.Count; i++)
            {
                Mod mod = LoadedMods[i];
                try
                {
                    if (mod.LoadInMenu && !mod.isDisabled)
                        mod.OnGUI();
                    else if (Application.loadedLevelName == "GAME" && !mod.isDisabled && allModsLoaded)
                        mod.OnGUI();
                }
                catch (Exception e)
                {
                    if (LogAllErrors)
                    {
                        StackFrame frame = new StackTrace(e, true).GetFrame(0);

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
                if (GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera") != null)
                {
                    //load mods
                    allModsLoaded = false;
                    fullyLoaded = true;
                    StartLoadingMods();
                }
            }

            for (int i = 0; i < LoadedMods.Count; i++)
            {
                Mod mod = LoadedMods[i];
                try
                {
                    if (mod.LoadInMenu && !mod.isDisabled)
                        mod.Update();
                    else if (Application.loadedLevelName == "GAME" && !mod.isDisabled && allModsLoaded)
                        mod.Update();
                }
                catch (Exception e)
                {
                    if (LogAllErrors)
                    {
                        StackFrame frame = new StackTrace(e, true).GetFrame(0);

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
            for (int i = 0; i < LoadedMods.Count; i++)
            {
                Mod mod = LoadedMods[i];
                try
                {
                    if (mod.LoadInMenu && !mod.isDisabled)
                        mod.FixedUpdate();
                    else if (Application.loadedLevelName == "GAME" && !mod.isDisabled && allModsLoaded)
                        mod.FixedUpdate();
                }
                catch (Exception e)
                {
                    if (LogAllErrors)
                    {
                        StackFrame frame = new StackTrace(e, true).GetFrame(0);

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
        static string MurzynskaMatematyka(string rawData)
        {
            using (System.Security.Cryptography.SHA1 sha256 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}