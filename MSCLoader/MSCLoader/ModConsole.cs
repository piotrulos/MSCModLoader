using UnityEngine;
using MSCLoader.Commands;
using UnityEngine.UI;

namespace MSCLoader
{
    /// <summary>
    /// The console for MSCLoader.
    /// </summary>
    public class ModConsole : Mod
	{
		public override string ID { get { return "MSCLoader_Console"; } }
		public override string Name { get { return "Console"; } }
		public override string Version { get { return ModLoader.Version; } }
		public override string Author { get { return "piotrulos"; } }

		public static bool IsOpen { get; private set; }
        public static ConsoleView console = new ConsoleView();
        private Keybind consoleKey = new Keybind("Open", "Open console", KeyCode.BackQuote);
        private Keybind consoleSizeKey = new Keybind("Console_size", "Make console bigger/smaller", KeyCode.BackQuote, KeyCode.LeftControl);
        /// <summary>
        /// Create console UI using UnityEngine.UI
        /// </summary>
        GameObject consoleObj, logView, scrollbar;
        public void CreateConsoleUI()
        {
            //Create parent gameobject for console.
            consoleObj = ModUI.CreateParent("MSCLoader Console", false);
            consoleObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            consoleObj.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            consoleObj.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            consoleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(346, 150);
            console = consoleObj.AddComponent<ConsoleView>();

            //Create console container
            GameObject consoleObjc = ModUI.CreateUIBase("MSCLoader ConsoleContainer", consoleObj);
            consoleObjc.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            consoleObjc.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            consoleObjc.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            consoleObjc.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

            consoleObj.GetComponent<ConsoleView>().viewContainer = consoleObjc; //set viewContainer in ConsoleView.cs
            //console = consoleObj.GetComponent<ConsoleView>();

            //Create input field
            GameObject consoleInput = ModUI.CreateInputField("InputField", "Enter command...", consoleObjc, 322, 30);
            consoleInput.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            consoleInput.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            consoleInput.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            consoleInput.GetComponent<InputField>().onEndEdit.AddListener(delegate { consoleObj.GetComponent<ConsoleView>().runCommand(); });

            consoleObj.GetComponent<ConsoleView>().inputField = consoleInput.GetComponent<InputField>();
          
            //Submit button
            GameObject enterBtn = ModUI.CreateButton("SubmitBtn",">",consoleObjc,24,30);
            enterBtn.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
            enterBtn.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
            enterBtn.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
            enterBtn.GetComponent<Button>().onClick.AddListener(() => consoleObj.GetComponent<ConsoleView>().runCommand());

            //Log view text
            logView = ModUI.CreateUIBase("LogView",consoleObjc);
            logView.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            logView.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            logView.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            logView.GetComponent<RectTransform>().sizeDelta = new Vector2(333, 120);
            logView.AddComponent<Image>().color = Color.black;
            logView.AddComponent<Mask>().showMaskGraphic = true;

            GameObject logViewTxt = ModUI.CreateTextBlock("LogText",">", logView, TextAnchor.LowerLeft, Color.white,false);
            logViewTxt.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            logViewTxt.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
            logViewTxt.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            logViewTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1425);
            logViewTxt.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            consoleObj.GetComponent<ConsoleView>().logTextArea = logViewTxt.GetComponent<Text>();
            logView.AddComponent<ScrollRect>().content = logViewTxt.GetComponent<RectTransform>();
            logView.GetComponent<ScrollRect>().horizontal = false;
            logView.GetComponent<ScrollRect>().inertia = false;
            logView.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Clamped;
            logView.GetComponent<ScrollRect>().scrollSensitivity = 30f;

            //Scrollbar
            scrollbar = ModUI.CreateScrollbar(consoleObjc,13,120,Scrollbar.Direction.BottomToTop);
            scrollbar.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
            scrollbar.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            scrollbar.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            scrollbar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-13, 0);

            logView.GetComponent<ScrollRect>().verticalScrollbar = scrollbar.GetComponent<Scrollbar>();

        }
        int conSizeStep = 0;
        public void ChangeConsoleSize() //change to dynamic scale later
        {
            conSizeStep++;
            switch(conSizeStep)
            {
                case 1:
                    consoleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(346, 400);
                    logView.GetComponent<RectTransform>().sizeDelta = new Vector2(333, 370);
                    scrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(13, 370);
                    break;
                case 2:
                    consoleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(346, 500);
                    logView.GetComponent<RectTransform>().sizeDelta = new Vector2(333, 470);
                    scrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(13, 470);
                    break;
                default:
                    consoleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(346, 150);
                    logView.GetComponent<RectTransform>().sizeDelta = new Vector2(333, 120);
                    scrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(13, 120);
                    conSizeStep = 0;
                    break;

            }

        }
        
        public override void Update()
        {
            if (consoleKey.IsDown() && !consoleSizeKey.IsDown())
            {
                console.toggleVisibility();
            }

            if (consoleSizeKey.IsDown())
            {
                ChangeConsoleSize();
            }
        }

        public override void OnLoad()
        {
            Keybind.Add(this, consoleKey);
            Keybind.Add(this, consoleSizeKey);
            CreateConsoleUI();
            console.setVisibility(false);
            ConsoleCommand.Add(new CommandClear());
            ConsoleCommand.Add(new CommandHelp());
            ConsoleCommand.Add(new CommandLogAll());
        }

        /// <summary>
        /// Print a message to console.
        /// </summary>
        /// <param name="str">Text or object to append to console.</param>
        public static void Print(string str)
        {
             console.console.appendLogLine(str);
		}
        /// <summary>
        /// OBSOLETE: For compatibility with 0.1 plugins, please use string str overload!
        /// </summary>
        /// <param name="obj">Text or object to append to console.</param>
        public static void Print(object obj)
        {
            console.console.appendLogLine(obj.ToString());
        }
        /// <summary>
        /// Append an error to the console.
        /// </summary>
        /// <param name="str">Text or object to append to error log.</param>
        public static void Error(string str)
		{          
            console.setVisibility(true);
            console.console.appendLogLine(string.Format("<color=red><b>Error: </b>{0}</color>", str));
        }

		/// <summary>
		/// Clear the console.
		/// </summary>
		public static void Clear()
		{
            console.console.clearConsole();
		}

	}
}
