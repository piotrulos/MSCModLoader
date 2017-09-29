using System;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
#pragma warning disable CS1591
    public class ModSettingsUI : MonoBehaviour
    {
        public ModSettings ms = null;
        public void LoadUI()
        {
            StartCoroutine(LoadUIc());
        }
        IEnumerator LoadUIc()
        {
            AssetBundle ab = new AssetBundle();
            yield return StartCoroutine(ModLoader.loadAssets.LoadBundle(new ModSettings(), "settingsui.unity3d", value => ab = value));
           
            ms.UI = ab.LoadAsset("MSCLoader Settings.prefab") as GameObject;

            ms.ModButton = ab.LoadAsset("ModButton.prefab") as GameObject;
            ms.ModButton_Pre = ab.LoadAsset("ModButton_Pre.prefab") as GameObject;
            ms.ModButton_Invalid = ab.LoadAsset("ModButton_Invalid.prefab") as GameObject;
            ms.ModViewLabel = ab.LoadAsset("ModViewLabel.prefab") as GameObject;

            ms.KeyBind = ab.LoadAsset("KeyBind.prefab") as GameObject;

            ms.HasAssets = ab.LoadAsset("HasAssets.prefab") as GameObject;
            ms.PluginOk = ab.LoadAsset("PluginOK.prefab") as GameObject;
            ms.PluginDisabled = ab.LoadAsset("PluginDisabled.prefab") as GameObject;
            ms.InMenu = ab.LoadAsset("InMenu.prefab") as GameObject;
            ms.update = ab.LoadAsset("Update.prefab") as GameObject;

            //exit coroutine
            ms.CreateSettingsUI();

            //freeup memory
            ab.Unload(false);
            Destroy(gameObject);
        }
    }
    public class ModSettings : Mod
    {

        public override bool LoadInMenu => true;
        public override bool UseAssetsFolder => true;
        public override string ID => "MSCLoader_Settings";
        public override string Name => "Settings";
        public override string Version => ModLoader.Version;
        public override string Author => "piotrulos";

        private static Mod selectedMod = null;

        public GameObject UI;
        public GameObject ModButton;
        public GameObject ModButton_Pre;
        public GameObject ModButton_Invalid;
        public GameObject ModViewLabel;
        public GameObject KeyBind;

        //icons for SettinsView
        public GameObject HasAssets;
        public GameObject PluginOk;
        public GameObject PluginDisabled;
        public GameObject InMenu;
        public GameObject update;

        private Keybind menuKey = new Keybind("Open", "Open menu", KeyCode.M, KeyCode.LeftControl);
        public SettingsView settings;

        public void CreateSettingsUI()
        {

            //ModConsole.Print(UI.name);
            UI = GameObject.Instantiate(UI);
            UI.AddComponent<ModUIDrag>();
            settings = UI.AddComponent<SettingsView>();
            UI.GetComponent<SettingsView>().settingView = UI;
            UI.GetComponent<SettingsView>().settingViewContainer = UI.transform.GetChild(0).gameObject;
            UI.GetComponent<SettingsView>().modList = UI.GetComponent<SettingsView>().settingViewContainer.transform.GetChild(2).gameObject;
            UI.GetComponent<SettingsView>().modView = UI.GetComponent<SettingsView>().modList.transform.GetChild(0).gameObject;
            UI.GetComponent<SettingsView>().modSettings = UI.GetComponent<SettingsView>().settingViewContainer.transform.GetChild(1).gameObject;
            UI.GetComponent<SettingsView>().ModSettingsView = UI.GetComponent<SettingsView>().modSettings.transform.GetChild(0).gameObject;
            UI.GetComponent<SettingsView>().goBackBtn = UI.GetComponent<SettingsView>().settingViewContainer.transform.GetChild(0).GetChild(0).gameObject;
            UI.GetComponent<SettingsView>().goBackBtn.GetComponent<Button>().onClick.AddListener(() => UI.GetComponent<SettingsView>().goBack());
            UI.GetComponent<SettingsView>().keybindsList = UI.GetComponent<SettingsView>().ModSettingsView.transform.GetChild(8).gameObject;
            UI.GetComponent<SettingsView>().DisableMod = UI.GetComponent<SettingsView>().ModSettingsView.transform.GetChild(4).GetComponent<Toggle>();
            UI.GetComponent<SettingsView>().DisableMod.onValueChanged.AddListener(UI.GetComponent<SettingsView>().disableMod);

            UI.GetComponent<SettingsView>().ModButton = ModButton;
            UI.GetComponent<SettingsView>().ModButton_Pre = ModButton_Pre;
            UI.GetComponent<SettingsView>().ModButton_Invalid = ModButton_Invalid;
            UI.GetComponent<SettingsView>().ModViewLabel = ModViewLabel;
            UI.GetComponent<SettingsView>().KeyBind = KeyBind;

            UI.GetComponent<SettingsView>().HasAssets = HasAssets;
            UI.GetComponent<SettingsView>().PluginOk = PluginOk;
            UI.GetComponent<SettingsView>().PluginDisabled = PluginDisabled;
            UI.GetComponent<SettingsView>().InMenu = InMenu;
            UI.GetComponent<SettingsView>().update = update;

            UI.GetComponent<SettingsView>().IDtxt = UI.GetComponent<SettingsView>().ModSettingsView.transform.GetChild(0).GetComponent<Text>();
            UI.GetComponent<SettingsView>().Nametxt = UI.GetComponent<SettingsView>().ModSettingsView.transform.GetChild(1).GetComponent<Text>();
            UI.GetComponent<SettingsView>().Versiontxt = UI.GetComponent<SettingsView>().ModSettingsView.transform.GetChild(2).GetComponent<Text>();
            UI.GetComponent<SettingsView>().Authortxt = UI.GetComponent<SettingsView>().ModSettingsView.transform.GetChild(3).GetComponent<Text>();

            UI.transform.SetParent(GameObject.Find("MSCLoader Canvas").transform, false);
            settings.setVisibility(false);
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


            string path = Path.Combine(ModLoader.GetModConfigFolder(mod), "keybinds.xml");

            // Clear file
            File.WriteAllText(path, "");


            // Write XML
            XmlDocument doc = new XmlDocument();
            XmlElement keybinds = doc.CreateElement(string.Empty, "Keybinds", string.Empty);


            foreach (Keybind bind in Keybind.Get(mod))
            {
                XmlElement keybind = doc.CreateElement(string.Empty, "Keybind", string.Empty);

                XmlElement name = doc.CreateElement(string.Empty, "ID", string.Empty);
                name.AppendChild(doc.CreateTextNode(bind.ID));
                keybind.AppendChild(name);

                XmlElement key = doc.CreateElement(string.Empty, "Key", string.Empty);
                key.AppendChild(doc.CreateTextNode(bind.Key.ToString()));
                keybind.AppendChild(key);

                XmlElement modifier = doc.CreateElement(string.Empty, "Modifier", string.Empty);
                modifier.AppendChild(doc.CreateTextNode(bind.Modifier.ToString()));
                keybind.AppendChild(modifier);

                keybinds.AppendChild(keybind);

            }

            doc.AppendChild(keybinds);
            doc.Save(path);

        }


        // Load all keybinds.
        public static void LoadBinds()
        {
            foreach (Mod mod in ModLoader.LoadedMods)
            {
                // Check if there are custom keybinds
                string path = Path.Combine(ModLoader.GetModConfigFolder(mod), "keybinds.xml");

                if (!File.Exists(path))
                {
                    SaveModBinds(mod);
                    continue;
                }

                // Load XML
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                foreach (XmlNode keybind in doc.GetElementsByTagName("Keybind"))
                {
                    XmlNode id = keybind.SelectSingleNode("ID");
                    XmlNode key = keybind.SelectSingleNode("Key");
                    XmlNode modifier = keybind.SelectSingleNode("Modifier");

                    // Check if its valid and fetch
                    if (id == null || key == null || modifier == null)
                    {
                        continue;
                    }

                    Keybind bind = Keybind.Keybinds.Find(x => x.Mod == mod && x.ID == id.InnerText);

                    if (bind == null)
                    {
                        continue;
                    }

                    // Set bind
                    try
                    {
                        KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), key.InnerText);
                        bind.Key = code;
                    }
                    catch (Exception e)
                    {
                        bind.Key = KeyCode.None;
                        ModConsole.Error(e.Message);
                    }

                    try
                    {
                        KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), modifier.InnerText);
                        bind.Modifier = code;
                    }
                    catch (Exception e)
                    {
                        bind.Modifier = KeyCode.None;
                        ModConsole.Error(e.Message);
                    }
                }
            }
        }

        // Load the keybinds.
        public override void OnLoad()
        {
            try
            {
                GameObject go = new GameObject();
                ModSettingsUI ui = go.AddComponent<ModSettingsUI>();
                ui.ms = this;
                ui.LoadUI();
            }
            catch (Exception e)
            {
                ModConsole.Print(e.Message); //debug only
            }
            Keybind.Add(this, menuKey);
            LoadBinds();
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
