using System;
using System.IO;
using System.Windows.Forms;

namespace VSIXProject1
{
    public partial class Form1 : Form
    {
        public static string mscManagedPath = "";
        public static string mwcManagedPath = "";
        public static string managedPath = "";
        public static string game = "Game.MySummerCar";
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

        public static string mscModsPath = "NONE";
        public static string mwcModsPath = "NONE";
        public static string advMiniDlls = "false";

        public Form1()
        {
            InitializeComponent();
            mscPathBox.Text = Properties.Settings.Default.mscpath;
            mwcPathBox.Text = Properties.Settings.Default.mwcpath;
            authorNameBox.Text = Properties.Settings.Default.author;
            comboBox1.SelectedIndex = 0;
        }

        private void browseManaged_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "mysummercar.exe|mysummercar.exe";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mscPathBox.Text = Path.GetDirectoryName(Path.GetFullPath(openFileDialog1.FileName));
            }
        }
        private void browseMWC_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "mywintercar.exe|mywintercar.exe";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                mwcPathBox.Text = Path.GetDirectoryName(Path.GetFullPath(openFileDialog1.FileName));
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
            if (string.IsNullOrEmpty(mscPathBox.Text) && comboBox1.SelectedIndex == 0)
            {
                MessageBox.Show("Please select My Summer Car path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(mwcPathBox.Text) && (comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2))
            {
                MessageBox.Show("Please select My Winter Car path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            mscManagedPath = Path.GetFullPath(Path.Combine(mscPathBox.Text, "mysummercar_Data", "Managed"));
            mwcManagedPath = Path.GetFullPath(Path.Combine(mwcPathBox.Text, "mywintercar_Data", "Managed"));
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

            string dsini = Path.GetFullPath(Path.Combine(mscPathBox.Text, "doorstop_config.ini"));
            if (File.Exists(dsini))
            {
                foreach (string l in File.ReadAllLines(dsini))
                {
                    if (l.StartsWith("mods"))
                    {
                        if (l.Contains("GF"))
                        {
                            mscModsPath = Path.GetFullPath(Path.Combine(mscPathBox.Text, "Mods"));
                        }
                        if (l.Contains("MD"))
                        {
                            mscModsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MySummerCar", "Mods"));
                        }
                        if (l.Contains("AD"))
                        {
                            mscModsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow", "Amistech", "My Summer Car", "Mods"));
                        }
                        break;
                    }
                }
            }
            string dsini2 = Path.GetFullPath(Path.Combine(mwcPathBox.Text, "doorstop_config.ini"));
            if (File.Exists(dsini2))
            {
                foreach (string l in File.ReadAllLines(dsini2))
                {
                    if (l.StartsWith("mods"))
                    {
                        if (l.Contains("GF"))
                        {
                            mwcModsPath = Path.GetFullPath(Path.Combine(mwcPathBox.Text, "Mods"));
                        }
                        if (l.Contains("MD"))
                        {
                            mwcModsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Winter Car", "Mods"));
                        }
                        if (l.Contains("AD"))
                        {
                            mwcModsPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow", "Amistech", "My Winter Car", "Mods"));
                        }
                        break;
                    }
                }
            }
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    mwcModsPath = "NONE";
                    managedPath = mscManagedPath;
                    game = "Game.MySummerCar";
                    break;
                case 1:
                    mscModsPath = "NONE";
                    managedPath = mwcManagedPath;
                    game = "Game.MyWinterCar";
                    break;
                case 2:
                    managedPath = mwcManagedPath;
                    game = "Game.MySummerCar_And_MyWinterCar";
                    break;
                default:
                    mwcModsPath = "NONE";
                    managedPath = mscManagedPath;
                    game = "Game.MySummerCar";
                    break;
            }
            Properties.Settings.Default.mscpath = mscPathBox.Text;
            Properties.Settings.Default.mwcpath = mwcPathBox.Text;
            Properties.Settings.Default.author = modAuthor;
            Properties.Settings.Default.Save();
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
