#if !Mini
using Harmony;
using System.Linq;

namespace MSCLoader.Commands;

internal class HarmonyCommand : ConsoleCommand
{
    public override string Name => "harmony";
    public override string Help => "Debug information about active harmony patches";

    public override void Run(string[] args)
    {
        HarmonyInstance harmony = HarmonyInstance.Create("MSCLoader.Dbg");
        var originals = harmony.GetPatchedMethods().ToList();
        foreach (var original in originals)
        {
            var info = harmony.GetPatchInfo(original);
            ModConsole.Print(original.FullDescription());
            foreach(var owner in info.Owners) ModConsole.Print(owner); 
        }

    }

}

#endif