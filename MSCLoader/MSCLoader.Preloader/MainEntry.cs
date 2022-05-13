using System;
using System.IO;
using System.Reflection;
using Harmony;
using IniParser.Model;
using IniParser;
using Ionic.Zip;
using System.Linq;

namespace MSCLoader.Preloader
{
    public static class MainEntry
    {
        static byte[] data = { 0x41, 0x6d, 0x69, 0x73, 0x74, 0x65, 0x63, 0x68, 0x0d, 0x00, 0x00, 0x00, 0x4d, 0x79, 0x20, 0x53, 0x75, 0x6d, 0x6d, 0x65, 0x72, 0x20, 0x43, 0x61, 0x72 };
        static long offset = 0;

        //Entry point for doorstop
        public static void Main(string[] args)
        {
            string[] launchArgs = System.Environment.GetCommandLineArgs(); //Environment CommandLine Arguments (in Main there are doorstop args only)

            if (File.Exists("MSCLoader_Preloader.txt")) File.Delete("MSCLoader_Preloader.txt");
            MDebug.Init();
            MDebug.Log($"Launch parameters");
            MDebug.Log($"{string.Join(" ", launchArgs)}",true);
            ReadSettings();
            if (launchArgs.Contains("-mscloader-disable"))
            { 
                MDebug.Log($"Detected -mscloader-disable flag, exiting...");
                return;
            }
            UnpackUpdate();
            OutputLogChecker(); //Enable output_log after possible game update  
            MDebug.Log("Waiting for engine...");
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyWatcher;
        }
        private static void UnpackUpdate()
        {
            if (File.Exists(Path.Combine("Updates", @"Core\update.zip")))
            {
                string modPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
                string managedPath = Path.Combine("mysummercar_Data", @"Managed");
                switch (cfg)
                {
                    case "GF":
                        modPath = Path.GetFullPath(Path.Combine("Mods", "")); 
                        break;
                    case "MD":
                        modPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
                        break;
                    case "AD":
                        modPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"..\LocalLow\Amistech\My Summer Car\Mods"));
                        break;
                    default:
                        modPath = Path.GetFullPath(Path.Combine("Mods", ""));
                        break;
                }
                MDebug.Log("Installing modloader update...");
                UnpackZip(Path.Combine("Updates", @"Core\update.zip"), Path.Combine("Updates", "Core"));
                UnpackZip(Path.Combine("Updates", @"Core\Managed.zip"), managedPath);
                UnpackZip(Path.Combine("Updates", @"Core\Mods.zip"), modPath);
                MDebug.Log("Installing modloader update done!");
            }
        }
        private static void UnpackZip(string fn, string target)
        {
            try
            {
                if (!ZipFile.IsZipFile(fn))
                {
                    MDebug.Log("[MODLOADER UPDATE FAILED]");
                    MDebug.Log($"Failed read zip file {fn}", true);
                }
                else
                {
                    ZipFile zip1 = ZipFile.Read(File.ReadAllBytes(fn));
                    foreach (ZipEntry zz in zip1)
                    {
                        MDebug.Log($"Unpacking: {zz.FileName}");
                        zz.Extract(target, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
                File.Delete(fn);
            }
            catch (Exception e)
            {
                MDebug.Log("[MODLOADER UPDATE FAILED]");
                MDebug.Log(e.ToString(), true);
            }
        }
        private static void ReadSettings()
        {
            try
            {
                MDebug.Log("Reading settings...");
                if (File.Exists("ModLoaderSettings.ini"))
                    File.Delete("ModLoaderSettings.ini");
                IniData ini = new FileIniDataParser().ReadFile("doorstop_config.ini");
                ini.Configuration.AssigmentSpacer = "";
                cfg = ini["MSCLoader"]["mods"];
                string skipIntro = ini["MSCLoader"]["skipIntro"];
                if (!bool.TryParse(skipIntro, out introSkip))
                {
                    introSkip = false;
                    MDebug.Log($"[FAIL] skipIntro - Parse failed, readed value: {skipIntro}, restoring default...");
                    ini["MSCLoader"]["skipIntro"] = "false";
                    new FileIniDataParser().WriteFile("doorstop_config.ini", ini, System.Text.Encoding.ASCII);
                }
                string skipCfg = ini["MSCLoader"]["skipConfigScreen"];
                if (!bool.TryParse(skipCfg, out cfgScreenSkip))
                {
                    cfgScreenSkip = false;
                    MDebug.Log($"[FAIL] skipConfigScreen - Parse failed, readed value: {skipCfg}, restoring default...");
                    ini["MSCLoader"]["skipConfigScreen"] = "false";
                    new FileIniDataParser().WriteFile("doorstop_config.ini", ini, System.Text.Encoding.ASCII);
                }
            }
            catch (Exception e)
            {
                MDebug.Log("[PRELOADER CRASH]");
                MDebug.Log(e.ToString(), true);
            }
        }
        private static void OutputLogChecker()
        {
            try
            {
                bool enLog = false;
                bool skipCfg = false;
                offset = FindBytes(Path.Combine("", @"mysummercar_Data\mainData"), data);

                using (FileStream stream = File.OpenRead(Path.Combine("", @"mysummercar_Data\mainData")))
                {
                    MDebug.Log("Checking output_log status...");
                    stream.Position = offset + 115;
                    if (stream.ReadByte() == 1)
                    {
                        enLog = false;
                    }
                    else
                    {
                        enLog = true;

                    }
                    stream.Position = offset + 96;
                    if (stream.ReadByte() == 1)
                    {
                        skipCfg = false;
                    }
                    else
                    {
                        skipCfg = true;

                    }
                    stream.Close();
                }
                if (enLog)
                {
                    using (FileStream stream = new FileStream(Path.Combine("", @"mysummercar_Data\mainData"), FileMode.Open, FileAccess.ReadWrite))
                    {
                        //output_log.txt
                        MDebug.Log("Enabling output_log...");
                        stream.Position = offset + 115;
                        stream.WriteByte(0x01);
                        stream.Close();
                    }
                }
                if (cfgScreenSkip != skipCfg)
                {
                    using (FileStream stream = new FileStream(Path.Combine("", @"mysummercar_Data\mainData"), FileMode.Open, FileAccess.ReadWrite))
                    {
                        MDebug.Log("Changing config screen skip...");
                        stream.Position = offset + 96;
                        if (cfgScreenSkip)
                            stream.WriteByte(0x00);
                        else
                            stream.WriteByte(0x01);
                        stream.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MDebug.Log("[PRELOADER CRASH]");
                MDebug.Log(e.ToString(), true);
            }
        }

        static void AssemblyWatcher(object sender, AssemblyLoadEventArgs args)
        {
            //System.dll -> Loaded at very end.
            if (args.LoadedAssembly.GetName().Name == "System")
            {
                AppDomain.CurrentDomain.AssemblyLoad -= AssemblyWatcher; //Remove from domain when done.
                InjectModLoader(); //Inject modloader
            }
        }
        public static string cfg;

        [HarmonyPatch(typeof(PlayMakerArrayListProxy))]
        [HarmonyPatch("Awake")]
        class InjectMSCLoader
        {
            static void Prefix() => ModLoader.Init_NP(cfg);
        }

        public static bool introSkip = false;
        public static bool cfgScreenSkip = false;
        public static bool introSkipped = false;
        [HarmonyPatch(typeof(PlayMakerFSM))]
        [HarmonyPatch("Awake")]
        class InjectIntroSkip
        {
            static void Prefix() => SkipIntro(introSkip);
        }
        
        [HarmonyPatch(typeof(HutongGames.PlayMaker.Actions.MousePickEvent))]
        [HarmonyPatch("DoMousePickEvent")]
        class InjectClickthroughFix
        {
            static bool Prefix()
            {
                if (UnityEngine.GUIUtility.hotControl != 0)
                {
                    return false; 
                }
                if(UnityEngine.EventSystems.EventSystem.current != null)
                {
                    if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        return false;
                    }
                }
                return true;

            }
        }

        public static void SkipIntro(bool skip)
        {
            if (!introSkipped && skip)
            {
                introSkipped = true;
                UnityEngine.Application.LoadLevel("MainMenu");
            }
        }
        static void InjectModLoader()
        {
            try
            {
                MDebug.Log("Injecting Main MSCLoader patches...");
                HarmonyInstance.Create("MSCLoader.Main").PatchAll(Assembly.GetExecutingAssembly());
                MDebug.Log("Done.");
            }
            catch (Exception e)
            {
                MDebug.Log("[PRELOADER CRASH]");
                MDebug.Log(e.ToString(), true);
            }
        }

        private static long FindBytes(string fileName, byte[] bytes)
        {
            long i, j;
            using (FileStream fs = File.OpenRead(fileName))
            {
                for (i = 0; i < fs.Length - bytes.Length; i++)
                {
                    fs.Seek(i, SeekOrigin.Begin);
                    for (j = 0; j < bytes.Length; j++)
                        if (fs.ReadByte() != bytes[j]) break;
                    if (j == bytes.Length) break;
                }
                fs.Close();
            }
            return i;
        }
    }

}