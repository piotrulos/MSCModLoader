using IniParser.Model;
using IniParser;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Ionic.Zip;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for MSCLoaderInstaller.xaml
    /// </summary>
    public partial class MSCLoaderInstaller : Page
    {
        private bool initialized = false;
        private MainWindow main;

        //MSCloader install status
        private bool isInstalled = false; //Is MSCLoader installed
        private bool resetConfig = false; //Config is out of date (or invalid), reset it.
        private bool fullinstall = false; //Do a full install
        private bool updateCore = false; //Only update core files
        private bool updateRefs = false; //Only update references
        private bool updateMSCL = false; //Only update MSCLoader files

        public MSCLoaderInstaller()
        {
            InitializeComponent();
        }
        public void Init(MainWindow m)
        {
            // What a mess please clean this up

            if (initialized)
            {
                UpdatePathText();
                return;
            }

            main = m;

            if (File.Exists(Path.Combine(Storage.gamePath, Storage.selectedGame == Game.MSC ? "mysummercar_Data" : "mywintercar_Data", "Managed", "MSCLoader.Preloader.dll")) && File.Exists(Path.Combine(Storage.gamePath, "winhttp.dll")) && File.Exists(Path.Combine(Storage.gamePath, "doorstop_config.ini")))
            {
                isInstalled = true;
            }
            if (File.Exists(Path.Combine(Storage.currentPath, "dbg.pack")))
                ToggleAdvancedRadioVisibility(Visibility.Visible);
            else
                ToggleAdvancedRadioVisibility(Visibility.Collapsed);
            if (isInstalled)
            {
                try
                {
                    resetConfig = CheckConfig();
                }
                catch (Exception e)
                {
                    Dbg.Log("Failed to read doorstop_config.ini");
                    Dbg.Log(e.ToString());
                    resetConfig = true;
                }
                if (Storage.modsPath == null)
                    Storage.modsPath = Path.GetFullPath(Path.Combine(Storage.gamePath, "Mods"));
                if (resetConfig)
                {
                    Dbg.Log("Outdated config - update Required");
                    fullinstall = true;
                }
                else
                {
                    Dbg.Log("Comparing Versions:");
                    try
                    {
                        VersionCompares();
                    }
                    catch (Exception e)
                    {
                        Dbg.Log($"Fatal Error when checking versions");
                        Dbg.Log(e.ToString());
                        MessageBox.Show($"Fatal Error when checking versions.{Environment.NewLine}{Environment.NewLine}{e.Message}", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        fullinstall = true;
                    }
                }
            }
            UpdateInstallationStatus();
            UpdatePathText();
            PleaseWait.Visibility = Visibility.Collapsed;
            initialized = true;
        }

        void UpdatePathText()
        {
            MSCFolderText.Text = $"{Storage.selectedGame} Folder: ";
            MSCFolderText.Inlines.Add(new Run(Storage.gamePath) { FontWeight = FontWeights.Bold, Foreground = Brushes.Wheat });
            ModsFolderText.Text = $"Mods Folder: ";
            ModsFolderText.Inlines.Add(new Run(Storage.modsPath) { FontWeight = FontWeights.Bold, Foreground = Brushes.Wheat });
        }

        void UpdateInstallationStatus()
        {
            ExecuteSelectedBtn.IsEnabled = false;
            InstallStatusText.Text = $"Status: ";
            if (!isInstalled)
            {
                InstallStatusText.Inlines.Add(new Run("Not installed") { FontWeight = FontWeights.Bold, Foreground = Brushes.Red });
                ToggleRadioButtonsVisibility(Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);
                return;
            }
            if (fullinstall || updateCore || updateRefs || updateMSCL)
            {
                InstallStatusText.Inlines.Add(new Run("Updates available") { FontWeight = FontWeights.Bold, Foreground = Brushes.LightBlue });
                ToggleRadioButtonsVisibility(resetConfig ? Visibility.Collapsed : Visibility.Visible, Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed, Visibility.Visible);
                return;
            }
            InstallStatusText.Inlines.Add(new Run("Installed and up to date") { FontWeight = FontWeights.Bold, Foreground = Brushes.LightGreen });
            ToggleRadioButtonsVisibility(Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible, Visibility.Visible);
        }

        void ToggleRadioButtonsVisibility(Visibility changeModsFolder, Visibility installMSCLoader, Visibility updateMSCLoader, Visibility reinstallMSCLoader, Visibility uninstallMSCLoader)
        {
            ModsFolderRadio.Visibility = changeModsFolder;
            InstallRadio.Visibility = installMSCLoader;
            UpdateRadio.Visibility = updateMSCLoader;
            ReinstallRadio.Visibility = reinstallMSCLoader;
            UninstallRadio.Visibility = uninstallMSCLoader;
        }

        void ToggleAdvancedRadioVisibility(Visibility advancedOptions)
        {
            AdvancedRadio.Visibility = advancedOptions;
        }

        private Version GetZipVersion(string zipPath)
        {
            if (!ZipFile.IsZipFile(zipPath))
            {
                Dbg.Log($"Failed to read file: {zipPath}");
                return new Version("0.0.0.0");
            }

            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                return zip.Comment != null ? new Version(zip.Comment) : new Version("0.0.0.0");
            }
        }

        void VersionCompares()
        {

            string managedPath = Path.Combine(Storage.gamePath,
                Storage.selectedGame == Game.MSC ? "mysummercar_Data" : "mywintercar_Data",
                "Managed");

            // Core Pack
            string corepack = Path.Combine(Storage.currentPath, Storage.is64 ? "core64.pack" : "core32.pack");
            Version coreVer = GetZipVersion(corepack);
            Dbg.Log($"Comparing core version: {coreVer}", true);
            if (InstallerHelpers.VersionCompare(coreVer, Path.Combine(Storage.gamePath, "winhttp.dll")))
                updateCore = true;

            // References
            string tempDir = Path.Combine(Storage.currentPath, "temp");
            Directory.CreateDirectory(tempDir);
            try
            {
                string refpack = Path.Combine(Storage.currentPath, "main_ref.pack");
                if (!ZipFile.IsZipFile(refpack))
                    Dbg.Log($"Failed to read file: {refpack}");
                using (ZipFile zip = ZipFile.Read(refpack))
                    zip.ExtractAll(tempDir);

                Dbg.Log("Comparing reference DLLs...", true);
                foreach (var dll in Directory.GetFiles(tempDir, "*.dll"))
                {
                    string target = Path.Combine(managedPath, Path.GetFileName(dll));
                    if (InstallerHelpers.VersionCompare(dll, target))
                        updateRefs = true;
                }
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }

            // MSCLoader
            string mscPack = Path.Combine(Storage.currentPath, Storage.selectedGame == Game.MSC ? "main_msc.pack" : "main_mwc.pack");
            Version mscVer = GetZipVersion(mscPack);
            Dbg.Log($"Comparing MSCLoader version: {mscVer}", true);
            if (InstallerHelpers.VersionCompare(mscVer, Path.Combine(managedPath, "MSCLoader.dll")))
                updateMSCL = true;

            main.SetMSCLoaderVer(mscVer.ToString());
        }

        bool CheckConfig()
        {
            if (File.Exists(Path.Combine(Storage.gamePath, "doorstop_config.ini")))
            {
                Dbg.Log("Reading.....doorstop_config.ini");
                IniData ini = new FileIniDataParser().ReadFile(Path.Combine(Storage.gamePath, "doorstop_config.ini"));
                ini.Configuration.AssigmentSpacer = "";
                string cfg = ini["MSCLoader"]["mods"];
                string skipIntro = ini["MSCLoader"]["skipIntro"];
                string skipConfigScreen = ini["MSCLoader"]["skipConfigScreen"];
                if (cfg != null && skipIntro != null)
                {
                    Storage.skipIntroCfg = bool.Parse(skipIntro);
                    if (skipConfigScreen != null)
                        Storage.skipConfigScreenCfg = bool.Parse(skipConfigScreen);
                    switch (cfg)
                    {
                        case "GF":
                            Storage.modsPath = Path.GetFullPath(Path.Combine(Storage.gamePath, "Mods"));
                            Storage.modsFolderCfg = ModsFolder.GameFolder;
                            break;
                        case "MD":
                            if(Storage.selectedGame == Game.MSC)
                                Storage.modsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySummerCar", "Mods"));
                            else
                                Storage.modsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Winter Car", "Mods"));
                            Storage.modsFolderCfg = ModsFolder.MyDocuments;
                            break;
                        case "AD":
                            if (Storage.selectedGame == Game.MSC)
                                Storage.modsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow", "Amistech", "My Summer Car", "Mods"));
                            else
                                Storage.modsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow", "Amistech", "My Winter Car", "Mods"));
                            Storage.modsFolderCfg = ModsFolder.Appdata;
                            break;
                        default:
                            Storage.modsPath = Path.GetFullPath(Path.Combine(Storage.gamePath, "Mods"));
                            Storage.modsFolderCfg = ModsFolder.GameFolder;
                            break;
                    }
                    Dbg.Log($"Found mods folder {Storage.modsPath}");
                    if (ini["General"]["target_assembly"] == null)
                    {
                        Dbg.Log("doorstop_config.ini is outdated (pre-4.x)");
                        return true;
                    }
                    else if ((ini["General"]["target_assembly"] != @"mysummercar_Data\Managed\MSCLoader.Preloader.dll" && Storage.selectedGame == Game.MSC) || (ini["General"]["target_assembly"] != @"mywintercar_Data\Managed\MSCLoader.Preloader.dll" && Storage.selectedGame == Game.MWC))
                    {
                        Dbg.Log("doorstop_config.ini invalid target_assembly");
                        return true;
                    }
                    return false;
                }
                else
                {
                    Dbg.Log("doorstop_config.ini is outdated (really old)");
                    return true;
                }

            }
            else return true;
        }

        private void ModsFolderRadio_Checked(object sender, RoutedEventArgs e)
        {
            Storage.selectedAction = SelectedAction.ChangeModsFolder;
            ExecuteSelectedBtn.IsEnabled = true;
        }

        private void InstallRadio_Checked(object sender, RoutedEventArgs e)
        {
            Storage.selectedAction = SelectedAction.InstallMSCLoader;
            ExecuteSelectedBtn.IsEnabled = true;
        }

        private void UpdateRadio_Checked(object sender, RoutedEventArgs e)
        {
            Storage.selectedAction = SelectedAction.UpdateMSCLoader;
            ExecuteSelectedBtn.IsEnabled = true;
        }

        private void ReinstallRadio_Checked(object sender, RoutedEventArgs e)
        {
            Storage.selectedAction = SelectedAction.ReinstallMSCLoader;
            ExecuteSelectedBtn.IsEnabled = true;
        }

        private void UninstallRadio_Checked(object sender, RoutedEventArgs e)
        {
            Storage.selectedAction = SelectedAction.UninstallMSCLoader;
            ExecuteSelectedBtn.IsEnabled = true;
        }

        private void AdvancedRadio_Checked(object sender, RoutedEventArgs e)
        {
            Storage.selectedAction = SelectedAction.AdvancedOptions;
            ExecuteSelectedBtn.IsEnabled = true;
        }

        private void ExecuteSelectedBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (Storage.selectedAction)
            {
                case SelectedAction.ChangeModsFolder:
                    main.SelectModsFolderPage(true);
                    break;
                case SelectedAction.InstallMSCLoader:
                    main.SelectModsFolderPage(false);
                    break;
                case SelectedAction.UpdateMSCLoader:
                    if (fullinstall)
                    {
                        main.InstallProgressPage().InstallMSCLoader(Storage.modsFolderCfg);
                    }
                    else
                    {
                        main.InstallProgressPage().UpdateMSCLoader(updateCore, updateRefs, updateMSCL);
                    }
                    break;
                case SelectedAction.ReinstallMSCLoader:
                    if (MessageBox.Show("Do you want to reinstall MSCLoader?", "Reinstall MSCLoader", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        main.InstallProgressPage().InstallMSCLoader(Storage.modsFolderCfg);
                    }
                    break;
                case SelectedAction.UninstallMSCLoader:
                    if (MessageBox.Show("Do you want to uninstall MSCLoader?", "Uninstall MSCLoader", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        main.InstallProgressPage().UninstalMSCLoader();
                    }
                    break;
                case SelectedAction.AdvancedOptions:
                    Advanced a = new Advanced(updateCore)
                    {
                        Owner = main,                       
                    };
                    a.ShowDialog();
                    break;
                default:
                    Dbg.Log("No action selected");
                    MessageBox.Show("No action selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
    }
}
