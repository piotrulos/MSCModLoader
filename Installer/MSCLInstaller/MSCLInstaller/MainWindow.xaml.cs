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

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
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
            mscli.Init();
        }
    }
}
