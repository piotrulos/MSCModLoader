using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    public partial class ModLoader : MonoBehaviour
    {
        void UnpackUpdates()
        {
            ModConsole.Print("Unpacking mod updates...");
            for (int i = 0; i < upd_tmp.Length; i++)
            {
                ModConsole.Print($"Unpacking file {upd_tmp[i]}");
                string zip = upd_tmp[i];
                try
                {
                    if (!ZipFile.IsZipFile(zip))
                    {
                        ModConsole.Error($"Failed read zip file {upd_tmp[i]}");
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
                    File.Delete(upd_tmp[i]);
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                }
            }
            ModUI.ShowMessage("Updating mods finished.", "Update mods");
            ContinueInit();
        }

        private bool cfmuErrored = false;
        private bool cfmuInProgress = false;
        private string cfmuResult = string.Empty;
        internal Slider updateProgress;
        internal Text updateTitle;
        internal Text updateStatus;
        IEnumerator CheckForModsUpdates()
        {
            int modUpdCount = 0;
            Mod[] mod = LoadedMods.Where(x => !x.ID.StartsWith("MSCLoader_")).ToArray();

            updateTitle.text = ("Checking for mod updates...").ToUpper();
            updateProgress.maxValue = mod.Length;
            updateStatus.text = "Connecting...";
            loadingMeta.transform.SetAsLastSibling();
            loadingMeta.SetActive(true);
            mod_aulist = new List<string>();
            for (int i = 0; i < mod.Length; i++)
            {
                updateProgress.value = i + 1;
                updateStatus.text = $"({i + 1}/{mod.Length}) - <color=aqua>{mod[i].Name}</color>";

                WebClient webClient = new WebClient();
                webClient.Headers.Add("user-agent", $"MSCLoader/{MSCLoader_Ver} ({SystemInfo.operatingSystem})");
                webClient.DownloadStringCompleted += cfmuDownloadCompl;
                webClient.DownloadStringAsync(new Uri($"{metadataURL}/man/{mod[i].ID}"));

                cfmuInProgress = true;
                while (cfmuInProgress)
                    yield return null;
                if (cfmuErrored)
                {
                    ModMetadata.ReadMetadata(mod[i]);
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
                                    System.Console.WriteLine("No metadata for " + mod[i].ID);
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
                            mod[i].RemMetadata = JsonConvert.DeserializeObject<ModsManifest>(cfmuResult);
                            Version v1 = new Version(mod[i].RemMetadata.version);
                            Version v2 = new Version(mod[i].Version);
                            switch (v1.CompareTo(v2))
                            {
                                case 0:
                                    if (File.Exists(GetMetadataFolder(string.Format("{0}.json", mod[i].ID))) && !cfmuResult.Equals(File.ReadAllText(GetMetadataFolder(string.Format("{0}.json", mod[i].ID)))))
                                    {
                                        File.WriteAllText(GetMetadataFolder(string.Format("{0}.json", mod[i].ID)), cfmuResult);
                                        mod[i].metadata = mod[i].RemMetadata;
                                    }
                                    else
                                    {
                                        File.WriteAllText(GetMetadataFolder(string.Format("{0}.json", mod[i].ID)), cfmuResult);
                                        mod[i].metadata = mod[i].RemMetadata;
                                    }
                                    break;
                                case 1:
                                    mod[i].hasUpdate = true;
                                    modUpdCount++;
                                    HasUpdateModList.Add(mod[i]);
                                    if (mod[i].RemMetadata.type != 3)
                                    {
                                        if (File.Exists(GetMetadataFolder(string.Format("{0}.json", mod[i].ID))) && !cfmuResult.Equals(File.ReadAllText(GetMetadataFolder(string.Format("{0}.json", mod[i].ID)))))
                                        {
                                            File.WriteAllText(GetMetadataFolder(string.Format("{0}.json", mod[i].ID)), cfmuResult);
                                            mod[i].metadata = mod[i].RemMetadata;
                                        }
                                        else
                                        {
                                            File.WriteAllText(GetMetadataFolder(string.Format("{0}.json", mod[i].ID)), cfmuResult);
                                            mod[i].metadata = mod[i].RemMetadata;
                                        }
                                    }
                                    if (mod[i].RemMetadata.type == 4)
                                    {
                                        mod_aulist.Add(mod[i].ID);
                                    }
                                    break;
                                case -1:
                                    if (mod[i].RemMetadata.sid_sign != SidChecksumCalculator(steamID + mod[i].ID) && mod[i].RemMetadata.type == 3)
                                        File.WriteAllText(GetMetadataFolder(string.Format("{0}.json", mod[i].ID)), cfmuResult);
                                    if (File.Exists(GetMetadataFolder($"{mod[i].ID}.json")) && !cfmuResult.Equals(File.ReadAllText(GetMetadataFolder(string.Format("{0}.json", mod[i].ID)))))
                                    {
                                        mod[i].hasUpdate = true;
                                        modUpdCount++;
                                    }
                                    if (!File.Exists(GetMetadataFolder(string.Format("{0}.json", mod[i].ID))))
                                        File.WriteAllText(GetMetadataFolder(string.Format("{0}.json", mod[i].ID)), cfmuResult);
                                    break;
                            }
                            ModMetadata.ReadMetadata(mod[i]);
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
            if (modUpdCount > 0)
            {
                modUpdates.text = string.Format("<size=20><color=aqua>New Version available for <color=orange>{0}</color> mods.</color></size>", modUpdCount);
                updateStatus.text = string.Format("Done! <color=lime>{0} updates available</color>", modUpdCount);
                if (mod_aulist.Count > 0)
                {
                    ModUI.ShowYesNoMessage($"There are updates to mods that can be updated automatically{Environment.NewLine}Mods: <color=aqua>{string.Join(", ", mod_aulist.ToArray())}</color>{Environment.NewLine}{Environment.NewLine}Do you want to download updates now?", "Mod Updates Available", DownloadUpdates);
                }
            }
            else
                updateStatus.text = string.Format("Done!");
            if (cfmuErrored)
                updateStatus.text = string.Format("<color=red>Connection error!</color>");
            yield return new WaitForSeconds(3f);
            if (!dnsaf)
                loadingMeta.SetActive(false);

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
        void DownloadUpdates()
        {
            if (GetCurrentScene() != CurrentScene.MainMenu)
            {
                ModUI.ShowMessage("You can do that only in main menu.", "Mod Updates");
                return;
            }
            else
            {
                dnsaf = true;
                StartCoroutine(DownloadUpdatesC());
            }
        }
        private bool downloadInProgress = false;
        // private bool downloadErrored = false;
        IEnumerator DownloadUpdatesC()
        {
            updateTitle.text = string.Format("Downloading mod updates...").ToUpper();
            updateProgress.maxValue = 100;
            updateStatus.text = string.Format("Connecting...");
            loadingMeta.SetActive(true);
            yield return null;

            for (int i = 0; i < mod_aulist.Count; i++)
            {
                string mod = mod_aulist[i];
                string dwl;
                WebClient getdwl = new WebClient();
                getdwl.Headers.Add("user-agent", string.Format("MSCLoader/{0} ({1})", MSCLoader_Ver, SystemInfo.operatingSystem));
                // webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted; ;
                dwl = getdwl.DownloadString(string.Format("{0}/dwl.php?core={1}", serverURL, mod));
                string[] result = dwl.Split('|');
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

                }
                else if (result[0] == "ok")
                {
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("user-agent", string.Format("MSCLoader/{0} ({1})", MSCLoader_Ver, SystemInfo.operatingSystem));
                    webClient.DownloadFileCompleted += DownloadModCompleted;
                    webClient.DownloadProgressChanged += DownlaodModProgress;
                    webClient.DownloadFileAsync(new Uri(string.Format("{0}/{1}", serverURL, result[1])), Path.Combine("upd_tmp", $"{mod}.zip"));
                    ModConsole.Print($"Downloading: <color=aqua>{mod}.zip</color>");
                    downloadInProgress = true;
                    while (downloadInProgress)
                    {
                        updateProgress.value = downloadPercentage;
                        updateStatus.text = $"({i + 1}/{mod_aulist.Count}) <color=lime>{mod}.zip</color> [<color=lime>{downloadPercentage}%</color>]";
                        yield return null;
                    }
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    System.Console.WriteLine("Unknown: " + result[0]);
                    ModConsole.Error($"Failed to download updates: Unknown server response.");
                }

            }
            updateProgress.value = 100;
            updateStatus.text = string.Format($"<color=lime>Download Complete</color>");
            ModUI.ShowMessage("To apply updates, please restart game.", "download completed");
            yield return new WaitForSeconds(3f);
            loadingMeta.SetActive(false);
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
