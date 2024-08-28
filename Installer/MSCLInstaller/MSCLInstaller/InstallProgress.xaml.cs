using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for InstallProgress.xaml
    /// </summary>
    public partial class InstallProgress : Page
    {
        Progress<int> progress;
        Progress<string> progressText, progressLog;
        
        public InstallProgress()
        {
            InitializeComponent();
            LogBox.Clear();
            progress = new Progress<int>(x => progressBar.Value = x);
            progressText = new Progress<string>(x => ProgressText.Text = x);
            progressLog = new Progress<string>(x => AddLog(x));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddLog($"shit");
        }
        internal void ChangeModsFolder(ModsFolder newfolder)
        {
            AddLog("Changing Mods Folder...");
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
                    break;
            }            
 
        }
        internal async Task ChangeModsFolderAndCopyAsync(string oldPath, ModsFolder newFolder)
        {
            ChangeModsFolder(newFolder);
            AddLog($"Copying mods from {oldPath} to {Storage.modsPath}...");
            progressBar.Maximum = 100;
            await Task.Run(() => ChangeModsFolderAndCopyWork(progress, progressText, progressLog));
        }

        private void ChangeModsFolderAndCopyWork(IProgress<int> progress, IProgress<string> progressText, IProgress<string> progressLog)
        {
            for (int i = 0; i < 100; i++)
            {
                //test
                progress.Report(i);
                progressText.Report($"File test {i}");
                progressLog.Report($"File test {i}");
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
