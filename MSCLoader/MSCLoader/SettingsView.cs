using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MSCLoader
{
#pragma warning disable CS1591
    public class SettingsView : MonoBehaviour
    {
        public GameObject settingView;
        public GameObject settingViewContainer;
        public GameObject modList;
        public GameObject modView;
        public GameObject modInfo;
        public GameObject ModKeyBinds;
        public GameObject modSettings;

        public GameObject goBackBtn;
        public GameObject keybindsList;
        public GameObject modSettingsList;


        //from AssetBundle
        public GameObject ModButton;
        public GameObject ModButton_Invalid;
        public GameObject ModViewLabel;
        public Toggle DisableMod;
        public GameObject KeyBind;
        public GameObject Checkbox, setBtn, slider;


        public Text IDtxt;
        public Text Nametxt;
        public Text Versiontxt;
        public Text Authortxt;

        int page = 0;

        Mod selected;
        bool loadedLabel = false;
        bool preloadedLabel = false;
        bool invalidLabel = false;
        bool disabledLabel = false;
        public void modButton(string name, string version, string author, Mod mod)
        {
            if (!loadedLabel)
            {
                GameObject modViewLabel = Instantiate(ModViewLabel);
                modViewLabel.GetComponent<Text>().text = "Loaded Mods:";
                modViewLabel.transform.SetParent(modView.transform, false);
                loadedLabel = true;
            }
            GameObject modButton;
            if (!mod.LoadInMenu && Application.loadedLevelName == "MainMenu")
            {
                if (mod.isDisabled)
                {
                    if (!disabledLabel)
                    {
                        GameObject modViewLabel = Instantiate(ModViewLabel);
                        modViewLabel.GetComponent<Text>().text = "Disabled Mods:";
                        modViewLabel.transform.SetParent(modView.transform, false);
                        disabledLabel = true;
                    }
                }
                else
                {
                    if (!preloadedLabel)
                    {
                        GameObject modViewLabel = Instantiate(ModViewLabel);
                        modViewLabel.GetComponent<Text>().text = "Ready to Load:";
                        modViewLabel.transform.SetParent(modView.transform, false);
                        preloadedLabel = true;
                    }                   
                }
                //blue background, yellow title
                modButton = Instantiate(ModButton);
                ColorBlock cb = modButton.GetComponent<Button>().colors;
                cb.normalColor = new Color32(0, 0, 255, 100);
                modButton.GetComponent<Button>().colors = cb;
                modButton.transform.GetChild(0).GetComponent<Text>().color = Color.yellow;
            }
            else
            {
                if (mod.isDisabled)
                {
                    if (!disabledLabel)
                    {
                        GameObject modViewLabel = Instantiate(ModViewLabel);
                        modViewLabel.GetComponent<Text>().text = "Disabled Mods:";
                        modViewLabel.transform.SetParent(modView.transform, false);
                        disabledLabel = true;
                    }
                    //blue background, red title
                    modButton = Instantiate(ModButton);
                    ColorBlock cb = modButton.GetComponent<Button>().colors;
                    cb.normalColor = new Color32(0, 0, 255, 100);
                    modButton.GetComponent<Button>().colors = cb;
                    modButton.transform.GetChild(0).GetComponent<Text>().color = Color.red;
                }
                else
                {
                    modButton = Instantiate(ModButton);
                }
            }

            modButton.AddComponent<ModInfo>().mod = mod;
            modButton.GetComponent<Button>().onClick.AddListener(() => settingView.GetComponent<SettingsView>().selectMod());
            modButton.transform.GetChild(0).GetComponent<Text>().text = name;
            modButton.transform.GetChild(1).GetComponent<Text>().text = version;
            modButton.transform.GetChild(2).GetComponent<Text>().text = author;
            modButton.transform.SetParent(modView.transform, false);
            if(mod.hasUpdate)
            {
                modButton.transform.GetChild(3).GetChild(0).gameObject.SetActive(true); //Add Update Icon
            }
            if (mod.UseAssetsFolder)
            {
                modButton.transform.GetChild(3).GetChild(1).gameObject.SetActive(true); //Add assets icon
            }
            if (mod.isDisabled)
            {
                modButton.transform.GetChild(3).GetChild(2).gameObject.SetActive(true); //Add plugin Disabled icon
                modButton.transform.GetChild(3).GetChild(2).GetComponent<Image>().color = Color.red;
            }
            else
            {
                modButton.transform.GetChild(3).GetChild(2).gameObject.SetActive(true); //Add plugin OK icon
            }
            if (mod.LoadInMenu)
            {
                modButton.transform.GetChild(3).GetChild(3).gameObject.SetActive(true);//Add Menu Icon
            }

        }

        public void SettingsList(Settings setting)
        {
            switch (setting.type)
            {
                case SettingsType.CheckBox:
                    GameObject checkbox = Instantiate(Checkbox);
                    checkbox.transform.GetChild(1).GetComponent<Text>().text = setting.Name;
                    checkbox.GetComponent<Toggle>().isOn = (bool)setting.Value;
                    checkbox.GetComponent<Toggle>().onValueChanged.AddListener(delegate { setting.Value = checkbox.GetComponent<Toggle>().isOn; });
                    checkbox.transform.SetParent(modSettingsList.transform, false);
                    break;
                case SettingsType.Button:
                    GameObject btn = Instantiate(setBtn);
                    btn.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = setting.Name;
                    btn.transform.GetChild(1).GetComponent<Text>().text = setting.Vals[0].ToString();
                    btn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(setting.DoAction.Invoke);
                    btn.transform.SetParent(modSettingsList.transform, false);
                    break;
            }
        }

        public void KeyBindsList(string name, KeyCode modifier, KeyCode key, string ID)
        {
            GameObject keyBind = Instantiate(KeyBind);
            keyBind.transform.GetChild(0).GetComponent<Text>().text = name;
            keyBind.AddComponent<KeyBinding>().modifierButton = keyBind.transform.GetChild(1).gameObject;
            keyBind.GetComponent<KeyBinding>().modifierDisplay = keyBind.transform.GetChild(1).GetChild(0).GetComponent<Text>();
            keyBind.GetComponent<KeyBinding>().keyButton = keyBind.transform.GetChild(3).gameObject;
            keyBind.GetComponent<KeyBinding>().keyDisplay = keyBind.transform.GetChild(3).GetChild(0).GetComponent<Text>();
            keyBind.GetComponent<KeyBinding>().key = key;
            keyBind.GetComponent<KeyBinding>().modifierKey = modifier;
            keyBind.GetComponent<KeyBinding>().mod = selected;
            keyBind.GetComponent<KeyBinding>().id = ID;
            keyBind.GetComponent<KeyBinding>().LoadBind();
            keyBind.transform.SetParent(keybindsList.transform, false);
        }
        public void RemoveChildren(Transform parent) //clear 
        {
            foreach (Transform child in parent)
                Destroy(child.gameObject);
        }
        public void goBack()
        {
            Animator anim = settingViewContainer.GetComponent<Animator>();
            switch (page)
            {
                case 0:
                    //nothing.
                    break;
                case 1:
                    page = 0;
                    SetScrollRect();
                    CreateList();
                    anim.SetBool("goSetting", false);
                    goBackBtn.SetActive(false);
                    break;
                case 2:
                    page = 1;
                    SetScrollRect();
                    anim.SetBool("goKeybind", false);
                    break;
                case 3:
                    page = 1;
                    SetScrollRect();
                    ModSettings.SaveSettings(selected);
                    anim.SetBool("goModSetting", false);
                    break;
            }

        }
        void SetScrollRect()
        {
            switch (page)
            {
                case 0:
                    modSettings.GetComponent<ScrollRect>().enabled = false;
                    ModKeyBinds.GetComponent<ScrollRect>().enabled = false;
                    modInfo.GetComponent<ScrollRect>().enabled = false;
                    modList.GetComponent<ScrollRect>().enabled = true;
                    break;
                case 1:
                    modSettings.GetComponent<ScrollRect>().enabled = false;
                    ModKeyBinds.GetComponent<ScrollRect>().enabled = false;
                    modInfo.GetComponent<ScrollRect>().enabled = true;
                    modList.GetComponent<ScrollRect>().enabled = false;
                    break;
                case 2:
                    modSettings.GetComponent<ScrollRect>().enabled = false;
                    ModKeyBinds.GetComponent<ScrollRect>().enabled = true;
                    modInfo.GetComponent<ScrollRect>().enabled = false;
                    modList.GetComponent<ScrollRect>().enabled = false;
                    break;
                case 3:
                    modSettings.GetComponent<ScrollRect>().enabled = true;
                    ModKeyBinds.GetComponent<ScrollRect>().enabled = false;
                    modInfo.GetComponent<ScrollRect>().enabled = false;
                    modList.GetComponent<ScrollRect>().enabled = false;
                    break;
            }
        }
        public void goToKeybinds()
        {
            settingViewContainer.GetComponent<Animator>().SetBool("goKeybind", true);
            page = 2;
            SetScrollRect();
        }
        public void goToSettings()
        {
            settingViewContainer.GetComponent<Animator>().SetBool("goModSetting", true);
            page = 3;
            SetScrollRect();
        }
        public void disableMod(bool ischecked)
        {
            if (selected.isDisabled != ischecked)
            {
                selected.isDisabled = ischecked;
                if(ischecked)
                    ModConsole.Print(string.Format("Mod <b><color=orange>{0}</color></b> is <color=red><b>Disabled</b></color>",selected.Name));
                else
                    ModConsole.Print(string.Format("Mod <b><color=orange>{0}</color></b> is <color=green><b>Enabled</b></color>", selected.Name));
                ModSettings.SaveSettings(selected);
            }
        }

        public void selectMod()
        {
            bool core = false;
            selected = EventSystem.current.currentSelectedGameObject.GetComponent<ModInfo>().mod;
            if (selected.ID.StartsWith("MSCLoader_"))
                core = true; //can't disable core components
            goBackBtn.SetActive(true);
            IDtxt.text = string.Format("ID: <b>{0}</b>", selected.ID);
            Nametxt.text = string.Format("Name: <b>{0}</b>", selected.Name);
            if(core)
                Versiontxt.text = string.Format("Version: <b>{0}</b>", selected.Version);
            else
            {
                if(selected.hasUpdate)
                Versiontxt.text = string.Format("Version: <b>{0}</b> (<color=lime>Update available</color>){2}(compiled for <b>v{1}</b>)", selected.Version, selected.compiledVersion, Environment.NewLine);
                else
                    Versiontxt.text = string.Format("Version: <b>{0}</b>{2}(compiled for <b>v{1}</b>)", selected.Version, selected.compiledVersion, Environment.NewLine);

            }
            Authortxt.text = string.Format("Author: <b>{0}</b>", selected.Author);
            if (Application.loadedLevelName == "MainMenu" && !core)
                DisableMod.interactable = true;
            else
                DisableMod.interactable = false;
            DisableMod.isOn = selected.isDisabled;
            RemoveChildren(keybindsList.transform);
            RemoveChildren(modSettingsList.transform);
            bool hasKeybinds = false;
            foreach (Keybind key in Keybind.Keybinds)
            {
                if (key.Mod == selected)
                {
                    hasKeybinds = true;
                    KeyBindsList(key.Name, key.Modifier, key.Key, key.ID);                  
                }
            }
            if(!hasKeybinds)
            {
                //no keybinds
                if (Application.loadedLevelName == "MainMenu" && !selected.LoadInMenu)
                {
                    GameObject modViewLabel = Instantiate(ModViewLabel);
                    modViewLabel.GetComponent<Text>().text = "This mod is not loaded or disabled.";
                    modViewLabel.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                    modViewLabel.GetComponent<Text>().fontStyle = FontStyle.Italic; 
                    modViewLabel.transform.SetParent(keybindsList.transform, false);
                }
                else
                {
                    GameObject modViewLabel = Instantiate(ModViewLabel);
                    modViewLabel.GetComponent<Text>().text = "This mod has no defined keybinds.";
                    modViewLabel.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                    modViewLabel.GetComponent<Text>().fontStyle = FontStyle.Italic;
                    modViewLabel.transform.SetParent(keybindsList.transform, false);
                }
            }
            settingViewContainer.GetComponent<Animator>().SetBool("goSetting", true);
            bool hasSettings = false;

            foreach (Settings set in Settings.modSettings)
            {
                if(set.Mod == selected)
                {
                    hasSettings = true;
                    SettingsList(set);                    
                }
            }
            if (!hasSettings)
            {
                GameObject modViewLabel = Instantiate(ModViewLabel);
                modViewLabel.GetComponent<Text>().text = "This mod has no defined Settings.";
                modViewLabel.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                modViewLabel.GetComponent<Text>().fontStyle = FontStyle.Italic;
                modViewLabel.transform.SetParent(modSettingsList.transform, false);
            }
            page = 1;
            SetScrollRect();
        }

        void CreateList()
        {
            loadedLabel = false;
            preloadedLabel = false;
            invalidLabel = false;
            disabledLabel = false;
            RemoveChildren(modView.transform);
            if (Application.loadedLevelName == "MainMenu")
            {
                foreach (Mod mod in ModLoader.LoadedMods)
                {
                    if (mod.LoadInMenu)
                        modButton(mod.Name, mod.Version, mod.Author, mod);
                }
                foreach (Mod mod in ModLoader.LoadedMods)
                {
                    if (!mod.LoadInMenu && !mod.isDisabled)
                        modButton(mod.Name, mod.Version, mod.Author, mod);
                }
            }
            else
            {
                foreach (Mod mod in ModLoader.LoadedMods)
                {
                    if (!mod.isDisabled)
                        modButton(mod.Name, mod.Version, mod.Author, mod);
                }
            }

            foreach (Mod mod in ModLoader.LoadedMods)
            {
                if (mod.isDisabled)
                    modButton(mod.Name, mod.Version, mod.Author, mod);
            }
            foreach (string s in ModLoader.InvalidMods)
            {
                if (!invalidLabel)
                {
                    GameObject modViewLabel = Instantiate(ModViewLabel);
                    modViewLabel.GetComponent<Text>().text = "Invalid/Broken Mods:";
                    modViewLabel.transform.SetParent(modView.transform, false);
                    invalidLabel = true;
                }
                GameObject invMod = Instantiate(ModButton_Invalid);
                invMod.transform.GetChild(0).GetComponent<Text>().text = s;
                invMod.transform.SetParent(modView.transform, false);
            }
        }
        public void toggleVisibility()
        {
            if (!settingViewContainer.activeSelf)
            {
                CreateList();
                page = 0;
                SetScrollRect();
                setVisibility(!settingViewContainer.activeSelf);
            }
            else
            {
                setVisibility(!settingViewContainer.activeSelf);
            }
        }

        public void setVisibility(bool visible)
        {
            settingViewContainer.SetActive(visible);
        }
    }
#pragma warning restore CS1591

}
