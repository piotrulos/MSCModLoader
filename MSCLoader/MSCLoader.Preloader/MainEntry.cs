using Harmony;
using IniParser;
using IniParser.Model;
using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Doorstop
{
    //Entry point for doorstop 4.0
    class Entrypoint
    {
        public static void Start()
        {
            //Just point to doorstop 3.x entry point
            MSCLoader.Preloader.MainEntry.Main();
        }
    }
}

namespace MSCLoader.Preloader
{
    public static class MainEntry
    {
        static byte[] mscData = { 0x41, 0x6D, 0x69, 0x73, 0x74, 0x65, 0x63, 0x68, 0x0D, 0x00, 0x00, 0x00, 0x4D, 0x79, 0x20, 0x53, 0x75, 0x6D, 0x6D, 0x65, 0x72, 0x20, 0x43, 0x61, 0x72 };
        static byte[] mwcData = { 0x41, 0x6D, 0x69, 0x73, 0x74, 0x65, 0x63, 0x68, 0x0D, 0x00, 0x00, 0x00, 0x4D, 0x79, 0x20, 0x57, 0x69, 0x6E, 0x74, 0x65, 0x72, 0x20, 0x43, 0x61, 0x72 };
        static string managedPath;
        static bool mwc = false;
        static bool signleInstancePatch = false;

        //Entry point for doorstop
        public static void Main()
        {
            string[] launchArgs = System.Environment.GetCommandLineArgs(); //Environment CommandLine Argument

            MDebug.Init();
            MDebug.Log($"Launch parameters");
            MDebug.Log($"{string.Join(" ", launchArgs)}", true);
            managedPath = Environment.GetEnvironmentVariable("DOORSTOP_MANAGED_FOLDER_DIR");
            if (managedPath.Contains("My Winter Car")) mwc = true;
            MDebug.Log($"Game: {(mwc ? "My Winter Car" : "My Summer Car")}", true);

            MDebug.Log($"Managed folder: {managedPath}");

            if (File.Exists(Path.Combine(managedPath, "obsolete.txt")))
                DeleteObsoleteFiles();
            ReadSettings();
            if (launchArgs.Contains("-mscloader-disable"))
            {
                MDebug.Log($"Detected -mscloader-disable flag, exiting...");
                return;
            }
            if (launchArgs.Contains("-mscloader-sipatch"))
            {
                MDebug.Log($"Detected -mscloader-sipatch (will attempt to disable single instance)");
                signleInstancePatch = true;
            }
            UnpackUpdate();
            MainDataPatcher(); //Enable output_log after game update  
            MDebug.Log("Waiting for engine...");
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyWatcher;
        }
        private static void UnpackUpdate()
        {
            if (File.Exists(Path.Combine("Updates", Path.Combine("Core", "update.zip"))))
            {
                string modPath;
                switch (cfg)
                {
                    case "GF":
                        modPath = Path.GetFullPath(Path.Combine("Mods", ""));
                        break;
                    case "MD":
                        modPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine(mwc ? "My Winter Car" : "MySummerCar", "Mods")));
                        break;
                    case "AD":
                        modPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $@"..\LocalLow\Amistech\{(mwc ? "My Winter Car" : "My Summer Car")}\Mods"));
                        break;
                    default:
                        modPath = Path.GetFullPath(Path.Combine("Mods", ""));
                        break;
                }
                MDebug.Log("Installing MSCLoader update...");
                UnpackZip(Path.Combine("Updates", Path.Combine("Core", "update.zip")), Path.Combine("Updates", "Core"));
                UnpackZip(Path.Combine("Updates", Path.Combine("Core", "Managed.zip")), managedPath);
                UnpackZip(Path.Combine("Updates", Path.Combine("Core", "Mods.zip")), modPath);
                MDebug.Log("Installing MSCLoader update done!");
            }
        }
        private static void UnpackZip(string fn, string target)
        {
            try
            {
                if (!ZipFile.IsZipFile(fn))
                {
                    MDebug.Log("[MSCLOADER UPDATE FAILED]");
                    MDebug.Log($"Invalid zip file {fn}", true);
                    File.Delete(fn);
                }
                else
                {
                    using (ZipFile zipFile = ZipFile.Read(fn))
                    {
                        foreach (ZipEntry zipEntry in zipFile)
                        {
                            MDebug.Log($"Unpacking: {zipEntry.FileName}");
                            if (zipEntry.FileName == "Ionic.Zip.Reduced.dll")
                            {
                                if (File.Exists(Path.Combine(target, "Ionic.Zip.Reduced.dll"))) continue;
                            }
                            zipEntry.Extract(target, ExtractExistingFileAction.OverwriteSilently);
                        }
                    }
                }
                File.Delete(fn);
            }
            catch (Exception e)
            {
                MDebug.Log("[MSCLOADER UPDATE FAILED]");
                MDebug.Log(e.ToString(), true);
                File.Delete(fn);
            }
        }
        private static void ReadSettings()
        {
            try
            {
                MDebug.Log("Reading settings...");
                if (File.Exists("ModLoaderSettings.ini")) //Pro crap
                    File.Delete("ModLoaderSettings.ini");
                IniData ini = new FileIniDataParser().ReadFile("doorstop_config.ini");
                cfg = ini["MSCLoader"]["mods"];
                string skipSplash = ini["MSCLoader"]["skipIntro"];
                if (!bool.TryParse(skipSplash, out splashSkip))
                {
                    splashSkip = false;
                    MDebug.Log($"[FAIL] skipIntro - Parse failed, value parsed as: {skipSplash}, restoring default...");
                    ini["MSCLoader"]["skipIntro"] = "false";
                    new FileIniDataParser().WriteFile("doorstop_config.ini", ini, System.Text.Encoding.ASCII);
                }
                string skipCfg = ini["MSCLoader"]["skipConfigScreen"];
                if (!bool.TryParse(skipCfg, out cfgScreenSkip))
                {
                    cfgScreenSkip = false;
                    MDebug.Log($"[FAIL] skipConfigScreen - Parse failed, value parsed as: {skipCfg}, restoring default...");
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
        private static void MainDataPatcher()
        {
            try
            {
                bool enLog = false;
                bool skipCfg = false;
                bool singleInstance = true;
                byte cfgScreenOffset = 96;
                byte outputLogOffset = 115;
                byte signleInstanceOffset = 117; //For native linux chromium conflict
                string mainDataPath = Path.Combine("", Path.Combine(mwc ? "mywintercar_Data" : "mysummercar_Data", "mainData"));
                long mainDataPosition = FindBytes(mainDataPath, mwc ? mwcData : mscData);
                using (FileStream stream = File.OpenRead(mainDataPath))
                {
                    MDebug.Log("Checking output_log status...");
                    stream.Position = mainDataPosition + outputLogOffset;
                    enLog = stream.ReadByte() != 1;
                    stream.Position = mainDataPosition + cfgScreenOffset;
                    skipCfg = stream.ReadByte() != 1;
                    stream.Position = mainDataPosition + signleInstanceOffset;
                    singleInstance = stream.ReadByte() == 1 ? true : false;
                    stream.Close();
                }
                if (enLog)
                {
                    using (FileStream stream = new FileStream(mainDataPath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        //output_log.txt
                        MDebug.Log("Enabling output_log...");
                        stream.Position = mainDataPosition + outputLogOffset;
                        stream.WriteByte(0x01);
                        stream.Close();
                    }
                }
                if (cfgScreenSkip != skipCfg)
                {
                    using (FileStream stream = new FileStream(mainDataPath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        MDebug.Log("Changing config screen skip...");
                        stream.Position = mainDataPosition + cfgScreenOffset;
                        if (cfgScreenSkip)
                            stream.WriteByte(0x00);
                        else
                            stream.WriteByte(0x01);
                        stream.Close();
                    }
                }
                if(signleInstancePatch && !singleInstance)
                {
                    MDebug.Log("Single instance was already disabled (-mscloader-sipatch flag) skipping...");
                }
                if (signleInstancePatch && singleInstance)
                {
                    using (FileStream stream = new FileStream(mainDataPath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        MDebug.Log("Disabling single instance (-mscloader-sipatch flag)...");
                        stream.Position = mainDataPosition + signleInstanceOffset;
                        stream.WriteByte(0x00);
                        stream.Close();
                    }
                }
                if(!signleInstancePatch && !singleInstance)
                {
                    using (FileStream stream = new FileStream(mainDataPath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        MDebug.Log("Reverting single instance to default (no -mscloader-sipatch flag)...");
                        stream.Position = mainDataPosition + signleInstanceOffset;
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

        private static void DeleteObsoleteFiles()
        {
            MDebug.Log("Deleting obsolete files...");
            string[] files = File.ReadAllLines(Path.Combine(managedPath, "obsolete.txt"));
            foreach (string file in files)
            {
                try
                {
                    MDebug.Log($"Deleting {file}...");
                    if (File.Exists(Path.Combine(managedPath, file)))
                        File.Delete(Path.Combine(managedPath, file));
                }
                catch (Exception e)
                {
                    MDebug.Log("Failed to delete file");
                    MDebug.Log(e.ToString(), true);
                }
            }
            File.Delete(Path.Combine(managedPath, "obsolete.txt"));
        }

        private static void AssemblyWatcher(object sender, AssemblyLoadEventArgs args)
        {
            //System.dll -> Loaded at very end.
            if (args.LoadedAssembly.GetName().Name == "System")
            {
                AppDomain.CurrentDomain.AssemblyLoad -= AssemblyWatcher; //Unsubscribe from event when done.
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

        public static bool splashSkip = false;
        public static bool cfgScreenSkip = false;

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
                if (UnityEngine.EventSystems.EventSystem.current != null)
                {
                    if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                    {
                        return false;
                    }
                }
                return true;

            }
        }
        [HarmonyPatch(typeof(HarmonyInstance), nameof(HarmonyInstance.UnpatchAll) , new[] { typeof(string) })]
        class HarmonyAntiGriefing 
        { 
            static bool Prefix(ref string harmonyID)
            {
                if(harmonyID == null || harmonyID == "")
                {
                    ModConsole.Error("<b>[UnpachAll()]</b> Please specify <b>harmonyID</b> for unpatching!");
                    return false;
                }
                if (harmonyID.StartsWith("MSCLoader"))
                {
                    ModConsole.Error("<b>[UnpachAll()]</b> Cannot unpatch MSCLoader patches, stop breaking shit!");
                    return false;

                }
                return true;
            }
        }
        [HarmonyPatch(typeof(HarmonyInstance), nameof(HarmonyInstance.Create), new[] { typeof(string) })]
        class HarmonyTrack
        {
            static bool Prefix(ref string id)
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("id cannot be null");
                }
                string assemblyLocation = string.Empty;
                StackTrace trace = new StackTrace(true);
                foreach (StackFrame frame in trace.GetFrames())
                {
                    MethodBase method = frame.GetMethod();
                    if (method.DeclaringType == null) break;
                    if (method.DeclaringType.Assembly.Location.Contains("Mods") || method.DeclaringType.Assembly.Location.Contains("References"))
                    {
                        assemblyLocation = method.DeclaringType.Assembly.Location;
                        break;
                    }
                   /* if (!method.DeclaringType.FullName.StartsWith("MSCLoader") && !method.DeclaringType.FullName.StartsWith("System.Diagnostics"))
                    {
                        assemblyLocation = method.DeclaringType.Assembly.Location;
                        break;
                    }*/
                }
                MSCLInternal.AddHarmonyInstance(assemblyLocation, id);
                return true;
            }
        }
        [HarmonyPatch(typeof(System.Diagnostics.Process), "Start_common", new[] { typeof(ProcessStartInfo), typeof(Process)})]
        class ProcessStartPatch
        {
            static bool Prefix(ref ProcessStartInfo startInfo, ref Process process)
            {
                try
                {
                    string assemblyFullName = string.Empty;
                    string assemblyLocation = string.Empty;
                    StackTrace trace = new StackTrace(true);
                    foreach (StackFrame frame in trace.GetFrames())
                    {
                        MethodBase method = frame.GetMethod();
                        if (method.DeclaringType == null) break;
                        if (method.DeclaringType.Assembly.Location.Contains("Mods") || method.DeclaringType.Assembly.Location.Contains("References"))
                        {
                            assemblyFullName = method.DeclaringType.FullName;
                            assemblyLocation = method.DeclaringType.Assembly.Location;
                            break;
                        }
                        if (!method.DeclaringType.FullName.StartsWith("MSCLoader") && !method.DeclaringType.FullName.StartsWith("System.Diagnostics"))
                        {
                            assemblyFullName = method.DeclaringType.FullName;
                            assemblyLocation = method.DeclaringType.Assembly.Location;
                            break;
                        }
                    }
                    if (startInfo.FileName.Contains("cmd.exe") || startInfo.FileName.Contains("powershell.exe") || startInfo.FileName.StartsWith("cmd") || startInfo.FileName.StartsWith("powershell") || startInfo.FileName.StartsWith("conhost") || startInfo.FileName.StartsWith("cscript") || startInfo.FileName.StartsWith("wscript") || startInfo.FileName.StartsWith("conhost.exe") || startInfo.FileName.Contains(".bat") || startInfo.FileName.Contains(".ps1") || startInfo.FileName.Contains(".cmd") || startInfo.FileName.Contains(".vbs") || startInfo.FileName.Contains(".js") || startInfo.FileName.Contains(".scr"))
                    {
                        ModUI.ShowMessage($"Malicious execution detected{Environment.NewLine}{Environment.NewLine}From file: <color=yellow>{Path.GetFileName(assemblyLocation)}</color>{Environment.NewLine}{Environment.NewLine}Attempt to execute: <color=yellow>{startInfo.FileName}</color>{Environment.NewLine}With arguments: <color=yellow>{startInfo.Arguments}</color>{Environment.NewLine}{Environment.NewLine}More details in output_log", "Malicious Behavior");

                        Console.WriteLine($"=========== WARNING ===============");
                        Console.WriteLine($"[{assemblyFullName}]: {assemblyLocation}");
                        Console.WriteLine($"Attempt to execute: {startInfo.FileName}");
                        Console.WriteLine($"Arguments: {startInfo.Arguments}");
                        Console.WriteLine($"CreateNoWindow: {startInfo.CreateNoWindow}");
                        Console.WriteLine($"UseShellExecute: {startInfo.UseShellExecute}");
                        Console.WriteLine($"WindowStyle: {startInfo.WindowStyle}");
                        Console.WriteLine($"WorkingDirectory: {startInfo.WorkingDirectory}");
                        Console.WriteLine($"FullTrace:");
                        Console.WriteLine($"{trace}");
                        Console.WriteLine($"==================================");
                        throw new InvalidOperationException("Operation not allowed due to security reasons.");
                    }
                    if (startInfo.WindowStyle == ProcessWindowStyle.Hidden || startInfo.WindowStyle == ProcessWindowStyle.Minimized || startInfo.CreateNoWindow)
                    {
                        Console.WriteLine($"=========== WARNING ===============");
                        Console.WriteLine($"[{assemblyFullName}]: {assemblyLocation}");
                        Console.WriteLine($"Attempt to execute: {startInfo.FileName}");
                        Console.WriteLine($"Arguments: {startInfo.Arguments}");
                        Console.WriteLine($"CreateNoWindow: {startInfo.CreateNoWindow}");
                        Console.WriteLine($"UseShellExecute: {startInfo.UseShellExecute}");
                        Console.WriteLine($"WindowStyle: {startInfo.WindowStyle}");
                        Console.WriteLine($"WorkingDirectory: {startInfo.WorkingDirectory}");
                        Console.WriteLine($"FullTrace:");
                        Console.WriteLine($"{trace}");
                        Console.WriteLine($"==================================");
                        ModUI.ShowMessage($"Suspicious execution detected{Environment.NewLine}{Environment.NewLine}File name: <color=yellow>{Path.GetFileName(assemblyLocation)}</color>{Environment.NewLine}{Environment.NewLine}Attempt to execute: <color=yellow>{startInfo.FileName}</color>{Environment.NewLine}With arguments: <color=yellow>{startInfo.Arguments}</color>{Environment.NewLine}{Environment.NewLine}More details in output_log", "Suspicious Behaviour");
                        throw new InvalidOperationException("Operation not allowed due to security reasons.");
                    }
                    Console.WriteLine($"=========== WARNING ===============");
                    Console.WriteLine($"[{assemblyFullName}]: {assemblyLocation}");
                    Console.WriteLine($"Execute: {startInfo.FileName}");
                    Console.WriteLine($"Arguments: {startInfo.Arguments}");
                    Console.WriteLine($"CreateNoWindow: {startInfo.CreateNoWindow}");
                    Console.WriteLine($"UseShellExecute: {startInfo.UseShellExecute}");
                    Console.WriteLine($"WindowStyle: {startInfo.WindowStyle}");
                    Console.WriteLine($"WorkingDirectory: {startInfo.WorkingDirectory}");
                    Console.WriteLine($"FullTrace:");
                    Console.WriteLine($"{trace}");
                    Console.WriteLine($"==================================");
                    if (ModMenu.ShowWarnStatus())
                    {
                        ModConsole.Warning($"[{assemblyFullName}] External execution detected (you can hide this warning in the Main Settings)");
                        ModConsole.Warning($"Executed: <b>{startInfo.FileName}</b> Arguments: <b>{startInfo.Arguments}</b>");
                    }
                    if (ModMenu.ForbidExternalLaunch())
                    {
                        ModConsole.Error($"[{assemblyFullName}] External execution forbidden by your settings, to allow this mod to execute below command, please change the settings in the main settings");
                        ModConsole.Warning($"Command: <b>{startInfo.FileName}</b> Arguments: <b>{startInfo.Arguments}</b>");
                        throw new InvalidOperationException("Operation not allowed due to security reasons.");
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error {ex.Message}");
                }
                throw new InvalidOperationException("Operation not allowed due to security reasons.");
            }
        }

        public static void SkipSplashScreen(bool skip)
        {
            if (skip)
            {
                MDebug.Log("Skipping splash screen...");
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
                SkipSplashScreen(splashSkip);
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