using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        RButton,
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
        private string settingName = string.Empty;
        private object valueName = string.Empty;
        /// <summary>
        /// The ID of the settings (Should only be used once in your mod).
        /// </summary>
        public string ID;
     
        /// <summary>
        /// Visible name for your setting.
        /// </summary>
        public string Name { get => settingName; set { settingName = value; UpdateName(); } }

        /// <summary>
        /// The Mod this Setting belongs to (This is set when using Add whatever).
        /// </summary>
        public Mod Mod;

        /// <summary>
        /// Default Value for setting.
        /// </summary>
        public object Value { get => valueName; set { valueName = value; UpdateValue(); } }

        /// <summary>
        /// Action to execute for specifed setting.
        /// </summary>
        public Action DoAction;

        internal UnityAction DoUnityAction;
        /// <summary>
        /// Type of setting.
        /// </summary>
        public SettingsType SettingType;

        /// <summary>
        /// Helpful additional variables.
        /// </summary>
        public object[] Vals;

        internal Text NameText;
        internal Text ValueText;
        internal GameObject button, checkbox, label, slider, textbox;
        internal SettingsGroup header;
        void UpdateName()
        {
            if (NameText != null)
            {
                NameText.text = Name;
            }
        }
        void UpdateValue()
        {
            if (ValueText != null)
            {
                if (SettingType==SettingsType.TextBox)
                    ValueText.GetComponentInParent<InputField>().text = Value.ToString();
                else
                    ValueText.text = Value.ToString();
            }
        }
        /// <summary>
        /// Return all settings for mod.
        /// </summary>
        /// <param name="mod">The mod to get the settings for.</param>
        /// <returns>List of Settings for the mod.</returns>
        public static List<Settings> Get(Mod mod) => mod.modSettingsList;

        /// <summary> 
        /// Return all default settings for mod.
        /// </summary>
        /// <param name="mod">The mod to get the settings for.</param>
        /// <returns>List of Settings for the mod.</returns>
        public static List<Settings> GetDefault(Mod mod) => mod.modSettingsDefault;


        /// <summary>
        /// Constructor for Settings
        /// </summary>
        /// <param name="id">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="value">Default Value for this setting</param>
        public Settings(string id, string name, object value)
        {
            ID = id;
            Name = name;
            Value = value;
            DoAction = null;
        }

        /// <summary>
        /// Constructor for Settings
        /// </summary>
        /// <param name="id">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="doAction">Function to execute for this setting</param>
        public Settings(string id, string name, Action doAction)
        {
            ID = id;
            Name = name;
            Value = "DoAction";
            DoAction = doAction;
        }

        internal Settings(string id, string name, UnityAction doUnityAction, bool blockSuspension)
        {
            ID = id;
            Name = name;
            Value = "DoUnityAction";
            DoUnityAction = doUnityAction;
        }
        /// <summary>
        /// Constructor for Settings
        /// </summary>
        /// <param name="id">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="value">Default Value for this setting</param>
        /// <param name="doAction">Function to execute for this setting</param>
        public Settings(string id, string name, object value, Action doAction)
        {
            ID = id;
            Name = name;
            Value = value;
            DoAction = doAction;
        }

        internal Settings(string id, string name, object value, UnityAction doAction, bool blockSuspension)
        {
            ID = id;
            Name = name;
            Value = value;
            DoUnityAction = doAction;
        }
        internal Settings(Mod mod, string id, string name, object value, Action doAction, SettingsType type)
        {
            Mod = mod;
            ID = id;
            Name = name;
            Value = value;
            DoAction = doAction;
            SettingType = type;
        }
        /// <summary>
        /// Hides "reset all settings to default" button.
        /// </summary>
        public static void HideResetAllButton(Mod mod) => mod.hideResetAllSettings = true;

        /// <summary>
        /// Add checkbox to settings menu
        /// Can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="settingID">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="value">Default Value for this setting</param>
        /// <param name="onValueChanged">Function to execute when checkbox value change</param>
        /// <returns>SettingsCheckBox</returns>
        public static SettingsCheckBox AddCheckBox(Mod mod, string settingID, string name, bool value = false, Action onValueChanged = null)
        {
            Settings set = new Settings(mod, settingID, name, value, onValueChanged, SettingsType.CheckBox);
            mod.modSettingsList.Add(set);
            mod.modSettingsDefault.Add(new Settings(mod, settingID, name, value, onValueChanged, SettingsType.CheckBox));
            return new SettingsCheckBox(set);
        }
        /// <summary>
        /// Add checkbox group (radio buttons) to settings menu
        /// Can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="settingID">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="value">Default Value for this setting</param>
        /// <param name="group">Group name (all checkboxes should have same group)</param>
        /// <param name="onValueChanged">Function to execute when checkbox value change</param>
        /// <returns>SettingsCheckBoxGroup</returns>
        public static SettingsCheckBoxGroup AddCheckBoxGroup(Mod mod, string settingID, string name, bool value = false, string group = null, Action onValueChanged = null)
        {
            Settings set = new Settings(mod, settingID, name, value, onValueChanged, SettingsType.CheckBoxGroup);
            set.Vals = new object[1] { group };
            mod.modSettingsList.Add(set);
            mod.modSettingsDefault.Add(new Settings(mod, settingID, name, value, onValueChanged, SettingsType.CheckBoxGroup));
            return new SettingsCheckBoxGroup(set);
        }

        /// <summary>
        /// Add Integer Slider to settings menu
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="settingID">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="minValue">minimum int value</param>
        /// <param name="maxValue">maximum int value</param>
        /// <param name="value">Default Value for this setting</param>
        /// <param name="onValueChanged">Function to execute when slider value change</param>
        /// <param name="textValues">Optional text values array (array index = slider value)</param>
        /// <returns>SettingsSliderInt</returns>
        public static SettingsSliderInt AddSlider(Mod mod, string settingID, string name, int minValue, int maxValue, int value = 0, Action onValueChanged = null, string[] textValues = null)
        {
            Settings set = new Settings(mod, settingID, name, value, onValueChanged, SettingsType.Slider);
            mod.modSettingsDefault.Add(new Settings(mod, settingID, name, value, onValueChanged, SettingsType.Slider));
            set.Vals = new object[4] { minValue, maxValue, true, textValues };
            if (textValues != null)
            {
                if (textValues.Length <= (maxValue - minValue))
                {
                    ModConsole.Error($"[<b>{mod.ID}</b>] AddSlider: array of textValues is smaller than slider range (min to max).");
                }
            }
            mod.modSettingsList.Add(set);
            return new SettingsSliderInt(set);

        }

        /// <summary>
        /// Add Slider to settings menu
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="settingID">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="minValue">minimum float value</param>
        /// <param name="maxValue">maximum float value</param>
        /// <param name="value">Default Value for this setting</param>
        /// <param name="onValueChanged">Function to execute when slider value chang</param>
        /// <param name="decimalPoints">Round value to number of decimal points</param>
        /// <returns></returns>
        public static SettingsSlider AddSlider(Mod mod, string settingID, string name, float minValue, float maxValue, float value = 0f, Action onValueChanged = null, int decimalPoints = 2)
        {
            Settings set = new Settings(mod, settingID, name, value, onValueChanged, SettingsType.Slider);
            mod.modSettingsDefault.Add(new Settings(mod, settingID, name, value, onValueChanged, SettingsType.Slider));
            set.Vals = new object[5] { minValue, maxValue, false, null, decimalPoints };
            if (decimalPoints < 0)
            {
                ModConsole.Error($"[<b>{mod.ID}</b>] AddSlider: decimalPoints cannot be negative (defaulting to 2)");
                set.Vals[4] = (int)2;
            }
            mod.modSettingsList.Add(set);
            return new SettingsSlider(set);
        }

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="settingID">Your unique settings ID</param>
        /// <param name="name">Name of text box</param>
        /// <param name="value">Default TextBox value</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        public static SettingsTextBox AddTextBox(Mod mod, string settingID, string name, string value, string placeholderText) => AddTextBox(mod, settingID, name, value, placeholderText, InputField.ContentType.Standard);

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="settingID">Your unique settings ID</param>
        /// <param name="name">Name of text box</param>
        /// <param name="value">Default TextBox value</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        /// <param name="contentType">InputField content type</param>
        public static SettingsTextBox AddTextBox(Mod mod, string settingID, string name, string value, string placeholderText, InputField.ContentType contentType)
        {
            Settings set = new Settings(mod, settingID, name, value, null, SettingsType.TextBox); 
            mod.modSettingsDefault.Add(new Settings(mod, settingID, name, value, null, SettingsType.TextBox));

            set.Vals = new object[3] { placeholderText, Color.white, contentType };
            mod.modSettingsList.Add(set);
            return new SettingsTextBox(set);
        }

        /// <summary>
        /// Add button that can execute function.
        /// </summary>
        /// <param name="mod">your mod</param>
        /// <param name="settingID">unique settings ID</param>
        /// <param name="name">Text on the button</param>
        /// <param name="onClick">What to do when button is clicked</param>
        public static void AddButton(Mod mod, string settingID, string name, Action onClick) => AddButton(mod, settingID, name, onClick, new Color32(85, 38, 0, 255), Color.white);
        /// <summary>
        /// Add button that can execute function.
        /// </summary>
        /// <param name="mod">your mod</param>
        /// <param name="settingID">unique settings ID</param>
        /// <param name="name">Text on the button</param>
        /// <param name="onClick">What to do when button is clicked</param>
        /// <param name="btnColor">Button background color</param>
        /// <param name="buttonTextColor">Button text color</param>
        public static void AddButton(Mod mod, string settingID, string name, Action onClick, UnityEngine.Color btnColor, UnityEngine.Color buttonTextColor)
        {
            Settings set = new Settings(mod, settingID, name, "DoAction", onClick, SettingsType.Button);
            set.Vals = new object[2] { btnColor, buttonTextColor };
            mod.modSettingsList.Add(set);
        }

        /// <summary>
        /// Add custom reset to default button
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="name">Button name</param>
        /// <param name="sets">array of settings to reset</param>
        public static void AddResetButton(Mod mod, string name, Settings[] sets)
        {
            if (sets != null)
            {
                Settings setting = new Settings("MSCL_ResetSpecificMod", name, null);
                setting.Mod = mod;
                setting.Vals = new object[5];
                setting.SettingType = SettingsType.RButton;
                setting.Vals[0] = sets;
                mod.modSettingsList.Add(setting);
            }
            else
            {
                ModConsole.Error($"[<b>{mod.ID}</b>] AddResetButton: provide at least one setting to reset.");
            }
        }

        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        public static void AddHeader(Mod mod, string HeaderTitle) => AddHeader(mod, HeaderTitle, false);
     
        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
        public static void AddHeader(Mod mod, string HeaderTitle, bool collapsedByDefault = false) => AddHeader(mod, HeaderTitle, new Color32(95, 34, 18, 255), new Color32(236, 229, 2, 255),collapsedByDefault);

        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor) => AddHeader(mod, HeaderTitle, backgroundColor, false);
     
        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor, bool collapsedByDefault = false) => AddHeader(mod, HeaderTitle, backgroundColor, new Color32(236, 229, 2, 255), collapsedByDefault);

        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        /// <param name="textColor">Text Color of header</param>
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor, Color textColor) => AddHeader(mod, HeaderTitle, backgroundColor, textColor, false);

        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        /// <param name="textColor">Text Color of header</param>      
        /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor, Color textColor, bool collapsedByDefault = false)
        {
            Settings setting = new Settings(null, HeaderTitle, null)
            {
                Mod = mod,
                Vals = new object[4] { HeaderTitle, backgroundColor, textColor, collapsedByDefault },
                SettingType = SettingsType.Header
            };
            mod.modSettingsList.Add(setting);
        }

        /// <summary>
        /// Add dynamic Header, same as AddHeader but returns value, you can collapse/expand/change color of it from other settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
        /// <returns>SettingsDynamicHeader</returns>
        public static SettingsDynamicHeader AddDynamicHeader(Mod mod, string HeaderTitle, bool collapsedByDefault = false)
        {
            Color backgroundColor = new Color32(95, 34, 18, 255);
            Color textColor = new Color32(236, 229, 2, 255);

            Settings setting = new Settings(null, HeaderTitle, null)
            {
                Mod = mod,
                Vals = new object[4] { HeaderTitle, backgroundColor, textColor, collapsedByDefault },
                SettingType = SettingsType.Header
            };
            mod.modSettingsList.Add(setting);
            return new SettingsDynamicHeader(setting); 
        }

        /// <summary>
        /// Add just a text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="text">Just a text (supports unity rich text)</param>
        public static void AddText(Mod mod, string text)
        {
            Settings setting = new Settings(null, text, null)
            {
                Mod = mod,
                SettingType = SettingsType.Text
            };
            mod.modSettingsList.Add(setting);
        }

        /// <summary>
        /// Add dynamic text (it is not saved)
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="text">Just a text (supports unity rich text)</param>
        /// <returns>SettingsDynamicText</returns>
        public static SettingsDynamicText AddDynamicText(Mod mod, string text)
        {
            Settings setting = new Settings(null, text, null)
            {
                Mod = mod,
                SettingType = SettingsType.Text
            };
            mod.modSettingsList.Add(setting);
            return new SettingsDynamicText(setting);
        }
        /// <summary>
        /// Get value of setting.
        /// </summary>
        /// <returns>Raw value of setting</returns>
        public object GetValue() => Value; //Return whatever is there

        #region Pre-1.2 settings (Compatibility only)
        /// <summary>
        /// Add checkbox to settings menu (only bool Value accepted)
        /// Can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddCheckBox(Mod mod, Settings setting)
        {
            setting.Mod = mod;
            mod.modSettingsDefault.Add(new Settings(setting.ID, setting.Name, setting.Value) { Mod = mod });

            if (setting.Value is bool)
            {
                setting.SettingType = SettingsType.CheckBox;
                mod.modSettingsList.Add(setting);
            }
            else
            {
                ModConsole.Error($"[<b>{mod.ID}</b>] AddCheckBox: non-bool value.");
            }
        }
        /// <summary>
        /// Add checkbox to settings menu with group, only one checkbox can be set to true in this group
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="group">Unique group name, same for all checkboxes that will be grouped</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddCheckBox(Mod mod, Settings setting, string group)
        {
            setting.Mod = mod;
            mod.modSettingsDefault.Add(new Settings(setting.ID, setting.Name, setting.Value) { Mod = mod });
            setting.Vals = new object[1];

            if (setting.Value is bool)
            {
                setting.SettingType = SettingsType.CheckBoxGroup;
                setting.Vals[0] = group;
                mod.modSettingsList.Add(setting);
            }
            else
            {
                ModConsole.Error($"[<b>{mod.ID}</b>] AddCheckBox: non-bool value.");
            }
        }

        /// <summary>
        /// Add button that can execute function.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="description">Short optional description for this button</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddButton(Mod mod, Settings setting, UnityEngine.Color normalColor, UnityEngine.Color highlightedColor, UnityEngine.Color pressedColor, UnityEngine.Color buttonTextColor, string description = null)
        {
            setting.Mod = mod;
            setting.Vals = new object[2];
            if (string.IsNullOrEmpty(description))
                description = string.Empty;

            if (setting.DoAction != null || setting.DoUnityAction != null)
            {
                setting.SettingType = SettingsType.Button;
                setting.Vals[0] = normalColor;
                setting.Vals[1] = buttonTextColor;
                mod.modSettingsList.Add(setting);
            }
            else
            {
                ModConsole.Error($"[<b>{mod.ID}</b>] AddButton: Action cannot be null.");
            }
        }

        /// <summary>
        /// Add Slider, slider can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="maxValue">Max value of slider</param>
        /// <param name="minValue">Min value of slider</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddSlider(Mod mod, Settings setting, int minValue, int maxValue) => AddSlider(mod, setting, minValue, maxValue, null);

        /// <summary>
        /// Add Slider, slider can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="maxValue">Max value of slider</param>
        /// <param name="minValue">Min value of slider</param>
        /// <param name="textValues">Array of text values (array index equals to slider value)</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddSlider(Mod mod, Settings setting, int minValue, int maxValue, string[] textValues)
        {
            setting.Mod = mod;
            mod.modSettingsDefault.Add(new Settings(setting.ID, setting.Name, setting.Value) { Mod = mod });
            setting.Vals = new object[4];

            //sometimes is double or Single (this should fix that, exclude types)
            if (setting.Value.GetType() != typeof(float) || setting.Value.GetType() != typeof(string))
            {
                setting.SettingType = SettingsType.Slider;
                setting.Vals[0] = minValue;
                setting.Vals[1] = maxValue;
                setting.Vals[2] = true;
                if (textValues == null)
                {
                    setting.Vals[3] = null;
                }
                else
                {
                    setting.Vals[3] = textValues;
                    if (textValues.Length <= (maxValue - minValue))
                    {
                        ModConsole.Error($"[<b>{mod.ID}</b>] AddSlider: array of textValues is smaller than slider range (min to max).");
                    }
                }

                mod.modSettingsList.Add(setting);
            }
            else
            {
                ModConsole.Error($"[<b>{mod.ID}</b>] AddSlider: only int allowed here");
            }
        }

        /// <summary>
        /// Add Slider, slider can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="maxValue">Max value of slider</param>
        /// <param name="minValue">Min value of slider</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddSlider(Mod mod, Settings setting, float minValue, float maxValue) => AddSlider(mod, setting, minValue, maxValue, 2);


        /// <summary>
        /// Add Slider, slider can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="maxValue">Max value of slider</param>
        /// <param name="minValue">Min value of slider</param>
        /// <param name="decimalPoints">Round value to set number of decimal points (default = 2)</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddSlider(Mod mod, Settings setting, float minValue, float maxValue, int decimalPoints)
        {
            setting.Mod = mod;
            mod.modSettingsDefault.Add(new Settings(setting.ID, setting.Name, setting.Value) { Mod = mod });
            setting.Vals = new object[5];

            if (setting.Value is float || setting.Value is double)
            {
                setting.SettingType = SettingsType.Slider;
                setting.Vals[0] = minValue;
                setting.Vals[1] = maxValue;
                setting.Vals[2] = false;
                setting.Vals[3] = null;
                setting.Vals[4] = decimalPoints;
                mod.modSettingsList.Add(setting);
            }
            else
            {
                ModConsole.Error($"[<b>{mod.ID}</b>] AddSlider: only float allowed here");
            }
            if (decimalPoints < 0)
            {
                ModConsole.Error($"[<b>{mod.ID}</b>] AddSlider: decimalPoints cannot be negative.");
            }
        }

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddTextBox(Mod mod, Settings setting, string placeholderText) => AddTextBox(mod, setting, placeholderText, UnityEngine.Color.white);

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        /// <param name="titleTextColor">Text color of title</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddTextBox(Mod mod, Settings setting, string placeholderText, UnityEngine.Color titleTextColor) => AddTextBox(mod, setting, placeholderText, titleTextColor, InputField.ContentType.Standard);

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        /// <param name="titleTextColor">Text color of title</param>
        /// <param name="contentType">InputField content type</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddTextBox(Mod mod, Settings setting, string placeholderText, UnityEngine.Color titleTextColor, InputField.ContentType contentType)
        {
            setting.Mod = mod;
            mod.modSettingsDefault.Add(new Settings(setting.ID, setting.Name, setting.Value) { Mod = mod });

            setting.Vals = new object[3];
            setting.SettingType = SettingsType.TextBox;
            setting.Vals[0] = placeholderText;
            setting.Vals[1] = titleTextColor;
            setting.Vals[2] = contentType;
            mod.modSettingsList.Add(setting);
        }
        #endregion

        //Unused shit for pro compatibility
        internal static void AddHeader(Mod mod, string HeaderTitle, UnityEngine.Color backgroundColor, UnityEngine.Color textColor, Settings set)
        {
            Settings setting = set;
            setting.Mod = mod;
            setting.Vals = new object[3];
            setting.SettingType = SettingsType.Header;
            setting.Vals[0] = HeaderTitle;
            setting.Vals[1] = backgroundColor;
            setting.Vals[2] = textColor;
            mod.modSettingsList.Add(setting);
        }
    }
    /// <summary>
    /// Settings checkbox
    /// </summary>
    public class SettingsCheckBox
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
            catch(Exception ex)
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

        internal SettingsCheckBox(Settings setting)
        {
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
}
