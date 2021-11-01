using Ionic.Zip;
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
    internal class ModVersions
    {
        public List<MetaVersion> versions = new List<MetaVersion>();
    }
    internal class MetaVersion
    {
        public string mod_id;
        public string mod_version;
        public byte mod_type;
        public int mod_rev;
    }
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
        public int rev;

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
        public static bool VerifyOwnership(Mod mod)
        {
            string steamID;
            string key;
            string auth = ModLoader.GetMetadataFolder("auth.bin");
            if (!File.Exists(auth))
            {
                ModConsole.Error("No auth key detected. Please set auth first.");
                return false;
            }
            key = File.ReadAllText(auth);
            steamID = Steamworks.SteamUser.GetSteamID().ToString();
            if (ModLoader.CheckSteam())
            {
                string dwl = string.Empty;
                WebClient getdwl = new WebClient();
                getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                try
                {
                    dwl = getdwl.DownloadString($"{ModLoader.serverURL}/meta_u.php?steam={steamID}&key={key}&modid={mod.ID}");
                }
                catch (Exception e)
                {
                    ModConsole.Error($"Failed to verify key");
                    Console.WriteLine(e);
                    return false;
                }
                string[] result = dwl.Split('|');
                if (result[0] == "error")
                {
                    switch (result[1])
                    {
                        case "0":
                            ModConsole.Error($"Invalid or non existent key");
                            break;
                        case "1":
                            ModConsole.Error($"Database error");
                            break;
                        case "2":
                            ModConsole.Error($"User not found");
                            break;
                        case "3":
                            ModConsole.Error($"This Mod ID doesn't exist in database, create it first");
                            break;
                        case "4":
                            ModConsole.Error($"You are not owner of this mod");
                            break;
                        default:
                            ModConsole.Error($"Unknown error");
                            break;
                    }
                    return false;
                }
                else if (result[0] == "ok")
                {
                    return true;
                }
                else
                {
                    ModConsole.Error($"Unknown server respnse");
                    return false;
                }
            }
            else
            {
                ModConsole.Error($"steam auth failed");
                return false;
            }
        }
        public static void AuthMe(string key)
        {
            string steamID;
            if (ModLoader.CheckSteam())
            {
                steamID = Steamworks.SteamUser.GetSteamID().ToString();
                string dwl = string.Empty;
                WebClient getdwl = new WebClient();
                getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                try
                {
                    dwl = getdwl.DownloadString($"{ModLoader.serverURL}/auth.php?steam={steamID}&key={key}");
                }
                catch (Exception e)
                {
                    ModConsole.Error($"Failed to verify key");
                    Console.WriteLine(e);
                }
                string[] result = dwl.Split('|');
                if (result[0] == "error")
                {
                    switch (result[1])
                    {
                        case "0":
                            ModConsole.Error($"Invalid or non existent key");
                            break;
                        case "1":
                            ModConsole.Error($"Database error");
                            break;
                        case "2":
                            ModConsole.Error($"User not found, login on webiste first.");
                            break;
                        default:
                            ModConsole.Error($"Unknown error");
                            break;
                    }
                }
                else if (result[0] == "ok")
                {
                    string path = ModLoader.GetMetadataFolder("auth.bin");
                    if (File.Exists(path)) File.Delete(path);
                    File.WriteAllText(path, key);
                    ModConsole.Print("<color=lime>Key added successfully</color>");
                }
            }
            else
            {
                ModConsole.Error("No valid steam detected");
            }
        }
        public static void CreateMetadata(Mod mod)
        {
            string steamID;
            string key;
            string auth = ModLoader.GetMetadataFolder("auth.bin");
            if (!File.Exists(auth))
            {
                ModConsole.Error("No auth key detected. Please set auth first.");
                return;
            }
            key = File.ReadAllText(auth);
            if (ModLoader.CheckSteam())
            {
                try
                {
                    new Version(mod.Version);
                }
                catch
                {
                    ModConsole.Error($"Invalid version: {mod.Version}{Environment.NewLine}Please use proper version format: (0.0 or 0.0.0 or 0.0.0.0)");
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
                        type = 1
                    };
                    string path = ModLoader.GetMetadataFolder($"{mod.ID}.json");
                    if (File.Exists(path))
                    {
                        ModConsole.Error("Metadata file already exists, to update use update command");
                        return;
                    }
                    string dwl = string.Empty;
                    WebClient getdwl = new WebClient();
                    getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                    try
                    {
                        dwl = getdwl.DownloadString($"{ModLoader.serverURL}/meta_c.php?steam={steamID}&key={key}&modid={mm.modID}&ver={mm.version}&sign={mm.sign}&sid={mm.sid_sign}");
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error($"Failed to verify key");
                        Console.WriteLine(e);
                    }
                    string[] result = dwl.Split('|');
                    if (result[0] == "error")
                    {
                        switch (result[1])
                        {
                            case "0":
                                ModConsole.Error($"Invalid or non existent key");
                                break;
                            case "1":
                                ModConsole.Error($"Database error");
                                break;
                            case "2":
                                ModConsole.Error($"User not found");
                                break;
                            case "3":
                                ModConsole.Error($"This Mod ID already exist in database");
                                break;
                            default:
                                ModConsole.Error($"Unknown error");
                                break;
                        }
                    }
                    else if (result[0] == "ok")
                    {
                        string serializedData = JsonConvert.SerializeObject(mm, Formatting.Indented);
                        File.WriteAllText(path, serializedData);
                        ModConsole.Print("<color=lime>Metadata created successfully</color>");
                        ModConsole.Print("<color=lime>To edit details like description, links, conflicts, etc. go to metadata website.</color>");
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
   /*     public static void UpdateMetadata(Mod mod)
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
        }*/
        public static void UploadUpdateMenu(Mod mod)
        {
            if (!File.Exists(ModLoader.GetMetadataFolder($"{mod.ID}.json")))
            {
                ModConsole.Error("Metadata file doesn't exists, to create use create command");
                return;
            }   
            try
            {
                new Version(mod.Version);
            }
            catch
            {
                ModConsole.Error($"Invalid version: {mod.Version}{Environment.NewLine}Please use proper version format: (0.0 or 0.0.0 or 0.0.0.0)");
                return;
            }
            if (!VerifyOwnership(mod))
                return;
            ModMenuButton mmb = GameObject.Find("MSCLoader Mod Menu").GetComponentInChildren<ModMenuButton>();
            ModMenuView muv = GameObject.Find("MSCLoader Mod Menu").GetComponentInChildren<ModMenuView>();
            if (!mmb.opened)
                mmb.ButtonClicked();
            muv.universalView.FillUpdate(mod);
        }
        public static void UploadUpdate(Mod mod, bool assets, bool references, bool plus12, References[] referencesList = null)
        {
            string steamID;
            string key;
            string auth = ModLoader.GetMetadataFolder("auth.bin");
            if (!File.Exists(auth))
            {
                ModConsole.Error("No auth key detected. Please set auth first.");
                return;
            }
            key = File.ReadAllText(auth);
            if (!VerifyOwnership(mod))
                return;
            if (ModLoader.CheckSteam())
            {
                ModConsole.Print("Preparing Files...");
                steamID = Steamworks.SteamUser.GetSteamID().ToString();
                if (!Directory.Exists(Path.Combine("Updates", "Meta_temp")))
                    Directory.CreateDirectory(Path.Combine("Updates", "Meta_temp"));
                string dir = Path.Combine("Updates", "Meta_temp");

                try
                {
                    File.Copy(mod.fileName, Path.Combine(dir, Path.GetFileName(mod.fileName)));
                    if (assets)
                    {
                        Directory.CreateDirectory(Path.Combine(dir, "Assets"));
                        DirectoryCopy(Path.Combine(ModLoader.AssetsFolder, mod.ID), Path.Combine(Path.Combine(dir, "Assets"), mod.ID), true);
                    }
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                    return;
                }

                try
                {
                    ModConsole.Print("Zipping Files...");
                    ZipFile zip = new ZipFile();
                    if (assets)
                        zip.AddDirectory(Path.Combine(dir, "Assets"), "Assets");
                    zip.AddFile(Path.Combine(dir, Path.GetFileName(mod.fileName)), "");
                    zip.Save(Path.Combine(dir, $"{mod.ID}.zip"));
                    if (assets)
                        Directory.Delete(Path.Combine(dir, "Assets"), true);
                    File.Delete(Path.Combine(dir, Path.GetFileName(mod.fileName)));
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                    return;
                }

                ModLoader.Instance.UploadFileUpdate(mod, plus12, key);
            }
            else
            {
                ModConsole.Error("Steam auth failed");
            }
        }
        public static void UpdateVersionNumber(Mod mod, bool plus12)
        {
            ModConsole.Print("Updating metadata version...");
            string steamID;
            string key;
            string auth = ModLoader.GetMetadataFolder("auth.bin");
            if (!File.Exists(auth))
            {
                ModConsole.Error("No auth key detected. Please set auth first.");
                return;
            }
            if (!VerifyOwnership(mod))
                return;
            key = File.ReadAllText(auth);
            if (ModLoader.CheckSteam())
            {
                ModsManifest umm = mod.metadata;
                Version v1 = new Version(mod.Version);
                Version v2 = new Version(mod.metadata.version);
                if (umm.type != 1)
                {
                    switch (v1.CompareTo(v2))
                    {
                        case 0:
                            ModConsole.Error($"Mod version {mod.Version} is same as current metadata version {mod.metadata.version}, nothing to update.");
                            return;
                        case 1:
                            umm.version = mod.Version;
                            umm.sign = CalculateFileChecksum(mod.fileName);
                            break;
                        case -1:
                            ModConsole.Error($"Mod version {mod.Version} is <b>earlier</b> than current metadata version {mod.metadata.version}, cannot update.");
                            return;
                    }
                }
                if (plus12)
                    umm.type = 5;
                else
                    umm.type = 4;

                steamID = Steamworks.SteamUser.GetSteamID().ToString();
                string dwl = string.Empty;
                WebClient getdwl = new WebClient();
                getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                try
                {
                    dwl = getdwl.DownloadString($"{ModLoader.serverURL}/meta_v.php?steam={steamID}&key={key}&modid={mod.ID}&ver={umm.version}&sign={umm.sign}&type={umm.type}");
                }
                catch (Exception e)
                {
                    ModConsole.Error($"Failed to verify key");
                    Console.WriteLine(e);
                }
                string[] result = dwl.Split('|');
                if (result[0] == "error")
                {
                    switch (result[1])
                    {
                        case "0":
                            ModConsole.Error($"Invalid or non existent key");
                            break;
                        case "1":
                            ModConsole.Error($"Database error");
                            break;
                        case "2":
                            ModConsole.Error($"User not found");
                            break;
                        case "3":
                            ModConsole.Error($"This Mod ID doesn't exist in database");
                            break;
                        case "4":
                            ModConsole.Error($"This is not your mod.");
                            break;
                        default:
                            ModConsole.Error($"Unknown error");
                            break;
                    }
                }
                else if (result[0] == "ok")
                {
                    string metadataFile = ModLoader.GetMetadataFolder($"{mod.ID}.json");
                    string serializedData = JsonConvert.SerializeObject(umm, Formatting.Indented);
                    File.WriteAllText(metadataFile, serializedData);
                    ModConsole.Print("<color=green>Metadata file updated successfully!</color>");
                }
                else
                {
                    ModConsole.Error("Unknown response");
                }
            }
            else
            {
                ModConsole.Error("Steam auth failed");
            }
        }

        internal static void ReadUpdateInfo(ModVersions mv)
        {
            ModLoader.Instance.ModSelfUpdateList = new List<string>();
            for (int i = 0; i < mv.versions.Count; i++)
            {
                Mod mod = ModLoader.GetMod(mv.versions[i].mod_id, true);
                mod.UpdateInfo = mv.versions[i];
                Version v1 = new Version(mv.versions[i].mod_version);
                Version v2 = new Version(mod.Version);
                switch (v1.CompareTo(v2))
                {
                    case 1:
                        mod.hasUpdate = true;
                        ModLoader.Instance.HasUpdateModList.Add(mod);
                        if (mv.versions[i].mod_type == 4 || mv.versions[i].mod_type == 5 || mv.versions[i].mod_type == 6)
                        {
                            ModLoader.Instance.ModSelfUpdateList.Add(mod.ID);
                        }
                        ModConsole.Warning($"Update {mod.ID}");
                        break;
                    case -1:
                        if (mv.versions[i].mod_type == 6)
                        {
                            ModLoader.Instance.ModSelfUpdateList.Add(mod.ID);
                        }
                        break;
                }
                if (mod.metadata != null)
                {
                    if(mod.metadata.rev != mv.versions[i].mod_rev)
                    {
                        //TODO Upd meta
                    }
                }
            }
            ModMenu.instance.UI.GetComponent<ModMenuView>().RefreshTabs();
            if (ModLoader.Instance.ModSelfUpdateList.Count > 0)
            {
                if (!ModLoader.Instance.ModloaderUpdateMessage)
                    ModUI.ShowYesNoMessage($"There are updates to mods that can be updated automatically{Environment.NewLine}Mods: <color=aqua>{string.Join(", ", ModLoader.Instance.ModSelfUpdateList.ToArray())}</color>{Environment.NewLine}{Environment.NewLine}Do you want to download updates now?", "Mod Updates Available", ModLoader.Instance.DownloadModsUpdates);
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
                if (mod.RemMetadata.sign != CalculateFileChecksum(mod.fileName))
                {
                    mod.isDisabled = true;
                    return;
                }
            }
            if (mod.metadata.type == 1 || mod.metadata.type == 4 || mod.metadata.type == 5 || mod.metadata.type == 6)
            {
                if (mod.metadata.icon.iconFileName != null && mod.metadata.icon.iconFileName != string.Empty)
                {
                    if (mod.metadata.icon.isIconRemote)
                    {
                        if (!File.Exists(Path.Combine(ModLoader.MetadataFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName)))
                        {
                            WebClient webClient = new WebClient();
                            webClient.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                            webClient.DownloadFileCompleted += ModIcon_DownloadCompleted;
                            webClient.DownloadFileAsync(new Uri($"{ModLoader.metadataURL}/images/modicons/{mod.metadata.icon.iconFileName}"), Path.Combine(ModLoader.MetadataFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName));
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

        internal static ModsManifest LoadMetadata(Mod mod)
        {
            string metadataFile = ModLoader.GetMetadataFolder($"{mod.ID}.json");
            if (File.Exists(metadataFile))
            {
                string s = File.ReadAllText(metadataFile);
                return JsonConvert.DeserializeObject<ModsManifest>(s);
            }
            return null;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }
            DirectoryInfo[] dirs = dir.GetDirectories();

            Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
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
