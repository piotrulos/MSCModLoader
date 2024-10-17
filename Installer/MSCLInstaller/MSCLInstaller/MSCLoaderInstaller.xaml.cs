using IniParser.Model;
using IniParser;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Ionic.Zip;
using System.Diagnostics;

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
            if (initialized) 
            {
                UpdatePathText(); 
                return; 
            }
            
            main = m;
            if (File.Exists(Path.Combine(Storage.mscPath, "mysummercar_Data", "Managed", "MSCLoader.Preloader.dll")) && File.Exists(Path.Combine(Storage.mscPath, "winhttp.dll")) && File.Exists(Path.Combine(Storage.mscPath, "doorstop_config.ini")))
            {
                isInstalled = true;
            }
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
                    Storage.modsPath = Path.GetFullPath(Path.Combine(Storage.mscPath, "Mods"));
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
                        fullinstall = true;
                    }
                }
            }
            UpdateInstallationStatus();
            UpdatePathText();
            PleaseWait.Visibility = Visibility.Hidden;
            initialized = true;
        }

        void UpdatePathText()
        {
            MSCFolderText.Text = $"MSC Folder: ";
            MSCFolderText.Inlines.Add(new Run(Storage.mscPath) { FontWeight = FontWeights.Bold, Foreground = Brushes.Wheat });
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
        void VersionCompares()
        {
            string corepack;
            if (Storage.is64)
                corepack = Path.Combine(".", "core64.pack");
            else
                corepack = Path.Combine(".", "core32.pack");
            if(!File.Exists(corepack))
            {
                Dbg.MissingFilesError();
            }
            if (!ZipFile.IsZipFile(corepack))
            {
                Dbg.Log($"Failed to read file: {corepack}");
            }
            ZipFile zip1 = ZipFile.Read(corepack);
            Version coreVer = new Version("0.0.0.0");
            if (zip1.Comment != null)
            {
                coreVer = new Version(zip1.Comment);
            }
            zip1.Dispose();
            Dbg.Log("Comparing core file version...", true);
            if (InstallerHelpers.VersionCompare(coreVer, Path.Combine(Storage.mscPath, "winhttp.dll")))
            {
                updateCore = true;
            }
            Directory.CreateDirectory(Path.Combine(".", "temp"));
            string refpack = Path.Combine(".", "main_ref.pack");
            if (!File.Exists(refpack))
            {
                Dbg.MissingFilesError();
            }
            if (!ZipFile.IsZipFile(refpack))
            {
                Dbg.Log($"Failed to read file: {refpack}");
            }
            ZipFile zip2 = ZipFile.Read(refpack);
            zip2.ExtractAll(Path.Combine(".", "temp"));
            zip2.Dispose();
            Dbg.Log("Comparing references versions...", true);
            foreach (string f in Directory.GetFiles(Path.Combine(".", "temp")))
            {
                if (InstallerHelpers.VersionCompare(f, Path.Combine(Storage.mscPath, "mysummercar_Data", "Managed", Path.GetFileName(f))))
                {
                    updateRefs = true;
                }
            }
            Directory.Delete(Path.Combine(".", "temp"), true);
            string msc = Path.Combine(".", "main_msc.pack");
            if (!File.Exists(msc))
            {
                Dbg.MissingFilesError();
            }
            if (!ZipFile.IsZipFile(msc))
            {
                Dbg.Log($"Failed to read file: {msc}");
            }
            ZipFile zip3 = ZipFile.Read(msc);
            Version mscVer = new Version("0.0.0.0");
            if (zip3.Comment != null)      
            {
               
                mscVer = new Version(zip3.Comment);
            }
            zip3.Dispose();
            Dbg.Log("Comparing MSCLoader version...", true);
            if (InstallerHelpers.VersionCompare(mscVer, Path.Combine(Storage.mscPath, "mysummercar_Data", "Managed", "MSCLoader.dll")))
            {
                updateMSCL = true;
            }
            main.SetMSCLoaderVer(mscVer.ToString());

        }
        bool CheckConfig()
        {
            if (File.Exists(Path.Combine(Storage.mscPath, "doorstop_config.ini")))
            {
                Dbg.Log("Reading.....doorstop_config.ini");
                IniData ini = new FileIniDataParser().ReadFile(Path.Combine(Storage.mscPath, "doorstop_config.ini"));
                ini.Configuration.AssigmentSpacer = "";
                string cfg = ini["MSCLoader"]["mods"];
                string skipIntro = ini["MSCLoader"]["skipIntro"];
                string skipConfigScreen = ini["MSCLoader"]["skipConfigScreen"];
                if (cfg != null && skipIntro != null)
                {
                    Storage.skipIntroCfg = bool.Parse(skipIntro);
                    if(skipConfigScreen != null)
                        Storage.skipConfigScreenCfg = bool.Parse(skipConfigScreen);
                    switch (cfg)
                    {
                        case "GF":
                            Storage.modsPath = Path.GetFullPath(Path.Combine(Storage.mscPath, "Mods"));
                            Storage.modsFolderCfg = ModsFolder.GameFolder;
                            break;
                        case "MD":
                            Storage.modsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySummerCar", "Mods"));
                            Storage.modsFolderCfg = ModsFolder.MyDocuments;
                            break;
                        case "AD":
                            Storage.modsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow", "Amistech", "My Summer Car", "Mods"));
                            Storage.modsFolderCfg = ModsFolder.Appdata;
                            break;
                        default:
                            Storage.modsPath = Path.GetFullPath(Path.Combine(Storage.mscPath, "Mods"));
                            Storage.modsFolderCfg = ModsFolder.GameFolder;
                            break;
                    }
                    Dbg.Log($"Found mods folder {Storage.modsPath}");
                    if (ini["General"]["target_assembly"] == null)
                    {
                        Dbg.Log("doorstop_config.ini is outdated (pre-4.x)");
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
            Storage.selectedAction  = SelectedAction.ReinstallMSCLoader;
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
                    if(MessageBox.Show("Do you want to uninstall MSCLoader?", "Uninstall MSCLoader", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        main.InstallProgressPage().UninstalMSCLoader();
                    }
                    break;
                case SelectedAction.AdvancedOptions:
                    break;
                default:
                    Dbg.Log("No action selected");
                    MessageBox.Show("No action selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
    }
}
