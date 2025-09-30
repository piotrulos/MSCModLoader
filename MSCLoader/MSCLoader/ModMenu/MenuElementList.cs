using Ionic.Zip;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine.UI;

namespace MSCLoader
{
    internal class MenuElementList : MonoBehaviour
    {
        public Mod mod;
        public References refs;

        [Header("Values")]
        public RawImage icon;
        public Text Title, Author, Description, QuickInfo, WarningInfo, WarningText;
        public Button SettingsBtn, KeybindsBtn, MoreInfoBtn, WarningBtn, BugReportBtn;
        public Toggle DisableMod;
        public Texture2D invalidModIcon, ReferenceIcon, invalidReferenceIcon;

        [Header("Additional Values")]
        public Text DownloadInfoTxt;
        public Button DownloadUpdateBtn, OpenDownloadWebsiteBtn;

        [Header("Anim stuff")]
        public RectTransform contents;
        public GameObject quickInfo, warningInfo;
        private bool collapsed;
        private bool anim;
#if !Mini
        public void ModInfoFill()
        {
            if (mod == null) return;

            if (mod.isDisabled)
            {
                Title.text = $"<color=red>{mod.Name}</color>";
                WarningText.gameObject.SetActive(true);
                BugReportBtn.gameObject.SetActive(false);
                WarningText.text = "Mod is disabled";
            }
            else
            {
                Title.text = mod.Name;
                if (mod.isEA)
                {
                    WarningText.gameObject.SetActive(true);
                    WarningBtn.gameObject.SetActive(true);
                    WarningText.text = "<color=aqua>Early Access Mod</color>";
                    WarningInfo.text = $"This mod is a <color=aqua>Early Access Mod</color>. There may be bugs. {Environment.NewLine}Please respect any rules given by mod author, breaking them may result in blacklisting from ANY future Early Access mods.";
                }
                if (mod.proSettings || (mod.AdditionalReferences != null && mod.AdditionalReferences.Contains("MSCLoader.Features")))
                {
                    WarningText.gameObject.SetActive(true);
                    WarningBtn.gameObject.SetActive(true);
                    BugReportBtn.gameObject.SetActive(false);
                    WarningText.text = "<color=lightblue>Compatibility Mode (Pro)</color>";
                    WarningInfo.text = $"This mod runs in <color=lightblue>compatibility mode</color>. Some features might not work as intended. {Environment.NewLine}Check if there is new version (or remake) available.";
                }
            }
            Author.text = $"by <color=orange><b>{mod.Author}</b></color> (<color=aqua>{mod.Version}</color>)";
            if (string.IsNullOrEmpty(mod.Description))
                Description.text = "No short description provided...";
            else
                Description.text = mod.Description;
            DisableMod.isOn = mod.isDisabled;
            DisableMod.onValueChanged.AddListener(DisableThisMod);
            QuickInfo.text = $"<color=yellow>ID:</color> <color=aqua>{mod.ID}</color> (MSCLoader <color=yellow>{mod.compiledVersion}</color>){Environment.NewLine}";
            if (mod.hasUpdate)
                QuickInfo.text += $"<color=yellow>Version:</color> <color=aqua>{mod.Version}</color> (<color=lime>{mod.UpdateInfo.mod_version} available</color>){Environment.NewLine}";
            else
                QuickInfo.text += $"<color=yellow>Version:</color> <color=aqua>{mod.Version}</color>{Environment.NewLine}";
            QuickInfo.text += $"<color=yellow>Author:</color> <color=aqua>{mod.Author}</color>";

            if (mod.metadata != null)
            {
                if (!string.IsNullOrEmpty(mod.metadata.description))
                {
                    if (string.IsNullOrEmpty(mod.Description))
                        Description.text = mod.metadata.description;
                }
                if (!string.IsNullOrEmpty(mod.metadata.icon))
                {
                    if (File.Exists(Path.Combine(ModLoader.MetadataFolder, Path.Combine("Mod Icons", mod.metadata.icon))))
                    {
                        try
                        {
                            Texture2D t2d = new(1, 1);
                            t2d.LoadImage(File.ReadAllBytes(Path.Combine(ModLoader.MetadataFolder, Path.Combine("Mod Icons", mod.metadata.icon))));
                            icon.texture = t2d;
                        }
                        catch (Exception e)
                        {
                            ModConsole.Error(e.Message);
                            System.Console.WriteLine(e);
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (mod.Icon != null)
                        {
                            Texture2D t2d = new(1, 1);
                            t2d.LoadImage(mod.Icon);
                            icon.texture = t2d;
                        }
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error(e.Message);
                        System.Console.WriteLine(e);
                    }

                }
            }
            else
            {
                try
                {
                    if (mod.Icon != null)
                    {
                        Texture2D t2d = new Texture2D(1, 1);
                        t2d.LoadImage(mod.Icon);
                        icon.texture = t2d;
                    }
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                }

            }
        }

        public void ModButtonsPrep(UniversalView uv)
        {
            if (Settings.GetModSettings(mod).Count > 0)
            {
                SettingsBtn.gameObject.SetActive(true);
                SettingsBtn.onClick.AddListener(delegate
                {
                    uv.FillSettings(mod);
                });
            }
            if (Keybind.GetKeybinds(mod).Count > 0)
            {
                KeybindsBtn.gameObject.SetActive(true);
                KeybindsBtn.onClick.AddListener(delegate
                {
                    uv.FillKeybinds(mod);
                });
            }
            MoreInfoBtn.onClick.AddListener(delegate
            {
                uv.FillMetadataInfo(mod);
            });
            if (mod.metadata == null || !ModLoader.CheckSteam())
            {
                BugReportBtn.gameObject.SetActive(false);
            }
            BugReportBtn.onClick.AddListener(delegate
            {
                ShowBugReportWindow(mod);
            });
        }
        public void InvalidMod(InvalidMods mod)
        {
            Title.text = $"<color=red>{mod.FileName}</color>";
            WarningBtn.gameObject.SetActive(true);
            BugReportBtn.gameObject.SetActive(false);
            WarningText.gameObject.SetActive(true);
            WarningText.text = "Failed to load";
            Author.text = string.Empty;
            Description.text = $"Failed to load this mod{Environment.NewLine}Error: <color=yellow>{mod.ErrorMessage}</color>";
            WarningInfo.text = $"<color=orange>This Mod failed to load.</color>{Environment.NewLine}Error: <color=yellow>{mod.ErrorMessage}</color>";
            if (mod.IsManaged)
            {
                QuickInfo.text = $"<color=orange>This mod failed to load.</color>{Environment.NewLine}Guid: <color=yellow>{mod.AsmGuid}</color>";
                if (mod.AdditionalRefs.Count > 0)
                    QuickInfo.text += $"{Environment.NewLine}Additional references: <color=aqua>{string.Join(", ", mod.AdditionalRefs.ToArray())}</color>";
            }
            else
                QuickInfo.text = $"<color=orange>This doesn't look like a valid mod file.</color>";
            icon.texture = invalidModIcon;
            DisableMod.gameObject.SetActive(false);
            MoreInfoBtn.gameObject.SetActive(false);
        }
        public void ReferenceInfoFill(References rf)
        {
            if (rf.Invalid)
            {
                Title.text = $"<color=red>{rf.FileName}</color>";
                Author.text = string.Empty;
                WarningText.text = "Failed to load";
                Description.text = "<color=yellow>Failed to load this reference</color>";
                WarningText.gameObject.SetActive(true);
                WarningBtn.gameObject.SetActive(true);
                icon.texture = invalidReferenceIcon;
                WarningInfo.text = $"<color=orange>This Reference failed to load.</color>{Environment.NewLine}";
                WarningInfo.text += $"If this is native (c++) library, put in near .exe file{Environment.NewLine}";
                WarningInfo.text += $"Exception: <color=yellow>{rf.ExMessage}</color>";
                QuickInfo.text = $"<color=orange>This Reference failed to load.</color>{Environment.NewLine}";

                return;
            }
            Title.text = rf.AssemblyTitle;
            if (ModLoader.Instance.ReferencesList.Where(x => x.Guid == rf.Guid).ToArray().Length > 1)
            {
                WarningText.text = "Loaded more than once";
                WarningText.gameObject.SetActive(true);
                WarningBtn.gameObject.SetActive(true);
                WarningInfo.text = $"<color=yellow>This Reference has been detected to be loaded more than once.</color>{Environment.NewLine}";
                WarningInfo.text += $"This can cause namespace conflict issues.{Environment.NewLine}";
                WarningInfo.text += $"<color=yellow>If you created this, make sure to keep same file name when releasing new version of reference.</color>{Environment.NewLine}";
            }
            if (string.IsNullOrEmpty(rf.AssemblyAuthor))
                Author.text = $"by <color=orange>Unknown</color> (<color=aqua>{rf.AssemblyFileVersion}</color>)";
            else
                Author.text = $"by <color=orange>{rf.AssemblyAuthor}</color> (<color=aqua>{rf.AssemblyFileVersion}</color>)";
            if (string.IsNullOrEmpty(rf.AssemblyDescription))
                Description.text = "No description provided...";
            else
                Description.text = rf.AssemblyDescription;
            QuickInfo.text = $"<color=orange>{rf.FileName}</color>{Environment.NewLine}";
            QuickInfo.text += $"<color=yellow>ID:</color> <color=aqua>{rf.AssemblyID}</color>{Environment.NewLine}";
            QuickInfo.text += $"<color=yellow>Title:</color> <color=aqua>{rf.AssemblyTitle}</color>{Environment.NewLine}";
            if (string.IsNullOrEmpty(rf.AssemblyAuthor))
                QuickInfo.text += $"<color=yellow>Author:</color> <color=red>Unknown</color>{Environment.NewLine}";
            else
                QuickInfo.text += $"<color=yellow>Author:</color> <color=aqua>{rf.AssemblyAuthor}</color>{Environment.NewLine}";
            QuickInfo.text += $"<color=yellow>Version:</color> <color=aqua>{rf.AssemblyFileVersion}</color>{Environment.NewLine}";
            //  QuickInfo.text += $"<color=yellow>Guid test:</color> <color=aqua>{rf.Guid}</color>{Environment.NewLine}";

        }
        internal void ShowChangelog(string id, string ver, string name)
        {
            string dwl = string.Empty;
            WebClient getdwl = new WebClient();
            getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
            try
            {
                dwl = getdwl.DownloadString($"{ModLoader.serverURL}/changelog.php?mods={id}&vers={ver}&names={name}");
            }
            catch (Exception e)
            {
                dwl = "<color=red>Failed to download changelog...</color>";
                Console.WriteLine(e);
            }
            ModUI.ShowChangelogWindow(dwl);
        }
        PopupSetting bugReport;
        internal void ShowBugReportWindow(Mod mod)
        {
            if (ModLoader.HasUpdateModList.Contains(mod))
            {
                ModUI.ShowMessage("This mod has an update available! <color=aqua>Please update mod to the latest version before reporting bugs!</color>", "Error");
                return;
            }
            string dwl = string.Empty;
            WebClient getdwl = new WebClient();
            getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
            try
            {
                dwl = getdwl.DownloadString($"{ModLoader.serverURL}/mscl_bugreport.php?steam={ModLoader.steamID}&resid={mod.ID}");
            }
            catch (Exception e)
            {
                ModUI.ShowMessage($"Failed to get info from bug report server {Environment.NewLine} Error: <color=orange>{e.Message}</color>", "Error");
                Console.WriteLine(e);
                return;
            }
            string[] result = dwl.Split('|');
            if (result[0] == "error")
            {
                ModUI.ShowMessage($"Error: <color=orange>{result[1]}</color>", "Error");
                return;
            }
            if (result[0] == "disabled")
            {
                ModUI.ShowMessage($"This mod has disabled bug reports{Environment.NewLine}Reason: <color=orange>{result[1]}</color>", "Mod Author disabled bug reports");
                return;
            }
            if (result[0] == "banned")
            {
                ModUI.ShowMessage($"You have been banned from using bug reports{Environment.NewLine}Reason: <color=orange>{result[1]}</color>", "Banned");
                return;
            }
            bugReport = ModUI.CreatePopupSetting($"Bug Report ({mod.ID})", "Submit Bug Report");
            bugReport.AddText($"This is form where you can report bugs/issues with mod named: <color=aqua><b>{mod.Name}</b></color>. Please make sure to include as much information as possible.{Environment.NewLine}{Environment.NewLine}" +
                $"Trolling/Spam/Abuse can cause your report to be ignored, and may result in blacklisting you from using this feature in future.");
            bugReport.AddTextBox("modID", "Mod ID", mod.ID, string.Empty, false);
            bugReport.AddTextBox("bugReportTitle", "Bug Report Title", string.Empty, "Enter Bug Report Title...");
            bugReport.AddTextArea("bugReportDesc", "Bug Report Description", string.Empty, "Describe the bug/issue...");
            bugReport.AddText("You can also include your save file, this could help modder narrow down the issue.");
            bugReport.AddCheckBox("bugReportSaveFile", "Include Save File", false);
            bugReport.AddText("This report will also include error log");
            bugReport.ShowPopup(CreateBugReport, true);
            if (result[0] == "inactive")
                ModUI.ShowMessage($"This mod author was last active <color=aqua>{result[1]}</color>. {Environment.NewLine}There is a chance that your bug report will not be read.", "Inactive Mod Author");
        }
        class BugReportResult
        {
            public string modID = string.Empty;
            public string bugReportTitle = string.Empty;
            public string bugReportDesc = string.Empty;
            public bool bugReportSaveFile = false;

            public string version = string.Empty;
            public string modList = string.Empty;
        }
        internal void CreateBugReport(string response)
        {
            BugReportResult report = ModUI.ParsePopupResponse<BugReportResult>(response);
            if (report.bugReportTitle == string.Empty)
            {
                ModUI.ShowMessage("Please enter bug report title...", "Error");
                return;
            }
            if (report.bugReportDesc == string.Empty)
            {
                ModUI.ShowMessage("Please enter bug report description...", "Error");
                return;
            }
            Mod mod = ModLoader.GetModByID(report.modID);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"===== Loaded References ====={Environment.NewLine}");

            foreach (References m in ModLoader.Instance.ReferencesList)
            {
                sb.AppendLine($"{(mod.AdditionalReferences != null && mod.AdditionalReferences.Contains(m.AssemblyID) ? "-->" : "")} {Path.GetFileName(m.FileName)} [ID: {m.AssemblyID}] - {m.AssemblyTitle} v{m.AssemblyFileVersion} {(ModLoader.HasUpdateRefList.Contains(m) ? "(Update Pending)" : "")}");
            }
            sb.AppendLine($"===== Loaded Mods ====={Environment.NewLine}");

            foreach (Mod m in ModLoader.Instance.actualModList)
            {
                sb.AppendLine($"{(mod.ID == m.ID ? "===>" : "")} [ID: {m.ID}] - {m.Name} v{m.Version} {(m.isDisabled ? "(Disabled)" : "")} {((m.proSettings || (m.AdditionalReferences != null && m.AdditionalReferences.Contains("MSCLoader.Features"))) ? "(Compatiblity Pro)" : "")} {((m.Description != null && m.Description.Contains(MSCLInternal.ProLoaderMagic())) ? "*" : "")} {(ModLoader.HasUpdateModList.Contains(m) ? "(Update Pending)" : "")}");
            }

            sb.AppendLine($"===== Invalid files ====={Environment.NewLine}");

            foreach (InvalidMods m in ModLoader.InvalidMods)
            {
                sb.AppendLine($"{m.FileName} Error: {m.ErrorMessage} - GUID: {m.AsmGuid}");
            }
            if (!Directory.Exists(Path.Combine("Updates", "Br_temp")))
                Directory.CreateDirectory(Path.Combine("Updates", "Br_temp"));
            string dir = Path.Combine("Updates", "Br_temp");
            File.WriteAllText(Path.Combine(dir, "ModList.txt"), sb.ToString());
            report.modList = sb.ToString();

            report.version = mod.Version;
            File.WriteAllText(Path.Combine(dir, "bugReport.json"), Newtonsoft.Json.JsonConvert.SerializeObject(report));
            ModConsole.Print("Zipping Files...");
            ZipFile zip = new ZipFile();
            zip.AddFile(Path.Combine(dir, "bugReport.json"), "");
            zip.AddFile(Path.Combine(dir, "ModList.txt"), "");
            zip.AddFile(Path.Combine(".", "output_log.txt"), "");
            if (report.bugReportSaveFile)
            {
                if (File.Exists(Path.Combine(Application.persistentDataPath, "defaultES2File.txt")))
                    zip.AddFile(Path.Combine(Application.persistentDataPath, "defaultES2File.txt"), "Save");
                if (File.Exists(Path.Combine(Application.persistentDataPath, "items.txt")))
                    zip.AddFile(Path.Combine(Application.persistentDataPath, "items.txt"), "Save");
                if (File.Exists(Path.Combine(Application.persistentDataPath, "Mods.txt")))
                    zip.AddFile(Path.Combine(Application.persistentDataPath, "Mods.txt"), "Save");
            }
            zip.Save(Path.Combine(dir, $"BugReport_{mod.ID}.zip"));
            ModConsole.Print("Zipping Files... Done!");

            bugReport.ClosePopup();
            StartCoroutine(UploadBugReport(mod.ID, Path.Combine(dir, $"BugReport_{mod.ID}.zip")));
        }
        bool bugUploadInProgress = false;
        IEnumerator UploadBugReport(string ID, string file)
        {
            string steamID = Steamworks.SteamUser.GetSteamID().ToString();
            using (WebClient Client = new WebClient())
            {
                Client.Headers.Add("Content-Type", "binary/octet-stream");
                Client.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                Client.UploadFileCompleted += UploadBugReportCompleted;
                Client.UploadProgressChanged += UploadBugReportProgressChanged;
                bugUploadInProgress = true;
                Client.UploadFileAsync(new Uri($"{ModLoader.serverURL}/mscl_bugreport.php?steam={steamID}&resid={ID}"), "POST", file);
            }
            yield return null;
            while (bugUploadInProgress)
            {
                yield return null;
            }
            File.Delete(file);
        }

        private void UploadBugReportProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            bugUploadInProgress = true;
            ModConsole.Print("Uploading... " + e.ProgressPercentage);
        }

        private void UploadBugReportCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ModConsole.Error("Failed to upload");
                ModConsole.Error(e.Error.Message);
                return;
            }
            else
            {
                string s = System.Text.Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                string[] result = s.Split('|');
                if (result[0] == "error")
                {
                    ModConsole.Error($"Failed to upload bug report: {result[1]}");
                    return;
                }
                if (result[0] == "ok")
                {
                    ModUI.ShowMessage("Bug Report uploaded successfully!", "Success");
                }
            }
            bugUploadInProgress = false;

        }

        public void UpdateInfoFill()
        {
            if (refs != null)
            {
                Title.text = $"<color=lime>{refs.AssemblyTitle}</color>";
                if (string.IsNullOrEmpty(refs.AssemblyAuthor))
                    Author.text = $"by <color=orange>Unknown</color> (<color=aqua>{refs.AssemblyFileVersion}</color>)";
                else
                    Author.text = $"by <color=orange>{refs.AssemblyAuthor}</color> (<color=aqua>{refs.AssemblyFileVersion}</color>)";
                if (string.IsNullOrEmpty(refs.AssemblyDescription))
                    Description.text = "No description provided...";
                else
                    Description.text = refs.AssemblyDescription;
                DownloadInfoTxt.text = $"Update available ({refs.UpdateInfo.ref_version})";
                if (ModLoader.RefSelfUpdateList.Contains(refs.AssemblyID))
                    DownloadUpdateBtn.onClick.AddListener(delegate
                    {
                        ModLoader.Instance.DownloadRefUpdate(refs);
                    });
                else
                    DownloadUpdateBtn.gameObject.SetActive(false);
                OpenDownloadWebsiteBtn.gameObject.SetActive(false);
                MoreInfoBtn.onClick.AddListener(() => ShowChangelog(refs.AssemblyID, refs.UpdateInfo.ref_version, refs.AssemblyTitle));

                icon.texture = ReferenceIcon;
            }
            if (mod != null)
            {
                Title.text = $"<color=lime>{mod.Name}</color>";
                Author.text = $"by <color=orange>{mod.Author}</color> (<color=aqua>{mod.Version}</color>)";
                DownloadInfoTxt.text = $"New Version ({mod.UpdateInfo.mod_version})";
                if (ModLoader.ModSelfUpdateList.Contains(mod.ID))
                    DownloadUpdateBtn.onClick.AddListener(delegate
                    {
                        ModLoader.Instance.DownloadModUpdate(mod);
                    });
                else
                    DownloadUpdateBtn.gameObject.SetActive(false);
                if (!string.IsNullOrEmpty(mod.metadata.description))
                {
                    if (string.IsNullOrEmpty(mod.Description))
                        Description.text = mod.metadata.description;
                }
                if (!string.IsNullOrEmpty(mod.metadata.links[0]))
                {
                    OpenDownloadWebsiteBtn.onClick.AddListener(() => Application.OpenURL(mod.metadata.links[0]));
                    MoreInfoBtn.onClick.AddListener(() => ShowChangelog(mod.ID, mod.UpdateInfo.mod_version, mod.Name));
                }
                else if (!string.IsNullOrEmpty(mod.metadata.links[2]))
                {
                    OpenDownloadWebsiteBtn.onClick.AddListener(() => Application.OpenURL(mod.metadata.links[2]));
                    MoreInfoBtn.gameObject.SetActive(false);
                }
                else if (!string.IsNullOrEmpty(mod.metadata.links[1]))
                {
                    OpenDownloadWebsiteBtn.onClick.AddListener(() => Application.OpenURL(mod.metadata.links[1]));
                    MoreInfoBtn.gameObject.SetActive(false);
                }
                else
                {
                    OpenDownloadWebsiteBtn.gameObject.SetActive(false);
                    MoreInfoBtn.gameObject.SetActive(false);
                }
                if (!string.IsNullOrEmpty(mod.metadata.icon))
                {
                    if (File.Exists(Path.Combine(ModLoader.MetadataFolder, Path.Combine("Mod Icons", mod.metadata.icon))))
                    {
                        try
                        {
                            Texture2D t2d = new Texture2D(1, 1);
                            t2d.LoadImage(File.ReadAllBytes(Path.Combine(ModLoader.MetadataFolder, Path.Combine("Mod Icons", mod.metadata.icon))));
                            icon.texture = t2d;
                        }
                        catch (Exception e)
                        {
                            ModConsole.Error(e.Message);
                            Console.WriteLine(e);
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (mod.Icon != null)
                        {
                            Texture2D t2d = new Texture2D(1, 1);
                            t2d.LoadImage(mod.Icon);
                            icon.texture = t2d;
                        }
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error(e.Message);
                        Console.WriteLine(e);
                    }

                }

            }
        }

        public void DisableThisMod(bool ischecked)
        {
            if (mod.isDisabled != ischecked)
            {
                if (mod.metadata != null)
                {
                    if (mod.metadata.type == 9)
                    {
                        ModUI.ShowMessage($"This mod cannot be enabled. {Environment.NewLine}Reason: <color=yellow>{mod.metadata.msg}</color>");
                        return;
                    }
                }
                bool onEnableExist = false;
                mod.isDisabled = ischecked;
                if (ischecked)
                {
                    try
                    {
                        if (mod.newEnDisFormat)
                        {
                            if (mod.A_OnModDisabled != null)
                            {
                                mod.A_OnModDisabled.Invoke();
                                mod.disableWarn = false;
                            }
                        }
                        else
                        {
                            mod.OnModDisabled();
                        }
                    }
                    catch (Exception e)
                    {
                        ModLoader.ModException(e, mod);
                    }
                    Title.text = $"<color=red>{mod.Name}</color>";
                    WarningText.gameObject.SetActive(true);
                    WarningText.text = "Mod is disabled";
                    if (mod.disableWarn)
                    {
                        ModUI.ShowMessage("This mod was loaded in Main Menu, to fully disable this mod you need to restart the game.");
                        ModConsole.Print($"Mod <b><color=orange>{mod.Name}</color></b> marked to be disabled <color=red><b>Disabled</b></color> on next restart");
                    }
                    else
                        ModConsole.Print($"Mod <b><color=orange>{mod.Name}</color></b> has been <color=red><b>Disabled</b></color>");

                }
                else
                {
                    try
                    {
                        if (mod.newEnDisFormat)
                        {
                            if (mod.A_OnModEnabled != null)
                            {
                                mod.A_OnModEnabled.Invoke();
                                onEnableExist = true;
                            }
                        }
                        else
                        {
                            mod.OnModEnabled();
                        }
                    }
                    catch (Exception e)
                    {
                        ModLoader.ModException(e, mod);
                    }
                    Title.text = mod.Name;
                    WarningText.text = string.Empty;
                    WarningText.gameObject.SetActive(false);
                    if (onEnableExist)
                        ModConsole.Print($"Mod <b><color=orange>{mod.Name}</color></b> has been <color=green><b>Enabled</b></color>");
                    else
                        ModConsole.Print($"Mod <b><color=orange>{mod.Name}</color></b> has been <color=green><b>Enabled</b></color>, restart may be required.");
                }
                ModMenu.SaveSettings(mod);
            }
        }
#endif
        public void ToggleInfo()
        {
            if (anim) return;
            if (collapsed)
            {
                StartCoroutine(Anim(true));
                quickInfo.SetActive(false);
            }
            else
            {
                StartCoroutine(Anim(false));
                quickInfo.SetActive(true);
            }
        }
        public void ToggleWarning()
        {
            if (anim) return;
            if (collapsed)
            {
                StartCoroutine(Anim(true));
                warningInfo.SetActive(false);
            }
            else
            {
                StartCoroutine(Anim(false));
                warningInfo.SetActive(true);
            }
        }
        IEnumerator Anim(bool expand)
        {

            anim = true;
            if (expand)
            {
                for (int i = 0; i < 50; i++)
                {
                    contents.localScale = new Vector3(1, (float)System.Math.Round(contents.localScale.y + 0.02f, 2), 1);
                    yield return null;
                }
                collapsed = false;
            }
            else
            {

                for (int i = 0; i < 50; i++)
                {
                    contents.localScale = new Vector3(1, (float)System.Math.Round(contents.localScale.y - 0.02f, 2), 1);
                    yield return null;
                }
                collapsed = true;

            }
            anim = false;
        }
    }
}