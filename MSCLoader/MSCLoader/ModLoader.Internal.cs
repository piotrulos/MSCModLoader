#if !Mini
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MSCLoader;

public partial class ModLoader
{
    internal static bool LogAllErrors = false;
    internal static List<InvalidMods> InvalidMods;
    internal static ModLoader Instance;
    internal static bool unloader = false;
    internal static bool returnToMainMenu = false;
    internal static List<string> saveErrors;

    internal Mod[] actualModList = new Mod[0];
    internal Mod[] BC_ModList = new Mod[0];
    internal static List<Mod> HasUpdateModList = new List<Mod>();
    internal static List<References> HasUpdateRefList = new List<References>();
    internal List<string> crashedGuids = new List<string>();
    internal List<string> modIDsReferences = new List<string>();
    internal List<References> ReferencesList = new List<References>();
    internal string[] stdRef = new string[] { "mscorlib", "System.Core", "UnityEngine", "PlayMaker", "MSCLoader", "System", "Assembly-CSharp", "Assembly-CSharp-firstpass", "Assembly-UnityScript", "Assembly-UnityScript-firstpass", "ES2", "Ionic.Zip", "UnityEngine.UI", "0Harmony", "cInput", "Newtonsoft.Json", "System.Xml" };

    //Old stuff
    internal Mod[] PLoadMods = new Mod[0];
    internal Mod[] SecondPassMods = new Mod[0];
    internal Mod[] OnGUImods = new Mod[0];
    internal Mod[] UpdateMods = new Mod[0];
    internal Mod[] FixedUpdateMods = new Mod[0];
    internal Mod[] OnSaveMods = new Mod[0];

    //New Stuff
    internal Mod[] Mod_OnNewGame = new Mod[0];   //When New Game is started
    internal Mod[] Mod_PreLoad = new Mod[0];     //Phase 1 (mod loading)
    internal Mod[] Mod_OnLoad = new Mod[0];      //Phase 2 (mod loading)  
    internal Mod[] Mod_PostLoad = new Mod[0];    //Phase 3 (mod loading)
    internal Mod[] Mod_OnSave = new Mod[0];      //When game saves
    internal Mod[] Mod_OnGUI = new Mod[0];       //Calls unity OnGUI
    internal Mod[] Mod_Update = new Mod[0];      //Calls unity Update
    internal Mod[] Mod_FixedUpdate = new Mod[0]; //Calls unity FixedUpdate

    internal int currentBuild = Assembly.GetExecutingAssembly().GetName().Version.Revision;
    internal int newBuild = 0;
    internal string newVersion = MSCLoader_Ver;
    internal MSCUnloader mscUnloader;

    internal static string steamID;
    internal static bool loaderPrepared = false;
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
}

#endif