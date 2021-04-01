using UnityEngine;
using MSCLoader.Commands;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

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

        public override bool UseAssetsFolder => true;
        public override bool LoadInMenu => true;

        public static bool IsOpen { get; private set; }

        public static ConsoleView console;
        private GameObject UI;

        private Keybind consoleKey = new Keybind("Open", "Open console", KeyCode.BackQuote);

        public static Settings typing = new Settings("typeConsole", "Start typing when you open console", false);
        static Settings ConsoleFontSize = new Settings("consoleFont", "Change console font size:", 12, ChangeFontSize);

        public override void ModSettings()
        {
            Keybind.Add(this, consoleKey);

            Settings.AddHeader(this, "Console Settings", new Color32(0, 128, 0, 255));
            Settings.AddText(this, "Basic settings for console");
            Settings.AddCheckBox(this, typing);
            Settings.AddSlider(this, ConsoleFontSize, 10, 20);
        }

        public override void ModSettingsLoaded()
        {
            ChangeFontSize();
        }

        public static void ChangeFontSize()
        {
            console.logTextArea.fontSize = int.Parse(ConsoleFontSize.GetValue().ToString());
        }

        public void CreateConsoleUI()
        {
            AssetBundle ab = LoadAssets.LoadBundle(this, "console.unity3d");
            UI = ab.LoadAsset<GameObject>("MSCLoader Console.prefab");
            Texture2D cursor = ab.LoadAsset<Texture2D>("resizeCur.png");
            Texture2D cursorX = ab.LoadAsset<Texture2D>("resizeCurX.png");
            ab.Unload(false);
            UI = Object.Instantiate(UI);
            UI.name = "MSCLoader Console";
            console = UI.AddComponent<ConsoleView>();
            console.viewContainer = UI.transform.GetChild(0).gameObject;
            console.inputField = console.viewContainer.transform.GetChild(0).gameObject.GetComponent<InputField>();
            console.viewContainer.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(() => console.runCommand());
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
       
        public override void Update()
        {
            if (consoleKey.GetKeybindDown())
            {
                console.toggleVisibility();
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().LoadConsoleSize();
            }
        }

        public override void OnMenuLoad()
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
            console.setVisibility(false);
            console.viewContainer.transform.GetChild(5).gameObject.GetComponent<ConsoleUIResizer>().LoadConsoleSize();
            ConsoleCommand.Add(new CommandVersion());
            ConsoleCommand.Add(new CommandLogAll());
            ConsoleCommand.Add(new ManifestCommand());
        }
#pragma warning restore CS1591
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
            System.Console.WriteLine(string.Format("MSCLoader Message: {0}", Regex.Replace(str, "<.*?>", string.Empty)));
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
            System.Console.WriteLine(string.Format("MSCLoader Message: {0}", obj));
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
                console.setVisibility(true);
                console.controller.AppendLogLine(string.Format("<color=red><b>Error: </b>{0}</color>", str));
            }
            catch { }
            System.Console.WriteLine(string.Format("MSCLoader ERROR: {0}", Regex.Replace(str, "<.*?>", string.Empty)));
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
                console.setVisibility(true);
                console.controller.AppendLogLine(string.Format("<color=yellow><b>Warning: </b>{0}</color>", str));
            }
            catch { }
            System.Console.WriteLine(string.Format("MSCLoader WARNING: {0}", Regex.Replace(str, "<.*?>", string.Empty)));
        }
    }
}