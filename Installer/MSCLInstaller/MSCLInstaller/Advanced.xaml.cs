using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for Advanced.xaml
    /// </summary>
    public partial class Advanced : Window
    {
        bool endbg = false;
        public Advanced(bool core)
        {
            InitializeComponent();
            if (core)
            {
                DebugButton.Content = "Disable Debugging";
                DebugStatus.Text = "Please update MSCLoader first";
                DebugStatus.Foreground = Brushes.Red;
                DebugButton.Visibility = Visibility.Collapsed;
                CopyDebugButton.Visibility = Visibility.Collapsed;
                return;
            }
            switch (MD5FileHashes.MD5HashFile(Path.Combine(Storage.mscPath, "mysummercar_Data", "Mono", "mono.dll")))
            {
                case MD5FileHashes.mono64normal:
                    ButtonSet(false, true);
                    break;
                case MD5FileHashes.mono64debug:
                    ButtonSet(true, false);
                    break;
                default:
                    DebugStatus.Text = "Unknown mono.dll version";
                    DebugStatus.Foreground = Brushes.Red;
                    ButtonSet(false, true);
                    break;
            }
        }

        private void ButtonSet(bool enabled, bool enabledbg)
        {
            DebugButton.IsEnabled = enabled;
            endbg = enabledbg;
            if (enabledbg)
            {
                DebugButton.Visibility = Visibility.Collapsed;
                CopyDebugButton.IsEnabled = true;
                DebugButton.Content = "Enable Debugging";
                DebugStatus.Text = "Copy Debug script to mods folder";
                DebugStatus.Foreground = Brushes.Green;
            }
            else
            {
                DebugButton.Content = "Disable Debugging";
                DebugStatus.Text = "Disable Old Debugging";
                DebugStatus.Foreground = Brushes.Orange;
            }
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            string monoPath = Path.Combine(Storage.mscPath, "mysummercar_Data", "Mono", "mono.dll");
            Dbg.Log("Reading.....dbg.pack");
            if (File.Exists($"{monoPath}.normal"))
            {
                File.Delete(monoPath);
                File.Move($"{monoPath}.normal", monoPath);
                Dbg.Log("Recovering backup.....mono.dll");
                MessageBox.Show("Debugging Disabled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ButtonSet(false, true);
                return;
            }
            else
                MessageBox.Show("OG file not found, to disable debugging verify game files on Steam.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            ButtonSet(false, false);
        }
        private void CopyDebugButton_Click(object sender, RoutedEventArgs e)
        {
            string dbgpack = "debugpack";
            string packPath = Path.Combine(Storage.currentPath, "dbg.pack");
            string tempPath = Path.Combine(Storage.currentPath, "temp");
            if (File.Exists(packPath))
            {
                if (!ZipFile.IsZipFile(packPath))
                {
                    Dbg.Log("dbg.pack error");
                    MessageBox.Show("Error reading dbg.pack file", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    using (ZipFile zip = ZipFile.Read(packPath))
                    {
                        for (int i = 0; i < zip.Entries.Count; i++)
                        {
                            ZipEntry zz = zip[i];
                            zz.ExtractWithPassword(tempPath, ExtractExistingFileAction.OverwriteSilently, dbgpack);
                        }
                    }

                }
            }
            else
            {
                Dbg.Log("dbg.pack not found");
                MessageBox.Show("Error reading dbg.pack file", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                File.Copy(Path.GetFullPath(Path.Combine(tempPath, "pdb2mdb.exe")), Path.Combine(Storage.modsPath, "pdb2mdb.exe"), true);
                Dbg.Log("Copying debug file.....pdb2mdb.exe");
                File.Copy(Path.GetFullPath(Path.Combine(tempPath, "debug.bat")), Path.Combine(Storage.modsPath, "debug.bat"), true);
                Dbg.Log("Copying debug file.....debug.bat");
                MessageBox.Show("Files copied successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy files: {ex.Message}!", "Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Directory.Delete(tempPath, true);

        }
    }


}

