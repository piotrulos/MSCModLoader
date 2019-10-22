﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
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

        internal static Settings expWarning = new Settings("mscloader_expWarning", "Show experimental warning", true);
        internal static Settings modPath = new Settings("mscloader_modPath", "Show mods folder", true, ModLoader.MainMenuPath);
        private static Settings modSetButton = new Settings("mscloader_modSetButton", "Show settings button in bottom right corner", true, ModSettingsToggle);
        internal static Settings forceMenuVsync = new Settings("mscloader_forceMenuVsync", "60FPS limit in Main Menu", true, VSyncSwitchCheckbox);
        internal static Settings openLinksOverlay = new Settings("mscloader_openLinksOverlay", "Open URLs in steam overlay", true);

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
            Settings.AddHeader(this, "Basic Settings", new Color32(0, 128, 0, 255));
            Settings.AddText(this, "All basic settings for MSCLoader");
            Settings.AddCheckBox(this, expWarning);
            Settings.AddCheckBox(this, modPath);
            Settings.AddCheckBox(this, modSetButton);
            Settings.AddCheckBox(this, forceMenuVsync);
            Settings.AddCheckBox(this, openLinksOverlay);
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
            CreateSettingsUI();
            Keybind.Add(this, menuKey);
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
            settings.setVisibility(false);
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
        public void ResetBinds(Mod mod)
        {
            if (mod != null)
            {
                // Delete file
                string path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "keybinds.json");

                // Revert binds
                foreach (Keybind bind in Keybind.Get(mod))
                {
                    Keybind original = Keybind.DefaultKeybinds.Find(x => x.Mod == mod && x.ID == bind.ID);

                    if (original != null)
                    {
                        ModConsole.Print(original.Key.ToString() + " -> " + bind.Key.ToString());
                        bind.Key = original.Key;
                        bind.Modifier = original.Modifier;
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

            foreach (Keybind bind in Keybind.Get(mod))
            {
                Keybinds keybinds = new Keybinds
                {
                    ID = bind.ID,
                    Key = bind.Key,
                    Modifier = bind.Modifier
                };

                list.keybinds.Add(keybinds);
            }

            string serializedData = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, serializedData);

        }

        // Save settings for a single mod to config file.
        public static void SaveSettings(Mod mod)
        {
            SettingsList list = new SettingsList();
            list.isDisabled = mod.isDisabled;
            string path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "settings.json");

            foreach (Settings set in Settings.Get(mod))
            {
                if (set.type == SettingsType.Button || set.type == SettingsType.Header || set.type == SettingsType.Text)
                    continue;

                Setting sets = new Setting
                {
                    ID = set.ID,
                    Value = set.Value
                };

                list.settings.Add(sets);
            }

            string serializedData = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, serializedData);

        }

        // Load all keybinds.
        public static void LoadBinds()
        {
            foreach (Mod mod in ModLoader.LoadedMods)
            {
                //delete old xml file (if exists)
                string path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "keybinds.xml");
                if (File.Exists(path))
                    File.Delete(path);

                // Check if there is custom keybinds file (if not, create)
                path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "keybinds.json");
                if (!File.Exists(path))
                {
                    SaveModBinds(mod);
                    continue;
                }

                //Load and deserialize 
                KeybindList keybinds = new KeybindList();
                string serializedData = File.ReadAllText(path);
                keybinds = JsonConvert.DeserializeObject<KeybindList>(serializedData);
                if (keybinds.keybinds.Count == 0)
                    continue;
                foreach (var kb in keybinds.keybinds)
                {
                    Keybind bind = Keybind.Keybinds.Find(x => x.Mod == mod && x.ID == kb.ID);
                    if (bind == null)
                        continue;
                    bind.Key = kb.Key;
                    bind.Modifier = kb.Modifier;
                }
            }
        }

        // Load all settings.
        public static void LoadSettings()
        {
            foreach (Mod mod in ModLoader.LoadedMods)
            {
                // Check if there is custom settings file (if not, ignore)
                string path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "settings.json");
                if (!File.Exists(path))            
                    SaveSettings(mod); //create settings file if not exists.
                

                //Load and deserialize 
                SettingsList settings = new SettingsList();
                string serializedData = File.ReadAllText(path);
                settings = JsonConvert.DeserializeObject<SettingsList>(serializedData);
                mod.isDisabled = settings.isDisabled;
                try
                {
                    if (mod.LoadInMenu && !mod.isDisabled && mod.fileName != null)
                    {
                        mod.OnMenuLoad();
                    }
                }
                catch (Exception e)
                {
                    StackFrame frame = new StackTrace(e, true).GetFrame(0);

                    string errorDetails = string.Format("{2}<b>Details: </b>{0} in <b>{1}</b>", e.Message, frame.GetMethod(), Environment.NewLine);
                    ModConsole.Error(string.Format("Mod <b>{0}</b> throw an error!{1}", mod.ID, errorDetails));
                    if (ModLoader.devMode)
                        ModConsole.Error(e.ToString());
                    UnityEngine.Debug.Log(e);
                }
                if (Settings.Get(mod).Count == 0)
                    continue;

                foreach (var kb in settings.settings)
                {
                    Settings set = Settings.modSettings.Find(x => x.Mod == mod && x.ID == kb.ID);
                    if (set == null)
                        continue;
                    set.Value = kb.Value;
                }
                mod.ModSettingsLoaded();
            }
        }


        // Open menu if the key is pressed.
        public override void Update()
        {
            // Open menu
            if (menuKey.IsDown())
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
            }

            void OnDisable()
            {
                modSettingButton.SetActive(false);
            }
        }
    }
#pragma warning restore CS1591
}
