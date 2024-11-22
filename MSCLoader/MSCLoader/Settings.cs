#if !Mini
using System.Collections.Generic;

namespace MSCLoader;


public class SettingsList
{
    public bool isDisabled { get; set; }
    public List<Setting> settings = new List<Setting>();
}
public class Setting
{
    public string ID { get; set; }
    public object Value { get; set; }
}
public enum SettingsType
{
    CheckBoxGroup,
    CheckBox,
    Button,
    RButton,
    Slider,
    TextBox,
    Header,
    Text,
    DropDown,
    ColorPicker
}

/// <summary>
/// Add simple settings for mods.
/// </summary>
public partial class Settings
{
    private static Mod mod = null;

    internal static void ModSettings(Mod modEntry)
    {
        mod = modEntry;
    }

}

#endif
