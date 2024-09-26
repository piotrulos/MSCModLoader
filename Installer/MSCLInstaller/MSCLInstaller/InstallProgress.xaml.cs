using IniParser;
using IniParser.Model;
using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for InstallProgress.xaml
    /// </summary>
    public partial class InstallProgress : Page
    {       
        private Progress<(string text, string log)> progressLog = new();
        private Progress<(int percent, int max, bool isIndeterminate)> progress = new();
        
        public InstallProgress()
        {
            InitializeComponent();
            LogBox.Clear();
            progressLog.ProgressChanged += (_, e) =>
            {
                ProgressText.Text = e.text;
                AddLog(e.log);
            };
            progress.ProgressChanged += (_, e) =>
            {
                progressBar.IsIndeterminate = e.isIndeterminate;
                progressBar.Maximum = e.max;
                progressBar.Value = e.percent;
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddLog($"shit");
        }
        internal async void ChangeModsFolder(ModsFolder newfolder,bool deleteTarget)
        {
            AddLog("Changing Mods Folder...");
            await Task.Run(() => ChangeModsFolderWork(newfolder, deleteTarget, progressLog,progress));
        }
        internal async void ChangeModsFolderAndCopyAsync(string oldPath, ModsFolder newFolder)
        {
            ChangeModsFolder(newFolder, true);
            AddLog($"Copying mods from {oldPath} to {Storage.modsPath}...");
            progressBar.Maximum = 100;
            await Task.Run(() => ChangeModsFolderAndCopyWork(progressLog, progress));
        }
        private void ChangeModsFolderWork(ModsFolder newfolder, bool deleteTarget, IProgress<(string text, string log)> progressLog, IProgress<(int percent, int max, bool isIndeterminate)> progress)
        {
            progress.Report((0, 10, true));
            System.Threading.Thread.Sleep(1500);
            string newCfg = "GF";
            switch (newfolder)
            {
                case ModsFolder.GameFolder:
                    newCfg = "GF";
                    break;
                case ModsFolder.MyDocuments:
                    newCfg = "MD";
                    break;
                case ModsFolder.Appdata:
                    newCfg = "AD";
                    break;
                default:
                    //The fuck happened here?
                    break;
            }
            System.Threading.Thread.Sleep(100);
            if (File.Exists(Path.Combine(Storage.mscPath, "doorstop_config.ini")))
            {
                progressLog.Report(("Reading: doorstop_config.ini","Reading.....doorstop_config.ini"));
                IniData ini = new FileIniDataParser().ReadFile(Path.Combine(Storage.mscPath, "doorstop_config.ini"));
                System.Threading.Thread.Sleep(100);
                ini["MSCLoader"]["mods"] = newCfg;
                progressLog.Report(("Writing: doorstop_config.ini", "Writing.....doorstop_config.ini"));
                new FileIniDataParser().WriteFile(Path.Combine(Storage.mscPath, "doorstop_config.ini"), ini, System.Text.Encoding.ASCII);
                System.Threading.Thread.Sleep(100);
                progressLog.Report(("Done", "Done!"));
            }
            else
            {
                //This is probably super outdated MSCLoader.
                progressLog.Report(("ERROR: doorstop_config.ini does not exist", "ERROR: doorstop_config.ini does not exist"));
                progressLog.Report(("ERROR: doorstop_config.ini does not exist", "Please re-install MSCLoader first."));
                return;
            }
            System.Threading.Thread.Sleep(100);
            if (deleteTarget)
            {
                if (Directory.Exists(Storage.modsPath))
                {
                    progressLog.Report(("Deleting existing folder", $"Deleting.....{Storage.modsPath}"));
                    Directory.Delete(Storage.modsPath, true);
                    System.Threading.Thread.Sleep(100);
                }
                progressLog.Report(("Creating new folder structure", $"Creating new folder structure"));
                progress.Report((0, 5, false));

                System.Threading.Thread.Sleep(100);
                Directory.CreateDirectory(Storage.modsPath);
                progressLog.Report(($"Creating: {Storage.modsPath}", $"Creating.....{Storage.modsPath}"));
                progress.Report((1, 5, false));
                System.Threading.Thread.Sleep(100);
                Directory.CreateDirectory(Path.Combine(Storage.modsPath, "Assets"));
                progressLog.Report(($"Creating Assets folder", $"Creating.....{Path.Combine(Storage.modsPath, "Assets")}"));
                progress.Report((2, 5, false));
                System.Threading.Thread.Sleep(100);
                Directory.CreateDirectory(Path.Combine(Storage.modsPath, "Config"));
                progressLog.Report(($"Creating Config folder", $"Creating.....{Path.Combine(Storage.modsPath, "Config")}"));
                progress.Report((3, 5, false));
                System.Threading.Thread.Sleep(100);
                Directory.CreateDirectory(Path.Combine(Storage.modsPath, "References"));
                progressLog.Report(($"Creating References folder", $"Creating.....{Path.Combine(Storage.modsPath, "References")}"));
                progress.Report((4, 5, false));
            }
            System.Threading.Thread.Sleep(100);
            ExtractFiles(Path.Combine(".", "main_msc.pack"), Path.Combine(".", "temp"), progressLog, progress, true);
            System.Threading.Thread.Sleep(100);
            ExtractFiles(Path.Combine(".", "temp","Mods.zip"), Storage.modsPath, progressLog, progress, false);
            System.Threading.Thread.Sleep(100);
            Directory.Delete(Path.Combine(".", "temp"), true);
            progress.Report((5, 5, false));
            progressLog.Report(("Done", "Done!"));

        }

        private void ChangeModsFolderAndCopyWork(IProgress<(string text, string log)> progressLog, IProgress<(int percent, int max, bool isIndeterminate)> progress)
        {
            progress.Report((0, 10, true));
            System.Threading.Thread.Sleep(1500);
            for (int i = 0; i < 10; i++)
            {
                progressLog.Report(($"File test {i}", $"File test {i}"));
                progress.Report((i, 10, false));
                System.Threading.Thread.Sleep(100);
            }
        }
        
        private void AddLog(string log)
        {            
            LogBox.AppendText($"{log}{Environment.NewLine}");            
            LogBox.ScrollToEnd();
            Dbg.Log(log);
        }
        private bool ExtractFiles(string fn, string target, IProgress<(string text, string log)> progressLog, IProgress<(int percent, int max, bool isIndeterminate)> progress, bool isTemp)
        {
            try
            {
                if (!ZipFile.IsZipFile(fn))
                {
                    progressLog.Report(($"ERROR: Failed read file: {Path.GetFileName(fn)}", $"ERROR: Failed read file.....{Path.GetFileName(fn)}"));
                    return false;
                }
                else
                {
                    ZipFile zip1 = ZipFile.Read(fn);
                    progressLog.Report(($"Reading file: {Path.GetFileName(fn)}", $"Reading file.....{Path.GetFileName(fn)}"));
                    if(!isTemp)
                        progress.Report((0,zip1.Entries.Count,false));
                    System.Threading.Thread.Sleep(100);

                    for(int i = 0; i < zip1.Entries.Count; i++)
                    {
                        ZipEntry zz = zip1[i];
                        if (!isTemp)
                        {
                            progressLog.Report(($"Copying file: {zz.FileName}", $"Copying file.....{zz.FileName}"));
                            progress.Report((i, zip1.Entries.Count, false));
                        }
                        zz.Extract(target, ExtractExistingFileAction.OverwriteSilently);
                        System.Threading.Thread.Sleep(100);

                    }
                    zip1.Dispose();
                }
                
                return true;
            }
            catch (Exception e)
            {
                progressLog.Report(($"Failed read zip file {e.Message}", $"Failed read zip file{Environment.NewLine}{e}"));
                return false;
            }
        }
    }
}
