using System;
using System.Collections.Generic;

namespace MSCLoader
{
#pragma warning disable CS1591
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
        Slider,
        TextBox,
        Header,
        Text
    }
#pragma warning restore CS1591

    /// <summary>
    /// Add simple settings for mods.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// List of settings
        /// </summary>
        public static List<Settings> modSettings = new List<Settings>();

        /// <summary>
        /// The ID of the settings (Should only be used once in your mod).
        /// </summary>
        public string ID { get; set; }
     
        /// <summary>
        /// Visible name for your setting.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Mod this Setting belongs to (This is set when using Add whatever).
        /// </summary>
        public Mod Mod { get; set; }

        /// <summary>
        /// Default Value for setting.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Action to execute for specifed setting.
        /// </summary>
        public Action DoAction { get; set; }

        /// <summary>
        /// Type of setting.
        /// </summary>
        public SettingsType type { get; set; }

        /// <summary>
        /// Helpful additional variables.
        /// </summary>
        public object[] Vals { get; set; }

        /// <summary>
        /// Constructor for Settings
        /// </summary>
        /// <param name="iD">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="value">Default Value for this setting</param>
        /// <example><code source="SettingsExamples.cs" region="Constructor1" lang="C#" 
        /// title="Settings constructor" /></example>
        public Settings(string iD, string name, object value)
        {
            ID = iD;
            Name = name;
            Value = value;
            DoAction = null;
        }

        /// <summary>
        /// Constructor for Settings
        /// </summary>
        /// <param name="iD">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="doAction">Function to execute for this setting</param>
        /// <example><code source="SettingsExamples.cs" region="Constructor2" lang="C#" 
        /// title="Settings constructor" /></example>
        public Settings(string iD, string name, Action doAction)
        {
            ID = iD;
            Name = name;
            Value = "DoAction";
            DoAction = doAction;
        }

        /// <summary>
        /// Constructor for Settings
        /// </summary>
        /// <param name="iD">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="value">Default Value for this setting</param>
        /// <param name="doAction">Function to execute for this setting</param>
        /// <example><code source="SettingsExamples.cs" region="Constructor3" lang="C#" 
        /// title="Settings constructor" /></example>
        public Settings(string iD, string name, object value, Action doAction)
        {
            ID = iD;
            Name = name;
            Value = value;
            DoAction = doAction;
        }

        /// <summary>
        /// Return all settings for mod.
        /// </summary>
        /// <param name="mod">The mod to get the settings for.</param>
        /// <returns>List of Settings for the mod.</returns>
        public static List<Settings> Get(Mod mod) => modSettings.FindAll(x => x.Mod == mod);

        /// <summary>
        /// Add checkbox to settings menu (only <see langword="bool"/> Value accepted)
        /// Can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <example><code source="SettingsExamples.cs" region="AddCheckBox" lang="C#" 
        /// title="Add checkbox" /></example>
        public static void AddCheckBox(Mod mod, Settings setting)
        {
            setting.Mod = mod;

            if (setting.Value is bool)
            {
                setting.type = SettingsType.CheckBox;
                modSettings.Add(setting);
            }
            else
            {
                ModConsole.Error("AddCheckBox: non-bool value.");
            }
        }

        /// <summary>
        /// Add checkbox to settings menu with group, only one checkbox can be set to <see langword="true"/> in this group
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="group">Unique group name, same for all checkboxes that will be grouped</param>
        /// <example><code source="SettingsExamples.cs" region="AddCheckBoxGroup" lang="C#" 
        /// title="Add grouped checkboxes" /></example>
        public static void AddCheckBox(Mod mod, Settings setting, string group)
        {
            setting.Mod = mod;
            setting.Vals = new object[1];
            
            if (setting.Value is bool)
            {
                setting.type = SettingsType.CheckBoxGroup;
                setting.Vals[0] = group;
                modSettings.Add(setting);
            }
            else
            {
                ModConsole.Error("AddCheckBox: non-bool value.");
            }
        }

        /// <summary>
        /// Add button that can execute function.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="description">Short optional description for this button</param>
        /// <example><code source="SettingsExamples.cs" region="AddButton" lang="C#" 
        /// title="Add button" /></example>
        public static void AddButton(Mod mod, Settings setting, string description = null) => AddButton(mod, setting, new UnityEngine.Color32(0, 113, 166, 255), new UnityEngine.Color32(0, 153, 166, 255), new UnityEngine.Color32(0, 183, 166, 255), description);
        /// <summary>
        /// Add button that can execute function.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="normalColor">Button color</param>
        /// <param name="highlightedColor">Button color when highlighted</param>
        /// <param name="pressedColor">Button color when pressed</param>
        /// <param name="description">Short optional description for this button</param>
        /// <example><code source="SettingsExamples.cs" region="AddButton" lang="C#" 
        /// title="Add button" /></example>
        public static void AddButton(Mod mod, Settings setting, UnityEngine.Color normalColor, UnityEngine.Color highlightedColor, UnityEngine.Color pressedColor, string description = null) => AddButton(mod, setting, normalColor, highlightedColor, pressedColor, UnityEngine.Color.white, description);
        /// <summary>
        /// Add button that can execute function.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="normalColor">Button color</param>
        /// <param name="highlightedColor">Button color when highlighted</param>
        /// <param name="pressedColor">Button color when pressed</param>
        /// <param name="buttonTextColor">Text color on Button</param>
        /// <param name="description">Short optional description for this button</param>
        /// <example><code source="SettingsExamples.cs" region="AddButton" lang="C#" 
        /// title="Add button" /></example>
        public static void AddButton(Mod mod, Settings setting, UnityEngine.Color normalColor, UnityEngine.Color highlightedColor, UnityEngine.Color pressedColor, UnityEngine.Color buttonTextColor, string description = null)
        {
            setting.Mod = mod;
            setting.Vals = new object[5];
            if (description == null)
                description = string.Empty;

            if (setting.DoAction != null)
            {
                setting.type = SettingsType.Button;
                setting.Vals[0] = description;
                setting.Vals[1] = normalColor;
                setting.Vals[2] = highlightedColor;
                setting.Vals[3] = pressedColor;
                setting.Vals[4] = buttonTextColor;
                modSettings.Add(setting);
            }
            else
            {
                ModConsole.Error("AddButton: Action cannot be null.");
            }
        }

        /// <summary>
        /// Add Slider, slider can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="maxValue">Max value of slider</param>
        /// <param name="minValue">Min value of slider</param>
        /// <example><code source="SettingsExamples.cs" region="AddSlider" lang="C#" 
        /// title="Add Slider" /></example>
        public static void AddSlider(Mod mod, Settings setting, int minValue, int maxValue)
        {
            setting.Mod = mod;
            setting.Vals = new object[3];

            //sometimes is double or Single (this should fix that, exclude types)
            if (setting.Value.GetType() != typeof(float) || setting.Value.GetType() != typeof(string))
            {
                setting.type = SettingsType.Slider;
                setting.Vals[0] = minValue;
                setting.Vals[1] = maxValue;
                setting.Vals[2] = true;
                modSettings.Add(setting);
            }
            else
            {
                ModConsole.Error("AddSlider: only int allowed here");
            }
        }

        /// <summary>
        /// Add Slider, slider can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="maxValue">Max value of slider</param>
        /// <param name="minValue">Min value of slider</param>
        /// <example><code source="SettingsExamples.cs" region="AddSlider" lang="C#" 
        /// title="Add Slider" /></example>
        public static void AddSlider(Mod mod, Settings setting, float minValue, float maxValue)
        {
            setting.Mod = mod;
            setting.Vals = new object[3];

            if (setting.Value is float)
            {
                setting.type = SettingsType.Slider;
                setting.Vals[0] = minValue;
                setting.Vals[1] = maxValue;
                setting.Vals[2] = false;
                modSettings.Add(setting);
            }
            else
            {
                ModConsole.Error("AddSlider: only float allowed here");
            }
        }

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        public static void AddTextBox(Mod mod, Settings setting, string placeholderText) => AddTextBox(mod, setting, placeholderText, UnityEngine.Color.white);

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        /// <param name="titleTextColor">Text color of title</param>
        public static void AddTextBox(Mod mod, Settings setting, string placeholderText, UnityEngine.Color titleTextColor)
        {
            setting.Mod = mod;
            setting.Vals = new object[2];
            setting.type = SettingsType.TextBox;
            setting.Vals[0] = placeholderText;
            setting.Vals[1] = titleTextColor;
            modSettings.Add(setting);
        }

        /// <summary>
        /// Add Header, blue title bar that can be used to separate settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        public static void AddHeader(Mod mod, string HeaderTitle) => AddHeader(mod, HeaderTitle, UnityEngine.Color.blue, UnityEngine.Color.white);

        /// <summary>
        /// Add Header, blue title bar that can be used to separate settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        public static void AddHeader(Mod mod, string HeaderTitle, UnityEngine.Color backgroundColor) => AddHeader(mod, HeaderTitle, backgroundColor, UnityEngine.Color.white);

        /// <summary>
        /// Add Header, blue title bar that can be used to separate settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        /// <param name="textColor">Text Color of header</param>
        public static void AddHeader(Mod mod, string HeaderTitle, UnityEngine.Color backgroundColor, UnityEngine.Color textColor)
        {
            Settings setting = new Settings(null, HeaderTitle, null);
            setting.Mod = mod;
            setting.Vals = new object[3];
            setting.type = SettingsType.Header;
            setting.Vals[0] = HeaderTitle;
            setting.Vals[1] = backgroundColor;
            setting.Vals[2] = textColor;
            modSettings.Add(setting);
        }

        public static void AddText(Mod mod, string text)
        {
            Settings setting = new Settings(null, text, null);
            setting.Mod = mod;
            setting.type = SettingsType.Text;
            modSettings.Add(setting);
        }
        /// <summary>
        /// Get value of setting.
        /// </summary>
        /// <returns><see cref="Value"/> of setting</returns>
        /// <example><code source="SettingsExamples.cs" region="GetValue" lang="C#" 
        /// title="Get Value" /></example>
        public object GetValue() => Value; //Return whatever is there
    }
}
