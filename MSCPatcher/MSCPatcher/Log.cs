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
                logBox.AppendText($"{Environment.NewLine}{log}{Environment.NewLine}================={Environment.NewLine}");
            else if (before)
                logBox.AppendText($"{Environment.NewLine}{log}{Environment.NewLine}");
            else if (separator)
                logBox.AppendText($"{log}{Environment.NewLine}================={Environment.NewLine}");
            else
                logBox.AppendText($"{log}{Environment.NewLine}");
            try
            {
                File.WriteAllText("Log.txt", logBox.Text);
            }
            catch
            {
                //cannot create Log file for unknown reasons.
            }
        }
    }
}
