using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
#pragma warning disable CS1591
    public class ModSettings : Mod
    {

        public override bool LoadInMenu => true;
        public override bool UseAssetsFolder => true;
        public override string ID => "MSCLoader_Settings";
        public override string Name => "Settings";
        public override string Version => ModLoader.Version;
        public override string Author => "piotrulos";

        private static Mod selectedMod = null;

        GameObject UI;
        GameObject ModButton;
        GameObject ModButton_Invalid;
        GameObject ModViewLabel;
        GameObject KeyBind;
        GameObject Checkbox, setBtn, slider;

        private Keybind menuKey = new Keybind("Open", "Open menu", KeyCode.M, KeyCode.LeftControl);
        public SettingsView settings;

        public override void OnMenuLoad()
        {
            CreateSettingsUI();
            Keybind.Add(this, menuKey);
        }

        public void CreateSettingsUI()
        {
            AssetBundle ab = LoadAssets.LoadBundle(this, "settingsui.unity3d");

            UI = ab.LoadAsset("MSCLoader Settings.prefab") as GameObject;

            ModButton = ab.LoadAsset("ModButton.prefab") as GameObject;
            ModButton_Invalid = ab.LoadAsset("ModButton_Invalid.prefab") as GameObject;
            ModViewLabel = ab.LoadAsset("ModViewLabel.prefab") as GameObject;

            KeyBind = ab.LoadAsset("KeyBind.prefab") as GameObject;

            Checkbox = ab.LoadAsset("Checkbox.prefab") as GameObject;
            setBtn = ab.LoadAsset("Button.prefab") as GameObject;
            slider = ab.LoadAsset("Slider.prefab") as GameObject;

            //ModConsole.Print(UI.name);
            UI = GameObject.Instantiate(UI);
            UI.AddComponent<ModUIDrag>();
            settings = UI.AddComponent<SettingsView>();
            UI.GetComponent<SettingsView>().settingView = UI;
            UI.GetComponent<SettingsView>().settingViewContainer = UI.transform.GetChild(0).gameObject;

            UI.GetComponent<SettingsView>().modList = UI.GetComponent<SettingsView>().settingViewContainer.transform.GetChild(3).gameObject;
            UI.GetComponent<SettingsView>().modView = UI.GetComponent<SettingsView>().modList.transform.GetChild(0).gameObject;
            UI.GetComponent<SettingsView>().modInfo = UI.GetComponent<SettingsView>().settingViewContainer.transform.GetChild(2).gameObject;
            GameObject ModSettingsView = UI.GetComponent<SettingsView>().modInfo.transform.GetChild(0).gameObject;
            UI.GetComponent<SettingsView>().ModKeyBinds = UI.GetComponent<SettingsView>().settingViewContainer.transform.GetChild(1).gameObject;
            UI.GetComponent<SettingsView>().keybindsList = UI.GetComponent<SettingsView>().ModKeyBinds.transform.GetChild(0).GetChild(4).gameObject;

            UI.GetComponent<SettingsView>().modSettings = UI.GetComponent<SettingsView>().settingViewContainer.transform.GetChild(4).gameObject;
            UI.GetComponent<SettingsView>().modSettingsList = UI.GetComponent<SettingsView>().modSettings.transform.GetChild(0).GetChild(4).gameObject;

            UI.GetComponent<SettingsView>().goBackBtn = UI.GetComponent<SettingsView>().settingViewContainer.transform.GetChild(0).GetChild(0).gameObject;
            UI.GetComponent<SettingsView>().goBackBtn.GetComponent<Button>().onClick.AddListener(() => UI.GetComponent<SettingsView>().goBack());
            UI.GetComponent<SettingsView>().DisableMod = ModSettingsView.transform.GetChild(5).GetComponent<Toggle>();
            UI.GetComponent<SettingsView>().DisableMod.onValueChanged.AddListener(UI.GetComponent<SettingsView>().disableMod);
            ModSettingsView.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(() => UI.GetComponent<SettingsView>().goToKeybinds());
            ModSettingsView.transform.GetChild(9).GetComponent<Button>().onClick.AddListener(() => UI.GetComponent<SettingsView>().goToSettings());

            UI.GetComponent<SettingsView>().ModButton = ModButton;
            UI.GetComponent<SettingsView>().ModButton_Invalid = ModButton_Invalid;
            UI.GetComponent<SettingsView>().ModViewLabel = ModViewLabel;
            UI.GetComponent<SettingsView>().KeyBind = KeyBind;

            UI.GetComponent<SettingsView>().Checkbox = Checkbox;
            UI.GetComponent<SettingsView>().setBtn = setBtn;
            UI.GetComponent<SettingsView>().slider = slider;

            UI.GetComponent<SettingsView>().IDtxt = ModSettingsView.transform.GetChild(0).GetComponent<Text>();
            UI.GetComponent<SettingsView>().Nametxt = ModSettingsView.transform.GetChild(1).GetComponent<Text>();
            UI.GetComponent<SettingsView>().Versiontxt = ModSettingsView.transform.GetChild(2).GetComponent<Text>();
            UI.GetComponent<SettingsView>().Authortxt = ModSettingsView.transform.GetChild(3).GetComponent<Text>();

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
                if (set.type == SettingsType.Button)
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
