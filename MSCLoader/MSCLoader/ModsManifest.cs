using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MSCLoader
{
    internal class ModsManifest
    {
        public string modID;
        public string version;
        public string description;
        public ManifestLinks links =new ManifestLinks();
        public ManifestIcon icon = new ManifestIcon();
        public ManifestMinReq minimumRequirements = new ManifestMinReq();
        public ManifestModConflict modConflicts = new ManifestModConflict();
        public ManifestModRequired requiredMods = new ManifestModRequired();
        public string sign;
        public string sid_sign;
        public byte type;
    }
    internal class ManifestLinks
    {
        public string nexusLink = null;
        public string rdLink = null;
        public string githubLink = null;
    }
    internal class ManifestIcon
    {
        public string iconFileName = null;
        public bool isIconRemote = false;
        public bool isIconUrl = false;
    }
    internal class ManifestMinReq
    {
        public string MSCLoaderVer = null;
        public uint MSCbuildID = 0;
        public bool disableIfVer = false;
    }
    internal class ManifestModConflict
    {
        public string modIDs = null;
        public string customMessage = null;
        public bool disableIfConflict = false;
    }
    internal class ManifestModRequired
    {
        public string modID = null;
        public string minVer = null;
        public string customMessage = null;
    }
    internal class ManifestStuff
    {
        public static void CreateManifest(Mod mod)
        {
            string steamID;
            if (ModLoader.CheckSteam())
            {
                steamID = Steamworks.SteamUser.GetSteamID().ToString();
                try
                {
                    ModsManifest mm = new ModsManifest
                    {
                        modID = mod.ID,
                        version = mod.Version,
                        description = "<i>No description provided...</i>",
                        sign = AzjatyckaMatematyka(mod.fileName),
                        sid_sign = ModLoader.MurzynskaMatematyka(steamID+mod.ID),
                        type = 0
                    };
                    string path = ModLoader.GetMetadataFolder(string.Format("{0}.json", mod.ID));
                    if (File.Exists(path))
                    {
                        ModConsole.Error("Metadata file already exists, to update use update command");
                        return;
                    }
                    string serializedData = JsonConvert.SerializeObject(mm, Formatting.Indented);
                    File.WriteAllText(path, serializedData);
                    ModConsole.Print("<color=green>Raw metadata file created successfully</color>");
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    UnityEngine.Debug.Log(e);
                }
            }
            else
            {
                ModConsole.Error("No valid steam detected");
            }

        }
        public static void UpdateManifest(Mod mod)
        {
            if (!File.Exists(ModLoader.GetMetadataFolder(string.Format("{0}.json", mod.ID))))
            {
                ModConsole.Error("Metadata file doesn't exists, to create use create command");
                return;
            }
            if(mod.RemMetadata == null)
            {
                ModConsole.Error(string.Format("Your metadata file doesn't seem to be public, you need to upload first before you can update file.{0}If you want to just recreate metadata, delete old file and use create command", Environment.NewLine));
                return;
            }
            string steamID;
            if (ModLoader.CheckSteam())
            {
                steamID = Steamworks.SteamUser.GetSteamID().ToString();
                if (mod.RemMetadata.sid_sign != ModLoader.MurzynskaMatematyka(steamID + mod.ID))
                {
                    ModConsole.Error("This mod doesn't belong to you, can't continue");
                    return;
                }
                try
                {
                    ModsManifest umm = mod.metadata;
                    Version v1 = new Version(mod.Version);
                    Version v2 = new Version(mod.metadata.version);
                    switch (v1.CompareTo(v2))
                    {
                        case 0:
                            ModConsole.Error(string.Format("Mod version {0} is same as current metadata version {1}, nothing to update.",mod.Version, mod.metadata.version));
                            break;
                        case 1:
                            umm.version = mod.Version;
                            umm.sign = AzjatyckaMatematyka(mod.fileName);
                            string msad = ModLoader.GetMetadataFolder(string.Format("{0}.json", mod.ID));
                            string serializedData = JsonConvert.SerializeObject(umm, Formatting.Indented);
                            File.WriteAllText(msad, serializedData);
                            ModConsole.Print("<color=green>Metadata file updated successfully, you can upload it now!</color>");
                            break;
                        case -1:
                            ModConsole.Error(string.Format("Mod version {0} is <b>earlier</b> than current metadata version {1}, cannot update.", mod.Version, mod.metadata.version));
                            break;
                    }
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    UnityEngine.Debug.Log(e);

                }
            }
            else
            {
                ModConsole.Error("No valid steam detected");
            }
        }
        internal static string AzjatyckaMatematyka(string fn)
        {
            byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(File.ReadAllBytes(fn));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

}
