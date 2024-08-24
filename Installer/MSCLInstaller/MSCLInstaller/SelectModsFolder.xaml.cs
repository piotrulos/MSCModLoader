using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;

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
            gfPath = Path.GetFullPath(Path.Combine(Storage.mscPath, "Mods"));
            GameFolderPathText.Text = gfPath;
            if (changeFolder && gfPath == Storage.modsPath)
            {
                GameFolderRB.IsChecked = true;
                currentFolder = ModsFolder.GameFolder;
                GFCurrentText.Visibility = Visibility.Visible;
            }
            mdPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySummerCar", "Mods"));
            MyDocumentsPathText.Text = mdPath;
            if (mdPath.Contains("OneDrive"))
            {
                MyDocumentsRB.IsEnabled = false;
                OneDriveWarning.Visibility = Visibility.Visible;
            }
            else
            {
                if (changeFolder && mdPath == Storage.modsPath)
                {
                    MyDocumentsRB.IsChecked = true;
                    currentFolder = ModsFolder.MyDocuments;
                    MDCurrentText.Visibility = Visibility.Visible;
                }
            }
            adPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow", "Amistech", "My Summer Car", "Mods"));
            AppdataPathText.Text = adPath;


            if (changeFolder && adPath == Storage.modsPath)
            {
                AppdataRB.IsChecked = true;
                currentFolder = ModsFolder.Appdata;
                ADCurrentText.Visibility = Visibility.Visible;
            }

            if (!changeFolder)
            {
                currentFolder = ModsFolder.GameFolder;
                GameFolderRB.IsChecked = true;
            }
        }
        private void SelectModsFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            if (changeFolder)
            {
                if (selectedFolder == currentFolder)
                {
                    main.MSCLoaderInstallerPage();
                    return;
                }
            }

            Storage.modsPath = selectedFolder == ModsFolder.GameFolder ? gfPath : selectedFolder == ModsFolder.MyDocuments ? mdPath : adPath;

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
