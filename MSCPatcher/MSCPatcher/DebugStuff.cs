using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MSCPatcher
{
    class DebugStuff
    {

        static string monoPath = null;

        public static int checkDebugStatus()
        {
            if (Form1.mscPath != "(unknown)")
            {
                monoPath = Path.Combine(Form1.mscPath, @"mysummercar_Data\Mono\mono.dll");
                switch(Form1.MD5HashFile(monoPath))
                {
                    case MD5FileHashes.mono32normal:
                        return 1;
                    case MD5FileHashes.mono32debug:
                        return 2;
                    case MD5FileHashes.mono64normal:
                        return 3;
                    case MD5FileHashes.mono64debug:
                        return 4;
                    default:
                        return 0;
                }
                
            }
            return 0;
        }

        public static void EnableDebugging(bool is64, string modpath)
        {
            Patcher.DeleteIfExists(String.Format("{0}.normal", monoPath));
            File.Move(monoPath, String.Format("{0}.normal", monoPath));
            if(is64)
                File.Copy(Path.GetFullPath(Path.Combine(@"Debug\64", "mono.dll")), monoPath, true);
            else
                File.Copy(Path.GetFullPath(Path.Combine(@"Debug\32", "mono.dll")), monoPath, true);
            Log.Write("Copying debug file.....mono.dll");
            File.Copy(Path.GetFullPath(Path.Combine("Debug", "pdb2mdb.exe")), Path.Combine(modpath, "pdb2mdb.exe"), true);
            Log.Write("Copying debug file.....pdb2mdb.exe");
            File.Copy(Path.GetFullPath(Path.Combine("Debug", "debug.bat")), Path.Combine(modpath,"debug.bat"), true);
            Log.Write("Copying debug file.....debug.bat");
            ProcessStartInfo startInfo = new ProcessStartInfo(Path.Combine("Debug", "en_debugger.bat"));
            startInfo.Verb = "runas";
            Process en_debug = Process.Start(startInfo);
            en_debug.WaitForExit();
            if (Environment.GetEnvironmentVariable("DNSPY_UNITY_DBG", EnvironmentVariableTarget.Machine) != null)
                MessageBox.Show("Debugging Enabled successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Failed to set debug variable!", "Failed!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void DisableDebugging()
        {
            Patcher.DeleteIfExists(monoPath);
            File.Move(String.Format("{0}.normal", monoPath), monoPath);
            Log.Write("Recovering backup.....mono.dll");
            MessageBox.Show("Debugging Disabled successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
