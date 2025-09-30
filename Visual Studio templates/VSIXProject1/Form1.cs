using System;
using System.IO;
using System.Windows.Forms;

namespace VSIXProject1
{
    public partial class Form1 : Form
    {
        public static string managedPath = "";
        public static string modName = "";
        public static string modAuthor = "";
        public static string modVersion = "";

        public static string assPM = "false";
        public static string assCS = "false";
        public static string asscInput = "false";
        public static string assUI = "false";
        public static string assHarmony = "false";
        public static string assCSf = "false";

        public static string setOnMenuLoad = "false";
        public static string setOnNewGame = "false";
        public static string setPreLoad = "false";
        public static string setOnLoad = "false";
        public static string setPostLoad = "false";
        public static string setOnSave = "false";
        public static string setOnGUI = "false";
        public static string setUpdate = "false";
        public static string setFixedUpdate = "false";

        public static string modsPath = "NONE";
        public static string advMiniDlls = "false";

        private string[] saveData = null;

        public Form1()
        {
            InitializeComponent();
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MSCLoader_template.txt")))
            {
                saveData = File.ReadAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MSCLoader_template.txt"));
                if (Directory.Exists(saveData[0]))
                {
                    managedPathBox.Text = saveData[0];
                }
                if (saveData.Length > 1)
                    authorNameBox.Text = saveData[1];
            }
            comboBox1.SelectedIndex = 0;
        }

        private void browseManaged_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                managedPathBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("https://github.com/piotrulos/MSCModLoader/wiki/How-to-use-templates");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel2.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("https://github.com/piotrulos/MSCModLoader/wiki");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel3.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("https://github.com/piotrulos/MSCModLoader/wiki/Order-of-execution-in-mod-class");
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            managedPath = managedPathBox.Text;
            modName = modNameBox.Text;
            modAuthor = authorNameBox.Text;
            modVersion = versionBox.Text;

            if (addPlaymakerDll.Checked) assPM = "true";
            if (addAssCSDll.Checked) assCS = "true";
            if (addcInputDll.Checked) asscInput = "true";
            if (addUIDll.Checked) assUI = "true";
            if (addHarmonyDll.Checked) assHarmony = "true";
            if (addAssCSfDll.Checked) assCSf = "true";

            if (setupOnMenuLoad.Checked) setOnMenuLoad = "true";
            if (setupOnNewGame.Checked) setOnNewGame = "true";
            if (setupOnPreLoad.Checked) setPreLoad = "true";
            if (setupOnLoad.Checked) setOnLoad = "true";
            if (setupPostLoad.Checked) setPostLoad = "true";
            if (setupOnSave.Checked) setOnSave = "true";
            if (setupOnGUI.Checked) setOnGUI = "true";
            if (setupUpdate.Checked) setUpdate = "true";
            if (setupFixedUpdate.Checked) setFixedUpdate = "true";
            if (advMiniDll.Checked) advMiniDlls = "true";

            if (string.IsNullOrEmpty(managedPath))
            {
                MessageBox.Show("Please select Managed path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string dsini = Path.GetFullPath(Path.Combine(managedPath, "..", "..", "doorstop_config.ini"));
            if (File.Exists(dsini))
            {
                foreach (string l in File.ReadAllLines(dsini))
                {
                    if (l.StartsWith("mods"))
                    {
                        if (l.Contains("GF"))
                        {
                            modsPath = Path.GetFullPath(Path.Combine(managedPath, "..", "..", "Mods"));
                        }
                        if (l.Contains("MD"))
                        {
                            modsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySummerCar", "Mods"));
                        }
                        if (l.Contains("AD"))
                        {
                            modsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow", "Amistech", "My Summer Car", "Mods"));
                        }
                        break;
                    }
                }

            }
            saveData = new string[] { managedPath, modAuthor };
            File.WriteAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MSCLoader_template.txt"), saveData);
            Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //Prevent resizing
            MaximumSize = Size;
            MinimumSize = Size;
        }
    }
}
