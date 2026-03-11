#if !Mini
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MSCLoader;

public partial class ModLoader
{
    internal static Game currentGame = Game.MySummerCar;
    internal static CurrentScene currentScene = CurrentScene.MainMenu;

    internal static bool LogAllErrors = false;
    internal static List<InvalidMods> InvalidMods;
    internal static List<Mod> IncompatibleMods = new List<Mod>();
    internal static ModLoader Instance;
    internal static bool unloader = false;
    internal static bool returnToMainMenu = false;
    internal static List<string> saveErrors;

    internal Mod[] actualModList = [];
#if MSC
    internal Mod[] BC_ModList = [];
#endif
    internal static List<Mod> HasUpdateModList = new List<Mod>();
    internal static List<References> HasUpdateRefList = new List<References>();
    internal List<string> crashedGuids = new List<string>();
    internal List<string> modIDsReferences = new List<string>();
    internal List<References> ReferencesList = new List<References>();
    internal string[] stdRef = ["mscorlib", "System.Core", "UnityEngine", "PlayMaker", "MSCLoader", "System", "Assembly-CSharp", "Assembly-CSharp-firstpass", "Assembly-UnityScript", "Assembly-UnityScript-firstpass", "ES2", "Ionic.Zip", "Ionic.Zip.Reduced", "UnityEngine.UI", "0Harmony", "cInput", "Newtonsoft.Json", "System.Xml"];

#if MSC
    //Old stuff
    internal Mod[] PLoadMods = [];
    internal Mod[] SecondPassMods = [];
    internal Mod[] OnGUImods = [];
    internal Mod[] UpdateMods = [];
    internal Mod[] FixedUpdateMods = [];
    internal Mod[] OnSaveMods = [];
#endif
    //New Stuff
    internal Mod[] Mod_OnNewGame = [];   //When New Game is started
    internal Mod[] Mod_PreLoad = [];     //Phase 1 (mod loading)
    internal Mod[] Mod_OnLoad = [];      //Phase 2 (mod loading)  
    internal Mod[] Mod_PostLoad = [];    //Phase 3 (mod loading)
    internal Mod[] Mod_OnSave = [];      //When game saves
    internal Mod[] Mod_OnGUI = [];       //Calls unity OnGUI
    internal Mod[] Mod_Update = [];      //Calls unity Update
    internal Mod[] Mod_FixedUpdate = []; //Calls unity FixedUpdate

    internal int currentBuild = MSCLInfo.Build;
    internal int newBuild = 0;
    internal string newVersion = MSCLoader_Ver;
    internal MSCUnloader mscUnloader;

    internal static string steamID;
    internal static bool loaderPrepared = false;
    internal static bool initCalled = false;
#if MSC
    internal static string ModsFolder = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine("MySummerCar", "Mods")));
#elif MWC
    internal static string ModsFolder = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine("My Winter Car", "Mods")));
#endif
    internal static string ManagedPath = Environment.GetEnvironmentVariable("DOORSTOP_MANAGED_FOLDER_DIR");
    internal static string ConfigFolder = Path.Combine(ModsFolder, "Config");
    internal static string SettingsFolder = Path.Combine(ConfigFolder, "Mod Settings");
    internal static string MetadataFolder = Path.Combine(ConfigFolder, "Mod Metadata");
    internal static string AssetsFolder = Path.Combine(ModsFolder, "Assets");
    internal string[] ModsUpdateDir;
    internal string[] RefsUpdateDir;
    internal static List<string> ModSelfUpdateList = new List<string>();
    internal static List<string> RefSelfUpdateList = new List<string>();

    internal List<string> MetadataUpdateList = new List<string>();
    internal GameObject mainMenuInfo;
    internal Animation menuInfoAnim;
    internal GUISkin guiskin;

    internal static readonly string serverURL = "http://my-summer-car.ovh"; //Main url
#if MSC
    internal static readonly string metadataURL = "man_v3/";
#elif MWC
    internal static readonly string metadataURL = "mwc_man/";
#endif
    // internal static readonly string serverURL = "http://localhost/msc2"; //localhost for testing only

    internal bool IsModsLoading = false;
    internal bool allModsLoaded = false;
    internal bool IsModsResetting = false;
    internal bool ModloaderUpdateMessage = false;

    internal static bool devMode = false;
    internal static string GetMetadataFolder(string fn) => Path.Combine(MetadataFolder, fn);

    internal static Mod GetModByID(string modID, bool includeDisabled = false)
    {
        Mod m = LoadedMods.Where(x => x.ID.Equals(modID)).FirstOrDefault();
        if (includeDisabled) return m; //if include disabled is true then just return (can be null)
        if (!m.isDisabled) return m; //if include disabled is false we go here to check if mod is not disabled and return it.
        return null; //null if any above if is false
    }

    void OnApplicationQuit()
    {
        //Save current log as "previous"
        try
        {
            string logPath = GetOutputLogPath();
            if (logPath != null)
            {
                string prevPath = GetOutputLogPreviousPath();
                if (File.Exists(prevPath)) File.Delete(prevPath);
                File.Copy(logPath, prevPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to copy output log: {ex.Message}");
        }
    }

    /// <summary>
    /// Resolves the path to the Unity output log file.
    /// Windows/Proton: output_log.txt in game directory.
    /// Linux native: Player.log in Application.persistentDataPath.
    /// </summary>
    internal static string GetOutputLogPath()
    {
        if (File.Exists("output_log.txt"))
            return Path.GetFullPath("output_log.txt");
        string playerLog = Path.Combine(Application.persistentDataPath, "Player.log");
        if (File.Exists(playerLog))
            return playerLog;
        if (File.Exists("Player.log"))
            return Path.GetFullPath("Player.log");
        return null;
    }

    internal static string GetOutputLogPreviousPath()
    {
        string logPath = GetOutputLogPath();
        if (logPath == null) return null;
        string dir = Path.GetDirectoryName(logPath);
        string name = Path.GetFileNameWithoutExtension(logPath);
        string ext = Path.GetExtension(logPath);
        return Path.Combine(dir, $"{name}_previous{ext}");
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
                }
                else if (build > 9841)
                {
                    windowsfixed = $"Windows 10 (10.0.{build})";
                    if (Sinfo.Contains("64bit"))
                        windowsfixed += " 64bit";
                }
                else
                {
                    windowsfixed = Sinfo;
                }

                // Detect Wine/Proton layer when running on Linux
                string wineTag = DetectWineLayer();
                if (!string.IsNullOrEmpty(wineTag))
                    windowsfixed += $" {wineTag}";

                return windowsfixed;
            }

            // Native Linux detection
            if (Sinfo.Contains("Linux"))
            {
                string distroInfo = GetLinuxDistroInfo();
                if (!string.IsNullOrEmpty(distroInfo))
                    return $"{Sinfo} ({distroInfo})";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return Sinfo;
    }

    /// <summary>
    /// Detect if running under Wine or Proton.
    /// Returns "[Proton]" or "[Wine]" or null.
    /// </summary>
    private static string DetectWineLayer()
    {
        try
        {
            // Proton sets STEAM_COMPAT_DATA_PATH
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("STEAM_COMPAT_DATA_PATH")))
                return "[Proton]";
            // Wine sets WINEPREFIX or WINELOADERNOEXEC
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WINEPREFIX"))
                || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WINELOADERNOEXEC")))
                return "[Wine]";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return null;
    }

    /// <summary>
    /// Read Linux distribution info from /etc/os-release.
    /// Returns "Ubuntu 24.04" style string, or null.
    /// </summary>
    private static string GetLinuxDistroInfo()
    {
        try
        {
            string osRelease = "/etc/os-release";
            if (!File.Exists(osRelease)) return null;
            string name = null;
            string version = null;
            foreach (string line in File.ReadAllLines(osRelease))
            {
                if (line.StartsWith("NAME="))
                    name = line.Substring(5).Trim('"');
                else if (line.StartsWith("VERSION_ID="))
                    version = line.Substring(11).Trim('"');
            }
            if (name != null)
                return version != null ? $"{name} {version}" : name;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return null;
    }
}

#endif