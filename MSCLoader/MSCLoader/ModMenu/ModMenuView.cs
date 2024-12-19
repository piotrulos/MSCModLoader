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
        InvalidMods[] filteredInvalidList = new InvalidMods[0];
        if (search == string.Empty)
        {
            filteredList = ModLoader.Instance.actualModList;
            filteredInvalidList = ModLoader.InvalidMods.ToArray();
        }
        else
        {
            filteredList = ModLoader.Instance.actualModList.Where(x => x.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || x.ID.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
            filteredInvalidList = ModLoader.InvalidMods.Where(x => x.FileName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
        }

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
            SettingsElement tx = CreateText(listView.transform, $"<color=aqua>A little empty here, seems like there are no mods installed.{Environment.NewLine}If you think that you installed mods, check if you put mods in correct folder.{Environment.NewLine}Current Mod folder is: <color=yellow>{ModLoader.ModsFolder}</color></color>");
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
        for (int i = 0; i < Settings.GetModSettings(ModLoader.LoadedMods[0]).Count; i++)
        {
            if (Settings.GetModSettings(ModLoader.LoadedMods[0])[i].SettingType == SettingsType.Header)
                currentTransform = SettingsHeader(Settings.GetModSettings(ModLoader.LoadedMods[0])[i], listView.transform);
            else
                SettingsList(Settings.GetModSettings(ModLoader.LoadedMods[0])[i], currentTransform);
        }
        GameObject keyBind = Instantiate(KeyBindPrefab);
        keyBind.GetComponent<KeyBinding>().LoadBind(ModLoader.LoadedMods[0].Keybinds[0], ModLoader.LoadedMods[0]);
        keyBind.transform.SetParent(currentTransform, false);
        for (int i = 0; i < Settings.GetModSettings(ModLoader.LoadedMods[1]).Count; i++)
        {
            if (Settings.GetModSettings(ModLoader.LoadedMods[1])[i].SettingType == SettingsType.Header)
                currentTransform = SettingsHeader(Settings.GetModSettings(ModLoader.LoadedMods[1])[i], listView.transform);
            else
                SettingsList(Settings.GetModSettings(ModLoader.LoadedMods[1])[i], currentTransform);
        }
        ModConsole.RefreshMainSettingsData();
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
            if (string.IsNullOrEmpty(mod.metadata.links[0]) && string.IsNullOrEmpty(mod.metadata.links[1]) && string.IsNullOrEmpty(mod.metadata.links[2]))
            {
                CreateText(header2.HeaderListView.transform, $"<color=yellow>This mod doesn't contain links</color>");
            }
            else
            {
                if (!string.IsNullOrEmpty(mod.metadata.links[0]))
                {
                    SettingsElement nexusBtn = CreateButton(header2.HeaderListView.transform, "SHOW ON <color=orange>NEXUSMODS.COM</color>", Color.white, new Color32(2, 35, 60, 255));
                    nexusBtn.settingName.alignment = TextAnchor.MiddleLeft;
                    nexusBtn.iconElement.texture = nexusBtn.iconPack[1];
                    nexusBtn.iconElement.gameObject.SetActive(true);
                    nexusBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links[0]));
                }
                if (!string.IsNullOrEmpty(mod.metadata.links[1]))
                {
                    SettingsElement rdBtn = CreateButton(header2.HeaderListView.transform, "SHOW ON <color=orange>RACEDEPARTMENT.COM</color>", Color.white, new Color32(2, 35, 49, 255));
                    rdBtn.settingName.alignment = TextAnchor.MiddleLeft;
                    rdBtn.iconElement.texture = rdBtn.iconPack[0];
                    rdBtn.iconElement.gameObject.SetActive(true);
                    rdBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links[1]));
                }
                if (!string.IsNullOrEmpty(mod.metadata.links[2]))
                {
                    SettingsElement ghBtn = CreateButton(header2.HeaderListView.transform, "SHOW ON <color=orange>GITHUB.COM</color>", Color.white, Color.black);
                    ghBtn.settingName.alignment = TextAnchor.MiddleLeft;
                    ghBtn.iconElement.texture = ghBtn.iconPack[2];
                    ghBtn.iconElement.gameObject.SetActive(true);
                    ghBtn.button.onClick.AddListener(() => OpenModLink(mod.metadata.links[2]));
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
        uploadBtn.settingName.alignment = TextAnchor.MiddleLeft;
        uploadBtn.iconElement.texture = uploadBtn.iconPack[(int)SettingsButton.ButtonIcon.CloudArrow];
        uploadBtn.iconElement.gameObject.SetActive(true);
        CreateText(header4.HeaderListView.transform, $"{Environment.NewLine}This button below updates mod's version only. Use this if you want to update version number only, without uploading update file. (mod will not be available as in-game update)");

        SettingsElement uploadBtn2 = CreateButton(header4.HeaderListView.transform, "Update Mod version only", Color.white, Color.black);
        uploadBtn2.button.onClick.AddListener(delegate
        {
            ModMetadata.UpdateModVersionNumber(mod);
        });
        uploadBtn2.settingName.alignment = TextAnchor.MiddleLeft;
        uploadBtn2.iconElement.texture = uploadBtn2.iconPack[(int)SettingsButton.ButtonIcon.Update];
        uploadBtn2.iconElement.gameObject.SetActive(true);
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

        if (Settings.GetModSettings(mod)[0].SettingType != SettingsType.Header)
        {
            SettingsGroup header = CreateHeader(listView.transform, "Settings", Color.cyan);
            currentTransform = header.HeaderListView.transform;
        }

        for (int i = 0; i < Settings.GetModSettings(mod).Count; i++)
        {
            if (Settings.GetModSettings(mod)[i].SettingType == SettingsType.Header)
                currentTransform = SettingsHeader(Settings.GetModSettings(mod)[i], listView.transform);
            else
            {
                SettingsList(Settings.GetModSettings(mod)[i], currentTransform);
            }
        }

        if (!mod.hideResetAllSettings)
        {
            SettingsElement rbtn = CreateButton(listView.transform, "Reset all settings to default", Color.white, Color.black);
            rbtn.settingName.alignment = TextAnchor.MiddleLeft;
            rbtn.iconElement.texture = rbtn.iconPack[(int)SettingsButton.ButtonIcon.Reset];
            rbtn.iconElement.gameObject.SetActive(true);
            rbtn.button.onClick.AddListener(delegate
            {
                ModMenu.ResetSettings(mod);
                universalView.FillSettings(mod);
                if (mod.newSettingsFormat)
                {
                    if (mod.A_ModSettingsLoaded != null)
                    {
                        Console.WriteLine($"Calling ModSettingsLoaded (for mod {mod.ID})");
                        mod.A_ModSettingsLoaded.Invoke();
                    }
                }
                else
                {
                    Console.WriteLine($"Calling ModSettingsLoaded [old format] (for mod {mod.ID})");
                    mod.ModSettingsLoaded();
                }
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
    private Transform SettingsHeader(ModSetting set, Transform listView)
    {
        SettingsHeader setting = (SettingsHeader)set;
        SettingsGroup header = CreateHeader(listView.transform, setting.Name, setting.TextColor);
        header.HeaderBackground.color = setting.BackgroundColor;
        setting.HeaderElement = header;
        if (setting.CollapsedByDefault) header.SetHeaderNoAnim(false);
        header.gameObject.SetActive(setting.IsVisible);
        return header.HeaderListView.transform;
    }

    internal void SettingsList(ModSetting set, Transform listView)
    {
        switch (set.SettingType)
        {
            case SettingsType.CheckBox:
                SettingsCheckBox settingCheckBox = (SettingsCheckBox)set;
                GameObject checkboxP = Instantiate(CheckBoxPrefab);
                SettingsElement checkbox = checkboxP.GetComponent<SettingsElement>();
                settingCheckBox.SettingsElement = checkbox;
                checkbox.SetupCheckbox(settingCheckBox.Name, settingCheckBox.Value, null);
                checkbox.checkBox.onValueChanged.AddListener(delegate
                {
                    settingCheckBox.Value = checkbox.checkBox.isOn;
                    if (settingCheckBox.DoAction != null)
                        settingCheckBox.DoAction.Invoke();
                });
                checkbox.transform.SetParent(listView, false);
                checkbox.gameObject.SetActive(settingCheckBox.IsVisible);
                break;
            case SettingsType.CheckBoxGroup:
                SettingsCheckBoxGroup settingsCheckBoxGroup = (SettingsCheckBoxGroup)set;
                GameObject group;
                if (listView.FindChild(settingsCheckBoxGroup.CheckBoxGroup) == null)
                {
                    group = new GameObject(settingsCheckBoxGroup.CheckBoxGroup);
                    group.AddComponent<ToggleGroup>();
                    group.transform.SetParent(listView, false);
                }
                else
                    group = listView.FindChild(settingsCheckBoxGroup.CheckBoxGroup).gameObject;
                GameObject checkboxGP = Instantiate(CheckBoxPrefab);
                SettingsElement checkboxG = checkboxGP.GetComponent<SettingsElement>();
                settingsCheckBoxGroup.SettingsElement = checkboxG;
                checkboxG.SetupCheckbox(settingsCheckBoxGroup.Name, settingsCheckBoxGroup.Value, group.GetComponent<ToggleGroup>());

                if (settingsCheckBoxGroup.Value)
                    checkboxG.checkBox.group.NotifyToggleOn(checkboxG.checkBox);
                checkboxG.checkBox.onValueChanged.AddListener(delegate
                {
                    settingsCheckBoxGroup.Value = checkboxG.checkBox.isOn;
                    if (settingsCheckBoxGroup.DoAction != null)
                        settingsCheckBoxGroup.DoAction.Invoke();
                });
                checkboxG.transform.SetParent(listView, false);
                checkboxG.gameObject.SetActive(settingsCheckBoxGroup.IsVisible);
                break;
            case SettingsType.Button:
                SettingsButton settingBtn = (SettingsButton)set;
                GameObject btnP = Instantiate(ButtonPrefab);
                SettingsElement btn = btnP.GetComponent<SettingsElement>();
                settingBtn.SettingsElement = btn;
                btn.SetupButton(settingBtn.Name.ToUpper(), settingBtn.TextColor, settingBtn.BackgroundColor);
                if (settingBtn.PredefinedIcon != SettingsButton.ButtonIcon.None)
                {
                    btn.settingName.alignment = TextAnchor.MiddleLeft;
                    if (settingBtn.PredefinedIcon == SettingsButton.ButtonIcon.Custom)
                    {
                        if (settingBtn.CustomIcon == null)
                        {
                            ModConsole.Error($"Custom icon for Button {settingBtn.Name} is null.");
                        }
                        btn.iconElement.texture = settingBtn.CustomIcon;
                    }
                    else
                    {
                        btn.iconElement.texture = btn.iconPack[(int)settingBtn.PredefinedIcon];
                    }
                    btn.iconElement.gameObject.SetActive(true);
                }
                btn.button.onClick.AddListener(settingBtn.DoAction.Invoke);
                btn.transform.SetParent(listView, false);
                btn.gameObject.SetActive(settingBtn.IsVisible);
                break;
            case SettingsType.RButton:
                SettingsResetButton settingRes = (SettingsResetButton)set;
                GameObject rbtnP = Instantiate(ButtonPrefab);
                SettingsElement rbtn = rbtnP.GetComponent<SettingsElement>();
                settingRes.SettingsElement = rbtn;
                rbtn.SetupButton(settingRes.Name.ToUpper(), Color.white, Color.black);
                rbtn.button.onClick.AddListener(delegate
                {
                    settingRes.ResetSettings();
                    universalView.FillSettings(settingRes.ThisMod);
                    if (settingRes.ThisMod.newSettingsFormat)
                    {
                        if (settingRes.ThisMod.A_ModSettingsLoaded != null)
                        {
                            settingRes.ThisMod.A_ModSettingsLoaded.Invoke();
                        }
                    }
                    else
                        settingRes.ThisMod.ModSettingsLoaded();
                });
                rbtn.transform.SetParent(listView, false);
                rbtn.gameObject.SetActive(settingRes.IsVisible);
                break;
            case SettingsType.SliderInt:
                SettingsSliderInt settingSliderInt = (SettingsSliderInt)set;
                GameObject slidrIntP = Instantiate(SliderPrefab);
                SettingsElement slidrInt = slidrIntP.GetComponent<SettingsElement>();
                settingSliderInt.SettingsElement = slidrInt;
                slidrInt.SetupSliderInt(settingSliderInt.Name, settingSliderInt.Value, settingSliderInt.MinValue, settingSliderInt.MaxValue, settingSliderInt.TextValues);
                slidrInt.slider.onValueChanged.AddListener(delegate
                {
                    settingSliderInt.SetValue((int)slidrInt.slider.value);
                    if (settingSliderInt.TextValues != null)
                    {
                        slidrInt.value.text = settingSliderInt.TextValues[settingSliderInt.Value];
                    }
                    if (settingSliderInt.DoAction != null)
                        settingSliderInt.DoAction.Invoke();
                });
                slidrInt.transform.SetParent(listView, false);
                slidrInt.gameObject.SetActive(settingSliderInt.IsVisible);
                break;
            case SettingsType.Slider:
                SettingsSlider settingSlider = (SettingsSlider)set;
                GameObject slidrP = Instantiate(SliderPrefab);
                SettingsElement slidr = slidrP.GetComponent<SettingsElement>();
                settingSlider.SettingsElement = slidr;
                slidr.SetupSlider(settingSlider.Name, settingSlider.Value, settingSlider.MinValue, settingSlider.MaxValue);
                slidr.slider.onValueChanged.AddListener(delegate
                {
                    settingSlider.SetValue((float)Math.Round(slidr.slider.value, settingSlider.DecimalPoints));
                    if (settingSlider.DoAction != null)
                        settingSlider.DoAction.Invoke();
                });
                slidr.transform.SetParent(listView, false);
                slidr.gameObject.SetActive(settingSlider.IsVisible);
                break;
            case SettingsType.TextBox:
                SettingsTextBox settingTxtBox = (SettingsTextBox)set;
                GameObject txtP = Instantiate(TextBoxPrefab);
                SettingsElement txt = txtP.GetComponent<SettingsElement>();
                settingTxtBox.SettingsElement = txt;
                txt.SetupTextBox(settingTxtBox.Name, settingTxtBox.Value, settingTxtBox.Placeholder, settingTxtBox.ContentType);
                txt.textBox.onValueChange.AddListener(delegate
                {
                    settingTxtBox.Value = txt.textBox.text;
                });
                txt.transform.SetParent(listView, false);
                txt.gameObject.SetActive(settingTxtBox.IsVisible);
                break;
            case SettingsType.DropDown:
                SettingsDropDownList settingDropDown = (SettingsDropDownList)set;
                GameObject ddlP = Instantiate(DropDownListPrefab);
                SettingsElement ddl = ddlP.GetComponent<SettingsElement>();
                settingDropDown.SettingsElement = ddl;
                ddl.settingName.text = settingDropDown.Name;
                ddl.dropDownList.Items = new List<DropDownListItem>();
                for (int i = 0; i < settingDropDown.ArrayOfItems.Length; i++)
                {
                    string s = settingDropDown.ArrayOfItems[i];
                    DropDownListItem ddli = new DropDownListItem(s, i.ToString());
                    ddl.dropDownList.Items.Add(ddli);
                }
                ddl.dropDownList.SelectedIndex = settingDropDown.Value;
                ddl.dropDownList.OnSelectionChanged = delegate
                {
                    settingDropDown.Value = ddl.dropDownList.SelectedIndex;
                    if (settingDropDown.DoAction != null)
                        settingDropDown.DoAction.Invoke();
                };
                ddl.transform.SetParent(listView, false);
                ddl.gameObject.SetActive(settingDropDown.IsVisible);
                break;
            case SettingsType.ColorPicker:
                SettingsColorPicker settingColorPicker = (SettingsColorPicker)set;
                GameObject colpp = Instantiate(ColorPickerPrefab);
                SettingsElement colp = colpp.GetComponent<SettingsElement>();
                settingColorPicker.SettingsElement = colp;
                colp.settingName.text = settingColorPicker.Name;
                colp.colorPicker.CurrentColor = settingColorPicker.GetValue();
                if (settingColorPicker.ShowAlpha)
                {
                    colp.colorPicker.AlphaSlider.SetActive(true);
                }
                colp.colorPicker.onValueChanged.AddListener((Color32 col) =>
                {
                    settingColorPicker.SetValue(col);
                    if (settingColorPicker.DoAction != null)
                        settingColorPicker.DoAction.Invoke();
                });
                colp.transform.SetParent(listView, false);
                colp.gameObject.SetActive(settingColorPicker.IsVisible);
                break;
            case SettingsType.Text:
                SettingsText settingText = (SettingsText)set;
                GameObject tx = Instantiate(LabelPrefab);
                SettingsElement label = tx.GetComponent<SettingsElement>();
                settingText.SettingsElement = label;
                label.settingName.text = settingText.Name;
                tx.transform.SetParent(listView, false);
                label.gameObject.SetActive(settingText.IsVisible);
                break;
            default:
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
