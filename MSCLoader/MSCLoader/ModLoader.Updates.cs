#if !Mini
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace MSCLoader;
public partial class ModLoader : MonoBehaviour
{
    void UnpackZipFile(string zip, string target)
    {
        ModConsole.Print($"<b><color=yellow>Unpacking:</color></b> <color=aqua>{zip}</color>");
        try
        {
            if (!ZipFile.IsZipFile(zip))
            {
                ModConsole.Error($"Invalid zip file: <b{zip}</b>");
                File.Delete(zip); //Delete if error (invalid zip file)
            }
            else
            {
                ZipFile zipFile = ZipFile.Read(File.ReadAllBytes(zip));
                foreach (ZipEntry zipEntry in zipFile)
                {
                    ModConsole.Print($"<b><color=yellow>Copying new file:</color></b> <color=aqua>{zipEntry.FileName}</color>");
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
        ModConsole.Print("<color=yellow>Unpacking updates...</color>");
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

        MSCLInternal.MSCLRequestAsync("mscl_versions.php", modvals);
        canvLoading.SetUpdateProgress(1, "Downloading mods update info...");
        while (MSCLInternal.AsyncRequestInProgress)
            yield return null;
        if (!MSCLInternal.AsyncRequestError)
        {
            string requestResult = MSCLInternal.AsyncRequestResult;
            if (requestResult.StartsWith("error"))
            {
                string[] ed = requestResult.Split('|');
                if (ed[0] == "error")
                {
                    ModConsole.Error("Failed to check for mod updates");
                    ModConsole.Error(ed[1]);
                }
                failed = true;
            }
            else if (requestResult.StartsWith("{"))
            {
                ModVersions v = JsonConvert.DeserializeObject<ModVersions>(requestResult);
                File.WriteAllText(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "updateInfo.json")), requestResult);
                ModMetadata.ReadUpdateInfo(v);
            }
            else
            {
                Console.WriteLine(requestResult);
                ModMetadata.ReadUpdateInfo(null);
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
        modlist = string.Join(",", ReferencesList.Select(x => x.AssemblyID).ToArray());
        modvals.Clear();
        modvals.Add("references", modlist);
        MSCLInternal.MSCLRequestAsync("mscl_versions.php", modvals);
        canvLoading.SetUpdateProgress(2, "Downloading references update info...");
        while (MSCLInternal.AsyncRequestInProgress)
            yield return null;
        if (!MSCLInternal.AsyncRequestError)
        {
            string requestResult = MSCLInternal.AsyncRequestResult;
            if (requestResult.StartsWith("error"))
            {
                string[] ed = requestResult.Split('|');
                if (ed[0] == "error")
                {
                    ModConsole.Error("Failed to check for mod updates");
                    ModConsole.Error(ed[1]);
                }
                failed = true;
            }
            else if (requestResult.StartsWith("{"))
            {
                RefVersions v = JsonConvert.DeserializeObject<RefVersions>(requestResult);
                File.WriteAllText(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings", "ref_updateInfo.json")), requestResult);
                ModMetadata.ReadRefUpdateInfo(v);
            }
            else
            {
                Console.WriteLine(requestResult);
                ModMetadata.ReadRefUpdateInfo(null);
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
        ModMetadata.GetSelfUpdateList();
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
        bool cfmuErrored = false, cfmuInProgress;
        string cfmuResult = string.Empty;
        canvLoading.SetUpdate("Downloading metadata files...", 0, MetadataUpdateList.Count, "Connecting...");
        dnsaf = true;
        for (int i = 0; i < MetadataUpdateList.Count; i++)
        {
            canvLoading.SetUpdateProgress(i + 1, $"({i + 1}/{MetadataUpdateList.Count}) - <color=aqua>{MetadataUpdateList[i]}</color>");
            cfmuInProgress = true;
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                webClient.DownloadStringCompleted += (sender, e) =>
                {
                    cfmuInProgress = false;
                    if (e.Error != null)
                    {
                        cfmuErrored = true;
                        cfmuResult = string.Empty;
                        ModConsole.Error("Failed to check for mod updates!");
                        ModConsole.Error(e.Error.Message);
                        Console.WriteLine(e.Error);
                    }
                    else
                    {
                        cfmuResult = e.Result;
                    }
                };
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
            StartCoroutine(DownloadSingleUpdateC(2, refer.AssemblyID));
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
        ModConsole.Print("It may take a few minutes for the update file to be available for download. You can see the file status on MSCLoader website");
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

    IEnumerator DownloadSingleUpdateC(byte type, string mod = null)
    {
        while (checkForUpdatesProgress)
            yield return null;
        while (downloadInProgress)
            yield return null;
        dnsaf = true;
        canvLoading.SetUpdate("Downloading MSCLoader Update...", 0, 100, "Connecting...");
        switch (type)
        {
            case 0:
                ModConsole.Print($"Downloading: <color=aqua>update.zip</color>");
                DownloadFile("mscl_download.php?type=core&id=msc", Path.Combine(Path.Combine("Updates", "Core"), "update.zip"));
                while (downloadInProgress)
                {
                    canvLoading.SetUpdateProgress(downloadPercentage, $"(ModLoader) <color=aqua>update.zip</color> [<color=lime>{downloadPercentage}%</color>]");
                    yield return null;
                }
                yield return new WaitForSeconds(1f);
                canvLoading.SetUpdateProgress(100, $"<color=lime>Download Complete</color>");
                ModUI.ShowMessage("You have to restart the game for the updates to take effect!", "download completed");
                break;
            case 1:
            case 2:
                if (type == 1)
                {
                    canvLoading.SetUpdateTitle("Downloading mod update...");
                    DownloadFile($"mscl_download.php?type=mod&id={mod}", Path.Combine(Path.Combine("Updates", "Mods"), $"{mod}.zip"));
                }
                if (type == 2)
                {
                    canvLoading.SetUpdateTitle("Downloading reference update...");
                    DownloadFile($"mscl_download.php?type=reference&id={mod}", Path.Combine(Path.Combine("Updates", "References"), $"{mod}.zip"));
                }
                yield return null;
                ModConsole.Print($"Downloading: <color=aqua>{mod}.zip</color>");
                while (downloadInProgress)
                {
                    canvLoading.SetUpdateProgress(downloadPercentage, $"<color=aqua>{mod}.zip</color> [<color=lime>{downloadPercentage}%</color>]");
                    yield return null;
                }
                canvLoading.SetUpdateProgress(100, $"<color=lime>Download Complete</color>");
                ModUI.ShowMessage("You have to restart the game for the updates to take effect!", "download completed");
                yield return new WaitForSeconds(1f);
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
        while (downloadInProgress)
            yield return null;
        dnsaf = true;
        canvLoading.SetUpdate("Downloading mod updates...", 0, 100, "Connecting...");
        yield return null;
        for (int i = 0; i < ModSelfUpdateList.Count; i++)
        {
            ModConsole.Print($"Downloading: <color=aqua>{ModSelfUpdateList[i]}.zip</color>");
            DownloadFile($"mscl_download.php?type=mod&id={ModSelfUpdateList[i]}", Path.Combine(Path.Combine("Updates", "Mods"), $"{ModSelfUpdateList[i]}.zip"));
            while (downloadInProgress)
            {
                canvLoading.SetUpdateProgress(downloadPercentage, $"({i + 1}/{ModSelfUpdateList.Count}) <color=aqua>{ModSelfUpdateList[i]}.zip</color> [<color=lime>{downloadPercentage}%</color>]");
                yield return null;
            }
            yield return new WaitForSeconds(1f);
        }
        canvLoading.SetUpdateTitle("Downloading references updates...");
        for (int i = 0; i < RefSelfUpdateList.Count; i++)
        {
            ModConsole.Print($"Downloading: <color=aqua>{RefSelfUpdateList[i]}.zip</color>");
            DownloadFile($"mscl_download.php?type=reference&id={RefSelfUpdateList[i]}", Path.Combine(Path.Combine("Updates", "References"), $"{RefSelfUpdateList[i]}.zip"));
            while (downloadInProgress)
            {
                canvLoading.SetUpdateProgress(downloadPercentage, $"({i + 1}/{RefSelfUpdateList.Count}) <color=aqua>{RefSelfUpdateList[i]}.zip</color> [<color=lime>{downloadPercentage}%</color>]");
                yield return null;
            }
            yield return new WaitForSeconds(1f);
        }
        canvLoading.SetUpdateProgress(100, $"<color=lime>Download Complete</color>");
        ModUI.ShowMessage("You need to restart the game for the updates to take effect!", "download completed");
        dnsaf = false;
        yield return new WaitForSeconds(3f);
        if (!dnsaf)
            canvLoading.ToggleUpdateUI(false);
    }
    int downloadPercentage = 0;
    internal void DownloadFile(string url, string path, bool desc = false)
    {
        using (WebClient webClient = new WebClient())
        {
            webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
            if (desc)
                webClient.DownloadFileCompleted += ModMetadata.DescritpionDownloadCompleted;
            else
                webClient.DownloadFileCompleted += DownloadModCompleted;
            webClient.DownloadProgressChanged += DownlaodModProgress;
            downloadInProgress = true;
            webClient.DownloadFileAsync(new Uri($"{serverURL}/{url}"), path, path);
        }
    }
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
            ModConsole.Error("Failed to download file!");
            ModConsole.Error(e.Error.Message);
            Console.WriteLine(e.Error);
            if(File.Exists(e.UserState.ToString()))
                File.Delete(e.UserState.ToString());
        }
        else
        {
            ModConsole.Print($"Download finished");
        }
    }

    private void DownloadRequiredFiles(List<string> refs, List<string> mods)
    {
        if (refs.Count == 0 && mods.Count == 0) return;
        ModUI.ShowYesNoMessage($"<color=yellow>Installed mods are missing required files!</color> {Environment.NewLine}{Environment.NewLine}{(refs.Count != 0 ? $"Missing References: <color=aqua>{string.Join(", ", refs.ToArray())}</color>{Environment.NewLine}" : "")}{(mods.Count != 0 ? $"Missing Mods: <color=aqua>{string.Join(", ", mods.ToArray())}</color>{Environment.NewLine}" : "")} {Environment.NewLine}These files are REQUIRED to run the mods correctly!{Environment.NewLine}Do you want to download these files now?", "Missing required files", delegate
        {
            StartCoroutine(DownloadRequiredFilesC(refs, mods));
        });
    }
    IEnumerator DownloadRequiredFilesC(List<string> refs, List<string> mods)
    {
        while (checkForUpdatesProgress)
            yield return null;
        while (downloadInProgress)
            yield return null;
        int totalCount = refs.Count + mods.Count;
        int currentCount = 0;
        dnsaf = true;
        canvLoading.SetUpdate("Downloading required files...", 0, 100, "Connecting...");
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < mods.Count; i++)
        {
            ModConsole.Print($"Downloading: <color=aqua>{mods[i]}.zip</color>");
            DownloadFile($"mscl_download.php?type=mod&id={mods[i]}", Path.Combine(Path.Combine("Updates", "Mods"), $"{mods[i]}.zip"));
            currentCount++;
            while (downloadInProgress)
            {
                canvLoading.SetUpdateProgress(downloadPercentage, $"({currentCount}/{totalCount}) <color=aqua>{mods[i]}.zip</color> [<color=lime>{downloadPercentage}%</color>]");
                yield return null;
            }
            yield return new WaitForSeconds(1f);
        }

        for (int i = 0; i < refs.Count; i++)
        {
            ModConsole.Print($"Downloading: <color=aqua>{refs[i]}.zip</color>");
            DownloadFile($"mscl_download.php?type=reference&id={refs[i]}", Path.Combine(Path.Combine("Updates", "References"), $"{refs[i]}.zip"));
            currentCount++;
            while (downloadInProgress)
            {
                canvLoading.SetUpdateProgress(downloadPercentage, $"({currentCount}/{totalCount}) <color=aqua>{refs[i]}.zip</color> [<color=lime>{downloadPercentage}%</color>]");
                yield return null;
            }
            yield return new WaitForSeconds(1f);
        }
        canvLoading.SetUpdateProgress(100, $"<color=lime>Download Complete</color>");
        ModUI.ShowMessage("You need to restart the game for the updates to take effect!", "download completed");
        dnsaf = false;
        yield return new WaitForSeconds(3f);
        if (!dnsaf)
            canvLoading.ToggleUpdateUI(false);
    }
}
#endif