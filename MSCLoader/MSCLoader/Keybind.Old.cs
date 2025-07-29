using System;
using System.Collections.Generic;

namespace MSCLoader;

/// <summary>
/// Add easily rebindable keybinds.
/// </summary>
public partial class Keybind
{
    /// <summary>
    /// The ID of the keybind (Should only be used once in your mod).
    /// </summary>
    [Obsolete("Please switch to new settings format", true)]
    public string ID { get => keybindBC.ID; set { } }

    /// <summary>
    /// The name that will be displayed in settings
    /// </summary>
    [Obsolete("Please switch to new settings format", true)]
    public string Name { get => keybindBC.Name; set { } }

    /// <summary>
    /// The KeyCode the user will have to press.
    /// </summary>
    [Obsolete("Please switch to new settings format", true)]
    public KeyCode Key { get => keybindBC.KeybKey; set { } }

    /// <summary>
    /// The modifier KeyCode the user will have to press with the Key.
    /// </summary>
    [Obsolete("Please switch to new settings format", true)]
    public KeyCode Modifier { get => keybindBC.KeybModif; set { } }

    /// <summary>
    /// The Mod this Keybind belongs to (This is set when using Keybind.Add).
    /// </summary>
    [Obsolete("Please switch to new settings format", true)]
    public Mod Mod { get; set; }

    /// <summary>
    /// Helpful additional variables.
    /// </summary>
    [Obsolete("Please switch to new settings format", true)]
    public object[] Vals { get; set; }

    internal SettingsKeybind keybindBC;

#if !Mini
    /// <summary>
    /// Add a keybind.
    /// </summary>
    /// <param name="mod">The instance of your mod.</param>
    /// <param name="key">The Keybind to add.</param>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("Please switch to SettingsKeybind variable", true)]
    public static void Add(Mod mod, Keybind key)
    {
        key.Mod = mod;
        keybindMod = mod;
        SettingsKeybind keybind = new SettingsKeybind(key.ID, key.Name, key.Key, key.Modifier);
        key.keybindBC = keybind;
        keybind.BCInstance = key;
        keybindMod.modKeybindsList.Add(keybind);
    }
    /// <summary>
    /// Add a keybind.
    /// </summary>
    /// <param name="mod">The instance of your mod.</param>
    /// <param name="id">The ID of the Keybind (Used only once in your mod).</param>
    /// <param name="name">The name of the Keybind that will be displayed.</param>
    /// <param name="key">The KeyCode the user will press.</param>
    /// <returns>Keybind</returns>
    [Obsolete("Remove 'this ,' parameter to switch to new format", true)]
    public static Keybind Add(Mod mod, string id, string name, KeyCode key)
    {
        return Add(mod, id, name, key, KeyCode.None);

    }
    /// <summary>
    /// Add a keybind.
    /// </summary>
    /// <param name="mod">The instance of your mod.</param>
    /// <param name="id">The ID of the Keybind (Used only once in your mod).</param>
    /// <param name="name">The name of the Keybind that will be displayed.</param>
    /// <param name="key">The KeyCode the user will press.</param>
    /// <param name="modifier">The modifier KeyCode the user will have to press.</param>
    /// <returns>Keybind</returns>
    [Obsolete("Remove 'this ,' parameter to switch to new format", true)]
    public static Keybind Add(Mod mod, string id, string name, KeyCode key, KeyCode modifier)
    {
        Keybind keyb = new Keybind(id, name, key, modifier) { Mod = mod };
        keybindMod = mod;
        SettingsKeybind keybind = new SettingsKeybind(id, name, key, modifier);
        keyb.keybindBC = keybind;
        keybind.BCInstance = keyb;

        keybindMod.modKeybindsList.Add(keybind);
        return keyb;

    }
    /// <summary>
    /// Add Header, blue title bar that can be used to separate settings.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="HeaderTitle">Title of your header</param>
    [Obsolete("Remove 'this ,' parameter to switch to new format", true)]
    public static void AddHeader(Mod mod, string HeaderTitle) => AddHeader(mod, HeaderTitle, Color.blue, Color.white);

    /// <summary>
    /// Add Header, blue title bar that can be used to separate settings.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="HeaderTitle">Title of your header</param>
    /// <param name="backgroundColor">Background color of header</param>
    [Obsolete("Remove 'this ,' parameter to switch to new format", true)]
    public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor) => AddHeader(mod, HeaderTitle, backgroundColor, Color.white);

    /// <summary>
    /// Add Header, blue title bar that can be used to separate settings.
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="HeaderTitle">Title of your header</param>
    /// <param name="backgroundColor">Background color of header</param>
    /// <param name="textColor">Text Color of header</param>
    [Obsolete("Remove 'this ,' parameter to switch to new format", true)]
    public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor, Color textColor)
    {
        keybindMod = mod;
        KeybindHeader header = new KeybindHeader(HeaderTitle, backgroundColor, textColor, false);
        keybindMod.modKeybindsList.Add(header);
    }
    /// <summary>
    /// Undocumented crap don't use
    /// </summary>
    /// <param name="mod"></param>
    /// <returns></returns>
    [Obsolete("Stop using undocumented crap", true)]
    public static List<Keybind> Get(Mod mod)
    {
        List<Keybind> crap = new List<Keybind>();
        foreach (ModKeybind setting in mod.modKeybindsList)
        {
            if (setting.IsHeader) continue;
            crap.Add(((SettingsKeybind)setting).BCInstance);
        }
        return crap;
    }

    /// <summary>
    /// Constructor for Keybind without modifier
    /// </summary>
    /// <param name="id">The ID of the Keybind.</param>
    /// <param name="name">The name of the Keybind.</param>
    /// <param name="key">The KeyCode the user will press.</param>
    [Obsolete("Please switch to new settings format", true)]
    public Keybind(string id, string name, KeyCode key)
    {
        SettingsKeybind keybind = new SettingsKeybind(id, name, key, KeyCode.None);
        keybindBC = keybind;
        ID = id;
        Name = name;
        Key = key;
        Modifier = KeyCode.None;
    }

    /// <summary>
    /// Constructor for Keybind
    /// </summary>
    /// <param name="id">The ID of the Keybind (Used only once in your mod).</param>
    /// <param name="name">The name of the Keybind that will be displayed.</param>
    /// <param name="key">The KeyCode the user will press.</param>
    /// <param name="modifier">The modifier KeyCode the user will have to press.</param>
    [Obsolete("Please switch to new settings format", true)]
    public Keybind(string id, string name, KeyCode key, KeyCode modifier)
    {
        SettingsKeybind keybind = new SettingsKeybind(id, name, key, modifier);
        keybindBC = keybind;
        ID = id;
        Name = name;
        Key = key;
        Modifier = modifier;
    }

    /// <summary>
    /// Check if keybind is being hold down. (Same behaviour as GetKey)
    /// </summary>
    /// <returns>true, if the keybind is being hold down.</returns>
    [Obsolete("Please switch to SettingsKeybind variable", true)]
    public bool GetKeybind()
    {
        return keybindBC.GetKeybind();
    }

    /// <summary>
    /// Check if the keybind was just pressed once. (Same behaviour as GetKeyDown)
    /// </summary>
    /// <returns>true, Check if the keybind was just pressed.</returns>

    [Obsolete("Please switch to SettingsKeybind variable", true)]
    public bool GetKeybindDown()
    {
        return keybindBC.GetKeybindDown();
    }

    /// <summary>
    /// Check if the keybind was just released. (Same behaviour as GetKeyUp)
    /// </summary>
    /// <returns>true, Check if the keybind was just released.</returns>
    [Obsolete("Please switch to SettingsKeybind variable", true)]
    public bool GetKeybindUp()
    {
        return keybindBC.GetKeybindUp();
    }

    /// <summary>
    /// [DEPRECATED] Checks if the Keybind is being held down.
    /// </summary>
    /// <returns>true, if the Keybind is being held down.</returns>
    [Obsolete("IsPressed() is deprecated, just rename it to GetKeybind()", true)]
    public bool IsPressed()
    {
        return GetKeybind();
    }

    /// <summary>
    /// [DEPRECATED] Checks if the Keybind was just pressed.
    /// </summary>
    /// <returns>true, if the Keybind is being pressed.</returns>
    [Obsolete("IsDown() is deprecated, just rename it to GetKeybindDown()", true)]
    public bool IsDown()
    {
        return GetKeybindDown();
    }
#endif
}
