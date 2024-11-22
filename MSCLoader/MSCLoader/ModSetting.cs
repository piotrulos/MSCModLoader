using System;
using UnityEngine.UI;

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

    internal SettingsElement SettingsElement;
    internal SettingsGroup SettingsHeader;
    internal ModSetting(string id, string name, Action doAction, SettingsType type)
    {
        ID = id;
        Name = name;
        DoAction = doAction;
        Type = type;
    }
    internal void SetElements(SettingsElement settingsElement, SettingsGroup header)
    {
        SettingsElement = settingsElement;
        SettingsHeader = header;
    }
}


/// <summary>
/// Settings checkbox
/// </summary>
public class SettingsCheckBox : ModSetting
{
    internal bool Value = false;
    internal bool DefaultValue = false;
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

    internal SettingsCheckBox(string id, string name, bool value, Action doAction, Settings setting) : base(id, name, doAction, SettingsType.CheckBox)
    {
        Value = value;
        DefaultValue = value;
        Instance = setting;
    }
}

/// <summary>
/// CheckBox group (aka radio button)
/// </summary>
public class SettingsCheckBoxGroup : ModSetting
{
    internal bool Value = false;
    internal bool DefaultValue = false;
    internal string CheckBoxGroup = string.Empty;
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
    internal SettingsCheckBoxGroup(string id, string name, bool value, string group, Action doAction, Settings setting) : base(id, name, doAction, SettingsType.CheckBoxGroup)
    {
        Value = value;
        DefaultValue = value;
        CheckBoxGroup = group;
        Instance = setting;
    }
}

/// <summary>
/// Integer version of Settings Slider
/// </summary>
public class SettingsSliderInt : ModSetting
{
    internal int Value = 0;
    internal int DefaultValue = 0;
    internal int MinValue = 0;
    internal int MaxValue = 100;
    internal string[] TextValues;
    
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
    internal SettingsSliderInt(string id, string name, int value, int minValue, int maxValue,  Action onValueChanged, string[] textValues, Settings s) : base(id, name, onValueChanged, SettingsType.Slider)
    {
        Value = value;
        DefaultValue = value;
        MinValue = minValue;
        MaxValue = maxValue;
        TextValues = textValues;
        Instance = s;
    }
}

/// <summary>
/// Settings Slider
/// </summary>
public class SettingsSlider : ModSetting
{
    internal float Value = 0;
    internal float DefaultValue = 0;
    internal float MinValue = 0;
    internal float MaxValue = 100;
    internal int DecimalPoints = 0;
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

    internal SettingsSlider(string id, string name, float value, float minValue, float maxValue, Action onValueChanged, int decimalPoints, Settings s) : base(id, name, onValueChanged, SettingsType.Slider)
    {   
        Value = value;
        DefaultValue = value;
        MinValue = minValue;
        MaxValue = maxValue;
        DecimalPoints = decimalPoints;
        Instance = s;
    }
}

/// <summary>
/// Settings TextBox
/// </summary>
public class SettingsTextBox : ModSetting
{
    internal string Value = string.Empty;
    internal string DefaultValue = string.Empty;
    internal string Placeholder = string.Empty;
    internal InputField.ContentType ContentType = InputField.ContentType.Standard;

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

    internal SettingsTextBox(string id, string name, string value, string placeholder, InputField.ContentType contentType, Settings s) : base(id, name, null, SettingsType.TextBox)
    {
        Value = value;
        DefaultValue = value;
        Placeholder = placeholder;
        ContentType = contentType;
        Instance = s;
    }
}
/// <summary>
/// Settings DropDown List
/// </summary>
public class SettingsDropDownList : ModSetting
{
    internal int Value = 0;
    internal string[] ArrayOfItems = new string[0];
    internal int DefaultValue = 0;

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

    internal SettingsDropDownList(string id, string name, string[] arrayOfItems, int defaultValue, Action onSelectionChanged, Settings s) : base(id, name, onSelectionChanged, SettingsType.DropDown)
    {
        Value = defaultValue;
        ArrayOfItems = arrayOfItems;
        DefaultValue = defaultValue;
        Instance = s;
    }
}
/// <summary>
/// Settings Color Picker
/// </summary>
public class SettingsColorPicker : ModSetting
{
    internal string colorValue = "0,0,0,255";
    internal String DefaultColorValue = "0,0,0,255";
    internal bool ShowAlpha = false;
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

    internal SettingsColorPicker(string id, string name, Color32 defaultColor, bool showAlpha, Action onColorChanged, Settings s) : base(id, name, onColorChanged, SettingsType.ColorPicker)
    {
        colorValue = $"{defaultColor.r},{defaultColor.g},{defaultColor.b},{defaultColor.a}";
        DefaultColorValue = $"{defaultColor.r},{defaultColor.g},{defaultColor.b},{defaultColor.a}";
        ShowAlpha = showAlpha;
        Instance = s;
    }
}

public class SettingsHeader : ModSetting
{
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
       /* if (Instance.header == null) return;
        if (skipAnimation)
        {
            Instance.header.SetHeaderNoAnim(false);
            return;
        }
        Instance.header.SetHeader(false);*/
    }

    public SettingsHeader(string id, string name, bool collapsedByDefault) : base(id, name, null, SettingsType.Header) 
    { 

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

