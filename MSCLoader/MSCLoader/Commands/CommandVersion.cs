using System;
using UnityEngine;

namespace MSCLoader.Commands
{
    internal class CommandVersion : ConsoleCommand
    {
        public override string Name => "ver";
        public override string Alias => "version";

        public override string Help => "Version information";

        public override void Run(string[] args)
        {
            ModConsole.Print(string.Format("Unity: <b>{0}</b>", Application.unityVersion));
            try
            {
                ModConsole.Print(string.Format("MSC buildID: <b>{0}</b>", Steamworks.SteamApps.GetAppBuildId())); //Get steam buildID
            }
            catch (Exception e)
            {
                ModConsole.Error(string.Format("<color=red>Failed to get build ID:</color> <b>{0}</b>", e.Message)); //Show steamworks error
            }
            ModConsole.Print(string.Format("MSCLoader: <b>{0}</b>", ModLoader.MSCLoader_Ver));
            ModConsole.Print(string.Format("Runtime: <b>{0}</b>", System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion));
        }
    }
}
