using MSCLoader;
using System.Linq;

namespace MSCLoader.Commands
{
    public class ManifestCommand : ConsoleCommand
    {
        // What the player has to type into the console to execute your commnad
        public override string Name => "Manifest";

        // The help that's displayed for your command when typing help
        public override string Help => "Command Description";
        public override bool ShowInHelp => false;
        // The function that's called when executing command
        public override void Run(string[] args)
        {
            if (args.Length == 2)
            {
                if (args[0].ToLower() == "create")
                {
                    Mod mod = ModLoader.LoadedMods.Where(w => w.ID == args[1]).FirstOrDefault();
                    if (mod != null)
                    {
                        ManifestStuff.CreateManifest(mod);
                    }
                    else
                    {
                        ModConsole.Error("Invalid ModID (ModID is case sensitive)");
                    }
                }
                else if (args[0].ToLower() == "update")
                {
                    Mod mod = ModLoader.LoadedMods.Where(w => w.ID == args[1]).FirstOrDefault();
                    if (mod != null)
                    {
                        ManifestStuff.UpdateManifest(mod);
                    }
                    else
                    {
                        ModConsole.Error("Invalid ModID (ModID is case sensitive)");
                    }
                }
            }
            else
            {
                ModConsole.Error("Invalid syntax");
            }
        }

    }
}
