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
            Log($"MSCLoader Installer Log {DateTime.Now:u}");
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

        public static void MissingFilesError()
        {
            MessageBox.Show($"Couldn't find any required core files.{Environment.NewLine}Please unpack all files before launching this installer!", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Log($"!!! ERROR !!!", true, true);
            Log($"Core files not found, exiting.");
            Environment.Exit(0);
        }
    }
}
