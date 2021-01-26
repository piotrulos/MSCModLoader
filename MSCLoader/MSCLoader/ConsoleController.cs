using System;
using System.Collections.Generic;
using System.Linq;

namespace MSCLoader
{
#pragma warning disable CS1591
    public delegate void CommandHandler(string[] args);

    public class ConsoleController
    {
        // Used to communicate with ConsoleView
        public delegate void LogChangedHandler(string[] log);
        public event LogChangedHandler logChanged;
        
        // Object to hold information about each command
        internal class CommandRegistration
        {
            public string command { get; private set; }
            public CommandHandler handler { get; private set; }
            public string help { get; private set; }
            public bool showInHelp { get; private set; }

            public CommandRegistration(string command, CommandHandler handler, string help, bool showInHelp)
            {
                this.command = command;
                this.handler = handler;
                this.help = help;
                this.showInHelp = showInHelp;
            }
        }

        // How many log lines should be retained?
        // Note that strings submitted to appendLogLine with embedded newlines will be counted as a single line.
#if DevMode
        const int scrollbackSize = 500;
#else
        const int scrollbackSize = 200;
#endif
        public Queue<string> scrollback;
        public List<string> commandHistory = new List<string>();
        internal Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();

        public string[] log { get; private set; } //Copy of scrollback as an array for easier use by ConsoleView

        public ConsoleController()
        {
            if (ModLoader.devMode && MSCUnloader.dm_pcon != null)
            {
                scrollback = new Queue<string>(MSCUnloader.dm_pcon);
            }
            else
            {
                scrollback = new Queue<string>(scrollbackSize);
            }
            RegisterCommand("help", HelpCommand, "This screen", "?");
            RegisterCommand("clear", ClearConsole, "Clears console screen", "cls");
        }

        public void RegisterCommand(string command, CommandHandler handler, string help, bool inHelp = true)
        {
            commands.Add(command, new CommandRegistration(command, handler, help, inHelp));
        }
        public void RegisterCommand(string command, CommandHandler handler, string help, string alias, bool inHelp = true)
        {
            CommandRegistration cmd = new CommandRegistration(command, handler, help, inHelp);
            commands.Add(command, cmd);
            commands.Add(alias, cmd);
        }
        void ClearConsole(string[] args)
        {
            scrollback.Clear();
            log = scrollback.ToArray();
            logChanged(log);
        }

        public void AppendLogLine(string line)
        {
            if (scrollback.Count >= scrollbackSize)
            {
                scrollback.Dequeue();
            }
            scrollback.Enqueue(line);

            log = scrollback.ToArray();
            logChanged?.Invoke(log);
        }

        public void RunCommandString(string commandString)
        {
            if (!string.IsNullOrEmpty(commandString))
            {
                AppendLogLine(string.Format("{1}<b><color=orange>></color></b> {0}", commandString, Environment.NewLine));

                string[] commandSplit = ParseArguments(commandString);
                string[] args = new string[0];
                if (commandSplit.Length < 1)
                {
                    AppendLogLine(string.Format("<color=red>Unable to process command:</color> <b>{0}</b>", commandString));
                    return;

                }
                else if (commandSplit.Length >= 2)
                {
                    int numArgs = commandSplit.Length - 1;
                    args = new string[numArgs];
                    Array.Copy(commandSplit, 1, args, 0, numArgs);
                }
                RunCommand(commandSplit[0].ToLower(), args);
                commandHistory.Add(commandString);
            }
        }

        void RunCommand(string command, string[] args)
        {
            if (!string.IsNullOrEmpty(command))
            {
                if (!commands.TryGetValue(command, out CommandRegistration reg))
                {
                    AppendLogLine(string.Format("Unknown command <b><color=red>{0}</color></b>, type <color=lime><b>help</b></color> for list.", command));
                }
                else
                {
                    if (reg.handler == null)
                    {
                        AppendLogLine(string.Format("<color=red>Unable to process command:</color> <b>{0}</b>, <color=red>handler was null.</color>", command));
                    }
                    else
                    {
                        reg.handler(args);
                    }
                }
            }
        }

        static string[] ParseArguments(string commandString)
        {
            LinkedList<char> parmChars = new LinkedList<char>(commandString.ToCharArray());
            bool inQuote = false;
            LinkedListNode<char> node = parmChars.First;
            while (node != null)
            {
                LinkedListNode<char> next = node.Next;
                if (node.Value == '"')
                {
                    inQuote = !inQuote;
                    parmChars.Remove(node);
                }
                if (!inQuote && node.Value == ' ')
                {
                    node.Value = '\n';
                }
                node = next;
            }
            char[] parmCharsArr = new char[parmChars.Count];
            parmChars.CopyTo(parmCharsArr, 0);
            return new string(parmCharsArr).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        void HelpCommand(string[] args)
        {
            ModConsole.Print("<color=green><b>Available commands:</b></color>");
            List<CommandRegistration> cmds = commands.Values.GroupBy(x => x.command).Select(g => g.First()).Distinct().ToList();
            foreach (CommandRegistration reg in cmds)
            {
                if(reg.showInHelp)
                    AppendLogLine(string.Format("<color=orange><b>{0}</b></color>: {1}", reg.command, reg.help));
            }
            if(ModLoader.GetCurrentScene() != CurrentScene.Game)
            {
                AppendLogLine("<b><color=red>More commands may appear after you load a save...</color></b>");
            }
        }
    }
    #pragma warning restore CS1591
}