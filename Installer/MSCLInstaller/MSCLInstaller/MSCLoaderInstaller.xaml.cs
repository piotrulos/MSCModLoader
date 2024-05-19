using IniParser.Model;
using IniParser;
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
using Ionic.Zip;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for MSCLoaderInstaller.xaml
    /// </summary>
    public partial class MSCLoaderInstaller : Page
    {

        bool resetConfig = false;
        bool fullinstall = false;
        bool updateCore = false;
        bool updateRefs = false;
        bool updateMSCL = false;

        public MSCLoaderInstaller()
        {
            InitializeComponent();
        }
        public void Init()
        {
            bool isInstalled = false;
            MSCFolderText.Text = $"MSC Folder: ";
            MSCFolderText.Inlines.Add(new Run(Storage.mscPath) { FontWeight = FontWeights.Bold });
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
                ModsFolderText.Text = $"Mods Folder: ";
                ModsFolderText.Inlines.Add(new Run(Storage.modsPath) { FontWeight = FontWeights.Bold });
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
                if (fullinstall || updateCore || updateRefs || updateMSCL)
                {
                    InstallStatusText.Text = $"Status: ";
                    InstallStatusText.Inlines.Add(new Run("Updates available") { FontWeight = FontWeights.Bold, Foreground = Brushes.Blue });
                    InstallBtn.Content = "Update MSCLoader";
                }
                else
                {
                    InstallStatusText.Text = $"Status: ";
                    InstallStatusText.Inlines.Add(new Run("Installed and up to date") { FontWeight = FontWeights.Bold, Foreground = Brushes.Green });
                    InstallBtn.IsEnabled = false;
                }
                UninstallBtn.IsEnabled = true;
            }
            else
            {
                InstallStatusText.Text = $"Status: ";
                InstallStatusText.Inlines.Add(new Run("Not installed") { FontWeight = FontWeights.Bold, Foreground = Brushes.Red });

            }
            PleaseWait.Visibility = Visibility.Hidden;

        }
        void VersionCompares()
        {
            string corepack;
            if (Storage.is64)
                corepack = Path.Combine(".", "core64.pack");
            else
                corepack = Path.Combine(".", "core32.pack");
            Directory.CreateDirectory(Path.Combine(".", "temp"));
            if (!ZipFile.IsZipFile(corepack))
            {
                Dbg.Log($"Invalid file: {corepack}");
            }
            ZipFile zip1 = ZipFile.Read(corepack);
            zip1.ExtractAll(Path.Combine(".", "temp"));
            zip1.Dispose();
            if (InstallerHelpers.VersionCompare(Path.Combine(".", "temp", "winhttp.dll"), Path.Combine(Storage.mscPath, "winhttp.dll")))
            {
                updateCore = true;
            }
            Directory.Delete(Path.Combine(".", "temp"), true);
            Directory.CreateDirectory(Path.Combine(".", "temp"));
            string refpack = Path.Combine(".", "main_ref.pack");
            if (!ZipFile.IsZipFile(refpack))
            {
                Dbg.Log($"Invalid file: {refpack}");
            }
            ZipFile zip2 = ZipFile.Read(refpack);
            zip2.ExtractAll(Path.Combine(".", "temp"));
            zip2.Dispose();
            foreach (string f in Directory.GetFiles(Path.Combine(".", "temp")))
            {
                if (InstallerHelpers.VersionCompare(f, Path.Combine(Storage.mscPath, "mysummercar_Data", "Managed", Path.GetFileName(f))))
                {
                    updateRefs = true;
                }
            }
            Directory.Delete(Path.Combine(".", "temp"), true);
            Directory.CreateDirectory(Path.Combine(".", "temp"));
            string msc = Path.Combine(".", "main_msc.pack");
            if (!ZipFile.IsZipFile(msc))
            {
                Dbg.Log($"Invalid file: {msc}");
            }
            ZipFile zip3 = ZipFile.Read(msc);
            zip3.ExtractAll(Path.Combine(".", "temp"));
            zip3.Dispose();
            ZipFile zip4 = ZipFile.Read(Path.Combine(".", "temp", "Managed.zip"));
            zip4.ExtractAll(Path.Combine(".", "temp"));
            zip4.Dispose();
            if (InstallerHelpers.VersionCompare(Path.Combine(".", "temp", "MSCLoader.dll"), Path.Combine(Storage.mscPath, "mysummercar_Data", "Managed", "MSCLoader.dll")))
            {
                updateMSCL = true;
            }
            Directory.Delete(Path.Combine(".", "temp"), true);

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
                if (cfg != null && skipIntro != null)
                {
                    switch (cfg)
                    {
                        case "GF":
                            Storage.modsPath = Path.GetFullPath(Path.Combine(Storage.mscPath, "Mods"));
                            break;
                        case "MD":
                            Storage.modsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySummerCar", "Mods"));
                            break;
                        case "AD":
                            Storage.modsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow", "Amistech", "My Summer Car", "Mods"));
                            break;
                        default:
                            Storage.modsPath = Path.GetFullPath(Path.Combine(Storage.mscPath, "Mods"));
                            break;
                    }
                    Dbg.Log($"Found mods folder {Storage.modsPath}");
                    if (ini["General"]["target_assembly"] == null)
                    {
                        Dbg.Log("doorstop_config.ini is outdated (pre-4.1)");
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
    }
}
