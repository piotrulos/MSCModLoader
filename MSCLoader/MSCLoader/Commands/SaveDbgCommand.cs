#if !Mini
using MSCLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSCLoader.Commands
{
    internal class SaveDbgCommand : ConsoleCommand
    {
        public override string Name => "SaveDbg";
        public override string Help => "Debug for Mods.txt save file";
        public override bool ShowInHelp => false;


        public override void Run(string[] args)
        {
            if (args.Length >= 2)
            {
                if (args[0].ToLower() == "mod")
                {
                    bool yes = false;
                    SaveLoad.LoadModsSaveData();
                    string[] tags = SaveLoad.saveFileData.GetTags();

                    foreach (string tag in tags)
                    {
                        if (args.Length > 2)
                        {
                            if (tag == $"{args[1]}||{args[2]}")
                            {
                                yes = true;
                                ES2Header header = new ES2Header();
                                if (SaveLoad.headers.ContainsKey(tag))
                                    SaveLoad.headers.TryGetValue(tag, out header);
                                switch (header.collectionType)
                                {
                                    case ES2Keys.Key._Null:
                                        ModConsole.Print($"[<color=lime>{ES2TypeManager.GetES2Type(header.valueType).type.Name}</color>] -> <color=aqua>{tag.Split('|')[2]}</color>");
                                        break;
                                    default:
                                        ModConsole.Print($"[<color=lime>{ES2TypeManager.GetES2Type(header.valueType).type.Name}</color>][<color=orange>{header.collectionType}</color>] -> <color=aqua>{tag.Split('|')[2]}</color>");
                                        break;
                                }
                                break;
                            }
                        }
                        else
                        {
                            if (tag.StartsWith($"{args[1]}||"))
                            {
                                yes = true;
                                ES2Header header = new ES2Header();
                                if (SaveLoad.headers.ContainsKey(tag))
                                    SaveLoad.headers.TryGetValue(tag, out header);
                                switch (header.collectionType)
                                {
                                    case ES2Keys.Key._Null:
                                        ModConsole.Print($"[<color=lime>{ES2TypeManager.GetES2Type(header.valueType).type.Name}</color>] -> <color=aqua>{tag.Split('|')[2]}</color>");
                                        break;
                                    default:
                                        ModConsole.Print($"[<color=lime>{ES2TypeManager.GetES2Type(header.valueType).type.Name}</color>][<color=orange>{header.collectionType}</color>] -> <color=aqua>{tag.Split('|')[2]}</color>");
                                        break;
                                }
                            }
                        }
                    }
                    if (!yes)
                    {
                        ModConsole.Print($"{args[1]} - not found");
                    }
                }
            }
            else if (args.Length == 1)
            {
                if (args[0].ToLower() == "list")
                {
                    SaveLoad.LoadModsSaveData();
                    string[] tags = SaveLoad.saveFileData.GetTags();
                    List<string> mods = new List<string>();
                    string last = string.Empty;                    
                    foreach (string tag in tags)
                    {
                        if (tag == "MSCLoaderInternalStuff") continue;
                        if(tag.Split('|')[0] != last)
                        {
                            last = tag.Split('|')[0];
                            mods.Add(last);
                        }
                    }
                    if(mods.Count == 0)
                    {
                        ModConsole.Print($"[<color=yellow>Mods.txt has no saved values.</color>]");
                        return;
                    }
                    ModConsole.Print($"[<color=red>!</color>] -> mod is not installed{Environment.NewLine}");                    
                    ModConsole.Print("List of mods with saved values:");
                    foreach (string m in mods)
                    {
                        bool ismod = ModLoader.IsModPresent(m, true);
                        int savedStuff = tags.Where(x => x.StartsWith(m)).Count();
                        if (ismod)
                        {
                            ModConsole.Print($"{m} (<color=orange>{savedStuff}</color><color=lime> saved values</color>)");
                        }
                        else
                        {
                            ModConsole.Print($"[<color=red>!</color>] {m} (<color=orange>{savedStuff}</color><color=lime> saved values</color>)");
                        }
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
#endif