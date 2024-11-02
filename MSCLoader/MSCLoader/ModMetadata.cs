#if !Mini
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
    public static bool VerifyOwnership(string ID, bool reference, bool sl = false)
    {
        string steamID;
        string key;
        string auth = ModLoader.GetMetadataFolder("auth.bin");
        if (!File.Exists(auth))
        {
            if (!sl)
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
                if (!sl)
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
                        if (!sl)
                            ModConsole.Error($"Invalid or non existent key");
                        break;
                    case "1":
                        if (!sl)
                            ModConsole.Error($"Database error");
                        break;
                    case "2":
                        if (!sl)
                            ModConsole.Error($"User not found");
                        break;
                    case "3":
                        if (!sl)
                            if (reference)
                                ModConsole.Error($"This Reference ID doesn't exist in database, create it first");
                            else
                                ModConsole.Error($"This Mod ID doesn't exist in database, create it first");
                        break;
                    case "4":
                        if (!sl)
                            if (reference)
                                ModConsole.Error($"You are not owner of this Reference");
                            else
                                ModConsole.Error($"You are not owner of this Mod");
                        break;
                    default:
                        if (!sl)
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
                if (!sl)
                    ModConsole.Error($"Unknown server respnse");
                return false;
            }
        }
        else
        {
            if (!sl)
                ModConsole.Error($"steam auth failed");
            return false;
        }
    }
    public static void AuthMe(string key)
    {
        string steamID;
        if (!ModLoader.CheckSteam())
        {
            ModConsole.Error($"Steam is required to use this feature");
            return;
        }
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
            //TODO: Update with current error codes 
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
    public static void CreateMetadata(Mod mod)
    {
        if (!ModLoader.CheckSteam())
        {
            ModConsole.Error($"Steam is required to use this feature");
            return;
        }
        string steamID;
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
        key = File.ReadAllText(auth);
        try
        {
            ModsManifest mm = new ModsManifest
            {
                modID = mod.ID,
                description = "<i>No description provided...</i>",
                sign = CalculateFileChecksum(mod.fileName),
                sid_sign = ModLoader.SidChecksumCalculator(steamID + mod.ID),
                type = 1
            };
            string path = ModLoader.GetMetadataFolder($"{mod.ID}.json");
            string dwl = string.Empty;
            Dictionary<string, string> data = new Dictionary<string, string> { { "steamID", steamID }, { "key",key }, { "resID", mm.modID }, { "version", mod.Version }, { "sign", mm.sign }, { "type", "mod" } };

            System.Collections.Specialized.NameValueCollection modvals = new System.Collections.Specialized.NameValueCollection
            {
                { "msclData", JsonConvert.SerializeObject(data) }
            };
            /////////////// TODO: Make this separate func.
            WebClient getdwl = new WebClient();
            getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
            try
            {
                //  dwl = getdwl.DownloadString($"{ModLoader.serverURL}/meta_c.php?steam={steamID}&key={key}&modid={mm.modID}&ver={mod.Version}&sign={mm.sign}&sid={mm.sid_sign}");
                byte[] sas = getdwl.UploadValues($"{ModLoader.serverURL}/mscl_create.php", "POST", modvals);
                dwl = Encoding.UTF8.GetString(sas, 0, sas.Length);
                ModConsole.Warning(dwl);
                return;
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to create mod metadata");
                Console.WriteLine(e);
                return;
            }
            ///////////////
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
        if (refs.AssemblyID.StartsWith("MSCLoader"))
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
        if (!VerifyOwnership(mod.ID, false))
            return;
        ModMenuButton mmb = GameObject.Find("MSCLoader Mod Menu").GetComponentInChildren<ModMenuButton>();
        ModMenuView muv = GameObject.Find("MSCLoader Mod Menu").GetComponentInChildren<ModMenuView>();
        if (!mmb.opened)
            mmb.ButtonClicked();
        muv.universalView.FillUpdate(mod);
    }
    public static void UploadUpdate(Mod mod, bool assets, bool references, References[] referencesList = null)
    {
        string key;
        string auth = ModLoader.GetMetadataFolder("auth.bin");
        if (!File.Exists(auth))
        {
            ModConsole.Error("No auth key detected. Please set auth first.");
            return;
        }
        key = File.ReadAllText(auth);
        if (!VerifyOwnership(mod.ID, false))
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
                if (references)
                    Directory.Delete(Path.Combine(dir, "References"), true);
                File.Delete(Path.Combine(dir, Path.GetFileName(mod.fileName)));
            }
            catch (Exception e)
            {
                ModConsole.Error(e.Message);
                System.Console.WriteLine(e);
                return;
            }

            ModLoader.Instance.UploadFileUpdate(mod.ID, key, false);
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
        if (!VerifyOwnership(refs.AssemblyID, true))
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
    