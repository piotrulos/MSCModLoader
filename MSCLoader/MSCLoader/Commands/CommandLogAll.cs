using System;
using System.IO;

namespace MSCLoader.Commands;

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

    public void Save()
    {
        try
        {
            string savedata = $"{ModLoader.LogAllErrors},{errors},{warnings},{messages}";
            File.WriteAllText(Path.Combine(ModLoader.SettingsFolder, Path.Combine("MSCLoader_Settings","logall.save")), savedata);
            ModConsole.Print($"<color=lime>Log All settings has been saved</color>");
        }
        catch (Exception e)
        {
            ModConsole.Error($"Error: {e.Message}");
            Console.WriteLine(e);
        }
    }
    public void Load()
    {
        try
        {
            if (File.Exists(Path.Combine(ModLoader.SettingsFolder, Path.Combine("MSCLoader_Settings","logall.save"))))
            {
                string savedata = File.ReadAllText(Path.Combine(ModLoader.SettingsFolder, Path.Combine("MSCLoader_Settings","logall.save")));
                string[] data = savedata.Split(',');
                ModLoader.LogAllErrors = bool.Parse(data[0]);
                errors = bool.Parse(data[1]);
                warnings = bool.Parse(data[2]);
                messages = bool.Parse(data[3]);
                if (errors || warnings || messages)
                {
                    if (!setup)
                    {
                        setup = true;
                        UnityEngine.Application.logMessageReceived += HandleLog;
                    }
                }
            }
        }
        catch (Exception e)
        {
            ModConsole.Error($"Error: {e.Message}");
            Console.WriteLine(e);
            File.Delete(Path.Combine(ModLoader.SettingsFolder, Path.Combine("MSCLoader_Settings","logall.save")));
        }
    }
    public override void Run(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0].ToLower())
            {
                case "save":
                    Save();
                    return;
                case "status":
                    ModConsole.Print($"<color=lime><b>Status:</b></color>");
                    if (setup)
                        ModConsole.Print($"<color=orange><b>enabled</b></color>: <color=lime>{setup}</color>");
                    else
                        ModConsole.Print($"<color=orange><b>enabled</b></color>: <color=red>{setup}</color>");
                    if (errors)
                        ModConsole.Print($"<color=orange><b>errors</b></color>: <color=lime>{errors}</color>");
                    else
                        ModConsole.Print($"<color=orange><b>errors</b></color>: <color=red>{errors}</color>");
                    if (warnings)
                        ModConsole.Print($"<color=orange><b>warnings</b></color>: <color=lime>{warnings}</color>");
                    else
                        ModConsole.Print($"<color=orange><b>warnings</b></color>: <color=red>{warnings}</color>");
                    if (messages)
                        ModConsole.Print($"<color=orange><b>messages</b></color>: <color=lime>{messages}</color>");
                    else
                        ModConsole.Print($"<color=orange><b>messages</b></color>: <color=red>{messages}</color>");
                    return;
                case "help":
                    ModConsole.Print($"<color=lime><b>Available settings:</b></color>{Environment.NewLine}" +
             $"<color=orange><b>save</b></color>: Save log all state{Environment.NewLine}" +
             $"<color=orange><b>status</b></color>: Current logall status{Environment.NewLine}" +
             $"<color=orange><b>mods</b></color>: log all errors from mod class{Environment.NewLine}" +
             $"<color=orange><b>errors</b></color>: Show all errors from game{Environment.NewLine}" +
             $"<color=orange><b>warnings</b></color>: Show all warnings from game{Environment.NewLine}" +
             $"<color=orange><b>messages</b></color>: Show all messages from game{Environment.NewLine}" +
             $"<color=orange><b>eveything</b></color>: Sets all above settings to [true|false]{Environment.NewLine}");
                    return;
                case "mods":
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
                case "errors":
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
                    break;
                case "warnings":
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
                    break;
                case "messages":
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
                    break;
                case "everything":
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
                    break;
                default:
                    ModConsole.Warning("<b>Usage:</b> log-all <mods|errors|warnings|messages|everything> [true|false]");
                    ModConsole.Print("Use <color=orange><b>log-all help</b></color> for more info");
                    break;
            }
            if (messages || warnings || errors)
            {
                if (!setup)
                {
                    setup = true;
                    UnityEngine.Application.logMessageReceived += HandleLog;
                }
            }
            else if (!messages && !warnings && !errors)
            {
                if (setup)
                {
                    setup = false;
                    UnityEngine.Application.logMessageReceived -= HandleLog;
                }
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
                {
                    ModConsole.console.controller.AppendLogLine($"<color=red><b>Error: </b>{logString}</color>");
                    if (ModMenu.dm_logST.GetValue())
                        ModConsole.console.controller.AppendLogLine($"<color=red>{stackTrace}</color>");
                    if (ModMenu.dm_operr.GetValue())
                        ModConsole.console.SetVisibility(true);
                }
                break;
            case UnityEngine.LogType.Assert:
                if (messages)
                    ModConsole.console.controller.AppendLogLine($"<color=aqua>{logString}</color>");
                break;
            case UnityEngine.LogType.Warning:
                if (warnings)
                {
                    ModConsole.console.controller.AppendLogLine($"<color=yellow><b>Warning: </b>{logString}</color>");
                    if (ModMenu.dm_warn.GetValue())
                        ModConsole.console.SetVisibility(true);
                }
                break;
            case UnityEngine.LogType.Log:
                if (messages)
                    ModConsole.console.controller.AppendLogLine($"<color=aqua>{logString}</color>");
                break;
            case UnityEngine.LogType.Exception:
                if (errors)
                {
                    ModConsole.console.controller.AppendLogLine($"<color=red><b>Exception: </b>{logString}</color>");
                    if (ModMenu.dm_logST.GetValue())
                        ModConsole.console.controller.AppendLogLine($"<color=red>{stackTrace}</color>");
                    if (ModMenu.dm_operr.GetValue())
                        ModConsole.console.SetVisibility(true);
                }
                break;
            default:
                break;
        }
    }
}
