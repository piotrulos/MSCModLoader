#if !Mini
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MSCLoader;

internal class ModMenu : Mod
{
    public override string ID => "MSCLoader_Settings";
    public override string Name => "[INTERNAL] Mod Menu";
    public override string Version => ModLoader.MSCLoader_Ver;
    public override string Author => "MSCLoader";

    internal GameObject UI;
    internal static byte cfmu_set = 0;
    internal static ModMenu instance;
    internal static SettingsCheckBox dm_logST, dm_operr, dm_warn, dm_pcon;
    internal static SettingsCheckBox expWarning, modPath, forceMenuVsync, openLinksOverlay, skipGameIntro, skipConfigScreen, filterAdPopups;

    private static SettingsCheckBoxGroup checkLaunch, checkDaily, checkWeekly;
    private System.Diagnostics.FileVersionInfo coreVer, doorstopVer;

    public override void ModSetup()
    {
        instance = this;
        SetupFunction(Setup.OnMenuLoad, Mod_OnMenuLoad);
        SetupFunction(Setup.ModSettings, Mod_Settings);
        SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoaded);
    }

    private void Mod_Settings()
    {
        Settings.ModSettings(this);
        if (ModLoader.devMode)
        {
            Settings.AddHeader("DevMode Settings", new Color32(0, 0, 128, 255), Color.green);
            dm_logST = Settings.AddCheckBox("MSCLoader_dm_logST", "Log-all stack trace (not recommended)", false);
            dm_operr = Settings.AddCheckBox("MSCLoader_dm_operr", "Log-all open console on error", false);
            dm_warn = Settings.AddCheckBox("MSCLoader_dm_warn", "Log-all open console on warning", false);
            dm_pcon = Settings.AddCheckBox("MSCLoader_dm_pcon", "Persistent console (sometimes may break font color)", false);
        }
        Settings.AddHeader("Basic Settings");
        expWarning = Settings.AddCheckBox("MSCLoader_expWarning", "Show experimental warning (experimental branch on Steam)", true);
        modPath = Settings.AddCheckBox("MSCLoader_modPath", "Show mods folder (top left corner)", true, ModLoader.MainMenuPath);
        forceMenuVsync = Settings.AddCheckBox("MSCLoader_forceMenuVsync", "60 FPS limit in Main Menu", true, VSyncSwitchCheckbox);
        openLinksOverlay = Settings.AddCheckBox("MSCLoader_openLinksOverlay", "Open URLs in Steam overlay", true);
        filterAdPopups = Settings.AddCheckBox("MSCLoader_filterMoneyPopups", "Filter out money begging popups or ads", false);

        Settings.AddText("Skip stuff");
        skipGameIntro = Settings.AddCheckBox("MSCLoader_skipGameIntro", "Skip game splash screen", false, SkipIntroSet);
        skipConfigScreen = Settings.AddCheckBox("MSCLoader_skipConfigScreen", "Skip configuration screen", false, SkipConfigScreen);

        Settings.AddHeader("Update Settings");
        Settings.AddText("How often MSCLoader checks for Mod/References updates.");
        checkLaunch = Settings.AddCheckBoxGroup("MSCLoader_checkOnLaunch", "Every launch", true, "cfmu_set");
        checkDaily = Settings.AddCheckBoxGroup("MSCLoader_checkEveryDay", "Daily", false, "cfmu_set");
        checkWeekly = Settings.AddCheckBoxGroup("MSCLoader_checkEveryWeek", "Weekly", false, "cfmu_set");

        Settings.AddHeader("MSCLoader Credits", Color.black);
        Settings.AddText("All source code contributors and used libraries are listed on GitHub");
        Settings.AddButton("Support on <color=orange>ko-fi.com</color>", () => System.Diagnostics.Process.Start("https://ko-fi.com/piotrulos44779"), new Color32(0, 64, 128, 255), Color.white, SettingsButton.ButtonIcon.KoFi);
        Settings.AddText("Non-GitHub contributions:");
        Settings.AddText("<color=aqua>BrennFuchS</color> - Default mod icon.");

        Settings.AddHeader("Detailed Version Information", new Color32(0, 128, 0, 255));
        coreVer = System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(ModLoader.ManagedPath, "MSCLoader.Preloader.dll"));
        doorstopVer = System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine("", "winhttp.dll"));
        SettingsText modulesVer = Settings.AddText($"MSCLoader modules:{Environment.NewLine}<color=yellow>Doorstop</color>: <color=aqua>v{doorstopVer.ProductVersion}</color>{Environment.NewLine}<color=yellow>MSCLoader.Preloader</color>: <color=aqua>v{coreVer.FileMajorPart}.{coreVer.FileMinorPart}.{coreVer.FileBuildPart} build {coreVer.FilePrivatePart}</color>{Environment.NewLine}<color=yellow>MSCLoader</color>: <color=aqua>v{ModLoader.MSCLoader_Ver} build {ModLoader.Instance.currentBuild}</color>");
        if (File.Exists(Path.Combine(ModLoader.ModsFolder, Path.Combine("References", "MSCCoreLibrary.dll"))))
        {
            System.Diagnostics.FileVersionInfo libVer = System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(ModLoader.ModsFolder, Path.Combine("References", "MSCCoreLibrary.dll")));
            modulesVer.SetValue(modulesVer.GetValue() + $"{Environment.NewLine}<color=yellow>MSCCoreLibrary</color>: <color=aqua>v{libVer.FileMajorPart}.{libVer.FileMinorPart}.{libVer.FileBuildPart} build {libVer.FilePrivatePart}</color>");
        }
        try
        {
            Settings.AddText($"Build-in libraries:{Environment.NewLine}<color=yellow>Harmony</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(ModLoader.ManagedPath, "0Harmony.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>Ionic.Zip</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(ModLoader.ManagedPath, "Ionic.Zip.Reduced.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>NAudio</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(ModLoader.ManagedPath, "NAudio.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>NAudio (Vorbis)</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(ModLoader.ManagedPath, "NVorbis.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>NAudio (Flac)</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(ModLoader.ManagedPath, "NAudio.Flac.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>Newtonsoft.Json</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(ModLoader.ManagedPath, "Newtonsoft.Json.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>INIFileParser</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(ModLoader.ManagedPath, "INIFileParser.dll")).FileVersion}</color>");
        }
        catch (Exception e) { Console.WriteLine(e); }
    }

    private void Mod_SettingsLoaded()
    {
        IniData ini = new FileIniDataParser().ReadFile("doorstop_config.ini");
        string skipIntro = ini["MSCLoader"]["skipIntro"];
        string skipCfg = ini["MSCLoader"]["skipConfigScreen"];
        bool introSkip, configSkip;
        if (!bool.TryParse(skipIntro, out introSkip))
        {
            skipGameIntro.SetValue(false);
            Console.WriteLine($"skipIntro - Excepted boolean, received '{skipIntro ?? "<null>"}'.");
        }
        else
        {
            skipGameIntro.SetValue(introSkip);
        }
        if (!bool.TryParse(skipCfg, out configSkip))
        {
            skipConfigScreen.SetValue(false);
            Console.WriteLine($"skipConfigScreen - Excepted boolean, received '{skipCfg ?? "<null>"}'.");
        }
        else
        {
            skipConfigScreen.SetValue(configSkip);
        }
        if (checkLaunch.GetValue())
            cfmu_set = 0;
        else if (checkDaily.GetValue())
            cfmu_set = 1;
        else if (checkWeekly.GetValue())
            cfmu_set = 7;
    }

    private void SkipIntroSet()
    {
        FileIniDataParser parser = new FileIniDataParser();
        IniData ini = parser.ReadFile("doorstop_config.ini");
        ini["MSCLoader"]["skipIntro"] = skipGameIntro.GetValue().ToString().ToLower();
        parser.WriteFile("doorstop_config.ini", ini, System.Text.Encoding.ASCII);
    }

    private void SkipConfigScreen()
    {
        if (coreVer.FilePrivatePart < 263)
        {
            ModUI.ShowMessage("To use <color=yellow>Skip Configuration Screen</color> you need to update the core module of MSCLoader, download latest version and launch <color=aqua>MSCLInstaller.exe</color> to update", "Outdated module");
            return;
        }
        FileIniDataParser parser = new FileIniDataParser();
        IniData ini = parser.ReadFile("doorstop_config.ini");
        ini["MSCLoader"]["skipConfigScreen"] = skipConfigScreen.GetValue().ToString().ToLower();
        parser.WriteFile("doorstop_config.ini", ini, System.Text.Encoding.ASCII);
    }
    private static void VSyncSwitchCheckbox()
    {
        if (ModLoader.GetCurrentScene() == CurrentScene.MainMenu)
        {
            if (forceMenuVsync.GetValue())
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;
        }
    }

    void Mod_OnMenuLoad()
    {
        try
        {
            CreateSettingsUI();
        }
        catch (Exception e)
        {
            ModUI.ShowMessage($"Fatal error:{Environment.NewLine}<color=orange>{e.Message}</color>{Environment.NewLine}Please reinstall MSCLoader to fix this issue", "Fatal Error");
        }
    }

    public void CreateSettingsUI()
    {
        AssetBundle ab = LoadAssets.LoadBundle(this, "settingsui.unity3d");
        GameObject UIp = ab.LoadAsset<GameObject>("MSCLoader Canvas menu.prefab");
        TextAsset assetver = ab.LoadAsset<TextAsset>("version.txt");
        if (assetver == null || !MSCLInfo.BuildType.StartsWith(assetver.text.Split('|')[0]) || !MSCLInfo.menuAssetVersion.Equals(assetver.text.Split('|')[1]))
        {
            throw new Exception($"Invalid MSCLoader asset file version (<color=aqua>settingsui.unity3d</color>), do not replace random MSCLoader files with other versions.{Environment.NewLine}");
        }
        UI = GameObject.Instantiate(UIp);
        GameObject.DontDestroyOnLoad(UI);
        GameObject.Destroy(UIp);
        ab.Unload(false);
        ModUI.popupSettingController = UI.GetComponent<PopupSettingController>();
    }

    // Reset keybinds
    public static void ResetBinds(Mod mod)
    {
        if (mod != null)
        {
            // Revert binds
            for (int i = 0; i < mod.modKeybindsList.Count; i++)
            {
                if (mod.modKeybindsList[i].IsHeader) continue;
                SettingsKeybind skb = (SettingsKeybind)mod.modKeybindsList[i];
                skb.ResetToDefault();
            }

            // Save binds
            SaveModBinds(mod);
        }
    }


    // Save all keybinds to config files.
    public static void SaveAllBinds()
    {
        for (int i = 0; i < ModLoader.LoadedMods.Count; i++)
        {
            SaveModBinds(ModLoader.LoadedMods[i]);
        }
    }


    // Save keybind for a single mod to config file.
    public static void SaveModBinds(Mod mod)
    {

        KeybindList list = new KeybindList();
        string path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "keybinds.json");
        for (int i = 0; i < mod.modKeybindsList.Count; i++)
        {
            if (mod.modKeybindsList[i].IsHeader) continue;
            SettingsKeybind skb = (SettingsKeybind)mod.modKeybindsList[i];
            Keybinds keybinds = new Keybinds
            {
                ID = skb.ID,
                Key = skb.KeybKey,
                Modifier = skb.KeybModif
            };

            list.keybinds.Add(keybinds);
        }
        string serializedData = JsonConvert.SerializeObject(list, Formatting.Indented);
        File.WriteAllText(path, serializedData);
    }

    // Reset settings
    public static void ResetSettings(Mod mod)
    {
        if (mod == null) return;
        for (int i = 0; i < Settings.GetModSettings(mod).Count; i++)
        {
            ResetSpecificSetting(Settings.GetModSettings(mod)[i]);
        }
        SaveSettings(mod);
    }
    internal static void ResetSpecificSetting(ModSetting set)
    {
        switch (set.SettingType)
        {
            case SettingsType.CheckBoxGroup:
                SettingsCheckBoxGroup scbg = (SettingsCheckBoxGroup)set;
                scbg.Value = scbg.DefaultValue;
                scbg.IsVisible = scbg.DefaultVisibility;
                break;
            case SettingsType.CheckBox:
                SettingsCheckBox scb = (SettingsCheckBox)set;
                scb.Value = scb.DefaultValue;
                scb.IsVisible = scb.DefaultVisibility;
                break;
            case SettingsType.Slider:
                SettingsSlider ss = (SettingsSlider)set;
                ss.Value = ss.DefaultValue;
                ss.IsVisible = ss.DefaultVisibility;
                break;
            case SettingsType.SliderInt:
                SettingsSliderInt ssi = (SettingsSliderInt)set;
                ssi.Value = ssi.DefaultValue;
                ssi.IsVisible = ssi.DefaultVisibility;
                break;
            case SettingsType.TextBox:
                SettingsTextBox stb = (SettingsTextBox)set;
                stb.Value = stb.DefaultValue;
                stb.IsVisible = stb.DefaultVisibility;
                break;
            case SettingsType.DropDown:
                SettingsDropDownList sddl = (SettingsDropDownList)set;
                sddl.Value = sddl.DefaultValue;
                sddl.IsVisible = sddl.DefaultVisibility;
                break;
            case SettingsType.ColorPicker:
                SettingsColorPicker scp = (SettingsColorPicker)set;
                scp.Value = scp.DefaultColorValue;
                scp.IsVisible = scp.DefaultVisibility;
                break;
            default:
                break;
        }
    }

    // Save settings for a single mod to config file.
    internal static void SaveSettings(Mod mod)
    {
        SettingsList list = new SettingsList();
        list.isDisabled = mod.isDisabled;
        string path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "settings.json");

        for (int i = 0; i < Settings.GetModSettings(mod).Count; i++)
        {
            switch (Settings.GetModSettings(mod)[i].SettingType)
            {
                case SettingsType.CheckBoxGroup:
                    SettingsCheckBoxGroup group = (SettingsCheckBoxGroup)Settings.GetModSettings(mod)[i];
                    list.settings.Add(new Setting(group.ID, group.Value));
                    break;
                case SettingsType.CheckBox:
                    SettingsCheckBox check = (SettingsCheckBox)Settings.GetModSettings(mod)[i];
                    list.settings.Add(new Setting(check.ID, check.Value));
                    break;
                case SettingsType.Slider:
                    SettingsSlider slider = (SettingsSlider)Settings.GetModSettings(mod)[i];
                    list.settings.Add(new Setting(slider.ID, slider.Value));
                    break;
                case SettingsType.SliderInt:
                    SettingsSliderInt sliderInt = (SettingsSliderInt)Settings.GetModSettings(mod)[i];
                    list.settings.Add(new Setting(sliderInt.ID, sliderInt.Value));
                    break;
                case SettingsType.TextBox:
                    SettingsTextBox textBox = (SettingsTextBox)Settings.GetModSettings(mod)[i];
                    list.settings.Add(new Setting(textBox.ID, textBox.Value));
                    break;
                case SettingsType.DropDown:
                    SettingsDropDownList dropDown = (SettingsDropDownList)Settings.GetModSettings(mod)[i];
                    list.settings.Add(new Setting(dropDown.ID, dropDown.Value));
                    break;
                case SettingsType.ColorPicker:
                    SettingsColorPicker colorPicker = (SettingsColorPicker)Settings.GetModSettings(mod)[i];
                    list.settings.Add(new Setting(colorPicker.ID, colorPicker.Value));
                    break;
                default:
                    break;
            }
        }

        string serializedData = JsonConvert.SerializeObject(list, Formatting.Indented);
        File.WriteAllText(path, serializedData);

    }

    // Load all keybinds.
    public static void LoadBinds()
    {
        for (int i = 0; i < ModLoader.LoadedMods.Count; i++)
        {
            if (ModLoader.LoadedMods[i].modKeybindsList.Count == 0) continue;
            List<ModKeybind> binds = ModLoader.LoadedMods[i].modKeybindsList;
            string path = Path.Combine(ModLoader.GetModSettingsFolder(ModLoader.LoadedMods[i]), "keybinds.json");
            if (!File.Exists(path))
            {
                SaveModBinds(ModLoader.LoadedMods[i]);
                continue;
            }
            //Load and deserialize 
            KeybindList keybinds = JsonConvert.DeserializeObject<KeybindList>(File.ReadAllText(path));
            if (keybinds.keybinds.Count == 0)
                continue;
            for (int k = 0; k < keybinds.keybinds.Count; k++)
            {
                SettingsKeybind bind = (SettingsKeybind)binds.Find(x => x.ID == keybinds.keybinds[k].ID);
                if (bind == null)
                    continue;
                bind.KeybKey = keybinds.keybinds[k].Key;
                bind.KeybModif = keybinds.keybinds[k].Modifier;
            }
        }
    }

    // Load all settings.
    internal static void LoadSettings()
    {
        for (int i = 0; i < ModLoader.LoadedMods.Count; i++)
        {
            // Check if there is custom settings file (if not, ignore)
            string path = Path.Combine(ModLoader.GetModSettingsFolder(ModLoader.LoadedMods[i]), "settings.json");
            if (!File.Exists(path))
                SaveSettings(ModLoader.LoadedMods[i]); //create settings file if not exists.
                                                       //Load and deserialize 
            SettingsList settings = JsonConvert.DeserializeObject<SettingsList>(File.ReadAllText(path));
            //  ModLoader.LoadedMods[i].isDisabled = settings.isDisabled;
            if (!ModLoader.LoadedMods[i].isDisabled)
            {
                try
                {
                    if (ModLoader.LoadedMods[i].newFormat && ModLoader.LoadedMods[i].fileName != null)
                    {
                        if (ModLoader.LoadedMods[i].A_OnMenuLoad != null)
                        {
                            Console.WriteLine($"Calling OnMenuLoad (for mod {ModLoader.LoadedMods[i].ID})");
                            ModLoader.LoadedMods[i].A_OnMenuLoad.Invoke();
                            ModLoader.LoadedMods[i].disableWarn = true;
                        }
                    }
                    else
                    {
#if MSC
                        if (ModLoader.LoadedMods[i].LoadInMenu && ModLoader.LoadedMods[i].fileName != null)
                        {
                            Console.WriteLine($"Calling OnMenuLoad [old format] (for mod {ModLoader.LoadedMods[i].ID})");
                            ModLoader.LoadedMods[i].OnMenuLoad();
                            ModLoader.LoadedMods[i].disableWarn = true;
                        }
                        if (ModLoader.CheckEmptyMethod(ModLoader.LoadedMods[i], "MenuOnLoad"))
                        {
                            Console.WriteLine($"Calling OnMenuLoad [old format pro] (for mod {ModLoader.LoadedMods[i].ID})");
                            ModLoader.LoadedMods[i].MenuOnLoad();
                            ModLoader.LoadedMods[i].disableWarn = true;
                        }
#endif
                    }
                }
                catch (Exception e)
                {
                    ModLoader.ModException(e, ModLoader.LoadedMods[i]);
                }
            }
            if (Settings.GetModSettings(ModLoader.LoadedMods[i]).Count == 0)
                continue;
            try
            {
                for (int j = 0; j < settings.settings.Count; j++)
                {
                    ModSetting ms = Settings.GetModSettings(ModLoader.LoadedMods[i]).Find(x => x.ID == settings.settings[j].ID);
                    if (ms == null)
                        continue;
                    switch (ms.SettingType)
                    {
                        case SettingsType.CheckBoxGroup:
                            SettingsCheckBoxGroup group = (SettingsCheckBoxGroup)ms;
                            group.SetValue(bool.Parse(settings.settings[j].Value.ToString()));
                            break;
                        case SettingsType.CheckBox:
                            SettingsCheckBox check = (SettingsCheckBox)ms;
                            check.SetValue(bool.Parse(settings.settings[j].Value.ToString()));
                            break;
                        case SettingsType.Slider:
                            SettingsSlider slider = (SettingsSlider)ms;
                            slider.SetValue(float.Parse(settings.settings[j].Value.ToString()));
                            break;
                        case SettingsType.SliderInt:
                            SettingsSliderInt sliderInt = (SettingsSliderInt)ms;
                            sliderInt.SetValue(int.Parse(settings.settings[j].Value.ToString()));
                            break;
                        case SettingsType.TextBox:
                            SettingsTextBox textBox = (SettingsTextBox)ms;
                            textBox.SetValue(settings.settings[j].Value.ToString());
                            break;
                        case SettingsType.DropDown:
                            SettingsDropDownList dropDown = (SettingsDropDownList)ms;
                            dropDown.SetSelectedItemIndex(int.Parse(settings.settings[j].Value.ToString()));
                            break;
                        case SettingsType.ColorPicker:
                            SettingsColorPicker colorPicker = (SettingsColorPicker)ms;
                            colorPicker.Value = settings.settings[j].Value.ToString();
                            break;
                        default:
                            break;
                    }
                }

                if (!ModLoader.LoadedMods[i].isDisabled)
                {
                    if (ModLoader.LoadedMods[i].newSettingsFormat)
                    {
                        if (ModLoader.LoadedMods[i].A_ModSettingsLoaded != null)
                        {
                            ModLoader.LoadedMods[i].A_ModSettingsLoaded.Invoke();
                        }
                    }
#if MSC
                    else
                        ModLoader.LoadedMods[i].ModSettingsLoaded();
#endif
                }
            }
            catch (Exception e)
            {
                ModLoader.ModException(e, ModLoader.LoadedMods[i]);
                ResetSettings(ModLoader.LoadedMods[i]); //Failed settings save file
            }
        }
    }

    internal static void ModMenuHandle()
    {
        GameObject.Find("Systems").transform.Find("OptionsMenu").gameObject.AddComponent<ModMenuHandler>().modMenuUI = instance.UI.transform.GetChild(0).gameObject;
        instance.UI.transform.GetChild(0).gameObject.SetActive(false);
    }

    public class ModMenuHandler : MonoBehaviour
    {
        public GameObject modMenuUI;
        private bool isApplicationQuitting = false;
        void OnEnable()
        {
            modMenuUI.SetActive(true);
            //    StartCoroutine(CursorPM());
        }
        void OnDisable()
        {
            if (isApplicationQuitting) return;
            modMenuUI.SetActive(false);
            if (ListStuff.settingsOpened)
            {
                SaveSettings(ModLoader.LoadedMods[0]);
                SaveSettings(ModLoader.LoadedMods[1]);
            }
        }
        void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }
    }
}

#endif