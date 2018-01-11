using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
