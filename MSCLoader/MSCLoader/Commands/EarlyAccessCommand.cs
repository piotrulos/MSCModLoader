using MSCLoader;
using System;
using System.IO;
using System.Linq;

namespace MSCLoader.Commands
{
    internal class EarlyAccessCommand : ConsoleCommand
    {
        public override string Name => "ea";

        public override string Help => "ea stuff (modders only)";

        public override bool ShowInHelp => false;

        public override void Run(string[] args)
        {
            if (args.Length == 2)
            {
                if (args[0].ToLower() == "create")
                {
                    try
                    {
                        byte[] randomShit = System.Security.Cryptography.MD5.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes(RandomString(16)));
                        string s = BitConverter.ToString(randomShit).Replace("-", "");
                        string output = Path.Combine(ModLoader.ModsFolder, "EA_Output");
                        if (File.Exists(Path.Combine(ModLoader.ModsFolder, args[1])))
                        {
                            if (!Directory.Exists(output))
                                Directory.CreateDirectory(output);
                            byte[] modFile = File.ReadAllBytes(Path.Combine(ModLoader.ModsFolder, args[1]));
                            byte[] modoutput = modFile.Cry_ScrambleByteRightEnc(System.Text.Encoding.ASCII.GetBytes(s));
                            File.WriteAllBytes(Path.Combine(output, $"{Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, args[1]))}.dII"), modoutput);
                            string txt = $"Here is your key:{Environment.NewLine}{Environment.NewLine}{s}{Environment.NewLine}Use it to whitelist your early access mod.";
                            File.WriteAllText(Path.Combine(output, $"{Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, args[1]))}.txt"), txt);
                            ModConsole.Print($"Go to: {Path.GetFullPath(output)}");
                        }
                        else
                        {
                            ModConsole.Error("File not found");
                        }
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error("Failed with error:");
                        ModConsole.Error(e.Message);
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    ModConsole.Error("Invalid syntax");
                }
            }
            else if (args.Length == 3)
            {
                if (args[0].ToLower() == "create")
                {
                    try
                    {
                        string s = args[2];
                        string output = Path.Combine(ModLoader.ModsFolder, "EA_Output");
                        if (File.Exists(Path.Combine(ModLoader.ModsFolder, args[1])))
                        {
                            if (!Directory.Exists(output))
                                Directory.CreateDirectory(output);
                            byte[] modFile = File.ReadAllBytes(Path.Combine(ModLoader.ModsFolder, args[1]));
                            byte[] modoutput = modFile.Cry_ScrambleByteRightEnc(System.Text.Encoding.ASCII.GetBytes(s));
                            File.WriteAllBytes(Path.Combine(output, $"{Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, args[1]))}.dII"), modoutput);
                            string txt = $"Here is your key:{Environment.NewLine}{Environment.NewLine}{s}{Environment.NewLine}Use it to whitelist your early access mod.";
                            File.WriteAllText(Path.Combine(output, $"{Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, args[1]))}.txt"), txt);
                            ModConsole.Print($"Go to: {Path.GetFullPath(output)}");
                        }
                        else
                        {
                            ModConsole.Error("File not found");
                        }
                    }
                    catch (Exception e)
                    {
                        ModConsole.Error("Failed with error:");
                        ModConsole.Error(e.Message);
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    ModConsole.Error("Invalid syntax");
                }
            }
            else
            {
                ModConsole.Error("Invalid syntax");
            }
        }

        private Random random = new Random();              
        string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
