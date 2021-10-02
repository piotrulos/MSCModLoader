using UnityEngine;
using MSCLoader.Commands;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System.Collections;
using System;

namespace MSCLoader
{
    /// <summary>
    /// The console for MSCLoader.
    /// </summary>
    public class ModConsole : Mod
    {
#pragma warning disable CS1591

        public override string ID => "MSCLoader_Console";
        public override string Name => "Console";
        public override string Version => ModLoader.MSCLoader_Ver;
        public override string Author => "piotrulos";
        public static bool IsOpen { get; private set; }
        
        public static ConsoleView console;
        private GameObject UI;

        private Keybind consoleKey;
        public static SettingsCheckBox typing;
        static SettingsSliderInt ConsoleFontSize;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnMenuLoad, Mod_OnMenuLoad);
            SetupFunction(Setup.Update, Mod_Update);
        }

        public override void ModSettings()
        {
            consoleKey = Keybind.Add(this, "Open", "<color=lime>Open console key combination</color>", KeyCode.BackQuote);
            Settings.AddHeader(this, "MSCLoader info", Color.black);
            Settings.AddText(this, $"<color=lime>MSCLoader {ModLoader.MSCLoader_Ver} build {ModLoader.Instance.currentBuild}</color>");
            if (ModLoader.Instance.newBuild > ModLoader.Instance.currentBuild)
            {
                Settings.AddText(this, $"New update available:");
                Settings.AddText(this, $"<color=aqua>MSCLoader {ModLoader.Instance.newVersion} build {ModLoader.Instance.newBuild}</color>");
            }
            else
            {
                Settings.AddText(this, $"No new updates available");
            }
            string sp = System.IO.Path.Combine(ModLoader.SettingsFolder, @"MSCLoader_Settings\lastCheck");
            if (System.IO.File.Exists(sp))
            {
                DateTime lastCheck;
                string lastCheckS = System.IO.File.ReadAllText(sp);
                DateTime.TryParse(lastCheckS, out lastCheck);
                Settings.AddText(this, $"Last checked for mod updates: <color=aqua>{lastCheck.ToString("dd.MM.yyyy HH:mm:ss")}</color>");

            }
            Settings.AddHeader(this, "Console Settings");
            Settings.AddText(this, "Basic settings for console");
            typing = Settings.AddCheckBox(this, "MSCLoader_ConsoleTyping", "Start typing when you open console", false);
            ConsoleFontSize = Settings.AddSlider(this, "MSCLoader_ConsoleFontSize", "Change console font size", 10, 20, 12, ChangeFontSize);
        }

        public override void ModSettingsLoaded()
        {
            ChangeFontSize();
        }
#pragma warning restore CS1591

        internal static void ChangeFontSize()
        {
            console.logTextArea.fontSize = ConsoleFontSize.GetValue();
        }

        void CreateConsoleUI()
        {
            AssetBundle ab = LoadAssets.LoadBundle(this, "console.unity3d");
            UI = ab.LoadAsset<GameObject>("MSCLoader Console.prefab");
            Texture2D cursor = ab.LoadAsset<Texture2D>("resizeCur.png");
            Texture2D cursorX = ab.LoadAsset<Texture2D>("resizeCurX.png");
            ab.Unload(false);
            UI = GameObject.Instantiate(UI);
            UI.name = "MSCLoader Console";
            console = UI.AddComponent<ConsoleView>();
            console.viewContainer = UI.transform.GetChild(0).gameObject;
            console.inputField = console.viewContainer.transform.GetChild(0).gameObject.GetComponent<InputField>();
            console.viewContainer.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(() => console.RunCommand());
            console.logTextArea = console.viewContainer.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Text>();
            console.viewContainer.transform.GetChild(4).gameObject.AddComponent<ConsoleUIResizer>().logview = console.viewContainer.transform.GetChild(2).gameObject;
            console.viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().scrollbar = console.viewContainer.transform.GetChild(3).gameObject;
            console.viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().otherResizer = console.viewContainer.transform.GetChild(5).gameObject;
            console.viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().cursor = cursor;
            console.viewContainer.transform.GetChild(5).gameObject.AddComponent<ConsoleUIResizer>().inputField = console.viewContainer.transform.GetChild(0).gameObject;
            console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().submitBtn = console.viewContainer.transform.GetChild(1).gameObject;
            console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().logview = console.viewContainer.transform.GetChild(2).gameObject;
            console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().scrollbar = console.viewContainer.transform.GetChild(3).gameObject;
            console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().otherResizer = console.viewContainer.transform.GetChild(4).gameObject;
            console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().cursor = cursorX;
            console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().Xresizer = true;
            EventTrigger trigger = console.viewContainer.transform.GetChild(4).gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => { console.viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().OnMouseEnter(); });
            trigger.delegates.Add(entry);
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener((eventData) => { console.viewContainer.transform.GetChild(4).gameObject.GetComponent<ConsoleUIResizer>().OnMouseExit(); });
            trigger.delegates.Add(entry);
            trigger = console.viewContainer.transform.GetChild(5).gameObject.GetComponent<EventTrigger>();
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => { console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().OnMouseEnter(); });
            trigger.delegates.Add(entry);
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener((eventData) => { console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().OnMouseExit(); });
            trigger.delegates.Add(entry);
            UI.transform.SetParent(ModUI.GetCanvas().transform, false);
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
            catch(System.Exception e)
            {
                ModUI.ShowMessage($"Fatal error:{System.Environment.NewLine}<color=orange>{e.Message}</color>{System.Environment.NewLine}Please install modloader correctly.", "Fatal Error");
            }
            console.controller = new ConsoleController();
            ConsoleCommand.cc = console.controller;
            console.SetVisibility(false);
            console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().LoadConsoleSize();
            ConsoleCommand.Add(new CommandVersion());
            ConsoleCommand.Add(new CommandLogAll());
            ConsoleCommand.Add(new MetadataCommand());
            ConsoleCommand.Add(new EarlyAccessCommand());
        }
        /// <summary>
        /// Print a message to console.
        /// </summary>
        /// <param name="str">Text to print to console.</param>
        /// <example><code source="Examples.cs" region="ModConsolePrint" lang="C#" 
        /// title="Example Code" /></example>
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
        /// <example><code source="Examples.cs" region="ModConsolePrint" lang="C#" 
        /// title="Example Code" /></example>
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
        /// <example><code source="Examples.cs" region="ModConsoleError" lang="C#" 
        /// title="Example Code" /></example>
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
        /// <example><code source="Examples.cs" region="ModConsoleError" lang="C#" 
        /// title="Example Code" /></example>
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
}