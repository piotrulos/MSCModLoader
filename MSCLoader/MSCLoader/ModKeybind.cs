#if !Mini 
using System;
using UnityEngine.UI;

namespace MSCLoader;

/// <summary>
/// Mod Setting base class
/// </summary>
public class ModKeybind
{
    internal string ID;
    internal string Name;
    internal SettingsGroup HeaderElement;
    internal bool IsHeader;

    internal ModKeybind(string id, string name, bool isHeader)
    {
        ID = id;
        Name = name;
        IsHeader = isHeader;
    }
}

/// <summary>
/// Keybind Header
/// </summary>
public class KeybindHeader : ModKeybind
{
    internal Color BackgroundColor = new Color32(95, 34, 18, 255);
    internal Color TextColor = new Color32(236, 229, 2, 255);
    internal bool CollapsedByDefault = false;

    /// <summary>
    /// Collapse this header
    /// </summary>
    public void Collapse() => Collapse(false);

    /// <summary>
    /// Collapse this header without animation
    /// </summary>
    /// <param name="skipAnimation">true = skip collapsing animation</param>
    public void Collapse(bool skipAnimation)
    {
        if (HeaderElement == null) return;
        if (skipAnimation)
        {
            HeaderElement.SetHeaderNoAnim(false);
            return;
        }
        HeaderElement.SetHeader(false);
    }

    /// <summary>
    /// Expand this Header
    /// </summary>
    public void Expand() => Expand(false);

    /// <summary>
    /// Expand this Header without animation
    /// </summary>
    /// <param name="skipAnimation">true = skip expanding animation</param>
    public void Expand(bool skipAnimation)
    {
        if (HeaderElement == null) return;
        if (skipAnimation)
        {
            HeaderElement.SetHeaderNoAnim(true);
            return;
        }
        HeaderElement.SetHeader(true);

    }

    /// <summary>
    /// Change title background color
    /// </summary>
    public void SetBackgroundColor(Color color)
    {
        if (HeaderElement == null) return;
        HeaderElement.HeaderBackground.color = color;
    }

    /// <summary>
    /// Change title text.
    /// </summary>
    public void SetTextColor(Color color)
    {
        if (HeaderElement == null) return;
        HeaderElement.HeaderTitle.color = color;
    }


    internal KeybindHeader(string name, Color backgroundColor, Color textColor, bool collapsedByDefault) : base(null, name,true)
    {
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        CollapsedByDefault = collapsedByDefault;
    }
}
/// <summary>
/// Keybind
/// </summary>
public class SettingsKeybind : ModKeybind
{
    internal KeyCode KeybKey;
    internal KeyCode KeybModif;
    internal KeyCode DefaultKeybKey;
    internal KeyCode DefaultKeybModif;
    internal Keybind BCInstance;

    internal SettingsKeybind(string id, string name, KeyCode key, KeyCode modifier) : base(id, name, false)
    {
        KeybKey = key;
        KeybModif = modifier;
        DefaultKeybKey = key;
        DefaultKeybModif = modifier;
    }

    /// <summary>
    /// Check if keybind is being hold down. (Same behaviour as GetKey)
    /// </summary>
    /// <returns>true, if the keybind is being hold down.</returns>
    public bool GetKeybind()
    {
        if (KeybModif != KeyCode.None)
        {
            return Input.GetKey(KeybModif) && Input.GetKey(KeybKey);
        }

        return Input.GetKey(KeybKey);
    }

    /// <summary>
    /// Check if the keybind was just pressed once. (Same behaviour as GetKeyDown)
    /// </summary>
    /// <returns>true, Check if the keybind was just pressed.</returns>
    public bool GetKeybindDown()
    {
        if (KeybModif != KeyCode.None)
        {
            return Input.GetKey(KeybModif) && Input.GetKeyDown(KeybKey);
        }

        return Input.GetKeyDown(KeybKey);
    }

    /// <summary>
    /// Check if the keybind was just released. (Same behaviour as GetKeyUp)
    /// </summary>
    /// <returns>true, Check if the keybind was just released.</returns>
    public bool GetKeybindUp()
    {
        if (KeybModif != KeyCode.None)
        {
            return Input.GetKey(KeybModif) && Input.GetKeyUp(KeybKey);
        }

        return Input.GetKeyUp(KeybKey);
    }
}
#endif