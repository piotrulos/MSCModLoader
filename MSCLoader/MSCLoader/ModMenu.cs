using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MSCLoader
{
    internal class ModMenu : Mod
    {
        public override string ID => "MSCLoader_Settings";
        public override string Name => "Settings (Main)";
        public override string Version => ModLoader.MSCLoader_Ver;
        public override string Author => "piotrulos";

        internal static SettingsCheckBox dm_disabler, dm_logST, dm_operr, dm_warn, dm_pcon;
        internal static SettingsCheckBox expWarning, modPath, forceMenuVsync, openLinksOverlay, skipGameIntro, syncLoad, showIcons;

        private static SettingsCheckBoxGroup checkLaunch, checkDaily, checkWeekly;


        public GameObject UI;
        internal static byte cfmu_set = 0;

        static ModMenu instance;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnMenuLoad, Mod_OnMenuLoad);
        }

        public override void ModSettings()
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
            expWarning = Settings.AddCheckBox(this, "MSCLoader_expWarning", "Show experimental warning (experimental on steam)", true);
            modPath = Settings.AddCheckBox(this, "MSCLoader_modPath", "Show mods folder (top left corner)", true, ModLoader.MainMenuPath);
            forceMenuVsync = Settings.AddCheckBox(this, "MSCLoader_forceMenuVsync", "60 FPS limit in Main Menu", true, VSyncSwitchCheckbox);
            openLinksOverlay = Settings.AddCheckBox(this, "MSCLoader_openLinksOverlay", "Open URLs in steam overlay", true);
            skipGameIntro = Settings.AddCheckBox(this, "MSCLoader_skipGameIntro", "Skip game Splash Screen", false, SkipIntroSet);
            Settings.AddText(this, "If you notice a big lag spike when opening mods tab, you can uncheck Show custom icons. This can happen if someone put unnecessary large icon.");
            showIcons = Settings.AddCheckBox(this, "MSCLoader_showIcons", "Show custom icons on mod list", true);

            Settings.AddText(this, $"If for whatever reason you want to save half a second of mods loading time, enable below option.{Environment.NewLine}(Loading progress <color=yellow>cannot</color> be displayed in synchronous mode, and game may look frozen during loading)");
            syncLoad = Settings.AddCheckBox(this, "MSCLoader_syncLoad", "Load mods synchronously", false);
            Settings.AddHeader(this, "Update Settings");
            Settings.AddText(this, "How often MSCLoader should check for Mod/References updates.");
            checkLaunch = Settings.AddCheckBoxGroup(this, "MSCLoader_checkLaunch", "Every launch", false, "cfmu_set");
            checkDaily = Settings.AddCheckBoxGroup(this, "MSCLoader_checkDaily", "Daily", true, "cfmu_set");
            checkWeekly = Settings.AddCheckBoxGroup(this, "MSCLoader_checkWeekly", "Weekly", false, "cfmu_set");
            Settings.AddHeader(this, "MSCLoader Credits",Color.black);
            Settings.AddText(this, "All source code contributors and used libraries are listed on Github");
            Settings.AddText(this, "Outside Github contributions:");
            Settings.AddText(this, "<color=aqua>Horsey4</color> - Backend for mod Early Access feature.");
            Settings.AddText(this, "<color=aqua>BrennFuchS</color> - New default mod icon and expanded Playmaker extensions.");

        }
        public override void ModSettingsLoaded()
        {
            IniData ini = new FileIniDataParser().ReadFile("doorstop_config.ini");
            ini.Configuration.AssigmentSpacer = "";
            string skipIntro = ini["MSCLoader"]["skipIntro"];
            bool introSkip;
            if (!bool.TryParse(skipIntro, out introSkip))
            {
                skipGameIntro.SetValue(false);
                ModConsole.Error($"Excepted boolean, received '{skipIntro ?? "<null>"}'.");
            }
            else
            {
                skipGameIntro.SetValue(introSkip);
            }
            if (checkLaunch.GetValue())
                cfmu_set = 0;
            else if (checkDaily.GetValue())
                cfmu_set = 1;
            else if (checkWeekly.GetValue())
                cfmu_set = 7;

        }

        private static void SkipIntroSet()
        {
            FileIniDataParser parser = new FileIniDataParser();
            IniData ini = parser.ReadFile("doorstop_config.ini");
            ini.Configuration.AssigmentSpacer = "";
            ini["MSCLoader"]["skipIntro"] = skipGameIntro.GetValue().ToString().ToLower();
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
                ModUI.ShowMessage($"Fatal error:{Environment.NewLine}<color=orange>{e.Message}</color>{Environment.NewLine}Please install modloader correctly.", "Fatal Error");
            }
        }

        public void CreateSettingsUI()
        {
            AssetBundle ab = LoadAssets.LoadBundle(this, "settingsui.unity3d");
            UI = GameObject.Instantiate(ab.LoadAsset<GameObject>("Mod Menu.prefab"));
            UI.transform.SetParent(ModUI.GetCanvas().transform, false);
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
                try
                {
                    if (ModLoader.LoadedMods[i].newFormat && ModLoader.LoadedMods[i].fileName != null)
                    {
                        ModLoader.LoadedMods[i].A_OnMenuLoad?.Invoke();
                    }
                    else
                    {
                        if (ModLoader.LoadedMods[i].LoadInMenu && !ModLoader.LoadedMods[i].isDisabled && ModLoader.LoadedMods[i].fileName != null)
                        {
                            ModLoader.LoadedMods[i].OnMenuLoad();
                        }
                        if (ModLoader.CheckEmptyMethod(ModLoader.LoadedMods[i], "MenuOnLoad"))
                        {
                            ModLoader.LoadedMods[i].MenuOnLoad();
                        }
                    }
                }
                catch (Exception e)
                {
                    ModLoader.ModException(e, ModLoader.LoadedMods[i]);
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
                    ModLoader.LoadedMods[i].ModSettingsLoaded();
                }
                catch (Exception e)
                {
                    ModLoader.ModException(e, ModLoader.LoadedMods[i]);
                }
            }
        }

        internal static void ModButton_temp()
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
