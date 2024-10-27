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
        public Advanced()
        {
            InitializeComponent();
            switch (MD5FileHashes.MD5HashFile(Path.Combine(Storage.mscPath, "mysummercar_Data","Mono","mono.dll")))
            {
                case MD5FileHashes.mono64normal:
                    ButtonSet(true, true);
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
                DebugButton.Content = "Enable Debugging";
                DebugStatus.Text = "Debugging is disabled (64-bit)";
                DebugStatus.Foreground = Brushes.Red;
            }
            else
            {
                DebugButton.Content = "Disable Debugging";
                DebugStatus.Text = "Debugging is enabled (64-bit)";
                DebugStatus.Foreground = Brushes.Green;
            }
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            string packPath = Path.Combine(".", "dbg.pack");
            string tempPath = Path.Combine(".", "temp");
            string monoPath = Path.Combine(Storage.mscPath, "mysummercar_Data","Mono","mono.dll");
            string rsf = "runa";
            string dbgpack = "debugpack";
            Dbg.Log("Reading.....dbg.pack");

            if (endbg)
            {
                MessageBox.Show("This action needs admin permissions, when prompted click yes to grant permission and in new window press any key to continue when prompted.","Enable Debugging",MessageBoxButton.OK,MessageBoxImage.Information);
                if(File.Exists(packPath))
                {
                    if (!ZipFile.IsZipFile(packPath))
                    {
                        Dbg.Log("dbg.pack error");
                        MessageBox.Show("Error reading dbg.pack file","Fatal Error",MessageBoxButton.OK,MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        ZipFile zip1 = ZipFile.Read(packPath);
                        for (int i = 0; i < zip1.Entries.Count; i++)
                        {
                            ZipEntry zz = zip1[i];                  
                            zz.ExtractWithPassword(tempPath, ExtractExistingFileAction.OverwriteSilently, dbgpack);
                        }
                        zip1.Dispose();
                    }
                }
                else
                {
                    Dbg.Log("dbg.pack not found");
                    MessageBox.Show("Error reading dbg.pack file", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (File.Exists($"{monoPath}.normal"))
                    File.Delete($"{monoPath}.normal");
                File.Move(monoPath, $"{monoPath}.normal");
                File.Copy(Path.GetFullPath(Path.Combine(tempPath, "mono.dll")), monoPath, true);
                Dbg.Log("Copying debug file.....mono.dll");
                File.Copy(Path.GetFullPath(Path.Combine(tempPath, "pdb2mdb.exe")), Path.Combine(Storage.modsPath, "pdb2mdb.exe"), true);
                Dbg.Log("Copying debug file.....pdb2mdb.exe");
                File.Copy(Path.GetFullPath(Path.Combine(tempPath, "debug.bat")), Path.Combine(Storage.modsPath, "debug.bat"), true);
                Dbg.Log("Copying debug file.....debug.bat");
                try
                {
                    Dbg.Log("Setting elevation for env variable set");
                    ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine(tempPath, "en_debugger.bat"));
                    startInfo.Verb = $"{rsf}s";
                    Process en_debug = Process.Start(startInfo);
                    en_debug.WaitForExit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Dbg.Log("Error", true, true);
                    Dbg.Log(ex.ToString());
                    return;

                }
                if (Environment.GetEnvironmentVariable("DNSPY_UNITY_DBG", EnvironmentVariableTarget.Machine) != null)
                {
                    MessageBox.Show("Debugging Enabled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ButtonSet(true, false);
                }
                else
                    MessageBox.Show("Failed to set debug variable!", "Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
                Directory.Delete(tempPath, true);
                return;
            }
            if (File.Exists($"{monoPath}.normal"))
            {
                File.Delete(monoPath);
                File.Move($"{monoPath}.normal", monoPath);
                Dbg.Log("Recovering backup.....mono.dll");
                MessageBox.Show("Debugging Disabled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ButtonSet(true, true);
                return;
            }
            else
                MessageBox.Show("OG file not found, to disable debugging verify game files on Steam.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            ButtonSet(false, false);
        }

    }
}
