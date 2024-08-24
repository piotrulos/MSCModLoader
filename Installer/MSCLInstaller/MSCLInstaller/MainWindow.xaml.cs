using System;
using System.IO;
using System.Windows;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SelectGameFolder sgf;
        MSCLoaderInstaller mscli;
        SelectModsFolder smf;
        public MainWindow()
        {
            InitializeComponent();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            if (File.Exists("Log.txt")) File.Delete("Log.txt");
            if (Directory.Exists(Path.Combine(".", "temp"))) Directory.Delete(Path.Combine(".", "temp"), true);
            Dbg.Init();
            Dbg.Log($"Current folder: {Path.GetFullPath(".")}");
            sgf = new SelectGameFolder();
            mscli = new MSCLoaderInstaller();
            smf = new SelectModsFolder();
            Storage.packFiles = Directory.GetFiles(".", "*.pack");
            if (Storage.packFiles.Length == 0)
            {
                Dbg.MissingFilesError();
            }
            if(!File.Exists("Ionic.Zip.Reduced.dll") || !File.Exists("INIFileParser.dll"))
            {
                Dbg.MissingFilesError();
            }

            SelectGameFolderPage(Game.MSC);
        }
        internal void ReInitInstallerPage()
        {
            mscli = new MSCLoaderInstaller();
        }
        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
       //     mscli = new MSCLoaderInstaller();

            //  MessageBox.Show(Environment.GetFolderPath(), "wtf", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void SelectGameFolderPage(Game game)
        {
            Dbg.Log("Select Game Folder", true, true);
            MSCIPageTitle.Text = "Select Game Folder";
            Storage.selectedGame = game;
            MainFrame.Content = sgf;
            sgf.Init(this);
        }

        public async void MSCLoaderInstallerPage()
        {
            Dbg.Log("MSCLoader Installer", true, true);
            MainFrame.Content = mscli;
            MSCIPageTitle.Text = "Please wait.... Checking current files";
            await InstallerHelpers.DelayedWork();
            MSCIPageTitle.Text = "Select Option Below";
            mscli.Init(this);
        }

        public void SelectModsFolderPage(bool change)
        {
            Dbg.Log("Select Mods Folder", true, true);
            MSCIPageTitle.Text = "Select Mods Folder";
            MainFrame.Content = smf;
            smf.Init(this, change);
        }
    }
}
