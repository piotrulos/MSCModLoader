﻿#if !Mini
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MSCLoader;

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
/*internal class ModsManifest
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
}*/

//Metadata V3
public class MSCLData
{
    public string modID;
    public string description;
    public List<string> links;
    public string icon;
    public MinimumRequirements minimumRequirements;
    public ModConflicts modConflicts;
    public string sign;
    public int type;
    public string msg;
    public int rev;
}
public class MinimumRequirements
{
    public string MSCLoaderVer;
    public int MSCbuildID;
    public bool disableIfVer;
}

public class ModConflicts
{
    public List<string> modIDs;
    public string customMessage;
    public bool disableIfConflict;
}

internal class ModMetadata
{

    public static bool VerifyOwnership(string ID, bool reference, bool sl = false)
    {
        if (!ModLoader.CheckSteam())
        {
            if (!sl)
                ModConsole.Error($"Steam is required to use this feature");
            return false;
        }
        string steamID = Steamworks.SteamUser.GetSteamID().ToString();
        string key;
        string auth = ModLoader.GetMetadataFolder("auth.bin");
        if (!File.Exists(auth))
        {
            if (!sl)
                ModConsole.Error("No auth key detected. Please set auth first.");
            return false;
        }
        key = File.ReadAllText(auth);
        string response = string.Empty;
        response = MSCLInternal.MSCLDataRequest("mscl_owner.php", new Dictionary<string, string> { { "steamID", steamID }, { "key", key }, { "resID", ID }, { "type", reference ? "reference" : "mod" } });
        string[] result = response.Split('|');
        switch (result[0])
        {
            case "error":
                if (!sl)
                    ModConsole.Error(result[1]);
                return false;
            case "ok":
                return true;
            default:
                Console.WriteLine($"Invalid resposne: {response}"); //Log invalid response to output_log.txt
                return false;
        }
    }

    public static void AuthMe(string key)
    {
        if (!ModLoader.CheckSteam())
        {
            ModConsole.Error($"Steam is required to use this feature");
            return;
        }
        string steamID = Steamworks.SteamUser.GetSteamID().ToString();
        string response = string.Empty;
        response = MSCLInternal.MSCLDataRequest("mscl_auth.php", new Dictionary<string, string> { { "steamID", steamID }, { "key", key } });
        string[] result = response.Split('|');
        switch (result[0])
        {
            case "error":
                ModConsole.Error(result[1]);
                break;
            case "ok":
                string path = ModLoader.GetMetadataFolder("auth.bin");
                if (File.Exists(path)) File.Delete(path);
                File.WriteAllText(path, key);
                ModConsole.Print("<color=lime>Successfully authenticated</color>");
                break;
            default:
                Console.WriteLine($"Invalid resposne: {response}"); //Log invalid response to output_log.txt
                break;

        }

    }

    public static void CreateModMetadata(Mod mod)
    {
        if (!ModLoader.CheckSteam())
        {
            ModConsole.Error($"Steam is required to use this feature");
            return;
        }
        string steamID = Steamworks.SteamUser.GetSteamID().ToString();
        string key;
        string auth = ModLoader.GetMetadataFolder("auth.bin");
        if (!File.Exists(auth))
        {
            ModConsole.Error("No auth key detected. Please set auth first.");
            return;
        }
        if (mod.ID.StartsWith("MSCLoader"))
        {
            ModConsole.Error("Not allowed ID pattern, ModID cannot start with MSCLoader.");
            return;
        }
        if (!MSCLInternal.ValidateVersion(mod.Version)) return;
        key = File.ReadAllText(auth);

        MSCLData mscldata = new MSCLData
        {
            modID = mod.ID,
            description = "<i>No description provided...</i>",
            sign = CalculateFileChecksum(mod.fileName),
            type = 1
        };
        string path = ModLoader.GetMetadataFolder($"{mod.ID}.json");
        string response = string.Empty;
        response = MSCLInternal.MSCLDataRequest("mscl_create.php", new Dictionary<string, string> { { "steamID", steamID }, { "key", key }, { "resID", mscldata.modID }, { "version", mod.Version }, { "sign", mscldata.sign }, { "type", "mod" } });

        string[] result = response.Split('|');
        switch (result[0])
        {
            case "error":
                ModConsole.Error(result[1]);
                break;
            case "ok":
                string serializedData = JsonConvert.SerializeObject(mscldata, Formatting.Indented);
                File.WriteAllText(path, serializedData);
                ModConsole.Print("<color=lime>Metadata created successfully</color>");
                ModConsole.Print("<color=lime>To edit details like description, links, conflicts, etc. go to MSCLoader website.</color>");
                ModConsole.Print($"<color=lime>To upload update file just type <b>metadata update {mod.ID}</b>.</color>");
                if (!ModLoader.Instance.checkForUpdatesProgress)
                    ModLoader.Instance.CheckForModsUpd(true);
                break;
            default:
                Console.WriteLine($"Invalid resposne: {response}"); //Log invalid response to output_log.txt
                break;
        }
    }

    public static void CreateReferenceMetadata(References refs)
    {
        if (!ModLoader.CheckSteam())
        {
            ModConsole.Error($"Steam is required to use this feature");
            return;
        }
        string steamID = Steamworks.SteamUser.GetSteamID().ToString();
        string key;
        string auth = ModLoader.GetMetadataFolder("auth.bin");
        if (!File.Exists(auth))
        {
            ModConsole.Error("No auth key detected. Please set auth first.");
            return;
        }
        if (refs.AssemblyID.StartsWith("MSCLoader"))
        {
            ModConsole.Error("Not allowed ID pattern, reference cannot start with MSCLoader.");
            return;
        }
        if (!MSCLInternal.ValidateVersion(refs.AssemblyFileVersion)) return;
        key = File.ReadAllText(auth);


        string response = string.Empty;
        response = MSCLInternal.MSCLDataRequest("mscl_create.php", new Dictionary<string, string> { { "steamID", steamID }, { "key", key }, { "resID", refs.AssemblyID }, { "version", refs.AssemblyFileVersion }, { "type", "reference" } });

        string[] result = response.Split('|');
        switch (result[0])
        {
            case "error":
                ModConsole.Error(result[1]);
                break;
            case "ok":
                ModConsole.Print("<color=lime>Reference registered successfully</color>");
                ModConsole.Print($"<color=lime>To upload update file just type <b>metadata update_ref {refs.AssemblyID}</b>.</color>");
                if (!ModLoader.Instance.checkForUpdatesProgress)
                    ModLoader.Instance.CheckForModsUpd(true);
                break;
            default:
                Console.WriteLine($"Invalid resposne: {response}"); //Log invalid response to output_log.txt
                break;
        }
    }

    public static void UploadUpdateMenu(Mod mod)
    {
        if (!File.Exists(ModLoader.GetMetadataFolder($"{mod.ID}.json")))
        {
            ModConsole.Error("Metadata file doesn't exists, to create use create command");
            return;
        }
        if (!MSCLInternal.ValidateVersion(mod.Version)) return;
        if (!VerifyOwnership(mod.ID, false)) return;
        ModMenuButton mmb = GameObject.Find("MSCLoader Mod Menu").GetComponentInChildren<ModMenuButton>();
        ModMenuView muv = GameObject.Find("MSCLoader Mod Menu").GetComponentInChildren<ModMenuView>();
        if (!mmb.opened)
            mmb.ButtonClicked();
        muv.universalView.FillUpdate(mod);
    }
    public static void UploadUpdate(Mod mod, bool assets, bool references, References[] referencesList = null)
    {
        if (!ModLoader.CheckSteam())
        {
            ModConsole.Error($"Steam is required to use this feature");
            return;
        }
        string key;
        string auth = ModLoader.GetMetadataFolder("auth.bin");
        if (!File.Exists(auth))
        {
            ModConsole.Error("No auth key detected. Please set auth first.");
            return;
        }
        key = File.ReadAllText(auth);
        if (!VerifyOwnership(mod.ID, false)) return;

        if (mod.UpdateInfo.mod_type != 1)
        {
            Version v1 = new Version(mod.Version);
            Version v2 = new Version(mod.UpdateInfo.mod_version);
            if(v1.ToString() != mod.Version)
                ModConsole.Warning($"Your set mod version <b>{mod.Version}</b> is actually read as <b>{v1.ToString()}</b>. Rememeber that stuff like leading zeros are ignored during version compare. You can ignore this warning if you want, this is just reminder.");
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
            Console.WriteLine(e);
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
            if (references)
                Directory.Delete(Path.Combine(dir, "References"), true);
            File.Delete(Path.Combine(dir, Path.GetFileName(mod.fileName)));
            ModConsole.Print("Complete!");

        }
        catch (Exception e)
        {
            ModConsole.Error(e.Message);
            Console.WriteLine(e);
            return;
        }

        ModLoader.Instance.UploadFileUpdate(mod.ID, key, false);
    }

    public static void UploadUpdateRef(References refs)
    {
        if (!ModLoader.CheckSteam())
        {
            ModConsole.Error($"Steam is required to use this feature");
            return;
        }
        string key;
        string auth = ModLoader.GetMetadataFolder("auth.bin");
        if (!File.Exists(auth))
        {
            ModConsole.Error("No auth key detected. Please set auth first.");
            return;
        }
        key = File.ReadAllText(auth);
        if (!VerifyOwnership(refs.AssemblyID, true)) return;
        if (!MSCLInternal.ValidateVersion(refs.AssemblyFileVersion)) return;

        ModConsole.Print("Preparing Files...");
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
            Console.WriteLine(e);
            return;
        }

        try
        {
            ModConsole.Print("Zipping Files...");
            ZipFile zip = new ZipFile();
            zip.AddFile(Path.Combine(dir, Path.GetFileName(refs.FileName)), "");
            zip.Save(Path.Combine(dir, $"{refs.AssemblyID}.zip"));
            File.Delete(Path.Combine(dir, Path.GetFileName(refs.FileName)));
            ModConsole.Print("Complete!");

        }
        catch (Exception e)
        {
            ModConsole.Error(e.Message);
            Console.WriteLine(e);
            return;
        }

        ModLoader.Instance.UploadFileUpdate(refs.AssemblyID, key, true);

    }
    public static void UpdateVersionNumber(Mod mod)
    {
        if (!ModLoader.CheckSteam())
        {
            ModConsole.Error($"Steam is required to use this feature");
            return;
        }
        ModConsole.Print("Updating metadata version...");
        string steamID = Steamworks.SteamUser.GetSteamID().ToString();
        string key;
        string auth = ModLoader.GetMetadataFolder("auth.bin");
        if (!File.Exists(auth))
        {
            ModConsole.Error("No auth key detected. Please set auth first.");
            return;
        }
        if (!VerifyOwnership(mod.ID, false)) return;
        key = File.ReadAllText(auth);

        if (mod.UpdateInfo == null)
        {
            ModConsole.Error($"Update information is null, click 'check for updates' in main settings first");
            return;
        }
        MSCLData umm = mod.metadata;
        Version v1 = new Version(mod.Version);
        Version v2 = new Version(mod.UpdateInfo.mod_version);
        if (v1.ToString() != mod.Version)
            ModConsole.Warning($"Your set mod version <b>{mod.Version}</b> is actually read as <b>{v1.ToString()}</b>. Rememeber that stuff like leading zeros are ignored during version compare. You can ignore this warning if you want, this is just reminder.");
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

        steamID = Steamworks.SteamUser.GetSteamID().ToString();
        string response = string.Empty;
        response = MSCLInternal.MSCLDataRequest("mscl_setversion.php", new Dictionary<string, string> { { "steamID", steamID }, { "key", key }, { "resID", mod.ID }, { "version", mod.Version }, { "sign", umm.sign }, { "type", "mod" } });

        string[] result = response.Split('|');
        switch (result[0])
        {
            case "error":
                ModConsole.Error(result[1]);
                break;
            case "ok":
                string metadataFile = ModLoader.GetMetadataFolder($"{mod.ID}.json");
                string serializedData = JsonConvert.SerializeObject(umm, Formatting.Indented);
                File.WriteAllText(metadataFile, serializedData);
                ModConsole.Print("<color=lime>Metadata file updated successfully!</color>");
                if (!ModLoader.Instance.checkForUpdatesProgress)
                    ModLoader.Instance.CheckForModsUpd(true);
                break;
            default:
                Console.WriteLine($"Invalid resposne: {response}"); //Log invalid response to output_log.txt
                break;
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
        if (mv is null) return;

        for (int i = 0; i < mv.versions.Count; i++)
        {
            try
            {
                Mod mod = ModLoader.GetMod(mv.versions[i].mod_id, true);
                if (mod == null) continue;
                mod.UpdateInfo = mv.versions[i];
                if (mv.versions[i].mod_type == 2 || mv.versions[i].mod_type == 9)
                {
                    mod.isDisabled = true;
                    ModMenu.SaveSettings(mod);
                }
                Version v1 = new Version(mv.versions[i].mod_version);
                Version v2 = new Version(mod.Version);
                switch (v1.CompareTo(v2))
                {
                    case 1:
                        mod.hasUpdate = true;
                        ModLoader.HasUpdateModList.Add(mod);
                        if (mv.versions[i].mod_type == 4 || mv.versions[i].mod_type == 6)
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
            catch (Exception e)
            {
                ModConsole.Error($"Failed to read update info for mod {mv.versions[i].mod_id}");
                ModConsole.Error($"{e.Message}");
                Console.WriteLine(e);
                continue;
            }
        }
        ModMenu.instance.UI.transform.GetChild(0).GetComponent<ModMenuView>().RefreshTabs();

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
                if (refs == null) continue;
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
        ModMenu.instance.UI.transform.GetChild(0).GetComponent<ModMenuView>().RefreshTabs();
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

        if (!ModLoader.Instance.ModloaderUpdateMessage && upd && ModLoader.Instance.updChecked)
        {
            //ModUI.ShowYesNoMessage($"There are updates available that can be updated automatically{Environment.NewLine}{Environment.NewLine}{Upd_list}Do you want to install updates now?", "Updates Available", ModLoader.Instance.DownloadModsUpdates);
            MsgBoxBtn[] btn1 = { ModUI.CreateMessageBoxBtn("YES", ModLoader.Instance.DownloadModsUpdates), ModUI.CreateMessageBoxBtn("NO") };
            MsgBoxBtn[] btn2 = { ModUI.CreateMessageBoxBtn("Show Changelog", ShowChangelogs, new Color32(0, 0, 80, 255), Color.white, true) };
            ModUI.ShowCustomMessage($"There are updates available that can be updated automatically{Environment.NewLine}{Environment.NewLine}{Upd_list}Do you want to install updates now?", "Updates Available", btn1, btn2);
        }
    }
    internal static void ShowChangelogs()
    {
        string dwl = string.Empty;
        WebClient getdwl = new WebClient();
        getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
        List<string> idLists = new List<string>();
        List<string> verLists = new List<string>();
        List<string> nameLists = new List<string>();

        for (int i = 0; i < ModLoader.ModSelfUpdateList.Count; i++)
        {
            Mod m = ModLoader.GetMod(ModLoader.ModSelfUpdateList[i], true);
            idLists.Add(ModLoader.ModSelfUpdateList[i]);
            verLists.Add(m.UpdateInfo.mod_version);
            nameLists.Add(m.Name);
        }
        for (int i = 0; i < ModLoader.RefSelfUpdateList.Count; i++)
        {
            References r = ModLoader.Instance.ReferencesList.Where(x => x.AssemblyID.Equals(ModLoader.RefSelfUpdateList[i])).FirstOrDefault();
            idLists.Add(ModLoader.RefSelfUpdateList[i]);
            verLists.Add(r.UpdateInfo.ref_version);
            nameLists.Add(r.AssemblyTitle);
        }
        try
        {
            dwl = getdwl.DownloadString($"{ModLoader.serverURL}/changelog.php?mods={string.Join(",", idLists.ToArray())}&vers={string.Join(",", verLists.ToArray())}&names={string.Join("|,|", nameLists.ToArray())}");
        }
        catch (Exception e)
        {
            dwl = "<color=red>Failed to download changelog...</color>";
            Console.WriteLine(e);
        }
        ModUI.ShowChangelogWindow(dwl);
    }
    internal static void ReadMetadata(Mod mod)
    {
        if (mod.metadata == null)
            return;
        if (mod.metadata.type == 9)
        {
            //Disabled by reason
            mod.isDisabled = true;
            ModMenu.SaveSettings(mod);
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
            ModMenu.SaveSettings(mod);
            if (!string.IsNullOrEmpty(mod.metadata.msg))
                ModConsole.Error($"Mod <b>{mod.ID}</b> has been disabled by author, Reason: <b>{mod.metadata.msg}</b>");
            else
                ModConsole.Error($"Mod <b>{mod.ID}</b> has been disabled by author, Reason: <i>No reason given...</i>");
            return;
        }
        if (mod.metadata.type == 3)
        {
            if (!ModLoader.CheckSteam())
            {
                if (mod.metadata.sign != CalculateFileChecksum(mod.fileName))
                {
                    if (!VerifyOwnership(mod.ID, false, true))
                        CheckForUpdatedDescription(mod);
                }
            }
            mod.metadata.type = 4;
        }
        if (mod.metadata.type == 1 || mod.metadata.type == 4 || mod.metadata.type == 6)
        {
            if (!string.IsNullOrEmpty(mod.metadata.icon.iconFileName))
            {
                if (mod.metadata.icon.isIconRemote)
                {
                    if (!File.Exists(Path.Combine(ModLoader.MetadataFolder, Path.Combine("Mod Icons", mod.metadata.icon.iconFileName))))
                    {
                        WebClient webClient = new WebClient();
                        webClient.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
                        webClient.DownloadFileCompleted += ModIcon_DownloadCompleted;
                        webClient.DownloadFileAsync(new Uri($"{ModLoader.serverURL}/images/modicons/{mod.metadata.icon.iconFileName}"), Path.Combine(ModLoader.MetadataFolder, Path.Combine("Mod Icons", mod.metadata.icon.iconFileName)));
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

    private static void CheckForUpdatedDescription(Mod mod)
    {
        string[] result;
        result = ModLoader.Instance.DownloadInfo(mod.ID, false).Split('|');
        if (result[0] == "ok")
        {
            using WebClient descritpion = new WebClient();
            descritpion.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
            descritpion.DownloadFileCompleted += DescritpionDownloadCompleted;
            descritpion.DownloadFileAsync(new Uri($"{ModLoader.serverURL}/{result[1]}"), Path.Combine(Path.Combine("Updates", "Mods"), $"{mod}.zip"), mod);
        }
    }

    private static void DescritpionDownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        Mod m = (Mod)e.UserState;
        m.isDisabled = true;
        ModUI.ShowMessage($"<color=yellow>{m.ID}</color> - Fatal crash when trying to read mod data.{Environment.NewLine}{Environment.NewLine}Error details: {Environment.NewLine}<color=aqua>{new BadImageFormatException().Message}</color>", "Crashed");
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
        ModUI.ShowYesNoMessage($"Some of the mods requires mod <color=aqua>{m}</color> to be installed.{Environment.NewLine}{Environment.NewLine}Do you want to download this mod now?", "Missing mods", delegate
        {
            ModLoader.Instance.DownloadRequiredMod(m);
        });
    }
    internal static string CalculateFileChecksum(string fn)
    {
        byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(File.ReadAllBytes(fn));
        return BitConverter.ToString(hash).Replace("-", "");
    }
}
#endif
