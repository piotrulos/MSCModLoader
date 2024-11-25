#if !Mini
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MSCLoader;


internal class SettingsList
{
    public bool isDisabled;
    public List<Setting> settings = new List<Setting>();
}
internal class Setting
{
    public string ID;
    public object Value;

    public Setting(string id, object value)
    {
        ID = id; 
        Value = value;
    }
}
internal enum SettingsType
{
    CheckBoxGroup,
    CheckBox,
    Button,
    RButton,
    Slider,
    SliderInt,
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
    private static Mod settingsMod = null;

    internal static List<ModSetting> Get(Mod mod) => mod.modSettingsList;

    internal static void ModSettings(Mod modEntry)
    {
        settingsMod = modEntry;
    }

    /// <summary>
    /// Hides "reset all settings to default" button.
    /// </summary>
    public static void HideResetAllButton(Mod mod) => mod.hideResetAllSettings = true;

    /// <summary>
    /// Add Header, header groups settings together
    /// </summary>
    /// <param name="HeaderTitle">Title of your header</param>
    /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
    /// <returns>SettingsHeader</returns>
    public static void AddHeader(string HeaderTitle, bool collapsedByDefault = false) => AddHeader(HeaderTitle, new Color32(95, 34, 18, 255), new Color32(236, 229, 2, 255), collapsedByDefault);
  
    /// <summary>
    /// Add Header, header groups settings together
    /// </summary>
    /// <param name="HeaderTitle">Title of your header</param>
    /// <param name="backgroundColor">Background color of header</param>
    /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
    /// <returns>SettingsHeader</returns>
    public static void AddHeader(string HeaderTitle, Color backgroundColor, bool collapsedByDefault = false) => AddHeader(HeaderTitle, backgroundColor, new Color32(236, 229, 2, 255), collapsedByDefault);

    /// <summary>
    /// Add Header, header groups settings together
    /// </summary>
    /// <param name="HeaderTitle">Title of your header</param>
    /// <param name="backgroundColor">Background color of header</param>
    /// <param name="textColor">Text Color of header</param>      
    /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
    /// <returns>SettingsHeader</returns>
    public static SettingsHeader AddHeader(string HeaderTitle, Color backgroundColor, Color textColor, bool collapsedByDefault = false)
    {
        if(settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddHeader() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }

        SettingsHeader s = new SettingsHeader(HeaderTitle, backgroundColor, textColor, collapsedByDefault);
        settingsMod.modSettingsList.Add(s);
        return s;
    }

    /// <summary>
    /// Add just a text
    /// </summary>
    /// <param name="text">Just a text (supports unity rich text)</param>
    /// <returns>SettingsText</returns>
    public static SettingsText AddText(string text)
    {
        if (settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddText() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }
        SettingsText s = new SettingsText(text);
        settingsMod.modSettingsList.Add(s);
        return s;
    }

    /// <summary>
    /// Add button that can execute function.
    /// </summary>
    /// <param name="name">Text on the button</param>
    /// <param name="onClick">What to do when button is clicked</param>
    public static SettingsButton AddButton(string name, Action onClick) => AddButton(name, onClick, new Color32(85, 38, 0, 255), Color.white);
  
    /// <summary>
    /// Add button that can execute function.
    /// </summary>
    /// <param name="name">Text on the button</param>
    /// <param name="onClick">What to do when button is clicked</param>
    /// <param name="btnColor">Button background color</param>
    /// <param name="buttonTextColor">Button text color</param>
    public static SettingsButton AddButton(string name, Action onClick, Color btnColor, Color buttonTextColor)
    {
        if(settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddButton() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }
        SettingsButton s = new SettingsButton(name, onClick, btnColor, buttonTextColor);
        settingsMod.modSettingsList.Add(s);
        return s;
    }

    /// <summary>
    /// Add checkbox to settings menu
    /// Can execute action when its value is changed.
    /// </summary>
    /// <param name="settingID">Unique settings ID for your mod</param>
    /// <param name="name">Name of the setting</param>
    /// <param name="value">Default Value for this setting</param>
    /// <param name="onValueChanged">Function to execute when checkbox value change</param>
    /// <returns>SettingsCheckBox</returns>
    public static SettingsCheckBox AddCheckBox(string settingID, string name, bool value = false, Action onValueChanged = null)
    {
        if (settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddCheckBox() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }
        SettingsCheckBox s = new SettingsCheckBox(settingID, name, value, onValueChanged);
        settingsMod.modSettingsList.Add(s);
        return s;
    }

    /// <summary>
    /// Add checkbox group (radio buttons) to settings menu
    /// Can execute action when its value is changed.
    /// </summary>
    /// <param name="settingID">Unique settings ID for your mod</param>
    /// <param name="name">Name of the setting</param>
    /// <param name="value">Default Value for this setting</param>
    /// <param name="group">Group name (all checkboxes should have same group)</param>
    /// <param name="onValueChanged">Function to execute when checkbox value change</param>
    /// <returns>SettingsCheckBoxGroup</returns>
    public static SettingsCheckBoxGroup AddCheckBoxGroup(string settingID, string name, bool value = false, string group = null, Action onValueChanged = null)
    {
        if(settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddCheckBoxGroup() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }
        SettingsCheckBoxGroup s = new SettingsCheckBoxGroup(settingID, name, value, group, onValueChanged);
        settingsMod.modSettingsList.Add(s);
        return s;
    }

    /// <summary>
    /// Add Integer Slider to settings menu
    /// </summary>
    /// <param name="settingID">Unique settings ID for your mod</param>
    /// <param name="name">Name of the setting</param>
    /// <param name="minValue">minimum int value</param>
    /// <param name="maxValue">maximum int value</param>
    /// <param name="value">Default Value for this setting</param>
    /// <param name="onValueChanged">Function to execute when slider value change</param>
    /// <param name="textValues">Optional text values array (array index = slider value)</param>
    /// <returns>SettingsSliderInt</returns>
    public static SettingsSliderInt AddSlider(string settingID, string name, int minValue, int maxValue, int value = 0, Action onValueChanged = null, string[] textValues = null)
    {
        if (settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddSlider() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }
        if(textValues != null && textValues.Length <= (maxValue - minValue))
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddSlider() error: textValues array is smaller than slider range (min to max).");
        }
        SettingsSliderInt s = new SettingsSliderInt(settingID, name, value, minValue, maxValue, onValueChanged, textValues);
        settingsMod.modSettingsList.Add(s);
        return s;
    }

    /// <summary>
    /// Add Slider to settings menu
    /// </summary>
    /// <param name="settingID">Unique settings ID for your mod</param>
    /// <param name="name">Name of the setting</param>
    /// <param name="minValue">minimum float value</param>
    /// <param name="maxValue">maximum float value</param>
    /// <param name="value">Default Value for this setting</param>
    /// <param name="onValueChanged">Function to execute when slider value chang</param>
    /// <param name="decimalPoints">Round value to number of decimal points</param>
    /// <returns>SettingsSlider</returns>
    public static SettingsSlider AddSlider(string settingID, string name, float minValue, float maxValue, float value = 0f, Action onValueChanged = null, int decimalPoints = 2)
    {
        if (settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddSlider() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }
        if (decimalPoints < 0)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddSlider() error: decimalPoints cannot be negative (defaulting to 2)");
            decimalPoints = 2;
        }
        SettingsSlider s = new SettingsSlider(settingID, name, value, minValue, maxValue, onValueChanged, decimalPoints);
        settingsMod.modSettingsList.Add(s);
        return s;
    }

    /// <summary>
    /// Add TextBox where user can type any text
    /// </summary>
    /// <param name="settingID">Your unique settings ID</param>
    /// <param name="name">Name of text box</param>
    /// <param name="value">Default TextBox value</param>
    /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
    /// <returns>SettingsTextBox</returns>
    public static SettingsTextBox AddTextBox(string settingID, string name, string value, string placeholderText) => AddTextBox(settingID, name, value, placeholderText, InputField.ContentType.Standard);

    /// <summary>
    /// Add TextBox where user can type any text
    /// </summary>
    /// <param name="settingID">Your unique settings ID</param>
    /// <param name="name">Name of text box</param>
    /// <param name="value">Default TextBox value</param>
    /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
    /// <param name="contentType">InputField content type</param>
    /// <returns>SettingsTextBox</returns>
    public static SettingsTextBox AddTextBox(string settingID, string name, string value, string placeholderText, InputField.ContentType contentType)
    {
        if (settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddTextBox() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }
        SettingsTextBox s = new SettingsTextBox(settingID, name, value, placeholderText, contentType);
        settingsMod.modSettingsList.Add(s);
        return s;
    }

    /// <summary>
    /// Add DropDown List
    /// </summary>
    /// <param name="settingID">unique settings ID</param>
    /// <param name="name">Name of the dropdown list</param>
    /// <param name="arrayOfItems">array of items that will be displayed in list</param>
    /// <param name="defaultSelected">default selected Index ID (default 0)</param>
    /// <param name="OnSelectionChanged">Action when item is selected</param>
    /// <returns>SettingsDropDownList</returns>
    public static SettingsDropDownList AddDropDownList(string settingID, string name, string[] arrayOfItems, int defaultSelected = 0, Action OnSelectionChanged = null)
    {
        if(settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddDropDownList() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }
        SettingsDropDownList s = new SettingsDropDownList(settingID, name, arrayOfItems, defaultSelected, OnSelectionChanged);
        settingsMod.modSettingsList.Add(s);
        return s;
    }
    /// <summary>
    /// Add Color Picker with RGB sliders
    /// </summary>
    /// <param name="mod">Your mod ID</param>
    /// <param name="settingID">unique settings ID</param>
    /// <param name="name">Title of color picker</param>
    /// <param name="OnColorChanged">Action on color changed</param>
    /// <returns>SettingsColorPicker</returns>
    public static SettingsColorPicker AddColorPickerRGB(string settingID, string name, Action OnColorChanged = null) => AddColorPickerRGBAInternal(settingID, name, new Color32(0, 0, 0, 255), OnColorChanged, false);
    /// <summary>
    /// Add Color Picker with RGBA sliders
    /// </summary>
    /// <param name="mod">Your mod ID</param>
    /// <param name="settingID">unique settings ID</param>
    /// <param name="name">Title of color picker</param>
    /// <param name="OnColorChanged">Action on color changed</param>
    /// <returns>SettingsColorPicker</returns>  
    public static SettingsColorPicker AddColorPickerRGBA(string settingID, string name, Action OnColorChanged = null) => AddColorPickerRGBAInternal(settingID, name, new Color32(0, 0, 0, 255), OnColorChanged, true);

    /// <summary>
    /// Add Color Picker with RGB sliders
    /// </summary>
    /// <param name="settingID">unique settings ID</param>
    /// <param name="name">Title of color picker</param>
    /// <param name="defaultColor">Default selected color</param>
    /// <param name="OnColorChanged">Action on color changed</param>
    /// <returns>SettingsColorPicker</returns>      
    public static SettingsColorPicker AddColorPickerRGB(string settingID, string name, Color32 defaultColor, Action OnColorChanged = null) => AddColorPickerRGBAInternal(settingID, name, defaultColor, OnColorChanged, false);
 
    /// <summary>
    /// Add Color Picker with RGBA sliders
    /// </summary>
    /// <param name="settingID">unique settings ID</param>
    /// <param name="name">Title of color picker</param>
    /// <param name="defaultColor">Default selected color</param>
    /// <param name="OnColorChanged">Action on color changed</param>
    /// <returns>SettingsColorPicker</returns>    
    public static SettingsColorPicker AddColorPickerRGBA(string settingID, string name, Color32 defaultColor, Action OnColorChanged = null) => AddColorPickerRGBAInternal(settingID, name, defaultColor, OnColorChanged, true);

    internal static SettingsColorPicker AddColorPickerRGBAInternal(string settingID, string name, Color32 defaultColor, Action OnColorChanged, bool showAlphaSlider)
    {
        if (settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddColorPicker() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }

        SettingsColorPicker s = new SettingsColorPicker(settingID, name, defaultColor, showAlphaSlider, OnColorChanged);
        settingsMod.modSettingsList.Add(s);
        return s;
    }

    /// <summary>
    /// Add Reset button to reset your mod's save file (only works when using unified save system)
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    public static void AddSaveResetButton(Mod mod)
    {
        AddButton("Reset Save File", delegate
        {
            if (ModLoader.CurrentScene != CurrentScene.MainMenu)
            {
                ModUI.ShowMessage("You can only use this in Main Menu");
                return;
            }
            ModUI.ShowYesNoMessage("Are you sure you want to reset this mod save file?", delegate
            {
                SaveLoad.ResetSaveForMod(mod);
                ModUI.ShowMessage("Save file for this mod has been reset");
            });
        });
    }

    /// <summary>
    /// Add custom reset to default button
    /// </summary>
    /// <param name="mod">Your mod instance</param>
    /// <param name="name">Button name</param>
    /// <param name="sets">array of settings to reset</param>
    public static SettingsResetButton AddResetButton(string name, ModSetting[] sets)
    {
        if (settingsMod == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddResetButton() error: unknown Mod instance, settings must be created inside your ModSettings function");
            return null;
        }

        if (sets == null)
        {
            ModConsole.Error($"[<b>{settingsMod}</b>] AddResetButton() error: provide at least one setting to reset.");
            return null;
        }
        SettingsResetButton s = new SettingsResetButton(settingsMod, name, sets);
        settingsMod.modSettingsList.Add(s);
        return s;
    }
}

#endif
