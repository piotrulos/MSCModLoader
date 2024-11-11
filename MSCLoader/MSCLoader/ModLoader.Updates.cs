#if !Mini
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;

namespace MSCLoader;
public partial class ModLoader : MonoBehaviour
{
    void UnpackZipFile(string zip, string target)
    {
        ModConsole.Print($"Unpacking {zip}");
        try
        {
            if (!ZipFile.IsZipFile(zip))
            {
                ModConsole.Error($"Invalid zip file: {zip}");
                File.Delete(zip); //Delete if error (invalid zip file)
            }
            else
            {
                ZipFile zipFile = ZipFile.Read(File.ReadAllBytes(zip));
                foreach (ZipEntry zipEntry in zipFile)
                {
                    ModConsole.Print($"Copying new file: {zipEntry.FileName}");
                    zipEntry.Extract(target, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            File.Delete(zip);
        }
        catch (Exception e)
        {
            ModConsole.Error(e.Message);
            Console.WriteLine(e);
            File.Delete(zip); //Delete if error (corrupted zip file)
        }
    }
    void UnpackUpdates()
    {
        ModConsole.Print("Unpacking updates...");
        for (int i = 0; i < ModsUpdateDir.Length; i++)
        {
            UnpackZipFile(ModsUpdateDir[i], ModsFolder);
        }
        for (int i = 0; i < RefsUpdateDir.Length; i++)
        {
            UnpackZipFile(RefsUpdateDir[i], Path.Combine(ModsFolder, "References"));
        }
        ModUI.ShowMessage("Updating mods finished.", "Update mods");
        ContinueInit();
    }

    private bool cfmuErrored = false;
    private bool cfmuInProgress = false;
    private string cfmuResult = string.Empty;
    internal bool checkForUpdatesProgress = false;
    IEnumerator CheckForRefModUpdates()
    {

        checkForUpdatesProgress = true;    
        canvLoading.SetUpdate("Checking for mod updates...", 0, 3, "Connecting...");

        dnsaf = true;
        bool failed = false;
        yield return new WaitForSeconds(.3f);
        string modlist = string.Join(",", actualModList.Select(x => x.ID).ToArray());
        System.Collections.Specialized.NameValueCollection modvals = new System.Collections.Specialized.NameValueCollection
        {
            { "mods", modlist }
        };
        cfmuInProgress = true;
        WebClient webClient = new WebClient();
     
        webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
        webClient.UploadValuesCompleted += ModsUpdateData;
        webClient.UploadProgressChanged += ModsUpdateDataProgress;
        webClient.UploadValuesAsync(new Uri($"{serverURL}/mods_ver.php"), "POST", modvals);
        
        canvLoading.SetUpdateProgress(1, "Downloading mods update info...");
        while (cfmuInProgress)
            yield return null;
        if (!cfmuErrored)
        {
            if (cfmuResult.StartsWith("error"))
            {
                string[] ed = cfmuResult.Split('|');
                if (ed[0] == "error")
                {
                    switch (ed[1])
                    {
                        case "0":
                            failed = true;
                            ModConsole.Error("Failed to check for mod updates");
                            ModConsole.Error("Invalid request");
                            break;
                        case "1":
                            failed = true;
                            ModConsole.Error("Failed to check for mod updates");
                            ModConsole.Error("Database connection problem");
                            break;
                        case "2":
                            //Not error just empty list
                            ModMetadata.ReadUpdateInfo(new ModVersions());
                            Console.WriteLine("No update info for selected mods");
                            break;
                        default:
                            failed = true;
                            ModConsole.Error("Failed to check for mod updates");
                            ModConsole.Error("Unknown error.");
                            break;
                    }
                }
            }
            else if (cfmuResult.StartsWith("{"))
            {
                ModVersions v = JsonConvert.DeserializeObject<ModVersions>(cfmuResult);
                File.WriteAllText(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "updateInfo.json")), cfmuResult);
                ModMetadata.ReadUpdateInfo(v);
            }
        }
        else
        {
            failed = true;
            if (File.Exists(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "updateInfo.json"))))
            {
                string s = File.ReadAllText(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "updateInfo.json")));
                ModVersions v = JsonConvert.DeserializeObject<ModVersions>(s);
                ModMetadata.ReadUpdateInfo(v);
            }
        }
        canvLoading.SetUpdateTitle("Checking for references updates...");
        yield return null;
        cfmuInProgress = true;
        modlist = string.Join(",", ReferencesList.Select(x => x.AssemblyID).ToArray());
        modvals.Clear();
        modvals.Add("refs", modlist);
        webClient.UploadValuesAsync(new Uri($"{serverURL}/refs_ver.php"), "POST", modvals);
        canvLoading.SetUpdateProgress(2, "Downloading references update info...");
        while (cfmuInProgress)
            yield return null;
        if (!cfmuErrored)
        {
            if (cfmuResult.StartsWith("error"))
            {
                string[] ed = cfmuResult.Split('|');
                if (ed[0] == "error")
                {
                    switch (ed[1])
                    {
                        case "0":
                            failed = true;
                            ModConsole.Error("Failed to check for references updates");
                            ModConsole.Error("Invalid request");
                            break;
                        case "1":
                            failed = true;
                            ModConsole.Error("Failed to check for references updates");
                            ModConsole.Error("Database connection problem");
                            break;
                        case "2":
                            //Not error just empty list
                            ModMetadata.ReadRefUpdateInfo(new RefVersions());
                            Console.WriteLine("No update info for selected references");
                            break;
                        default:
                            failed = true;
                            ModConsole.Error("Failed to check for references updates");
                            ModConsole.Error("Unknown error.");
                            break;
                    }
                }
            }
            else if (cfmuResult.StartsWith("{"))
            {
                RefVersions v = JsonConvert.DeserializeObject<RefVersions>(cfmuResult);
                File.WriteAllText(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "ref_updateInfo.json")), cfmuResult);
                ModMetadata.ReadRefUpdateInfo(v);
            }
        }
        else
        {
            failed = true;
            if (File.Exists(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "ref_updateInfo.json"))))
            {
                string s2 = File.ReadAllText(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "ref_updateInfo.json")));
                RefVersions v2 = JsonConvert.DeserializeObject<RefVersions>(s2);
                ModMetadata.ReadRefUpdateInfo(v2);
            }
        }

        if (failed)
            canvLoading.SetUpdateProgress(3, "<color=red>Failed getting update info</color>");
        else
            canvLoading.SetUpdateProgress(3, "Done!");
        checkForUpdatesProgress = false;
        dnsaf = false;
        if (MetadataUpdateList.Count > 0 && !failed)
        {
            StartCoroutine(DownloadMetadataFiles());
        }
        else
        {
            yield return new WaitForSeconds(3f);
            if (!dnsaf)
                canvLoading.ToggleUpdateUI(false);
        }
    }

    IEnumerator DownloadMetadataFiles()
    {
        while (checkForUpdatesProgress) yield return null;
        checkForUpdatesProgress = true;
        canvLoading.SetUpdate("Downloading metadata files...", 0, MetadataUpdateList.Count, "Connecting...");
        dnsaf = true;
        for (int i = 0; i < MetadataUpdateList.Count; i++)
        {
            canvLoading.SetUpdateProgress(i + 1, $"({i + 1}/{MetadataUpdateList.Count}) - <color=aqua>{MetadataUpdateList[i]}</color>");
            cfmuInProgress = true;
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                webClient.DownloadStringCompleted += cfmuDownloadCompl;
                webClient.DownloadStringAsync(new Uri($"{serverURL}/{metadataURL}{MetadataUpdateList[i]}"));
            }
            while (cfmuInProgress)
                yield return null;
            if (cfmuErrored)
            {
                break;
            }
            if (!string.IsNullOrEmpty(cfmuResult))
            {
                if (cfmuResult.StartsWith("error"))
                {
                    string[] ed = cfmuResult.Split('|');
                    if (ed[0] == "error")
                    {
                        Console.WriteLine($"DownloadMetadataFiles() Error: {ed[1]}");
                        yield return null;
                        continue;
                    }
                }
                else if (cfmuResult.StartsWith("{"))
                {
                    try
                    {
                        Mod m = GetModByID(MetadataUpdateList[i], true);
                        m.metadata = JsonConvert.DeserializeObject<MSCLData>(cfmuResult);
                        ModMetadata.ReadMetadata(m);
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error(e.Message);
                        Console.WriteLine(e);

                    }
                    yield return null;
                    continue;
                }
                else
                {
                    Console.WriteLine("Unknown response: " + cfmuResult);
                    yield return null;
                    continue;
                }
            }
        }
        canvLoading.SetUpdateProgress(MetadataUpdateList.Count, "Done!");
        if (cfmuErrored)
            canvLoading.SetUpdateStatus("<color=red>Connection error!</color>");
        checkForUpdatesProgress = false;
        dnsaf = false;
        MSCLInternal.SaveMSCLDataFile();
        yield return new WaitForSeconds(3f);
        if (!dnsaf)
            canvLoading.ToggleUpdateUI(false);
    }

    private void cfmuDownloadCompl(object sender, DownloadStringCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            cfmuErrored = true;
            cfmuInProgress = false;
            cfmuResult = string.Empty;
            ModConsole.Error("Failed to check for mod updates!");
            ModConsole.Error(e.Error.Message);
            Console.WriteLine(e.Error);
        }
        else
        {
            cfmuErrored = false;
            cfmuResult = e.Result;
            cfmuInProgress = false;
        }
    }

    private bool dnsaf = false;

    internal void DownloadModUpdate(Mod mod)
    {
        if (downloadInProgress)
        {
            ModUI.ShowMessage("Another download is already in progress!", "Mod Updates");
            return;
        }
        else
        {
            StartCoroutine(DownloadSingleUpdateC(1, mod.ID));
        }
    }
    internal void DownloadRefUpdate(References refer)
    {
        if (downloadInProgress)
        {
            ModUI.ShowMessage("Another download is already in progress!", "Mod Updates");
            return;
        }
        else
        {
            StartCoroutine(DownloadSingleUpdateC(3, refer.AssemblyID));
        }
    }
    internal void DownloadRequiredRef(string refer)
    {
        if (downloadInProgress)
        {
            ModUI.ShowMessage("Another download is already in progress!", "Mod Updates");
            return;
        }
        else
        {
            StartCoroutine(DownloadSingleUpdateC(4, refer));
        }
    }
    internal void DownloadRequiredMod(string mod)
    {
        if (downloadInProgress)
        {
            ModUI.ShowMessage("Another download is already in progress!", "Mod Updates");
            return;
        }
        else
        {
            StartCoroutine(DownloadSingleUpdateC(2, mod));
        }
    }
    internal void DownloadModloaderUpdate()
    {
        if (downloadInProgress)
        {
            ModUI.ShowMessage("Another download is already in progress!", "Mod Updates");
            return;
        }
        else
        {
            StartCoroutine(DownloadSingleUpdateC(0));
        }
    }

    internal void DownloadModsUpdates()
    {
        if (downloadInProgress)
        {
            ModUI.ShowMessage("Another download is already in progress!", "Mod Updates");
            return;
        }
        else
        {
            StartCoroutine(DownloadModsUpdatesC());
        }
    }

    internal void UploadFileUpdate(string ID, string ver, string key, bool isRef)
    {
        if (downloadInProgress)
        {
            ModUI.ShowMessage("Another download/upload is already in progress!", "Mod Updates");
            return;
        }
        else
        {
            StartCoroutine(UploadModFile(ID, ver, key, isRef));
        }
    }
    internal bool downloadInProgress = false;
    private bool downloadErrored = false;

    IEnumerator UploadModFile(string ID, string ver, string key, bool isRef)
    {
        dnsaf = true;
        string type = isRef ? "reference" : "mod";
        string tmpFolder = Path.Combine("Updates", "Meta_temp");
        if (CheckSteam())
        {
            if (!File.Exists(Path.Combine(tmpFolder, $"{ID}.zip")))
            {
                ModConsole.Error("Couldn't find zip file.");
            }
            else
            {
                steamID = Steamworks.SteamUser.GetSteamID().ToString();
                using (WebClient Client = new WebClient())
                {
                    Client.Headers.Add("Content-Type", "binary/octet-stream");
                    Client.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                    Client.UploadFileCompleted += UploadModUpdateCompleted;
                    Client.UploadProgressChanged += UploadModUpdateProgress;
                    downloadInProgress = true;
                    Client.UploadFileAsync(new Uri($"{serverURL}/mscl_upfile.php?steam={steamID}&key={key}&resid={ID}&ver={ver}&type={type}"), "POST", Path.Combine(tmpFolder, $"{ID}.zip"));
                }
                ModConsole.Print("Uploading File...");
                canvLoading.SetUpdate("Uploading update file...", 0, 100, "Connecting...");
                while (downloadInProgress)
                {
                    canvLoading.SetUpdateProgress(downloadPercentage, $"(Uploading) <color=aqua>{ID}.zip</color> [<color=lime>{downloadPercentage}%</color>]");
                    yield return null;
                }
                File.Delete(Path.Combine(tmpFolder, $"{ID}.zip"));
                if (!downloadErrored)
                {
                    canvLoading.SetUpdateProgress(100, $"<color=lime>Upload Complete</color>");
                    if (isRef)
                        ModMetadata.UpdateVersionNumberRef(ID);
                    else
                        ModMetadata.UpdateModVersionNumber(GetModByID(ID, true));
                }
            }
        }
        else
        {
            ModConsole.Error("Steam is required to use this feature");
            yield return null;
        }
        dnsaf = false;
        yield return new WaitForSeconds(3f);
        if (!dnsaf)
            canvLoading.ToggleUpdateUI(false);
    }

    private void UploadModUpdateProgress(object sender, UploadProgressChangedEventArgs e)
    {
        downloadInProgress = true;
        downloadPercentage = e.ProgressPercentage;
    }

    private void UploadModUpdateCompleted(object sender, UploadFileCompletedEventArgs e)
    {
        downloadInProgress = false;
        downloadPercentage = 100;
        if (e.Error != null)
        {
            downloadErrored = true;
            ModConsole.Error("Failed to upload mod update");
            ModConsole.Error(e.Error.Message);
            return;
        }
        else
        {
            string s = System.Text.Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
#if DEBUG
            ModConsole.Warning(s);
#endif
            string[] result = s.Split('|');
            if (result[0] == "error")
            {
                ModConsole.Error("Failed to upload mod update");
                ModConsole.Error(result[1]);
                
            }
            else if (result[0] == "ok")
            {
                downloadErrored = false;
            }
            else
            {
                Console.WriteLine(s);
            }
        }
    }

    internal string DownloadInfo(string ID, bool isRef)
    {
        string dwl = string.Empty;
        WebClient getdwl = new WebClient();
        getdwl.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
        try
        {
            if (isRef)
                dwl = getdwl.DownloadString($"{serverURL}/dwlr.php?core={ID}");
            else
                dwl = getdwl.DownloadString($"{serverURL}/dwl.php?core={ID}");
        }
        catch (Exception e)
        {
            ModConsole.Error("Downloading update info failed!");
            Console.WriteLine(e);
            return "error|0";
        }
        return dwl;
    }
    IEnumerator DownloadSingleUpdateC(byte type, string mod = null)
    {
        while (checkForUpdatesProgress)
            yield return null;
        dnsaf = true;
        canvLoading.SetUpdate("Downloading MSCLoader Updates...", 0, 100, "Connecting...");
        switch (type)
        {
            case 0:
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                    webClient.DownloadFileCompleted += DownloadModCompleted;
                    webClient.DownloadProgressChanged += DownlaodModProgress;
                    downloadInProgress = true;
                    webClient.DownloadFileAsync(new Uri($"{serverURL}/dwlc.php"), Path.Combine(Path.Combine("Updates", "Core"), $"update.zip"));
                }
                ModConsole.Print($"Downloading: <color=aqua>update.zip</color>");
                while (downloadInProgress)
                {
                    canvLoading.SetUpdateProgress(downloadPercentage, $"(ModLoader) <color=aqua>update.zip</color> [<color=lime>{downloadPercentage}%</color>]");
                    yield return null;
                }
                yield return new WaitForSeconds(1f);
                canvLoading.SetUpdateProgress(100, $"<color=lime>Download Complete</color>");
                ModUI.ShowMessage("You have to restart the Game for the updates to take effect!", "download completed");
                break;
            case 1:
            case 2:
            case 3:
            case 4:
                if (type == 2)
                    canvLoading.SetUpdateTitle("Downloading required mods...");
                else if (type == 3)
                    canvLoading.SetUpdateTitle("Downloading reference updates...");
                else if (type == 4)
                    canvLoading.SetUpdateTitle("Downloading required reference...");
                else
                    canvLoading.SetUpdateTitle("Downloading mod updates...");
                yield return null;
                bool failed = false;
                string[] result;
                if (type == 3 || type == 4)
                    result = DownloadInfo(mod, true).Split('|');
                else
                    result = DownloadInfo(mod, false).Split('|');
                if (result[0] == "error")
                {
                    switch (result[1])
                    {
                        case "0":
                            ModConsole.Error($"Failed to download updates: Invalid request");
                            break;
                        case "1":
                            ModConsole.Error($"Failed to download updates: Database connection error");
                            break;
                        case "2":
                            if (type != 2 && type != 4)
                                ModConsole.Error($"Failed to download updates: File not found");
                            break;
                        default:
                            ModConsole.Error($"Failed to download updates: Unknown error");
                            break;
                    }
                    failed = true;
                }
                else if (result[0] == "ok")
                {
                    using (WebClient webClient2 = new WebClient())
                    {
                        webClient2.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                        webClient2.DownloadFileCompleted += DownloadModCompleted;
                        webClient2.DownloadProgressChanged += DownlaodModProgress;
                        downloadInProgress = true;
                        if (type == 3 || type == 4)
                            webClient2.DownloadFileAsync(new Uri($"{serverURL}/{result[1]}"), Path.Combine(Path.Combine("Updates", "References"), $"{mod}.zip"));
                        else
                            webClient2.DownloadFileAsync(new Uri($"{serverURL}/{result[1]}"), Path.Combine(Path.Combine("Updates", "Mods"), $"{mod}.zip"));
                    }
                    ModConsole.Print($"Downloading: <color=aqua>{mod}.zip</color>");
                    while (downloadInProgress)
                    {
                        canvLoading.SetUpdateProgress(downloadPercentage, $"<color=aqua>{mod}.zip</color> [<color=lime>{downloadPercentage}%</color>]");
                        yield return null;
                    }
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    Console.WriteLine($"Unknown: {result[0]}");
                    ModConsole.Error($"Failed to download updates: Unknown server response.");
                    failed = true;
                }
                if (!failed)
                {
                    canvLoading.SetUpdateProgress(100, $"<color=lime>Download Complete</color>");
                    if (type == 2)
                        ModUI.ShowMessage("You need to restart the game if you want to install this mod.", "download completed");
                    else if (type == 4)
                        ModUI.ShowMessage("You need to restart the game if you want to install this reference.", "download completed");
                    else
                        ModUI.ShowMessage("You need to restart the game to apply updates.", "download completed");
                }
                else
                {
                    //fallback if no file present
                    if (type == 2)
                        Application.OpenURL($"{serverURL}/redir.php?mod={mod}");
                    else if (type == 4)
                        Application.OpenURL($"{serverURL}/refredir.php?ref={mod}");
                    else
                        canvLoading.SetUpdateStatus( $"<color=red>Download Failed</color>");
                }
                break;
            default:
                yield return null;
                break;
        }
        dnsaf = false;
        yield return new WaitForSeconds(3f);
        if (!dnsaf)
            canvLoading.modUpdateUI.SetActive(false);
    }
    IEnumerator DownloadModsUpdatesC()
    {
        while (checkForUpdatesProgress)
            yield return null;
        dnsaf = true;
        canvLoading.SetUpdate("Downloading mod updates...", 0, 100, "Connecting...");
        yield return null;
        bool failed = false;
        for (int i = 0; i < ModSelfUpdateList.Count; i++)
        {
            string[] result = DownloadInfo(ModSelfUpdateList[i], false).Split('|');
            if (result[0] == "error")
            {
                switch (result[1])
                {
                    case "0":
                        ModConsole.Error($"Failed to download updates: Invalid request");
                        break;
                    case "1":
                        ModConsole.Error($"Failed to download updates: Database connection error");
                        break;
                    case "2":
                        ModConsole.Error($"Failed to download updates: File not found");
                        break;
                    default:
                        ModConsole.Error($"Failed to download updates: Unknown error");
                        break;
                }
                failed = true;
            }
            else if (result[0] == "ok")
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                    webClient.DownloadFileCompleted += DownloadModCompleted;
                    webClient.DownloadProgressChanged += DownlaodModProgress;
                    downloadInProgress = true;
                    webClient.DownloadFileAsync(new Uri($"{serverURL}/{result[1]}"), Path.Combine(Path.Combine("Updates", "Mods"), $"{ModSelfUpdateList[i]}.zip"));
                }
                ModConsole.Print($"Downloading: <color=aqua>{ModSelfUpdateList[i]}.zip</color>");
                while (downloadInProgress)
                {
                    canvLoading.SetUpdateProgress(downloadPercentage, $"({i + 1}/{ModSelfUpdateList.Count}) <color=aqua>{ModSelfUpdateList[i]}.zip</color> [<color=lime>{downloadPercentage}%</color>]");
                    yield return null;
                }
                yield return new WaitForSeconds(1f);
            }
            else
            {
                Console.WriteLine($"Unknown: {result[0]}");
                ModConsole.Error($"Failed to download updates: Unknown server response.");
                failed = true;
            }

        }
        canvLoading.SetUpdateTitle("Downloading references updates...");
        for (int i = 0; i < RefSelfUpdateList.Count; i++)
        {
            string[] result = DownloadInfo(RefSelfUpdateList[i], true).Split('|');
            if (result[0] == "error")
            {
                switch (result[1])
                {
                    case "0":
                        ModConsole.Error($"Failed to download updates: Invalid request");
                        break;
                    case "1":
                        ModConsole.Error($"Failed to download updates: Database connection error");
                        break;
                    case "2":
                        ModConsole.Error($"Failed to download updates: File not found");
                        break;
                    default:
                        ModConsole.Error($"Failed to download updates: Unknown error");
                        break;
                }
                failed = true;
            }
            else if (result[0] == "ok")
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                    webClient.DownloadFileCompleted += DownloadModCompleted;
                    webClient.DownloadProgressChanged += DownlaodModProgress;
                    downloadInProgress = true;
                    webClient.DownloadFileAsync(new Uri($"{serverURL}/{result[1]}"), Path.Combine(Path.Combine("Updates", "References"), $"{RefSelfUpdateList[i]}.zip"));
                }
                ModConsole.Print($"Downloading: <color=aqua>{RefSelfUpdateList[i]}.zip</color>");
                while (downloadInProgress)
                {
                    canvLoading.SetUpdateProgress(downloadPercentage, $"({i + 1}/{RefSelfUpdateList.Count}) <color=aqua>{RefSelfUpdateList[i]}.zip</color> [<color=lime>{downloadPercentage}%</color>]");                    
                    yield return null;
                }
                yield return new WaitForSeconds(1f);
            }
            else
            {
                Console.WriteLine($"Unknown: {result[0]}");
                ModConsole.Error($"Failed to download updates: Unknown server response.");
                failed = true;
            }

        }
        if (!failed)
        {
            canvLoading.SetUpdateProgress(100, $"<color=lime>Download Complete</color>");
            ModUI.ShowMessage("You need to restart the game for the updates to take effect!", "download completed");
        }
        else
        {
            canvLoading.SetUpdateStatus($"<color=red>Download Failed!</color>");
        }
        dnsaf = false;
        yield return new WaitForSeconds(3f);
        if (!dnsaf)
            canvLoading.ToggleUpdateUI(false);
    }
    int downloadPercentage = 0;
    private void DownlaodModProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        downloadInProgress = true;
        downloadPercentage = e.ProgressPercentage;
    }

    private void DownloadModCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        downloadPercentage = 100;
        downloadInProgress = false;
        if (e.Error != null)
        {
            ModConsole.Error("Failed to download update file!");
            ModConsole.Error(e.Error.Message);
            System.Console.WriteLine(e.Error);
        }
        else
        {
            ModConsole.Print($"Download finished");
        }
    }
}
#endif