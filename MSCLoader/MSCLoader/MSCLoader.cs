using System;
using System.IO;
using System.Reflection;
using Harmony;
using IniParser.Model;
using IniParser;

namespace MSCLoader
{
#pragma warning disable CS1591
    public static class MSCLoader
    {
        static byte[] data = { 0x41, 0x6d, 0x69, 0x73, 0x74, 0x65, 0x63, 0x68, 0x0d, 0x00, 0x00, 0x00, 0x4d, 0x79, 0x20, 0x53, 0x75, 0x6d, 0x6d, 0x65, 0x72, 0x20, 0x43, 0x61, 0x72 };
        static long offset = 0;

        //Entry point for doorstop
        public static void Main(string[] args)
        {
            OutputLogChecker(); //Enable output_log after possible game update
            AppDomain.CurrentDomain.AssemblyLoad += AssemblyWatcher;
        }

        private static void OutputLogChecker()
        {
            try
            {
                bool enLog = false;
                offset = FindBytes(Path.Combine("", @"mysummercar_Data\mainData"), data);

                using (FileStream stream = File.OpenRead(Path.Combine("", @"mysummercar_Data\mainData")))
                {
                    stream.Position = offset + 115;
                    if (stream.ReadByte() == 1)
                    {
                        enLog = false;
                    }
                    else
                    {
                        enLog = true;

                    }
                    stream.Close();
                }

                if (enLog)
                {
                    using (var stream = new FileStream(Path.Combine("", @"mysummercar_Data\mainData"), FileMode.Open, FileAccess.ReadWrite))
                    {
                        //output_log.txt
                        stream.Position = offset + 115;
                        stream.WriteByte(0x01);
                        stream.Close();
                    }
                }
            }
            catch (Exception e)
            {
                //Generate early crash file (before output_log can happen)
                using (TextWriter tw = File.CreateText("MSCLOADER_CRASH.txt"))
                {

                    tw.WriteLine(e.ToString());
                    tw.WriteLine(e.Message);
                    tw.Flush();
                }
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
                if (File.Exists("ModLoaderSettings.json"))
                    File.Delete("ModLoaderSettings.json");
                IniData ini = new FileIniDataParser().ReadFile("doorstop_config.ini");
                ini.Configuration.AssigmentSpacer = "";
                cfg = ini["MSCLoader"]["mods"];
                string skipIntro = ini["MSCLoader"]["skipIntro"];
                if (!bool.TryParse(skipIntro, out introSkip))
                {
                    introSkip = false;
                    Console.WriteLine($"[FAIL] Parse failed, readed value: {skipIntro}");
                }
                HarmonyInstance.Create(nameof(MSCLoader)).PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                //Generate early crash file (before output_log can happen)
                using (TextWriter tw = File.CreateText("MSCLOADER_CRASH.txt"))
                {

                    tw.WriteLine(ex.ToString());
                    tw.WriteLine(ex.Message);
                    tw.Flush();
                }
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
#pragma warning restore CS1591 