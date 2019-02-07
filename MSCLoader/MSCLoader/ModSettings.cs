using Newtonsoft.Json;
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
        public override string Name => "Settings";
        public override string Version => ModLoader.Version;
        public override string Author => "piotrulos";

        private static Mod selectedMod = null;

      

        private Keybind menuKey = new Keybind("Open", "Open menu", KeyCode.M, KeyCode.LeftControl);
        public static Settings expWarning = new Settings("expWarning", "Don't show experimental warning", false);

        public SettingsView settings;
        public GameObject UI;
        public GameObject ModButton;
        public GameObject ModButton_Invalid;
        public GameObject ModLabel;
        public GameObject KeyBind;
        public GameObject Checkbox, setBtn, slider, textBox, header;

        public override void ModSettings()
        {
            Settings.AddHeader(this, "Basic Settings");
            Settings.AddCheckBox(this, expWarning);            
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

            //For mod settings
            Checkbox = ab.LoadAsset<GameObject>("Checkbox.prefab");
            setBtn = ab.LoadAsset<GameObject>("Button.prefab");
            slider = ab.LoadAsset<GameObject>("Slider.prefab");
            textBox = ab.LoadAsset<GameObject>("TextBox.prefab");
            header = ab.LoadAsset<GameObject>("Header.prefab");

            UI = GameObject.Instantiate(UI);
            UI.AddComponent<ModUIDrag>();

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
            settings.settingViewContainer.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => settings.setVisibility(false));
            settings.DisableMod = ModSettingsView.transform.GetChild(5).GetComponent<Toggle>();
            settings.DisableMod.onValueChanged.AddListener(settings.disableMod);

            settings.IDtxt = ModSettingsView.transform.GetChild(0).GetComponent<Text>();
            settings.Nametxt = ModSettingsView.transform.GetChild(1).GetComponent<Text>();
            settings.Versiontxt = ModSettingsView.transform.GetChild(2).GetComponent<Text>();
            settings.Authortxt = ModSettingsView.transform.GetChild(3).GetComponent<Text>();

            UI.transform.SetParent(GameObject.Find("MSCLoader Canvas").transform, false);
            settings.setVisibility(false);
            ab.Unload(false);
        }

        // Reset keybinds
        private void resetBinds()
        {
            if (selectedMod != null)
            {
                // Delete file
                string path = Path.Combine(ModLoader.GetModConfigFolder(selectedMod), "keybinds.xml");
                File.WriteAllText(path, "");

                // Revert binds
                foreach (Keybind bind in Keybind.Get(selectedMod))
                {
                    Keybind original = Keybind.DefaultKeybinds.Find(x => x.Mod == selectedMod && x.ID == bind.ID);

                    if (original != null)
                    {
                        bind.Key = original.Key;
                        bind.Modifier = original.Modifier;

                        ModConsole.Print(original.Key.ToString() + " -> " + bind.Key.ToString());
                    }
                }

                // Save binds
                SaveModBinds(selectedMod);
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
            string path = Path.Combine(ModLoader.GetModConfigFolder(mod), "keybinds.json");
            
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
            string path = Path.Combine(ModLoader.GetModConfigFolder(mod), "settings.json");

            foreach (Settings set in Settings.Get(mod))
            {
                if (set.type == SettingsType.Button || set.type == SettingsType.Header)
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
                string path = Path.Combine(ModLoader.GetModConfigFolder(mod), "keybinds.xml");
                if (File.Exists(path))
                    File.Delete(path);

                // Check if there is custom keybinds file (if not, create)
                path = Path.Combine(ModLoader.GetModConfigFolder(mod), "keybinds.json");
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
                foreach(var kb in keybinds.keybinds)
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
                string path = Path.Combine(ModLoader.GetModConfigFolder(mod), "settings.json");
                if (!File.Exists(path))
                    continue;

                //Load and deserialize 
                SettingsList settings = new SettingsList();
                string serializedData = File.ReadAllText(path);
                settings = JsonConvert.DeserializeObject<SettingsList>(serializedData);
                mod.isDisabled = settings.isDisabled;
                if (settings.settings.Count == 0)
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
    }
#pragma warning restore CS1591
}
