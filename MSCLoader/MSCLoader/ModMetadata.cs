using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    internal class RefVersions
    {
        public List<RefVersion> versions = new List<RefVersion>();
    }
    internal class RefVersion
    {
        public string ref_id;
        public string ref_version;
        public byte ref_type;
    }
    internal class ModsManifest
    {
        public string modID;
        // public string version;
        public string description;
        public ManifestLinks links = new ManifestLinks();
        public ManifestIcon icon = new ManifestIcon();
        public ManifestMinReq minimumRequirements = new ManifestMinReq();
        public ManifestModConflict modConflicts = new ManifestModConflict();
        public ManifestModRequired requiredMods = new ManifestModRequired();
        public string sign;
        public string sid_sign;
        public byte type;
        public string msg = null;
        public int rev = 0;

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
       // public bool isIconUrl = false;
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
        public static bool VerifyOwnership(string ID, bool reference)
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
                    if (reference)
                        dwl = getdwl.DownloadString($"{ModLoader.serverURL}/meta_ur.php?steam={steamID}&key={key}&refid={ID}");
                    else
                        dwl = getdwl.DownloadString($"{ModLoader.serverURL}/meta_u.php?steam={steamID}&key={key}&modid={ID}");
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
                            if (reference)
                                ModConsole.Error($"This Reference ID doesn't exist in database, create it first");
                            else
                                ModConsole.Error($"This Mod ID doesn't exist in database, create it first");
                            break;
                        case "4":
                            if (reference)
                                ModConsole.Error($"You are not owner of this Reference");
                            else
                                ModConsole.Error($"You are not owner of this Mod");
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
            if (mod.ID.StartsWith("MSCLoader"))
            {
                ModConsole.Error("Not allowed ID pattern, ModID cannot start with MSCLoader.");
                return;
            }
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
                        description = "<i>No description provided...</i>",
                        sign = CalculateFileChecksum(mod.fileName),
                        sid_sign = ModLoader.SidChecksumCalculator(steamID+mod.ID),
                        type = 1
                    };
                    string path = ModLoader.GetMetadataFolder($"{mod.ID}.json");
                    string dwl = string.Empty;
                    WebClient getdwl = new WebClient();
                    getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                    try
                    {
                        dwl = getdwl.DownloadString($"{ModLoader.serverURL}/meta_c.php?steam={steamID}&key={key}&modid={mm.modID}&ver={mod.Version}&sign={mm.sign}&sid={mm.sid_sign}");
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error($"Failed to create mod metadata");
                        Console.WriteLine(e);
                        return;
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
                        ModConsole.Print($"<color=lime>To upload update file just type <b>metadata update {mod.ID}</b>.</color>");
                        if (!ModLoader.Instance.checkForUpdatesProgress)
                            ModLoader.Instance.CheckForModsUpd(true);
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
        public static void CreateReference(References refs)
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
            if(refs.AssemblyID.StartsWith("MSCLoader"))
            {
                ModConsole.Error("Not allowed ID pattern, reference cannot start with MSCLoader.");
                return;
            }
            if (ModLoader.CheckSteam())
            {
                steamID = Steamworks.SteamUser.GetSteamID().ToString();
                try
                {
                    string dwl = string.Empty;
                    WebClient getdwl = new WebClient();
                    getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                    try
                    {
                        dwl = getdwl.DownloadString($"{ModLoader.serverURL}/meta_cr.php?steam={steamID}&key={key}&refid={refs.AssemblyID}&ver={refs.AssemblyFileVersion}");
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error($"Failed to create reference");
                        Console.WriteLine(e);
                        return;
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
                                ModConsole.Error($"This Reference ID already exist in database");
                                break;
                            default:
                                ModConsole.Error($"Unknown error");
                                break;
                        }
                    }
                    else if (result[0] == "ok")
                    {
                        ModConsole.Print("<color=lime>Reference registered successfully</color>");
                        ModConsole.Print($"<color=lime>To upload update file just type <b>metadata update_ref {refs.AssemblyID}</b>.</color>");
                        if (!ModLoader.Instance.checkForUpdatesProgress)
                            ModLoader.Instance.CheckForModsUpd(true);
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
            if (!VerifyOwnership(mod.ID,false))
                return;
            ModMenuButton mmb = GameObject.Find("MSCLoader Mod Menu").GetComponentInChildren<ModMenuButton>();
            ModMenuView muv = GameObject.Find("MSCLoader Mod Menu").GetComponentInChildren<ModMenuView>();
            if (!mmb.opened)
                mmb.ButtonClicked();
            muv.universalView.FillUpdate(mod);
        }
        public static void UploadUpdate(Mod mod, bool assets, bool references, bool plus12, References[] referencesList = null)
        {
            string key;
            string auth = ModLoader.GetMetadataFolder("auth.bin");
            if (!File.Exists(auth))
            {
                ModConsole.Error("No auth key detected. Please set auth first.");
                return;
            }
            key = File.ReadAllText(auth);
            if (!VerifyOwnership(mod.ID,false))
                return;
            if (ModLoader.CheckSteam())
            {
                if (mod.UpdateInfo.mod_type != 1)
                {
                    Version v1 = new Version(mod.Version);
                    Version v2 = new Version(mod.UpdateInfo.mod_version);
                    switch (v1.CompareTo(v2))
                    {
                        case 0:
                            ModConsole.Error($"Mod version <b>{mod.Version}</b> is same as current offered version <b>{mod.UpdateInfo.mod_version}</b>, nothing to update.");
                            return;
                        case -1:
                            ModConsole.Error($"Mod version <b>{mod.Version}</b> is <b>LOWER</b> than current offered version <b>{mod.UpdateInfo.mod_version}</b>, cannot update.");
                            return;
                    }
                }
                ModConsole.Print("Preparing Files...");
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
                    if (references)
                    {
                        Directory.CreateDirectory(Path.Combine(dir, "References"));
                        for (int i = 0; i < referencesList.Length; i++)
                        {
                            File.Copy(referencesList[i].FileName, Path.Combine(Path.Combine(dir, "References"), Path.GetFileName(referencesList[i].FileName)));
                        }
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
                    if (references)
                        zip.AddDirectory(Path.Combine(dir, "References"), "References");

                    zip.AddFile(Path.Combine(dir, Path.GetFileName(mod.fileName)), "");
                    zip.Save(Path.Combine(dir, $"{mod.ID}.zip"));
                    if (assets)
                        Directory.Delete(Path.Combine(dir, "Assets"), true);
                    if(references)
                        Directory.Delete(Path.Combine(dir, "References"), true);
                    File.Delete(Path.Combine(dir, Path.GetFileName(mod.fileName)));
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                    return;
                }

                ModLoader.Instance.UploadFileUpdate(mod.ID, key, false, plus12);
            }
            else
            {
                ModConsole.Error("Steam auth failed");
            }
        }

        public static void UploadUpdateRef(References refs)
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
            if (!VerifyOwnership(refs.AssemblyID,true))
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
                    File.Copy(refs.FileName, Path.Combine(dir, Path.GetFileName(refs.FileName)));
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
                    zip.AddFile(Path.Combine(dir, Path.GetFileName(refs.FileName)), "");
                    zip.Save(Path.Combine(dir, $"{refs.AssemblyID}.zip"));
                    File.Delete(Path.Combine(dir, Path.GetFileName(refs.FileName)));
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                    return;
                }

                ModLoader.Instance.UploadFileUpdate(refs.AssemblyID, key, true, false);
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
            if (!VerifyOwnership(mod.ID,false))
                return;
            key = File.ReadAllText(auth);
            if (ModLoader.CheckSteam())
            {
                if(mod.UpdateInfo == null) 
                {
                    ModConsole.Error($"Update information is null, click 'check for updates' in main settings first");
                    return;
                }
                ModsManifest umm = mod.metadata;
                if (mod.UpdateInfo.mod_type != 1)
                {
                    Version v1 = new Version(mod.Version);
                    Version v2 = new Version(mod.UpdateInfo.mod_version);
                    switch (v1.CompareTo(v2))
                    {
                        case 0:
                            ModConsole.Error($"Mod version <b>{mod.Version}</b> is same as current offered version <b>{mod.UpdateInfo.mod_version}</b>, nothing to update.");
                            return;
                        case 1:
                            umm.sign = CalculateFileChecksum(mod.fileName);
                            break;
                        case -1:
                            ModConsole.Error($"Mod version <b>{mod.Version}</b> is <b>LOWER</b> than current offered version <b>{mod.UpdateInfo.mod_version}</b>, cannot update.");
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
                    dwl = getdwl.DownloadString($"{ModLoader.serverURL}/meta_v.php?steam={steamID}&key={key}&modid={mod.ID}&ver={mod.Version}&sign={umm.sign}&type={umm.type}");
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
                            ModConsole.Error($"This Mod ID doesn't exist in database, create it first");

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
                    ModConsole.Print("<color=lime>Metadata file updated successfully!</color>");
                    if (!ModLoader.Instance.checkForUpdatesProgress)
                        ModLoader.Instance.CheckForModsUpd(true);
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
        public static void UpdateVersionNumberRef(string ID)
        {
            References refs = ModLoader.Instance.ReferencesList.Where(w => w.AssemblyID == ID).FirstOrDefault();
            ModConsole.Print("Updating version...");
            string steamID;
            string key;
            string auth = ModLoader.GetMetadataFolder("auth.bin");
            if (!File.Exists(auth))
            {
                ModConsole.Error("No auth key detected. Please set auth first.");
                return;
            }
            if (!VerifyOwnership(ID, true))
                return;
            key = File.ReadAllText(auth);
            if (ModLoader.CheckSteam())
            {
                steamID = Steamworks.SteamUser.GetSteamID().ToString();
                string dwl = string.Empty;
                WebClient getdwl = new WebClient();
                getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                try
                {
                    dwl = getdwl.DownloadString($"{ModLoader.serverURL}/meta_vr.php?steam={steamID}&key={key}&refid={ID}&ver={refs.AssemblyFileVersion}&type=1");
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
                            ModConsole.Error($"This Reference ID doesn't exist in database");
                            break;
                        case "4":
                            ModConsole.Error($"This is not your Reference.");
                            break;
                        default:
                            ModConsole.Error($"Unknown error");
                            break;
                    }
                }
                else if (result[0] == "ok")
                {
                    ModConsole.Print("<color=lime>Reference info updated successfully!</color>");
                    if (!ModLoader.Instance.checkForUpdatesProgress)
                        ModLoader.Instance.CheckForModsUpd(true);
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
            ModLoader.ModSelfUpdateList = new List<string>();
            ModLoader.Instance.MetadataUpdateList = new List<string>();
            ModLoader.HasUpdateModList = new List<Mod>();
            for (int i = 0; i < mv.versions.Count; i++)
            {
                try
                {
                    Mod mod = ModLoader.GetMod(mv.versions[i].mod_id, true);
                    mod.UpdateInfo = mv.versions[i];
                    if (mv.versions[i].mod_type == 2 || mv.versions[i].mod_type == 9)
                        mod.isDisabled = true;
                    Version v1 = new Version(mv.versions[i].mod_version);
                    Version v2 = new Version(mod.Version);
                    switch (v1.CompareTo(v2))
                    {
                        case 1:
                            mod.hasUpdate = true;
                            ModLoader.HasUpdateModList.Add(mod);
                            if (mv.versions[i].mod_type == 4 || mv.versions[i].mod_type == 5 || mv.versions[i].mod_type == 6)
                            {
                                ModLoader.ModSelfUpdateList.Add(mod.ID);
                            }
                            break;
                        case -1:
                            if (mv.versions[i].mod_type == 6)
                            {
                                mod.hasUpdate = true;
                                ModLoader.ModSelfUpdateList.Add(mod.ID);
                                ModLoader.HasUpdateModList.Add(mod);
                            }
                            break;
                    }
                    if (mod.metadata != null)
                    {
                        if (mod.metadata.rev != mv.versions[i].mod_rev)
                        {
                            ModLoader.Instance.MetadataUpdateList.Add(mod.ID);
                        }
                    }
                    else
                    {
                        ModLoader.Instance.MetadataUpdateList.Add(mod.ID);
                    }
                }
                catch(Exception e)
                {
                    ModConsole.Error($"Failed to read update info for mod {mv.versions[i].mod_id}");
                    ModConsole.Error($"{e.Message}");
                    Console.WriteLine(e);
                    continue;
                }
            }
            ModMenu.instance.UI.GetComponent<ModMenuView>().RefreshTabs();

        }
        internal static void ReadRefUpdateInfo(RefVersions mv)
        {
            ModLoader.RefSelfUpdateList = new List<string>();
            ModLoader.HasUpdateRefList = new List<References>();
            for (int i = 0; i < mv.versions.Count; i++)
            {
                try
                {
                    References refs = ModLoader.Instance.ReferencesList.Where(x => x.AssemblyID.Equals(mv.versions[i].ref_id)).FirstOrDefault();
                    refs.UpdateInfo = mv.versions[i];
                    Version v1 = new Version(mv.versions[i].ref_version);
                    Version v2 = new Version(refs.AssemblyFileVersion);
                    switch (v1.CompareTo(v2))
                    {
                        case 1:
                            ModLoader.HasUpdateRefList.Add(refs);
                            if (mv.versions[i].ref_type == 1)
                            {
                                ModLoader.RefSelfUpdateList.Add(refs.AssemblyID);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    ModConsole.Error($"Failed to read update info for reference {mv.versions[i].ref_id}");
                    ModConsole.Error($"{e.Message}");
                    Console.WriteLine(e);
                    continue;
                }
            }
            ModMenu.instance.UI.GetComponent<ModMenuView>().RefreshTabs();
            bool upd = false;
            string Upd_list = string.Empty;
            if (ModLoader.ModSelfUpdateList.Count > 0)
            {
                upd = true;
                Upd_list += $"Mods:{Environment.NewLine}";
                for (int i = 0; i < ModLoader.ModSelfUpdateList.Count; i++)
                {
                    Mod m = ModLoader.GetMod(ModLoader.ModSelfUpdateList[i], true);
                    Upd_list += $"<color=yellow>{m.Name}</color>: <color=aqua>{m.Version}</color> => <color=lime>{m.UpdateInfo.mod_version}</color>{Environment.NewLine}";
                }
                Upd_list += Environment.NewLine;
            }
            if (ModLoader.RefSelfUpdateList.Count > 0)
            {
                upd = true;
                Upd_list += $"References:{Environment.NewLine}";
                for (int i = 0; i < ModLoader.RefSelfUpdateList.Count; i++)
                {
                    References r = ModLoader.Instance.ReferencesList.Where(x => x.AssemblyID.Equals(ModLoader.RefSelfUpdateList[i])).FirstOrDefault();
                    Upd_list += $"<color=yellow>{r.AssemblyTitle}</color>: <color=aqua>{r.AssemblyFileVersion}</color> => <color=lime>{r.UpdateInfo.ref_version}</color>{Environment.NewLine}";
                }
                Upd_list += Environment.NewLine;
            }

            if (!ModLoader.Instance.ModloaderUpdateMessage && upd)
                ModUI.ShowYesNoMessage($"There are updates to mods that can be updated automatically{Environment.NewLine}{Environment.NewLine}{Upd_list}Do you want to install updates now?", "Updates Available", ModLoader.Instance.DownloadModsUpdates);

        }
        internal static void ReadMetadata(Mod mod)
        {
            if (mod.metadata == null)
                return;
            if (mod.metadata.type == 9)
            {
                //Disabled by reason
                mod.isDisabled = true;
                if (!string.IsNullOrEmpty(mod.metadata.msg))
                    ModConsole.Error($"Mod <b>{mod.ID}</b> has been disabled, Reason: <b>{mod.metadata.msg}</b>");
                else
                    ModConsole.Error($"Mod <b>{mod.ID}</b> has been disabled, Reason: <i>No reason given...</i>");
                return;
            }
            if (mod.metadata.type == 2)
            {
                //Disabled by user
                mod.isDisabled = true;
                if (!string.IsNullOrEmpty(mod.metadata.msg))
                    ModConsole.Error($"Mod <b>{mod.ID}</b> has been disabled by author, Reason: <b>{mod.metadata.msg}</b>");
                else
                    ModConsole.Error($"Mod <b>{mod.ID}</b> has been disabled by author, Reason: <i>No reason given...</i>");
                return;
            }
            if (mod.metadata.type == 3)
            {
                if (mod.metadata.sign != CalculateFileChecksum(mod.fileName))
                {
                    mod.isDisabled = true;
                    return;
                }
            }
            if (mod.metadata.type == 1 || mod.metadata.type == 4 || mod.metadata.type == 5 || mod.metadata.type == 6)
            {
                if (!string.IsNullOrEmpty(mod.metadata.icon.iconFileName))
                {
                    if (mod.metadata.icon.isIconRemote)
                    {
                        if (!File.Exists(Path.Combine(ModLoader.MetadataFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName)))
                        {
                            WebClient webClient = new WebClient();
                            webClient.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                            webClient.DownloadFileCompleted += ModIcon_DownloadCompleted;
                            webClient.DownloadFileAsync(new Uri($"{ModLoader.serverURL}/images/modicons/{mod.metadata.icon.iconFileName}"), Path.Combine(ModLoader.MetadataFolder, @"Mod Icons\" + mod.metadata.icon.iconFileName));
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
                                ModConsole.Error($"Mod <b>{mod.ID}</b> requires MSC build at least <b>{mod.metadata.minimumRequirements.MSCbuildID}</b>, your current build is <b>{Steamworks.SteamApps.GetAppBuildId()}</b>. Author marked this as required! Please update your game!");
                            }
                            else
                            {
                                ModConsole.Warning($"Mod <b>{mod.ID}</b> requires MSC build at least <b>{mod.metadata.minimumRequirements.MSCbuildID}</b>, your current build is <b>{Steamworks.SteamApps.GetAppBuildId()}</b>. This may cause issues! Please update your game!");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Can't get buildID compare: {e.Message}");
                    }
                }
                if (!string.IsNullOrEmpty(mod.metadata.minimumRequirements.MSCLoaderVer))
                {
                    Version v1 = new Version(mod.metadata.minimumRequirements.MSCLoaderVer);
                    Version v2 = new Version(ModLoader.MSCLoader_Ver);
                    if (v1.CompareTo(v2) == 1)
                    {
                        if (mod.metadata.minimumRequirements.disableIfVer)
                        {
                            mod.isDisabled = true;
                            ModConsole.Error($"Mod <b>{mod.ID}</b> requires MSCLoader at least <b>{mod.metadata.minimumRequirements.MSCLoaderVer}</b>, your current version is <b>{ModLoader.MSCLoader_Ver}</b>. Author marked this as required!");
                        }
                        else
                        {
                            ModConsole.Warning($"Mod <b>{mod.ID}</b> requires MSCLoader at least <b>{mod.metadata.minimumRequirements.MSCLoaderVer}</b>, your current version is <b>{ModLoader.MSCLoader_Ver}</b>. This may cause issues!");
                        }
                    }
                }
                if (!string.IsNullOrEmpty(mod.metadata.modConflicts.modIDs))
                {
                    string[] modIDs = mod.metadata.modConflicts.modIDs.Trim().Split(',');
                    for (int i = 0; i < modIDs.Length; i++)
                    {
                        if (ModLoader.GetMod(modIDs[i], true) != null)
                        {
                            if (mod.metadata.modConflicts.disableIfConflict)
                            {
                                mod.isDisabled = true;
                                if (!string.IsNullOrEmpty(mod.metadata.modConflicts.customMessage))
                                    ModConsole.Error($"Mod <color=orange><b>{mod.ID}</b></color> is marked as conflict with installed mod <color=orange><b>{modIDs[i]}</b></color>. Author's message: {mod.metadata.modConflicts.customMessage}");
                                else
                                    ModConsole.Error($"Mod <color=orange><b>{mod.ID}</b></color> is marked as conflict with installed mod <color=orange><b>{modIDs[i]}</b></color>.");
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(mod.metadata.modConflicts.customMessage))
                                    ModConsole.Warning($"Mod <color=red><b>{mod.ID}</b></color> is marked as conflict with installed mod <color=red><b>{modIDs[i]}</b></color>. Author's message: {mod.metadata.modConflicts.customMessage}");
                                else
                                    ModConsole.Warning($"Mod <color=red><b>{mod.ID}</b></color> is marked as conflict with installed mod <color=red><b>{modIDs[i]}</b></color>.");
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(mod.metadata.requiredMods.modID))
                {
                    string[] modIDs = mod.metadata.requiredMods.modID.Trim().Split(',');
                    string[] modIDvers = mod.metadata.requiredMods.minVer.Trim().Split(',');

                    for (int i = 0; i < modIDs.Length; i++)
                    {
                        if (ModLoader.GetMod(modIDs[i], true) == null)
                        {
                            mod.isDisabled = true;
                            if (!string.IsNullOrEmpty(mod.metadata.requiredMods.customMessage))
                                ModConsole.Error($"Mod <b>{mod.ID}</b> is missing required mod <b>{modIDs[i]}</b>. Author's message: {mod.metadata.requiredMods.customMessage}");
                            else
                                ModConsole.Error($"Mod <b>{mod.ID}</b> is missing required mod <b>{modIDs[i]}</b>.");
                            OpenRequiredDownloadPage(modIDs[i]);
                        }
                        else
                        {
                            try
                            {
                                Version v1 = new Version(modIDvers[i]);
                                Version v2 = new Version(ModLoader.GetMod(modIDs[i], true).Version);
                                if (v1.CompareTo(v2) == 1)
                                {
                                    if (!string.IsNullOrEmpty(mod.metadata.requiredMods.customMessage))
                                        ModConsole.Warning($"Mod <b>{mod.ID}</b> requires mod <b>{modIDs[i]}</b> to be at least version <b>{v1}</b>. Author's message: {mod.metadata.requiredMods.customMessage}");
                                    else
                                        ModConsole.Warning($"Mod <b>{mod.ID}</b> requires mod <b>{modIDs[i]}</b> to be at least version <b>{v1}</b>.");
                                    OpenRequiredDownloadPage(modIDs[i]);
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
            ModUI.ShowYesNoMessage($"Some of the mods requires mod <color=aqua>{m}</color> to be installed.{Environment.NewLine}{Environment.NewLine}Do you want to open download this mod now?", "Missing mods", delegate {
                ModLoader.Instance.DownloadRequiredMod(m);
            });
        }
        internal static string CalculateFileChecksum(string fn)
        {
            byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(File.ReadAllBytes(fn));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

}
