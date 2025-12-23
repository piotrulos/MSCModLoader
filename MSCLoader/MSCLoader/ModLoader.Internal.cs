#if !Mini
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSCLoader;

public partial class ModLoader
{
    internal static Game CurrentGame = Game.MySummerCar;

    internal static bool LogAllErrors = false;
    internal static List<InvalidMods> InvalidMods;
    internal static ModLoader Instance;
    internal static bool unloader = false;
    internal static bool returnToMainMenu = false;
    internal static List<string> saveErrors;

    internal Mod[] actualModList = [];
    internal Mod[] BC_ModList = [];
    internal static List<Mod> HasUpdateModList = new List<Mod>();
    internal static List<References> HasUpdateRefList = new List<References>();
    internal List<string> crashedGuids = new List<string>();
    internal List<string> modIDsReferences = new List<string>();
    internal List<References> ReferencesList = new List<References>();
    internal string[] stdRef = ["mscorlib", "System.Core", "UnityEngine", "PlayMaker", "MSCLoader", "System", "Assembly-CSharp", "Assembly-CSharp-firstpass", "Assembly-UnityScript", "Assembly-UnityScript-firstpass", "ES2", "Ionic.Zip", "Ionic.Zip.Reduced", "UnityEngine.UI", "0Harmony", "cInput", "Newtonsoft.Json", "System.Xml"];

    //Old stuff
    internal Mod[] PLoadMods = [];
    internal Mod[] SecondPassMods = [];
    internal Mod[] OnGUImods = [];
    internal Mod[] UpdateMods = [];
    internal Mod[] FixedUpdateMods = [];
    internal Mod[] OnSaveMods = [];

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
    internal static string ModsFolder = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine("MySummerCar", "Mods")));
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
    internal static readonly string metadataURL = "man_v3/";
    //internal static readonly string serverURL = "http://localhost/msc2"; //localhost for testing only

    internal bool IsModsLoading = false;
    internal bool allModsLoaded = false;
    internal bool IsModsResetting = false;
    internal bool ModloaderUpdateMessage = false;

    internal static Mod GetModByID(string modID, bool includeDisabled = false)
    {
        Mod m = LoadedMods.Where(x => x.ID.Equals(modID)).FirstOrDefault();
        if (includeDisabled) return m; //if include disabled is true then just return (can be null)
        if (!m.isDisabled) return m; //if include disabled is false we go here to check if mod is not disabled and return it.
        return null; //null if any above if is false
    }
}

#endif