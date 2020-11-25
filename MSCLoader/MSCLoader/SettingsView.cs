using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
#pragma warning disable CS1591
    public class SettingsView : MonoBehaviour
    {
        public ModSettings_menu ms;

        public GameObject settingViewContainer;
        public GameObject modList;
        public GameObject modView;
        public GameObject modInfo;
        public GameObject ModKeyBinds;
        public GameObject modSettings;
        public GameObject goBackBtn;
        public GameObject keybindsList;
        public GameObject modSettingsList;
        
        public Text InfoTxt;
        public Text noOfMods;
        public Text descriptionTxt;

        public Toggle DisableMod;
        public Toggle coreModCheckbox;

        public Button nexusLink;
        public Button rdLink;
        public Button ghLink;
        
        int page = 0;

        Mod selected_mod;
        public void modButton(string name, string version, string author, Mod mod)
        {

            GameObject modButton;
            if (!mod.LoadInMenu && Application.loadedLevelName == "MainMenu")
            {
                if (mod.isDisabled)
                {
                    modButton = Instantiate(ms.ModButton);
                    modButton.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.red;
                    modButton.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = "<color=red>Mod is disabled!</color>";
                }
                else
                {
                    modButton = Instantiate(ms.ModButton);
                    modButton.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.yellow;
                    modButton.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = "<color=yellow>Ready to load</color>";
                }
            }
            else
            {
                if (mod.isDisabled)
                {
                    modButton = Instantiate(ms.ModButton);
                    modButton.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.red;
                    modButton.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = "<color=red>Mod is disabled!</color>";
                }
                else
                {
                    if (mod.ID.StartsWith("MSCLoader_"))
                    {
                        if (coreModCheckbox.isOn)
                        {
                            modButton = Instantiate(ms.ModButton);
                            modButton.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.cyan;
                            modButton.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = "<color=cyan>Core Module!</color>";

                        }
                        else return;
                    }
                    else
                    {
                        modButton = Instantiate(ms.ModButton);
                        modButton.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.green;
                    }
                }
            }
            if (mod.ID.StartsWith("MSCLoader_")) //No details is needed for core modules
                modButton.transform.GetChild(1).GetChild(4).GetChild(0).GetComponent<Button>().interactable = false;
            modButton.transform.GetChild(1).GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(() => ms.settings.ModDetailsShow(mod));

            foreach (Settings set in Settings.modSettings)
            {
                if (set.Mod == mod)
                {
                    modButton.transform.GetChild(1).GetChild(4).GetChild(1).GetComponent<Button>().interactable = true;
                    modButton.transform.GetChild(1).GetChild(4).GetChild(1).GetComponent<Button>().onClick.AddListener(() => ms.settings.ModSettingsShow(mod));
                    break;
                }
            }
            foreach (Keybind key in Keybind.Keybinds)
            {
                if (key.Mod == mod)
                {
                    modButton.transform.GetChild(1).GetChild(4).GetChild(2).GetComponent<Button>().interactable = true;
                    modButton.transform.GetChild(1).GetChild(4).GetChild(2).GetComponent<Button>().onClick.AddListener(() => ms.settings.ModKeybindsShow(mod));
                    break;
                }
            }

            if (name.Length > 24)
                modButton.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = string.Format("{0}...", name.Substring(0, 22));
            else
                modButton.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = name;

            modButton.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = string.Format("by <color=orange>{0}</color>", author);
            modButton.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = version;
            modButton.transform.SetParent(modView.transform, false);
            if (mod.metadata != null && mod.metadata.icon.iconFileName != null && mod.metadata.icon.iconFileName != string.Empty)
            {
                Texture2D t2d = new Texture2D(1, 1);
                if (mod.metadata.icon.isIconRemote)
                {
                    if (File.Exists(Path.Combine(ModLoader.ManifestsFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName)))
                    {
                        try
                        {
                            t2d.LoadImage(File.ReadAllBytes(Path.Combine(ModLoader.ManifestsFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName)));
                            modButton.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = t2d;
                        }
                        catch (Exception e)
                        {
                            ModConsole.Error(e.Message);
                            System.Console.WriteLine(e);
                        }
                    }
                }
                else if (mod.metadata.icon.isIconUrl)
                {
                    try
                    {
                        WWW www = new WWW(mod.metadata.icon.iconFileName);
                        modButton.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = www.texture;
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error(e.Message);
                        System.Console.WriteLine(e);
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(ModLoader.GetModAssetsFolder(mod), mod.metadata.icon.iconFileName)))
                    {
                        try
                        {
                            t2d.LoadImage(File.ReadAllBytes(Path.Combine(ModLoader.GetModAssetsFolder(mod), mod.metadata.icon.iconFileName)));
                            modButton.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = t2d;
                        }
                        catch (Exception e)
                        {
                            ModConsole.Error(e.Message);
                            System.Console.WriteLine(e);
                        }
                    }
                }
            }
            if (mod.hasUpdate)
            {
                modButton.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = "<color=lime>UPDATE AVAILABLE!</color>";
            }
            if (mod.UseAssetsFolder)
            {
                modButton.transform.GetChild(2).GetChild(2).gameObject.SetActive(true); //Add assets icon
            }
            if (mod.isDisabled)
            {
                modButton.transform.GetChild(2).GetChild(1).gameObject.SetActive(true); //Add plugin Disabled icon
                modButton.transform.GetChild(2).GetChild(1).GetComponent<Image>().color = Color.red;
            }
            else
            {
                modButton.transform.GetChild(2).GetChild(1).gameObject.SetActive(true); //Add plugin OK icon
            }
            if (mod.LoadInMenu)
            {
                modButton.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);//Add Menu Icon
            }

        }
        public void ToggleCoreCheckbox()
        {
            if(page == 0)
            {
                CreateList();
            }
        }
        public void SettingsList(Settings setting)
        {
            switch (setting.type)
            {
                case SettingsType.CheckBox:
                    GameObject checkbox = Instantiate(ms.Checkbox);
                    checkbox.transform.GetChild(1).GetComponent<Text>().text = setting.Name;
                    checkbox.GetComponent<Toggle>().isOn = (bool)setting.Value;
                    checkbox.GetComponent<Toggle>().onValueChanged.AddListener(delegate
                    {
                        setting.Value = checkbox.GetComponent<Toggle>().isOn;
                        if (setting.DoAction != null)
                            setting.DoAction.Invoke();
                    });
                    checkbox.transform.SetParent(modSettingsList.transform, false);
                    break;
                case SettingsType.CheckBoxGroup:
                    GameObject group;
                    if (modSettingsList.transform.FindChild(setting.Vals[0].ToString()) == null)
                    {
                        group = new GameObject();
                        group.name = setting.Vals[0].ToString();
                        group.AddComponent<ToggleGroup>();
                        group.transform.SetParent(modSettingsList.transform, false);
                    }
                    else
                        group = modSettingsList.transform.FindChild(setting.Vals[0].ToString()).gameObject;
                    GameObject checkboxG = Instantiate(ms.Checkbox);
                    checkboxG.transform.GetChild(1).GetComponent<Text>().text = setting.Name;
                    checkboxG.GetComponent<Toggle>().group = group.GetComponent<ToggleGroup>();
                    checkboxG.GetComponent<Toggle>().isOn = (bool)setting.Value;
                    if((bool)setting.Value)
                        checkboxG.GetComponent<Toggle>().group.NotifyToggleOn(checkboxG.GetComponent<Toggle>());
                    checkboxG.GetComponent<Toggle>().onValueChanged.AddListener(delegate
                    {
                        setting.Value = checkboxG.GetComponent<Toggle>().isOn;
                        if (setting.DoAction != null)
                            setting.DoAction.Invoke();
                    });
                    checkboxG.transform.SetParent(modSettingsList.transform, false);
                    break;
                case SettingsType.Button:
                    GameObject btn = Instantiate(ms.setBtn);
                    btn.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = setting.Name;
                    btn.transform.GetChild(1).GetComponent<Text>().text = setting.Vals[0].ToString();
                    btn.transform.GetChild(1).GetComponent<Text>().color = (Color)setting.Vals[4];
                    if (setting.Vals[0].ToString() == null || setting.Vals[0].ToString() == string.Empty)
                        btn.transform.GetChild(1).gameObject.SetActive(false);
                    btn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(setting.DoAction.Invoke);
                    ColorBlock cb = btn.transform.GetChild(0).GetComponent<Button>().colors;
                    cb.normalColor = (Color)setting.Vals[1];
                    cb.highlightedColor = (Color)setting.Vals[2];
                    cb.pressedColor = (Color)setting.Vals[3];
                    btn.transform.GetChild(0).GetComponent<Button>().colors = cb; 
                    btn.transform.SetParent(modSettingsList.transform, false);
                    break;
                case SettingsType.RButton:
                    GameObject rbtn = Instantiate(ms.setBtn);
                    rbtn.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = setting.Name;
                    rbtn.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = Color.black;
                    rbtn.transform.GetChild(1).gameObject.SetActive(false);
                    rbtn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
                    {
                        ModSettings_menu.ResetSpecificSettings(setting.Mod, (Settings[])setting.Vals[0]);
                        ModSettingsShow(setting.Mod);
                        setting.Mod.ModSettingsLoaded();
                    });
                    ColorBlock rcb = rbtn.transform.GetChild(0).GetComponent<Button>().colors;
                    rcb.normalColor = new Color32(255, 187, 5, 255);
                    rcb.highlightedColor = new Color32(255, 230, 5, 255);
                    rcb.pressedColor = new Color32(255, 230, 5, 255);
                    rbtn.transform.GetChild(0).GetComponent<Button>().colors = rcb;
                    rbtn.transform.SetParent(modSettingsList.transform, false);
                    break;
                case SettingsType.Slider:
                    GameObject modViewLabel = Instantiate(ms.ModLabel);
                    modViewLabel.GetComponent<Text>().text = setting.Name;
                    modViewLabel.transform.SetParent(modSettingsList.transform, false);
                    GameObject slidr = Instantiate(ms.slider);
                    slidr.transform.GetChild(1).GetComponent<Text>().text = setting.Value.ToString();
                    slidr.transform.GetChild(0).GetComponent<Slider>().minValue = float.Parse(setting.Vals[0].ToString());
                    slidr.transform.GetChild(0).GetComponent<Slider>().maxValue = float.Parse(setting.Vals[1].ToString());
                    slidr.transform.GetChild(0).GetComponent<Slider>().value = float.Parse(setting.Value.ToString());
                    slidr.transform.GetChild(0).GetComponent<Slider>().wholeNumbers = (bool)setting.Vals[2];
                    if (setting.Vals[3] != null)
                    {
                        slidr.transform.GetChild(1).GetComponent<Text>().text = ((string[])setting.Vals[3])[int.Parse(setting.Value.ToString())];
                    }
                    slidr.transform.GetChild(0).GetComponent<Slider>().onValueChanged.AddListener(delegate
                    {
                        if ((bool)setting.Vals[2])
                            setting.Value = slidr.transform.GetChild(0).GetComponent<Slider>().value;
                        else
                            setting.Value = Math.Round(slidr.transform.GetChild(0).GetComponent<Slider>().value, 2);
                        if(setting.Vals[3] == null)
                        slidr.transform.GetChild(1).GetComponent<Text>().text = setting.Value.ToString();
                        else
                        {
                            slidr.transform.GetChild(1).GetComponent<Text>().text = ((string[])setting.Vals[3])[int.Parse(setting.Value.ToString())];
                        }
                        if (setting.DoAction != null)
                            setting.DoAction.Invoke();
                    });
                    slidr.transform.SetParent(modSettingsList.transform, false);
                    break;
                case SettingsType.TextBox:
                    GameObject modViewLabels = Instantiate(ms.ModLabel);
                    modViewLabels.GetComponent<Text>().text = setting.Name;
                    modViewLabels.GetComponent<Text>().color = (Color)setting.Vals[1];
                    modViewLabels.transform.SetParent(modSettingsList.transform, false);
                    GameObject txt = Instantiate(ms.textBox);
                    txt.transform.GetChild(0).GetComponent<Text>().text = setting.Vals[0].ToString();
                    txt.GetComponent<InputField>().text = setting.Value.ToString();
                    txt.GetComponent<InputField>().onValueChange.AddListener(delegate
                    {
                        setting.Value = txt.GetComponent<InputField>().text;
                    });
                    txt.transform.SetParent(modSettingsList.transform, false);
                    break;
                case SettingsType.Header:
                    GameObject hdr = Instantiate(ms.header);
                    hdr.transform.GetChild(0).GetComponent<Text>().text = setting.Name;
                    hdr.GetComponent<Image>().color = (Color)setting.Vals[1];
                    hdr.transform.GetChild(0).GetComponent<Text>().color = (Color)setting.Vals[2];
                    hdr.transform.SetParent(modSettingsList.transform, false);
                    break;
                case SettingsType.Text:
                    GameObject tx = Instantiate(ms.ModLabel);
                    tx.GetComponent<Text>().text = setting.Name;
                    tx.transform.SetParent(modSettingsList.transform, false);
                    break;
            }
        }
        public void KeyBindHeader(Keybind key)
        {
            GameObject hdr = Instantiate(ms.header);
            hdr.transform.GetChild(0).GetComponent<Text>().text = key.Name;
            hdr.GetComponent<Image>().color = (Color)key.Vals[1];
            hdr.transform.GetChild(0).GetComponent<Text>().color = (Color)key.Vals[2];
            hdr.transform.SetParent(keybindsList.transform, false);
        }
        public void KeyBindsList(Keybind key)
        {
            GameObject keyBind = Instantiate(ms.KeyBind);
            keyBind.transform.GetChild(0).GetComponent<Text>().text = key.Name;
            keyBind.AddComponent<KeyBinding>().modifierButton = keyBind.transform.GetChild(1).gameObject;
            keyBind.GetComponent<KeyBinding>().modifierDisplay = keyBind.transform.GetChild(1).GetChild(0).GetComponent<Text>();
            keyBind.GetComponent<KeyBinding>().keyButton = keyBind.transform.GetChild(3).gameObject;
            keyBind.GetComponent<KeyBinding>().keyDisplay = keyBind.transform.GetChild(3).GetChild(0).GetComponent<Text>();
            keyBind.GetComponent<KeyBinding>().key = key.Key;
            keyBind.GetComponent<KeyBinding>().modifierKey = key.Modifier;
            keyBind.GetComponent<KeyBinding>().mod = key.Mod;
            keyBind.GetComponent<KeyBinding>().id = key.ID;
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
                    anim.SetBool("goDetails", false);
                    goBackBtn.SetActive(false);
                    break;
                case 2:
                    page = 0;
                    SetScrollRect();
                    anim.SetBool("goKeybind", false);
                    goBackBtn.SetActive(false);
                    break;
                case 3:
                    page = 0;
                    SetScrollRect();
                    ModSettings_menu.SaveSettings(selected_mod);
                    anim.SetBool("goModSetting", false);
                    goBackBtn.SetActive(false);
                    RemoveChildren(modSettingsList.transform);
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
            if (selected_mod.isDisabled != ischecked)
            {
                selected_mod.isDisabled = ischecked;
                if (ischecked)
                    ModConsole.Print(string.Format("Mod <b><color=orange>{0}</color></b> is <color=red><b>Disabled</b></color>", selected_mod.Name));
                else
                    ModConsole.Print(string.Format("Mod <b><color=orange>{0}</color></b> is <color=green><b>Enabled</b></color>", selected_mod.Name));
                ModSettings_menu.SaveSettings(selected_mod);
            }
        }

        public void ModDetailsShow(Mod mod)
        {
            selected_mod = mod;
            goBackBtn.SetActive(true);
            InfoTxt.text = string.Format("<color=yellow>ID:</color> <b><color=lime>{0}</color></b>{1}", mod.ID, Environment.NewLine);
            InfoTxt.text += string.Format("<color=yellow>Name:</color> <b><color=lime>{0}</color></b>{1}", mod.Name, Environment.NewLine);

            if (mod.hasUpdate)
                InfoTxt.text += string.Format("<color=yellow>Version:</color> <b><color=orange>{0}</color></b> (<color=lime>{3} available</color>){2}(designed for <b><color=lime>v{1}</color></b>){2}", mod.Version, mod.compiledVersion, Environment.NewLine, mod.RemMetadata.version);
            else
                InfoTxt.text += string.Format("<color=yellow>Version:</color> <b><color=lime>{0}</color></b>{2}(designed for <b><color=lime>v{1}</color></b>){2}", mod.Version, mod.compiledVersion, Environment.NewLine);

            InfoTxt.text += string.Format("<color=yellow>Author:</color> <b><color=lime>{0}</color></b>", mod.Author);
            if (Application.loadedLevelName == "MainMenu")
                DisableMod.interactable = true;
            else
                DisableMod.interactable = false;
            DisableMod.isOn = mod.isDisabled;
            nexusLink.gameObject.SetActive(false);
            rdLink.gameObject.SetActive(false);
            ghLink.gameObject.SetActive(false);
            descriptionTxt.text = "<i>No description provided...</i>";
            if (mod.metadata != null)
            {
                if (!string.IsNullOrEmpty(mod.metadata.links.nexusLink))
                {
                    nexusLink.gameObject.SetActive(true);
                    nexusLink.onClick.RemoveAllListeners();
                    nexusLink.onClick.AddListener(() => OpenModLink(mod.metadata.links.nexusLink));
                }
                if (!string.IsNullOrEmpty(mod.metadata.links.rdLink))
                {
                    rdLink.gameObject.SetActive(true);
                    rdLink.onClick.RemoveAllListeners();
                    rdLink.onClick.AddListener(() => OpenModLink(mod.metadata.links.rdLink));
                }
                if (!string.IsNullOrEmpty(mod.metadata.links.githubLink))
                {
                    ghLink.gameObject.SetActive(true);
                    ghLink.onClick.RemoveAllListeners();
                    ghLink.onClick.AddListener(() => OpenModLink(mod.metadata.links.githubLink));
                }
                descriptionTxt.text = mod.metadata.description;
            }
            settingViewContainer.GetComponent<Animator>().SetBool("goDetails", true);
            
            page = 1;
            SetScrollRect();
        }
        private void OpenModLink(string url)
        {
            if ((bool)ModSettings_menu.openLinksOverlay.GetValue())
            {
                //try opening in steam overlay
                try
                {                 
                    Steamworks.SteamFriends.ActivateGameOverlayToWebPage(url);
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                    Application.OpenURL(url);
                    System.Console.WriteLine(url);
                }
            }
            else
            {
                Application.OpenURL(url);
                System.Console.WriteLine(url);
            }
        }
        public void ModKeybindsShow(Mod selected)
        {
            goBackBtn.SetActive(true);
            selected_mod = selected;
            RemoveChildren(keybindsList.transform);
            foreach (Keybind key in Keybind.Keybinds)
            {
                if (key.Mod == selected)
                {
                    if (key.ID == null && key.Vals != null)
                        KeyBindHeader(key);
                    else
                        KeyBindsList(key);
                }
            }
            ModKeyBinds.transform.GetChild(0).GetChild(6).GetComponent<Button>().onClick.RemoveAllListeners();
            ModKeyBinds.transform.GetChild(0).GetChild(6).GetComponent<Button>().onClick.AddListener(delegate 
            {
                ModSettings_menu.ResetBinds(selected);
                ModKeybindsShow(selected);
                });
            goToKeybinds();
        }
        public void ModSettingsShow(Mod selected)
        {
            goBackBtn.SetActive(true);
            selected_mod = selected;
            RemoveChildren(modSettingsList.transform);
            foreach (Settings set in Settings.modSettings)
            {
                if (set.Mod == selected)
                {
                    SettingsList(set);
                }
            }
            if (Settings.GetDefault(selected).Count == 0 || Settings.GetDefault(selected).Find(x => x.ID == "MSCL_HideResetAllButton") != null)
            {
                modSettings.transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
            }
            else
            {
                modSettings.transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
                modSettings.transform.GetChild(0).GetChild(6).GetComponent<Button>().onClick.RemoveAllListeners();
                modSettings.transform.GetChild(0).GetChild(6).GetComponent<Button>().onClick.AddListener(delegate
                {
                    ModSettings_menu.ResetSettings(selected);
                    ModSettingsShow(selected);
                    selected.ModSettingsLoaded();

                });
            }
            goToSettings();
        }
        void ModListHeader(string header,Color background, Color text)
        {
            GameObject hdr = Instantiate(ms.header);
            hdr.transform.GetChild(0).GetComponent<Text>().text = header;
            hdr.GetComponent<Image>().color = background;
            hdr.transform.GetChild(0).GetComponent<Text>().color = text;
            hdr.transform.SetParent(modView.transform, false);
        }
        void CreateList()
        {
            RemoveChildren(modView.transform);
            if(coreModCheckbox.isOn)
            {
                ModListHeader("Core Mods", new Color32(101, 34, 18, 255), new Color32(254, 254, 0, 255));
                foreach (Mod mod in ModLoader.LoadedMods.Where(x => x.ID.StartsWith("MSCLoader_")))
                {
                    modButton(mod.Name, mod.Version, mod.Author, mod);
                }
            }
            if (ModLoader.LoadedMods.Where(x => !x.ID.StartsWith("MSCLoader_")).Count() == 0)
            {
                ModListHeader("No mods installed", Color.blue, Color.white);
                if (ModLoader.InvalidMods.Count == 0)
                    return;
                ModListHeader("Invalid/Broken Mods", new Color32(101, 34, 18, 255), new Color32(254, 254, 0, 255));
                foreach (string s in ModLoader.InvalidMods)
                {
                    GameObject invMod = Instantiate(ms.ModButton_Invalid);
                    invMod.transform.GetChild(0).GetComponent<Text>().text = s;
                    invMod.transform.SetParent(modView.transform, false);
                }
                return;
            }
            if (ModLoader.LoadedMods.Where(x => x.hasUpdate).Count() > 0)
                ModListHeader("Mods with updates", new Color32(57, 57, 57, 255), new Color32(0, 255, 255, 255));
            foreach (Mod mod in ModLoader.LoadedMods.Where(x => x.hasUpdate))
            {
                modButton(mod.Name, mod.Version, mod.Author, mod);
            }
            if (Application.loadedLevelName == "MainMenu")
            {
                if (ModLoader.LoadedMods.Where(x => x.LoadInMenu && !x.ID.StartsWith("MSCLoader_")).Count() > 0)
                    ModListHeader("Loaded Mods", new Color32(57, 57, 57, 255), new Color32(0, 255, 255, 255));
                foreach (Mod mod in ModLoader.LoadedMods.Where(x => x.LoadInMenu && !x.ID.StartsWith("MSCLoader_")))
                {
                    if (mod.LoadInMenu)
                        modButton(mod.Name, mod.Version, mod.Author, mod);
                }
                ModListHeader("Ready to load", new Color32(57, 57, 57, 255), new Color32(0, 255, 255, 255));
                foreach (Mod mod in ModLoader.LoadedMods.Where(x => !x.ID.StartsWith("MSCLoader_")))
                {
                    if (!mod.LoadInMenu && !mod.isDisabled)
                        modButton(mod.Name, mod.Version, mod.Author, mod);
                }
            }
            else
            {
                ModListHeader("Loaded Mods", new Color32(57, 57, 57, 255), new Color32(0, 255, 255, 255));
                foreach (Mod mod in ModLoader.LoadedMods.Where(x => !x.ID.StartsWith("MSCLoader_")))
                {
                    if (!mod.isDisabled)
                        modButton(mod.Name, mod.Version, mod.Author, mod);
                }
            }
            if (ModLoader.LoadedMods.Where(x => x.isDisabled).Count() > 0)
                ModListHeader("Disabled mods", new Color32(101, 34, 18, 255), new Color32(254, 254, 0, 255));
            foreach (Mod mod in ModLoader.LoadedMods.Where(x => x.isDisabled))
            {
                if (mod.isDisabled)
                    modButton(mod.Name, mod.Version, mod.Author, mod);
            }
            if (ModLoader.InvalidMods.Count == 0)
                return;
            ModListHeader("Invalid/Broken Mods", new Color32(101, 34, 18, 255), new Color32(254, 254, 0, 255));
            foreach (string s in ModLoader.InvalidMods)
            {
                GameObject invMod = Instantiate(ms.ModButton_Invalid);
                invMod.transform.GetChild(0).GetComponent<Text>().text = s;
                invMod.transform.SetParent(modView.transform, false);
            }
        }
        public void toggleVisibility()
        {
            if (!settingViewContainer.activeSelf)
            {
                noOfMods.text = string.Format("<color=orange><b>{0}</b></color> Mods",ModLoader.LoadedMods.Count - 2);
                CreateList();
                page = 0;
                SetScrollRect();
                setVisibility(!settingViewContainer.activeSelf);
                goBackBtn.SetActive(false);
            }
            else
            {
                if (page == 3)
                {
                    ModSettings_menu.SaveSettings(selected_mod);
                    RemoveChildren(modSettingsList.transform);
                }
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
