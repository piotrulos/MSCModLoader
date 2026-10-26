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
        if (args.Length > 0)
        {
            for (int i = 0; i < MSCLInternal.harmonyInstances.Count; i++)
            {
                string[] s = MSCLInternal.harmonyInstances[i].Split('|');
                ModConsole.Print($"<color=aqua>{s[0]}</color> -> <color=lime>{s[1]}</color>");
            }
        }
        HarmonyInstance harmony = HarmonyInstance.Create("MSCLoader.Dbg");
        System.Collections.Generic.List<System.Reflection.MethodBase> originals = harmony.GetPatchedMethods().ToList();
        foreach (System.Reflection.MethodBase original in originals)
        {
            Patches info = harmony.GetPatchInfo(original);
            ModConsole.Print(original.FullDescription());
            foreach (string owner in info.Owners) ModConsole.Print(owner);
        }

    }

}

#endif