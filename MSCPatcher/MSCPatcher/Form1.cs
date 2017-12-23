using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using MSCPatcher.Instructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MSCPatcher
{
    public partial class Form1 : Form
    {
        private static string mscPath = "(unknown)";
        // TODO: make 'managedFolder' an absolute path
        private static string managedFolder = Path.Combine("mysummercar_Data", "Managed");
        private static char pathSeparator = System.IO.Path.DirectorySeparatorChar;
        private string AssemblyPath = Path.Combine(managedFolder, "Assembly-CSharp.dll");
        private string AssemblyFullPath = null;
        private string ModificationsXmlPath = "MSCPatcher.Modifications_MD.xml";

        private static string[] filenames = new string[] {
            "Assembly-CSharp.original.dll",
            "Mono.Cecil.dll",
            "Mono.Cecil.Rocks.dll",
            "MSCLoader.dll",
            "MSCPatcher.exe",
            "System.Xml.dll",
            "uAudio.dll"
        };

        // TODO: move these to a config file
        private static int[] oldfiles    = new int[] { 0, 1, 2, 3, 4, 5 }; //Files to delete if 0.1 is found
        private static int[] patchfiles  = new int[] { 3, 5, 6 };        //Files to install the loader
        private static int[] updatefiles = patchfiles;                 //Files to update the loader
        private static int[] removefiles = patchfiles;               //Files to uninstall the loader

        private string mdPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Path.Combine("MySummerCar", "Mods"));

        private string adPath = Path.GetFullPath(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            string.Format("..{0}LocalLow{0}Amistech{0}My Summer Car{0}Mods", pathSeparator)));

        private string gfPath = "?";

        private string modPath = "";
        private static Dictionary<string, AssemblyDefinition> mLoadedAssemblies = new Dictionary<string, AssemblyDefinition>();

        private bool isgameUpdated   = false; //game updated, remove backup and patch new Assembly-CSharp.dll
        private bool oldPatchFound   = false; //0.1 patch found, recover and patch Assembly-CSharp.original.dll and cleanup unused files
        private bool oldFilesFound   = false; //0.1 files found, but no patch, cleanup files and patch new Assembly-CSharp.dll
        private bool mscloaderUpdate = false; //new MSCLoader.dll found, but no new patch needed.
        FileVersionInfo mscLoaderVersion;
        private XElement mModifications;
        public Form1()
        {
            InitializeComponent();
            //check if idiot/user unpacked this to managed folder.
            if (File.Exists("Assembly-CSharp.dll") || File.Exists("UnityEngine.dll")) QuestionUsersDiscipline();
            if (File.Exists("MSCFolder.txt"))
            {
                mscPath = File.ReadAllText("MSCFolder.txt");
                Log(string.Format("Loaded saved MSC Folder: {0}", mscPath));
            }
            mscPathLabel.Text = mscPath;

            MDlabel.Text = mdPath;
            if (Directory.Exists(mdPath))
            {
                Log(string.Format("Found mods folder in: {0}", mdPath));
                MDlabel.ForeColor = Color.Green;
                MDradio.Checked = true;
            }
            ADlabel.Text = adPath;
            if (Directory.Exists(adPath))
            {
                Log(string.Format("Found mods folder in: {0}", adPath));
                ADlabel.ForeColor = Color.Green;
                ADradio.Checked = true;
            }
            try
            {
                mscLoaderVersion = FileVersionInfo.GetVersionInfo("MSCLoader.dll");
                string currentVersion;
                if (mscLoaderVersion.FileBuildPart != 0)
                    currentVersion = string.Format("{0}.{1}.{2}", mscLoaderVersion.FileMajorPart, mscLoaderVersion.FileMinorPart, mscLoaderVersion.FileBuildPart);
                else
                    currentVersion = string.Format("{0}.{1}", mscLoaderVersion.FileMajorPart, mscLoaderVersion.FileMinorPart);
                //string currentVersion = "0.2.1";
                string version;
                using (WebClient client = new WebClient())
                {
                    client.QueryString.Add("core", "stable");
                    version = client.DownloadString("http://my-summer-car.ml/ver.php");
                }
                if (version.Trim().Length > 8)
                    throw new Exception("Parse Error, please report that problem!");
                int i = currentVersion.CompareTo(version.Trim());
                if (i != 0)
                {
                    Log("");
                    Log(string.Format("MCSLoader v{0}, New version available: v{1}", currentVersion, version.Trim()));
                    if(MessageBox.Show(string.Format("New version is available: v{0}, wanna check it out?", version.Trim()), "MCSLoader v" + currentVersion,MessageBoxButtons.YesNo,MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        try
                        {
                            ProcessStartInfo updateURL = new ProcessStartInfo("https://github.com/piotrulos/MSCModLoader/releases");
                            Process.Start(updateURL);
                            Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Failed to open url!{1}{1}Error details:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (i == 0)
                    Log("");
                    Log(string.Format("{1}MCSLoader v{0} is up to date, no new version found.", currentVersion));
            }
            catch (Exception e)
            {
                Log(string.Format("Check for new version failed with error: {0}", e.Message));
            }
            Log("");
            Log("MSCPatcher ready!");
            Log("=================");

            if (mscPath != "(unknown)")
            {
                mscPathLabel.Text = mscPath;
                AssemblyFullPath = Path.Combine(mscPath, AssemblyPath);
                gfPath = Path.Combine(mscPath, @"Mods");
                GFlabel.Text = gfPath;
                if (Directory.Exists(gfPath))
                {
                    Log(string.Format("Found mods folder in: {0}", gfPath));
                    GFlabel.ForeColor = Color.Green;
                    GFradio.Checked = true;
                }
                Log(string.Format("Game folder set to: {0}{1}", mscPath,Environment.NewLine));
                checkPatchStatus();
            }


        }
        public void Log(string log)
        {
            logBox.AppendText(string.Format("{0}{1}", log, Environment.NewLine));
        }
        public void PatchCheck()
        {
            if(oldFilesFound) RemoveOldFiles();
            else if (isgameUpdated)
            {
                //Remove old backup and patch new game file.
                Log("");
                Log("Removing old backup!");
                Log("=================");
                RemoveIfExists(string.Format("{0}.backup", AssemblyFullPath));
            }
            else if (oldPatchFound)
            {
                if (File.Exists(Path.Combine(mscPath, managedFolder, "Assembly-CSharp.original.dll")))
                {
                    Log("");
                    Log("Recovering backup file!");
                    Log("=================");

                    RemoveIfExists(AssemblyFullPath);
                    File.Move(Path.Combine(mscPath, managedFolder, "Assembly-CSharp.original.dll"), AssemblyFullPath);
                    Log("Recovering.....Assembly-CSharp.original.dll");

                    RemoveOldFiles();
                    PatchThis();
                }
                else
                {
                    MessageBox.Show(string.Format(
                            "0.1 backup file not found in:{1}{0}{1}Can't continue with upgrade{1}{1}Please check integrity files in steam, to recover original file.",
                            Path.Combine(mscPath, managedFolder, "Assembly-CSharp.original.dll"), Environment.NewLine),
                            "Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                    );
                    Log("Error!");
                }
            }
            else if(mscloaderUpdate)
            {
                Log("");
                Log("MSCLoader.dll update!");
                Log("=================");

                foreach (int i in updatefiles)
                {
                    string currentFile = filenames[i];
                    RemoveIfExists(Path.Combine(mscPath, managedFolder, currentFile));
                }

                CopyCoreAssets();

                Log("MSCLoader.dll update successful!");
                Log("");
                MessageBox.Show(
                    "Update successfull!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                checkPatchStatus();
            }
            else
            {
                PatchThis();
            }

        }

        public void PatchThis()
        {
            Log("");
            Log("Start patching files!");
            Log("=================");
            
            RemoveIfExists(Path.Combine(mscPath, managedFolder, "MSCLoader.dll"));
            foreach (int i in patchfiles) {
                string currentFile = filenames[i];
                CopyIfDoesntExist(currentFile, Path.Combine(mscPath, managedFolder));
            }

            var cSharpAssembly = LoadAssembly(AssemblyFullPath);
            var mscLoader = LoadAssembly("MSCLoader.dll");
            var coreLibrary =
                cSharpAssembly.MainModule.AssemblyResolver.Resolve(
                    (AssemblyNameReference)cSharpAssembly.MainModule.TypeSystem.Corlib);

            mLoadedAssemblies.Add("Assembly-CSharp", cSharpAssembly);
            mLoadedAssemblies.Add("MSCPatcher", mscLoader);
            mLoadedAssemblies.Add("CoreLibrary", coreLibrary);

            //Launch the patching

            mModifications = LoadModifications(ModificationsXmlPath);

            PatchModifications();

            // We backup the original dll
            File.Move(AssemblyFullPath, String.Format("{0}.backup", AssemblyFullPath));
            Log("Creating.....Assembly-CSharp.dll.backup");

            Log("Writing the new Assembly in " + AssemblyFullPath);

            GetAssembly("Assembly-CSharp").Write(AssemblyFullPath);

            Log("Finished Writing");

            CopyCoreAssets();

            Log("Patching successfull!");
            Log("");
            MessageBox.Show("Patching successfull!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            checkPatchStatus();
        }

        //We can make this more generic, but i'm done with this
        void RemoveOldFiles() {
            Log("");
            Log("Cleaning old files!");
            Log("=================");
            //Remove old 0.1 unused files
            foreach (int i in oldfiles) {
                RemoveIfExists(Path.Combine(mscPath, managedFolder, filenames[i]));
            }
        }

        bool RemoveIfExists(string filename) {
            if (File.Exists(filename))
            {
                File.Delete(filename);
                Log(string.Format("Removing.....{0}", filename));
                return true;
            }  return false;
        }

        void CopyIfDoesntExist(string filename, string target) {
            if(!File.Exists(Path.Combine(target, filename)))
            {
                Log(string.Format("Copying new file.....{0}", filename));
                File.Copy(
                    Path.GetFullPath(Path.Combine(filename, "")),
                    Path.Combine(target, filename)
                );
            }
        }

        void CopyCoreAssets() {
            Log("Copying Core Assets.....MSCLoader_Core");
            string AssetsLoaderCore = Path.Combine("Assets", "MSCLoader_Core");
            if (!Directory.Exists(Path.Combine(modPath, AssetsLoaderCore)))
            {
                Directory.CreateDirectory(Path.Combine(modPath, AssetsLoaderCore));
            }
            RemoveIfExists(Path.Combine(modPath, AssetsLoaderCore, "core.unity3d"));
            File.Copy(
                Path.GetFullPath(Path.Combine(AssetsLoaderCore, "core.unity3d")),
                Path.Combine(modPath, AssetsLoaderCore, "core.unity3d")
            );

            Log("Copying Core Assets.....MSCLoader_Settings");
            string AssetsLoaderSettings = Path.Combine("Assets", "MSCLoader_Settings");
            if (!Directory.Exists(Path.Combine(modPath, AssetsLoaderSettings)))
            {
                Directory.CreateDirectory(Path.Combine(modPath, AssetsLoaderSettings));
            }
            RemoveIfExists(Path.Combine(modPath, AssetsLoaderSettings, "settingsui.unity3d"));
            File.Copy(
                Path.GetFullPath(Path.Combine(AssetsLoaderSettings, "settingsui.unity3d")),
                Path.Combine(modPath, AssetsLoaderSettings, "settingsui.unity3d")
            );

            Log("Copying Core Assets.....MSCLoader_Console");
            string AssetsLoaderConsole = Path.Combine("Assets", "MCSLoader_Console");
            if (!Directory.Exists(Path.Combine(modPath, AssetsLoaderConsole)))
            {
                Directory.CreateDirectory(Path.Combine(modPath, AssetsLoaderConsole));
            }
            RemoveIfExists(Path.Combine(modPath, AssetsLoaderConsole, "console.unity3d"));
            File.Copy(
                Path.GetFullPath(Path.Combine(AssetsLoaderConsole, "console.unity3d")),
                Path.Combine(modPath, AssetsLoaderConsole, "console.unity3d")
            );
            Log("Copying Core Assets Completed!");
            Log("=================");
        }

        public static AssemblyDefinition GetAssembly(string name)
        {
            if (mLoadedAssemblies.ContainsKey(name))
            {
                return mLoadedAssemblies[name];
            }

            AssemblyDefinition assembly = LoadAssembly(name);

            if (assembly != null)
            {
                mLoadedAssemblies.Add(name, assembly);
                return assembly;
            }
            return null;
        }

        public static AssemblyDefinition LoadAssembly(string path)
        {
            if (!path.EndsWith(".dll"))
                path += ".dll";

            if (File.Exists(path))
            {
                try
                {
                    var resolver = new DefaultAssemblyResolver();
                    resolver.AddSearchDirectory(Path.Combine(mscPath, @"mysummercar_Data\Managed"));
                    AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(path, new ReaderParameters { AssemblyResolver = resolver });
                    return assembly;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Couldn't load assembly " + path + Environment.NewLine + e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   // MessageBox.Show(e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Assembly " + path + " doesn't exist", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        public void PatchModifications()
        {
            foreach (XElement classNode in mModifications.Elements("Class"))
            {
                // We load the class in which the modifications will take place
                string nameTypeToPatch = classNode.Attribute("Name").Value;
                TypeDefinition typeToPatch = GetAssembly("Assembly-CSharp").MainModule.Types.FirstOrDefault(t => t.Name == nameTypeToPatch);

                if (typeToPatch == null)
                {
                    MessageBox.Show("Couldn't find type/class named" + nameTypeToPatch,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    continue;
                }

                foreach (XElement methodNode in classNode.Elements("Method"))
                {
                    string nameMethodTopatch = methodNode.Attribute("Name").Value;
                    MethodDefinition methodToPatch = typeToPatch.Methods.FirstOrDefault(m => m.Name == nameMethodTopatch);

                    if (methodToPatch == null)
                    {
                        MessageBox.Show("Couldn't find method named" + nameMethodTopatch, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    Mono.Cecil.Cil.MethodBody methodBody = methodToPatch.Body;

                    ILProcessor processor = methodBody.GetILProcessor();

                    // By default, we place the modification just before the "ret" instruction
                    // (i.e. before the last instruction)
                    int indexBegin = methodToPatch.Body.Instructions.Count - 1;

                    // But, if the user specified a location, we place the modification there
                    if (methodNode.Attribute("Location") != null)
                    {
                        indexBegin = int.Parse(methodNode.Attribute("Location").Value);
                    }

                    // If the user specified a count of instructions to delete,
                    // we delete them
                    if (methodNode.Attribute("DeleteCount") != null)
                    {
                        int countInstrToDelete = int.Parse(methodNode.Attribute("DeleteCount").Value);

                        for (int i = 0; i < countInstrToDelete; i++)
                        {
                            processor.Remove(methodToPatch.Body.Instructions.ElementAt(indexBegin));
                        }
                    }

                    Instruction locationInstr = methodToPatch.Body.Instructions.ElementAt(indexBegin);
                    Instruction prevInstr = locationInstr.Previous;

                    foreach (XElement instrNode in methodNode.Elements("Instruction"))
                    {
                        Instruction instr = Call.ParseInstruction(processor, typeToPatch, instrNode);
                        //MessageBox.Show(instr.ToString());
                        if (instr == null)
                        {
                            continue;
                        }

                        if (prevInstr == null)
                            processor.InsertBefore(locationInstr, instr);
                        else
                            processor.InsertAfter(prevInstr, instr);

                        prevInstr = instr;
                    }

                    // Optimize the method
                    methodToPatch.Body.OptimizeMacros();
                }
            }
        }

        public XElement LoadModifications(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            return XElement.Load(stream);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            if (AssemblyFullPath != null)
            {
                if (MDradio.Checked)
                {
                    ModificationsXmlPath = "MSCPatcher.Modifications_MD.xml";
                    try
                    {
                        modPath = mdPath;
                        PatchCheck();
                        if (!Directory.Exists(mdPath))
                        {
                            //if mods folder not exists, create it.
                            Directory.CreateDirectory(mdPath);
                        }                      
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message +Environment.NewLine+"Please restart patcher!","Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (GFradio.Checked)
                {
                    ModificationsXmlPath = "MSCPatcher.Modifications_GF.xml";
                    try
                    {
                        modPath = gfPath;
                        PatchCheck();
                        if (!Directory.Exists(gfPath))
                        {
                            //if mods folder not exists, create it.
                            Directory.CreateDirectory(gfPath);
                        }                     
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + Environment.NewLine + "Please restart patcher!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (ADradio.Checked)
                {
                    ModificationsXmlPath = "MSCPatcher.Modifications_AD.xml";
                    try
                    {
                        modPath = adPath;
                        PatchCheck();
                        if (!Directory.Exists(adPath))
                        {
                            //if mods folder not exists, create it.
                            Directory.CreateDirectory(adPath);
                        }                       
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + Environment.NewLine + "Please restart patcher!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            else
            {
                MessageBox.Show("Select game path first", "Error path unknown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectPathToMSC.ShowDialog() == DialogResult.OK)
            { 
                mscPath = Path.GetDirectoryName(selectPathToMSC.FileName);
                mscPathLabel.Text = mscPath;
                AssemblyFullPath = Path.Combine(mscPath, AssemblyPath);
                gfPath = Path.Combine(mscPath, @"Mods");
                GFlabel.Text = gfPath;
                if (Directory.Exists(gfPath))
                {
                    Log(string.Format("Found mods folder in: {0}", gfPath));
                    GFlabel.ForeColor = Color.Green;
                    GFradio.Checked = true;
                }
                Log(string.Format("Game folder set to: {0}", mscPath));
                File.WriteAllText("MSCFolder.txt", mscPath);
                Log(string.Format("Game folder is saved as: {0}{1}", mscPath,Environment.NewLine));
                checkPatchStatus();
            }

        }
        void checkPatchStatus()
        {
            mscloaderUpdate = false;
            isgameUpdated = false;
            oldPatchFound = false;
            oldFilesFound = false;
            button1.Enabled = false;
            button3.Enabled = false;
            bool newpatchfound = false;
            bool oldpatchfound = false;
            TypeDefinition typeToPatch = AssemblyDefinition.ReadAssembly(AssemblyFullPath).MainModule.Types.FirstOrDefault(t => t.Name == "PlayMakerArrayListProxy");
            MethodDefinition methodToPatch = typeToPatch.Methods.FirstOrDefault(m => m.Name == "Awake");
            Mono.Cecil.Cil.MethodBody methodBody = methodToPatch.Body;
            // MessageBox.Show(methodBody.CodeSize.ToString()); //debug

            // TODO: store these code length magic numbers in a config file
            
            // patch not found
            if (methodBody.CodeSize == 24) 
                newpatchfound = false;
            // patch found
            else if (methodBody.CodeSize == 29) 
                newpatchfound = true;

            if (!newpatchfound)
            {
                TypeDefinition typeToPatch2 = AssemblyDefinition.ReadAssembly(AssemblyFullPath).MainModule.Types.FirstOrDefault(t => t.Name == "StartGame");
                MethodDefinition methodToPatch2 = typeToPatch2.Methods.FirstOrDefault(m => m.Name == ".ctor");
                Mono.Cecil.Cil.MethodBody methodBody2 = methodToPatch2.Body;

                // not patched
                if (methodBody2.CodeSize == 18)
                    oldpatchfound = false;
                // 0.1 patch found
                else if (methodBody2.CodeSize == 23)
                    oldpatchfound = true;
            }
            if (!newpatchfound && !oldpatchfound)
            {
                if (File.Exists(Path.Combine(mscPath, managedFolder, "Assembly-CSharp.original.dll")))
                {
                    statusLabelText.Text = "Not patched, but MSCLoader 0.1 found (probably there was game update)";
                    Log(statusLabelText.Text);
                    button1.Text = "Install MSCLoader";
                    button1.Enabled = true;
                    oldFilesFound = true;
                }
                else if (File.Exists(Path.Combine(mscPath, managedFolder, "MSCLoader.dll")) 
                      && File.Exists(Path.Combine(mscPath, managedFolder, "Assembly-CSharp.dll.backup")))
                {
                    statusLabelText.Text = "Not patched, but MSCLoader found (probably there was game update)";
                    Log(statusLabelText.Text);
                    button1.Text = "Install MSCLoader";
                    button1.Enabled = true;
                    isgameUpdated = true;
                }
                else
                {
                    statusLabelText.Text = "Not installed";
                    Log("Patch not found, ready to install patch");
                    button1.Text = "Install MSCLoader";
                    button1.Enabled = true;
                }
                statusLabelText.ForeColor = Color.Red;

            }
            else if (newpatchfound)
            {
                if (MD5HashFile(Path.Combine(mscPath, managedFolder, "MSCLoader.dll"))
                 == MD5HashFile(Path.GetFullPath(Path.Combine("MSCLoader.dll", ""))))
                {
                    statusLabelText.Text = "Installed, MSCLoader.dll is up to date.";
                    Log("Newest patch found, no need to patch again");
                    button1.Enabled = false;
                    button3.Enabled = true;
                    statusLabelText.ForeColor = Color.Green;
                    mscloaderUpdate = false;
                }
                else
                {
                    statusLabelText.Text = "Installed, but MSCLoader.dll mismatch, Update?";
                    Log("Newest patch found, but MSCLoader.dll version mismatch, update MSCLoader?");
                    button1.Enabled = true;
                    button1.Text = "Update MSCLoader";
                    button3.Enabled = true;
                    statusLabelText.ForeColor = Color.Blue;
                    mscloaderUpdate = true;
                }
            }
            else if (oldpatchfound)
            {
                statusLabelText.Text = "0.1 patch found, upgrade available";
                Log("Old patch found, ready to upgrade");
                button1.Text = "Upgrade MSCLoader";
                button1.Enabled = true;
                statusLabelText.ForeColor = Color.Orange;
                oldPatchFound = true;
            }
        }
        public string MD5HashFile(string fn)
        {
            byte[] hash = MD5.Create().ComputeHash(File.ReadAllBytes(fn));
            return BitConverter.ToString(hash).Replace("-", "");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                        "Do you want to remove MSCLoader from game?",
                        "Remove?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                button3.Enabled = false;
                Log("");
                Log("Removing MSCLoader from game");
                Log("=================");
                if (File.Exists(string.Format("{0}.backup", AssemblyFullPath)))
                {
                    RemoveIfExists(AssemblyFullPath);
                    File.Move(string.Format("{0}.backup", AssemblyFullPath), AssemblyFullPath);

                    foreach (int i in removefiles) {
                        string currentFile = filenames[i];
                        RemoveIfExists(Path.Combine(mscPath, managedFolder, currentFile));
                    }

                    Log("=================");
                    Log("MSCLoader removed successfully!");
                    Log("");
                    MessageBox.Show("MSCLoader removed successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Log("Error! Backup file not found");
                    MessageBox.Show(string.Format("Backup file not found in:{1}{0}{1}Can't continue{1}{1}Please check integrity files in steam, to recover original file.", String.Format("{0}.backup", AssemblyFullPath), Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                checkPatchStatus();
            }
        }
        void LaunchMSCsruSteam()
        {
            try
            {
                Log("");
                Log("Starting game on steam");
                ProcessStartInfo mscSteamUrl = new ProcessStartInfo("steam://rungameid/516750");
                Process.Start(mscSteamUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to run MSC, is steam installed correctly?{1}{1}Error details:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void QuestionUsersDiscipline() {
            DialogResult response = MessageBox.Show(
                    "Did you read the instructions? (Readme.txt)", 
                    "Question for you!",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
            );
            if (response == DialogResult.Yes)
            {
                MessageBox.Show(string.Format(
                    "Why are you lying?{0}Or maybe you can't read?{0}If you could read, you would know what you did wrong.", Environment.NewLine),
                    "You are a liar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            else
            {
                MessageBox.Show(string.Format(
                    "Yes I see.{0}Go back to readme and you will know what you did wrong.", Environment.NewLine),
                    "RTFM!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            Environment.Exit(0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LaunchMSCsruSteam();
        }
    }
}
