//Class for adding console commands
using System;

namespace MSCLoader.Commands
{
    internal class CommandLogAll : ConsoleCommand
    {
        // What the player has to type into the console to execute your commnad
        public override string Name => "log-all";
        public override string Alias => "logall";

        // The help that's displayed for your command when typing help
        public override string Help => "Log <b>ALL</b> errors/warnings (Warning! May spam console)";

        private bool errors = false;
        private bool warnings = false;
        private bool messages = false;
        private bool setup = false;
        public override void Run(string[] args)
        {
            if (args.Length > 0)
            {
                if (!setup)
                {
                    setup = true;
                    UnityEngine.Application.logMessageReceived += HandleLog;
                }
                if (args[0].ToLower() == "help")
                {
                    ModConsole.Print($"<color=green><b>Available settings:</b></color>{Environment.NewLine}" +
                        $"<color=orange><b>mods</b></color>: log all errors from mod class{Environment.NewLine}" +
                        $"<color=orange><b>errors</b></color>: Show all errors from game{Environment.NewLine}" +
                        $"<color=orange><b>warnings</b></color>: Show all warnings from game{Environment.NewLine}" +
                        $"<color=orange><b>messages</b></color>: Show all messages from game{Environment.NewLine}" +
                        $"<color=orange><b>eveything</b></color>: Sets all above settings to [true|false]{Environment.NewLine}");
                    return;
                }
                else if (args[0].ToLower() == "mods")
                {
                    if (args.Length == 2)
                    {
                        if (args[1].ToLower() == "true")
                            ModLoader.LogAllErrors = true;
                        else
                            ModLoader.LogAllErrors = false;
                        ModConsole.Print($"<color=orange>Log All errors for mods set to <b>{ModLoader.LogAllErrors}</b></color>");
                    }
                    else
                    {
                        ModLoader.LogAllErrors = !ModLoader.LogAllErrors;
                        ModConsole.Print($"<color=orange>Log All errors for mods set to <b>{ModLoader.LogAllErrors}</b></color>");
                    }
                    return;
                }
                else if (args[0].ToLower() == "errors")
                {
                    if (args.Length == 2)
                    {
                        if (args[1].ToLower() == "true")
                            errors = true;
                        else
                            errors = false;
                        ModConsole.Print($"<color=orange>Log All errors set to <b>{errors}</b></color>");
                    }
                    else
                    {
                        errors = !errors;
                        ModConsole.Print($"<color=orange>Log All errors set to <b>{errors}</b></color>");
                    }
                    return;
                }
                else if (args[0].ToLower() == "warnings")
                {
                    if (args.Length == 2)
                    {
                        if (args[1].ToLower() == "true")
                            warnings = true;
                        else
                            warnings = false;
                        ModConsole.Print($"<color=orange>Log All warnings set to <b>{warnings}</b></color>");
                    }
                    else
                    {
                        warnings = !warnings;
                        ModConsole.Print($"<color=orange>Log All warnings set to <b>{warnings}</b></color>");
                    }
                    return;
                }
                else if (args[0].ToLower() == "messages")
                {
                    if (args.Length == 2)
                    {
                        if (args[1].ToLower() == "true")
                            messages = true;
                        else
                            messages = false;
                        ModConsole.Print($"<color=orange>Log All messages set to <b>{messages}</b></color>");
                    }
                    else
                    {
                        messages = !messages;
                        ModConsole.Print($"<color=orange>Log All messages set to <b>{messages}</b></color>");
                    }
                    return;
                }
                else if (args[0].ToLower() == "everything")
                {
                    if (args.Length == 2)
                    {
                        if (args[1].ToLower() == "true")
                        {
                            messages = true;
                            warnings = true;
                            errors = true;
                            ModLoader.LogAllErrors = true;
                            ModConsole.Print("<color=orange>Log everything set to <b>true</b></color>");

                        }
                        else
                        {
                            messages = false;
                            warnings = false;
                            errors = false;
                            ModLoader.LogAllErrors = false;
                            ModConsole.Print("<color=orange>Log everything set to <b>false</b></color>");

                        }
                    }
                    else
                    {
                        ModConsole.Warning("For <b>everything</b> specify [true|false]");
                    }
                    return;
                }
                else
                {
                    ModConsole.Warning("<b>Usage:</b> log-all <mods|errors|warnings|messages|everything> [true|false]");
                    ModConsole.Print("Use <color=orange><b>log-all help</b></color> for more info");
                }

            }
            else
            {
                ModConsole.Warning("<b>Usage:</b> log-all <mods|errors|warnings|messages|everything> [true|false]");
                ModConsole.Print("Use <color=orange><b>log-all help</b></color> for more info");
            }
        }
        void HandleLog(string logString, string stackTrace, UnityEngine.LogType type)
        {
            switch (type)
            {
                case UnityEngine.LogType.Error:
                    if (errors)
                        ModConsole.console.controller.AppendLogLine($"<color=red><b>Error: </b>{logString}</color>");
                    if ((bool)ModMenu.dm_logST.GetValue())
                        ModConsole.console.controller.AppendLogLine($"<color=red>{stackTrace}</color>");
                    if ((bool)ModMenu.dm_operr.GetValue())
                        ModConsole.console.setVisibility(true);
                    break;
                case UnityEngine.LogType.Assert:
                    if (messages)
                        ModConsole.console.controller.AppendLogLine($"<color=aqua>{logString}</color>");
                    break;
                case UnityEngine.LogType.Warning:
                    if (warnings)
                        ModConsole.console.controller.AppendLogLine($"<color=yellow><b>Warning: </b>{logString}</color>");
                    if ((bool)ModMenu.dm_warn.GetValue())
                        ModConsole.console.setVisibility(true);
                    break;
                case UnityEngine.LogType.Log:
                    if (messages)
                        ModConsole.console.controller.AppendLogLine($"<color=aqua>{logString}</color>");
                    break;
                case UnityEngine.LogType.Exception:
                    if (errors)
                        ModConsole.console.controller.AppendLogLine($"<color=red><b>Exception: </b>{logString}</color>");
                    if ((bool)ModMenu.dm_logST.GetValue())
                        ModConsole.console.controller.AppendLogLine($"<color=red>{stackTrace}</color>");
                    if ((bool)ModMenu.dm_operr.GetValue())
                        ModConsole.console.setVisibility(true);
                    break;
                default:
                    break;
            }
        }
    }
}