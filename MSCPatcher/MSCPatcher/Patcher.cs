using System.IO;

namespace MSCPatcher
{
    class Patcher
    {
        public static void DeleteIfExists(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
                Log.Write(string.Format("Removing.....{0}", Path.GetFileName(filename)));
            }
        }

        public static void CopyReferences(string mscPath)
        {
            DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll"));
            DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll.mdb"));
            DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.pdb"));
            DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\uAudio.dll"));

            if (File.Exists(Path.GetFullPath(Path.Combine("MSCLoader.dll", ""))))
            {
                File.Copy(Path.GetFullPath(Path.Combine("MSCLoader.dll", "")), Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll"));
                Log.Write("Copying new file.....MSCLoader.dll");
            }

            if (File.Exists(Path.GetFullPath(Path.Combine("MSCLoader.dll.mdb", ""))))
            {
                File.Copy(Path.GetFullPath(Path.Combine("MSCLoader.dll.mdb", "")), Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll.mdb"));
                Log.Write("Copying new file.....MSCLoader.dll.mdb");
            }

            if (File.Exists(Path.GetFullPath(Path.Combine("MSCLoader.pdb", ""))))
            {
                File.Copy(Path.GetFullPath(Path.Combine("MSCLoader.pdb", "")), Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.pdb"));
                Log.Write("Copying new file.....MSCLoader.pdb");
            }
            if (!File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\System.Xml.dll")))
            {
                File.Copy(Path.GetFullPath(Path.Combine("System.Xml.dll", "")), Path.Combine(mscPath, @"mysummercar_Data\Managed\System.Xml.dll"));
                Log.Write("Copying new file.....System.Xml.dll");
            }
            if (!File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Newtonsoft.Json.dll")))
            {
                File.Copy(Path.GetFullPath(Path.Combine("Newtonsoft.Json.dll", "")), Path.Combine(mscPath, @"mysummercar_Data\Managed\Newtonsoft.Json.dll"));
                Log.Write("Copying new file.....Newtonsoft.Json.dll");
            }
            if (!File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\System.Data.dll")))
            {
                File.Copy(Path.GetFullPath(Path.Combine("System.Data.dll", "")), Path.Combine(mscPath, @"mysummercar_Data\Managed\System.Data.dll"));
                Log.Write("Copying new file.....System.Data.dll");
            }
            if (!File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\System.Runtime.Serialization.dll")))
            {
                File.Copy(Path.GetFullPath(Path.Combine("System.Runtime.Serialization.dll", "")), Path.Combine(mscPath, @"mysummercar_Data\Managed\System.Runtime.Serialization.dll"));
                Log.Write("Copying new file.....System.Runtime.Serialization.dll");
            }
            if (!File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\NAudio.dll")))
            {
                File.Copy(Path.GetFullPath(Path.Combine("NAudio.dll", "")), Path.Combine(mscPath, @"mysummercar_Data\Managed\NAudio.dll"));
                Log.Write("Copying new file.....NAudio.dll");
            }
            if (!File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\NVorbis.dll")))
            {
                File.Copy(Path.GetFullPath(Path.Combine("NVorbis.dll", "")), Path.Combine(mscPath, @"mysummercar_Data\Managed\NVorbis.dll"));
                Log.Write("Copying new file.....NVorbis.dll");
            }
        }

        public static void CopyCoreAssets(string modPath)
        {
            Log.Write("Copying Core Assets.....MSCLoader_Core");

            if (!Directory.Exists(Path.Combine(modPath, @"Assets\MSCLoader_Core")))
                Directory.CreateDirectory(Path.Combine(modPath, @"Assets\MSCLoader_Core"));
            else
                File.Delete(Path.Combine(modPath, @"Assets\MSCLoader_Core\core.unity3d"));

            File.Copy(Path.GetFullPath(Path.Combine(@"Assets\MSCLoader_Core", "core.unity3d")), Path.Combine(modPath, @"Assets\MSCLoader_Core\core.unity3d"));

            Log.Write("Copying Core Assets.....MSCLoader_Settings");

            if (!Directory.Exists(Path.Combine(modPath, @"Assets\MSCLoader_Settings")))
                Directory.CreateDirectory(Path.Combine(modPath, @"Assets\MSCLoader_Settings"));
            else
                File.Delete(Path.Combine(modPath, @"Assets\MSCLoader_Settings\settingsui.unity3d"));

            File.Copy(Path.GetFullPath(Path.Combine(@"Assets\MSCLoader_Settings", "settingsui.unity3d")), Path.Combine(modPath, @"Assets\MSCLoader_Settings\settingsui.unity3d"));

            Log.Write("Copying Core Assets.....MSCLoader_Console");

            if (!Directory.Exists(Path.Combine(modPath, @"Assets\MSCLoader_Console")))
                Directory.CreateDirectory(Path.Combine(modPath, @"Assets\MSCLoader_Console"));
            else
                File.Delete(Path.Combine(modPath, @"Assets\MSCLoader_Console\console.unity3d"));

            File.Copy(Path.GetFullPath(Path.Combine(@"Assets\MSCLoader_Console", "console.unity3d")), Path.Combine(modPath, @"Assets\MSCLoader_Console\console.unity3d"));

            Log.Write("Copying Core Assets Completed!", false, true);
        }
    }
}
