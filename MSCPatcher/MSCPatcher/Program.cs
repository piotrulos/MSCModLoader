using System;
using System.Windows.Forms;

namespace MSCPatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception e)
            {
                //Show error instead of crash if Mono.cecil dlls are missing!
                MessageBox.Show(string.Format("MSCPatcher initialization failed!{1}Make sure you unpacked all files from archive.{1}{1}Error Details:{1}{0}", e.InnerException.Message, Environment.NewLine), "MSCPatcher Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
