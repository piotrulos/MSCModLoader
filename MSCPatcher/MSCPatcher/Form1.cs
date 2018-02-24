using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using MSCPatcher.Instructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        public static Form1 form1;
        public static string mscPath = "(unknown)";
        private string AssemblyPath = @"mysummercar_Data\Managed\Assembly-CSharp.dll";
        private string AssemblyFullPath = null;
        private string ModificationsXmlPath = "MSCPatcher.Modifications_MD.xml";

        private string mdPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");

        private string adPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"..\LocalLow\Amistech\My Summer Car\Mods"));

        private string gfPath = "?";

        private string modPath = "";

        private static Dictionary<string, AssemblyDefinition> mLoadedAssemblies = new Dictionary<string, AssemblyDefinition>();
        private bool is64bin = false;
        private bool isgameUpdated = false; //game updated, remove backup and patch new Assembly-CSharp.dll
        private bool oldPatchFound = false; //0.1 patch found, recover and patch Assembly-CSharp.original.dll and cleanup unused files
        private bool oldFilesFound = false; //0.1 files found, but no patch, cleanup files and patch new Assembly-CSharp.dll
        private bool mscloaderUpdate = false; //new MSCLoader.dll found, but no new patch needed.
        FileVersionInfo mscLoaderVersion;
        private XElement mModifications;

        public Form1()
        {
            form1 = this;
            InitializeComponent();
            Log.logBox = logBox;
            if (File.Exists("Assembly-CSharp.dll") || File.Exists("UnityEngine.dll")) //check if idiot unpacked this to managed folder.
            {
                if (MessageBox.Show("Did you read the instructions? (Readme.txt)", "Question for you!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MessageBox.Show(string.Format("Why are you lying?{0}Or maybe you can't read?{0}If you could read, you would know what you did wrong.", Environment.NewLine), "You are a liar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(string.Format("Yes I see.{0}Go back to readme and you will know what you did wrong.", Environment.NewLine), "You are a liar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                Environment.Exit(0);
            }
            if (File.Exists("MSCFolder.txt"))
            {
                mscPath = File.ReadAllText("MSCFolder.txt");
                Log.Write(string.Format("Loaded saved MSC Folder: {0}", mscPath));
            }
            mscPathLabel.Text = mscPath;

            MDlabel.Text = mdPath;
            if (Directory.Exists(mdPath))
            {
                Log.Write(string.Format("Found mods folder in: {0}", mdPath));
                MDlabel.ForeColor = Color.Green;
                MDradio.Checked = true;
                modPath = mdPath;
            }
            ADlabel.Text = adPath;
            if (Directory.Exists(adPath))
            {
                Log.Write(string.Format("Found mods folder in: {0}", adPath));
                ADlabel.ForeColor = Color.Green;
                ADradio.Checked = true;
                modPath = adPath;
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
                    Log.Write(string.Format("{2}MCSLoader v{0}, New version available: v{1}", currentVersion, version.Trim(), Environment.NewLine));
                    if (MessageBox.Show(string.Format("New version is available: v{0}, wanna check it out?", version.Trim()), "MCSLoader v" + currentVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start("https://github.com/piotrulos/MSCModLoader/releases");
                            Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Failed to open url!{1}{1}Error details:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    statusBarLabel.Text = string.Format("New version available: v{0}", version.Trim());
                }
                else if (i == 0)
                {
                    Log.Write(string.Format("{1}MCSLoader v{0} is up to date, no new version found.", currentVersion, Environment.NewLine));
                    statusBarLabel.Text = "MSCPatcher Ready!";
                }
            }
            catch (Exception e)
            {
                Log.Write(string.Format("Check for new version failed with error: {0}", e.Message));
                statusBarLabel.Text = "MSCPatcher Ready!";
            }
            Log.Write("MSCPatcher ready!", true, true);

            if (mscPath != "(unknown)")
            {
                mscPathLabel.Text = mscPath;
                AssemblyFullPath = Path.Combine(mscPath, AssemblyPath);
                gfPath = Path.Combine(mscPath, @"Mods");
                GFlabel.Text = gfPath;
                if (Directory.Exists(gfPath))
                {
                    Log.Write(string.Format("Found mods folder in: {0}", gfPath));
                    GFlabel.ForeColor = Color.Green;
                    GFradio.Checked = true;
                    modPath = gfPath;
                }
                Log.Write(string.Format("Game folder set to: {0}{1}", mscPath, Environment.NewLine));

                MainData.loadMainData(OutputlogLabel, resDialogLabel, resDialogCheck);

                debugStatusInfo();
                checkPatchStatus();
            }

            if(!Environment.Is64BitOperatingSystem)
            {
                status64.Text = "You are not running 64-bit Windows.";
                status64.ForeColor = Color.Red;
                status64g.Text = "64-bit patch cannot be installed.";
                status64g.ForeColor = Color.Red;
            }
            else
            {
                status64.Text = "You are running 64-bit Windows.";
                status64.ForeColor = Color.Green;
                check64Info();
            }
        }
        void check64Info()
        {
            install64.Enabled = false;
            remove64.Enabled = false;
            switch(Patch64.check64status())
            {
                case 1:
                    status64g.ForeColor = Color.Red;
                    status64g.Text = "64-bit patch is not installed";
                    install64.Enabled = true;
                    break;
                case 2:
                    status64g.ForeColor = Color.Green;
                    status64g.Text = "64-bit patch is installed!";
                    remove64.Enabled = true;
                    break;
                default:
                    status64g.ForeColor = Color.Red;
                    status64g.Text = "Unknown mysummercar.exe detected.";
                    break;
            }
        }
        void debugStatusInfo()
        {
            enDebug.Enabled = false;
            disDebug.Enabled = false;
            switch(DebugStuff.checkDebugStatus())
            {
                case 1: //32 normal
                    debugStatus.ForeColor = Color.Red;
                    debugStatus.Text = "Debugging is disabled (32-bit)";
                    debugStatus2.ForeColor = Color.Green;
                    debugStatus2.Text = "You can enable debugging!";
                    enDebug.Enabled = true;
                    is64bin = false;
                    break;
                case 2: //32 debug
                    debugStatus.ForeColor = Color.Green;
                    debugStatus.Text = "Debugging is enabled (32-bit)";
                    debugStatus2.ForeColor = Color.Green;
                    debugStatus2.Text = "You can debug your mods!";
                    disDebug.Enabled = true;
                    is64bin = false;
                    break;
                case 3: //64 normal
                    debugStatus.ForeColor = Color.Red;
                    debugStatus.Text = "Debugging is disabled (64-bit)";
                    debugStatus2.ForeColor = Color.Green;
                    debugStatus2.Text = "You can enable debugging!";
                    enDebug.Enabled = true;
                    is64bin = true;
                    break;
                case 4: //64 debug
                    debugStatus.ForeColor = Color.Green;
                    debugStatus.Text = "Debugging is enabled (64-bit)";
                    debugStatus2.ForeColor = Color.Green;
                    debugStatus2.Text = "You can debug your mods!";
                    disDebug.Enabled = true;
                    is64bin = true;
                    break;
                default: //unknown mono detected (updated unity?)
                    debugStatus.ForeColor = Color.Red;
                    debugStatus.Text = "Unknown files detected!";
                    debugStatus2.ForeColor = Color.Red;
                    debugStatus2.Text = "Cannot enable debugging!";
                    break;
            }
        }
        public void PatchCheck()
        {
            if (oldFilesFound)
            {
                Log.Write("Cleaning old files!", true, true);
                //Remove old 0.1 unused files and patch game
                Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.original.dll"));
                Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Mono.Cecil.dll"));
                Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Mono.Cecil.Rocks.dll"));
                Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll"));
                Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCPatcher.exe"));
                Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\System.Xml.dll"));

                PatchThis();
            }
            else if (isgameUpdated)
            {
                //Remove old backup and patch new game file.
                Log.Write("Removing old backup!", true, true);
                Patcher.DeleteIfExists(String.Format("{0}.backup", AssemblyFullPath));

                PatchThis();
            }
            else if (oldPatchFound)
            {
                if (File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.original.dll")))
                {
                    if (File.Exists(AssemblyFullPath))
                    {
                        Log.Write("Recovering backup file!", true, true);

                        Patcher.DeleteIfExists(AssemblyFullPath);

                        File.Move(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.original.dll"), AssemblyFullPath);
                        Log.Write("Recovering.....Assembly-CSharp.original.dll");
                    }
                    else
                    {
                        Log.Write("Recovering backup file!", true, true);

                        File.Move(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.original.dll"), AssemblyFullPath);
                        Log.Write("Recovering.....Assembly-CSharp.original.dll");
                    }
                    //Removing old files
                    Log.Write("Cleaning old files!", true, true);
                    Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Mono.Cecil.dll"));
                    Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Mono.Cecil.Rocks.dll"));
                    Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll"));
                    Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCPatcher.exe"));
                    Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\System.Xml.dll"));

                    PatchThis();
                }
                else
                {
                    MessageBox.Show(string.Format("0.1 backup file not found in:{1}{0}{1}Can't continue with upgrade{1}{1}Please check integrity files in steam, to recover original file.", Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.original.dll"), Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusBarLabel.Text = "Error!";
                }
            }
            else if (mscloaderUpdate)
            {
                Log.Write("MSCLoader.dll update!", true, true);

                Patcher.CopyReferences(mscPath);

                Patcher.CopyCoreAssets(modPath);

                Log.Write("MSCLoader.dll update successful!");
                Log.Write("");
                MessageBox.Show("Update successfull!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusBarLabel.Text = "Update successfull!";
                checkPatchStatus();
            }
            else
            {
                PatchThis();
            }

        }
        public void PatchThis()
        {
            Log.Write("Start patching files!", true, true);

            Patcher.CopyReferences(mscPath);

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
            Log.Write("Creating.....Assembly-CSharp.dll.backup");

            Log.Write(string.Format("Patching.....{0}", Path.GetFileName(AssemblyFullPath)));

            GetAssembly("Assembly-CSharp").Write(AssemblyFullPath);

            Log.Write("Finished Patching");

            Patcher.CopyCoreAssets(modPath);

            Log.Write("Patching successfull!");
            Log.Write("");
            MessageBox.Show("Patching successfull!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            statusBarLabel.Text = "Patching successfull!";
            checkPatchStatus();
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
                    MessageBox.Show(string.Format("Couldn't load assembly {0}{1}{2}", path, Environment.NewLine, e.Message), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    form1.statusBarLabel.Text = string.Format("Couldn't load assembly {0}", Path.GetFileName(path));
                }
            }
            else
            {
                MessageBox.Show(string.Format("Assembly {0} doesn't exist", path), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                form1.statusBarLabel.Text = string.Format("Assembly {0} doesn't exist", Path.GetFileName(path));
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
                    MessageBox.Show(string.Format("Couldn't find type/class named {0}", nameTypeToPatch), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                foreach (XElement methodNode in classNode.Elements("Method"))
                {
                    string nameMethodTopatch = methodNode.Attribute("Name").Value;
                    MethodDefinition methodToPatch = typeToPatch.Methods.FirstOrDefault(m => m.Name == nameMethodTopatch);

                    if (methodToPatch == null)
                    {
                        MessageBox.Show(string.Format("Couldn't find method named {0}", nameMethodTopatch), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Error:{1}{0}{1}{1}Please restart patcher!", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        MessageBox.Show(string.Format("Error:{1}{0}{1}{1}Please restart patcher!", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        MessageBox.Show(string.Format("Error:{1}{0}{1}{1}Please restart patcher!", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                //Apply changes to mainData 
                MainData.ApplyChanges(enOutputlog.Checked, resDialogCheck.Checked, false);

                //check if everything is ok
                MainData.loadMainData(OutputlogLabel, resDialogLabel, resDialogCheck);

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
                    Log.Write(string.Format("Found mods folder in: {0}", gfPath));
                    GFlabel.ForeColor = Color.Green;
                    GFradio.Checked = true;
                    modPath = gfPath;
                }
                Log.Write(string.Format("Game folder set to: {0}", mscPath));
                File.WriteAllText("MSCFolder.txt", mscPath);
                Log.Write(string.Format("Game folder is saved as: {0}{1}", mscPath, Environment.NewLine));
                MainData.loadMainData(OutputlogLabel, resDialogLabel, resDialogCheck);
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
            if (methodBody.CodeSize == 24)
            {
                //not patched
                newpatchfound = false;
            }
            else if (methodBody.CodeSize == 29)
            {
                //patch found
                newpatchfound = true;
            }
            if (!newpatchfound)
            {
                TypeDefinition typeToPatch2 = AssemblyDefinition.ReadAssembly(AssemblyFullPath).MainModule.Types.FirstOrDefault(t => t.Name == "StartGame");
                MethodDefinition methodToPatch2 = typeToPatch2.Methods.FirstOrDefault(m => m.Name == ".ctor");
                Mono.Cecil.Cil.MethodBody methodBody2 = methodToPatch2.Body;
                if (methodBody2.CodeSize == 18)
                {
                    //not patched
                    oldpatchfound = false;
                }
                else if (methodBody2.CodeSize == 23)
                {
                    //0.1 patch found
                    oldpatchfound = true;
                }
            }
            if (!newpatchfound && !oldpatchfound)
            {
                if (File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.original.dll")))
                {
                    statusLabelText.Text = "Not patched, but MSCLoader 0.1 found (probably there was game update)";
                    Log.Write("Patch not found, but MSCLoader 0.1 files found (probably there was game update)");
                    button1.Text = "Install MSCLoader";
                    button1.Enabled = true;
                    oldFilesFound = true;
                }
                else if (File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll")) && File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.dll.backup")))
                {
                    statusLabelText.Text = "Not patched, but MSCLoader found (probably there was game update)";
                    Log.Write("Patch not found, but MSCLoader files found (looks like there was game update)");
                    button1.Text = "Install MSCLoader";
                    button1.Enabled = true;
                    isgameUpdated = true;
                }
                else
                {
                    statusLabelText.Text = "Not installed";
                    Log.Write("Patch not found, ready to install patch");
                    button1.Text = "Install MSCLoader";
                    button1.Enabled = true;
                }
                statusLabelText.ForeColor = Color.Red;

            }
            else if (newpatchfound)
            {
                if (MD5HashFile(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll")) == MD5HashFile(Path.GetFullPath(Path.Combine("MSCLoader.dll", ""))))
                {
                    statusLabelText.Text = "Installed, MSCLoader.dll is up to date.";
                    Log.Write("Newest patch found, no need to patch again");
                    button1.Enabled = false;
                    button3.Enabled = true;
                    statusLabelText.ForeColor = Color.Green;
                    mscloaderUpdate = false;
                }
                else
                {
                    statusLabelText.Text = "Installed, but MSCLoader.dll mismatch, Update?";
                    Log.Write("Newest patch found, but MSCLoader.dll version mismatch, update MSCLoader?");
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
                Log.Write("Old patch found, ready to upgrade");
                button1.Text = "Upgrade MSCLoader";
                button1.Enabled = true;
                statusLabelText.ForeColor = Color.Orange;
                oldPatchFound = true;
            }
        }
        public static string MD5HashFile(string fn)
        {
            byte[] hash = MD5.Create().ComputeHash(File.ReadAllBytes(fn));
            return BitConverter.ToString(hash).Replace("-", "");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to remove MSCLoader from game?", "Remove?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    button3.Enabled = false;
                    Log.Write("Removing MSCLoader from game", true, true);

                    if (File.Exists(String.Format("{0}.backup", AssemblyFullPath)))
                    {
                        Patcher.DeleteIfExists(AssemblyFullPath);
                        File.Move(String.Format("{0}.backup", AssemblyFullPath), AssemblyFullPath);
                        Log.Write("Recovering.....Assembly-CSharp.dll.backup");

                        Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll"));
                        Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\uAudio.dll"));
                        Patcher.DeleteIfExists(Path.Combine(mscPath, @"mysummercar_Data\Managed\System.Xml.dll"));

                        Log.Write("", false, true);
                        Log.Write("MSCLoader removed successfully!");
                        Log.Write("");
                        MessageBox.Show("MSCLoader removed successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        statusBarLabel.Text = "MSCLoader removed successfully!";
                    }
                    else
                    {
                        Log.Write("Error! Backup file not found");
                        MessageBox.Show(string.Format("Backup file not found in:{1}{0}{1}Can't continue{1}{1}Please check integrity files in steam, to recover original file.", String.Format("{0}.backup", AssemblyFullPath), Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    checkPatchStatus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusBarLabel.Text = "Error: " + ex.Message;
                }
            }
        }
        void LaunchMSCsruSteam()
        {
            try
            {
                Log.Write("Starting game on steam", true, false);
                Process.Start("steam://rungameid/516750");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to run MSC, is steam installed correctly?{1}{1}Error details:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            LaunchMSCsruSteam();
        }

        private void engineButton_Click(object sender, EventArgs e)
        {
            //Apply changes to mainData 
            MainData.ApplyChanges(enOutputlog.Checked, resDialogCheck.Checked);

            //check if everything is ok
            MainData.loadMainData(OutputlogLabel, resDialogLabel, resDialogCheck);

        }

        private void enDebug_Click(object sender, EventArgs e)
        {
            if(debugCheckbox.Checked)
            {
                DebugStuff.EnableDebugging(is64bin, modPath);
                debugStatusInfo();
            }
            else
            {
                MessageBox.Show("Please select that you understand what debugging is.", "Read info first", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void disDebug_Click(object sender, EventArgs e)
        {
            DebugStuff.DisableDebugging();
            debugStatusInfo();
        }

        private void linkDebug_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("https://github.com/piotrulos/MSCModLoader/wiki/Debugging-your-mods-in-Visual-Studio");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to open url!{1}{1}Error details:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void install64_Click(object sender, EventArgs e)
        {
            Patch64.install64();
            check64Info();
            debugStatusInfo();
        }

        private void remove64_Click(object sender, EventArgs e)
        {
            Patch64.remove64();
            check64Info();
            debugStatusInfo();
        }
    }
}