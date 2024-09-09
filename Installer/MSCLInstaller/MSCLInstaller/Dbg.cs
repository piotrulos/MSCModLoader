using System;
using System.Diagnostics;
using System.Windows;

namespace MSCLInstaller
{
    class Dbg
    {
        private static TraceSource ts = new TraceSource("MSCLInstaller");
        private static TextWriterTraceListener tw = new TextWriterTraceListener("Log.txt");

        public static void Init()
        {
            ts.Switch.Level = SourceLevels.All;
            ts.Listeners.Add(tw);
            Log($"MSCLoader Installer Log {DateTime.Now:u}");
            Log($"Installer Version {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
        }
        public static void Log(string message, bool newline = false, bool separator = false)
        {
            if (newline) tw.WriteLine("");
            tw.WriteLine(message);
            if (separator) tw.WriteLine("=================");
            tw.Flush();
        }

        public static void MissingFilesError()
        {
            MessageBox.Show($"Couldn't find any required core files.{Environment.NewLine}Please unpack all files before launching this installer!", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Dbg.Log($"!!! CRASH !!!", true, true);
            Dbg.Log($"Core files not found, exiting.");
            Environment.Exit(0);
        }
    }
}
