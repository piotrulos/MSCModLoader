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
            try
            {
                ModsManifest mm = new ModsManifest
                {
                    modID = mod.ID,
                    version = mod.Version,
                    description = "<i>No description provided...</i>",
                    sign = AzjatyckaMatematyka(mod.fileName),
                    type = 0                    
                };
                string path = ModLoader.GetMetadataFolder(string.Format("{0}.json", mod.ID));
                if(File.Exists(path))
                {
                    ModConsole.Error("Metadata file already exists");
                    return;
                }
                string serializedData = JsonConvert.SerializeObject(mm, Formatting.Indented);
                File.WriteAllText(path, serializedData);
                ModConsole.Print("<color=green>Raw metadata file created successfully</color>");
            }
            catch (Exception e)
            {
                ModConsole.Error(e.Message);
            }

        }
        internal static string AzjatyckaMatematyka(string fn)
        {
            byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(File.ReadAllBytes(fn));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

}
