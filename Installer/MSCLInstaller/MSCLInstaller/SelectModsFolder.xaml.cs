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
            string oldPath = Storage.modsPath;
            string newPath = selectedFolder == ModsFolder.GameFolder ? gfPath : selectedFolder == ModsFolder.MyDocuments ? mdPath : adPath;
            //If Change Folder is selected
            if (changeFolder)
            {
                if (selectedFolder == currentFolder)
                {
                    Dbg.Log("Selected folder is same as current, no changes will be made");
                    main.MSCLoaderInstallerPage(); //No change, go back.
                    return;
                }
                if (MessageBox.Show($"Your new Mods folder will be changed to {Environment.NewLine}{newPath}{Environment.NewLine}{Environment.NewLine}Continue?", "Mods Folder", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Dbg.Log($"Changing mods folder from {oldPath} to {newPath}");
                    if (MessageBox.Show($"Do you want to copy all mods from old location to new one?{Environment.NewLine}{Environment.NewLine}Selecting No will create blank Mods folder in new location", "Copy Old Mods?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Dbg.Log($"Selected Yes, copying old mods from {oldPath} to {newPath}");
                        ChangeModsFolderStart(true, oldPath, newPath, selectedFolder);
                    }
                    else
                    {
                        Dbg.Log($"Creating new blank mods folder in {newPath}");
                        ChangeModsFolderStart(false, oldPath, newPath, selectedFolder);
                    }
                }
                return;
            }
            Storage.modsPath = newPath;
            main.InstallProgressPage().InstallMSCLoader(selectedFolder);
        }
        private void ChangeModsFolderStart(bool copyOldMods, string oldPath, string newPath, ModsFolder newFolder)
        {
            bool deleteTargetFolder = false;

            if (Directory.Exists(newPath) && copyOldMods)
            {
                Dbg.Log($"New path for Mods folder {newPath} already exists");
                if (MessageBox.Show($"Warning!{Environment.NewLine}Selected mods path already exists, all files will be deleted in new path!{Environment.NewLine}{Environment.NewLine}Continue?", "Mods Folder", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    Dbg.Log("Cancelling folder change");
                    return;
                }
            }
            if (Directory.Exists(newPath) && !copyOldMods)
            {
                Dbg.Log($"New path for Mods folder {newPath} already exists");
                MessageBoxResult messageBoxResult = MessageBox.Show($"Warning!{Environment.NewLine}Selected mods path already exists, do you want to delete it and make it new Mods folder?{Environment.NewLine}{Environment.NewLine}Yes - Delete and make it as new blank Mods folder{Environment.NewLine}No - Keep all files in target folder{Environment.NewLine}Cancel - Cancel folder change", "Mods Folder", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    Dbg.Log("Cancelling folder change");
                    return;
                }
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    deleteTargetFolder = true;
                }
            }
            Storage.modsPath = newPath;
            if (copyOldMods)
            {
               main.InstallProgressPage().ChangeModsFolderAndCopyAsync(oldPath, newFolder);
            }
            else
            {
               main.InstallProgressPage().ChangeModsFolder(newFolder, deleteTargetFolder);
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
