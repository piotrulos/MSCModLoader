using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Diagnostics;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for SelectModsFolder.xaml
    /// </summary>
    public partial class SelectModsFolder : Page
    {
        MainWindow main;
        string gfPath;
        string mdPath;
        string adPath;
        ModsFolder currentFolder = ModsFolder.GameFolder;
        ModsFolder selectedFolder = ModsFolder.GameFolder;
        bool changeFolder = false;
        public SelectModsFolder()
        {
            InitializeComponent();
        }

        public void Init(MainWindow m, bool change)
        {
            main = m;
            changeFolder = change;
            PopulatePaths();
        }

        private void PopulatePaths()
        {
            Dbg.Log("Getting paths");
            gfPath = Path.GetFullPath(Path.Combine(Storage.mscPath, "Mods"));
            GameFolderPathText.Text = gfPath;
            Dbg.Log($"Game folder path: {gfPath}");
            if (changeFolder && gfPath == Storage.modsPath)
            {
                GameFolderRB.IsChecked = true;
                currentFolder = ModsFolder.GameFolder;
                GFCurrentText.Visibility = Visibility.Visible;
                Dbg.Log("[Game folder is current path]");
            }
            mdPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySummerCar", "Mods"));
            MyDocumentsPathText.Text = mdPath;
            Dbg.Log($"My documents path: {mdPath}");
            if (mdPath.Contains("OneDrive"))
            {
                MyDocumentsRB.IsEnabled = false;
                OneDriveWarning.Visibility = Visibility.Visible;
                Dbg.Log("[OneDrive path detected]");
            }
            else
            {
                if (changeFolder && mdPath == Storage.modsPath)
                {
                    MyDocumentsRB.IsChecked = true;
                    currentFolder = ModsFolder.MyDocuments;
                    MDCurrentText.Visibility = Visibility.Visible;
                    Dbg.Log("[My documents is current path]");
                }
            }
            adPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow", "Amistech", "My Summer Car", "Mods"));
            AppdataPathText.Text = adPath;
            Dbg.Log($"Appdata path: {adPath}");
            if (changeFolder && adPath == Storage.modsPath)
            {
                AppdataRB.IsChecked = true;
                currentFolder = ModsFolder.Appdata;
                ADCurrentText.Visibility = Visibility.Visible;
                Dbg.Log("[Appdata is current path]");
            }

            if (!changeFolder)
            {
                currentFolder = ModsFolder.GameFolder;
                GameFolderRB.IsChecked = true;
            }
        }
        private void SelectModsFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            Storage.modsPath = selectedFolder == ModsFolder.GameFolder ? gfPath : selectedFolder == ModsFolder.MyDocuments ? mdPath : adPath;

            if (changeFolder)
            {
                if (selectedFolder == currentFolder)
                {
                    main.MSCLoaderInstallerPage();
                    return;
                }
                main.InstallProgressPage().DoSomeSHit();

            }
        }

        private void GameFolderRB_Checked(object sender, RoutedEventArgs e)
        {
            selectedFolder = ModsFolder.GameFolder;
        }
        private void MyDocumentsRB_Checked(object sender, RoutedEventArgs e)
        {
            selectedFolder = ModsFolder.MyDocuments;
        }

        private void AppdataRB_Checked(object sender, RoutedEventArgs e)
        {
            selectedFolder = ModsFolder.Appdata;
        }
    }
}
