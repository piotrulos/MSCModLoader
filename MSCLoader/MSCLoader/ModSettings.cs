using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
#pragma warning disable CS1591
    public class ModSettings_menu : Mod
    {

        public override bool LoadInMenu => true;
        public override bool UseAssetsFolder => true;
        public override string ID => "MSCLoader_Settings";
        public override string Name => "Settings (Main)";
        public override string Version => ModLoader.MSCLoader_Ver;
        public override string Author => "piotrulos";

        private Keybind menuKey = new Keybind("Open", "Open menu", KeyCode.M, KeyCode.LeftControl);

        internal static Settings dm_disabler = new Settings("mscloader_dm_disabler", "Disable mods throwing errors", false);
        internal static Settings dm_logST = new Settings("mscloader_dm_logST", "Log-all stack trace (not recommended)", false);
        internal static Settings dm_operr = new Settings("mscloader_dm_operr", "Log-all open console on error", false);
        internal static Settings dm_warn = new Settings("mscloader_dm_warn", "Log-all  open console on warning", false);
        internal static Settings dm_pcon = new Settings("mscloader_dm_pcon", "Persistent console", false);

        internal static Settings expWarning = new Settings("mscloader_expWarning", "Show experimental warning", true);
        internal static Settings modPath = new Settings("mscloader_modPath", "Show mods folder", true, ModLoader.MainMenuPath);
        private static Settings modSetButton = new Settings("mscloader_modSetButton", "Show settings button in bottom right corner", true, ModSettingsToggle);
        internal static Settings forceMenuVsync = new Settings("mscloader_forceMenuVsync", "60FPS limit in Main Menu", true, VSyncSwitchCheckbox);
        internal static Settings openLinksOverlay = new Settings("mscloader_openLinksOverlay", "Open URLs in steam overlay", true);
        internal static Settings showCoreModsDf = new Settings("mscloader_showCoreModsDf", "Show core modules by default", true);
        internal static Settings skipGameIntro = new Settings("mscloader_skipGameIntro", "Skip game Splash Screen", false, SkipIntroSet);
        internal static Settings syncLoad = new Settings("mscloader_syncLoad", "Load mods synchronously", false);

        private static Settings expUIScaling = new Settings("mscloader_expUIScaling", "Ultra-widescreen UI scaling", false, ExpUIScaling);
        private static Settings tuneScaling = new Settings("mscloader_tuneScale", "Tune scaling:", 1f, ChangeUIScaling);

        private static Settings checkLaunch = new Settings("mscloader_checkLaunch", "Every launch", true);
        private static Settings checkDaily = new Settings("mscloader_checkDaily", "Daily", false);
        private static Settings checkWeekly = new Settings("mscloader_checkWeekly", "Weekly", false);

        public SettingsView settings;
        public GameObject UI;
        public GameObject ModButton;
        public GameObject ModButton_Invalid;
        public GameObject ModLabel;
        public GameObject KeyBind;
        public GameObject Checkbox, setBtn, slider, textBox, header;
        public GameObject Button_ms;
        internal static byte cfmu_set = 0;

        static ModSettings_menu instance;

        public override void ModSettings()
        {
            instance = this;
            Keybind.Add(this, menuKey);
            if (ModLoader.devMode)
            {
                Settings.AddHeader(this, "DevMode Settings", new Color32(101, 34, 18, 255), new Color32(254, 254, 0, 255));
                Settings.AddCheckBox(this, dm_disabler);
                Settings.AddCheckBox(this, dm_logST);
                Settings.AddCheckBox(this, dm_operr);
                Settings.AddCheckBox(this, dm_warn);
                Settings.AddCheckBox(this, dm_pcon);
            }
            Settings.AddHeader(this, "Basic Settings", new Color32(0, 128, 0, 255));
            Settings.AddText(this, "All basic settings for MSCLoader");
            Settings.AddCheckBox(this, expWarning);
            Settings.AddCheckBox(this, modPath);
            Settings.AddCheckBox(this, modSetButton);
            Settings.AddCheckBox(this, forceMenuVsync);
            Settings.AddCheckBox(this, openLinksOverlay);
            Settings.AddCheckBox(this, showCoreModsDf);
            Settings.AddCheckBox(this, skipGameIntro);
            Settings.AddText(this, $"If for whatever reason you want to save half a second of mods loading time, enable below option.{Environment.NewLine}(Loading progress <b>cannot</b> be displayed in synchronous mode)");
            Settings.AddCheckBox(this, syncLoad);
            Settings.AddHeader(this, "Mod Update Check", new Color32(0, 128, 0, 255));
            Settings.AddText(this, "Check for mod updates:");
            Settings.AddCheckBox(this, checkLaunch, "cfmu_set");
            Settings.AddCheckBox(this, checkDaily, "cfmu_set");
            Settings.AddCheckBox(this, checkWeekly, "cfmu_set");
            Settings.AddHeader(this, "Experimental Scaling", new Color32(101, 34, 18, 255), new Color32(254, 254, 0, 255));
            Settings.AddText(this, "This option enables <color=orange>experimental UI scaling</color> for <color=orange>ultra-widescreen monitor setup</color>. Turn on this checkbox first, then run game in ultra-widescreen resolution. You can then tune scaling using slider below, but default value (1) should be ok.");
            Settings.AddCheckBox(this, expUIScaling);
            Settings.AddSlider(this, tuneScaling, 0f, 1f);

        }
        public override void ModSettingsLoaded()
        {
            IniData ini = new FileIniDataParser().ReadFile("doorstop_config.ini");
            ini.Configuration.AssigmentSpacer = "";
            string skipIntro = ini["MSCLoader"]["skipIntro"];
            bool introSkip;
            if (!bool.TryParse(skipIntro, out introSkip))
            {
                skipGameIntro.Value = false;
                ModConsole.Error(string.Format("Excepted boolean, received '{0}'.", skipIntro ?? "<null>"));
            }
            else
            {
                skipGameIntro.Value = introSkip;
            }
            ModSettingsToggle();
            ExpUIScaling();
            if ((bool)checkLaunch.GetValue())
                cfmu_set = 0;
            if ((bool)checkDaily.GetValue())
                cfmu_set = 1;
            if ((bool)checkWeekly.GetValue())
                cfmu_set = 7;

        }
        private static void ModSettingsToggle()
        {
            instance.Button_ms.SetActive((bool)modSetButton.GetValue());
        }

        private static void ChangeUIScaling()
        {
            if ((bool)expUIScaling.GetValue())
            {
                ModUI.GetCanvas().GetComponent<CanvasScaler>().matchWidthOrHeight = float.Parse(tuneScaling.GetValue().ToString());
            }
        }
        private static void ExpUIScaling()
        {
            if ((bool)expUIScaling.GetValue())
            {
                ModUI.GetCanvas().GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                ChangeUIScaling();
            }
            else
            {
                ModUI.GetCanvas().GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            }
        }

        private static void SkipIntroSet()
        {
            FileIniDataParser parser = new FileIniDataParser();
            IniData ini = parser.ReadFile("doorstop_config.ini");
            ini.Configuration.AssigmentSpacer = "";
            ini["MSCLoader"]["skipIntro"] = ((bool)skipGameIntro.GetValue()).ToString().ToLower();
            parser.WriteFile("doorstop_config.ini", ini, System.Text.Encoding.ASCII);           
        }

        private static void VSyncSwitchCheckbox()
        {
            if (ModLoader.GetCurrentScene() == CurrentScene.MainMenu)
            {
                if ((bool)forceMenuVsync.GetValue())
                    QualitySettings.vSyncCount = 1;
                else
                    QualitySettings.vSyncCount = 0;
            }
        }
        public override void OnMenuLoad()
        {
            try
            {
                CreateSettingsUI();
            }
            catch(Exception e)
            {
                ModUI.ShowMessage($"Fatal error:{Environment.NewLine}<color=orange>{e.Message}</color>{Environment.NewLine}Please install modloader correctly.", "Fatal Error");
            }
        }

        public void CreateSettingsUI()
        {
            AssetBundle ab = LoadAssets.LoadBundle(this, "settingsui.unity3d");

            UI = ab.LoadAsset<GameObject>("MSCLoader Settings.prefab");

            ModButton = ab.LoadAsset<GameObject>("ModButton.prefab");
            ModButton_Invalid = ab.LoadAsset<GameObject>("ModButton_Invalid.prefab");

            ModLabel = ab.LoadAsset<GameObject>("ModViewLabel.prefab");

            KeyBind = ab.LoadAsset<GameObject>("KeyBind.prefab");

            Button_ms = ab.LoadAsset<GameObject>("Button_ms.prefab");
            //For mod settings
            Checkbox = ab.LoadAsset<GameObject>("Checkbox.prefab");
            setBtn = ab.LoadAsset<GameObject>("Button.prefab");
            slider = ab.LoadAsset<GameObject>("Slider.prefab");
            textBox = ab.LoadAsset<GameObject>("TextBox.prefab");
            header = ab.LoadAsset<GameObject>("Header.prefab");

            UI = GameObject.Instantiate(UI);
            UI.AddComponent<ModUIDrag>();
            UI.name = "MSCLoader Settings";

            settings = UI.AddComponent<SettingsView>();
            settings.ms = this;
            settings.settingViewContainer = UI.transform.GetChild(0).gameObject;
            settings.modList = settings.settingViewContainer.transform.GetChild(3).gameObject;
            settings.modView = settings.modList.transform.GetChild(0).gameObject;
            settings.modInfo = settings.settingViewContainer.transform.GetChild(2).gameObject;
            GameObject ModSettingsView = settings.modInfo.transform.GetChild(0).gameObject;
            settings.ModKeyBinds = settings.settingViewContainer.transform.GetChild(1).gameObject;
            settings.keybindsList = settings.ModKeyBinds.transform.GetChild(0).GetChild(4).gameObject;
            settings.modSettings = settings.settingViewContainer.transform.GetChild(4).gameObject;
            settings.modSettingsList = settings.modSettings.transform.GetChild(0).GetChild(4).gameObject;
            settings.coreModCheckbox = settings.settingViewContainer.transform.GetChild(6).GetChild(0).GetComponent<Toggle>();
            settings.coreModCheckbox.onValueChanged.AddListener(delegate { settings.ToggleCoreCheckbox(); });
            settings.noOfMods = settings.settingViewContainer.transform.GetChild(6).GetChild(1).GetComponent<Text>();
            settings.goBackBtn = settings.settingViewContainer.transform.GetChild(0).GetChild(1).gameObject;
            settings.goBackBtn.GetComponent<Button>().onClick.AddListener(() => settings.goBack());
            settings.settingViewContainer.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => settings.toggleVisibility());
            settings.DisableMod = ModSettingsView.transform.GetChild(2).GetComponent<Toggle>();
            settings.DisableMod.onValueChanged.AddListener(settings.disableMod);

            settings.InfoTxt = ModSettingsView.transform.GetChild(0).GetComponent<Text>();
            settings.descriptionTxt = ModSettingsView.transform.GetChild(8).GetComponent<Text>();

            settings.nexusLink = ModSettingsView.transform.GetChild(4).GetComponent<Button>();
            settings.rdLink = ModSettingsView.transform.GetChild(5).GetComponent<Button>();
            settings.ghLink = ModSettingsView.transform.GetChild(6).GetComponent<Button>();

            UI.transform.SetParent(ModUI.GetCanvas().transform, false);
            settings.SetVisibility(false);
            Button_ms = GameObject.Instantiate(Button_ms);
            Button_ms.name = "MSCLoader Settings button";
            Button_ms.transform.SetParent(ModUI.GetCanvas().transform, false);
            Button_ms.GetComponent<Button>().onClick.AddListener(() => settings.toggleVisibility());
            Button_ms.SetActive(true);
            if (!(bool)modSetButton.GetValue())
                Button_ms.SetActive(false);
            ab.Unload(false);
        }

        // Reset keybinds
        public static void ResetBinds(Mod mod)
        {
            if (mod != null)
            {
                // Revert binds
                Keybind[] bind = Keybind.Get(mod).ToArray();
                for (int i = 0; i < bind.Length; i++)
                {
                    Keybind original = Keybind.GetDefault(mod).Find(x => x.ID == bind[i].ID);

                    if (original != null)
                    {
                        bind[i].Key = original.Key;
                        bind[i].Modifier = original.Modifier;
                    }
                }

                // Save binds
                SaveModBinds(mod);
            }
        }


        // Save all keybinds to config files.
        public static void SaveAllBinds()
        {
            foreach (Mod mod in ModLoader.LoadedMods)
            {
                SaveModBinds(mod);
            }
        }


        // Save keybind for a single mod to config file.
        public static void SaveModBinds(Mod mod)
        {

            KeybindList list = new KeybindList();
            string path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "keybinds.json");

            Keybind[] binds = Keybind.Get(mod).ToArray();
            for (int i = 0; i < binds.Length; i++)
            {
                if (binds[i].ID == null || binds[i].Vals != null)
                    continue;
                Keybinds keybinds = new Keybinds
                {
                    ID = binds[i].ID,
                    Key = binds[i].Key,
                    Modifier = binds[i].Modifier
                };

                list.keybinds.Add(keybinds);
            }

            string serializedData = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, serializedData);

        }

        // Reset settings
        public static void ResetSettings(Mod mod)
        {
            if (mod != null)
            {
                // Revert settings
                Settings[] set = Settings.Get(mod).ToArray();
                for (int i = 0; i < set.Length; i++)
                {
                    Settings original = Settings.GetDefault(mod).Find(x => x.ID == set[i].ID);

                    if (original != null)
                    {
                        set[i].Value = original.Value;
                    }
                }

                // Save settings
                SaveSettings(mod);
            }
        }
        public static void ResetSpecificSettings(Mod mod, Settings[] sets)
        {
            if (mod != null)
            {
                // Revert settings
                for (int i = 0; i < sets.Length; i++)
                {
                    Settings original = Settings.GetDefault(mod).Find(x => x.ID == sets[i].ID);

                    if (original != null)
                    {
                        sets[i].Value = original.Value;
                    }
                }

                // Save settings
                SaveSettings(mod);
            }
        }
        // Save settings for a single mod to config file.
        public static void SaveSettings(Mod mod)
        {
            SettingsList list = new SettingsList();
            list.isDisabled = mod.isDisabled;
            string path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "settings.json");

            Settings[] set = Settings.Get(mod).ToArray();
            for (int i = 0; i < set.Length; i++)
            {
                if (set[i].type == SettingsType.Button || set[i].type == SettingsType.RButton || set[i].type == SettingsType.Header || set[i].type == SettingsType.Text)
                    continue;

                Setting sets = new Setting
                {
                    ID = set[i].ID,
                    Value = set[i].Value
                };

                list.settings.Add(sets);
            }

            string serializedData = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, serializedData);

        }

        // Load all keybinds.
        public static void LoadBinds()
        {
            Mod[] binds = ModLoader.LoadedMods.Where(mod => Keybind.Get(mod).Count > 0).ToArray();
            for (int i = 0; i < binds.Length; i++)
            {
                //delete old xml file (if exists)
                string path = Path.Combine(ModLoader.GetModSettingsFolder(binds[i]), "keybinds.xml");
                if (File.Exists(path))
                    File.Delete(path);

                // Check if there is custom keybinds file (if not, create)
                path = Path.Combine(ModLoader.GetModSettingsFolder(binds[i]), "keybinds.json");
                if (!File.Exists(path))
                {
                    SaveModBinds(binds[i]);
                    continue;
                }

                //Load and deserialize 
                KeybindList keybinds = JsonConvert.DeserializeObject<KeybindList>(File.ReadAllText(path));
                if (keybinds.keybinds.Count == 0)
                    continue;
                for (int k = 0; k < keybinds.keybinds.Count; k++)
                {
                    Keybind bind = Keybind.Keybinds.Find(x => x.Mod == binds[i] && x.ID == keybinds.keybinds[k].ID);
                    if (bind == null)
                        continue;
                    bind.Key = keybinds.keybinds[k].Key;
                    bind.Modifier = keybinds.keybinds[k].Modifier;
                }
            }
        }

        // Load all settings.
        public static void LoadSettings()
        {
            for (int i = 0; i < ModLoader.LoadedMods.Count; i++)
            {
                // Check if there is custom settings file (if not, ignore)
                string path = Path.Combine(ModLoader.GetModSettingsFolder(ModLoader.LoadedMods[i]), "settings.json");
                if (!File.Exists(path))
                    SaveSettings(ModLoader.LoadedMods[i]); //create settings file if not exists.


                //Load and deserialize 
                SettingsList settings = JsonConvert.DeserializeObject<SettingsList>(File.ReadAllText(path));
                ModLoader.LoadedMods[i].isDisabled = settings.isDisabled;
                try
                {
                    if (ModLoader.LoadedMods[i].LoadInMenu && !ModLoader.LoadedMods[i].isDisabled && ModLoader.LoadedMods[i].fileName != null)
                    {
                        ModLoader.LoadedMods[i].OnMenuLoad();
                    }
                }
                catch (Exception e)
                {

                    string errorDetails = string.Format("{2}<b>Details: </b>{0} in <b>{1}</b>", e.Message, new StackTrace(e, true).GetFrame(0).GetMethod(), Environment.NewLine);
                    ModConsole.Error(string.Format("Mod <b>{0}</b> throw an error!{1}", ModLoader.LoadedMods[i].ID, errorDetails));
                    if (ModLoader.devMode)
                        ModConsole.Error(e.ToString());
                    System.Console.WriteLine(e);
                }
                if (Settings.Get(ModLoader.LoadedMods[i]).Count == 0)
                    continue;

                for (int j = 0; j < settings.settings.Count; j++)
                {
                    Settings set = Settings.modSettings.Find(x => x.Mod == ModLoader.LoadedMods[i] && x.ID == settings.settings[j].ID);
                    if (set == null)
                        continue;
                    set.Value = settings.settings[j].Value;
                }
                try
                {
                    ModLoader.LoadedMods[i].ModSettingsLoaded();
                }
                catch (Exception e)
                {

                    string errorDetails = string.Format("{2}<b>Details: </b>{0} in <b>{1}</b>", e.Message, new StackTrace(e, true).GetFrame(0).GetMethod(), Environment.NewLine);
                    ModConsole.Error(string.Format("Mod <b>{0}</b> throw an error!{1}", ModLoader.LoadedMods[i].ID, errorDetails));
                    if (ModLoader.devMode)
                        ModConsole.Error(e.ToString());
                    System.Console.WriteLine(e);
                }
            }
        }


        // Open menu if the key is pressed.
        public override void Update()
        {
            // Open menu
            if (menuKey.GetKeybindDown())
            {
                settings.toggleVisibility();
            }
        }

        // SETUP LOGIC FOR THE MOD SETTINGS BUTTON (FREDTWEAK)
        public override void OnLoad()
        {
            GameObject.Find("Systems").transform.Find("OptionsMenu").gameObject.AddComponent<ModSettingButtonHandler>().modSettingButton = Button_ms;
            Button_ms.SetActive(false);
        }

        public class ModSettingButtonHandler : MonoBehaviour
        {
            public GameObject modSettingButton;

            void OnEnable()
            {
                if ((bool)modSetButton.GetValue())
                    modSettingButton.SetActive(true);

                StartCoroutine(CursorPM());
            }
            IEnumerator CursorPM()
            {
                yield return null;
                //Fix that shitty custom playmaker cursor to regular system one.
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            void OnDisable()
            {
                if (modSettingButton != null)
                    modSettingButton.SetActive(false);
            }
        }
    }
#pragma warning restore CS1591
}
