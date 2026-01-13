using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MSCLInstaller
{
    class Dbg
    {
        private static TraceSource ts = new TraceSource("MSCLInstaller");
        private static TextWriterTraceListener tw;

        public static void Init()
        {
            Stream logFile = File.Create("Log.txt");
            tw = new TextWriterTraceListener(logFile);
            ts.Switch.Level = SourceLevels.All;
            ts.Listeners.Add(tw);
            Log($"MSCLoader Installer Log {DateTime.Now}");
            Log($"Installer Version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
        }
        public static void Log(string message, bool newline = false, bool separator = false)
        {
            if (tw == null) return;
            if (newline) tw.WriteLine("");
            tw.WriteLine(message);
            if (separator) tw.WriteLine("=================");
            tw.Flush();
        }

        public static void MissingFileError(string missingFileName)
        {
            string message =
                $"A required core file could not be found:{Environment.NewLine}" +
                $"{missingFileName}{Environment.NewLine}{Environment.NewLine}" +
                "Please ensure all files are fully extracted before launching the installer.";

            MessageBox.Show(
                message,
                "Fatal Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Log($"FATAL: Missing core file '{missingFileName}'. Shutting down application.");
            Application.Current.Shutdown(-1); // Do not use Environment.Exit unless a hard, immediate termination is required.
        }

    }
}
