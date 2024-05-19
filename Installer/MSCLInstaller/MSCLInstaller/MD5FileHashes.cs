using System.Security.Cryptography;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MSCLInstaller
{
    public static class MD5FileHashes
    {
        /*
         * mono.dll MD5 hashes (for enabling debug mode):
         * 
         * F190C7ECFE414FB407137C1D95AC310E (64-bit, normal)
         * 3EE5C3BD42B61AE820A028AF35DF99C0 (64-bit, debug)
         * 
         */
        public const string mono64normal = "F190C7ECFE414FB407137C1D95AC310E";
        public const string mono64debug = "3EE5C3BD42B61AE820A028AF35DF99C0";

        /*
         * Game Executable
         * 
         * E9F1D7E11359DA995881B9280E1726B3 (32-bit, msc vanilla old)
         * 7C37795F08588D952C4B3289DE7AB2EA (64-bit, msc vanilla dx11)
         * 3C3F1460A074993E7F483F08318A2015 (64-bit, msc vanilla dx9)
         * 
         */
        public const string msc32 = "E9F1D7E11359DA995881B9280E1726B3";
        public const string msc64 = "7C37795F08588D952C4B3289DE7AB2EA";
        public const string msc64d9 = "3C3F1460A074993E7F483F08318A2015";

        public static string MD5HashFile(string fn)
        {
            byte[] hash = MD5.Create().ComputeHash(File.ReadAllBytes(fn));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

    public enum Game
    {
        MSC,
        //For future use
        MSC_IMA,
        MWC
    }

    public static class InstallerHelpers
    {
        public static bool VersionCompare(string file1, string file2)
        {
            Version newVer, oldVer;
            if (File.Exists(file1))
                newVer = new Version(FileVersionInfo.GetVersionInfo(file1).FileVersion);
            else return false;
            if (File.Exists(file2))
                oldVer = new Version(FileVersionInfo.GetVersionInfo(file2).FileVersion);
            else return false;

            switch (newVer.CompareTo(oldVer))
            {
                case 1:
                    Dbg.Log($"{Path.GetFileName(file1)} ({oldVer} => {newVer}) Update available");
                    return true;
                case 0:
                    return false;
                case -1:
                    Dbg.Log($"{Path.GetFileName(file1)} ({oldVer} => {newVer}) Older than installed");
                    return false;
                default:
                    Dbg.Log($"{Path.GetFileName(file1)} ({oldVer} => {newVer}) WTF"); //never happens
                    return false;

            }
        }
        public static void DeleteIfExists(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
                    Dbg.Log($"Deleting file.....{Path.GetFileName(filename)}");
            }
        }
        public static async Task DelayedWork()
        {
            await Task.Delay(500);
        }

    }
}
