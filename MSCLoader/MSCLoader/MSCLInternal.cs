using System.Net;
using System.Text;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel.Design;
using UnityStandardAssets.Water;

namespace MSCLoader;

internal class MSCLInternal
{
    internal static bool ValidateVersion(string version)
    {
        try
        {
            new Version(version);
        }
        catch
        {
            ModConsole.Error($"Invalid version: {version}{Environment.NewLine}Please use proper version format: (0.0 or 0.0.0 or 0.0.0.0)");
            return false;
        }
        return true;
    }
    internal static string MSCLDataRequest(string reqPath, Dictionary<string, string> data)
    {
        System.Collections.Specialized.NameValueCollection msclData = new System.Collections.Specialized.NameValueCollection { { "msclData", JsonConvert.SerializeObject(data) } };
        string response = "";
        using (WebClient MSCLDconn = new WebClient())
        {
            MSCLDconn.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
            try
            {
                byte[] raw = MSCLDconn.UploadValues($"{ModLoader.serverURL}/{reqPath}", "POST", msclData);
                response = Encoding.UTF8.GetString(raw, 0, raw.Length);
            }
            catch (Exception e)
            {
                ModConsole.Error($"Request failed with error: {e.Message}");
                Console.WriteLine(e);
                response = "error";
            }
        }
#if DEBUG
        ModConsole.Warning(response);
#endif
        return response;
    }

    internal static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }
        DirectoryInfo[] dirs = dir.GetDirectories();

        Directory.CreateDirectory(destDirName);

        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }

        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }

    internal static void SaveMSCLDataFile()
    {
        if (!ES2.Exists(ModLoader.GetMetadataFolder("MSCLData.bin")))
        {
            ES2.Save(new byte[1] { 0x01 }, $"{ModLoader.GetMetadataFolder("MSCLData.bin")}?tag=MSCLData");
        }
        using (ES2Writer writer = ES2Writer.Create(ModLoader.GetMetadataFolder("MSCLData.bin")))
        {
            JsonSerializerSettings config = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
            for (int i = 0; i < ModLoader.Instance.MetadataUpdateList.Count; i++)
            {
                Mod mod = ModLoader.GetModByID(ModLoader.Instance.MetadataUpdateList[i], true);
                if(mod.metadata == null) continue;
                string serializedData = JsonConvert.SerializeObject(mod.metadata, config);
                byte[] bytes = Encoding.UTF8.GetBytes(serializedData);
                writer.Write(bytes, $"{mod.ID}||metadata");
            }
            writer.Save();
        }
    }
    internal static void SaveMSCLDataFile(Mod mod)
    {
        if (!ES2.Exists(ModLoader.GetMetadataFolder("MSCLData.bin")))
        {
            ES2.Save(new byte[1] { 0x01 }, $"{ModLoader.GetMetadataFolder("MSCLData.bin")}?tag=MSCLData");
        }
        if (mod.metadata == null) return;
        JsonSerializerSettings config = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.None
        };
        string serializedData = JsonConvert.SerializeObject(mod.metadata, config);
        byte[] bytes = Encoding.UTF8.GetBytes(serializedData);

        ES2.Save(bytes, $"{ModLoader.GetMetadataFolder("MSCLData.bin")}?tag={mod.ID}||metadata");
    }
    internal static void LoadMSCLDataFile()
    {
        if (!ES2.Exists(ModLoader.GetMetadataFolder("MSCLData.bin")))
        {
            ES2.Save(new byte[1] { 0x01 }, $"{ModLoader.GetMetadataFolder("MSCLData.bin")}?tag=MSCLData");
            string[] oldm = Directory.GetFiles(ModLoader.GetMetadataFolder(""), "*.json");
            if (oldm.Length > 0)
            {
                for (int i = 0; i < oldm.Length; i++)
                {
                    File.Delete(oldm[i]);
                }
            }
            return;
        }
        using(ES2Reader reader = ES2Reader.Create(ModLoader.GetMetadataFolder("MSCLData.bin")))
        {
            for (int i = 0; i < ModLoader.Instance.actualModList.Length; i++)
            {
                Mod mod = ModLoader.Instance.actualModList[i];
                if(!reader.TagExists($"{mod.ID}||metadata")) continue; 
                byte[] bytes = reader.ReadArray<byte>($"{mod.ID}||metadata");
                string serializedData = Encoding.UTF8.GetString(bytes);
                mod.metadata = JsonConvert.DeserializeObject<MSCLData>(serializedData);
            }
        }
    }
    internal static bool MSCLDataExists(string modID)
    {
        return ES2.Exists($"{ModLoader.GetMetadataFolder("MSCLData.bin")}?tag={modID}||metadata");
    }
}

