using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SelectGameFolder selectGameFolder;
        MSCLoaderInstaller mscloaderInstaller;
        SelectModsFolder selectModsFolder;
        InstallProgress installProgress;
        Version MSCLInstallerVer;
        public MainWindow()
        {
            InitializeComponent();
            MSCLInstallerVer = Assembly.GetExecutingAssembly().GetName().Version;
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            if (File.Exists("Log.txt")) File.Delete("Log.txt");
            if (Directory.Exists(Path.Combine(".", "temp"))) Directory.Delete(Path.Combine(".", "temp"), true);
            Dbg.Init();
            Dbg.Log($"Current folder: {Path.GetFullPath(".")}");
            selectGameFolder = new SelectGameFolder();
            mscloaderInstaller = new MSCLoaderInstaller();
            selectModsFolder = new SelectModsFolder();
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
            StatusBarText.Text = $"MSCLInstaller (ver. {MSCLInstallerVer})";

        }
        internal void SetMSCLoaderVer(string ver)
        {
            StatusBarText.Text = $"MSCLInstaller (ver. {MSCLInstallerVer}) | MSCLoader (ver. {ver})";
        }
        internal void ReInitInstallerPage()
        {
            mscloaderInstaller = new MSCLoaderInstaller();
        }
        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
          //  installProgress = new InstallProgress();
           // MainFrame.Content = installProgress;

       //     mscli = new MSCLoaderInstaller();

            //  MessageBox.Show(Environment.GetFolderPath(), "wtf", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal InstallProgress InstallProgressPage()
        {
            installProgress = new InstallProgress(this);
            MainFrame.Content = installProgress;
            return installProgress;
        }

        public void SelectGameFolderPage(Game game)
        {
            Dbg.Log("Select Game Folder", true, true);
            MSCIPageTitle.Text = "Select Game Folder";
            Storage.selectedGame = game;
            MainFrame.Content = selectGameFolder;
            selectGameFolder.Init(this);
        }

        public async void MSCLoaderInstallerPage()
        {
            Dbg.Log("MSCLoader Installer", true, true);
            MainFrame.Content = mscloaderInstaller;
            MSCIPageTitle.Text = "Please wait.... Checking current files";
            await InstallerHelpers.DelayedWork();
            MSCIPageTitle.Text = "Select Option Below";
            mscloaderInstaller.Init(this);
        }

        public void SelectModsFolderPage(bool change)
        {
            Dbg.Log("Select Mods Folder", true, true);
            MSCIPageTitle.Text = "Select Mods Folder";
            MainFrame.Content = selectModsFolder;
            selectModsFolder.Init(this, change);
        }
    }
}
