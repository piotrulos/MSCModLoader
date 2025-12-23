using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for SelectGameFolder.xaml
    /// </summary>
    public partial class SelectGameFolder : Page
    {
        MainWindow main;
        string exeName = "mysummercar.exe";
        string savePath = "MSCFolder.txt";
        public SelectGameFolder()
        {
            InitializeComponent();
        }
        public void Init(MainWindow m)
        {
            main = m;
            switch (Storage.selectedGame)
            {
                case Game.MSC:
                    main.Title += " - My Summer Car";
                    exeName = "mysummercar.exe";
                    savePath = "MSCFolder.txt";
                    break;
                case Game.MSC_IMA:
                    main.Title += " - My Summer Car (Community)";
                    break;
                case Game.MWC:
                    main.Title += " - My Winter Car";
                    exeName = "mywintercar.exe"; //TODO: validate
                    savePath = "MWCFolder.txt";
                    break;
            }
            FindOnSteam();
            if (string.IsNullOrEmpty(Storage.gamePath))
            {
                if (File.Exists(savePath))
                {
                    string mscPath = File.ReadAllText(savePath);
                    if (Directory.Exists(mscPath))
                    {
                        Storage.gamePath = mscPath;
                        MSCFolder.Text = Storage.gamePath;
                        Dbg.Log($"Loaded saved {Storage.selectedGame} Folder: {mscPath}");
                        GameInfo();
                    }
                    else
                    {
                        Dbg.Log($"Saved {Storage.selectedGame} Folder, doesn't exists: {mscPath}");
                        try
                        {
                            File.Delete(savePath);
                        }
                        catch (Exception e)
                        {
                            Dbg.Log($"Error deleting {savePath}", true, true);
                            Dbg.Log(e.ToString());
                        }
                    }
                }
            }

        }

        private void FindOnSteam()
        {
            Dbg.Log($"Trying get {Storage.selectedGame} path from steam...");
            string steamPath = null;
            RegistryKey steam = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Valve\\Steam");
            if (steam == null)
            {
                Dbg.Log("Error: Steam not found");
                return;
            }
            steamPath = steam.GetValue("InstallPath").ToString();
            if (string.IsNullOrEmpty(steamPath))
            {
                Dbg.Log("Error: Steam InstallPath is null! Trying SteamService installpath_default");
                RegistryKey steam2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Valve\\SteamService");
                if (steam2 == null)
                {
                    Dbg.Log("Error: Steam is most likely not correctly installed");
                    return;
                }

                steamPath = steam2.GetValue("installpath_default").ToString();
                if (string.IsNullOrEmpty(steamPath))
                {
                    Dbg.Log($"Error: SteamService installpath_default is null! Failed to find {Storage.selectedGame} path from steam");
                    return;
                }

            }

            string msc = Path.Combine(steamPath, "steamapps", "common", Storage.selectedGame == Game.MSC ? "My Summer Car" : "My Winter Car"); //TODO: validate path
            if (File.Exists(Path.Combine(msc, exeName)))
            {
                Storage.gamePath = msc;
            }
            else
            {
                Dbg.Log($"Trying get {Storage.selectedGame} from additional library folders...");
                string steamLib = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
                if (File.Exists(Path.Combine(steamLib)))
                {
                    string[] lib = File.ReadAllLines(steamLib);
                    foreach (string s in lib)
                    {
                        if (s.Trim().StartsWith("\"path\""))
                        {
                            string p = Path.GetFullPath(s.Trim().Split('"')[3]);
                            if (string.IsNullOrEmpty(p))
                            {
                                Dbg.Log($"Warn: Got invalid path in libraryfolders.vdf: {s}");
                                continue;
                            }
                            string msclib = Path.Combine(p, "steamapps", "common", Storage.selectedGame == Game.MSC ? "My Summer Car" : "My Winter Car");
                            if (File.Exists(Path.Combine(msclib, exeName)))
                            {
                                Storage.gamePath = msclib;
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(Storage.gamePath))
            {
                MSCFolder.Text = Storage.gamePath;
                Dbg.Log($"Got {Storage.selectedGame} path from steam: {Storage.gamePath}");
                GameInfo();
            }
        }



        private void GameInfo()
        {
            string game = Path.Combine(Storage.gamePath, exeName);
            if (File.Exists(game))
            {
                DetectedGameText.Text = string.Empty;
                switch (MD5FileHashes.MD5HashFile(game))
                {
                    case MD5FileHashes.msc64:
                    case MD5FileHashes.msc64d9:
                        DetectedGameText.Inlines.Add(new Run("Detected: ") { FontWeight = FontWeights.Bold });
                        DetectedGameText.Inlines.Add(new Bold(new Run("My Summer Car (64-bit)") { Foreground = Brushes.LightGreen }));
                        break;
                    case MD5FileHashes.msc32:
                        Storage.is64 = false;
                        DetectedGameText.Inlines.Add(new Run("Detected: ") { FontWeight = FontWeights.Bold });
                        DetectedGameText.Inlines.Add(new Bold(new Run("My Summer Car (32-bit)") { Foreground = Brushes.LightGreen }));
                        break;
                    default:
                        DetectedGameText.Inlines.Add(new Run("Detected: ") { FontWeight = FontWeights.Bold });
                        DetectedGameText.Inlines.Add(new Bold(new Run("Unknown") { Foreground = Brushes.Red }));
                        break;
                }
                GoNext.IsEnabled = true;
            }

        }
        private void MSCFBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = $"{exeName}|{exeName}"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Storage.gamePath = Path.GetFullPath(Path.GetDirectoryName(openFileDialog.FileName));
                MSCFolder.Text = Storage.gamePath;
                Dbg.Log($"{Storage.selectedGame} path set manually: {Storage.gamePath}");
                try
                {
                    File.WriteAllText(savePath, Storage.gamePath);
                }
                catch (Exception ex)
                {
                    Dbg.Log($"Error creating {savePath}", true, true);
                    Dbg.Log(ex.ToString());
                }
                GameInfo();
            }
        }

        private void GoNext_Click(object sender, RoutedEventArgs e)
        {
            main.MSCLoaderInstallerPage();
        }
    }
}
