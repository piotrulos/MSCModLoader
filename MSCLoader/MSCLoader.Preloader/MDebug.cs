using System;
using System.Diagnostics;

namespace MSCLoader.Preloader
{
    class MDebug
    {
        private static TraceSource ts = new TraceSource("MSCLoader");
        private static TextWriterTraceListener tw = new TextWriterTraceListener("MSCLoader_Preloader.txt");

        public static void Init()
        {
            ts.Switch.Level = SourceLevels.All;       
            ts.Listeners.Add(tw);
            Log("MSCLoader Preloader Log");
            Log(DateTime.Now.ToString("u"), true);
        }

        public static void Log(string message, bool newline = false)
        {
            tw.WriteLine(message);
            if (newline) tw.WriteLine("");
            tw.Flush();
        }
    }
}
