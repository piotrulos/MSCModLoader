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
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExHandler);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        static void ExHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show(string.Format("MSCPatcher initialization failed!{1}Make sure you unpacked all files from archive.{1}{1}Error Details:{1}{0}", e.GetFullMessage(), Environment.NewLine), "MSCPatcher Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Get Full Exception messages (including inner exceptions) but without stack trace.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns></returns>
        public static string GetFullMessage(this Exception ex)
        {
            return ex.InnerException == null
                 ? ex.Message
                 : ex.Message + " --> " + ex.InnerException.GetFullMessage();
        }
    }
}
