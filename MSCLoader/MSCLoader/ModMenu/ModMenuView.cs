using MSCLoader;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    internal class ModMenuView : MonoBehaviour
    {
        public Text ModTab, ReferenceTab, UpdateTab;

        public GameObject ModElementPrefab, ReferenceElementPrefab, UpdateElementPrefab;
        public GameObject HeaderGroupPrefab;
        public GameObject ButtonPrefab, CheckBoxPrefab, KeyBindPrefab, LabelPrefab, SliderPrefab, TextBoxPrefab;
        public UniversalView universalView;

        public bool modList = false;
        public GameObject modListView;
        public void ModMenuOpened()
        {
            ModTab.text = $"Mods ({ModLoader.Instance.actualModList.Length})";
            ReferenceTab.text = $"References ({ModLoader.Instance.ReferencesList.Count})";
            UpdateTab.text = $"Updates ({ModLoader.Instance.HasUpdateModList.Count})";
            if (modList) ModList(modListView);
        }

        public void ModList(GameObject listView)
        {
            RemoveChildren(listView.transform);
            if(ModLoader.Instance.actualModList.Length == 0)
            {
                GameObject tx2 = Instantiate(LabelPrefab);
                tx2.GetComponent<Text>().text = $"<color=aqua>A little empty here, seems like there is no mods installed.{Environment.NewLine}If you think that you installed mods, check if you put mods in correct folder.{Environment.NewLine}Current Mod folder is: <color=yellow>{ModLoader.ModsFolder}</color></color>";
                tx2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tx2.transform.SetParent(listView.transform, false);
            }
            for (int i = 0; i < ModLoader.Instance.actualModList.Length; i++)
            {
                GameObject mod = GameObject.Instantiate(ModElementPrefab);
                mod.GetComponent<MenuElementList>().mod = ModLoader.Instance.actualModList[i];
                mod.GetComponent<MenuElementList>().ModInfoFill();
                mod.GetComponent<MenuElementList>().ModButtonsPrep(universalView);
                mod.transform.SetParent(listView.transform, false);
            }
            for(int i = 0; i < ModLoader.InvalidMods.Count; i++)
            {
                GameObject mod = GameObject.Instantiate(ModElementPrefab);
                mod.GetComponent<MenuElementList>().InvalidMod(ModLoader.InvalidMods[i]);
                mod.transform.SetParent(listView.transform, false);
            }
        }

        public void UpdateList(GameObject listView)
        {
            RemoveChildren(listView.transform);
            if (ModLoader.Instance.HasUpdateModList.Count == 0)
            {
                GameObject tx2 = Instantiate(LabelPrefab);
                tx2.GetComponent<Text>().text = $"<color=aqua>Everything seems to be up to date!</color>";
                tx2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tx2.transform.SetParent(listView.transform, false);
            }
            for (int i = 0; i < ModLoader.Instance.HasUpdateModList.Count; i++)
            {
                GameObject mod = GameObject.Instantiate(UpdateElementPrefab);
                mod.GetComponent<MenuElementList>().mod = ModLoader.Instance.HasUpdateModList[i];
                mod.GetComponent<MenuElementList>().UpdateInfoFill();
                mod.transform.SetParent(listView.transform, false);
            }
        }  
        public void ReferencesList(GameObject listView)
        {
            RemoveChildren(listView.transform);
            if (ModLoader.Instance.ReferencesList.Count == 0)
            {
                GameObject tx2 = Instantiate(LabelPrefab);
                tx2.GetComponent<Text>().text = $"<color=aqua>No additional references are installed.</color>";
                tx2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tx2.transform.SetParent(listView.transform, false);
            }
            for (int i = 0; i < ModLoader.Instance.ReferencesList.Count; i++)
            {
                GameObject mod = GameObject.Instantiate(ReferenceElementPrefab);
                mod.GetComponent<MenuElementList>().ReferenceInfoFill(ModLoader.Instance.ReferencesList[i]);
                mod.transform.SetParent(listView.transform, false);
            }
        }
        public void MainSettingsList(GameObject listView)
        {
            RemoveChildren(listView.transform);
            Transform currentTransform = null;
            for (int i = 0; i < Settings.Get(ModLoader.LoadedMods[0]).Count; i++)
            {
                if (Settings.Get(ModLoader.LoadedMods[0])[i].SettingType == SettingsType.Header)
                    currentTransform = SettingsHeader(Settings.Get(ModLoader.LoadedMods[0])[i], listView.transform);
                else
                    SettingsList(Settings.Get(ModLoader.LoadedMods[0])[i], currentTransform);
            }
            GameObject keyBind = Instantiate(KeyBindPrefab);
            keyBind.GetComponent<KeyBinding>().LoadBind(ModLoader.LoadedMods[0].Keybinds[0], ModLoader.LoadedMods[0]);
            keyBind.transform.SetParent(currentTransform, false);
            for (int i = 0; i < Settings.Get(ModLoader.LoadedMods[1]).Count; i++)
            {
                if (Settings.Get(ModLoader.LoadedMods[1])[i].SettingType == SettingsType.Header)
                    currentTransform = SettingsHeader(Settings.Get(ModLoader.LoadedMods[1])[i], listView.transform);
                else
                    SettingsList(Settings.Get(ModLoader.LoadedMods[1])[i], currentTransform);
            }
        }
        public void MetadataInfoList(GameObject listView, Mod mod)
        {
            RemoveChildren(listView.transform);
            listView.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
            //Info Header
            GameObject hdr = GameObject.Instantiate(HeaderGroupPrefab);
            SettingsGroup header = hdr.GetComponent<SettingsGroup>();
            header.HeaderTitle.text = "Mod Information".ToUpper();
            hdr.transform.SetParent(listView.transform, false);
            GameObject tx = Instantiate(LabelPrefab);
            tx.GetComponent<Text>().text = $"<color=yellow>ID:</color> <color=aqua>{mod.ID}</color> (Compiled using MSCLoader <color=yellow>{mod.compiledVersion}</color>){Environment.NewLine}" +
                $"<color=yellow>Version:</color> <color=aqua>{mod.Version}</color>{Environment.NewLine}" +
                $"<color=yellow>Author:</color> <color=aqua>{mod.Author}</color>{Environment.NewLine}" +
                $"<color=yellow>Additional references used by this Mod:</color>{Environment.NewLine}";
            if (mod.AdditionalReferences != null)
                tx.GetComponent<Text>().text += $"<color=aqua>{string.Join(", ",mod.AdditionalReferences)}</color>";
            else
                tx.GetComponent<Text>().text += $"<color=aqua>[None]</color>";
            tx.transform.SetParent(header.HeaderListView.transform, false);
            if (mod.metadata == null)
            {
                GameObject hdr2 = GameObject.Instantiate(HeaderGroupPrefab);
                SettingsGroup header2 = hdr2.GetComponent<SettingsGroup>();
                header2.HeaderTitle.text = "No metadata".ToUpper();
                header2.HeaderTitle.color = Color.yellow;
                // header2.HeaderBackground.color = (Color)setting.Vals[1];
                hdr2.transform.SetParent(listView.transform, false);
                GameObject tx2 = Instantiate(LabelPrefab);
                tx2.GetComponent<Text>().text = $"<color=yellow>This mod doesn't contain additional information</color>";
                tx2.transform.SetParent(header2.HeaderListView.transform, false);
            }
            else
            {
                GameObject hdr2 = GameObject.Instantiate(HeaderGroupPrefab);
                SettingsGroup header2 = hdr2.GetComponent<SettingsGroup>();
                header2.HeaderTitle.text = "Website Links".ToUpper();
                header2.HeaderTitle.color = Color.yellow;
                // header2.HeaderBackground.color = (Color)setting.Vals[1];
                hdr2.transform.SetParent(listView.transform, false);
                if (string.IsNullOrEmpty(mod.metadata.links.nexusLink) && string.IsNullOrEmpty(mod.metadata.links.rdLink) && string.IsNullOrEmpty(mod.metadata.links.githubLink))
                {
                    GameObject tx2 = Instantiate(LabelPrefab);
                    tx2.GetComponent<Text>().text = $"<color=yellow>This mod doesn't contain links</color>";
                    tx2.transform.SetParent(header2.HeaderListView.transform, false);
                }
                else
                {
                    if (!string.IsNullOrEmpty(mod.metadata.links.nexusLink))
                    {
                        GameObject nexusBtnP = Instantiate(ButtonPrefab);
                        SettingsElement nexusBtn = nexusBtnP.GetComponent<SettingsElement>();
                        nexusBtn.settingName.text = "SHOW ON <color=orange>NEXUSMODS.COM</color>";
                        nexusBtn.settingName.alignment = TextAnchor.MiddleLeft;
                        nexusBtn.iconElement.texture = nexusBtn.iconPack[1];
                        nexusBtn.iconElement.gameObject.SetActive(true);
                        nexusBtn.button.GetComponent<Image>().color = new Color32(2, 35, 60, 255);
                        nexusBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links.nexusLink));
                        nexusBtn.transform.SetParent(header2.HeaderListView.transform, false);
                    }
                    if (!string.IsNullOrEmpty(mod.metadata.links.rdLink))
                    {
                        GameObject rdBtnP = Instantiate(ButtonPrefab);
                        SettingsElement rdBtn = rdBtnP.GetComponent<SettingsElement>();
                        rdBtn.settingName.text = "SHOW ON <color=orange>RACEDEPARTMENT.COM</color>";
                        rdBtn.settingName.alignment = TextAnchor.MiddleLeft; 
                        rdBtn.iconElement.texture = rdBtn.iconPack[0];
                        rdBtn.iconElement.gameObject.SetActive(true);
                        rdBtn.button.GetComponent<Image>().color = new Color32(2, 35, 49, 255);
                        rdBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links.rdLink));
                        rdBtn.transform.SetParent(header2.HeaderListView.transform, false);
                    }
                    if (!string.IsNullOrEmpty(mod.metadata.links.githubLink))
                    {
                        GameObject ghBtnP = Instantiate(ButtonPrefab);
                        SettingsElement ghBtn = ghBtnP.GetComponent<SettingsElement>();
                        ghBtn.settingName.text = "SHOW ON <color=orange>GITHUB.COM</color>";
                        ghBtn.settingName.alignment = TextAnchor.MiddleLeft;
                        ghBtn.iconElement.texture = ghBtn.iconPack[2];
                        ghBtn.iconElement.gameObject.SetActive(true);
                        ghBtn.button.GetComponent<Image>().color = Color.black;
                        ghBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links.githubLink));
                        ghBtn.transform.SetParent(header2.HeaderListView.transform, false);
                    }
                }
                GameObject hdr3 = GameObject.Instantiate(HeaderGroupPrefab);
                SettingsGroup header3 = hdr3.GetComponent<SettingsGroup>();
                header3.HeaderTitle.text = "Description".ToUpper();
                header3.HeaderTitle.color = Color.yellow;
                // header2.HeaderBackground.color = (Color)setting.Vals[1];
                hdr3.transform.SetParent(listView.transform, false);
                GameObject tx3 = Instantiate(LabelPrefab);
                tx3.GetComponent<Text>().text = mod.metadata.description;
                tx3.transform.SetParent(header3.HeaderListView.transform, false);
            }
        }
        internal static void OpenModLink(string url)
        {
            if (ModMenu.openLinksOverlay.GetValue())
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
        public void ModSettingsList(GameObject listView, Mod mod)
        {
            RemoveChildren(listView.transform);
            listView.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
            Transform currentTransform = null;
            //If first settings element is not header, create one.
            if (mod.proSettings)
            {
                GameObject tx2 = Instantiate(LabelPrefab);
                tx2.GetComponent<Text>().text = $"<color=aqua>Incompatible settings format! Settings below may not load correctly.</color>";
                tx2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                tx2.transform.SetParent(listView.transform, false);
            }
            if (Settings.Get(mod)[0].SettingType != SettingsType.Header)
            {
                GameObject hdr = GameObject.Instantiate(HeaderGroupPrefab);
                SettingsGroup header = hdr.GetComponent<SettingsGroup>();
                header.HeaderTitle.text = "Settings".ToUpper();
                hdr.transform.SetParent(listView.transform, false);
                currentTransform = header.HeaderListView.transform;
            }

            for (int i = 0; i < Settings.Get(mod).Count; i++)
            {
                if (Settings.Get(mod)[i].SettingType == SettingsType.Header)
                    currentTransform = SettingsHeader(Settings.Get(mod)[i], listView.transform);
                else
                    SettingsList(Settings.Get(mod)[i], currentTransform);
            }
            GameObject rbtnP = Instantiate(ButtonPrefab);
            SettingsElement rbtn = rbtnP.GetComponent<SettingsElement>();
            rbtn.settingName.text = "Reset all settings to default".ToUpper();
            rbtn.settingName.color = Color.white;
            rbtn.button.GetComponent<Image>().color = Color.black;
            rbtn.button.onClick.AddListener(delegate
            {
                ModMenu.ResetSettings(mod);
                universalView.FillSettings(mod);
                mod.ModSettingsLoaded();
            });
            rbtn.transform.SetParent(listView.transform, false);
        }
        public void KeyBindsList(GameObject listView, Mod mod)
        {
            RemoveChildren(listView.transform);
            listView.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
            Transform currentTransform = null;
            //If first settings element is not header, create one.
            if (mod.Keybinds[0].ID != null && mod.Keybinds[0].Vals == null)
            {
                GameObject hdr = GameObject.Instantiate(HeaderGroupPrefab);
                SettingsGroup header = hdr.GetComponent<SettingsGroup>();
                header.HeaderTitle.text = "Keybinds".ToUpper();
                hdr.transform.SetParent(listView.transform, false);
                currentTransform = header.HeaderListView.transform;
            }
            for (int i = 0; i < mod.Keybinds.Count; i++)
            {
                if (mod.Keybinds[i].ID == null && mod.Keybinds[i].Vals != null)
                {
                    GameObject hdr = GameObject.Instantiate(HeaderGroupPrefab);
                    SettingsGroup header = hdr.GetComponent<SettingsGroup>();
                    header.HeaderTitle.text = mod.Keybinds[i].Name.ToUpper();
                    header.HeaderTitle.color = (Color)mod.Keybinds[i].Vals[1];
                    header.HeaderBackground.color = (Color)mod.Keybinds[i].Vals[0];
                    hdr.transform.SetParent(listView.transform, false);
                    currentTransform = header.HeaderListView.transform;
                }
                else
                {
                    GameObject keyBind = Instantiate(KeyBindPrefab);
                    keyBind.GetComponent<KeyBinding>().LoadBind(mod.Keybinds[i], mod);
                    keyBind.transform.SetParent(currentTransform, false);
                }
            }
            GameObject rbtnP = Instantiate(ButtonPrefab);
            SettingsElement rbtn = rbtnP.GetComponent<SettingsElement>();
            rbtn.settingName.text = "Reset all Keybinds to default".ToUpper();
            rbtn.settingName.color = Color.white;
            rbtn.button.GetComponent<Image>().color = Color.black;
            rbtn.button.onClick.AddListener(delegate
            {
                ModMenu.ResetBinds(mod);
                universalView.FillKeybinds(mod);
            });
            rbtn.transform.SetParent(listView.transform, false);
        }
        Transform SettingsHeader(Settings setting, Transform listView)
        {
            GameObject hdr = GameObject.Instantiate(HeaderGroupPrefab);
            SettingsGroup header = hdr.GetComponent<SettingsGroup>();
            header.HeaderTitle.text = setting.Name.ToUpper();
            header.HeaderTitle.color = (Color)setting.Vals[2];
            header.HeaderBackground.color = (Color)setting.Vals[1];
            hdr.transform.SetParent(listView.transform, false);
            return header.HeaderListView.transform;

        }

        public void SettingsList(Settings setting, Transform listView)
        {
            switch (setting.SettingType)
            {
                case SettingsType.CheckBox:
                    GameObject checkboxP = Instantiate(CheckBoxPrefab);
                    SettingsElement checkbox = checkboxP.GetComponent<SettingsElement>();
                    setting.NameText = checkbox.settingName;
                    setting.NameText.text = setting.Name;
                    try
                    {
                        checkbox.checkBox.isOn = bool.Parse(setting.Value.ToString());
                    }
                    catch(Exception e)
                    {
                        ModConsole.Error($"Settings [ID: <b>{setting.ID}</b>] Invalid value <b>{setting.Value}</b>{Environment.NewLine}<b>Error details:</b> {e.Message}");
                        Console.WriteLine(e);
                        checkbox.checkBox.isOn = false;
                        setting.Value = false;
                    }
                    checkbox.checkBox.onValueChanged.AddListener(delegate
                    {
                        setting.Value = checkbox.checkBox.isOn;
                        if (setting.DoAction != null)
                            setting.DoAction.Invoke();
                    });
                    checkbox.transform.SetParent(listView, false);
                    break;
                case SettingsType.CheckBoxGroup:
                    GameObject group;
                    if (listView.FindChild(setting.Vals[0].ToString()) == null)
                    {
                        group = new GameObject(setting.Vals[0].ToString());
                        group.AddComponent<ToggleGroup>();
                        group.transform.SetParent(listView, false);
                    }
                    else
                        group = listView.FindChild(setting.Vals[0].ToString()).gameObject;
                    GameObject checkboxGP = Instantiate(CheckBoxPrefab);
                    SettingsElement checkboxG = checkboxGP.GetComponent<SettingsElement>();
                    setting.NameText = checkboxG.settingName;
                    setting.NameText.text = setting.Name;

                    checkboxG.checkBox.group = group.GetComponent<ToggleGroup>();
                    try
                    {
                        checkboxG.checkBox.isOn = bool.Parse(setting.Value.ToString());
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error($"Settings [ID: <b>{setting.ID}</b>] Invalid value <b>{setting.Value}</b>{Environment.NewLine}<b>Error details:</b> {e.Message}");
                        Console.WriteLine(e);
                        checkboxG.checkBox.isOn = false;
                        setting.Value = false;
                    }
                    if ((bool)setting.Value)
                        checkboxG.checkBox.group.NotifyToggleOn(checkboxG.checkBox);
                    checkboxG.checkBox.onValueChanged.AddListener(delegate
                    {
                        setting.Value = checkboxG.checkBox.isOn;
                        if (setting.DoAction != null)
                            setting.DoAction.Invoke();
                    });
                    checkboxG.transform.SetParent(listView, false);
                    break;
                case SettingsType.Button:
                    GameObject btnP = Instantiate(ButtonPrefab);
                    SettingsElement btn = btnP.GetComponent<SettingsElement>();
                    setting.NameText = btn.settingName;
                    setting.NameText.text = setting.Name.ToUpper();                    
                    setting.NameText.color = (Color)setting.Vals[1];
                    btn.button.GetComponent<Image>().color = (Color)setting.Vals[0];
                    btn.button.onClick.AddListener(setting.DoAction.Invoke);
                    btn.transform.SetParent(listView, false);
                    break;
                case SettingsType.RButton:
                    GameObject rbtnP = Instantiate(ButtonPrefab);
                    SettingsElement rbtn = rbtnP.GetComponent<SettingsElement>();

                    setting.NameText = rbtn.settingName;
                    setting.NameText.text = setting.Name.ToUpper(); 
                    setting.NameText.color = Color.white;
                    rbtn.button.GetComponent<Image>().color = Color.black;
                    rbtn.button.onClick.AddListener(delegate
                    {
                        ModMenu.ResetSpecificSettings(setting.Mod, (Settings[])setting.Vals[0]);
                        universalView.FillSettings(setting.Mod);
                        setting.Mod.ModSettingsLoaded();
                    });
                    rbtn.transform.SetParent(listView, false);
                    break;
                case SettingsType.Slider:
                    GameObject slidrP = Instantiate(SliderPrefab);
                    SettingsElement slidr = slidrP.GetComponent<SettingsElement>();
                    setting.NameText = slidr.settingName;
                    setting.NameText.text = setting.Name;
                    slidr.value.text = setting.Value.ToString();
                    slidr.slider.minValue = float.Parse(setting.Vals[0].ToString());
                    slidr.slider.maxValue = float.Parse(setting.Vals[1].ToString());
                    try 
                    { 
                        slidr.slider.value = float.Parse(setting.Value.ToString()); 
                    } 
                    catch (Exception e)
                    {
                        ModConsole.Error($"Settings [ID: <b>{setting.ID}</b>] Invalid value <b>{setting.Value}</b>{Environment.NewLine}<b>Error details:</b> {e.Message}");
                        System.Console.WriteLine(e); 
                        setting.Value = 0;
                    }
                    slidr.slider.wholeNumbers = (bool)setting.Vals[2];
                    if (setting.Vals[3] != null)
                    {
                        slidr.value.text = ((string[])setting.Vals[3])[int.Parse(setting.Value.ToString())];
                    }
                    slidr.slider.onValueChanged.AddListener(delegate
                    {
                        if (slidr.slider.wholeNumbers)
                            setting.Value = (int)slidr.slider.value;
                        else
                            setting.Value = Math.Round(slidr.slider.value, int.Parse(setting.Vals[4].ToString()));
                        if (setting.Vals[3] == null)
                            slidr.value.text = setting.Value.ToString();
                        else
                        {
                            slidr.value.text = ((string[])setting.Vals[3])[int.Parse(setting.Value.ToString())];
                        }
                        if (setting.DoAction != null)
                            setting.DoAction.Invoke();
                    });
                    slidr.transform.SetParent(listView, false);
                    break;
                case SettingsType.TextBox:
                    GameObject txtP = Instantiate(TextBoxPrefab);
                    SettingsElement txt = txtP.GetComponent<SettingsElement>();

                    setting.NameText = txt.settingName;
                    setting.NameText.text = setting.Name;
                    setting.NameText.color = (Color)setting.Vals[1];
                    txt.placeholder.text = setting.Vals[0].ToString();
                    txt.textBox.contentType = (InputField.ContentType)setting.Vals[2];
                    txt.textBox.text = setting.Value.ToString();
                    txt.textBox.onValueChange.AddListener(delegate
                    {
                        setting.Value = txt.textBox.text;
                    });
                    txt.transform.SetParent(listView, false);
                    break;
                case SettingsType.Header:
                    break;
                case SettingsType.Text:
                    GameObject tx = Instantiate(LabelPrefab);
                    setting.NameText = tx.GetComponent<Text>();
                    setting.NameText.text = setting.Name;
                    tx.transform.SetParent(listView, false);
                    break;
            }
        }
        public void RemoveChildren(Transform parent)
        {
            if (parent.childCount > 0)
            {
                for (int i = 0; i < parent.childCount; i++)
                    Destroy(parent.GetChild(i).gameObject);
            }
        }
    }
}