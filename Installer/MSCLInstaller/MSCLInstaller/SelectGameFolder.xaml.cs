using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    /// Interaction logic for SelectGameFolder.xaml
    /// </summary>
    public partial class SelectGameFolder : Page
    {
        public SelectGameFolder()
        {
            InitializeComponent();
        }
        public void Init()
        {
            FindMSCOnSteam();
            if (string.IsNullOrEmpty(Storage.mscPath))
            {
                //Read saved path
            }
            
        }

        private void FindMSCOnSteam()
        {
            string steamPath = null;
            RegistryKey steam = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Valve\\Steam");
            if (steam != null)
            {
                steamPath = steam.GetValue("InstallPath").ToString();
            }
            string msc = Path.Combine(steamPath, "steamapps", "common", "My Summer Car");
            if (File.Exists(Path.Combine(msc, "mysummercar.exe")))
            {
                Storage.mscPath = msc;
            }
            else
            {
                string steamLib = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
                if (File.Exists(Path.Combine(steamLib)))
                {
                    string[] lib = File.ReadAllLines(steamLib);
                    foreach (string s in lib)
                    {
                        if (s.Trim().StartsWith("\"path\""))
                        {
                            string msclib = Path.Combine(s.Trim().Split('"')[3], "steamapps", "common", "My Summer Car");
                            if (File.Exists(Path.Combine(msclib, "mysummercar.exe")))
                            {
                                Storage.mscPath = msclib;
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(Storage.mscPath))
                MSCFolder.Text = Storage.mscPath;
        }
        void ReadMSCInfo()
        {

        }
        private void MSCFBrowse_Click(object sender, RoutedEventArgs e)
        {
            //Folder select
        }
    }
}
