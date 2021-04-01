using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace MSCPatcher
{
    public partial class Form1 : Form
    {
        public static Form1 form1;
        public static string mscPath = "(unknown)";

        private string AssemblyPath = @"mysummercar_Data\Managed\Assembly-CSharp.dll";
        private string AssemblyFullPath = null;
        private string InitMethod = "Init_MD";
        private string mdPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MySummerCar\Mods");
        private string adPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"..\LocalLow\Amistech\My Summer Car\Mods"));
        private string gfPath = "?";
        private string modPath = "";

        private bool is64bin = false;
        private bool newPatchFound = false; //nonproxy patch found, recover and upgrade
        private bool isgameUpdated = false; //game updated, remove backup and patch new 
        private bool oldPatchFound = false; //0.1 patch found, recover and patch Assembly-CSharp.original.dll and cleanup unused files
        private bool oldFilesFound = false; //0.1 files found, but no patch, cleanup files and patch new Assembly-CSharp.dll
        private bool mscloaderUpdate = false; //new MSCLoader.dll found, but no new patch needed.
        FileVersionInfo mscLoaderVersion;

        public Form1()
        {
            form1 = this;
            InitializeComponent();
            Log.logBox = logBox;
            if (File.Exists("Assembly-CSharp.dll") || File.Exists("UnityEngine.dll") || File.Exists("mysummercar.exe") || File.Exists("mainData") || File.Exists("mono.dll") || File.Exists("CSteamworks.dll") || Directory.Exists("References")) //check if idiot unpacked this to game folder.
            {
                if (MessageBox.Show("Did you read the instructions? (Readme.txt)", "Question for you!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MessageBox.Show(string.Format("Why are you lying?{0}Or maybe you can't read?{0}If you could read, you would know where not to unpack files.", Environment.NewLine), "You are a liar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(string.Format("Yes I see.{0}Go back to readme and you will know you would know where not to unpack files.", Environment.NewLine), "Read manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                Environment.Exit(0);
            }
            if (!File.Exists("MSCLoader.dll"))
            {
                MessageBox.Show(string.Format("Don't run this file from zip folder. Unpack all files and start patcher again.", Environment.NewLine), "Read the freaking instructions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            if (File.Exists("MSCFolder.txt"))
            {
                mscPath = File.ReadAllText("MSCFolder.txt");
                if (!Directory.Exists(mscPath))
                {
                    Log.Write(string.Format("Saved MSC Folder, doesn't exists: {0}", mscPath));
                    mscPath = "(unknown)";
                    try
                    {
                        File.Delete("MSCFolder.txt");
                    }
                    catch (Exception e)
                    {
                        Log.Write("Error", true, true);
                        Log.Write(e.Message);
                        Log.Write(e.ToString());
                    }
                }
                else
                {
                    Log.Write(string.Format("Loaded saved MSC Folder: {0}", mscPath));
                }
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
            Log.Write(string.Format("Current folder: {0}", Path.GetFullPath(".")));

            try
            {
                mscLoaderVersion = FileVersionInfo.GetVersionInfo("MSCLoader.dll");
                string currentVersion;
                if (mscLoaderVersion.FileBuildPart != 0)
                    currentVersion = string.Format("{0}.{1}.{2}", mscLoaderVersion.FileMajorPart, mscLoaderVersion.FileMinorPart, mscLoaderVersion.FileBuildPart);
                else
                    currentVersion = string.Format("{0}.{1}", mscLoaderVersion.FileMajorPart, mscLoaderVersion.FileMinorPart);
 
                string version;
                string res;
                using (WebClient client = new WebClient())
                {
                    client.QueryString.Add("core", "stable");
                    res = client.DownloadString("http://my-summer-car.ml/ver.php");
                }
                string[] result = res.Split('|');
                if (result[0] == "error")
                {
                    switch (result[1])
                    {
                        case "0":
                            throw new Exception("Unknown branch");
                        case "1":
                            throw new Exception("Database connection error");
                        default:
                            throw new Exception("Unknown error");
                    }
                }
                else if (result[0] == "ok")
                {
                    if (result[1].Trim().Length > 8)
                        throw new Exception("Parse Error, please report that problem!");
                    version = result[1].Trim();
                }
                else
                {
                    throw new Exception("Unknown server response.");
                }
                int i = currentVersion.CompareTo(version.Trim());
                if (i != 0)
                {
                    Log.Write(string.Format("{2}MCSLoader v{0}, New version available: v{1}", currentVersion, version.Trim(), Environment.NewLine));
                    if (MessageBox.Show(string.Format("New version is available: v{0}, wanna check it out?", version.Trim()), "MCSLoader v" + currentVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        try
                        {
                           // if (File.Exists("MSCLoader_Launcher.exe"))
                             //   Process.Start("MSCLoader_Launcher.exe");
                           // else
                            Process.Start("https://www.nexusmods.com/mysummercar/mods/147");
                            Environment.Exit(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Failed to open update info!{1}{1}Error details:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log.Write("Error", true, true);
                            Log.Write(ex.Message);
                            Log.Write(ex.ToString());
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
                Log.Write("Error", true, true);
                Log.Write(string.Format("Check for new version failed with error: {0}", e.Message));
                statusBarLabel.Text = "MSCPatcher Ready!";
                Log.Write(e.ToString());
            }

            if (!Directory.Exists("Debug"))
            {
                groupBox5.Visible = false;
                basicDebugInfo.Visible = true;
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

                MainData.LoadMainData(OutputlogLabel, resDialogLabel, resDialogCheck);

                DebugStatusInfo();
                CheckPatchStatus();
            }
        }

        void DebugStatusInfo()
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
                    is64bin = true;
                    break;
            }
        }
        public void PatchStarter()
        {
            if(modPath == Path.GetFullPath("."))
            {
                throw new Exception("Modloader Files unpacked into mod folder. Cannot install.");
            }
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

                StartPatching();
            }
            else if (newPatchFound)
            {
                if (File.Exists(string.Format("{0}.backup", AssemblyFullPath)))
                {
                    Patcher.DeleteIfExists(AssemblyFullPath);
                    File.Move(string.Format("{0}.backup", AssemblyFullPath), AssemblyFullPath);
                    Log.Write("Recovering.....Assembly-CSharp.dll.backup");
                }
                else
                {
                    Log.Write("Error! Backup file not found");
                    MessageBox.Show(string.Format("Backup file not found in:{1}{0}{1}Can't continue{1}{1}Please check integrity files in steam, to recover original file.", String.Format("{0}.backup", AssemblyFullPath), Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                StartPatching();

            }
            else if (isgameUpdated)
            {
                //Remove old backup and patch new game file.
                Log.Write("Removing old backup!", true, true);
                Patcher.DeleteIfExists(string.Format("{0}.backup", AssemblyFullPath));

                StartPatching();
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

                    StartPatching();
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

                Patcher.ProcessReferences(mscPath, false);
                Patcher.CopyCoreAssets(modPath);
                Patcher.DeleteIfExists(Path.Combine(mscPath, @"doorstop_config.ini"));
                Log.Write("Generating config file.....doorstop_config.ini");
                using (TextWriter tw = File.CreateText(Path.Combine(mscPath, @"doorstop_config.ini")))
                {
                    tw.WriteLine(@"[UnityDoorstop]");
                    tw.WriteLine(@"enabled=true");
                    tw.WriteLine(@"targetAssembly=mysummercar_Data\Managed\MSCLoader.dll");
                    tw.WriteLine(@"redirectOutputLog=true");
                    tw.WriteLine(@"ignoreDisableSwitch=true");
                    tw.WriteLine(@"[MSCLoader]");

                    switch (InitMethod)
                    {
                        case "Init_MD":
                            tw.WriteLine(@"mods=MD");
                            break;
                        case "Init_GF":
                            tw.WriteLine(@"mods=GF");
                            break;
                        case "Init_AD":
                            tw.WriteLine(@"mods=AD");
                            break;
                        default:
                            tw.WriteLine(@"mods=GF");
                            break;
                    }
                    tw.WriteLine(@"skipIntro=false");
                    tw.Flush();
                }
                Log.Write("MSCLoader update successful!");
                Log.Write("");
                MessageBox.Show("Update successfull!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusBarLabel.Text = "Update successfull!";
                CheckPatchStatus();
            }
            else
            {
                StartPatching();
            }

        }
        public void StartPatching()
        {
            Log.Write("Start installing MSCLoader!", true, true);

            try
            {
                Patcher.ProcessReferences(mscPath, false);
                if (is64bin)
                {
                    Patcher.DeleteIfExists(Path.Combine(mscPath, @"winhttp.dll"));
                    if (File.Exists(Path.GetFullPath(Path.Combine("w64.dll", ""))))
                    {
                        File.Copy(Path.GetFullPath(Path.Combine("w64.dll", "")), Path.Combine(mscPath, @"winhttp.dll"));
                        Log.Write("Copying new file.....winhttp.dll");
                    }
                    else
                    {
                        throw new FileNotFoundException("File \"w64.dll\" not found, please redownload modlaoder and/or unpack all files", "w64.dll");
                    }
                }
                else
                {
                    Patcher.DeleteIfExists(Path.Combine(mscPath, @"winhttp.dll"));
                    if (File.Exists(Path.GetFullPath(Path.Combine("w32.dll", ""))))
                    {
                        File.Copy(Path.GetFullPath(Path.Combine("w32.dll", "")), Path.Combine(mscPath, @"winhttp.dll"));
                        Log.Write("Copying new file.....winhttp.dll");
                    }
                    else
                    {
                        throw new FileNotFoundException("File \"w32.dll\" not found, please redownload modlaoder and/or unpack all files", "w32.dll");
                    }
                }
                Patcher.CopyCoreAssets(modPath);               
                Log.Write("Creating Config file!", true, true);
                Patcher.DeleteIfExists(Path.Combine(mscPath, @"doorstop_config.ini"));
                Log.Write("Generating config file.....doorstop_config.ini");
                using (TextWriter tw = File.CreateText(Path.Combine(mscPath, @"doorstop_config.ini")))
                {
                    tw.WriteLine(@"[UnityDoorstop]");
                    tw.WriteLine(@"enabled=true");
                    tw.WriteLine(@"targetAssembly=mysummercar_Data\Managed\MSCLoader.dll");
                    tw.WriteLine(@"redirectOutputLog=true");
                    tw.WriteLine(@"ignoreDisableSwitch=true");
                    tw.WriteLine(@"[MSCLoader]");
                    switch (InitMethod)
                    {
                        case "Init_MD":
                            tw.WriteLine(@"mods=MD");
                            break;
                        case "Init_GF":
                            tw.WriteLine(@"mods=GF");
                            break;
                        case "Init_AD":
                            tw.WriteLine(@"mods=AD");
                            break;
                        default:
                            tw.WriteLine(@"mods=GF");
                            break;
                    }
                    tw.WriteLine(@"skipIntro=false");
                    tw.Flush();
                }

                Log.Write("Install successfull!");
                Log.Write("");
                MessageBox.Show("Install successfull!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusBarLabel.Text = "Install successfull!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error while installing: {0}",ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Write("Error", true, true);
                Log.Write(ex.Message);
                Log.Write(ex.ToString());
            }
            CheckPatchStatus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            if (AssemblyFullPath != null)
            {
                if (MDradio.Checked)
                {
                    InitMethod = "Init_MD";
                    bool isCloudIgnore = true;
                    if (Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).ToString() == "OneDrive")
                        if (MessageBox.Show($"You are about to set your mods folder to a directory that is part of OneDrive cloud.{Environment.NewLine}This is not supported due to On-Demand files that may cause issues.{Environment.NewLine}{Environment.NewLine}Do you want to continue?", "Cloud folder warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                            isCloudIgnore = false;
                    if (isCloudIgnore)
                    {
                        try
                        {
                            modPath = mdPath;
                            PatchStarter();
                            if (!Directory.Exists(mdPath))
                            {
                                //if mods folder not exists, create it.
                                Directory.CreateDirectory(mdPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(string.Format("Error:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Log.Write("Error", true, true);
                            Log.Write(ex.Message);
                            Log.Write(ex.ToString());
                        }
                    }

                }
                else if (GFradio.Checked)
                {
                    InitMethod = "Init_GF";
                    try
                    {
                        modPath = gfPath;
                        PatchStarter();
                        if (!Directory.Exists(gfPath))
                        {
                            //if mods folder not exists, create it.
                            Directory.CreateDirectory(gfPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Error:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.Write("Error", true, true);
                        Log.Write(ex.Message);
                        Log.Write(ex.ToString());
                    }
                }
                else if (ADradio.Checked)
                {
                    InitMethod = "Init_AD";
                    try
                    {
                        modPath = adPath;
                        PatchStarter();
                        if (!Directory.Exists(adPath))
                        {
                            //if mods folder not exists, create it.
                            Directory.CreateDirectory(adPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Error:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.Write("Error", true, true);
                        Log.Write(ex.Message);
                        Log.Write(ex.ToString());
                    }
                }
                //Apply changes to mainData 
                MainData.ApplyChanges(enOutputlog.Checked, resDialogCheck.Checked, false);

                //check if everything is ok
                MainData.LoadMainData(OutputlogLabel, resDialogLabel, resDialogCheck);

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
                Log.Write(string.Format("Current folder: {0}", Path.GetFullPath(".")));
                Log.Write(string.Format("Game folder set to: {0}", mscPath));
                try
                {
                    File.WriteAllText("MSCFolder.txt", mscPath);
                }
                catch
                {
                    //cannot create MSCFolder file for unknown reasons.
                }
                Log.Write(string.Format("Game folder is saved as: {0}{1}", mscPath, Environment.NewLine));
                MainData.LoadMainData(OutputlogLabel, resDialogLabel, resDialogCheck);
                CheckPatchStatus();
                DebugStatusInfo();
                //Check64Info();
            }

        }
        void CheckPatchStatus()
        {
            mscloaderUpdate = false;
            isgameUpdated = false;
            oldPatchFound = false;
            oldFilesFound = false;
            button1.Enabled = false;
            button3.Enabled = false;
            bool newpatchfound = false;
            bool oldpatchfound = false;
            bool proxypatchfound = false;
            try
            {
                bool isInjected = false;
                if (MDradio.Checked)
                    isInjected = IsPatched(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.dll"), "PlayMakerArrayListProxy", "Awake", "MSCLoader.dll", "MSCLoader.ModLoader", "Init_MD");
                else if (GFradio.Checked)
                    isInjected = IsPatched(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.dll"), "PlayMakerArrayListProxy", "Awake", "MSCLoader.dll", "MSCLoader.ModLoader", "Init_GF");
                else if (ADradio.Checked)
                    isInjected = IsPatched(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.dll"), "PlayMakerArrayListProxy", "Awake", "MSCLoader.dll", "MSCLoader.ModLoader", "Init_AD");

                if (isInjected)
                    newpatchfound = true;
                else
                    newpatchfound = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Patch checking error: {0}",ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Write("Error", true, true);
                Log.Write(ex.Message);
                Log.Write(ex.ToString());
            }
            if (!newpatchfound)
            {
                try
                {
                    bool isInjected = IsPatched(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.dll"), "StartGame", ".ctor", "MSCLoader.dll", "MSCLoader.ModLoader", "Init");
                    if (isInjected)
                        oldpatchfound = true;
                    else
                        oldpatchfound = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("Patch checking error: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Write("Error", true, true);
                    Log.Write(ex.Message);
                    Log.Write(ex.ToString());
                }
            }
            if (!newpatchfound && !oldpatchfound)
            {
                if(File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll")) && File.Exists(Path.Combine(mscPath, @"winhttp.dll")) && File.Exists(Path.Combine(mscPath, @"doorstop_config.ini")))
                {
                    proxypatchfound = true;
                }
                else if (File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.original.dll")))
                {
                    statusLabelText.Text = "Not installed, but MSCLoader 0.1 found (upgrade required)";
                    Log.Write("Proxy patch not found, but MSCLoader 0.1 files found (upgrade required)");
                    button1.Text = "Install MSCLoader";
                    button1.Enabled = true;
                    oldFilesFound = true;
                }
                else if (File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll")) && File.Exists(Path.Combine(mscPath, @"mysummercar_Data\Managed\Assembly-CSharp.dll.backup")))
                {
                    statusLabelText.Text = "Not installed, but MSCLoader files found (upgrade required)";
                    Log.Write("Proxy patch not found, but MSCLoader files found (upgrade required)");
                    button1.Text = "Install MSCLoader";
                    button1.Enabled = true;
                    isgameUpdated = true;
                }
                else
                {
                    statusLabelText.Text = "Not installed";
                    Log.Write("Proxy patch not found, ready to install MSCLoader");
                    button1.Text = "Install MSCLoader";
                    button1.Enabled = true;
                }
                statusLabelText.ForeColor = Color.Red;
            }
            else if (newpatchfound)
            {
                statusLabelText.Text = "Patch upgrade available!";
                Log.Write("Old patch found, ready to upgrade");
                button1.Text = "Upgrade MSCLoader";
                button1.Enabled = true;
                statusLabelText.ForeColor = Color.Orange;
                newPatchFound = true;
            }
            else if (oldpatchfound)
            {
                statusLabelText.Text = "0.1 patch found, upgrade available";
                Log.Write("0.1 patch found, ready to upgrade");
                button1.Text = "Upgrade MSCLoader";
                button1.Enabled = true;
                statusLabelText.ForeColor = Color.Orange;
                oldPatchFound = true;
            }
            if (proxypatchfound)
            {
                if (MD5HashFile(Path.Combine(mscPath, @"mysummercar_Data\Managed\MSCLoader.dll")) == MD5HashFile(Path.GetFullPath(Path.Combine("MSCLoader.dll", ""))))
                {
                    statusLabelText.Text = "Installed, MSCLoader.dll is up to date.";
                    Log.Write("MSCLoader is up to date.");
                    button1.Enabled = false;
                    button3.Enabled = true;
                    statusLabelText.ForeColor = Color.Green;
                    mscloaderUpdate = false;
                }
                else
                {
                    statusLabelText.Text = "Installed, but MSCLoader update available, Update?";
                    Log.Write("MSCLoader.dll version mismatch, update available.");
                    button1.Enabled = true;
                    button1.Text = "Update MSCLoader";
                    button3.Enabled = true;
                    statusLabelText.ForeColor = Color.Blue;
                    mscloaderUpdate = true;
                }
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
                    Patcher.DeleteIfExists(string.Format("{0}.backup", AssemblyFullPath));
                    Patcher.DeleteIfExists(Path.Combine(mscPath, "winhttp.dll"));
                    Patcher.DeleteIfExists(Path.Combine(mscPath, "doorstop_config.ini"));
                    Patcher.ProcessReferences(mscPath, true);
                    Log.Write("");
                    Log.Write("MSCLoader removed successfully!");
                    Log.Write("");
                    MessageBox.Show("MSCLoader removed successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    statusBarLabel.Text = "MSCLoader removed successfully!";
                    CheckPatchStatus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusBarLabel.Text = "Error: " + ex.Message;
                    Log.Write("Error", true, true);
                    Log.Write(ex.Message);
                    Log.Write(ex.ToString());
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
                Log.Write("Error", true, true);
                Log.Write(ex.Message);
                Log.Write(ex.ToString());
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
            MainData.LoadMainData(OutputlogLabel, resDialogLabel, resDialogCheck);

        }

        private void enDebug_Click(object sender, EventArgs e)
        {
            if(debugCheckbox.Checked)
            {
                DebugStuff.EnableDebugging(is64bin, modPath);
                DebugStatusInfo();
            }
            else
            {
                MessageBox.Show("Please select that you understand what debugging is.", "Read info first", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void disDebug_Click(object sender, EventArgs e)
        {
            DebugStuff.DisableDebugging();
            DebugStatusInfo();
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
                Log.Write("Error", true, true);
                Log.Write(ex.Message);
                Log.Write(ex.ToString());
            }
        }

        private void PatchThis(string mainPath, string assemblyToPatch, string assemblyType, string assemblyMethod, string loaderAssembly, string loaderType, string loaderMethod)
        {
            DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(mainPath);

            ModuleDefinition assembly = ModuleDefinition.ReadModule(mainPath + "/" + assemblyToPatch, new ReaderParameters { ReadWrite = true, AssemblyResolver = resolver });
            ModuleDefinition loader = ModuleDefinition.ReadModule(mainPath + "/" + loaderAssembly);
            MethodDefinition methodToInject = loader.GetType(loaderType).Methods.Single(x => x.Name == loaderMethod);
            MethodDefinition methodToHook = assembly.GetType(assemblyType).Methods.First(x => x.Name == assemblyMethod);

            Instruction loaderInit = Instruction.Create(OpCodes.Call, assembly.ImportReference(methodToInject));
            ILProcessor processor = methodToHook.Body.GetILProcessor();
            processor.InsertBefore(methodToHook.Body.Instructions[0], loaderInit);
            assembly.Write();
            assembly.Dispose();
            loader.Dispose();
        }
        private bool IsPatched(string assemblyToPatch, string assemblyType, string assemblyMethod, string loaderAssembly, string loaderType, string loaderMethod)
        {

            ModuleDefinition assembly = ModuleDefinition.ReadModule(assemblyToPatch);
            ModuleDefinition loader = ModuleDefinition.ReadModule(loaderAssembly);
            MethodDefinition methodToInject = loader.GetType(loaderType).Methods.Single(x => x.Name == loaderMethod);
            MethodDefinition methodToHook = assembly.GetType(assemblyType).Methods.First(x => x.Name == assemblyMethod);

            foreach (Instruction instruction in methodToHook.Body.Instructions)
            {
                if (instruction.OpCode.Equals(OpCodes.Call) && instruction.Operand.ToString().Equals($"System.Void {loaderType}::{loaderMethod}()"))
                {
                    assembly.Dispose();
                    loader.Dispose();
                    return true;
                }
            }
            assembly.Dispose();
            loader.Dispose();
            return false;
        }
    }
}