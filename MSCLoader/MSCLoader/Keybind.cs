using System.Collections.Generic;

namespace MSCLoader;

/// <summary>
/// Add easily rebindable keybinds.
/// </summary>
public partial class Keybind
{
#if !Mini
    private static Mod keybindMod = null;
    internal static void ModSettings(Mod modEntry)
    {
        keybindMod = modEntry;
    }
    internal static List<ModKeybind> GetKeybinds(Mod modEntry) => modEntry.modKeybindsList;
    /// <summary>
    /// Add a keybind.
    /// </summary>
    /// <param name="id">The ID of the Keybind (Used only once in your mod).</param>
    /// <param name="name">The name of the Keybind that will be displayed.</param>
    /// <param name="key">The KeyCode the user will press.</param>
    /// <returns>SettingsKeybind</returns>
    public static SettingsKeybind Add(string id, string name, KeyCode key) => Add(id, name, key, KeyCode.None);

    /// <summary>
    /// Add a keybind with modifier.
    /// </summary>
    /// <param name="id">The ID of the Keybind (Used only once in your mod).</param>
    /// <param name="name">The name of the Keybind that will be displayed.</param>
    /// <param name="key">The KeyCode the user will press.</param>
    /// <param name="modifier">The modifier KeyCode the user will have to press.</param>
    /// <returns>SettingsKeybind</returns>
    public static SettingsKeybind Add(string id, string name, KeyCode key, KeyCode modifier)
    {
        if (keybindMod == null)
        {
            ModConsole.Error($"[<b>{keybindMod}</b>] Keybind.Add() error: unknown Mod instance, keybinds must be created inside your ModSettings function");
            return null;
        }
        SettingsKeybind keybind = new SettingsKeybind(id, name, key, modifier);
        keybindMod.modKeybindsList.Add(keybind);
        return keybind;
    }
    /// <summary>
    /// Add Header, that can be used to group keybinds.
    /// </summary>
    /// <param name="HeaderTitle">Title of your header</param>
    /// <param name="collapsedByDefault">Collapsed by default</param>
    /// <returns>KeybindHeader</returns>
    public static KeybindHeader AddHeader(string HeaderTitle, bool collapsedByDefault = false) => AddHeader(HeaderTitle, Color.blue, Color.white, collapsedByDefault);

    /// <summary>
    /// Add Header, that can be used to group keybinds..
    /// </summary>
    /// <param name="HeaderTitle">Title of your header</param>
    /// <param name="backgroundColor">Background color of header</param>
    /// <param name="textColor">Text Color of header</param>
    /// <param name="collapsedByDefault">Collapsed by default</param>
    /// <returns>KeybindHeader</returns>
    public static KeybindHeader AddHeader(string HeaderTitle, Color backgroundColor, Color textColor, bool collapsedByDefault = false)
    {
        if (keybindMod == null)
        {
            ModConsole.Error($"[<b>{keybindMod}</b>] Keybind.AddHeader() error: unknown Mod instance, keybinds must be created inside your ModSettings function");
            return null;
        }
        KeybindHeader header = new KeybindHeader(HeaderTitle, backgroundColor, textColor, collapsedByDefault);
        keybindMod.modKeybindsList.Add(header);
        return header;
    }
#endif
}
