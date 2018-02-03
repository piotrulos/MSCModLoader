using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MSCPatcher
{
    class Patch64
    {

        public static int check64status()
        {

            if (Form1.mscPath != "(unknown)")
            {
                string exePath = Path.Combine(Form1.mscPath, "mysummercar.exe");
                switch (Form1.MD5HashFile(exePath))
                {
                    case MD5FileHashes.exe32:
                        return 1;
                    case MD5FileHashes.exe64:
                        return 2;
                    default:
                        return 0;
                }

            }
            return 0;
        }

        public static void install64()
        {
            if (Form1.mscPath != "(unknown)")
            {
                //backup
                File.Move(Path.Combine(Form1.mscPath, "mysummercar.exe"), String.Format("{0}.32", Path.Combine(Form1.mscPath, "mysummercar.exe")));
                if (File.Exists(Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll.normal")))
                {
                    File.Move(Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll.normal"), String.Format("{0}.32", Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll")));
                }
                else
                {
                    File.Move(Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll"), String.Format("{0}.32", Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll")));
                }
                File.Move(Path.Combine(Form1.mscPath, @"mysummercar_Data\Plugins\CSteamworks.dll"), String.Format("{0}.32", Path.Combine(Form1.mscPath, @"mysummercar_Data\Plugins\CSteamworks.dll")));

                //copy
                File.Copy(Path.GetFullPath(Path.Combine("64bit", "mysummercar.exe")), Path.Combine(Form1.mscPath, "mysummercar.exe"), true);
                Log.Write("Copying file.....mysummercar.exe");
                File.Copy(Path.GetFullPath(Path.Combine("64bit", "steam_api64.dll")), Path.Combine(Form1.mscPath, "steam_api64.dll"), true);
                Log.Write("Copying file.....steam_api64.dll");
                File.Copy(Path.GetFullPath(Path.Combine("64bit", @"mysummercar_Data\Mono\mono.dll")), Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll"), true);
                Log.Write("Copying file.....mono.dll");
                File.Copy(Path.GetFullPath(Path.Combine("64bit", @"mysummercar_Data\Plugins\CSteamworks.dll")), Path.Combine(Form1.mscPath, @"mysummercar_Data\Plugins\CSteamworks.dll"), true);
                Log.Write("Copying file.....CSteamworks.dll");
                MessageBox.Show("64-bit patch installed successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public static void remove64()
        {
            if (Form1.mscPath != "(unknown)")
            {
                try
                {
                    if (File.Exists(Path.Combine(Form1.mscPath, "mysummercar.exe.32")))
                    {
                        Patcher.DeleteIfExists(Path.Combine(Form1.mscPath, "mysummercar.exe"));
                        Patcher.DeleteIfExists(Path.Combine(Form1.mscPath, "steam_api64.dll"));
                        File.Move(Path.Combine(Form1.mscPath, "mysummercar.exe.32"), Path.Combine(Form1.mscPath, "mysummercar.exe"));
                    }
                    else
                        throw new Exception("Backup file not found. Please verify file integrity on steam before continue!");
                    if (File.Exists(Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll.32")))
                    {
                        Patcher.DeleteIfExists(Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll"));
                        Patcher.DeleteIfExists(Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll.normal"));
                        File.Move(Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll.32"), Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll"));
                    }
                    else
                        throw new Exception("Backup file not found. Please verify file integrity on steam before continue!");
                    if (File.Exists(Path.Combine(Form1.mscPath, @"mysummercar_Data\Plugins\CSteamworks.dll.32")))
                    {
                        Patcher.DeleteIfExists(Path.Combine(Form1.mscPath, @"mysummercar_Data\Plugins\CSteamworks.dll"));
                        File.Move(Path.Combine(Form1.mscPath, @"mysummercar_Data\Plugins\CSteamworks.dll.32"), Path.Combine(Form1.mscPath, @"mysummercar_Data\Plugins\CSteamworks.dll"));
                    }
                    else
                        throw new Exception("Backup file not found. Please verify file integrity on steam before continue!");

                    MessageBox.Show("32-bit version restored successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Failed to restore 32-bit version.{1}{1}Error details:{1}{0}", e.Message, Environment.NewLine), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

