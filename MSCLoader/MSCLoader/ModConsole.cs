#if !Mini
using MSCLoader.Commands;
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace MSCLoader;

/// <summary>
/// MSCLoader console related functions.
/// </summary>
public class ModConsole : Mod
{

    public override string ID => "MSCLoader_Console";
    public override string Name => "[INTERNAL] Console";
    public override string Version => ModLoader.MSCLoader_Ver;
    public override string Author => "piotrulos";

    internal static bool IsOpen;
    internal static ConsoleView console;
    internal static SettingsCheckBox typing;
    internal static SettingsSliderInt ConsoleFontSize;

    private GameObject UI;
    private Keybind consoleKey;
    public override void ModSetup()
    {
        SetupFunction(Setup.OnMenuLoad, Mod_OnMenuLoad);
        SetupFunction(Setup.Update, Mod_Update);
        SetupFunction(Setup.ModSettings, Mod_Settings);
        SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoaded);
    }

    private void Mod_Settings()
    {
        consoleKey = Keybind.Add(this, "Open", "<color=lime>Open console key combination</color>", KeyCode.BackQuote);
        Settings.AddHeader(this, "MSCLoader info", Color.black);
        if (ModLoader.Instance.newBuild > ModLoader.Instance.currentBuild)
        {
            Settings.AddText(this, $"<color=orange>MSCLoader {ModLoader.MSCLoader_Ver} build {ModLoader.Instance.currentBuild}</color> -> <color=lime>MSCLoader {ModLoader.Instance.newVersion} build {ModLoader.Instance.newBuild}</color>");
        }
        else
        {
            Settings.AddText(this, $"<color=lime>MSCLoader {ModLoader.MSCLoader_Ver} build {ModLoader.Instance.currentBuild}</color>");
        }
        string sp = Path.Combine(ModLoader.SettingsFolder, Path.Combine("MSCLoader_Settings", "lastCheck"));
        if (File.Exists(sp))
        {
            DateTime lastCheck;
            string lastCheckS = File.ReadAllText(sp);
            DateTime.TryParse(lastCheckS, out lastCheck);
            Settings.AddText(this, $"Last checked for mod updates: <color=aqua>{lastCheck.ToString("dd.MM.yyyy HH:mm:ss")}</color>");

        }
        Settings.AddButton(this, "checkForUpd", "Check For Mods Updates", delegate
        {
            if (!ModLoader.Instance.checkForUpdatesProgress)
                ModLoader.Instance.CheckForModsUpd(true);
        }, Color.black, Color.white);
        Settings.AddHeader(this, "Console Settings");
        Settings.AddText(this, "Basic settings for console");
        typing = Settings.AddCheckBox(this, "MSCLoader_ConsoleTyping", "Start typing when you open console", false);
        ConsoleFontSize = Settings.AddSlider(this, "MSCLoader_ConsoleFontSize", "Change console font size", 10, 20, 12, ChangeFontSize);
    }

    private void Mod_SettingsLoaded()
    {
        ChangeFontSize();
    }


    internal static void ChangeFontSize()
    {
        console.logTextArea.fontSize = ConsoleFontSize.GetValue();
    }

    void CreateConsoleUI()
    {
        AssetBundle ab = LoadAssets.LoadBundle(this, "console.unity3d");
        GameObject UIp = ab.LoadAsset<GameObject>("MSCLoader Canvas console.prefab");
        UI = GameObject.Instantiate(UIp);
        console = UI.GetComponentInChildren<ConsoleView>();
        GameObject.DontDestroyOnLoad(UI);
        GameObject.Destroy(UIp);
        ab.Unload(false);
    }

    void Mod_Update()
    {
        if (consoleKey.GetKeybindDown())
        {
            console.ToggleVisibility();
        }
    }

    void Mod_OnMenuLoad()
    {
        try
        {
            CreateConsoleUI();
        }
        catch (System.Exception e)
        {
            ModUI.ShowMessage($"Fatal error:{System.Environment.NewLine}<color=orange>{e.Message}</color>{System.Environment.NewLine}Please install MSCLoader correctly.", "Fatal Error");
        }
        ConsoleCommand.cc = console.controller;
        console.SetVisibility(false);
        console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().LoadConsoleSize();

        //Add Console Commands
        ConsoleCommand.Add(new CommandVersion());
        ConsoleCommand.Add(new CommandLogAll());
        ConsoleCommand.Add(new MetadataCommand());
        ConsoleCommand.Add(new EarlyAccessCommand());
        ConsoleCommand.Add(new SaveDbgCommand());
    }
    /// <summary>
    /// Print a message to console.
    /// </summary>
    /// <param name="str">Text to print to console.</param>
    public static void Print(string str)
    {
        try
        {
            console.controller.AppendLogLine(str);
        }
        catch { }
        System.Console.WriteLine($"MSCLoader Message: {Regex.Replace(str, "<.*?>", string.Empty)}");
    }
    /// <summary>
    /// Prints anything to console.
    /// </summary>
    /// <param name="obj">Text or object to print to console.</param>
    public static void Print(object obj)
    {
        try
        {
            console.controller.AppendLogLine(obj.ToString());
        }
        catch { }
        System.Console.WriteLine($"MSCLoader Message: {obj}");
    }

    /// <summary>
    /// Print an error to the console.
    /// </summary>
    /// <param name="str">Text to print to error log.</param>
    public static void Error(string str)
    {
        try
        {
            console.SetVisibility(true);
            console.controller.AppendLogLine($"<color=red><b>Error: </b>{str}</color>");
        }
        catch { }
        System.Console.WriteLine($"MSCLoader ERROR: {Regex.Replace(str, "<.*?>", string.Empty)}");
    }

    /// <summary>
    /// Print an warning to the console.
    /// </summary>
    /// <param name="str">Text to print to error log.</param>
    public static void Warning(string str)
    {
        try
        {
            console.SetVisibility(true);
            console.controller.AppendLogLine($"<color=yellow><b>Warning: </b>{str}</color>");
        }
        catch { }
        System.Console.WriteLine($"MSCLoader WARNING: {Regex.Replace(str, "<.*?>", string.Empty)}");
    }

    //compatibility layer with pro

    /// <summary>
    /// Same as ModConsole.Print(string);
    /// </summary>
    /// <param name="text">Text to print to console.</param>
    public static void Log(string text) => Print(text);

    /// <summary>
    /// Same as ModConsole.Print(obj);
    /// </summary>
    /// <param name="obj">object to print to console.</param>
    public static void Log(object obj) => Print(obj);

    /// <summary>
    /// Same as ModConsole.Error(string);
    /// </summary>
    /// <param name="text">Error to print to console.</param>
    public static void LogError(string text) => Error(text);

    /// <summary>
    /// Same as ModConsole.Warning(string);
    /// </summary>
    /// <param name="text">Warning to print to console.</param>
    public static void LogWarning(string text) => Warning(text);

    /// <summary>
    /// Logs a list (and optionally its elements) to the ModConsole and output_log.txt
    /// </summary>
    /// <param name="list">List to print.</param>
    /// <param name="printAllElements">(Optional) Should it log all elements of the list/array or should it only log the list/array itself. (default: true)</param>
    public static void Log(IList list, bool printAllElements = true)
    {
        // Check if it should print the elements or the list itself.
        if (printAllElements)
        {
            Log(list.ToString());
            for (int i = 0; i < list.Count; i++) Log(list[i]);
        }
        else Log(list.ToString());
    }
}

#endif