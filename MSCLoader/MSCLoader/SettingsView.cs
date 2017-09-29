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
        public GameObject modSettings;
        public GameObject ModSettingsView;
        public GameObject goBackBtn;
        public GameObject keybindsList;

        //from AssetBundle
        public GameObject ModButton;
        public GameObject ModButton_Pre;
        public GameObject ModButton_Invalid;
        public GameObject ModViewLabel;
        public Toggle DisableMod;
        public GameObject KeyBind;

        //icons
        public GameObject HasAssets;
        public GameObject PluginOk;
        public GameObject PluginDisabled;
        public GameObject InMenu;
        public GameObject update;

        public Text IDtxt;
        public Text Nametxt;
        public Text Versiontxt;
        public Text Authortxt;

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
                modButton = Instantiate(ModButton_Pre);
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
                    modButton = Instantiate(ModButton_Pre);
                }
                else
                {
                    modButton = Instantiate(ModButton);
                }
            }              
            //ModButton = Instantiate(ModButton);
            modButton.AddComponent<ModInfo>().mod = mod;
            modButton.GetComponent<Button>().onClick.AddListener(() => settingView.GetComponent<SettingsView>().selectMod());
            modButton.transform.GetChild(0).GetComponent<Text>().text = name;
            modButton.transform.GetChild(1).GetComponent<Text>().text = version;
            modButton.transform.GetChild(2).GetComponent<Text>().text = author;
            modButton.transform.SetParent(modView.transform, false);
            if(mod.hasUpdate)
            {
                GameObject hasupdate = Instantiate(update);
                hasupdate.transform.SetParent(modButton.transform.GetChild(3), false); //Add Update Icon
            }
            if (mod.UseAssetsFolder)
            {
                GameObject hasAssets = Instantiate(HasAssets);
                hasAssets.transform.SetParent(modButton.transform.GetChild(3), false); //Add assets icon
            }
            if (mod.isDisabled)
            {
                GameObject pluginDisabled = Instantiate(PluginDisabled);
                pluginDisabled.transform.SetParent(modButton.transform.GetChild(3), false); //Add plugin Disabled icon
            }
            else
            {
                GameObject pluginOK = Instantiate(PluginOk);
                pluginOK.transform.SetParent(modButton.transform.GetChild(3), false); //Add plugin OK icon
            }
            if (mod.LoadInMenu)
            {
                GameObject inMenu = Instantiate(InMenu);
                inMenu.transform.SetParent(modButton.transform.GetChild(3), false); //Add Menu Icon
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
            modList.SetActive(true);
            modSettings.SetActive(false);
            goBackBtn.SetActive(false);
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
            }
        }

        public void selectMod()
        {
            bool core = false;
            selected = EventSystem.current.currentSelectedGameObject.GetComponent<ModInfo>().mod;
            if (selected.ID.StartsWith("MSCLoader_"))
                core = true; //can't disable core components
            goBackBtn.SetActive(true);
            modList.SetActive(false);
            modSettings.SetActive(true);
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
            foreach (Keybind key in Keybind.Keybinds)
            {
                if (key.Mod == selected)
                {
                    KeyBindsList(key.Name, key.Modifier, key.Key, key.ID);
                }
                else
                {
                    //no keybinds
                }
            }
            //ModConsole.Print(EventSystem.current.currentSelectedGameObject.GetComponent<ModInfo>().mod.ID); //debug
        }
        public void toggleVisibility()
        {
            if (!settingViewContainer.activeSelf)
            {
                loadedLabel = false;
                preloadedLabel = false;
                invalidLabel = false;
                disabledLabel = false;
                RemoveChildren(modView.transform);
                setVisibility(!settingViewContainer.activeSelf);
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
                        if(!mod.isDisabled)
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
                        modViewLabel.GetComponent<Text>().text = "Invalid Mods:";
                        modViewLabel.transform.SetParent(modView.transform, false);
                        invalidLabel = true;
                    }
                    GameObject invMod = Instantiate(ModButton_Invalid);
                    invMod.transform.GetChild(0).GetComponent<Text>().text = s;
                    invMod.transform.SetParent(modView.transform, false);
                }
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
