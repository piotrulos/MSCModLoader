global using UnityEngine;
using System;
#if !Mini
using Newtonsoft.Json;
#endif
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
        ModsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine("MySummerCar", "Mods"));
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
                break;
            case "Intro":
                CurrentScene = CurrentScene.NewGameIntro;

                if (!IsModsDoneResetting && !IsModsResetting)
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
                StartLoadingMods(!ModMenu.syncLoad.GetValue());
                ModMenu.ModMenuHandle();
                break;
            case "Ending":
                CurrentScene = CurrentScene.Ending;
                break;
        }
    }

    private void StartLoadingMods(bool async)
    {
        if (!allModsLoaded && !IsModsLoading)
        {
            IsModsLoading = true;
            if (async)
                StartCoroutine(LoadModsAsync());
            else
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
        InvalidMods = new List<string>();
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
            ModUI.ShowMessage($"<b><color=orange>DON'T use Vortex</color></b> to update MSCLoader, or to install tools or mods.{Environment.NewLine}<b><color=orange>Vortex isn't supported by MSCLoader</color></b>, because it's implementation is broken that breaks Mods folder by putting wrong files into it.{Environment.NewLine}{Environment.NewLine}MSCLoader will try to fix your mods folder now, <b><color=orange>please restart your game.</color></b>{Environment.NewLine}If this message shows again after restart, rebuild your Mods folder from scratch.", "Fatal Error");
            return;
        }
        LoadMod(new ModConsole(), MSCLoader_Ver);
        LoadedMods[0].ModSettings();
        LoadMod(new ModMenu(), MSCLoader_Ver);
        LoadedMods[1].ModSettings();
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
        if (launchArgs.Contains("-mscloader-disable")) ModUI.ShowMessage("To use <color=yellow>-mscloader-disable</color> launch option, you need to update core module of MSCLoader, download latest version and launch <color=aqua>MSCPatcher.exe</color> to update","Outdated module");
    }
    void ContinueInit()
    {
#if !Mini
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
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
            webClient.DownloadStringCompleted += SAuthCheckCompleted;
            webClient.DownloadStringAsync(new Uri($"{serverURL}/sauth.php?sid={steamID}"));
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
            System.Console.WriteLine(e);
            if (CheckSteam())
            {
                System.Console.WriteLine(new AccessViolationException().Message);
                Environment.Exit(0);
            }
        }
        PreLoadMods();
        if (InvalidMods.Count > 0)
            ModConsole.Print($"<b><color=orange>Loaded <color=aqua>{actualModList.Length}</color> mods (<color=magenta>{InvalidMods.Count}</color> failed to load)!</color></b>{Environment.NewLine}");
        else
            ModConsole.Print($"<b><color=orange>Loaded <color=aqua>{actualModList.Length}</color> mods!</color></b>{Environment.NewLine}");
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
        if (!rtmm)
        {
            CheckForModsUpd();
        }

        if (devMode)
            ModConsole.Warning("You are running ModLoader in <color=red><b>DevMode</b></color>, this mode is <b>only for modders</b> and shouldn't be use in normal gameplay.");
        System.Console.WriteLine(SystemInfoFix()); //operating system version to output_log.txt

        if (saveErrors != null)
        {
            if (saveErrors.Count > 0 && wasSaving)
            {
                ModUI.ShowMessage($"Some mod thrown an error during saving{Environment.NewLine}Check console for more information!");
                for (int i = 0; i < saveErrors.Count; i++)
                {
                    ModConsole.Error(saveErrors[i]);
                }
            }
            wasSaving = false;
        }
#endif
    }
    internal void CheckForModsUpd(bool force = false)
    {
#if !Mini
        string sp = Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "lastCheck"));
        if (force)
        {
            DownloadUpdateData();
            File.WriteAllText(sp, DateTime.Now.ToString());
            return;
        }
        if (ModMenu.cfmu_set != 0)
        {
            if (File.Exists(sp))
            {
                DateTime lastCheck;
                string lastCheckS = File.ReadAllText(sp);
                DateTime.TryParse(lastCheckS, out lastCheck);
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
                            ModMetadata.ReadRefUpdateInfo(new RefVersions());
                        }
                    }
                    else
                    {
                        DownloadUpdateData();
                        File.WriteAllText(sp, DateTime.Now.ToString());
                    }

                }
            }
            else
            {
                DownloadUpdateData();
                File.WriteAllText(sp, DateTime.Now.ToString());
            }
        }
        else
        {
            DownloadUpdateData();
            File.WriteAllText(sp, DateTime.Now.ToString());
        }
#endif
    }

    private void DownloadUpdateData()
    {
        StartCoroutine(CheckForRefModUpdates());
    }
    private void ModsUpdateDataProgress(object sender, UploadProgressChangedEventArgs e)
    {
        cfmuInProgress = true;
    }
    private void ModsUpdateData(object sender, UploadValuesCompletedEventArgs e)
    {
        cfmuInProgress = false;
        if (e.Error != null)
        {
            ModConsole.Error("Failed to check for mods updates");
            ModConsole.Error(e.Error.Message);
            Console.WriteLine(e.Error);
            cfmuErrored = true;
            return;
        }
        else
        {
            cfmuResult = System.Text.Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
        }
    }

    [Serializable]
    class SaveOtk
    {
        public string k1;
        public string k2;
    }

    private void SAuthCheckCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
#if !Mini
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
                    System.Console.WriteLine("Unknown: " + ed[0]);
                    throw new Exception("Unknown server response.");
                }
            }
            bool ret = Steamworks.SteamApps.GetCurrentBetaName(out string Name, 128);
            if (ret && ModMenu.expWarning.GetValue())
            {
                if (!Name.StartsWith("default_")) //default is NOT experimental branch
                    ModUI.ShowMessage($"<color=orange><b>Warning:</b></color>{Environment.NewLine}You are using beta build: <color=orange><b>{Name}</b></color>{Environment.NewLine}{Environment.NewLine}Remember that some mods may not work correctly on beta branches.", "Experimental build warning");
            }
            System.Console.WriteLine($"MSC buildID: <b>{Steamworks.SteamApps.GetAppBuildId()}</b>");
            if (Steamworks.SteamApps.GetAppBuildId() == 1)
                throw new DivideByZeroException();
        }
        catch (Exception ex)
        {
            string sp = Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings","otk.bin"));
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
                            System.Console.WriteLine(new AccessViolationException().Message);
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("offline");
                    }
                }
                else
                {
                    steamID = null;
                    ModConsole.Error("SteamAPI failed with error: " + ex.Message);
                    if (CheckSteam())
                    {
                        System.Console.WriteLine(new AccessViolationException().Message);
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
                    System.Console.WriteLine(new AccessViolationException().Message);
                    Environment.Exit(0);
                }
            }
            System.Console.WriteLine(ex);
        }
#endif
    }

    private void LoadReferences()
    {
        if (Directory.Exists(Path.Combine(ModsFolder, "References")))
        {
            string[] files = Directory.GetFiles(Path.Combine(ModsFolder, "References"), "*.dll");
            string[] managedStuff = Directory.GetFiles(Path.Combine("mysummercar_Data", "Managed"), "*.dll");
            string[] alreadyIncluded = (from s in managedStuff select Path.GetFileName(s)).ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetFileName(files[i]) == "0Harmony12.dll" || Path.GetFileName(files[i]) == "0Harmony-1.2.dll" || alreadyIncluded.Contains(Path.GetFileName(files[i])))
                {
                    ModConsole.Warning($"<b>{Path.GetFileName(files[i])}</b> already exist in <b>{Path.GetFullPath(Path.Combine("mysummercar_Data", "Managed"))}</b> - skipping");
                    File.Delete(files[i]);
                    continue;
                }
                try
                {
                    Assembly asm = Assembly.LoadFrom(files[i]);
                    if (asm.EntryPoint != null)
                    {
                        asm.EntryPoint.Invoke(null, new object[] { new string[] { MSCLoader_Ver } });
                    }
                    LoadReferencesMeta(asm, files[i]);
                }
                catch (Exception e)
                {
                    ModConsole.Error($"<b>References/{Path.GetFileName(files[i])}</b> - Failed to load.");
                    References reference = new References()
                    {
                        FileName = Path.GetFileName(files[i]),
                        Invalid = true,
                        ExMessage = e.GetFullMessage()
                    };
                    ReferencesList.Add(reference);

                }
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

    }
    private void LoadCoreAssets()
    {
        ModConsole.Print("Loading core assets...");
        AssetBundle ab = LoadAssets.LoadBundle("MSCLoader.CoreAssets.core.unity3d");
        guiskin = ab.LoadAsset<GUISkin>("MSCLoader.guiskin");
        ModUI.canvasPrefab = ab.LoadAsset<GameObject>("CanvasPrefab.prefab");
        mainMenuInfo = ab.LoadAsset<GameObject>("MSCLoader Info.prefab");
        GameObject loadingP = ab.LoadAsset<GameObject>("LoadingCanvas.prefab");
        GameObject mbP = ab.LoadAsset<GameObject>("PopupsCanvas.prefab");

        ModUI.CreateCanvases();
        GameObject mb = GameObject.Instantiate(mbP);
        mb.name = "MSCLoader Canvas msgbox";
        ModUI.messageBoxCv = mb.GetComponent<MessageBoxesCanvas>();
        DontDestroyOnLoad(mb);
 
        GameObject loading = GameObject.Instantiate(loadingP);
        loading.name = "MSCLoader Canvas loading";
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
        //Clean vortex BS when some idiot try to use it to update modloader.
        bool hardFail = false;
        if (File.Exists(Path.Combine(ModsFolder, "INIFileParser.dll")))
            File.Delete(Path.Combine(ModsFolder, "INIFileParser.dll"));
        if (File.Exists(Path.Combine(ModsFolder, "Ionic.Zip.dll")))
            File.Delete(Path.Combine(ModsFolder, "Ionic.Zip.dll"));
        if (File.Exists(Path.Combine(ModsFolder, "Mono.Cecil.dll")))
            File.Delete(Path.Combine(ModsFolder, "Mono.Cecil.dll"));
        if (File.Exists(Path.Combine(ModsFolder, "Mono.Cecil.Rocks.dll")))
            File.Delete(Path.Combine(ModsFolder, "Mono.Cecil.Rocks.dll"));
        if (File.Exists(Path.Combine(ModsFolder, "MSCPatcher.exe")))
            File.Delete(Path.Combine(ModsFolder, "MSCPatcher.exe"));
        if (File.Exists(Path.Combine(ModsFolder, "w32.dll")))
            File.Delete(Path.Combine(ModsFolder, "w32.dll"));
        if (File.Exists(Path.Combine(ModsFolder, "w64.dll")))
            File.Delete(Path.Combine(ModsFolder, "w64.dll"));
        if (File.Exists(Path.Combine(ModsFolder, "winhttp.dll")))
        {
            File.Delete(Path.Combine(ModsFolder, "winhttp.dll"));
            hardFail = true;
        }

        if (File.Exists(Path.Combine(ModsFolder, "MSCLoader.dll")))
        {
            File.Delete(Path.Combine(ModsFolder, "MSCLoader.dll"));
            hardFail = true;
        }

        return hardFail;
    }

    /// <summary>
    /// Toggle main menu path via settings
    /// </summary>
    internal static void MainMenuPath()
    {
        Instance.mainMenuInfo.transform.GetChild(1).gameObject.SetActive(ModMenu.modPath.GetValue());
    }
    Text modUpdates;
    private void MainMenuInfo()
    {
        Text info, mf;
        mainMenuInfo = Instantiate(mainMenuInfo);
        mainMenuInfo.name = "MSCLoader Info";
        menuInfoAnim = mainMenuInfo.GetComponent<Animation>();
        menuInfoAnim.Play("fade_in");
        info = mainMenuInfo.transform.GetChild(0).gameObject.GetComponent<Text>();
        mf = mainMenuInfo.transform.GetChild(1).gameObject.GetComponent<Text>();
        modUpdates = mainMenuInfo.transform.GetChild(2).gameObject.GetComponent<Text>();
        info.text = $"Mod Loader MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready! (<color=orange>Checking for updates...</color>)";
        WebClient client = new WebClient();
        client.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");

        //client.Proxy = new WebProxy("127.0.0.1:8888"); //ONLY FOR TESTING
        client.DownloadStringCompleted += VersionCheckCompleted;
        string branch = "unknown";
        if (experimental)
            branch = "exp";
        else
            branch = "stable";
        client.DownloadStringAsync(new Uri($"{serverURL}/ver.php?core={branch}"));

        mf.text = $"<color=orange>Mods folder:</color> {ModsFolder}";
        MainMenuPath();
        modUpdates.text = string.Empty;
        mainMenuInfo.transform.SetParent(ModUI.GetCanvas(0).transform, false);
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
                int newBuild = 0;
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
                    if (experimental)
                        info.text = $"MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready! [<color=magenta>Experimental</color> <color=lime>build {currentBuild}</color>] (<color=aqua>Update is available</color>)";
                    else
                        info.text = $"MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready! (<color=aqua>Update is available</color>)";
                    ModloaderUpdateMessage = true;
                    MsgBoxBtn[] btn1 = { ModUI.CreateMessageBoxBtn("YES", DownloadModloaderUpdate), ModUI.CreateMessageBoxBtn("NO") };
                    MsgBoxBtn[] btn2 = { ModUI.CreateMessageBoxBtn("Show Changelog", ShowChangelog, new Color32(0, 0, 80, 255), Color.white, true) };
                    ModUI.ShowCustomMessage($"New ModLoader version is available{Environment.NewLine}<color=yellow>{MSCLoader_Ver} build {currentBuild}</color> => <color=lime>{newVersion} build {newBuild}</color>{Environment.NewLine}{Environment.NewLine}Do you want to download it now?", "MSCLoader update", btn1, btn2);
                }
                else
                {
                    if (experimental)
                        info.text = $"MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready! [<color=magenta>Experimental</color> <color=lime>build {currentBuild}</color>]";
                    else
                        info.text = $"MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready! (<color=lime>Up to date</color>)";
                }
            }
            else
            {
                Console.WriteLine("Unknown: " + result[0]);
                throw new Exception("Unknown server response.");
            }
        }
        catch (Exception ex)
        {
            ModConsole.Error($"Check for new version failed with error: {ex.Message}");
            if (devMode)
                ModConsole.Error(ex.ToString());
            Console.WriteLine(ex);
            if (experimental)
                info.text = $"MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready! [<color=magenta>Experimental</color> <color=lime>build {currentBuild}</color>]";
            else
                info.text = $"MSCLoader <color=cyan>v{MSCLoader_Ver}</color> is ready!";

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
            saveErrors.Add($"Mod <b>{mod.ID}</b> throw an error!{errorDetails}");
        else
            ModConsole.Error($"Mod <b>{mod.ID}</b> throw an error!{errorDetails}");
        if (devMode)
        {
            if (save)
                saveErrors.Add(e.ToString());
            else
                ModConsole.Error(e.ToString());
        }
        System.Console.WriteLine(e);
    }
    MSCLoaderCanvasLoading canvLoading;
    IEnumerator NewGameMods()
    {
        ModConsole.Print("<color=aqua>==== Resetting mods ====</color>");
        canvLoading.modLoadingUI.transform.SetAsLastSibling(); //Always on top
        canvLoading.modLoadingUI.SetActive(true);
        canvLoading.lTitle.text = "Resetting mods".ToUpper();
        canvLoading.lProgress.maxValue = BC_ModList.Length + Mod_OnNewGame.Length;
        for (int i = 0; i < Mod_OnNewGame.Length; i++)
        {
            canvLoading.lProgress.value++;
            canvLoading.lMod.text = Mod_OnNewGame[i].Name;
            yield return null;
            try
            {
                Mod_OnNewGame[i].A_OnNewGame.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_OnNewGame[i]);
            }

        }
        for (int i = 0; i < BC_ModList.Length; i++)
        {
            canvLoading.lProgress.value++;
            canvLoading.lMod.text = BC_ModList[i].Name;
            yield return null;
            try
            {
                BC_ModList[i].OnNewGame();
            }
            catch (Exception e)
            {
                ModException(e, BC_ModList[i]);
            }

        }
        canvLoading.lMod.text = "Resetting Done! You can skip intro now!";
        yield return new WaitForSeconds(1f);
        canvLoading.modLoadingUI.SetActive(false);
        IsModsDoneResetting = true;
        ModConsole.Print("<color=aqua>==== Resetting mods finished ====</color>");
        IsModsResetting = false;
    }

    IEnumerator LoadMods()
    {
        ModConsole.Print("<color=aqua>==== Loading mods (Phase 1) ====</color><color=#505050ff>");
        canvLoading.lTitle.text = "Loading mods - Phase 1".ToUpper();
        canvLoading.lMod.text = "Loading mods. Please wait...";
        canvLoading.lProgress.maxValue = 100;
        canvLoading.modLoadingUI.transform.SetAsLastSibling(); //Always on top
        canvLoading.modLoadingUI.SetActive(true);
        yield return null;
        for (int i = 0; i < Mod_PreLoad.Length; i++)
        {
            if (Mod_PreLoad[i].isDisabled) continue;
            try
            {
                Mod_PreLoad[i].A_PreLoad.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_PreLoad[i]);
            }
        }
        for (int i = 0; i < PLoadMods.Length; i++)
        {
            if (PLoadMods[i].isDisabled) continue;
            try
            {
                PLoadMods[i].PreLoad();
            }
            catch (Exception e)
            {
                ModException(e, PLoadMods[i]);
            }
        }
        canvLoading.lProgress.value = 33;
        canvLoading.lTitle.text = "Waiting...".ToUpper();
        canvLoading.lMod.text = "Waiting for game to finish load...";
        while (GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera") == null)
            yield return null;
        canvLoading.lTitle.text = "Loading mods - Phase 2".ToUpper();
        canvLoading.lMod.text = "Loading mods. Please wait...";
        ModConsole.Print("</color><color=aqua>==== Loading mods (Phase 2) ====</color><color=#505050ff>");
        yield return null;
        for (int i = 0; i < Mod_OnLoad.Length; i++)
        {
            if (Mod_OnLoad[i].isDisabled) continue;
            try
            {
                Mod_OnLoad[i].A_OnLoad.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_OnLoad[i]);
            }
        }
        for (int i = 0; i < BC_ModList.Length; i++)
        {
            if (BC_ModList[i].isDisabled) continue;
            try
            {
                BC_ModList[i].OnLoad();
            }
            catch (Exception e)
            {
                ModException(e, BC_ModList[i]);
            }
        }
        canvLoading.lProgress.value = 66;
        ModMenu.LoadBinds();
        canvLoading.lTitle.text = "Loading mods - Phase 3".ToUpper();
        canvLoading.lMod.text = "Loading mods. Please wait...";
        ModConsole.Print("</color><color=aqua>==== Loading mods (Phase 3) ====</color><color=#505050ff>");
        yield return null;
        for (int i = 0; i < Mod_PostLoad.Length; i++)
        {
            if (Mod_PostLoad[i].isDisabled) continue;
            try
            {
                Mod_PostLoad[i].A_PostLoad.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_PostLoad[i]);
            }
        }
        for (int i = 0; i < SecondPassMods.Length; i++)
        {
            if (SecondPassMods[i].isDisabled) continue;
            try
            {
                SecondPassMods[i].SecondPassOnLoad();
            }
            catch (Exception e)
            {
                ModException(e, SecondPassMods[i]);
            }
        }

        canvLoading.lProgress.value = 100;
        canvLoading.lMod.text = "Finishing touches...";
        yield return null;
        #if !Mini
        GameObject.Find("ITEMS").FsmInject("Save game", SaveMods);
#endif
        ModConsole.Print("</color>");
        allModsLoaded = true;
        canvLoading.modLoadingUI.SetActive(false);
    }

    IEnumerator LoadModsAsync()
    {
        ModConsole.Print("<color=aqua>==== Loading mods (Phase 1) ====</color><color=#505050ff>");
        canvLoading.lTitle.text = "Loading mods - Phase 1".ToUpper();
        canvLoading.lMod.text = "Loading mods. Please wait...";
        canvLoading.lProgress.maxValue = PLoadMods.Length + BC_ModList.Length + SecondPassMods.Length;
        canvLoading.lProgress.maxValue += Mod_PreLoad.Length + Mod_OnLoad.Length + Mod_PostLoad.Length;

        canvLoading.modLoadingUI.transform.SetAsLastSibling(); //Always on top
        canvLoading.modLoadingUI.SetActive(true);
        yield return null;
        for (int i = 0; i < Mod_PreLoad.Length; i++)
        {
            canvLoading.lProgress.value++;
            canvLoading.lMod.text = Mod_PreLoad[i].ID;
            if (Mod_PreLoad[i].isDisabled) continue;
            yield return null;
            try
            {
                Mod_PreLoad[i].A_PreLoad.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_PreLoad[i]);
            }
        }
        for (int i = 0; i < PLoadMods.Length; i++)
        {
            canvLoading.lProgress.value++;
            canvLoading.lMod.text = PLoadMods[i].ID;
            if (PLoadMods[i].isDisabled) continue;
            yield return null;
            try
            {
                PLoadMods[i].PreLoad();
            }
            catch (Exception e)
            {
                ModException(e, PLoadMods[i]);
            }
        }
        canvLoading.lTitle.text = "Waiting...".ToUpper();
        canvLoading.lMod.text = "Waiting for game to finish load...";
        while (GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera") == null)
            yield return null;
        canvLoading.lTitle.text = "Loading mods - Phase 2".ToUpper();
        canvLoading.lMod.text = "Loading mods. Please wait...";
        ModConsole.Print("</color><color=aqua>==== Loading mods (Phase 2) ====</color><color=#505050ff>");
        yield return null;
        for (int i = 0; i < Mod_OnLoad.Length; i++)
        {
            canvLoading.lProgress.value++;
            canvLoading.lMod.text = Mod_OnLoad[i].ID;
            if (Mod_OnLoad[i].isDisabled) continue;
            yield return null;
            try
            {
                Mod_OnLoad[i].A_OnLoad.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_OnLoad[i]);
            }
        }
        for (int i = 0; i < BC_ModList.Length; i++)
        {
            canvLoading.lProgress.value++;
            canvLoading.lMod.text = BC_ModList[i].ID;
            if (BC_ModList[i].isDisabled) continue;
            yield return null;
            try
            {
                BC_ModList[i].OnLoad();
            }
            catch (Exception e)
            {
                ModException(e, BC_ModList[i]);
            }
        }
        ModMenu.LoadBinds();
        canvLoading.lTitle.text = "Loading mods - Phase 3".ToUpper();
        canvLoading.lMod.text = "Loading mods. Please wait...";
        ModConsole.Print("</color><color=aqua>==== Loading mods (Phase 3) ====</color><color=#505050ff>");
        yield return null;
        for (int i = 0; i < Mod_PostLoad.Length; i++)
        {
            canvLoading.lProgress.value++;
            canvLoading.lMod.text = Mod_PostLoad[i].ID;
            if (Mod_PostLoad[i].isDisabled) continue;
            yield return null;
            try
            {
                Mod_PostLoad[i].A_PostLoad.Invoke();
            }
            catch (Exception e)
            {
                ModException(e, Mod_PostLoad[i]);
            }
        }
        for (int i = 0; i < SecondPassMods.Length; i++)
        {
            canvLoading.lProgress.value++;
            canvLoading.lMod.text = SecondPassMods[i].ID;
            if (SecondPassMods[i].isDisabled) continue;
            yield return null;
            try
            {
                SecondPassMods[i].SecondPassOnLoad();
            }
            catch (Exception e)
            {
                ModException(e, SecondPassMods[i]);
            }
        }
        canvLoading.lProgress.value = canvLoading.lProgress.maxValue;
        canvLoading.lMod.text = "Finishing touches...";
        yield return null;
        #if !Mini
        GameObject.Find("ITEMS").FsmInject("Save game", SaveMods);
#endif
        ModConsole.Print("</color>");
        allModsLoaded = true;
        canvLoading.modLoadingUI.SetActive(false);
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
                OnSaveMods[i].OnSave();
            }
            catch (Exception e)
            {
                ModException(e, OnSaveMods[i], true);
            }
        }
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
        string dwl = string.Empty;
        WebClient getdwl = new WebClient();
        getdwl.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
        try
        {
            dwl = getdwl.DownloadString($"{earlyAccessURL}/ea_test?steam={steamID}&file={Path.GetFileNameWithoutExtension(file)}");
        }
        catch (Exception e)
        {
            ModConsole.Error($"Failed to check early access info for mod {Path.GetFileNameWithoutExtension(file)}");
            Console.WriteLine(e);
        }
        string[] result = dwl.Split('|');
        if (result[0] == "error")
        {
            switch (result[1])
            {
                case "0":
                    ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - Failed to check early access info: Invalid request{Environment.NewLine}");
                    break;
                case "1":
                    ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - You are not whitelisted to use this mod{Environment.NewLine}");
                    break;
                case "2":
                    ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - File not registered/Early Access ended{Environment.NewLine}");
                    break;
                default:
                    ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - Failed to check early access info: Unknown error{Environment.NewLine}");
                    break;
            }
        }
        else if (result[0] == "ok")
        {
            byte[] input = File.ReadAllBytes(file);
            byte[] key = Encoding.ASCII.GetBytes(result[1]);
            LoadDLL($"{Path.GetFileNameWithoutExtension(file)}.dll", input.Cry_ScrambleByteRightDec(key));
        }
    }
    private void PreLoadMods()
    {
        // Load .dll files
        string[] files = Directory.GetFiles(ModsFolder);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".dll"))
            {
                LoadDLL(files[i]);
            }
            if (files[i].EndsWith(".dII"))
            {
                if (files.Contains(files[i].Replace(".dII", ".dll")))
                {
                    ModConsole.Error($"<b>{Path.GetFileName(files[i])}</b> - normal .dll version of this .dii file already exists in Mods folder, please delete one to avoid issues. <b>(skipping loading .dii version)</b>{Environment.NewLine}");
                }
                else
                {
                    LoadEADll(files[i]);
                }
            }
        }
        actualModList = LoadedMods.Where(x => !x.ID.StartsWith("MSCLoader_")).ToArray();
        BC_ModList = actualModList.Where(x => !x.newFormat).ToArray();

        PLoadMods = BC_ModList.Where(x => CheckEmptyMethod(x, "PreLoad")).ToArray();
        SecondPassMods = BC_ModList.Where(x => x.SecondPass || CheckEmptyMethod(x, "PostLoad")).ToArray();
        OnGUImods = BC_ModList.Where(x => CheckEmptyMethod(x, "OnGUI") || CheckEmptyMethod(x, "MenuOnGUI")).ToArray();
        UpdateMods = BC_ModList.Where(x => CheckEmptyMethod(x, "Update") || CheckEmptyMethod(x, "MenuUpdate")).ToArray();
        FixedUpdateMods = BC_ModList.Where(x => CheckEmptyMethod(x, "FixedUpdate") || CheckEmptyMethod(x, "MenuFixedUpdate")).ToArray();
        OnSaveMods = BC_ModList.Where(x => CheckEmptyMethod(x, "OnSave")).ToArray();

        Mod_OnNewGame = actualModList.Where(x => x.newFormat && x.A_OnNewGame != null).ToArray();
        Mod_PreLoad = actualModList.Where(x => x.newFormat && x.A_PreLoad != null).ToArray();
        Mod_OnLoad = actualModList.Where(x => x.newFormat && x.A_OnLoad != null).ToArray();
        Mod_PostLoad = actualModList.Where(x => x.newFormat && x.A_PostLoad != null).ToArray();
        Mod_OnSave = actualModList.Where(x => x.newFormat && x.A_OnSave != null).ToArray();
        Mod_OnGUI = actualModList.Where(x => x.newFormat && x.A_OnGUI != null).ToArray();
        Mod_Update = LoadedMods.Where(x => x.newFormat && x.A_Update != null).ToArray();
        Mod_FixedUpdate = actualModList.Where(x => x.newFormat && x.A_FixedUpdate != null).ToArray();

        //cleanup files if not in dev mode
        if (!devMode)
        {

            string cleanupLast = Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings","lastCleanupCheck"));
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
            ModMetadata.ReadMetadata(LoadedMods[i]);
            if (LoadedMods[i].ID.StartsWith("MSCLoader_"))
                continue;
            try
            {
                LoadedMods[i].ModSettings();
            }
            catch (Exception e)
            {
                if (LoadedMods[i].proSettings) System.Console.WriteLine(e);
                else
                {
                    ModConsole.Error($"Settings error for mod <b>{LoadedMods[i].ID}</b>{Environment.NewLine}<b>Details:</b> {e.Message}");
                    if (devMode)
                        ModConsole.Error(e.ToString());
                    System.Console.WriteLine(e);
                }
            }
        }
        ModMenu.LoadSettings();
    }

    private void LoadDLL(string file, byte[] byteFile = null)
    {
        bool mscl = false;
        List<string> addRef = new List<string>();
        try
        {
            Assembly asm = null;
            if (byteFile == null)
                asm = Assembly.LoadFrom(file);
            else
                asm = Assembly.Load(byteFile);
            bool isMod = false;
            AssemblyName[] list = asm.GetReferencedAssemblies();

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
                ModConsole.Warning($"File <b>{Path.GetFileName(file)}</b> is targeting runtime version <b>{asm.ImageRuntimeVersion}</b> which is different that current running version <b>{Assembly.GetExecutingAssembly().ImageRuntimeVersion}</b>. This may cause unexpected behaviours, check your target assembly.");

            // Look through all public classes                
            Type[] asmTypes = asm.GetTypes();
            for (int j = 0; j < asmTypes.Length; j++)
            {
                if (typeof(Mod).IsAssignableFrom(asmTypes[j]))
                {
                    Mod m = (Mod)Activator.CreateInstance(asmTypes[j]);
                    if (m.ID.StartsWith("MSCLoader_")) continue;
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
                ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - doesn't look like a mod or missing Mod subclass!{Environment.NewLine}<b>Details:</b> File loaded correctly, but failed to find Mod methods.{Environment.NewLine}If this is a reference put this file into \"<b>References</b>\" folder.");
                InvalidMods.Add(Path.GetFileName(file));
            }
        }
        catch (Exception e)
        {
            if (mscl)
            {
                ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - crashed during load.<b>Details:</b> {e.GetFullMessage()}{Environment.NewLine}");
                if (addRef.Count > 0)
                {
                    if ((addRef.Contains("MSCLoader.Features") && !ReferencesList.Select(x => x.AssemblyID).Contains("MSCLoader.Features")) || (addRef.Contains("MSCLoader.Helpers") && !ReferencesList.Select(x => x.AssemblyID).Contains("MSCLoader.Helpers")))
                        ModUI.ShowYesNoMessage($"<color=yellow>{Path.GetFileName(file)}</color> - looks like a mod, but It crashed trying to load.{Environment.NewLine}{Environment.NewLine}Detected additional references used by this mod: {Environment.NewLine}<color=aqua>{string.Join(", ", addRef.ToArray())}</color> {Environment.NewLine}{Environment.NewLine} Looks like missing compatibility pack.{Environment.NewLine} Open download page?", "Crashed", delegate
                         {
                             Application.OpenURL("https://www.nexusmods.com/mysummercar/mods/732");
                         });
                    else
                        ModUI.ShowMessage($"<color=yellow>{Path.GetFileName(file)}</color> - looks like a mod, but It crashed trying to load.{Environment.NewLine} {Environment.NewLine}Detected additional references used by this mod: {Environment.NewLine}<color=aqua>{string.Join(", ", addRef.ToArray())}</color> {Environment.NewLine}{Environment.NewLine}Check mod download page for required references.", "Crashed");
                }
                else
                    ModUI.ShowMessage($"<color=yellow>{Path.GetFileName(file)}</color> - looks like a mod, but It crashed trying to load.{Environment.NewLine}Reason: Unknown", "Crashed");

            }
            else
            {
                if (byteFile != null)
                {
                    ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - failed to load this as valid early access file. Most likely there is new updated file available!{Environment.NewLine}<b>Details:</b> {e.GetType().Name}{Environment.NewLine}");
                }
                else
                {
                    if (string.IsNullOrEmpty(e.Message))
                        ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - doesn't look like a mod, remove this file from mods folder!{Environment.NewLine}<b>Details:</b> {e.GetType().Name}{Environment.NewLine}");
                    else
                        ModConsole.Error($"<b>{Path.GetFileName(file)}</b> - doesn't look like a mod, remove this file from mods folder!{Environment.NewLine}<b>Details:</b> {e.GetFullMessage()}{Environment.NewLine}");
                }
            }
            if (devMode)
                ModConsole.Error(e.ToString());
            System.Console.WriteLine(e);
            InvalidMods.Add(Path.GetFileName(file));
        }

    }

    private void LoadMod(Mod mod, string msver, string fname = null, string[] additionalRef = null)
    {
        // Check if mod already exists
        if (!LoadedMods.Contains(mod) && !LoadedMods.Select(x => x.ID).Contains(mod.ID))
        {
            LoadedMods.Add(mod);
            Console.WriteLine($"Detected As: {mod.Name} (ID: {mod.ID}) v{mod.Version}");
            // Create config folder
            if (!Directory.Exists(Path.Combine(SettingsFolder, mod.ID)))
            {
                Directory.CreateDirectory(Path.Combine(SettingsFolder, mod.ID));
            }
            mod.compiledVersion = msver;
            mod.fileName = fname;
            mod.AdditionalReferences = additionalRef;
            try
            {
                mod.ModSetup();
                if (mod.newFormat && mod.fileName == null)
                {
                    mod.A_OnMenuLoad?.Invoke();
                }
            }
            catch (Exception e)
            {
                ModException(e, mod);
            }
            mod.metadata = ModMetadata.LoadMetadata(mod);
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
        if (allModsLoaded && fullyLoaded)
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
            }
        }
    }

    internal static string SidChecksumCalculator(string rawData)
    {
        System.Security.Cryptography.SHA1 sha256 = System.Security.Cryptography.SHA1.Create();
        byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }
    internal static string SystemInfoFix()
    {
        try
        {
            if (SystemInfo.operatingSystem.Contains("Windows"))
            {
                string Sinfo = SystemInfo.operatingSystem;
                int build = int.Parse(Sinfo.Split('(')[1].Split(')')[0].Split('.')[2]);
                if (build > 9841)
                {
                    string windows10fix = $"Windows 10 (10.0.{build})";
                    if (SystemInfo.operatingSystem.Contains("64bit"))
                        windows10fix += " 64bit";

                    return windows10fix;
                }
                else return SystemInfo.operatingSystem;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return SystemInfo.operatingSystem;
    }
}
