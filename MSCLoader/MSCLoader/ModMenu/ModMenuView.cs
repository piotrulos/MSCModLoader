using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace MSCLoader;

internal class ModMenuView : MonoBehaviour
{
    public Text ModTab, ReferenceTab, UpdateTab;

    public GameObject ModElementPrefab, ReferenceElementPrefab, UpdateElementPrefab;
    public GameObject HeaderGroupPrefab;
    public GameObject ButtonPrefab, CheckBoxPrefab, KeyBindPrefab, LabelPrefab, SliderPrefab, TextBoxPrefab;
    public GameObject DropDownListPrefab, ColorPickerPrefab;
    public UniversalView universalView;

    public bool modList = false;
    public GameObject modListView;
#if !Mini
    public void RefreshTabs()
    {
        if (ModLoader.InvalidMods.Count > 0)
            ModTab.text = $"Mods (<color=lime>{ModLoader.Instance.actualModList.Length}</color>/<color=magenta>{ModLoader.InvalidMods.Count}</color>)";
        else
            ModTab.text = $"Mods (<color=lime>{ModLoader.Instance.actualModList.Length}</color>)";
        ReferenceTab.text = $"References (<color=aqua>{ModLoader.Instance.ReferencesList.Count}</color>)";
        UpdateTab.text = $"Updates (<color=yellow>{ModLoader.HasUpdateModList.Count + ModLoader.HasUpdateRefList.Count}</color>)";
    }
    public void ModMenuOpened()
    {
        RefreshTabs();
        if (modList) ModList(modListView, string.Empty);
    }
    System.Collections.IEnumerator ModListAsync(GameObject listView, string search)
    {
        Mod[] filteredList = new Mod[0];
        string[] filteredInvalidList = new string[0];
        if (search == string.Empty)
        {
            filteredList = ModLoader.Instance.actualModList;
            filteredInvalidList = ModLoader.InvalidMods.ToArray();
        }
        else
        {
            filteredList = ModLoader.Instance.actualModList.Where(x => x.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || x.ID.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
            filteredInvalidList = ModLoader.InvalidMods.Where(x => x.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
        }
        //  ModConsole.Warning(filteredList.Length.ToString());
        for (int i = 0; i < filteredList.Length; i++)
        {
            GameObject mod = Instantiate(ModElementPrefab);
            mod.GetComponent<MenuElementList>().mod = filteredList[i];
            mod.GetComponent<MenuElementList>().ModInfoFill();
            mod.GetComponent<MenuElementList>().ModButtonsPrep(universalView);
            mod.transform.SetParent(listView.transform, false);
            mod.SetActive(true);
            yield return null;
        }
        for (int i = 0; i < filteredInvalidList.Length; i++)
        {
            GameObject mod = GameObject.Instantiate(ModElementPrefab);
            mod.GetComponent<MenuElementList>().InvalidMod(filteredInvalidList[i]);
            mod.transform.SetParent(listView.transform, false);
            mod.SetActive(true);
            yield return null;
        }
        if (filteredList.Length == 0 && filteredInvalidList.Length == 0)
        {
            SettingsElement tx = CreateText(listView.transform, $"<color=aqua>Nothing found</color>");
            tx.settingName.alignment = TextAnchor.MiddleCenter;
        }
    }

    public void ModList(GameObject listView, string search)
    {
        StopAllCoroutines();
        RemoveChildren(listView.transform);
        if (ModLoader.Instance.actualModList.Length == 0 && search == string.Empty)
        {
            SettingsElement tx = CreateText(listView.transform, $"<color=aqua>A little empty here, seems like there is no mods installed.{Environment.NewLine}If you think that you installed mods, check if you put mods in correct folder.{Environment.NewLine}Current Mod folder is: <color=yellow>{ModLoader.ModsFolder}</color></color>");
            tx.settingName.alignment = TextAnchor.MiddleCenter;
        }
        StartCoroutine(ModListAsync(listView, search));
        return;
    }

    public void UpdateList(GameObject listView)
    {
        RemoveChildren(listView.transform);
        if (ModLoader.HasUpdateModList.Count == 0 && ModLoader.HasUpdateRefList.Count == 0)
        {
            SettingsElement tx = CreateText(listView.transform, $"<color=aqua>Everything seems to be up to date!</color>");
            tx.settingName.alignment = TextAnchor.MiddleCenter;
        }
        for (int i = 0; i < ModLoader.HasUpdateModList.Count; i++)
        {
            GameObject mod = GameObject.Instantiate(UpdateElementPrefab);
            mod.GetComponent<MenuElementList>().mod = ModLoader.HasUpdateModList[i];
            mod.GetComponent<MenuElementList>().UpdateInfoFill();
            mod.transform.SetParent(listView.transform, false);
            mod.SetActive(true);
        }
        for (int i = 0; i < ModLoader.HasUpdateRefList.Count; i++)
        {
            GameObject mod = GameObject.Instantiate(UpdateElementPrefab);
            mod.GetComponent<MenuElementList>().refs = ModLoader.HasUpdateRefList[i];
            mod.GetComponent<MenuElementList>().UpdateInfoFill();
            mod.transform.SetParent(listView.transform, false);
            mod.SetActive(true);
        }
    }
    public void ReferencesList(GameObject listView)
    {
        RemoveChildren(listView.transform);
        if (ModLoader.Instance.ReferencesList.Count == 0)
        {
            SettingsElement tx = CreateText(listView.transform, $"<color=aqua>No additional references are installed.</color>");
            tx.settingName.alignment = TextAnchor.MiddleCenter;
        }
        for (int i = 0; i < ModLoader.Instance.ReferencesList.Count; i++)
        {
            GameObject mod = GameObject.Instantiate(ReferenceElementPrefab);
            mod.GetComponent<MenuElementList>().ReferenceInfoFill(ModLoader.Instance.ReferencesList[i]);
            mod.transform.SetParent(listView.transform, false);
            mod.SetActive(true);
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
        SaveLoad.LoadModsSaveData();
        int savedDataCount = SaveLoad.saveFileData.GetTags().Where(x => x.StartsWith($"{mod.ID}||")).Count();
        listView.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
        //Info Header
        SettingsGroup header = CreateHeader(listView.transform, "Mod Information", Color.cyan);
        SettingsElement tx = CreateText(header.HeaderListView.transform, $"<color=yellow>ID:</color> <color=aqua>{mod.ID}</color> (Compiled using MSCLoader <color=yellow>{mod.compiledVersion}</color>){Environment.NewLine}" +
            $"<color=yellow>Version:</color> <color=aqua>{mod.Version}</color>{Environment.NewLine}" +
            $"<color=yellow>Author:</color> <color=aqua>{mod.Author}</color>{Environment.NewLine}" +
            $"<color=yellow>Additional references used by this Mod:</color>{Environment.NewLine}");
        if (mod.AdditionalReferences != null)
            tx.settingName.text += $"<color=aqua>{string.Join(", ", mod.AdditionalReferences)}</color>";
        else
            tx.settingName.text += $"<color=aqua>[None]</color>";
        if (savedDataCount > 0)
        {
            tx.settingName.text += $"{Environment.NewLine}{Environment.NewLine}<color=yellow>Unified save system: </color><color=aqua>{savedDataCount}</color><color=lime> saved values</color>";
            SettingsElement rbtn = CreateButton(header.HeaderListView.transform, "Reset save file", Color.white, Color.black);
            rbtn.button.onClick.AddListener(delegate
            {
                if (ModLoader.GetCurrentScene() != CurrentScene.MainMenu)
                {
                    ModUI.ShowMessage("You can only use this option in main menu.");
                }
                ModUI.ShowYesNoMessage($"Resetting this mod's save file will reset this mod to default state. {Environment.NewLine}This cannot be undone.{Environment.NewLine}{Environment.NewLine}Are you sure you want to continue?", "Warning", delegate
                {
                    SaveLoad.ResetSaveForMod(mod);
                    MetadataInfoList(listView, mod);
                });
            });
        }
        if (mod.metadata == null)
        {
            SettingsGroup header2 = CreateHeader(listView.transform, "No metadata", Color.yellow);
            CreateText(header2.HeaderListView.transform, $"<color=yellow>This mod doesn't contain additional information</color>");
        }
        else
        {
            SettingsGroup header2 = CreateHeader(listView.transform, "Website Links", Color.yellow);
            if (string.IsNullOrEmpty(mod.metadata.links.nexusLink) && string.IsNullOrEmpty(mod.metadata.links.rdLink) && string.IsNullOrEmpty(mod.metadata.links.githubLink))
            {
                CreateText(header2.HeaderListView.transform, $"<color=yellow>This mod doesn't contain links</color>");
            }
            else
            {
                if (!string.IsNullOrEmpty(mod.metadata.links.nexusLink))
                {
                    SettingsElement nexusBtn = CreateButton(header2.HeaderListView.transform, "SHOW ON <color=orange>NEXUSMODS.COM</color>", Color.white, new Color32(2, 35, 60, 255));
                    nexusBtn.settingName.alignment = TextAnchor.MiddleLeft;
                    nexusBtn.iconElement.texture = nexusBtn.iconPack[1];
                    nexusBtn.iconElement.gameObject.SetActive(true);
                    nexusBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links.nexusLink));
                }
                if (!string.IsNullOrEmpty(mod.metadata.links.rdLink))
                {
                    SettingsElement rdBtn = CreateButton(header2.HeaderListView.transform, "SHOW ON <color=orange>RACEDEPARTMENT.COM</color>", Color.white, new Color32(2, 35, 49, 255));
                    rdBtn.settingName.alignment = TextAnchor.MiddleLeft;
                    rdBtn.iconElement.texture = rdBtn.iconPack[0];
                    rdBtn.iconElement.gameObject.SetActive(true);
                    rdBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links.rdLink));
                }
                if (!string.IsNullOrEmpty(mod.metadata.links.githubLink))
                {
                    SettingsElement ghBtn = CreateButton(header2.HeaderListView.transform, "SHOW ON <color=orange>GITHUB.COM</color>", Color.white, Color.black);
                    ghBtn.settingName.alignment = TextAnchor.MiddleLeft;
                    ghBtn.iconElement.texture = ghBtn.iconPack[2];
                    ghBtn.iconElement.gameObject.SetActive(true);
                    ghBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links.githubLink));
                }
            }
            SettingsGroup header3 = CreateHeader(listView.transform, "Description", Color.yellow);
            CreateText(header3.HeaderListView.transform, mod.metadata.description);
        }
    }
    internal void MetadataUploadForm(GameObject listView, Mod mod)
    {
        RemoveChildren(listView.transform);
        listView.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
        //Info Header
        SettingsGroup header = CreateHeader(listView.transform, "Mod Information", Color.cyan);
        SettingsElement tx = CreateText(header.HeaderListView.transform, $"<color=yellow>ID:</color> <color=aqua>{mod.ID}</color> (Compiled using MSCLoader <color=yellow>{mod.compiledVersion}</color>){Environment.NewLine}" +
            $"<color=yellow>Version:</color> <color=aqua>{mod.Version}</color>{Environment.NewLine}" +
            $"<color=yellow>Author:</color> <color=aqua>{mod.Author}</color>{Environment.NewLine}" +
            $"<color=yellow>Additional references used by this Mod:</color>{Environment.NewLine}");
        if (mod.AdditionalReferences != null)
            tx.settingName.text += $"<color=aqua>{string.Join(", ", mod.AdditionalReferences)}</color>";
        else
            tx.settingName.text += $"<color=aqua>[None]</color>";
        //----------------------------------
        bool assets = false, references = false;
        List<References> refList = new List<References>();
        //----------------------------------
        SettingsGroup header2 = CreateHeader(listView.transform, "Bundle Assets", Color.yellow);
        SettingsElement tx2 = CreateText(header2.HeaderListView.transform, "");
        if (!System.IO.Directory.Exists(System.IO.Path.Combine(ModLoader.AssetsFolder, mod.ID)))
            tx2.settingName.text = $"<color=yellow>Looks like this mod doesn't have assets folder.</color>";
        else
        {
            tx2.settingName.text = $"Select below option if you want to include Assets folder (in most cases you should do it)";
            GameObject checkboxP = Instantiate(CheckBoxPrefab);
            SettingsElement checkbox = checkboxP.GetComponent<SettingsElement>();
            checkbox.settingName.text = "Include Assets Folder";
            checkbox.checkBox.isOn = true;
            assets = true;
            checkbox.checkBox.onValueChanged.AddListener(delegate
            {
                assets = checkbox.checkBox.isOn;
            });
            checkbox.transform.SetParent(header2.HeaderListView.transform, false);
        }
        SettingsGroup header3 = CreateHeader(listView.transform, "Bundle References", Color.yellow);
        SettingsElement tx3 = CreateText(header3.HeaderListView.transform, "");
        if (mod.AdditionalReferences == null)
            tx3.settingName.text = $"<color=yellow>Looks like this mod doesn't use additional references.</color>";
        else
        {
            tx3.settingName.text = $"<color=yellow>You can bundle references{Environment.NewLine}Do it only if reference is exclusive to your mod, otherwise you should create Reference update separately.</color>{Environment.NewLine}";
            foreach (string rf in mod.AdditionalReferences)
            {
                if (rf.StartsWith("MSCLoader"))
                {
                    tx3.settingName.text += $"{Environment.NewLine}<color=red><b>{rf}</b></color> - Cannot be bundled (blacklisted)";
                    continue;
                }
                References refe = ModLoader.Instance.ReferencesList.Where(x => x.AssemblyID == rf).FirstOrDefault();
                if (refe == null)
                {
                    tx3.settingName.text += $"{Environment.NewLine}<color=aqua><b>{rf}</b></color> - Looks like mod, cannot be bundled.";
                    continue;
                }
                else
                {
                    if (refe.UpdateInfo != null)
                    {
                        if (refe.UpdateInfo.ref_type == 1)
                        {
                            tx3.settingName.text += $"{Environment.NewLine}<color=lime><b>{rf}</b></color> - Reference is updated separately";
                            continue;
                        }
                        else
                        {
                            tx3.settingName.text += $"{Environment.NewLine}<color=orange><b>{rf}</b></color> - Registered Reference, update it first";
                            continue;
                        }
                    }
                    else
                    {
                        references = true;
                        GameObject checkboxP3 = Instantiate(CheckBoxPrefab);
                        SettingsElement checkbox3 = checkboxP3.GetComponent<SettingsElement>();
                        checkbox3.settingName.text = rf;
                        checkbox3.checkBox.isOn = false;
                        checkbox3.checkBox.onValueChanged.AddListener(delegate
                        {
                            if (checkbox3.checkBox.isOn)
                            {
                                refList.Add(refe);
                            }
                            else
                            {
                                refList.Remove(refe);
                            }
                        });
                        checkbox3.transform.SetParent(header3.HeaderListView.transform, false);
                    }
                }
            }
        }
        SettingsGroup header4 = CreateHeader(listView.transform, "Upload", Color.yellow);
        CreateText(header4.HeaderListView.transform, "<b><color=aqua>Recomended option!</color></b> Updates mod information to latest version and Uploads it, so it's available as in-game update.");

        SettingsElement uploadBtn = CreateButton(header4.HeaderListView.transform, "Upload and Update Mod", Color.white, Color.black);
        uploadBtn.button.onClick.AddListener(delegate
        {
            ModMetadata.UploadUpdate(mod, assets, references, refList.ToArray());
        });
        CreateText(header4.HeaderListView.transform, $"{Environment.NewLine}This button below updates mod's version only. Use this if you want to update version number only, without uploading update file. (mod will not be available as in-game update)");

        SettingsElement uploadBtn2 = CreateButton(header4.HeaderListView.transform, "Update Mod version only", Color.white, Color.black);
        uploadBtn2.button.onClick.AddListener(delegate
        {
            ModMetadata.UpdateVersionNumber(mod);
        });
    }
    internal static void OpenModLink(string url)
    {
        if (ModMenu.openLinksOverlay.GetValue())
        {
            //try opening in steam overlay
            try
            {
#if !Mini
                Steamworks.SteamFriends.ActivateGameOverlayToWebPage(url);
#endif
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
            SettingsGroup header = CreateHeader(listView.transform, "Incompatible settings", Color.white);
            header.HeaderBackground.color = Color.red;
            CreateText(header.HeaderListView.transform, $"<color=aqua>Incompatible settings format! Settings below may not load or work correctly.</color>{Environment.NewLine}Report that to mod author to use proper settings format.");
        }
        if (Settings.Get(mod)[0].SettingType != SettingsType.Header)
        {
            SettingsGroup header = CreateHeader(listView.transform, "Settings", Color.cyan);
            currentTransform = header.HeaderListView.transform;
        }

        for (int i = 0; i < Settings.Get(mod).Count; i++)
        {
            if (Settings.Get(mod)[i].SettingType == SettingsType.Header)
                currentTransform = SettingsHeader(Settings.Get(mod)[i], listView.transform);
            else
            {
                SettingsList(Settings.Get(mod)[i], currentTransform);
            }
        }
        if (!mod.hideResetAllSettings)
        {
            SettingsElement rbtn = CreateButton(listView.transform, "Reset all settings to default", Color.white, Color.black);
            rbtn.button.onClick.AddListener(delegate
            {
                ModMenu.ResetSettings(mod);
                universalView.FillSettings(mod);
                if (mod.newSettingsFormat)
                {
                    if (mod.A_ModSettingsLoaded != null)
                    {
                        mod.A_ModSettingsLoaded.Invoke();
                    }
                }
                else
                    mod.ModSettingsLoaded();
            });
        }
    }
    public void KeyBindsList(GameObject listView, Mod mod)
    {
        RemoveChildren(listView.transform);
        listView.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
        Transform currentTransform = null;
        //If first settings element is not header, create one.
        if (mod.Keybinds[0].ID != null && mod.Keybinds[0].Vals == null)
        {
            SettingsGroup header = CreateHeader(listView.transform, "Keybinds", Color.yellow);
            currentTransform = header.HeaderListView.transform;
        }
        for (int i = 0; i < mod.Keybinds.Count; i++)
        {
            if (mod.Keybinds[i].ID == null && mod.Keybinds[i].Vals != null)
            {
                SettingsGroup header = CreateHeader(listView.transform, mod.Keybinds[i].Name, (Color)mod.Keybinds[i].Vals[1]);
                header.HeaderBackground.color = (Color)mod.Keybinds[i].Vals[0];
                currentTransform = header.HeaderListView.transform;
            }
            else
            {
                GameObject keyBind = Instantiate(KeyBindPrefab);
                keyBind.GetComponent<KeyBinding>().LoadBind(mod.Keybinds[i], mod);
                keyBind.transform.SetParent(currentTransform, false);
            }
        }
        SettingsElement rbtn = CreateButton(listView.transform, "Reset all Keybinds to default", Color.white, Color.black);
        rbtn.button.onClick.AddListener(delegate
        {
            ModMenu.ResetBinds(mod);
            universalView.FillKeybinds(mod);
        });
    }
    Transform SettingsHeader(Settings setting, Transform listView)
    {
        SettingsGroup header = CreateHeader(listView.transform, setting.Name, (Color)setting.Vals[2]);
        header.HeaderBackground.color = (Color)setting.Vals[1];
        setting.header = header;
        if ((bool)setting.Vals[3]) header.SetHeaderNoAnim(false);
        return header.HeaderListView.transform;
    }

    public void SettingsList(Settings setting, Transform listView)
    {
        switch (setting.SettingType)
        {
            case SettingsType.CheckBox:
                GameObject checkboxP = Instantiate(CheckBoxPrefab);
                SettingsElement checkbox = checkboxP.GetComponent<SettingsElement>();
                setting.SettingsElement = checkbox;
                checkbox.settingName.text = setting.Name;
                try
                {
                    checkbox.checkBox.isOn = bool.Parse(setting.Value.ToString());
                }
                catch (Exception e)
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
                setting.SettingsElement = checkboxG;
                checkboxG.settingName.text = setting.Name;
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
                    if (setting.DoUnityAction != null)
                        setting.DoUnityAction.Invoke();
                });
                checkboxG.transform.SetParent(listView, false);
                break;
            case SettingsType.Button:
                GameObject btnP = Instantiate(ButtonPrefab);
                SettingsElement btn = btnP.GetComponent<SettingsElement>();
                setting.SettingsElement = btn;
                btn.settingName.text = setting.Name.ToUpper();
                btn.settingName.color = (Color)setting.Vals[1];
                btn.button.GetComponent<Image>().color = (Color)setting.Vals[0];
                if (setting.Value.ToString() == "DoUnityAction")
                    btn.button.onClick.AddListener(setting.DoUnityAction.Invoke);
                else
                    btn.button.onClick.AddListener(setting.DoAction.Invoke);
                btn.transform.SetParent(listView, false);
                break;
            case SettingsType.RButton:
                GameObject rbtnP = Instantiate(ButtonPrefab);
                SettingsElement rbtn = rbtnP.GetComponent<SettingsElement>();
                setting.SettingsElement = rbtn;
                rbtn.settingName.text = setting.Name.ToUpper();
                rbtn.settingName.color = Color.white;
                rbtn.button.GetComponent<Image>().color = Color.black;
                rbtn.button.onClick.AddListener(delegate
                {
                    ModMenu.ResetSpecificSettings(setting.Mod, (Settings[])setting.Vals[0]);
                    universalView.FillSettings(setting.Mod);
                    if (setting.Mod.newSettingsFormat)
                    {
                        if (setting.Mod.A_ModSettingsLoaded != null)
                        {
                            setting.Mod.A_ModSettingsLoaded.Invoke();
                        }
                    }
                    else
                        setting.Mod.ModSettingsLoaded();
                });
                rbtn.transform.SetParent(listView, false);
                break;
            case SettingsType.Slider:
                GameObject slidrP = Instantiate(SliderPrefab);
                SettingsElement slidr = slidrP.GetComponent<SettingsElement>();
                setting.SettingsElement = slidr;
                slidr.settingName.text = setting.Name;
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
                    if (setting.DoUnityAction != null)
                        setting.DoUnityAction.Invoke();
                });
                slidr.transform.SetParent(listView, false);
                break;
            case SettingsType.TextBox:
                GameObject txtP = Instantiate(TextBoxPrefab);
                SettingsElement txt = txtP.GetComponent<SettingsElement>();
                setting.SettingsElement = txt;
                txt.settingName.text = setting.Name;
                txt.settingName.color = (Color)setting.Vals[1];
                txt.placeholder.text = setting.Vals[0].ToString();
                txt.textBox.contentType = (InputField.ContentType)setting.Vals[2];
                txt.textBox.text = setting.Value.ToString();
                txt.textBox.onValueChange.AddListener(delegate
                {
                    setting.Value = txt.textBox.text;
                });
                txt.transform.SetParent(listView, false);
                break;
            case SettingsType.DropDown:
                GameObject ddlP = Instantiate(DropDownListPrefab);
                SettingsElement ddl = ddlP.GetComponent<SettingsElement>();
                setting.SettingsElement = ddl;
                ddl.settingName.text = setting.Name;
                ddl.dropDownList.Items = new List<DropDownListItem>();
                int i = 0;
                foreach (string s in (string[])setting.Vals[0])
                {
                    DropDownListItem ddli = new DropDownListItem(s, i.ToString());
                    ddl.dropDownList.Items.Add(ddli);
                    i++;
                }
                ddl.dropDownList.SelectedIndex = int.Parse(setting.Value.ToString());
                ddl.dropDownList.OnSelectionChanged = delegate
                {
                    setting.Value = ddl.dropDownList.SelectedIndex;
                    if (setting.DoAction != null)
                        setting.DoAction.Invoke();
                };
                ddl.transform.SetParent(listView, false);
                break;
            case SettingsType.ColorPicker:
                GameObject colpp = Instantiate(ColorPickerPrefab);
                SettingsElement colp = colpp.GetComponent<SettingsElement>();
                setting.SettingsElement = colp;
                colp.settingName.text = setting.Name;
                string[] colb = setting.Value.ToString().Split(',');
                colp.colorPicker.CurrentColor = new Color32(byte.Parse(colb[0]), byte.Parse(colb[1]), byte.Parse(colb[2]), byte.Parse(colb[3]));
                if ((bool)setting.Vals[0])
                {
                    colp.colorPicker.AlphaSlider.SetActive(true);
                }
                colp.colorPicker.onValueChanged.AddListener((Color32 col) =>
                {
                    setting.Value = $"{col.r},{col.g},{col.b},{col.a}";
                    if (setting.DoAction != null)
                        setting.DoAction.Invoke();
                });
                colp.transform.SetParent(listView, false);
                break;
            case SettingsType.Text:
                GameObject tx = Instantiate(LabelPrefab);
                SettingsElement label = tx.GetComponent<SettingsElement>();
                setting.SettingsElement = label;
                label.settingName.text = setting.Name;
                tx.transform.SetParent(listView, false);
                break;
            case SettingsType.Header:
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

    public SettingsGroup CreateHeader(Transform listView, string title, Color textColor)
    {
        GameObject hdr = GameObject.Instantiate(HeaderGroupPrefab);
        SettingsGroup header = hdr.GetComponent<SettingsGroup>();
        header.HeaderTitle.text = title.ToUpper();
        header.HeaderTitle.color = textColor;
        hdr.transform.SetParent(listView, false);
        return header;
    }
    public SettingsElement CreateButton(Transform listView, string text, Color textColor, Color btnColor)
    {
        GameObject btnP = Instantiate(ButtonPrefab);
        SettingsElement btn = btnP.GetComponent<SettingsElement>();
        btn.settingName.text = text.ToUpper();
        btn.settingName.color = textColor;
        btn.button.GetComponent<Image>().color = btnColor;
        btn.transform.SetParent(listView.transform, false);
        return btn;
    }
    public SettingsElement CreateText(Transform listView, string text)
    {
        GameObject tx = Instantiate(LabelPrefab);
        SettingsElement txt = tx.GetComponent<SettingsElement>();
        txt.settingName.text = text;
        tx.transform.SetParent(listView.transform, false);
        return txt;
    }
#endif
}
