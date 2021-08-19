using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

namespace MSCLoader
{
    internal class ModsManifest
    {
        public string modID;
        public string version;
        public string description;
        public ManifestLinks links =new ManifestLinks();
        public ManifestIcon icon = new ManifestIcon();
        public ManifestMinReq minimumRequirements = new ManifestMinReq();
        public ManifestModConflict modConflicts = new ManifestModConflict();
        public ManifestModRequired requiredMods = new ManifestModRequired();
        public string sign;
        public string sid_sign;
        public byte type;
        public string msg = null;
    }
    internal class ManifestLinks
    {
        public string nexusLink = null;
        public string rdLink = null;
        public string githubLink = null;
    }
    internal class ManifestIcon
    {
        public string iconFileName = null;
        public bool isIconRemote = false;
        public bool isIconUrl = false;
    }
    internal class ManifestMinReq
    {
        public string MSCLoaderVer = null;
        public uint MSCbuildID = 0;
        public bool disableIfVer = false;
    }
    internal class ManifestModConflict
    {
        public string modIDs = null;
        public string customMessage = null;
        public bool disableIfConflict = false;
    }
    internal class ManifestModRequired
    {
        public string modID = null;
        public string minVer = null;
        public string customMessage = null;
    }
    internal class ModMetadata
    {
        public static void CreateMetadata(Mod mod)
        {
            string steamID;
            if (ModLoader.CheckSteam())
            {
                try
                {
                    new Version(mod.Version);
                }
                catch
                {
                    ModConsole.Error(string.Format("Invalid version: {0}{1}Please use proper version format: (0.0 or 0.0.0 or 0.0.0.0)",mod.Version,Environment.NewLine));
                    return;
                }
                steamID = Steamworks.SteamUser.GetSteamID().ToString();
                try
                {
                    ModsManifest mm = new ModsManifest
                    {
                        modID = mod.ID,
                        version = mod.Version,
                        description = "<i>No description provided...</i>",
                        sign = CalculateFileChecksum(mod.fileName),
                        sid_sign = ModLoader.SidChecksumCalculator(steamID+mod.ID),
                        type = 0
                    };
                    string path = ModLoader.GetMetadataFolder(string.Format("{0}.json", mod.ID));
                    if (File.Exists(path))
                    {
                        ModConsole.Error("Metadata file already exists, to update use update command");
                        return;
                    }
                    string serializedData = JsonConvert.SerializeObject(mm, Formatting.Indented);
                    File.WriteAllText(path, serializedData);
                    ModConsole.Print("<color=green>Raw metadata file created successfully</color>");
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                }
            }
            else
            {
                ModConsole.Error("No valid steam detected");
            }

        }
        public static void UpdateMetadata(Mod mod)
        {
            if (!File.Exists(ModLoader.GetMetadataFolder(string.Format("{0}.json", mod.ID))))
            {
                ModConsole.Error("Metadata file doesn't exists, to create use create command");
                return;
            }
            if (mod.RemMetadata == null)
            {
                ModConsole.Error(string.Format("Your metadata file doesn't seem to be public, you need to upload first before you can update file.{0}If you want to just recreate metadata, delete old file and use create command", Environment.NewLine));
                return;
            }
            string steamID;
            if (ModLoader.CheckSteam())
            {
                try
                {
                    new Version(mod.Version);
                }
                catch
                {
                    ModConsole.Error(string.Format("Invalid version: {0}{1}Please use proper version format: (0.0 or 0.0.0 or 0.0.0.0)", mod.Version, Environment.NewLine));
                    return;
                }
                steamID = Steamworks.SteamUser.GetSteamID().ToString();
                if (mod.RemMetadata.sid_sign != ModLoader.SidChecksumCalculator(steamID + mod.ID))
                {
                    ModConsole.Error("This mod doesn't belong to you, can't continue");
                    return;
                }
                try
                {
                    ModsManifest umm = mod.metadata;
                    Version v1 = new Version(mod.Version);
                    Version v2 = new Version(mod.metadata.version);
                    switch (v1.CompareTo(v2))
                    {
                        case 0:
                            ModConsole.Error(string.Format("Mod version {0} is same as current metadata version {1}, nothing to update.", mod.Version, mod.metadata.version));
                            break;
                        case 1:
                            umm.version = mod.Version;
                            umm.sign = CalculateFileChecksum(mod.fileName);
                            string msad = ModLoader.GetMetadataFolder(string.Format("{0}.json", mod.ID));
                            string serializedData = JsonConvert.SerializeObject(umm, Formatting.Indented);
                            File.WriteAllText(msad, serializedData);
                            ModConsole.Print("<color=green>Metadata file updated successfully, you can upload it now!</color>");
                            break;
                        case -1:
                            ModConsole.Error(string.Format("Mod version {0} is <b>earlier</b> than current metadata version {1}, cannot update.", mod.Version, mod.metadata.version));
                            break;
                    }
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);

                }
            }
            else
            {
                ModConsole.Error("No valid steam detected");
            }
        }

        internal static void ReadMetadata(Mod mod)
        {
            if (mod.metadata == null && mod.RemMetadata == null)
                return;
            if (mod.metadata == null && mod.RemMetadata != null)
                mod.metadata = mod.RemMetadata;
            else if (mod.metadata != null && mod.RemMetadata == null)
                mod.RemMetadata = mod.metadata;
            if (mod.metadata.type == 9)
            {
                //Disabled by reason
                mod.isDisabled = true;
                if (!string.IsNullOrEmpty(mod.metadata.msg))
                    ModConsole.Error(string.Format("Mod <b>{0}</b> has been disabled, Reason: <b>{1}</b>", mod.ID, mod.metadata.msg));
                else
                    ModConsole.Error(string.Format("Mod <b>{0}</b> has been disabled, Reason: <i>No reason given...</i>", mod.ID));
                return;
            }
            if (mod.metadata.type == 2)
            {
                //Disabled by user
                mod.isDisabled = true;
                if (!string.IsNullOrEmpty(mod.metadata.msg))
                    ModConsole.Error(string.Format("Mod <b>{0}</b> has been disabled by author, Reason: <b>{1}</b>", mod.ID, mod.metadata.msg));
                else
                    ModConsole.Error(string.Format("Mod <b>{0}</b> has been disabled by author, Reason: <i>No reason given...</i>", mod.ID));
                return;
            }
            if (mod.RemMetadata.type == 3 && !mod.hasUpdate)
            {
                if (mod.RemMetadata.sign != ModMetadata.CalculateFileChecksum(mod.fileName))
                {
                    mod.isDisabled = true;
                    return;
                }
            }
            if (mod.metadata.type == 1 || mod.metadata.type == 4)
            {
                if (mod.metadata.icon.iconFileName != null && mod.metadata.icon.iconFileName != string.Empty)
                {
                    if (mod.metadata.icon.isIconRemote)
                    {
                        if (!File.Exists(Path.Combine(ModLoader.MetadataFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName)))
                        {
                            WebClient webClient = new WebClient();
                            webClient.Headers.Add("user-agent", string.Format("MSCLoader/{0} ({1})", ModLoader.MSCLoader_Ver, SystemInfo.operatingSystem));
                            webClient.DownloadFileCompleted += ModIcon_DownloadCompleted;
                            webClient.DownloadFileAsync(new Uri(string.Format("{0}/images/modicons/{1}", ModLoader.metadataURL, mod.metadata.icon.iconFileName)), Path.Combine(ModLoader.MetadataFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName));
                        }
                    }
                }
                if (mod.metadata.minimumRequirements.MSCbuildID > 0)
                {
                    try
                    {
                        if (mod.metadata.minimumRequirements.MSCbuildID > Steamworks.SteamApps.GetAppBuildId())
                        {
                            if (mod.metadata.minimumRequirements.disableIfVer)
                            {
                                mod.isDisabled = true;
                                ModConsole.Error(string.Format("Mod <b>{0}</b> requires MSC build at least <b>{1}</b>, your current build is <b>{2}</b>. Author marked this as required!", mod.ID, mod.metadata.minimumRequirements.MSCbuildID, Steamworks.SteamApps.GetAppBuildId()));
                            }
                            else
                            {
                                ModConsole.Warning(string.Format("Mod <b>{0}</b> requires MSC build at least <b>{1}</b>, your current build is <b>{2}</b>. This may cause issues!", mod.ID, mod.metadata.minimumRequirements.MSCbuildID, Steamworks.SteamApps.GetAppBuildId()));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Can't get buildID compare " + e);
                    }
                }
                if (mod.metadata.minimumRequirements.MSCLoaderVer != null && mod.metadata.minimumRequirements.MSCLoaderVer != string.Empty)
                {
                    Version v1 = new Version(mod.metadata.minimumRequirements.MSCLoaderVer);
                    Version v2 = new Version(ModLoader.MSCLoader_Ver);
                    if (v1.CompareTo(v2) == 1)
                    {
                        if (mod.metadata.minimumRequirements.disableIfVer)
                        {
                            mod.isDisabled = true;
                            ModConsole.Error(string.Format("Mod <b>{0}</b> requires MSCLoader at least <b>{1}</b>, your current version is <b>{2}</b>. Author marked this as required!", mod.ID, mod.metadata.minimumRequirements.MSCLoaderVer, ModLoader.MSCLoader_Ver));
                        }
                        else
                        {
                            ModConsole.Warning(string.Format("Mod <b>{0}</b> requires MSCLoader at least <b>{1}</b>, your current version is <b>{2}</b>. This may cause issues!", mod.ID, mod.metadata.minimumRequirements.MSCLoaderVer, ModLoader.MSCLoader_Ver));
                        }
                    }
                }
                if (mod.metadata.modConflicts.modIDs != null && mod.metadata.modConflicts.modIDs != string.Empty)
                {
                    string[] modIDs = mod.metadata.modConflicts.modIDs.Trim().Split(',');
                    for (int i = 0; i < modIDs.Length; i++)
                    {
                        if (ModLoader.LoadedMods.Select(s => s.ID).Where(x => x.Equals(modIDs[i])).Count() != 0)
                        {
                            if (mod.metadata.modConflicts.disableIfConflict)
                            {
                                mod.isDisabled = true;
                                if (mod.metadata.modConflicts.customMessage != null && mod.metadata.modConflicts.customMessage != string.Empty)
                                    ModConsole.Error(string.Format("Mod <color=orange><b>{0}</b></color> is marked as conflict with installed mod <color=orange><b>{1}</b></color>. Author's message: {2}", mod.ID, modIDs[i], mod.metadata.modConflicts.customMessage));
                                else
                                    ModConsole.Error(string.Format("Mod <color=orange><b>{0}</b></color> is marked as conflict with installed mod <color=orange><b>{1}</b></color>.", mod.ID, modIDs[i]));
                            }
                            else
                            {
                                if (mod.metadata.modConflicts.customMessage != null && mod.metadata.modConflicts.customMessage != string.Empty)
                                    ModConsole.Warning(string.Format("Mod <color=red><b>{0}</b></color> is marked as conflict with installed mod <color=red><b>{1}</b></color>. Author's message: {2}", mod.ID, modIDs[i], mod.metadata.modConflicts.customMessage));
                                else
                                    ModConsole.Warning(string.Format("Mod <color=red><b>{0}</b></color> is marked as conflict with installed mod <color=red><b>{1}</b></color>.", mod.ID, modIDs[i]));
                            }
                        }
                    }
                }
                if (mod.metadata.requiredMods.modID != null && mod.metadata.requiredMods.modID != string.Empty)
                {
                    string[] modIDs = mod.metadata.requiredMods.modID.Trim().Split(',');
                    string[] modIDvers = mod.metadata.requiredMods.minVer.Trim().Split(',');

                    for (int i = 0; i < modIDs.Length; i++)
                    {
                        string m = modIDs[i];
                        if (ModLoader.LoadedMods.Select(s => s.ID).Where(x => x.Equals(m)).Count() == 0)
                        {
                            mod.isDisabled = true;
                            if (mod.metadata.requiredMods.customMessage != null && mod.metadata.requiredMods.customMessage != string.Empty)
                                ModConsole.Error($"Mod <b>{mod.ID}</b> is missing required mod <b>{m}</b>. Author's message: {mod.metadata.requiredMods.customMessage}");
                            else
                                ModConsole.Error($"Mod <b>{mod.ID}</b> is missing required mod <b>{m}</b>.");
                            OpenRequiredDownloadPage(m);
                        }
                        else
                        {
                            try
                            {
                                Version v1 = new Version(modIDvers[i]);
                                Version v2 = new Version(ModLoader.LoadedMods.Where(x => x.ID.Equals(m)).FirstOrDefault().Version);
                                if (v1.CompareTo(v2) == 1)
                                {
                                    if (mod.metadata.requiredMods.customMessage != null && mod.metadata.requiredMods.customMessage != string.Empty)
                                        ModConsole.Warning(string.Format("Mod <b>{0}</b> requires mod <b>{1}</b> to be at least version <b>{3}</b>. Author's message: {2}", mod.ID, m, mod.metadata.requiredMods.customMessage, v1));
                                    else
                                        ModConsole.Warning(string.Format("Mod <b>{0}</b> requires mod <b>{1}</b> to be at least version <b>{2}</b>.", mod.ID, m, v1));
                                }
                            }
                            catch (Exception e)
                            {
                                System.Console.WriteLine(e);
                            }
                        }
                    }
                }
            }
        }
        private static void ModIcon_DownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                System.Console.WriteLine(e.Error);
        }
        static bool showedMissingModMessage = false;
        static void OpenRequiredDownloadPage(string m)
        {
            if (showedMissingModMessage) return;
            showedMissingModMessage = true;
            ModUI.ShowYesNoMessage($"Some of the mods requires mod <color=aqua>{m}</color> to be installed.{Environment.NewLine}{Environment.NewLine}Do you want to open download page for this mod?", "Missing mods", delegate {

                Application.OpenURL($"http://my-summer-car.ml/redir.php?mod={m}");
            });
        }
        internal static string CalculateFileChecksum(string fn)
        {
            byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(File.ReadAllBytes(fn));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

}
