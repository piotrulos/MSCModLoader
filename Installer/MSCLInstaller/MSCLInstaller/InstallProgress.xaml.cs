using IniParser;
using IniParser.Model;
using System;
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
        Progress<(int percent, string text, string log)> progress = new Progress<(int percent, string text, string log)>();
        
        public InstallProgress()
        {
            InitializeComponent();
            LogBox.Clear();
            progress.ProgressChanged += (_, e) =>
            {
                progressBar.Value = e.percent;
                ProgressText.Text = e.text;
                AddLog(e.log);
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddLog($"shit");
        }
        internal void ChangeModsFolder(ModsFolder newfolder)
        {
            AddLog("Changing Mods Folder...");

        }
        internal async Task ChangeModsFolderAndCopyAsync(string oldPath, ModsFolder newFolder)
        {
            ChangeModsFolder(newFolder);
            AddLog($"Copying mods from {oldPath} to {Storage.modsPath}...");
            progressBar.Maximum = 100;
            await Task.Run(() => ChangeModsFolderAndCopyWork(progress));
        }
        private void ChangeModsFolderWork(ModsFolder newfolder, IProgress<(int percent, string text, string log)> progress)
        {
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
            if (File.Exists(Path.Combine(Storage.mscPath, "doorstop_config.ini")))
            {
                AddLog("Reading.....doorstop_config.ini");
                IniData ini = new FileIniDataParser().ReadFile(Path.Combine(Storage.mscPath, "doorstop_config.ini"));
                ini["MSCLoader"]["mods"] = newCfg;
                AddLog("Writing.....doorstop_config.ini");
                new FileIniDataParser().WriteFile(Path.Combine(Storage.mscPath, "doorstop_config.ini"), ini);
                AddLog("Done!");
            }
            else
            {
                //This is probably super outdated MSCLoader.
                AddLog("ERROR: doorstop_config.ini does not exist.");
                AddLog("Please re-install MSCLoader first.");
            }
            for (int i = 0; i < 100; i++)
            {
                //test
                progress.Report((i, $"File test {i}", $"File test {i}"));
                System.Threading.Thread.Sleep(500);
            }
        }

        private void ChangeModsFolderAndCopyWork(IProgress<(int percent, string text, string log)> progress)
        {

            for (int i = 0; i < 10; i++)
            {
                progress.Report((i, $"File test {i}", $"File test {i}"));
                System.Threading.Thread.Sleep(500);
            }
        }

        private void AddLog(string log)
        {
            LogBox.AppendText($"{log}{Environment.NewLine}");
            LogBox.ScrollToEnd();
            Dbg.Log(log);
        }
    }
}
