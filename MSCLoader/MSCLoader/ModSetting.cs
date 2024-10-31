using System;

namespace MSCLoader;

/// <summary>
/// Mod Setting base class
/// </summary>
public class ModSetting
{
    internal string ID;
    internal string Name;
    internal Action DoAction;
    internal SettingsType Type;

    internal ModSetting(string id, string name, Action doAction, SettingsType type)
    {
        ID = id;
        Name = name;
        DoAction = doAction;
        Type = type;
    }
}


/// <summary>
/// Settings checkbox
/// </summary>
public class SettingsCheckBox : ModSetting
{
    internal bool Value = false;
    /// <summary>
    /// Settings Instance (used for custom reset button)
    /// </summary>
    public Settings Instance;

    /// <summary>
    /// Get checkbox value
    /// </summary>
    /// <returns>true/false</returns>
    public bool GetValue()
    {
        try
        {
            return bool.Parse(Instance.GetValue().ToString());
        }
        catch (Exception ex)
        {
            ModConsole.Error($"Settings [ID: <b>{Instance.ID}</b>] Invalid value <b>{Instance.Value}</b>{Environment.NewLine}<b>Error details:</b> {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Set checkbox value
    /// </summary>
    /// <param name="value">true/false</param>
    public void SetValue(bool value)
    {
        Instance.Value = value;
    }

    internal SettingsCheckBox(string id, string name, bool value, Action doAction, SettingsType type, Settings setting) : base(id, name, doAction, type)
    {
        Value = value;
        Instance = setting;
    }
}

/// <summary>
/// CheckBox group (aka radio button)
/// </summary>
public class SettingsCheckBoxGroup
{
    /// <summary>
    /// Settings Instance (used for custom reset button)
    /// </summary>
    public Settings Instance;

    /// <summary>
    /// Get checkbox value
    /// </summary>
    /// <returns>true/false</returns>
    public bool GetValue()
    {
        try
        {
            return bool.Parse(Instance.GetValue().ToString());
        }
        catch (Exception ex)
        {
            ModConsole.Error($"Settings [ID: <b>{Instance.ID}</b>] Invalid value <b>{Instance.Value}</b>{Environment.NewLine}<b>Error details:</b> {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Set checkbox value
    /// </summary>
    /// <param name="value">true/false</param>
    public void SetValue(bool value)
    {
        Instance.Value = value;
    }
    internal SettingsCheckBoxGroup(Settings setting)
    {
        Instance = setting;
    }
}

/// <summary>
/// Integer version of Settings Slider
/// </summary>
public class SettingsSliderInt
{
    /// <summary>
    /// Settings Instance (used for custom reset button)
    /// </summary>
    public Settings Instance;

    /// <summary>
    /// Get slider value
    /// </summary>
    /// <returns>slider value in int</returns>
    public int GetValue()
    {
        try
        {
            return int.Parse(Instance.GetValue().ToString());
        }
        catch (Exception ex)
        {
            ModConsole.Error($"Settings [ID: <b>{Instance.ID}</b>] Invalid value <b>{Instance.Value}</b>{Environment.NewLine}<b>Error details:</b> {ex.Message}");
            return 0;
        }
    }
    internal SettingsSliderInt(Settings s)
    {
        Instance = s;
    }
}

/// <summary>
/// Settings Slider
/// </summary>
public class SettingsSlider
{
    /// <summary>
    /// Settings Instance (used for custom reset button)
    /// </summary>
    public Settings Instance;

    /// <summary>
    /// Get slider value
    /// </summary>
    /// <returns>slider value in float</returns>
    public float GetValue()
    {
        try
        {
            return float.Parse(Instance.GetValue().ToString());
        }
        catch (Exception ex)
        {
            ModConsole.Error($"Settings [ID: <b>{Instance.ID}</b>] Invalid value <b>{Instance.Value}</b>{Environment.NewLine}<b>Error details:</b> {ex.Message}");
            return 0;
        }
    }

    internal SettingsSlider(Settings s)
    {
        Instance = s;
    }
}

/// <summary>
/// Settings TextBox
/// </summary>
public class SettingsTextBox
{
    /// <summary>
    /// Settings Instance (used for custom reset button)
    /// </summary>
    public Settings Instance;

    /// <summary>
    /// Get TextBox value
    /// </summary>
    /// <returns>TextBox string value</returns>
    public string GetValue()
    {
        return Instance.GetValue().ToString();
    }

    /// <summary>
    /// Set value for textbox
    /// </summary>
    /// <param name="value">value</param>
    public void SetValue(string value)
    {
        Instance.Value = value;
    }

    internal SettingsTextBox(Settings s)
    {
        Instance = s;
    }
}
/// <summary>
/// Settings DropDown List
/// </summary>
public class SettingsDropDownList
{
    /// <summary>
    /// Settings Instance (used for custom reset button)
    /// </summary>
    public Settings Instance;

    /// <summary>
    /// Get DropDownList selected Item Index (can be accessed from anywhere)
    /// </summary>
    /// <returns>DropDownList selectedIndex as int</returns>
    public int GetSelectedItemIndex()
    {
        return int.Parse(Instance.GetValue().ToString());
    }

    /// <summary>
    /// Get DropDownList selected Item Name (Only possible if settings are open).
    /// </summary>
    /// <returns>DropDownList selected item name as string</returns>
    public string GetSelectedItemName()
    {
        if (Instance.SettingsElement != null)
        {
            return Instance.SettingsElement.value.text;
        }
        else
        {
            ModConsole.Error("[SettingsDropDownList] ItemName can only be obtained when settings are open.");
            return null;
        }
    }

    /// <summary>
    /// Set DropDownList selected Item Index
    /// </summary>
    /// <param name="value">index</param>
    public void SetSelectedItemIndex(int value)
    {
        Instance.Value = value;
    }

    internal SettingsDropDownList(Settings s)
    {
        Instance = s;
    }
}
/// <summary>
/// Settings Color Picker
/// </summary>
public class SettingsColorPicker
{
    /// <summary>
    /// Settings Instance (used for custom reset button)
    /// </summary>
    public Settings Instance;

    /// <summary>
    /// Get Color32 value
    /// </summary>
    /// <returns>TextBox string value</returns>
    public Color32 GetValue()
    {
        string[] colb = Instance.GetValue().ToString().Split(',');
        new Color32(byte.Parse(colb[0]), byte.Parse(colb[1]), byte.Parse(colb[2]), byte.Parse(colb[3]));
        return new Color32(byte.Parse(colb[0]), byte.Parse(colb[1]), byte.Parse(colb[2]), byte.Parse(colb[3]));
    }

    /// <summary>
    /// Set Color32 value
    /// </summary>
    /// <param name="col">value</param>
    public void SetValue(Color32 col)
    {
        Instance.Value = $"{col.r},{col.g},{col.b},{col.a}";
    }

    internal SettingsColorPicker(Settings s)
    {
        Instance = s;
    }
}

/// <summary>
/// Settings Dynamic Header
/// </summary>
public class SettingsDynamicHeader
{
    /// <summary>
    /// Settings Instance
    /// </summary>
    public Settings Instance;

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
        if (Instance.header == null) return;
        if (skipAnimation)
        {
            Instance.header.SetHeaderNoAnim(false);
            return;
        }
        Instance.header.SetHeader(false);
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
        if (Instance.header == null) return;
        if (skipAnimation)
        {
            Instance.header.SetHeaderNoAnim(true);
            return;
        }
        Instance.header.SetHeader(true);

    }

    /// <summary>
    /// Change title background color
    /// </summary>
    public void SetBackgroundColor(Color color)
    {
        if (Instance.header == null) return;
        Instance.header.HeaderBackground.color = color;
    }

    /// <summary>
    /// Change title text.
    /// </summary>
    public void SetTextColor(Color color)
    {
        if (Instance.header == null) return;
        Instance.header.HeaderTitle.color = color;
    }

    internal SettingsDynamicHeader(Settings s)
    {
        Instance = s;
    }
}

/// <summary>
/// Settings Dynamic Text
/// </summary>
public class SettingsDynamicText
{
    /// <summary>
    /// Settings Instance
    /// </summary>
    public Settings Instance;

    /// <summary>
    /// Get TextBox value
    /// </summary>
    /// <returns>TextBox string value</returns>
    public string GetValue()
    {
        return Instance.Name;
    }

    /// <summary>
    /// Set value for textbox
    /// </summary>
    /// <param name="value">value</param>
    public void SetValue(string value)
    {
        Instance.Name = value;
    }

    internal SettingsDynamicText(Settings s)
    {
        Instance = s;
    }
}

