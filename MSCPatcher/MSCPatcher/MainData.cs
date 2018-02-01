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

        public static void loadMainData(Label outputlog)
        {
            if (Form1.mscPath != "(unknown)")
            {
                mainDataPath = Path.Combine(Form1.mscPath, @"mysummercar_Data\mainData");
                offset = FindBytes(mainDataPath, data);
                using (var stream = new FileStream(mainDataPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    //output_log.txt
                    stream.Position = offset + 115;
                    if(stream.ReadByte() == 1)
                    {
                        outputlog.ForeColor = Color.Green;
                        outputlog.Text = "Enabled";
                    }
                    else
                    {
                        outputlog.ForeColor = Color.Red;
                        outputlog.Text = "Disabled";
                    }
                    // stream.WriteByte(0x04);
                    stream.Close();
                }
            }
        }
        public static void ApplyChanges(bool output_log)
        {
            if(mainDataPath != null)
            {
                using (var stream = new FileStream(mainDataPath, FileMode.Open, FileAccess.ReadWrite))
                {
                    //output_log.txt
                    stream.Position = offset + 115;

                    if (output_log)
                        stream.WriteByte(0x01);
                    else
                        stream.WriteByte(0x00);

                    stream.Close();
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
