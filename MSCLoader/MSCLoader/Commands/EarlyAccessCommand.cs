#if !Mini
using System;
using System.IO;
using System.Linq;

namespace MSCLoader.Commands;

internal class EarlyAccessCommand : ConsoleCommand
{
    public override string Name => "ea";

    public override string Help => "ea stuff (modders only)";

    public override bool ShowInHelp => false;

    private readonly string output = Path.Combine(ModLoader.ModsFolder, "EA_Output");
    public override void Run(string[] args)
    {
        if (args.Length == 2)
        {
            if (args[0].ToLower() == "create")
            {
                CreateEA(args[1]);
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

    private System.Random random = new System.Random();
    string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    void GenerateFile(string file, string key)
    {
        if (File.Exists(Path.Combine(ModLoader.ModsFolder, file)))
        {
            byte[] header = { 0x45, 0x41, 0x4D, 0x33 };
            byte[] modFile = File.ReadAllBytes(Path.Combine(ModLoader.ModsFolder, file));
            byte[] modoutput = [];
            try
            {
                modoutput = modFile.EncByteArray(System.Text.Encoding.ASCII.GetBytes(key));
            }
            catch(Exception e)
            {
                ModConsole.Error(e.Message);
                Console.WriteLine(e);
                return;
            }
            File.WriteAllBytes(Path.Combine(output, $"{Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, file))}.dll"), header.Concat(modoutput).ToArray());
            string txt = $"EAM3|{key}{Environment.NewLine}{Environment.NewLine}Use this command to register your mod:{Environment.NewLine}{Environment.NewLine}!ea registerfile {Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, file))} {key}{Environment.NewLine}{Environment.NewLine}If you already registered that file before and want to update key use this:{Environment.NewLine}{Environment.NewLine}!ea setkey {Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, file))} {key}";
            File.WriteAllText(Path.Combine(output, $"{Path.GetFileNameWithoutExtension(Path.Combine(ModLoader.ModsFolder, file))}.txt"), txt);
            // ModConsole.Print($"Go to: {Path.GetFullPath(output)}");
        }
        else
        {
            ModConsole.Error("File not found");
        }
    }

    void CreateEA(string file)
    {
        if (!File.Exists(Path.Combine(ModLoader.ModsFolder, file)))
        {
            ModConsole.Error("File not found");
            return;
        }
        if (!Directory.Exists(output))
        {
            Directory.CreateDirectory(output);
        }
        else
        {
            if (File.Exists(Path.Combine(output, file.Replace(".dll", ".txt"))))
            {
                string[] data = File.ReadAllLines(Path.Combine(output, file.Replace(".dll", ".txt")));
                if (data[0].StartsWith("EAM3|"))
                {
                    ShowEAWindow(file, data[0].Split('|')[1], false);
                    return;
                }
            }
        }
        ShowEAWindow(file, string.Empty, true);
    }
    void ShowEAWindow(string file, string key, bool newFile)
    {
        byte[] randomShit = System.Security.Cryptography.MD5.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes(RandomString(32)));
        string generatedKey = BitConverter.ToString(randomShit).Replace("-", "");
        PopupSetting eaWindow = ModUI.CreatePopupSetting("Create EA file", "Close");

        if (newFile)
        {
            GenerateFile(file, generatedKey);
            eaWindow.AddText($"EA file for: <color=aqua>{file}</color> has been created{Environment.NewLine}");
            eaWindow.AddButton("Open EA folder", delegate
            {
                System.Diagnostics.Process.Start(output);
            }, SettingsButton.ButtonIcon.Folder);
        }
        else
        {
            eaWindow.AddText($"EA file for: <color=aqua>{file}</color> already exists{Environment.NewLine}");
            eaWindow.AddText($"Select option below on how you want to update this file");
            eaWindow.AddText($"<color=orange>Update with existing key</color> - This will update EA file with existing key");
            eaWindow.AddText($"<color=orange>Update with new key</color> - This will generate new key and update EA file, making old key invalid");
            
            eaWindow.AddButton("Update with existing key", delegate
            {
                GenerateFile(file, key);
                ModUI.ShowMessage($"EA file for: <color=aqua>{file}</color> has been updated");
                eaWindow.ClosePopup();
            }, SettingsButton.ButtonIcon.Update);
            eaWindow.AddButton("Update with new key", delegate
            {
                GenerateFile(file, generatedKey);
                ModUI.ShowMessage($"EA file for: <color=aqua>{file}</color> has been updated with new key");
                eaWindow.ClosePopup();
            }, SettingsButton.ButtonIcon.Add);
            eaWindow.AddButton("Open EA folder", delegate
            {
                System.Diagnostics.Process.Start(output);
            }, SettingsButton.ButtonIcon.Folder);
        }

        eaWindow.ShowPopup(null);
    }
}
#endif