using System;
using UnityEngine;

namespace MSCLoader.Commands;

internal class CommandVersion : ConsoleCommand
{
    public override string Name => "ver";
    public override string Alias => "version";

    public override string Help => "Version information";

    public override void Run(string[] args)
    {
        #if !Mini
        ModConsole.Print($"Unity: <b>{Application.unityVersion}</b>");
        try
        {
            ModConsole.Print($"MSC buildID: <b>{Steamworks.SteamApps.GetAppBuildId()}</b>"); //Get steam buildID
        }
        catch (Exception e)
        {
            ModConsole.Error($"<color=red>Failed to get build ID:</color> <b>{e.Message}</b>"); //Show steamworks error
        }
        ModConsole.Print($"MSCLoader: <b>{ModLoader.MSCLoader_Ver}</b> build <b>{ModLoader.Instance.currentBuild}</b>");
        ModConsole.Print($"Runtime: <b>{System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion}</b>");
#endif
    }
}
