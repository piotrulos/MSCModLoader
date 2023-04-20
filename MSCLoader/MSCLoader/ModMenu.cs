#if !Mini
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace MSCLoader
{
    internal class ModMenu : Mod
    {
        public override string ID => "MSCLoader_Settings";
        public override string Name => "[INTERNAL] Mod Menu";
        public override string Version => ModLoader.MSCLoader_Ver;
        public override string Author => "piotrulos";

        internal GameObject UI;
        internal static byte cfmu_set = 0;
        internal static ModMenu instance;
        internal static SettingsCheckBox dm_disabler, dm_logST, dm_operr, dm_warn, dm_pcon;
        internal static SettingsCheckBox expWarning, modPath, forceMenuVsync, openLinksOverlay, skipGameIntro, skipConfigScreen;

        private static SettingsCheckBoxGroup checkLaunch, checkDaily, checkWeekly;
        private System.Diagnostics.FileVersionInfo coreVer;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnMenuLoad, Mod_OnMenuLoad);
            SetupFunction(Setup.ModSettings, Mod_Settings);
            SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoaded);
        }

        private void Mod_Settings()
        {
            instance = this;
            if (ModLoader.devMode)
            {
                Settings.AddHeader(this, "DevMode Settings", new Color32(0, 0, 128, 255), Color.green);
                dm_disabler = Settings.AddCheckBox(this, "MSCLoader_dm_disabler", "Disable mods throwing errors", false);
                dm_logST = Settings.AddCheckBox(this, "MSCLoader_dm_logST", "Log-all stack trace (not recommended)", false);
                dm_operr = Settings.AddCheckBox(this, "MSCLoader_dm_operr", "Log-all open console on error", false);
                dm_warn = Settings.AddCheckBox(this, "MSCLoader_dm_warn", "Log-all open console on warning", false);
                dm_pcon = Settings.AddCheckBox(this, "MSCLoader_dm_pcon", "Persistent console (sometimes may break font)", false);
            }
            Settings.AddHeader(this, "Basic Settings");
            expWarning = Settings.AddCheckBox(this, "MSCLoader_expWarning", "Show experimental warning (experimental branch on Steam)", true);
            modPath = Settings.AddCheckBox(this, "MSCLoader_modPath", "Show mods folder (top left corner)", true, ModLoader.MainMenuPath);
            forceMenuVsync = Settings.AddCheckBox(this, "MSCLoader_forceMenuVsync", "60 FPS limit in Main Menu", true, VSyncSwitchCheckbox);
            openLinksOverlay = Settings.AddCheckBox(this, "MSCLoader_openLinksOverlay", "Open URLs in Steam overlay", true);
            Settings.AddText(this, "Skip stuff:");
            skipGameIntro = Settings.AddCheckBox(this, "MSCLoader_skipGameIntro", "Skip game splash screen", false, SkipIntroSet);
            skipConfigScreen = Settings.AddCheckBox(this, "MSCLoader_skipConfigScreen", "Skip configuration screen", false, SkipConfigScreen);

            Settings.AddHeader(this, "Update Settings");
            Settings.AddText(this, "How often MSCLoader checks for Mod/References updates.");
            checkLaunch = Settings.AddCheckBoxGroup(this, "MSCLoader_checkOnLaunch", "Every launch", true, "cfmu_set");
            checkDaily = Settings.AddCheckBoxGroup(this, "MSCLoader_checkEveryDay", "Daily", false, "cfmu_set");
            checkWeekly = Settings.AddCheckBoxGroup(this, "MSCLoader_checkEveryWeek", "Weekly", false, "cfmu_set");

            Settings.AddHeader(this, "MSCLoader Credits",Color.black);
            Settings.AddText(this, "All source code contributors and used libraries are listed on GitHub");
            Settings.AddText(this, "Non-GitHub contributions:");
            Settings.AddText(this, "<color=aqua>Horsey4</color> - Early Access feature backend code.");
            Settings.AddText(this, "<color=aqua>BrennFuchS</color> - New default mod icon and expanded PlayMaker extensions.");

            Settings.AddHeader(this, "Detailed Version Information", new Color32(0, 128, 0, 255));
            coreVer = System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "MSCLoader.Preloader.dll"));
            Settings.AddText(this, $"MSCLoader modules:{Environment.NewLine}<color=yellow>MSCLoader.Preloader</color>: <color=aqua>v{coreVer.FileMajorPart}.{coreVer.FileMinorPart}.{coreVer.FileBuildPart} build {coreVer.FilePrivatePart}</color>{Environment.NewLine}<color=yellow>MSCLoader</color>: <color=aqua>v{ModLoader.MSCLoader_Ver} build {ModLoader.Instance.currentBuild}</color>");
            Settings.AddText(this, $"Build-in libraries:{Environment.NewLine}<color=yellow>Harmony</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "0Harmony.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>Ionic.Zip</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "Ionic.Zip.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>NAudio</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "NAudio.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>NAudio (Vorbis)</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "NVorbis.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>NAudio (Flac)</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "NAudio.Flac.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>Newtonsoft.Json</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "Newtonsoft.Json.dll")).FileVersion}</color>{Environment.NewLine}" +
                $"<color=yellow>INIFileParser</color>: <color=aqua>v{System.Diagnostics.FileVersionInfo.GetVersionInfo(Path.Combine(Path.Combine("mysummercar_Data", "Managed"), "INIFileParser.dll")).FileVersion}</color>");


        }
     
        private void Mod_SettingsLoaded()
        {

            IniData ini = new FileIniDataParser().ReadFile("doorstop_config.ini");
            ini.Configuration.AssigmentSpacer = "";
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
            ini.Configuration.AssigmentSpacer = "";
            ini["MSCLoader"]["skipIntro"] = skipGameIntro.GetValue().ToString().ToLower();
            parser.WriteFile("doorstop_config.ini", ini, System.Text.Encoding.ASCII);           
        }

        private void SkipConfigScreen()
        {
            if (coreVer.FilePrivatePart < 263)
            {
                ModUI.ShowMessage("To use <color=yellow>Skip Configuration Screen</color> you need to update the core module of MSCLoader, download the latest version and launch <color=aqua>MSCPatcher.exe</color> to update", "Outdated module");
                return;
            }
            FileIniDataParser parser = new FileIniDataParser();
            IniData ini = parser.ReadFile("doorstop_config.ini");
            ini.Configuration.AssigmentSpacer = "";
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
            catch(Exception e)
            {
                ModUI.ShowMessage($"Fatal error:{Environment.NewLine}<color=orange>{e.Message}</color>{Environment.NewLine}Please install MSCLoader correctly.", "Fatal Error");
            }
        }

        public void CreateSettingsUI()
        {
            AssetBundle ab = LoadAssets.LoadBundle(this, "settingsui.unity3d");
            GameObject UIp = ab.LoadAsset<GameObject>("Mod Menu.prefab");
            GameObject UIp2 = ab.LoadAsset<GameObject>("Mod Download.prefab");
            UI = GameObject.Instantiate(UIp);
            UI.name = "MSCLoader Mod Menu";
            UI.transform.SetParent(ModUI.GetCanvas(1).transform, false);
            GameObject.Destroy(UIp);

           /* if (ModLoader.devMode) //Move this into new module
            {
                GameObject UI2 = GameObject.Instantiate(UIp2);
                UI2.name = "MSCLoader Mod Download Menu";
                UI2.transform.SetParent(ModUI.GetCanvas(5).transform, false);
                GameObject.Destroy(UIp2);
                UI2.SetActive(true);
            }*/
            ab.Unload(false);
        }

        // Reset keybinds
        public static void ResetBinds(Mod mod)
        {
            if (mod != null)
            {
                // Revert binds
                Keybind[] bind = Keybind.Get(mod).ToArray();
                for (int i = 0; i < bind.Length; i++)
                {
                    Keybind original = Keybind.GetDefault(mod).Find(x => x.ID == bind[i].ID);

                    if (original != null)
                    {
                        bind[i].Key = original.Key;
                        bind[i].Modifier = original.Modifier;
                    }
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

            Keybind[] binds = Keybind.Get(mod).ToArray();
            for (int i = 0; i < binds.Length; i++)
            {
                if (binds[i].ID == null || binds[i].Vals != null)
                    continue;
                Keybinds keybinds = new Keybinds
                {
                    ID = binds[i].ID,
                    Key = binds[i].Key,
                    Modifier = binds[i].Modifier
                };

                list.keybinds.Add(keybinds);
            }
            string serializedData = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, serializedData);

        }

        // Reset settings
        public static void ResetSettings(Mod mod)
        {
            if (mod != null)
            {
                // Revert settings
                Settings[] set = Settings.Get(mod).ToArray();
                for (int i = 0; i < set.Length; i++)
                {
                    Settings original = Settings.GetDefault(mod).Find(x => x.ID == set[i].ID);

                    if (original != null)
                    {
                        set[i].Value = original.Value;
                    }
                }

                // Save settings
                SaveSettings(mod);
            }
        }
        public static void ResetSpecificSettings(Mod mod, Settings[] sets)
        {
            if (mod != null)
            {
                // Revert settings
                for (int i = 0; i < sets.Length; i++)
                {
                    Settings original = Settings.GetDefault(mod).Find(x => x.ID == sets[i].ID);

                    if (original != null)
                    {
                        sets[i].Value = original.Value;
                    }
                }

                // Save settings
                SaveSettings(mod);
            }
        }

        // Save settings for a single mod to config file.
        public static void SaveSettings(Mod mod)
        {
            SettingsList list = new SettingsList();
            list.isDisabled = mod.isDisabled;
            string path = Path.Combine(ModLoader.GetModSettingsFolder(mod), "settings.json");

            Settings[] set = Settings.Get(mod).ToArray();
            for (int i = 0; i < set.Length; i++)
            {
                if (set[i].SettingType == SettingsType.Button || set[i].SettingType == SettingsType.RButton || set[i].SettingType == SettingsType.Header || set[i].SettingType == SettingsType.Text)
                    continue;

                Setting sets = new Setting
                {
                    ID = set[i].ID,
                    Value = set[i].Value
                };

                list.settings.Add(sets);
            }
            string serializedData = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, serializedData);

        }

        // Load all keybinds.
        public static void LoadBinds()
        {
            Mod[] binds = ModLoader.LoadedMods.Where(mod => Keybind.Get(mod).Count > 0).ToArray();
            for (int i = 0; i < binds.Length; i++)
            {
                // Check if there is custom keybinds file (if not, create)
                string path = Path.Combine(ModLoader.GetModSettingsFolder(binds[i]), "keybinds.json");
                if (!File.Exists(path))
                {
                    SaveModBinds(binds[i]);
                    continue;
                }
                //Load and deserialize 
                KeybindList keybinds = JsonConvert.DeserializeObject<KeybindList>(File.ReadAllText(path));
                if (keybinds.keybinds.Count == 0)
                    continue;
                for (int k = 0; k < keybinds.keybinds.Count; k++)
                {
                    Keybind bind = binds[i].Keybinds.Find(x => x.ID == keybinds.keybinds[k].ID);
                    if (bind == null)
                        continue;
                    bind.Key = keybinds.keybinds[k].Key;
                    bind.Modifier = keybinds.keybinds[k].Modifier;
                }
            }
        }

        // Load all settings.
        public static void LoadSettings()
        {
            for (int i = 0; i < ModLoader.LoadedMods.Count; i++)
            {
                // Check if there is custom settings file (if not, ignore)
                string path = Path.Combine(ModLoader.GetModSettingsFolder(ModLoader.LoadedMods[i]), "settings.json");
                if (!File.Exists(path))
                    SaveSettings(ModLoader.LoadedMods[i]); //create settings file if not exists.
                //Load and deserialize 
                SettingsList settings = JsonConvert.DeserializeObject<SettingsList>(File.ReadAllText(path));
                ModLoader.LoadedMods[i].isDisabled = settings.isDisabled;
                if (!ModLoader.LoadedMods[i].isDisabled)
                {
                    try
                    {
                        if (ModLoader.LoadedMods[i].newFormat && ModLoader.LoadedMods[i].fileName != null)
                        {
                            if (ModLoader.LoadedMods[i].A_OnMenuLoad != null)
                            {
                                ModLoader.LoadedMods[i].A_OnMenuLoad.Invoke();
                                ModLoader.LoadedMods[i].disableWarn = true;
                            }
                        }
                        else
                        {
                            if (ModLoader.LoadedMods[i].LoadInMenu && ModLoader.LoadedMods[i].fileName != null)
                            {
                                ModLoader.LoadedMods[i].OnMenuLoad();
                                ModLoader.LoadedMods[i].disableWarn = true;
                            }
                            if (ModLoader.CheckEmptyMethod(ModLoader.LoadedMods[i], "MenuOnLoad"))
                            {
                                ModLoader.LoadedMods[i].MenuOnLoad();
                                ModLoader.LoadedMods[i].disableWarn = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ModLoader.ModException(e, ModLoader.LoadedMods[i]);
                    }
                }
                if (Settings.Get(ModLoader.LoadedMods[i]).Count == 0)
                    continue;

                for (int j = 0; j < settings.settings.Count; j++)
                {
                    Settings set = Settings.Get(ModLoader.LoadedMods[i]).Find(x => x.ID == settings.settings[j].ID);
                    if (set == null)
                        continue;
                    set.Value = settings.settings[j].Value;
                }
                try
                {
                    if (!ModLoader.LoadedMods[i].isDisabled)
                    {
                        if (ModLoader.LoadedMods[i].newSettingsFormat)
                        {
                            if (ModLoader.LoadedMods[i].A_ModSettingsLoaded != null)
                            {
                                ModLoader.LoadedMods[i].A_ModSettingsLoaded.Invoke();
                            }
                        }
                        else
                            ModLoader.LoadedMods[i].ModSettingsLoaded();
                    }
                }
                catch (Exception e)
                {
                    ModLoader.ModException(e, ModLoader.LoadedMods[i]);
                }
            }
        }

        internal static void ModMenuHandle()
        {
            GameObject.Find("Systems").transform.Find("OptionsMenu").gameObject.AddComponent<ModMenuHandler>().modMenuUI = instance.UI;
            instance.UI.SetActive(false);
        }

        public class ModMenuHandler : MonoBehaviour
        {
            public GameObject modMenuUI;

            void OnEnable()
            {
                modMenuUI.SetActive(true);
                StartCoroutine(CursorPM());
            }
            IEnumerator CursorPM()
            {
                yield return new WaitForSeconds(0.5f);
                //Fix that shitty custom playmaker cursor to regular system one.
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            void OnDisable()
            {
                modMenuUI.SetActive(false);
                if (ListStuff.settingsOpened)
                {
                    SaveSettings(ModLoader.LoadedMods[0]);
                    SaveSettings(ModLoader.LoadedMods[1]);
                }
            }
        }
    }
}
#endif