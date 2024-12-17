global using UnityEngine;


#if !Mini
using HutongGames.PlayMaker;
using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using UnityEngine.UI;

namespace MSCLoader;

/// <summary>
/// This is main Mod Loader class.
/// </summary>
public partial class ModLoader : MonoBehaviour
{
    /// <summary>
    /// A list of all loaded mods.
    /// </summary>
    public static List<Mod> LoadedMods { get; internal set; }

    /// <summary>
    /// The current version of the ModLoader.
    /// </summary>
    public static readonly string MSCLoader_Ver;

    /// <summary>
    /// Is this version of ModLoader experimental (this is NOT game experimental branch)
    /// </summary>
#if Debug
    public static readonly bool experimental = true;
#else
    public static readonly bool experimental = false;
#endif

    internal static bool devMode = false;
    internal static string GetMetadataFolder(string fn) => Path.Combine(MetadataFolder, fn);
    private MSCLoaderCanvasLoading canvLoading;

    //Constructor version number
    static ModLoader()
    {
        if (Assembly.GetExecutingAssembly().GetName().Version.Build == 0)
            MSCLoader_Ver = $"{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}";
        else
            MSCLoader_Ver = $"{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}.{Assembly.GetExecutingAssembly().GetName().Version.Build}";
    }
    void Awake()
    {
        if (GameObject.Find("Music") != null)
            GameObject.Find("Music").GetComponent<AudioSource>().Stop();
    }

    /// <summary>
    /// Main init
    /// </summary>
    internal static void Init_NP(string cfg)
    {
        switch (cfg)
        {
            case "GF":
                Init_GF();
                break;
            case "MD":
                Init_MD();
                break;
            case "AD":
                Init_AD();
                break;
            default:
                Init_GF();
                break;
        }
    }

    internal static void Init_MD()
    {
        if (unloader) return;
        ModsFolder = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine("MySummerCar", "Mods")));
        PrepareModLoader();
    }

    internal static void Init_GF()
    {
        if (unloader) return;
        ModsFolder = Path.GetFullPath(Path.Combine("Mods", ""));
        PrepareModLoader();
    }

    internal static void Init_AD()
    {
        if (unloader) return;
        ModsFolder = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "Mods"));
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
    bool vse = false;
    private void OnLevelWasLoaded(int level)
    {
        switch (Application.loadedLevelName)
        {
            case "MainMenu":
                CurrentScene = CurrentScene.MainMenu;
                if (GameObject.Find("Music"))
                    GameObject.Find("Music").GetComponent<AudioSource>().Play();
                if (QualitySettings.vSyncCount != 0)
                    vse = true;
                if (ModMenu.forceMenuVsync.GetValue() && !vse)
                    QualitySettings.vSyncCount = 1; //vsync in menu
                if (GameObject.Find("MSCLoader Info") == null)
                {
                    MainMenuInfo();
                }
                if (allModsLoaded)
                {
                    loaderPrepared = false;
                    mscUnloader.MSCLoaderReset();
                    unloader = true;
                    return;
                }
                if (IsReferencePresent("MSCCoreLibrary"))
                    TimeSchedulerCalls("stop");
                break;
            case "Intro":
                CurrentScene = CurrentScene.NewGameIntro;

                if (!IsModsResetting)
                {
                    IsModsResetting = true;
                    StartCoroutine(NewGameMods());
                }
                break;
            case "GAME":
                CurrentScene = CurrentScene.Game;
                if (ModMenu.forceMenuVsync.GetValue() && !vse)
                    QualitySettings.vSyncCount = 0;

                menuInfoAnim.Play("fade_out");
                if (IsReferencePresent("MSCCoreLibrary"))
                    TimeSchedulerCalls("start");
                StartLoadingMods();
                ModMenu.ModMenuHandle();
                break;
            case "Ending":
                CurrentScene = CurrentScene.Ending;
                if (IsReferencePresent("MSCCoreLibrary"))
                    TimeSchedulerCalls("stop");
                break;
        }
    }

    private void TimeSchedulerCalls(string call)
    {
        switch (call)
        {
            case "start":
                MSCCoreLibrary.TimeScheduler.StartScheduler();
                break;
            case "stop":
                MSCCoreLibrary.TimeScheduler.StopScheduler();
                break;
            case "load":
                MSCCoreLibrary.TimeScheduler.LoadScheduler();
                break;
            case "save":
                MSCCoreLibrary.TimeScheduler.SaveScheduler();
                break;
            default:
                ModConsole.Error($"Invalid TimeScheduler call: {call}");
                break;
        }
    }
    private void StartLoadingMods()
    {
        if (!allModsLoaded && !IsModsLoading)
        {
            IsModsLoading = true;
            StartCoroutine(LoadMods());
        }
    }

    private void Init()
    {
        Console.WriteLine($"{Environment.NewLine}[MSCLoader Init]");
        string[] launchArgs = Environment.GetCommandLineArgs();
        Console.WriteLine("Launch arguments:");
        Console.WriteLine(string.Join(" ", launchArgs));
        if (launchArgs.Contains("-mscloader-devmode")) devMode = true;
        //Set config and Assets folder in selected mods folder
        ConfigFolder = Path.Combine(ModsFolder, "Config");
        SettingsFolder = Path.Combine(ConfigFolder, "Mod Settings");
        AssetsFolder = Path.Combine(ModsFolder, "Assets");

        //Move from old to new location if updated from before 1.1
        if (!Directory.Exists(SettingsFolder) && Directory.Exists(ConfigFolder))
        {
            Directory.CreateDirectory(SettingsFolder);
            foreach (string dir in Directory.GetDirectories(ConfigFolder))
            {
                if (new DirectoryInfo(dir).Name != "Mod Settings")
                {
                    try
                    {
                        Directory.Move(dir, Path.Combine(SettingsFolder, new DirectoryInfo(dir).Name));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.Message} (Failed to update folder structure)");
                    }
                }
            }
        }
        MetadataFolder = Path.Combine(ConfigFolder, "Mod Metadata");

        if (GameObject.Find("MSCUnloader") == null)
        {
            GameObject go = new GameObject("MSCUnloader", typeof(MSCUnloader));
            mscUnloader = go.GetComponent<MSCUnloader>();
            DontDestroyOnLoad(go);
        }
        else
        {
            mscUnloader = GameObject.Find("MSCUnloader").GetComponent<MSCUnloader>();
        }
        allModsLoaded = false;
        LoadedMods = new List<Mod>();
        InvalidMods = new List<InvalidMods>();
        mscUnloader.reset = false;
        if (!Directory.Exists(ModsFolder))
            Directory.CreateDirectory(ModsFolder);
        if (!Directory.Exists("Updates"))
            Directory.CreateDirectory("Updates");
        if (!Directory.Exists(Path.Combine("Updates", "Mods")))
            Directory.CreateDirectory(Path.Combine("Updates", "Mods"));
        if (!Directory.Exists(Path.Combine("Updates", "References")))
            Directory.CreateDirectory(Path.Combine("Updates", "References"));
        if (!Directory.Exists(Path.Combine("Updates", "Core")))
            Directory.CreateDirectory(Path.Combine("Updates", "Core"));

        if (!Directory.Exists(ConfigFolder))
        {
            Directory.CreateDirectory(ConfigFolder);
            Directory.CreateDirectory(SettingsFolder);
            Directory.CreateDirectory(MetadataFolder);
            Directory.CreateDirectory(Path.Combine(MetadataFolder, "Mod Icons"));
        }
        if (!Directory.Exists(MetadataFolder))
        {
            Directory.CreateDirectory(MetadataFolder);
            Directory.CreateDirectory(Path.Combine(MetadataFolder, "Mod Icons"));

        }
        if (!Directory.Exists(AssetsFolder))
            Directory.CreateDirectory(AssetsFolder);

        LoadCoreAssets();
        if (CheckVortexBS())
        {
            ModUI.ShowMessage($"<b><color=orange>DON'T use Vortex</color></b> to update MSCLoader, or to install tools or mods.{Environment.NewLine}<b><color=orange>Vortex isn't supported by MSCLoader</color></b>, because it's implementation breaks Mods folder by putting wrong files into it.{Environment.NewLine}{Environment.NewLine}MSCLoader will try to fix your mods folder now, <b><color=orange>please restart your game.</color></b>{Environment.NewLine}If this message shows again after restart, rebuild your Mods folder from scratch.", "Fatal Error");
            return;
        }

        LoadMod(new ModConsole(), MSCLoader_Ver);
        LoadedMods[0].A_ModSettings.Invoke();
        LoadMod(new ModMenu(), MSCLoader_Ver);
        LoadedMods[1].A_ModSettings.Invoke();
        ModMenu.LoadSettings();
        if (experimental)
            ModConsole.Print($"<color=lime>ModLoader <b><color=aqua>v{MSCLoader_Ver}</color></b> ready</color> [<color=magenta>Experimental</color> <color=lime>build {currentBuild}</color>]");
        else
            ModConsole.Print($"<color=lime>ModLoader <b><color=aqua>v{MSCLoader_Ver}</color></b> ready</color>");
        MainMenuInfo();
        ModsUpdateDir = Directory.GetFiles(Path.Combine("Updates", "Mods"), "*.zip");
        RefsUpdateDir = Directory.GetFiles(Path.Combine("Updates", "References"), "*.zip");
        if (ModsUpdateDir.Length == 0 && RefsUpdateDir.Length == 0)
        {
            ContinueInit();
        }
        else
        {
            UnpackUpdates();
        }
        if (launchArgs.Contains("-mscloader-disable")) ModUI.ShowMessage("To use <color=yellow>-mscloader-disable</color> launch option, you need to update the core module of MSCLoader, download latest version and launch <color=aqua>MSCLInstaller.exe</color> to update", "Outdated module");
    }
    void ContinueInit()
    {
        LoadReferences();
        try
        {
            if (File.Exists(Path.GetFullPath(Path.Combine("LAUNCHER.exe", ""))) || File.Exists(Path.GetFullPath(Path.Combine("SmartSteamEmu64.dll", ""))) || File.Exists(Path.GetFullPath(Path.Combine("SmartSteamEmu.dll", ""))))
            {
                ModConsole.Print($"<b><color=orange>Hello <color=lime>{"SmartSteamEmu!"}</color>!</color></b>");
                throw new Exception("[EMULATOR] Do What You Want, Cause A Pirate Is Free... You Are A Pirate!");
            }
            if (ModMetadata.CalculateFileChecksum(Path.Combine("", "steam_api.dll")) != "7B857C897BC69313E4936DC3DCCE5193")
            {
                ModConsole.Print($"<b><color=orange>Hello <color=lime>{"Emulator!"}</color>!</color></b>");
                throw new Exception("[EMULATOR] Do What You Want, Cause A Pirate Is Free... You Are A Pirate!");
            }
            if (ModMetadata.CalculateFileChecksum(Path.Combine("", "steam_api64.dll")) != "6C23EAB28F1CD1BFD128934B08288DD8")
            {
                ModConsole.Print($"<b><color=orange>Hello <color=lime>{"Emulator!"}</color>!</color></b>");
                throw new Exception("[EMULATOR] Do What You Want, Cause A Pirate Is Free... You Are A Pirate!");
            }

            Steamworks.SteamAPI.Init();
            steamID = Steamworks.SteamUser.GetSteamID().ToString();
            ModConsole.Print($"<b><color=orange>Hello <color=lime>{Steamworks.SteamFriends.GetPersonaName()}</color>!</color></b>");
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                webClient.DownloadStringCompleted += SAuthCheckCompleted;
                webClient.DownloadStringAsync(new Uri($"{serverURL}/sauth.php?sid={steamID}"));
            }
            if (LoadedMods.Count >= 100)
                Steamworks.SteamFriends.SetRichPresence("status", $"This madman is playing with {actualModList.Length} mods.");
            else if (LoadedMods.Count >= 50)
                Steamworks.SteamFriends.SetRichPresence("status", $"Playing with {actualModList.Length} mods. Crazy!");
            else
                Steamworks.SteamFriends.SetRichPresence("status", $"Playing with {actualModList.Length} mods.");
        }
        catch (Exception e)
        {
            steamID = null;
            ModConsole.Error("Steam client doesn't exists.");
            if (devMode)
                ModConsole.Error(e.ToString());
            Console.WriteLine(e);
            if (CheckSteam())
            {
                Console.WriteLine(new AccessViolationException().Message);
                Environment.Exit(0);
            }
        }
        SaveLoad.LoadModsSaveData();
        PreLoadMods();
        if (InvalidMods.Count > 0)
            ModConsole.Print($"<b><color=orange>Loaded <color=aqua>{actualModList.Length}</color> mods (<color=magenta>{InvalidMods.Count}</color> failed to load)!</color></b>{Environment.NewLine}");
        else
            ModConsole.Print($"<b><color=orange>Loaded <color=aqua>{actualModList.Length}</color> mods!</color></b>{Environment.NewLine}");
        MSCLInternal.LoadMSCLDataFile();
        LoadModsSettings();
        ModMenu.LoadBinds();
        GameObject old_callbacks = new GameObject("BC Callbacks");
        old_callbacks.transform.SetParent(gameObject.transform, false);
        if (OnGUImods.Length > 0) old_callbacks.AddComponent<BC_ModOnGUI>().modLoader = this;
        if (UpdateMods.Length > 0) old_callbacks.AddComponent<BC_ModUpdate>().modLoader = this;
        if (FixedUpdateMods.Length > 0) old_callbacks.AddComponent<BC_ModFixedUpdate>().modLoader = this;
        GameObject mod_callbacks = new GameObject("MSCLoader Callbacks");
        mod_callbacks.transform.SetParent(gameObject.transform, false);
        if (Mod_OnGUI.Length > 0) mod_callbacks.AddComponent<A_ModOnGUI>().modLoader = this;
        if (Mod_Update.Length > 0) mod_callbacks.AddComponent<A_ModUpdate>().modLoader = this;
        if (Mod_FixedUpdate.Length > 0) mod_callbacks.AddComponent<A_ModFixedUpdate>().modLoader = this;
        if (!returnToMainMenu)
        {
            CheckForModsUpd();
        }

        if (devMode)
            ModConsole.Warning("You are running ModLoader in <color=red><b>DevMode</b></color>, this mode is <b>only for modders</b> and shouldn't be used in normal gameplay.");
        System.Console.WriteLine(SystemInfoFix()); //operating system version to output_log.txt
        if (InvalidMods.Count > 0)
        {
            ModConsole.Error("Some files failed to load, scroll up to see more info.");
        }
        if (saveErrors != null)
        {
            if (saveErrors.Count > 0 && wasSaving)
            {
                ModUI.ShowMessage($"Some mods have thrown an error during saving{Environment.NewLine}Check console for more information!");
                for (int i = 0; i < saveErrors.Count; i++)
                {
                    ModConsole.Error(saveErrors[i]);
                }
            }
            wasSaving = false;
        }

    }
    internal static void HandleCanv(GameObject go)
    {
        Instance.HandleCCanv(go);
    }
    void HandleCCanv(GameObject go)
    {
        StartCoroutine(HandleCanvC(go));
    }
    IEnumerator HandleCanvC(GameObject go)
    {
        yield return null;
        go.SetActive(true);
    }
    internal void CheckForModsUpd(bool force = false)
    {
        string sp = Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "lastCheck"));
        if (force || ModMenu.cfmu_set == 0 || !File.Exists(sp))
        {
            DownloadUpdateData();
            File.WriteAllText(sp, DateTime.Now.ToString());
            return;
        }
        DateTime lastCheck;
        DateTime.TryParse(File.ReadAllText(sp), out lastCheck);
        if ((DateTime.Now - lastCheck).TotalDays >= ModMenu.cfmu_set || (DateTime.Now - lastCheck).TotalDays < 0)
        {
            DownloadUpdateData();
            File.WriteAllText(sp, DateTime.Now.ToString());
        }
        else
        {
            if (File.Exists(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "updateInfo.json"))))
            {
                string s = File.ReadAllText(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "updateInfo.json")));
                ModVersions v = JsonConvert.DeserializeObject<ModVersions>(s);
                ModMetadata.ReadUpdateInfo(v);
                if (File.Exists(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "ref_updateInfo.json"))))
                {
                    string s2 = File.ReadAllText(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "ref_updateInfo.json")));
                    RefVersions v2 = JsonConvert.DeserializeObject<RefVersions>(s2);
                    ModMetadata.ReadRefUpdateInfo(v2);
                }
                else
                {
                    ModMetadata.ReadRefUpdateInfo(null);
                }
            }
            else
            {
                DownloadUpdateData();
                File.WriteAllText(sp, DateTime.Now.ToString());
            }

        }

    }
    internal bool updChecked = false;
    private void DownloadUpdateData()
    {
        StartCoroutine(CheckForRefModUpdates());
        updChecked = true;
    }

    [Serializable]
    class SaveOtk
    {
        public string k1;
        public string k2;
    }

    private void SAuthCheckCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
        try
        {
            if (e.Error != null)
            {
                ModConsole.Error(e.Error.Message);
                Console.WriteLine(e.Error);
                return;
            }

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
                    SaveOtk s = new SaveOtk
                    {
                        k1 = ed[1],
                        k2 = ed[2]
                    };
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    string sp = Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "otk.bin"));
                    FileStream st = new FileStream(sp, FileMode.Create);
                    f.Serialize(st, s);
                    st.Close();
                }
                else
                {
                    Console.WriteLine("Unknown: " + ed[0]);
                    throw new Exception("Unknown server response.");
                }
            }
            bool ret = Steamworks.SteamApps.GetCurrentBetaName(out string Name, 128);
            if (ret && ModMenu.expWarning.GetValue())
            {
                if (!Name.StartsWith("default_")) //default is NOT experimental branch
                    ModUI.ShowMessage($"<color=orange><b>Warning:</b></color>{Environment.NewLine}You are using beta build: <color=orange><b>{Name}</b></color>{Environment.NewLine}{Environment.NewLine}Remember that some mods may not work correctly on beta branches.", "Experimental build warning");
            }
            Console.WriteLine($"MSC buildID: <b>{Steamworks.SteamApps.GetAppBuildId()}</b>");
            if (Steamworks.SteamApps.GetAppBuildId() < 100)
                throw new DivideByZeroException();
        }
        catch (Exception ex)
        {
            string sp = Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "otk.bin"));
            if (e.Error != null)
            {
                if (File.Exists(sp))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    FileStream st = new FileStream(sp, FileMode.Open);
                    SaveOtk s = f.Deserialize(st) as SaveOtk;
                    st.Close();
                    string otk = "otk_" + SidChecksumCalculator($"{steamID}{s.k1}");
                    if (s.k2.CompareTo(otk) != 0)
                    {
                        File.Delete(sp);
                        steamID = null;
                        ModConsole.Error("SteamAPI failed with error: " + ex.Message);
                        if (CheckSteam())
                        {
                            Console.WriteLine(new AccessViolationException().Message);
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        Console.WriteLine("offline");
                    }
                }
                else
                {
                    steamID = null;
                    ModConsole.Error("SteamAPI failed with error: " + ex.Message);
                    if (CheckSteam())
                    {
                        Console.WriteLine(new AccessViolationException().Message);
                        Environment.Exit(0);
                    }
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
                if (CheckSteam())
                {
                    Console.WriteLine(new AccessViolationException().Message);
                    Environment.Exit(0);
                }
            }
            Console.WriteLine(ex);
        }
    }

    private void LoadReferences()
    {
        if (Directory.Exists(Path.Combine(ModsFolder, "References")))
        {
            string[] files = Directory.GetFiles(Path.Combine(ModsFolder, "References"), "*.dll");
            string[] managedStuff = Directory.GetFiles(Path.Combine("mysummercar_Data", "Managed"), "*.dll");
            string[] alreadyIncluded = (from s in managedStuff select Path.GetFileName(s)).ToArray();
            string[] unusedFiles = new string[0];
            if (File.Exists(Path.Combine(Path.Combine(ModsFolder, "References"), "unused.txt")))
            {
                unusedFiles = File.ReadAllLines(Path.Combine(Path.Combine(ModsFolder, "References"), "unused.txt"));
            }
            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetFileName(files[i]) == "0Harmony12.dll" || Path.GetFileName(files[i]) == "0Harmony-1.2.dll" || alreadyIncluded.Contains(Path.GetFileName(files[i])))
                {
                    ModConsole.Warning($"<b>{Path.GetFileName(files[i])}</b> already exist in <b>{Path.GetFullPath(Path.Combine("mysummercar_Data", "Managed"))}</b> - skipping");
                    File.Delete(files[i]);
                    continue;
                }
                if (unusedFiles.Contains(Path.GetFileName(files[i])))
                {
                    ModConsole.Print($"<b>{Path.GetFileName(files[i])}</b> is marked as no longer needed after update - deleting");
                    File.Delete(files[i]);
                    continue;
                }
                try
                {
                    Assembly asm = Assembly.LoadFrom(files[i]);
                    LoadReferencesMeta(asm, files[i]);
                }
                catch (Exception e)
                {
                    ModConsole.Error($"<b>References/{Path.GetFileName(files[i])}</b> - Failed to load.");
                    Console.WriteLine(e);
                    References reference = new References()
                    {
                        FileName = Path.GetFileName(files[i]),
                        Invalid = true,
                        ExMessage = e.GetFullMessage()
                    };
                    ReferencesList.Add(reference);
                }
            }
            if (File.Exists(Path.Combine(Path.Combine(ModsFolder, "References"), "unused.txt")))
            {
                File.Delete(Path.Combine(Path.Combine(ModsFolder, "References"), "unused.txt"));
            }
        }
        else
        {
            Directory.CreateDirectory(Path.Combine(ModsFolder, "References"));
        }
    }
    private void LoadReferencesMeta(Assembly ass, string fn)
    {
        References reference = new References()
        {
            AssemblyID = ass.GetName().Name,
            FileName = fn
        };
        if (Attribute.IsDefined(ass, typeof(AssemblyTitleAttribute)))
            reference.AssemblyTitle = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(ass, typeof(AssemblyTitleAttribute))).Title;
        if (Attribute.IsDefined(ass, typeof(AssemblyCompanyAttribute)))
            reference.AssemblyAuthor = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(ass, typeof(AssemblyCompanyAttribute))).Company;
        if (Attribute.IsDefined(ass, typeof(AssemblyDescriptionAttribute)))
            reference.AssemblyDescription = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(ass, typeof(AssemblyDescriptionAttribute))).Description;
        if (Attribute.IsDefined(ass, typeof(AssemblyFileVersionAttribute)))
            reference.AssemblyFileVersion = ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(ass, typeof(AssemblyFileVersionAttribute))).Version;
        if (Attribute.IsDefined(ass, typeof(System.Runtime.InteropServices.GuidAttribute)))
            reference.Guid = ((System.Runtime.InteropServices.GuidAttribute)Attribute.GetCustomAttribute(ass, typeof(System.Runtime.InteropServices.GuidAttribute))).Value;
        ReferencesList.Add(reference);
        Console.WriteLine($"GUID: {reference.Guid}");
        Console.WriteLine($"ReferenceID: {reference.AssemblyID} (v{reference.AssemblyFileVersion}) [{reference.AssemblyAuthor}]");
    }
    private void LoadCoreAssets()
    {
        ModConsole.Print("Loading core assets...");
        AssetBundle ab = LoadAssets.LoadBundle("MSCLoader.CoreAssets.core.unity3d");
        guiskin = ab.LoadAsset<GUISkin>("MSCLoader.guiskin");
        ModUI.canvasPrefab = ab.LoadAsset<GameObject>("CanvasPrefab.prefab");
        mainMenuInfo = ab.LoadAsset<GameObject>("MSCLoader Info.prefab");
        GameObject loadingP = ab.LoadAsset<GameObject>("MSCLoader Canvas loading.prefab");
        GameObject mbP = ab.LoadAsset<GameObject>("MSCLoader Canvas msgbox.prefab");

        ModUI.PrepareDefaultCanvas();
        GameObject mb = GameObject.Instantiate(mbP);
        ModUI.messageBoxCv = mb.GetComponent<MessageBoxesCanvas>();
        DontDestroyOnLoad(mb);

        GameObject loading = GameObject.Instantiate(loadingP);
        canvLoading = loading.GetComponent<MSCLoaderCanvasLoading>();
        canvLoading.lHeader.text = $"MSCLOADER <color=green>{MSCLoader_Ver}</color>";
        DontDestroyOnLoad(loading);

        GameObject.Destroy(loadingP);
        GameObject.Destroy(mbP);
        ModConsole.Print("Loading core assets completed!");
        ab.Unload(false);
    }

    private bool CheckVortexBS()
    {
        if (File.Exists(Path.Combine(ModsFolder, "MSCLoader.dll")))
        {
            File.Delete(Path.Combine(ModsFolder, "MSCLoader.dll"));
            return true;
        }
        return false;
    }

    /// <summary>
    /// Toggle main menu path via settings
    /// </summary>
    internal static void MainMenuPath()
    {
        Instance.mainMenuInfo.transform.GetChild(1).gameObject.SetActive(ModMenu.modPath.GetValue());
    }

    private void MainMenuInfo()
    {
        Text info, mf;
        mainMenuInfo = Instantiate(mainMenuInfo);
        mainMenuInfo.name = "MSCLoader Info";
        menuInfoAnim = mainMenuInfo.GetComponent<Animation>();
        menuInfoAnim.Play("fade_in");
        info = mainMenuInfo.transform.GetChild(0).gameObject.GetComponent<Text>();
        mf = mainMenuInfo.transform.GetChild(1).gameObject.GetComponent<Text>();
        info.text = $"Mod Loader MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready! (<color=orange>Checking for updates...</color>)";
        CheckMSCLoaderVersion();
        mf.text = $"<color=orange>Mods folder:</color> {ModsFolder}";
        MainMenuPath();
        mainMenuInfo.transform.SetParent(ModUI.GetCanvas(0).transform, false);
    }

    private void CheckMSCLoaderVersion()
    {
        using (WebClient client = new WebClient())
        {
            client.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
            client.DownloadStringCompleted += VersionCheckCompleted;
            string branch = "unknown";
            if (experimental)
                branch = "msc_exp";
            else
                branch = "msc_stable";
            client.DownloadStringAsync(new Uri($"{serverURL}/mscl_corever.php?core={branch}"));
        }
    }

    private void VersionCheckCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
        Text info = mainMenuInfo.transform.GetChild(0).gameObject.GetComponent<Text>();
        try
        {
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }

            string[] result = e.Result.Split('|');
            switch (result[0])
            {
                case "error":
                    throw new Exception(result[1]);
                case "ok":
                    try
                    {
                        newBuild = int.Parse(result[2]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw new Exception("Failed to parse build number");
                    }

                    if (newBuild > currentBuild)
                    {
                        newVersion = result[1];
                        info.text = $"MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready!{(experimental ? $" [<color=magenta>Experimental</color> <color=lime>build {currentBuild}</color>]" : "")} (<color=aqua>Update is available</color>)";
                        ModloaderUpdateMessage = true;
                        MsgBoxBtn[] btn1 = { ModUI.CreateMessageBoxBtn("YES", DownloadModloaderUpdate), ModUI.CreateMessageBoxBtn("NO") };
                        MsgBoxBtn[] btn2 = { ModUI.CreateMessageBoxBtn("Show Changelog", ShowChangelog, new Color32(0, 0, 80, 255), Color.white, true) };
                        ModUI.ShowCustomMessage($"New ModLoader version is available{Environment.NewLine}<color=yellow>{MSCLoader_Ver} build {currentBuild}</color> => <color=lime>{newVersion} build {newBuild}</color>{Environment.NewLine}{Environment.NewLine}Do you want to download it now?", "MSCLoader update", btn1, btn2);
                    }
                    else
                    {
                        info.text = $"MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready!{(experimental ? $" [<color=magenta>Experimental</color> <color=lime>build {currentBuild}</color>]" : "")} (<color=lime>Up to date</color>)";
                    }
                    break;
                default:
                    Console.WriteLine("Unknown: " + result[0]);
                    throw new Exception("Unknown result");
            }
        }
        catch (Exception ex)
        {
            ModConsole.Error($"Check for new version failed with error: {ex.Message}");
            if (devMode)
                ModConsole.Error(ex.ToString());
            Console.WriteLine(ex);
            info.text = $"MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready!{(experimental ? $" [<color=magenta>Experimental</color> <color=lime>build {currentBuild}</color>]" : "")}";
        }
        if (devMode)
            info.text += " [<color=red><b>Dev Mode!</b></color>]";
    }
    internal void ShowChangelog()
    {
        string dwl = string.Empty;
        WebClient getdwl = new WebClient();
        getdwl.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
        try
        {
            dwl = getdwl.DownloadString($"{serverURL}/changelog.php?mods=MSCLoader&vers={newVersion}&names=MSCLoader");
        }
        catch (Exception e)
        {
            dwl = "<color=red>Failed to download changelog...</color>";
            Console.WriteLine(e);
        }
        ModUI.ShowChangelogWindow(dwl);
    }
    internal static void ModException(Exception e, Mod mod, bool save = false)
    {
        string errorDetails = $"{Environment.NewLine}<b>Details: </b>{e.Message} in <b>{new StackTrace(e, true).GetFrame(0).GetMethod()}</b>";
        if (save)
            saveErrors.Add($"Mod <b>{mod.ID}</b> has thrown an error!{errorDetails}");
        else
            ModConsole.Error($"Mod <b>{mod.ID}</b> has thrown an error!{errorDetails}");
        if (devMode)
        {
            if (save)
                saveErrors.Add(e.ToString());
            else
                ModConsole.Error(e.ToString());
        }
        Console.WriteLine(e);
    }

    IEnumerator NewGameMods()
    {
        ModConsole.Print("<color=aqua>==== Resetting mods ====</color>");
        SaveLoad.ResetSaveFile();
        canvLoading.SetLoading("Resetting mods", 0, BC_ModList.Length + Mod_OnNewGame.Length, "Preparing....");
        for (int i = 0; i < Mod_OnNewGame.Length; i++)
        {
            canvLoading.SetLoadingProgress(Mod_OnNewGame[i].Name);
            yield return null;
            try
            {
                Console.WriteLine($"Calling OnNewGame (for mod {Mod_OnNewGame[i].ID})");
                Mod_OnNewGame[i].A_OnNewGame.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_OnNewGame[i]);
            }

        }
        for (int i = 0; i < BC_ModList.Length; i++)
        {
            canvLoading.SetLoadingProgress(BC_ModList[i].Name);
            yield return null;
            try
            {
                Console.WriteLine($"Calling OnNewGame [old] (for mod {BC_ModList[i].ID})");
                BC_ModList[i].OnNewGame();
            }
            catch (Exception e)
            {
                ModException(e, BC_ModList[i]);
            }

        }
        canvLoading.SetLoadingStatus("Resetting Done! You can skip intro now!");
        yield return new WaitForSeconds(1f);
        canvLoading.ToggleLoadingUI(false);
        ModConsole.Print("<color=aqua>==== Resetting mods finished ====</color>");
        IsModsResetting = false;
    }
    void MouseFix()
    {
        PlayMakerFSM curFSM = GameObject.Find("PLAYER").GetPlayMaker("Update Cursor");
        FsmState curLock = curFSM.GetState("Update cursor");
        FsmState curUnLock = curFSM.GetState("In Menu");
        curLock.Actions[2] = new SetMouseCursorFix(true);
        curUnLock.Actions[1] = new SetMouseCursorFix(false);
    }

    IEnumerator LoadMods()
    {
        ModConsole.Print("<color=aqua>==== Loading mods (Phase 1) ====</color><color=#505050ff>");
        int totalCount = PLoadMods.Length + BC_ModList.Length + SecondPassMods.Length + Mod_PreLoad.Length + Mod_OnLoad.Length + Mod_PostLoad.Length;
        canvLoading.SetLoading("Loading mods - Phase 1", 0, totalCount, "Loading mods. Please wait...");
        yield return null;
        for (int i = 0; i < Mod_PreLoad.Length; i++)
        {
            canvLoading.SetLoadingProgress(Mod_PreLoad[i].ID);
            if (Mod_PreLoad[i].isDisabled) continue;
            yield return null;
            try
            {
                Console.WriteLine($"Calling PreLoad (for mod {Mod_PreLoad[i].ID})");
                Mod_PreLoad[i].A_PreLoad.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_PreLoad[i]);
            }
        }
        for (int i = 0; i < PLoadMods.Length; i++)
        {
            canvLoading.SetLoadingProgress(PLoadMods[i].ID);
            if (PLoadMods[i].isDisabled) continue;
            yield return null;
            try
            {
                Console.WriteLine($"Calling PreLoad [old] (for mod {PLoadMods[i].ID})");
                PLoadMods[i].PreLoad();
            }
            catch (Exception e)
            {
                ModException(e, PLoadMods[i]);
            }
        }
        canvLoading.SetLoadingTitle("Waiting...");
        canvLoading.SetLoadingStatus("Waiting for game to finish load...");
        while (GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera") == null)
            yield return null;
        MouseFix();
        GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera").AddComponent<UnifiedRaycast>();
        yield return null;
        canvLoading.SetLoadingTitle("Loading mods - Phase 2");
        canvLoading.SetLoadingStatus("Loading mods. Please wait...");
        ModConsole.Print("</color><color=aqua>==== Loading mods (Phase 2) ====</color><color=#505050ff>");
        yield return null;
        for (int i = 0; i < Mod_OnLoad.Length; i++)
        {
            canvLoading.SetLoadingProgress(Mod_OnLoad[i].ID);
            if (Mod_OnLoad[i].isDisabled) continue;
            yield return null;
            try
            {
                Console.WriteLine($"Calling OnLoad (for mod {Mod_OnLoad[i].ID})");
                Mod_OnLoad[i].A_OnLoad.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_OnLoad[i]);
            }
        }
        for (int i = 0; i < BC_ModList.Length; i++)
        {
            canvLoading.SetLoadingProgress(BC_ModList[i].ID);
            if (BC_ModList[i].isDisabled) continue;
            yield return null;
            try
            {
                Console.WriteLine($"Calling OnLoad [old] (for mod {BC_ModList[i].ID})");
                BC_ModList[i].OnLoad();
            }
            catch (Exception e)
            {
                ModException(e, BC_ModList[i]);
            }
        }
        ModMenu.LoadBinds();
        canvLoading.SetLoadingTitle("Loading mods - Phase 3");
        canvLoading.SetLoadingStatus("Loading mods. Please wait...");
        ModConsole.Print("</color><color=aqua>==== Loading mods (Phase 3) ====</color><color=#505050ff>");
        yield return null;
        for (int i = 0; i < Mod_PostLoad.Length; i++)
        {
            canvLoading.SetLoadingProgress(Mod_PostLoad[i].ID);
            if (Mod_PostLoad[i].isDisabled) continue;
            yield return null;
            try
            {
                Console.WriteLine($"Calling PostLoad (for mod {Mod_PostLoad[i].ID})");
                Mod_PostLoad[i].A_PostLoad.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_PostLoad[i]);
            }
        }
        for (int i = 0; i < SecondPassMods.Length; i++)
        {
            canvLoading.SetLoadingProgress(SecondPassMods[i].ID);
            if (SecondPassMods[i].isDisabled) continue;
            yield return null;
            try
            {
                Console.WriteLine($"Calling SecondPassOnLoad (for mod {SecondPassMods[i].ID})");
                SecondPassMods[i].SecondPassOnLoad();
            }
            catch (Exception e)
            {
                ModException(e, SecondPassMods[i]);
            }
        }
        canvLoading.SetLoadingProgress((int)canvLoading.lProgress.maxValue, (int)canvLoading.lProgress.maxValue);
        canvLoading.SetLoadingStatus("Finishing touches...");
        yield return null;
        GameObject.Find("ITEMS").GetComponent<PlayMakerFSM>().FsmInject("Save game", SaveMods);
        ModConsole.Print("</color>");
        if (IsReferencePresent("MSCCoreLibrary"))
            TimeSchedulerCalls("load");
        yield return null;
        allModsLoaded = true;
        canvLoading.ToggleLoadingUI(false);
    }

    private static bool wasSaving = false;
    private void SaveMods()
    {
        saveErrors = new List<string>();
        wasSaving = true;
        for (int i = 0; i < Mod_OnSave.Length; i++)
        {
            if (Mod_OnSave[i].isDisabled) continue;
            try
            {
                Console.WriteLine($"Calling OnSave (for mod {Mod_OnSave[i].ID})");
                Mod_OnSave[i].A_OnSave.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_OnSave[i], true);
            }
        }
        for (int i = 0; i < OnSaveMods.Length; i++)
        {
            if (OnSaveMods[i].isDisabled) continue;
            try
            {
                Console.WriteLine($"Calling OnSave [old] (for mod {OnSaveMods[i].ID})");
                OnSaveMods[i].OnSave();
            }
            catch (Exception e)
            {
                ModException(e, OnSaveMods[i], true);
            }
        }
        if (IsReferencePresent("MSCCoreLibrary"))
            TimeSchedulerCalls("save");
    }

    internal static bool CheckEmptyMethod(Mod mod, string methodName)
    {
        //TO TRASH
        MethodInfo method = mod.GetType().GetMethod(methodName);
        return (method.IsVirtual && method.DeclaringType == mod.GetType() && method.GetMethodBody().GetILAsByteArray().Length > 2);
    }

    private void LoadEADll(string file)
    {
        if (!CheckSteam()) return;
        string response = string.Empty;
        response = MSCLInternal.MSCLDataRequest("mscl_ea.php", new Dictionary<string, string> { { "steamID", steamID }, { "file", Path.GetFileNameWithoutExtension(file) } });
        string[] result = response.Split('|');
        switch (result[0])
        {
            case "error":
                ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - {result[1]}");
                break;
            case "ok":
                byte[] input;
                using (BinaryReader reader = new BinaryReader(File.OpenRead(file)))
                {
                    reader.BaseStream.Seek(3, SeekOrigin.Begin);
                    input = new byte[reader.BaseStream.Length - 3];
                    reader.Read(input, 0, input.Length);
                }
                byte[] key = Encoding.ASCII.GetBytes(result[1]);
                LoadDLL($"{Path.GetFileNameWithoutExtension(file)}.dll", input.Cry_ScrambleByteRightDec(key));
                break;
            default:
                Console.WriteLine(result);
                break;
        }
    }

    private void PreLoadMods()
    {
        // Load .dll files
        string[] files = Directory.GetFiles(ModsFolder, "*.dll");
        string[] unusedFiles = new string[0];
        if (File.Exists(Path.Combine(ModsFolder, "unused.txt")))
        {
            unusedFiles = File.ReadAllLines(Path.Combine(ModsFolder, "unused.txt"));
        }
        for (int i = 0; i < files.Length; i++)
        {
            if (unusedFiles.Contains(Path.GetFileName(files[i])))
            {
                ModConsole.Print($"<b>{Path.GetFileName(files[i])}</b>: has been marked as no longer needed after update - deleting");
                File.Delete(files[i]);
                continue;
            }

            if (MSCLInternal.IsEAFile(files[i]))
            {
                LoadEADll(files[i]);
            }
            else
            {
                LoadDLL(files[i]);
            }
        }
        if (File.Exists(Path.Combine(ModsFolder, "unused.txt")))
        {
            File.Delete(Path.Combine(ModsFolder, "unused.txt"));
        }
        actualModList = LoadedMods.Where(x => !x.ID.StartsWith("MSCLoader_")).ToArray();
        BC_ModList = actualModList.Where(x => !x.newFormat).ToArray();

        PLoadMods = BC_ModList.Where(x => CheckEmptyMethod(x, "PreLoad")).ToArray();
        SecondPassMods = BC_ModList.Where(x => CheckEmptyMethod(x, "PostLoad") || CheckEmptyMethod(x, "SecondPassOnLoad")).ToArray();
        OnGUImods = BC_ModList.Where(x => CheckEmptyMethod(x, "OnGUI")).ToArray();
        UpdateMods = BC_ModList.Where(x => CheckEmptyMethod(x, "Update")).ToArray();
        FixedUpdateMods = BC_ModList.Where(x => CheckEmptyMethod(x, "FixedUpdate")).ToArray();
        OnSaveMods = BC_ModList.Where(x => CheckEmptyMethod(x, "OnSave")).ToArray();

        Mod_OnNewGame = actualModList.Where(x => x.newFormat && x.A_OnNewGame != null).ToArray();
        Mod_PreLoad = actualModList.Where(x => x.newFormat && x.A_PreLoad != null).ToArray();
        Mod_OnLoad = actualModList.Where(x => x.newFormat && x.A_OnLoad != null).ToArray();
        Mod_PostLoad = actualModList.Where(x => x.newFormat && x.A_PostLoad != null).ToArray();
        Mod_OnSave = actualModList.Where(x => x.newFormat && x.A_OnSave != null).ToArray();
        Mod_OnGUI = actualModList.Where(x => x.newFormat && x.A_OnGUI != null).ToArray();
        Mod_Update = LoadedMods.Where(x => x.newFormat && x.A_Update != null).ToArray();
        Mod_FixedUpdate = actualModList.Where(x => x.newFormat && x.A_FixedUpdate != null).ToArray();
        GetRequiredFilesData();
        //cleanup files if not in dev mode
        if (!devMode)
        {
            string cleanupLast = Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "lastCleanupCheck"));
            if (File.Exists(cleanupLast))
            {
                string lastCheckS = File.ReadAllText(cleanupLast);
                DateTime.TryParse(lastCheckS, out DateTime lastCheck);
                if ((DateTime.Now - lastCheck).TotalDays >= 14 || (DateTime.Now - lastCheck).TotalDays < 0)
                {
                    bool found = false;
                    List<string> cleanupList = new List<string>();
                    foreach (string dir in Directory.GetDirectories(AssetsFolder))
                    {
                        if (!LoadedMods.Exists(x => x.ID == new DirectoryInfo(dir).Name))
                        {
                            found = true;
                            cleanupList.Add(new DirectoryInfo(dir).Name);
                        }
                    }
                    if (found)
                        ModUI.ShowYesNoMessage($"There are unused mod files/assets that can be cleaned up.{Environment.NewLine}{Environment.NewLine}List of unused mod files:{Environment.NewLine}<color=aqua>{string.Join(", ", cleanupList.ToArray())}</color>{Environment.NewLine}Do you want to clean them up?", "Unused files found", CleanupFolders);
                    File.WriteAllText(cleanupLast, DateTime.Now.ToString());
                }
            }
            else
            {
                File.WriteAllText(cleanupLast, DateTime.Now.ToString());
            }

        }

    }
    void GetRequiredFilesData()
    {
        //  modIDsReferences = new List<string>();
        if (modIDsReferences.Count == 0 && crashedGuids.Count == 0)
            return;
        List<string> missingModIDsReferences = new List<string>();
        for (int i = 0; i < modIDsReferences.Count; i++)
        {
            if (!ReferencesArePresent(GetModByID(modIDsReferences[i]).AdditionalReferences))
            {
                missingModIDsReferences.Add(modIDsReferences[i]);
            }
        }
        if (missingModIDsReferences.Count == 0 && crashedGuids.Count == 0)
            return;
        string response = MSCLInternal.MSCLDataRequest("mscl_required.php", new Dictionary<string, List<string>> { { "ModIDs", missingModIDsReferences }, { "guids", crashedGuids } });
        //RequiredList
        if (response.StartsWith("error"))
        {
            string[] result = response.Split('|');
            ModConsole.Error(result[1]);
        }
        else if (response.StartsWith("{"))
        {
            RequiredList list = JsonConvert.DeserializeObject<RequiredList>(response);
            List<string> refsToDwl = new List<string>();
            List<string> modsToDwl = new List<string>();
            for (int i = 0; i < list.references.Count; i++)
            {
                if (!IsReferencePresent(list.references[i]))
                    refsToDwl.Add(list.references[i]);
            }
            for (int i = 0; i < list.mods.Count; i++)
            {
                if (!IsModPresent(list.mods[i], true))
                    modsToDwl.Add(list.mods[i]);
            }
            DownloadRequiredFiles(refsToDwl, modsToDwl);
        }
        else
        {
            Console.WriteLine(response);
        }
    }

    void CleanupFolders()
    {
        string[] setFold = Directory.GetDirectories(SettingsFolder);
        for (int i = 0; i < setFold.Length; i++)
        {
            if (!LoadedMods.Exists(x => x.ID == new DirectoryInfo(setFold[i]).Name))
            {
                try
                {
                    Directory.Delete(setFold[i], true);
                }
                catch (Exception ex)
                {
                    ModConsole.Error($"{ex.Message} (corrupted file?)");
                }
            }
        }
        string[] assFold = Directory.GetDirectories(AssetsFolder);
        for (int i = 0; i < assFold.Length; i++)
        {
            if (!LoadedMods.Exists(x => x.ID == new DirectoryInfo(assFold[i]).Name))
            {
                try
                {
                    Directory.Delete(assFold[i], true);
                }
                catch (Exception ex)
                {
                    ModConsole.Error($"{ex.Message} (corrupted file?)");
                }
            }
        }
    }
    private void LoadModsSettings()
    {
        for (int i = 0; i < LoadedMods.Count; i++)
        {
            if (LoadedMods[i].ID.StartsWith("MSCLoader_"))
                continue;
            ModMetadata.ReadMetadata(LoadedMods[i]);
            try
            {
                Settings.ModSettings(LoadedMods[i]);
                if (LoadedMods[i].newSettingsFormat)
                {
                    if (LoadedMods[i].A_ModSettings != null)
                    {
                        LoadedMods[i].A_ModSettings.Invoke();
                    }
                }
                else
                    LoadedMods[i].ModSettings();
            }
            catch (Exception e)
            {
                if (LoadedMods[i].proSettings) System.Console.WriteLine(e); // No need to spam console with pro settings errors.
                else
                {
                    ModConsole.Error($"Settings error for mod <b>{LoadedMods[i].ID}</b>{Environment.NewLine}<b>Details:</b> {e.Message}");
                    if (devMode)
                        ModConsole.Error(e.ToString());
                    System.Console.WriteLine(e);
                }
            }
        }
        ModMenu.LoadSettings(); //Maybe put that in same loop as load so we don't need do another loop.
    }

    private void LoadDLL(string file, byte[] byteFile = null)
    {
        bool mscl = false;
        HashSet<string> addRef = new HashSet<string>();
        string asmGuid = "unknown";
        try
        {
            Assembly asm = null;
            if (byteFile == null)
                asm = Assembly.LoadFrom(file);
            else
                asm = Assembly.Load(byteFile);
            bool isMod = false;
            AssemblyName[] list = asm.GetReferencedAssemblies();
            if (Attribute.IsDefined(asm, typeof(System.Runtime.InteropServices.GuidAttribute)))
                asmGuid = ((System.Runtime.InteropServices.GuidAttribute)Attribute.GetCustomAttribute(asm, typeof(System.Runtime.InteropServices.GuidAttribute))).Value;
            Console.WriteLine($"GUID: {asmGuid}");
            string msVer = null;
            for (int i = 0; i < list.Length; i++)
            {
                if (!stdRef.Contains(list[i].Name))
                {
                    addRef.Add(list[i].Name);
                }
                if (list[i].Name == "MSCLoader")
                {
                    mscl = true;
                    string[] verparse = list[i].Version.ToString().Split('.');
                    if (list[i].Version.ToString() == "1.0.0.0")
                        msVer = "0.1";
                    else
                    {
                        if (verparse[2] == "0")
                            msVer = $"{verparse[0]}.{verparse[1]}";
                        else
                            msVer = $"{verparse[0]}.{verparse[1]}.{verparse[2]}";
                    }
                }
            }

            //Warn about wrong .net target, source of some mod crashes.
            if (!asm.ImageRuntimeVersion.Equals(Assembly.GetExecutingAssembly().ImageRuntimeVersion))
                ModConsole.Warning($"File <b>{Path.GetFileName(file)}</b> is targeting runtime version <b>{asm.ImageRuntimeVersion}</b> which is different than current running version <b>{Assembly.GetExecutingAssembly().ImageRuntimeVersion}</b>. This may cause unexpected behaviours, check your target assembly.");

            // Look through all public classes
            Type[] asmTypes = asm.GetTypes();

            for (int j = 0; j < asmTypes.Length; j++)
            {
                if (asmTypes[j] == null) continue;

                // Console.WriteLine($"{file} - {asmTypes[j].Name}"); //dbg

                if (asmTypes[j].IsSubclassOf(typeof(Mod)))
                {
                    Mod m = (Mod)Activator.CreateInstance(asmTypes[j]);
                    if (m.ID.StartsWith("MSCLoader_")) continue;
                    if (string.IsNullOrEmpty(m.ID.Trim()))
                    {
                        //Do not allow null/empty/whitespace modID.
                        Console.Write("Empty mod ID");
                        continue;
                    }
                    m.asmGuid = asmGuid;
                    isMod = true;
                    if (addRef.Count > 0)
                        LoadMod(m, msVer, file, addRef.ToArray());
                    else
                        LoadMod(m, msVer, file);
                    break;
                }
                else
                {
                    isMod = false;
                }
            }
            if (!isMod)
            {
                crashedGuids.Add(asmGuid);
                ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - doesn't look like a mod or missing Mod subclass!{Environment.NewLine}<b>Details:</b> File loaded correctly, but failed to find Mod methods.{Environment.NewLine}If this is a reference put this file into \"<b>References</b>\" folder.{Environment.NewLine}");
                InvalidMods.Add(new InvalidMods(Path.GetFileName(file), true, "File loaded correctly, but failed to find Mod methods.", addRef.ToList(), asmGuid));
            }
        }
        catch (Exception e)
        {
            if (mscl)
            {
                crashedGuids.Add(asmGuid);
                ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - crashed during load.<b>Details:</b> {e.GetFullMessage()}");
                InvalidMods.Add(new InvalidMods(Path.GetFileName(file), true, e.GetFullMessage(), addRef.ToList(), asmGuid));

                if (addRef.Count > 0)
                {
                    List<string> filteredRef = new List<string>();
                    foreach (string s in addRef)
                    {
                        if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == s))
                        {
                            filteredRef.Add(s);
                        }
                    }
                    if (filteredRef.Count > 0)
                        ModConsole.Print($"<color=red>Potential missing files: </color><color=aqua>{string.Join(", ", filteredRef.ToArray())}</color>{Environment.NewLine}");
                }
            }
            else
            {
                if (byteFile != null)
                {
                    ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - failed to load this as valid early access file. Most likely there is new updated file available!{Environment.NewLine}<b>Details:</b> {e.GetType().Name}{Environment.NewLine}");
                    InvalidMods.Add(new InvalidMods(Path.GetFileName(file), false, "failed to load this as valid early access file. Most likely there is new updated file available!"));
                }
                else
                {
                    if (string.IsNullOrEmpty(e.Message))
                        ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - doesn't look like a mod, remove this file from mods folder!{Environment.NewLine}<b>Details:</b> {e.GetType().Name}{Environment.NewLine}");
                    else
                        ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - doesn't look like a mod, remove this file from mods folder!{Environment.NewLine}<b>Details:</b> {e.GetFullMessage()}{Environment.NewLine}");
                    InvalidMods.Add(new InvalidMods(Path.GetFileName(file), false, e.GetFullMessage()));
                }
            }
            if (devMode)
                ModConsole.Error(e.ToString());
            Console.WriteLine(e);
            //InvalidMods.Add(Path.GetFileName(file));
        }

    }
    private bool ReferencesArePresent(string[] refs)
    {
        int count = 0;
        Assembly[] LoadedAsms = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < LoadedAsms.Length; i++)
        {
            if (refs.Contains(LoadedAsms[i].GetName().Name))
            {
                count++;
                if (count == refs.Length)
                    return true;
            }
        }
        return false;
    }
    private void LoadMod(Mod mod, string msver, string fname = null, string[] additionalRef = null)
    {
        string pm = string.Empty;
        // Check if mod already exists
        if (!LoadedMods.Contains(mod) && !LoadedMods.Select(x => x.ID).Contains(mod.ID))
        {
            LoadedMods.Add(mod);
            if (mod.Description != null) pm = mod.Description.Contains(MSCLInternal.ProLoaderMagic()) ? "*" : "";
            Console.WriteLine($"Detected As: {mod.Name} (ID: {mod.ID}) v{mod.Version} (by {mod.Author}) {pm}");
            // Create config folder
            if (!Directory.Exists(Path.Combine(SettingsFolder, mod.ID)))
            {
                Directory.CreateDirectory(Path.Combine(SettingsFolder, mod.ID));
            }
            mod.compiledVersion = msver;
            mod.fileName = fname;
            mod.AdditionalReferences = additionalRef;
            if (mod.AdditionalReferences != null)
            {
                modIDsReferences.Add(mod.ID);
            }
            try
            {
                Console.WriteLine($"Calling ModSetup (for mod {mod.ID})");
                mod.ModSetup();
                if (mod.newFormat && mod.fileName == null)
                {
                    Console.WriteLine($"Calling OnMenuLoad (for mod {mod.ID})");
                    mod.A_OnMenuLoad?.Invoke();
                }
            }
            catch (Exception e)
            {
                ModException(e, mod);
            }
        }
        else
        {
            ModConsole.Error($"Mod already loaded (or duplicated ID): <b>{mod.ID}</b>");
        }
    }

    internal void A_OnGUI()
    {
        GUI.skin = guiskin;
        for (int i = 0; i < Mod_OnGUI.Length; i++)
        {
            if (Mod_OnGUI[i].isDisabled)
                continue;
            try
            {
                if (allModsLoaded || Mod_OnGUI[i].menuCallbacks)
                    Mod_OnGUI[i].A_OnGUI.Invoke();
            }
            catch (Exception e)
            {
                ModExceptionHandler(e, Mod_OnGUI[i]);
            }
        }
    }

    internal void BC_OnGUI()
    {
        GUI.skin = guiskin;
        for (int i = 0; i < OnGUImods.Length; i++)
        {
            if (OnGUImods[i].isDisabled)
                continue;
            try
            {
                if (allModsLoaded || OnGUImods[i].LoadInMenu)
                    OnGUImods[i].OnGUI();
            }
            catch (Exception e)
            {
                ModExceptionHandler(e, OnGUImods[i]);
            }
        }
    }
    internal void A_Update()
    {
        for (int i = 0; i < Mod_Update.Length; i++)
        {
            if (Mod_Update[i].isDisabled)
                continue;
            try
            {
                if (allModsLoaded || Mod_Update[i].menuCallbacks)
                    Mod_Update[i].A_Update.Invoke();
            }
            catch (Exception e)
            {
                ModExceptionHandler(e, Mod_Update[i]);
            }
        }
    }

    internal void BC_Update()
    {
        for (int i = 0; i < UpdateMods.Length; i++)
        {
            if (UpdateMods[i].isDisabled)
                continue;
            try
            {
                if (allModsLoaded || UpdateMods[i].LoadInMenu)
                    UpdateMods[i].Update();
            }
            catch (Exception e)
            {
                ModExceptionHandler(e, UpdateMods[i]);
            }
        }
    }
    internal void A_FixedUpdate()
    {
        for (int i = 0; i < Mod_FixedUpdate.Length; i++)
        {
            if (Mod_FixedUpdate[i].isDisabled)
                continue;
            try
            {
                if (allModsLoaded || Mod_FixedUpdate[i].LoadInMenu)
                    Mod_FixedUpdate[i].A_FixedUpdate.Invoke();
            }
            catch (Exception e)
            {
                ModExceptionHandler(e, Mod_FixedUpdate[i]);
            }
        }
    }
    internal void BC_FixedUpdate()
    {
        for (int i = 0; i < FixedUpdateMods.Length; i++)
        {
            if (FixedUpdateMods[i].isDisabled)
                continue;
            try
            {
                if (allModsLoaded || FixedUpdateMods[i].LoadInMenu)
                    FixedUpdateMods[i].FixedUpdate();
            }
            catch (Exception e)
            {
                ModExceptionHandler(e, FixedUpdateMods[i]);
            }
        }
    }

    void ModExceptionHandler(Exception e, Mod mod)
    {
        if (LogAllErrors)
        {
            ModException(e, mod);
        }
        if (allModsLoaded)
            mod.modErrors++;
        if (devMode)
        {
            if (mod.modErrors == 30)
            {
                ModConsole.Error($"Mod <b>{mod.ID}</b> spams <b>too many errors each frame</b>! Last error: ");
                ModConsole.Error(e.ToString());
                if (ModMenu.dm_disabler.GetValue())
                {
                    mod.isDisabled = true;
                    ModConsole.Warning($"[DevMode] Mod <b>{mod.ID}</b> has been disabled!");
                }
                else
                {
                    ModConsole.Warning($"[DevMode] Mod <b>{mod.ID}</b> is still running!");
                }
            }
        }
        else
        {
            if (mod.modErrors >= 30)
            {
                mod.isDisabled = true;
                ModConsole.Error($"Mod <b>{mod.ID}</b> has been <b>disabled!</b> Because it spams too many errors each frame!{Environment.NewLine}Report this problem to mod author.{Environment.NewLine}Last error message:");
                ModConsole.Error(e.GetFullMessage());
                Console.WriteLine(e);
            }
        }
    }

    internal static string SidChecksumCalculator(string rawData)
    {
        System.Security.Cryptography.SHA1 sha256 = System.Security.Cryptography.SHA1.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }
    internal static string SystemInfoFix()
    {
        string Sinfo = SystemInfo.operatingSystem;
        try
        {
            if (Sinfo.Contains("Windows"))
            {
                string windowsfixed = Sinfo;
                int build = int.Parse(Sinfo.Split('(')[1].Split(')')[0].Split('.')[2]);
                if (build > 21999)
                {
                    windowsfixed = $"Windows 11 (10.0.{build})";
                    if (Sinfo.Contains("64bit"))
                        windowsfixed += " 64bit";
                    return windowsfixed;
                }
                else if (build > 9841)
                {
                    windowsfixed = $"Windows 10 (10.0.{build})";
                    if (Sinfo.Contains("64bit"))
                        windowsfixed += " 64bit";
                    return windowsfixed;
                }
                else return Sinfo;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return Sinfo;
    }
}
#endif