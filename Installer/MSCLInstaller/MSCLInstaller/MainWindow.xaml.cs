using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                MessageBox.Show($"Couldn't find any required core files.{Environment.NewLine}Please unpack all files before launching this installer!", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Dbg.Log($"!!! CRASH !!!", true, true);
                Dbg.Log($"Core files not found, exiting.");
                Environment.Exit(0);
            }
            SelectGameFolderPage(Game.MSC);
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("wtf", "wtf", MessageBoxButton.OK, MessageBoxImage.Error);
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
