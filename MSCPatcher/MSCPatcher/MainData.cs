using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MSCPatcher
{
    public class MainData
    {
        //Data closest to needed values
        static byte[] data = { 0x41, 0x6d, 0x69, 0x73, 0x74, 0x65, 0x63, 0x68, 0x0d, 0x00, 0x00, 0x00, 0x4d, 0x79, 0x20, 0x53, 0x75, 0x6d, 0x6d, 0x65, 0x72, 0x20, 0x43, 0x61, 0x72 };
        static long offset = 0;
        static string mainDataPath = null;

        public static void loadMainData(Label outputlog, Label resDialog, CheckBox resDialogCheck)
        {
            if (Form1.mscPath != "(unknown)")
            {
                mainDataPath = Path.Combine(Form1.mscPath, @"mysummercar_Data\mainData");
                offset = FindBytes(mainDataPath, data);
                try
                {
                    using (FileStream stream = File.OpenRead(mainDataPath))
                    //using (var stream = new FileStream(mainDataPath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        //output_log.txt
                        stream.Position = offset + 115;
                        if (stream.ReadByte() == 1)
                        {
                            outputlog.ForeColor = Color.Green;
                            outputlog.Text = "Enabled";
                        }
                        else
                        {
                            outputlog.ForeColor = Color.Red;
                            outputlog.Text = "Disabled";
                        }

                        //resolution dialog
                        stream.Position = offset + 96;
                        if (stream.ReadByte() == 1)
                        {
                            resDialog.ForeColor = Color.Green;
                            resDialog.Text = "Enabled";
                            resDialogCheck.Checked = false;
                        }
                        else
                        {
                            resDialog.ForeColor = Color.Red;
                            resDialog.Text = "Disabled";
                            resDialogCheck.Checked = true;

                        }

                        stream.Close();
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(string.Format("Failed to read data from file.{1}{1}Error details:{1}{0}", e.Message, Environment.NewLine), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public static void ApplyChanges(bool output_log, bool resDialog, bool mb = true)
        {
            if(mainDataPath != null)
            {
                try
                {
                    using (var stream = new FileStream(mainDataPath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        //output_log.txt
                        stream.Position = offset + 115;

                        if (output_log)
                            stream.WriteByte(0x01);
                        else
                            stream.WriteByte(0x00);

                        //resolution dialog
                        stream.Position = offset + 96;
                        if (!resDialog)
                            stream.WriteByte(0x01);
                        else
                            stream.WriteByte(0x00);

                        stream.Close();
                    }
                    if(mb)
                        MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    //thrown when file is in use in other app (like game is running)
                    MessageBox.Show(string.Format("Failed to write data to file.{1}{1}Error details:{1}{0}", e.Message, Environment.NewLine), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private static long FindBytes(string fileName, byte[] bytes)
        {
            long i, j;
            using (FileStream fs = File.OpenRead(fileName))
            {
                for (i = 0; i < fs.Length - bytes.Length; i++)
                {
                    fs.Seek(i, SeekOrigin.Begin);
                    for (j = 0; j < bytes.Length; j++)
                        if (fs.ReadByte() != bytes[j]) break;
                    if (j == bytes.Length) break;
                }
                fs.Close();
            }
            return i;
        }

    }
}
