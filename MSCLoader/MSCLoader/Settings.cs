﻿using System;
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
        public object Value;

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
        //TODO: setactive();  
        internal GameObject button, checkbox, label, slider, textbox;

        void UpdateName()
        {
            if (NameText != null)
            {
                NameText.text = Name;
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
        /// <example><code source="SettingsExamples.cs" region="Constructor1" lang="C#" 
        /// title="Settings constructor" /></example>
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
        /// <example><code source="SettingsExamples.cs" region="Constructor2" lang="C#" 
        /// title="Settings constructor" /></example>
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
        /// <example><code source="SettingsExamples.cs" region="Constructor3" lang="C#" 
        /// title="Settings constructor" /></example>
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
        public static void HideResetAllButton(Mod mod) => mod.modSettingsDefault.Add(new Settings("MSCL_HideResetAllButton", null, null) { Mod = mod });


        public static SettingsCheckBox AddCheckBox(Mod mod, string settingID, string name, bool value = false, Action onValueChanged = null)
        {
            Settings set = new Settings(mod, settingID, name, value, onValueChanged, SettingsType.CheckBox);
            mod.modSettingsList.Add(set);
            mod.modSettingsDefault.Add(new Settings(mod, settingID, name, value, onValueChanged, SettingsType.CheckBox));
            return new SettingsCheckBox(set);
        }
        public static SettingsCheckBoxGroup AddCheckBoxGroup(Mod mod, string settingID, string name, bool value = false, string group = null, Action onValueChanged = null)
        {
            Settings set = new Settings(mod, settingID, name, value, onValueChanged, SettingsType.CheckBoxGroup);
            set.Vals = new object[1] { group };
            mod.modSettingsList.Add(set);
            mod.modSettingsDefault.Add(new Settings(mod, settingID, name, value, onValueChanged, SettingsType.CheckBoxGroup));
            return new SettingsCheckBoxGroup(set);
        }

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
        /// Add checkbox to settings menu (only bool Value accepted)
        /// Can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <example><code source="SettingsExamples.cs" region="AddCheckBox" lang="C#" 
        /// title="Add checkbox" /></example>
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
        /// <example><code source="SettingsExamples.cs" region="AddCheckBoxGroup" lang="C#" 
        /// title="Add grouped checkboxes" /></example>
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
            if (string.IsNullOrEmpty(description))
                description = string.Empty;

            if (setting.DoAction != null || setting.DoUnityAction != null)
            {
                setting.SettingType = SettingsType.Button;
                setting.Vals[0] = description;
                setting.Vals[1] = normalColor;
                setting.Vals[2] = highlightedColor;
                setting.Vals[3] = pressedColor;
                setting.Vals[4] = buttonTextColor;
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
        /// <example><code source="SettingsExamples.cs" region="AddSlider" lang="C#" 
        /// title="Add Slider" /></example>
        public static void AddSlider(Mod mod, Settings setting, int minValue, int maxValue) => AddSlider(mod, setting, minValue, maxValue, null);
      
        /// <summary>
        /// Add Slider, slider can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="maxValue">Max value of slider</param>
        /// <param name="minValue">Min value of slider</param>
        /// <param name="textValues">Array of text values (array index equals to slider value)</param>
        /// <example><code source="SettingsExamples.cs" region="AddSlider" lang="C#" 
        /// title="Add Slider" /></example>
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
        /// <example><code source="SettingsExamples.cs" region="AddSlider" lang="C#" 
        /// title="Add Slider" /></example>
        public static void AddSlider(Mod mod, Settings setting, float minValue, float maxValue) => AddSlider(mod, setting, minValue, maxValue, 2);


        /// <summary>
        /// Add Slider, slider can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="maxValue">Max value of slider</param>
        /// <param name="minValue">Min value of slider</param>
        /// <param name="decimalPoints">Round value to set number of decimal points (default = 2)</param>
        /// <example><code source="SettingsExamples.cs" region="AddSlider" lang="C#" 
        /// title="Add Slider" /></example>
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
            if(decimalPoints < 0)
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
        public static void AddTextBox(Mod mod, Settings setting, string placeholderText) => AddTextBox(mod, setting, placeholderText, UnityEngine.Color.white);

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        /// <param name="titleTextColor">Text color of title</param>
        public static void AddTextBox(Mod mod, Settings setting, string placeholderText, UnityEngine.Color titleTextColor) => AddTextBox(mod, setting, placeholderText, titleTextColor, InputField.ContentType.Standard);

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        /// <param name="titleTextColor">Text color of title</param>
        /// <param name="contentType">InputField content type</param>
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

        //Static settings types

        /// <summary>
        /// Add Header, blue title bar that can be used to separate settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        public static void AddHeader(Mod mod, string HeaderTitle) => AddHeader(mod, HeaderTitle, new UnityEngine.Color32(101, 34, 18, 255), new Color32(236, 229, 2, 255));

        /// <summary>
        /// Add Header, blue title bar that can be used to separate settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        public static void AddHeader(Mod mod, string HeaderTitle, UnityEngine.Color backgroundColor) => AddHeader(mod, HeaderTitle, backgroundColor, new Color32(236, 229, 2, 255));

        /// <summary>
        /// Add Header, blue title bar that can be used to separate settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        /// <param name="textColor">Text Color of header</param>
        public static void AddHeader(Mod mod, string HeaderTitle, UnityEngine.Color backgroundColor, UnityEngine.Color textColor)
        {
            Settings setting = new Settings(null, HeaderTitle, null)
            {
                Mod = mod,
                Vals = new object[3] { HeaderTitle, backgroundColor, textColor },
                SettingType = SettingsType.Header
            };
            mod.modSettingsList.Add(setting);
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
        /// Get value of setting.
        /// </summary>
        /// <returns><see cref="Value"/> of setting</returns>
        /// <example><code source="SettingsExamples.cs" region="GetValue" lang="C#" 
        /// title="Get Value" /></example>
        public object GetValue() => Value; //Return whatever is there

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

    public class SettingsCheckBox
    {
        public Settings Instance;

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
        public void SetValue(bool value)
        {
            Instance.Value = value;
        }
        internal SettingsCheckBox(Settings setting)
        {
            Instance = setting;
        }
    }
    public class SettingsCheckBoxGroup
    {
       public Settings Instance;

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
        public void SetValue(bool value)
        {
            Instance.Value = value;
        }
        internal SettingsCheckBoxGroup(Settings setting)
        {
            Instance = setting;
        }
    }
    public class SettingsSliderInt
    {
        public Settings Instance;

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

    public class SettingsSlider
    {
        public Settings Instance;

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

    public class SettingsTextBox
    {
        public Settings Instance;

        public string GetValue()
        {
            return Instance.GetValue().ToString();
        }

        public void SetValue(string value)
        {
            Instance.Value = value;
        }
        internal SettingsTextBox(Settings s)
        {
            Instance = s;
        }
    }
}
