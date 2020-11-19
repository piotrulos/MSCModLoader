using System;
using System.IO;
using System.Windows.Forms;

namespace MSCPatcher
{
    class Log
    {
        public static TextBox logBox;

        public static void Write(string log, bool before = false, bool separator = false)
        {
            if (before && separator)
                logBox.AppendText(string.Format("{1}{0}{1}================={1}", log, Environment.NewLine));
            else if (before)
                logBox.AppendText(string.Format("{1}{0}{1}", log, Environment.NewLine));
            else if (separator)
                logBox.AppendText(string.Format("{0}{1}================={1}", log, Environment.NewLine));
            else
                logBox.AppendText(string.Format("{0}{1}", log, Environment.NewLine));

            File.WriteAllText(Path.Combine("Log.txt", ""), logBox.Text);
        }
    }
}
