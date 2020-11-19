using Ionic.Zip;
using System;
using System.IO;
using System.Windows.Forms;

namespace MSCPatcher
{
    class Patcher
    {
        public static void DeleteIfExists(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
                Log.Write(string.Format("Removing file.....{0}", Path.GetFileName(filename)));
            }
        }
 
        public static void ProcessReferences(string mscPath, bool remove)
        {
            DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll"));
            DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll.mdb"));
            DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.pdb"));
            DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\uAudio.dll"));
            DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Ionic.Zip.dll"));
            if (!remove)
            {
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
                if (File.Exists(Path.GetFullPath(Path.Combine("Ionic.Zip.dll", ""))))
                {
                    File.Copy(Path.GetFullPath(Path.Combine("Ionic.Zip.dll", "")), Path.Combine(mscPath, @"mysummercar_Data\Managed\Ionic.Zip.dll"));
                    Log.Write("Copying new file.....Ionic.Zip.dll");
                }
            }
            try
            {
                if (!File.Exists(Path.Combine("References.zip", "")))
                    throw new FileNotFoundException("File \"References.zip\" not found, please redownload modlaoder and unpack all files", "References.zip");
                string zip = Path.Combine("References.zip","");
                if (!ZipFile.IsZipFile(zip))
                {
                    throw new Exception("Failed read zip file");
                }
                ZipFile zip1 = ZipFile.Read(zip);
                foreach (ZipEntry zz in zip1)
                {
                    if (remove)
                    {
                        DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\" + zz.FileName));
                    }
                    else
                    {
                        Log.Write("Copying new file....." + zz.FileName);
                        zz.Extract(Path.Combine(mscPath, @"mysummercar_Data\Managed\"), ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error while reading references: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Write("Error", true, true);
                Log.Write(ex.Message);
                Log.Write(ex.ToString());
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

            Log.Write("Copying Core Assets Completed!", false, false);
        }
    }
}
