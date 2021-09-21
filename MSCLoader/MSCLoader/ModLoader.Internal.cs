using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MSCLoader
{
    public partial class ModLoader
    {
        internal static bool LogAllErrors = false;
        internal static List<string> InvalidMods;
        internal static ModLoader Instance;
        internal static bool unloader = false;
        internal static bool rtmm = false;
        internal static List<string> saveErrors;

        internal Mod[] actualModList = new Mod[0];
        internal Mod[] BC_ModList = new Mod[0];
        internal List<Mod> HasUpdateModList = new List<Mod>();
        internal List<References> ReferencesList = new List<References>();

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

        internal string expBuild = Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString();
        internal MSCUnloader mscUnloader;

        internal static string steamID;
        internal static bool loaderPrepared = false;
        internal static string ModsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
        internal static string ConfigFolder = Path.Combine(ModsFolder, "Config");
        internal static string SettingsFolder = Path.Combine(ConfigFolder, "Mod Settings");
        internal static string MetadataFolder = Path.Combine(ConfigFolder, "Mod Metadata");
        internal static string AssetsFolder = Path.Combine(ModsFolder, "Assets");
        internal string[] upd_tmp;
        internal List<string> mod_aulist;
        internal GameObject mainMenuInfo;
        internal GameObject loading;
        internal GameObject loadingMeta;
        internal Animation menuInfoAnim;
        internal GUISkin guiskin;

        internal static readonly string serverURL = "http://my-summer-car.ml";
        internal static readonly string metadataURL = "http://my-summer-car.ml:4000";
        //private readonly string serverURL = "http://localhost/msc2"; //localhost for testing only
        //private readonly string metadataURL = "http://localhost:4000";

        internal bool IsModsLoading = false;
        internal bool fullyLoaded = false;
        internal bool allModsLoaded = false;
        internal bool IsModsResetting = false;
        internal bool IsModsDoneResetting = false;
    }
}
