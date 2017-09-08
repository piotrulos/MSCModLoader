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

        //icons
        public GameObject HasAssets;
        public GameObject PluginOk;
        public GameObject PluginDisabled;
        public GameObject InMenu;

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

            GameObject keyBind = ModUI.CreateUIBase("KeyBind", keybindsList);
            keyBind.AddComponent<LayoutElement>().preferredHeight = 35;
            keyBind.AddComponent<KeyBinding>();
            GameObject keyName = ModUI.CreateTextBlock("Key Name", name, keyBind, TextAnchor.MiddleLeft);
            keyName.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            keyName.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            keyName.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
            keyName.GetComponent<RectTransform>().sizeDelta = new Vector2(290, 17);

            GameObject keyButton = ModUI.CreateUIBase("Modifier Button", keyBind);
            keyButton.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            keyButton.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            keyButton.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            keyButton.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 17);
            keyButton.AddComponent<Image>();
            keyButton.AddComponent<Button>().targetGraphic = keyButton.GetComponent<Image>();
            ColorBlock cb = keyButton.GetComponent<Button>().colors;
            cb.normalColor = new Color32(0x14, 0x14, 0x14, 0xFF);
            cb.highlightedColor = new Color32(0x50, 0x50, 0x50, 0xFF);
            cb.pressedColor = new Color32(0x64, 0x64, 0x64, 0xFF);
            keyButton.GetComponent<Button>().colors = cb;
            keyButton.GetComponent<Button>().targetGraphic = keyButton.GetComponent<Image>();

            GameObject BtnTxt = ModUI.CreateTextBlock("Text", "", keyButton, TextAnchor.MiddleCenter, Color.white);
            BtnTxt.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            BtnTxt.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            BtnTxt.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            BtnTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 17);

            keyBind.GetComponent<KeyBinding>().modifierButton = keyButton;
            keyBind.GetComponent<KeyBinding>().modifierDisplay = BtnTxt.GetComponent<Text>();

            GameObject plus = ModUI.CreateTextBlock("Text", "+", keyBind, TextAnchor.MiddleCenter, Color.white);
            plus.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            plus.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
            plus.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            plus.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 17);
            plus.GetComponent<Text>().fontStyle = FontStyle.Bold;
            plus.GetComponent<Text>().resizeTextForBestFit = true;

            GameObject keyButton2 = ModUI.CreateUIBase("Keybind Button", keyBind);
            keyButton2.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
            keyButton2.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
            keyButton2.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
            keyButton2.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 17);
            keyButton2.AddComponent<Image>();
            keyButton2.AddComponent<Button>().targetGraphic = keyButton2.GetComponent<Image>();
            keyButton2.GetComponent<Button>().colors = cb;
            keyButton2.GetComponent<Button>().targetGraphic = keyButton2.GetComponent<Image>();

            GameObject BtnTxt2 = ModUI.CreateTextBlock("Text", "", keyButton2, TextAnchor.MiddleCenter, Color.white);
            BtnTxt2.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            BtnTxt2.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            BtnTxt2.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            BtnTxt2.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 17);

            keyBind.GetComponent<KeyBinding>().keyButton = keyButton2;
            keyBind.GetComponent<KeyBinding>().keyDisplay = BtnTxt2.GetComponent<Text>();
            keyBind.GetComponent<KeyBinding>().key = key;
            keyBind.GetComponent<KeyBinding>().modifierKey = modifier;
            keyBind.GetComponent<KeyBinding>().mod = selected;
            keyBind.GetComponent<KeyBinding>().id = ID;
            keyBind.GetComponent<KeyBinding>().LoadBind();

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
            selected = EventSystem.current.currentSelectedGameObject.GetComponent<ModInfo>().mod;
            goBackBtn.SetActive(true);
            modList.SetActive(false);
            modSettings.SetActive(true);
            IDtxt.text = string.Format("ID: <b>{0}</b>", selected.ID);
            Nametxt.text = string.Format("Name: <b>{0}</b>", selected.Name);
            Versiontxt.text = string.Format("Version: <b>{0}</b>", selected.Version);
            Authortxt.text = string.Format("Author: <b>{0}</b>", selected.Author);
            if (Application.loadedLevelName == "MainMenu")
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
