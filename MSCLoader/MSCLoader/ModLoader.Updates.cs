#if !Mini
using Ionic.Zip;
using Newtonsoft.Json;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine.UI;

namespace MSCLoader
{
    public partial class ModLoader : MonoBehaviour
    {
        void UnpackUpdates()
        {
            #if !Mini
            ModConsole.Print("Unpacking mod updates...");
            for (int i = 0; i < ModsUpdateDir.Length; i++)
            {
                ModConsole.Print($"Unpacking file {ModsUpdateDir[i]}");
                string zip = ModsUpdateDir[i];
                try
                {
                    if (!ZipFile.IsZipFile(zip))
                    {
                        ModConsole.Error($"Failed read zip file {ModsUpdateDir[i]}");
                    }
                    else
                    {
                        ZipFile zip1 = ZipFile.Read(File.ReadAllBytes(zip));
                        foreach (ZipEntry zz in zip1)
                        {
                            ModConsole.Print($"Copying new file: {zz.FileName}");
                            zz.Extract(ModsFolder, ExtractExistingFileAction.OverwriteSilently);
                        }
                    }
                    File.Delete(ModsUpdateDir[i]);
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                }
            }
            for (int i = 0; i < RefsUpdateDir.Length; i++)
            {
                ModConsole.Print($"Unpacking file {RefsUpdateDir[i]}");
                string zip = RefsUpdateDir[i];
                try
                {
                    if (!ZipFile.IsZipFile(zip))
                    {
                        ModConsole.Error($"Failed read zip file {RefsUpdateDir[i]}");
                    }
                    else
                    {
                        ZipFile zip1 = ZipFile.Read(File.ReadAllBytes(zip));
                        foreach (ZipEntry zz in zip1)
                        {
                            ModConsole.Print($"Copying new file: {zz.FileName}");
                            zz.Extract(Path.Combine(ModsFolder, "References"), ExtractExistingFileAction.OverwriteSilently);
                        }
                    }
                    File.Delete(RefsUpdateDir[i]);
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                }
            }
            ModUI.ShowMessage("Updating mods finished.", "Update mods");
            ContinueInit();
#endif
        }

        private bool cfmuErrored = false;
        private bool cfmuInProgress = false;
        private string cfmuResult = string.Empty;
        internal bool checkForUpdatesProgress = false;
        IEnumerator CheckForRefModUpdates()
        {

            checkForUpdatesProgress = true;
            canvLoading.uTitle.text = "Checking for mod updates...".ToUpper();
            canvLoading.uProgress.maxValue = 3;
            canvLoading.uStatus.text = "Connecting...";
            canvLoading.modUpdateUI.transform.SetAsLastSibling();
            canvLoading.modUpdateUI.SetActive(true);
            dnsaf = true;
            bool failed = false;
            yield return new WaitForSeconds(.3f);
            string modlist = string.Join(",", actualModList.Select(x => x.ID).ToArray());
            System.Collections.Specialized.NameValueCollection modvals = new System.Collections.Specialized.NameValueCollection();
            modvals.Add("mods", modlist);
            cfmuInProgress = true;
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
            webClient.UploadValuesCompleted += ModsUpdateData;
            webClient.UploadProgressChanged += ModsUpdateDataProgress;
            webClient.UploadValuesAsync(new Uri($"{serverURL}/mods_ver.php"), "POST", modvals);
            canvLoading.uProgress.value = 1;
            canvLoading.uStatus.text = "Downloading mods update info...";
            while (cfmuInProgress)
                yield return null;
#if !Mini   
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
                                ModConsole.Error("Failed to check for mods updates");
                                ModConsole.Error("Invalid request");
                                break;
                            case "1":
                                failed = true;
                                ModConsole.Error("Failed to check for mods updates");
                                ModConsole.Error("Database connection problem");
                                break;
                            case "2":
                                //Not error just empty list
                                ModMetadata.ReadUpdateInfo(new ModVersions());
                                Console.WriteLine("No update info for selected mods");
                                break;
                            default:
                                failed = true;
                                ModConsole.Error("Failed to check for mods updates");
                                ModConsole.Error("Unknown error.");
                                break;
                        }
                    }
                }
                else if (cfmuResult.StartsWith("{"))
                {
                    ModVersions v = JsonConvert.DeserializeObject<ModVersions>(cfmuResult);
                    File.WriteAllText(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings","updateInfo.json")), cfmuResult);
                    ModMetadata.ReadUpdateInfo(v);
                }
            }
            else
            {
                failed = true;
                if (File.Exists(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings","updateInfo.json"))))
                {
                    string s = File.ReadAllText(Path.Combine(SettingsFolder, Path.Combine("MSCLoader_Settings","updateInfo.json")));
                    ModVersions v = JsonConvert.DeserializeObject<ModVersions>(s);
                    ModMetadata.ReadUpdateInfo(v);
                }
            }
            canvLoading.uTitle.text = "Checking for References updates...".ToUpper();
            canvLoading.uProgress.value = 2;
            yield return null;
            cfmuInProgress = true;
            modlist = string.Join(",", ReferencesList.Select(x => x.AssemblyID).ToArray());
            modvals.Clear();
            modvals.Add("refs", modlist);
            webClient.UploadValuesAsync(new Uri($"{serverURL}/refs_ver.php"), "POST", modvals);
            canvLoading.uStatus.text = "Downloading references update info...";
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

            canvLoading.uProgress.value = 3;
            if (failed)
                canvLoading.uStatus.text = string.Format("<color=red>Failed getting update info</color>");
            else
                canvLoading.uStatus.text = string.Format("Done!");
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
                    canvLoading.modUpdateUI.SetActive(false);
            }
#endif
        }

        IEnumerator DownloadMetadataFiles()
        {
            while (checkForUpdatesProgress) yield return null;
            checkForUpdatesProgress = true;
            canvLoading.uTitle.text = ("Updating metadata information...").ToUpper();
            canvLoading.uProgress.maxValue = MetadataUpdateList.Count;
            canvLoading.uStatus.text = "Connecting...";
            canvLoading.modUpdateUI.transform.SetAsLastSibling();
            canvLoading.modUpdateUI.SetActive(true);
            dnsaf = true;
            for (int i = 0; i < MetadataUpdateList.Count; i++)
            {
                canvLoading.uProgress.value = i + 1;
                canvLoading.uStatus.text = $"({i + 1}/{MetadataUpdateList.Count}) - <color=aqua>{MetadataUpdateList[i]}</color>";
                cfmuInProgress = true;
                WebClient webClient = new WebClient();
                webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                webClient.DownloadStringCompleted += cfmuDownloadCompl;
                webClient.DownloadStringAsync(new Uri($"{metadataURL}/man_v2/{MetadataUpdateList[i]}"));
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
                            switch (ed[1])
                            {
                                case "0":
                                    System.Console.WriteLine("No metadata for " + MetadataUpdateList[i]);
                                    yield return null;
                                    continue;
                                case "1":
                                    System.Console.WriteLine("Database connection problem");
                                    yield return null;
                                    continue;
                                default:
                                    System.Console.WriteLine("Unknown error.");
                                    yield return null;
                                    continue;
                            }
                        }
                    }
                    else if (cfmuResult.StartsWith("{"))
                    {
                        try
                        {
                            Mod m = GetMod(MetadataUpdateList[i], true);
                            File.WriteAllText(GetMetadataFolder($"{MetadataUpdateList[i]}.json"), cfmuResult);
                            m.metadata = ModMetadata.LoadMetadata(m);
                            ModMetadata.ReadMetadata(m);
                        }
                        catch (Exception e)
                        {
                            ModConsole.Error(e.Message);
                            System.Console.WriteLine(e);

                        }
                        yield return null;
                        continue;
                    }
                    else
                    {
                        System.Console.WriteLine("Unknown response: " + cfmuResult);
                        yield return null;
                        continue;
                    }
                }
            }
            canvLoading.uStatus.text = "Done!";
            if (cfmuErrored)
                canvLoading.uStatus.text = "<color=red>Connection error!</color>";
            checkForUpdatesProgress = false;
            dnsaf = false;
            yield return new WaitForSeconds(3f);
            if (!dnsaf)
                canvLoading.modUpdateUI.SetActive(false);
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
                System.Console.WriteLine(e.Error);
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
                ModUI.ShowMessage("Another download is in progress.", "Mod Updates");
                return;
            }
            else
            {
                StartCoroutine(DownloadSingleUpdateC(1, mod.ID));
            }
        }
        internal void DownloadRefUpdate(References mod)
        {
            if (downloadInProgress)
            {
                ModUI.ShowMessage("Another download is in progress.", "Mod Updates");
                return;
            }
            else
            {
                StartCoroutine(DownloadSingleUpdateC(3, mod.AssemblyID));
            }
        }
        internal void DownloadRequiredMod(string mod)
        {
            if (downloadInProgress)
            {
                ModUI.ShowMessage("Another download is in progress.", "Mod Updates");
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
                ModUI.ShowMessage("Another download is in progress.", "Mod Updates");
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
                ModUI.ShowMessage("Another download is in progress.", "Mod Updates");
                return;
            }
            else
            {
                StartCoroutine(DownloadModsUpdatesC());
            }
        }

        internal void UploadFileUpdate(string ID, string key, bool isRef, bool plus12)
        {
            if (downloadInProgress)
            {
                ModUI.ShowMessage("Another download/upload is in progress.", "Mod Updates");
                return;
            }
            else
            {
                StartCoroutine(UploadModFile(ID, plus12, key, isRef));
            }
        }
        internal bool downloadInProgress = false;
        private bool downloadErrored = false;

        IEnumerator UploadModFile(string ID, bool plus12, string key, bool isRef)
        {
            dnsaf = true;
            if (CheckSteam())
            {
                if (!File.Exists(Path.Combine("Updates", $"Meta_temp\\{ID}.zip")))
                {
                    ModConsole.Error("Couldn't find zip file.");
                }
                else
                {
                    #if !Mini
                    steamID = Steamworks.SteamUser.GetSteamID().ToString();
                    WebClient Client = new WebClient();
                    Client.Headers.Add("Content-Type", "binary/octet-stream");
                    Client.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                    Client.UploadFileCompleted += UploadModUpdateCompleted;
                    Client.UploadProgressChanged += UploadModUpdateProgress;
                    downloadInProgress = true;
                    canvLoading.uProgress.maxValue = 100;
                    if (isRef)
                        Client.UploadFileAsync(new Uri($"{serverURL}/meta_fr.php?steam={steamID}&key={key}&refid={ID}"), "POST", Path.Combine("Updates", $"Meta_temp\\{ID}.zip"));
                    else
                        Client.UploadFileAsync(new Uri($"{serverURL}/meta_f.php?steam={steamID}&key={key}&modid={ID}"), "POST", Path.Combine("Updates", $"Meta_temp\\{ID}.zip"));
                    ModConsole.Print("Uploading File...");
                    canvLoading.uTitle.text = string.Format("Uploading Update File...").ToUpper();
                    canvLoading.uStatus.text = string.Format("Connecting...");
                    canvLoading.modUpdateUI.SetActive(true);
                    while (downloadInProgress)
                    {
                        canvLoading.uProgress.value = downloadPercentage;
                        canvLoading.uStatus.text = $"(Uploading) <color=aqua>{ID}.zip</color> [<color=lime>{downloadPercentage}%</color>]";
                        yield return null;
                    }
                    canvLoading.uProgress.value = 100;
                    File.Delete(Path.Combine("Updates", $"Meta_temp\\{ID}.zip"));
                    if (!downloadErrored)
                    {
                        canvLoading.uStatus.text = string.Format($"<color=lime>Upload Complete</color>");
                        if (isRef)
                            ModMetadata.UpdateVersionNumberRef(ID);
                        else
                            ModMetadata.UpdateVersionNumber(GetMod(ID, true), plus12);
                    }
#endif
                }
            }
            else
            {
                ModConsole.Error("Steam auth failed");
                yield return null;
            }
            dnsaf = false;
            yield return new WaitForSeconds(3f);
            if (!dnsaf)
                canvLoading.modUpdateUI.SetActive(false);
        }

        private void UploadModUpdateProgress(object sender, UploadProgressChangedEventArgs e)
        {
            downloadInProgress = true;
            downloadPercentage = e.ProgressPercentage;
            //throw new NotImplementedException();
        }

        private void UploadModUpdateCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            downloadInProgress = false;
            downloadPercentage = 100;
            if (e.Error != null)
            {
                downloadErrored = true;
                ModConsole.Error("Failed to upload Mod Update");
                ModConsole.Error(e.Error.Message);
                return;
            }
            else
            {
                string s = System.Text.Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                string[] result = s.Split('|');
                if (result[0] == "error")
                {
                    ModConsole.Error("Failed to upload Mod Update");
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
                            ModConsole.Error($"This is not your mod");
                            break;
                        case "5":
                            ModConsole.Error($"File rejected by server");
                            break;
                        default:
                            ModConsole.Error($"Unknown error");
                            break;
                    }
                }
                else if (result[0] == "ok")
                {
                    downloadErrored = false;
                }
                else
                {
                    ModConsole.Error($"Unknown response");
                }
            }
        }

        string DownloadInfo(string ID, bool isRef)
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
                ModConsole.Error("Downloading update info Failed");
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
            switch (type)
            {
                case 0:
                    canvLoading.uTitle.text = "Downloading Modloader Update...".ToUpper();
                    canvLoading.uProgress.maxValue = 100;
                    canvLoading.uStatus.text = "Connecting...";
                    canvLoading.modUpdateUI.SetActive(true);
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                    webClient.DownloadFileCompleted += DownloadModCompleted;
                    webClient.DownloadProgressChanged += DownlaodModProgress;
                    downloadInProgress = true;
                    webClient.DownloadFileAsync(new Uri($"{serverURL}/dwlc.php"), Path.Combine(Path.Combine("Updates", "Core"), $"update.zip"));
                    ModConsole.Print($"Downloading: <color=aqua>update.zip</color>");
                    while (downloadInProgress)
                    {
                        canvLoading.uProgress.value = downloadPercentage;
                        canvLoading.uStatus.text = $"(ModLoader) <color=aqua>update.zip</color> [<color=lime>{downloadPercentage}%</color>]";
                        yield return null;
                    }
                    yield return new WaitForSeconds(1f);
                    canvLoading.uProgress.value = 100;
                    canvLoading.uStatus.text = $"<color=lime>Download Complete</color>";
                    ModUI.ShowMessage("To apply updates, please restart game.", "download completed");
                    break;
                case 1:
                case 2:
                case 3:
                    if (type == 2)
                        canvLoading.uTitle.text = "Downloading required mod...".ToUpper();
                    else if (type == 3)
                        canvLoading.uTitle.text = "Downloading reference update...".ToUpper();
                    else
                        canvLoading.uTitle.text = "Downloading mod update...".ToUpper();
                    canvLoading.uProgress.maxValue = 100;
                    canvLoading.uStatus.text = "Connecting...";
                    canvLoading.modUpdateUI.SetActive(true);
                    yield return null;
                    bool failed = false;
                    string[] result;
                    if (type == 3)
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
                                if (type != 2)
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
                        WebClient webClient2 = new WebClient();
                        webClient2.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                        webClient2.DownloadFileCompleted += DownloadModCompleted;
                        webClient2.DownloadProgressChanged += DownlaodModProgress;
                        downloadInProgress = true;
                        if (type == 3)
                            webClient2.DownloadFileAsync(new Uri($"{serverURL}/{result[1]}"), Path.Combine(Path.Combine("Updates", "References"), $"{mod}.zip"));
                          else
                            webClient2.DownloadFileAsync(new Uri($"{serverURL}/{result[1]}"), Path.Combine(Path.Combine("Updates", "Mods"), $"{mod}.zip"));
                        ModConsole.Print($"Downloading: <color=aqua>{mod}.zip</color>");
                        while (downloadInProgress)
                        {
                            canvLoading.uProgress.value = downloadPercentage;
                            canvLoading.uStatus.text = $"<color=aqua>{mod}.zip</color> [<color=lime>{downloadPercentage}%</color>]";
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
                        canvLoading.uProgress.value = 100;
                        canvLoading.uStatus.text = $"<color=lime>Download Complete</color>";
                        if (type == 2)
                            ModUI.ShowMessage("To install this mod, please restart game.", "download completed");
                        else
                            ModUI.ShowMessage("To apply updates, please restart game.", "download completed");
                    }
                    else
                    {
                        if (type == 2)
                            Application.OpenURL($"{serverURL}/redir.php?mod={mod}");
                        canvLoading.uStatus.text = $"<color=red>Download Failed</color>";
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
            canvLoading.uTitle.text = "Downloading mod updates...".ToUpper();
            canvLoading.uProgress.maxValue = 100;
            canvLoading.uStatus.text = "Connecting...";
            canvLoading.modUpdateUI.SetActive(true);
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
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                    webClient.DownloadFileCompleted += DownloadModCompleted;
                    webClient.DownloadProgressChanged += DownlaodModProgress;
                    downloadInProgress = true;
                    webClient.DownloadFileAsync(new Uri($"{serverURL}/{result[1]}"), Path.Combine(Path.Combine("Updates", "Mods"), $"{ModSelfUpdateList[i]}.zip"));
                    ModConsole.Print($"Downloading: <color=aqua>{ModSelfUpdateList[i]}.zip</color>");
                    while (downloadInProgress)
                    {
                        canvLoading.uProgress.value = downloadPercentage;
                        canvLoading.uStatus.text = $"({i + 1}/{ModSelfUpdateList.Count}) <color=aqua>{ModSelfUpdateList[i]}.zip</color> [<color=lime>{downloadPercentage}%</color>]";
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
            canvLoading.uTitle.text = "Downloading References updates...".ToUpper();
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
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfoFix()})");
                    webClient.DownloadFileCompleted += DownloadModCompleted;
                    webClient.DownloadProgressChanged += DownlaodModProgress;
                    downloadInProgress = true;
                    webClient.DownloadFileAsync(new Uri($"{serverURL}/{result[1]}"), Path.Combine(Path.Combine("Updates", "References"), $"{RefSelfUpdateList[i]}.zip"));
                    ModConsole.Print($"Downloading: <color=aqua>{RefSelfUpdateList[i]}.zip</color>");
                    while (downloadInProgress)
                    {
                        canvLoading.uProgress.value = downloadPercentage;
                        canvLoading.uStatus.text = $"({i + 1}/{RefSelfUpdateList.Count}) <color=aqua>{RefSelfUpdateList[i]}.zip</color> [<color=lime>{downloadPercentage}%</color>]";
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
                canvLoading.uProgress.value = 100;
                canvLoading.uStatus.text = $"<color=lime>Download Complete</color>";
                ModUI.ShowMessage("To apply updates, please restart game.", "download completed");
            }
            else
            {
                canvLoading.uStatus.text = $"<color=red>Download Failed</color>";
            }
            dnsaf = false;
            yield return new WaitForSeconds(3f);
            if(!dnsaf)
                canvLoading.modUpdateUI.SetActive(false);
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
}
