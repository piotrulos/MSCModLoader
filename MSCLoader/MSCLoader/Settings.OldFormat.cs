#if !Mini
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MSCLoader
{
    public partial class Settings
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
        [Obsolete("No longer used", true)]
        public string Name { get => settingName; set { settingName = value; UpdateName(); } }

        /// <summary>
        /// The Mod this Setting belongs to (This is set when using Add whatever).
        /// </summary>
        public Mod Mod;

        /// <summary>
        /// Default Value for setting.
        /// </summary>
        [Obsolete("No longer used", true)]
        public object Value { get { GetValue(); return valueName; } set { valueName = value; UpdateValue(); } }

        /// <summary>
        /// Action to execute for specifed setting.
        /// </summary>
        public Action DoAction;

        /// <summary>
        /// Type of setting.
        /// </summary>
        internal SettingsType SettingType;

        /// <summary>
        /// Helpful additional variables.
        /// </summary>
        public object[] Vals;

        //internal Text NameText;
        //internal Text ValueText;        
        internal SettingsElement SettingsElement;
        internal SettingsGroup header;
        [Obsolete("No longer used")]
        void UpdateName()
        {
            if (SettingsElement == null) return;
            if (SettingsElement.settingName != null)
            {
                SettingsElement.settingName.text = Name;
            }
        }
        [Obsolete]
        void UpdateValue()
        {
            if (SettingsElement == null) return;
            if (SettingsElement.value != null)
            {
                switch (SettingType)
                {
                    case SettingsType.TextBox:
                        SettingsElement.textBox.text = Value.ToString();
                        break;
                    case SettingsType.DropDown:
                        SettingsElement.dropDownList.SelectedIndex = int.Parse(Value.ToString());
                        break;
                    default:
                        SettingsElement.value.text = Value.ToString();
                        break;
                }
            }
        }



        /// <summary>
        /// Constructor for Settings
        /// </summary>
        /// <param name="id">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="value">Default Value for this setting</param>        
        [Obsolete("No longer used", true)]
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
        [Obsolete("No longer used", true)]
        public Settings(string id, string name, Action doAction)
        {
            ID = id;
            Name = name;
            Value = "DoAction";
            DoAction = doAction;
        }

        /// <summary>
        /// Constructor for Settings
        /// </summary>
        /// <param name="id">Unique settings ID for your mod</param>
        /// <param name="name">Name of the setting</param>
        /// <param name="value">Default Value for this setting</param>
        /// <param name="doAction">Function to execute for this setting</param>
        [Obsolete("No longer used", true)]
        public Settings(string id, string name, object value, Action doAction)
        {
            ID = id;
            Name = name;
            Value = value;
            DoAction = doAction;
        }

        [Obsolete]
        internal Settings(Mod mod, string id, string name, object value, Action doAction, SettingsType type)
        {
            Mod = mod;
            ID = id;
            Name = name;
            Value = value;
            DoAction = doAction;
            SettingType = type;
        }

        internal Settings(ModSetting set)
        {
            modSetting = set;
        }
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsCheckBox AddCheckBox(Mod mod, string settingID, string name, bool value = false, Action onValueChanged = null)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            return AddCheckBox(settingID, name, value, onValueChanged);
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsCheckBoxGroup AddCheckBoxGroup(Mod mod, string settingID, string name, bool value = false, string group = null, Action onValueChanged = null)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            return AddCheckBoxGroup(settingID, name, value, group, onValueChanged);
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsSliderInt AddSlider(Mod mod, string settingID, string name, int minValue, int maxValue, int value = 0, Action onValueChanged = null, string[] textValues = null)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            return AddSlider(settingID, name, minValue, maxValue, value, onValueChanged, textValues);
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsSlider AddSlider(Mod mod, string settingID, string name, float minValue, float maxValue, float value = 0f, Action onValueChanged = null, int decimalPoints = 2)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            return AddSlider(settingID, name, minValue, maxValue, value, onValueChanged, decimalPoints);
        }

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="settingID">Your unique settings ID</param>
        /// <param name="name">Name of text box</param>
        /// <param name="value">Default TextBox value</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsTextBox AddTextBox(Mod mod, string settingID, string name, string value, string placeholderText, InputField.ContentType contentType)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            return AddTextBox(settingID, name, value, placeholderText, contentType);
        }

        /// <summary>
        /// Add DropDown List
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="settingID">unique settings ID</param>
        /// <param name="name">Name of the dropdown list</param>
        /// <param name="arrayOfItems">array of items that will be displayed in list</param>
        /// <param name="defaultSelected">default selected Index ID (default 0)</param>
        /// <param name="OnSelectionChanged">Action when item is selected</param>
        /// <returns>SettingsDropDownList</returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsDropDownList AddDropDownList(Mod mod, string settingID, string name, string[] arrayOfItems, int defaultSelected = 0, Action OnSelectionChanged = null)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            return AddDropDownList(settingID, name, arrayOfItems, defaultSelected, OnSelectionChanged);
        }
        /// <summary>
        /// Add Color Picker with RGB sliders
        /// </summary>
        /// <param name="mod">Your mod ID</param>
        /// <param name="settingID">unique settings ID</param>
        /// <param name="name">Title of color picker</param>
        /// <param name="OnColorChanged">Action on color changed</param>
        /// <returns>SettingsColorPicker</returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsColorPicker AddColorPickerRGB(Mod mod, string settingID, string name, Action OnColorChanged = null) => AddColorPickerRGBInternal(mod, settingID, name, new Color32(0, 0, 0, 255), OnColorChanged, false);
        /// <summary>
        /// Add Color Picker with RGBA sliders
        /// </summary>
        /// <param name="mod">Your mod ID</param>
        /// <param name="settingID">unique settings ID</param>
        /// <param name="name">Title of color picker</param>
        /// <param name="OnColorChanged">Action on color changed</param>
        /// <returns>SettingsColorPicker</returns>  
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsColorPicker AddColorPickerRGBA(Mod mod, string settingID, string name, Action OnColorChanged = null) => AddColorPickerRGBInternal(mod, settingID, name, new Color32(0, 0, 0, 255), OnColorChanged, true);
        /// <summary>
        /// Add Color Picker with RGB sliders
        /// </summary>
        /// <param name="mod">Your mod ID</param>
        /// <param name="settingID">unique settings ID</param>
        /// <param name="name">Title of color picker</param>
        /// <param name="defaultColor">Default selected color</param>
        /// <param name="OnColorChanged">Action on color changed</param>
        /// <returns>SettingsColorPicker</returns>      
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsColorPicker AddColorPickerRGB(Mod mod, string settingID, string name, Color32 defaultColor, Action OnColorChanged = null) => AddColorPickerRGBInternal(mod, settingID, name, defaultColor, OnColorChanged, false);
        /// <summary>
        /// Add Color Picker with RGBA sliders
        /// </summary>
        /// <param name="mod">Your mod ID</param>
        /// <param name="settingID">unique settings ID</param>
        /// <param name="name">Title of color picker</param>
        /// <param name="defaultColor">Default selected color</param>
        /// <param name="OnColorChanged">Action on color changed</param>
        /// <returns>SettingsColorPicker</returns>    
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsColorPicker AddColorPickerRGBA(Mod mod, string settingID, string name, Color32 defaultColor, Action OnColorChanged = null) => AddColorPickerRGBInternal(mod, settingID, name, defaultColor, OnColorChanged, true);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal static SettingsColorPicker AddColorPickerRGBInternal(Mod mod, string settingID, string name, Color32 defaultColor, Action OnColorChanged, bool showAlphaSlider)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            return AddColorPickerRGBAInternal(settingID, name, defaultColor, OnColorChanged, showAlphaSlider);
        }
        /// <summary>
        /// Add button that can execute function.
        /// </summary>
        /// <param name="mod">your mod</param>
        /// <param name="name">Text on the button</param>
        /// <param name="onClick">What to do when button is clicked</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddButton(Mod mod, string name, Action onClick) => AddButton(mod, $"{mod.ID}_btn", name, onClick, new Color32(85, 38, 0, 255), Color.white);

        /// <summary>
        /// Add button that can execute function.
        /// </summary>
        /// <param name="mod">your mod</param>
        /// <param name="name">Text on the button</param>
        /// <param name="onClick">What to do when button is clicked</param>
        /// <param name="btnColor">Button background color</param>
        /// <param name="buttonTextColor">Button text color</param>
        /// 
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddButton(Mod mod, string name, Action onClick, Color btnColor, Color buttonTextColor) => AddButton(mod, $"{mod.ID}_btn", name, onClick, btnColor, buttonTextColor);

        /// <summary>
        /// Add button that can execute function.
        /// </summary>
        /// <param name="mod">your mod</param>
        /// <param name="settingID">unique settings ID</param>
        /// <param name="name">Text on the button</param>
        /// <param name="onClick">What to do when button is clicked</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddButton(Mod mod, string settingID, string name, Action onClick, Color btnColor, Color buttonTextColor)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            AddButton(name, onClick, btnColor, buttonTextColor);
        }

        /// <summary>
        /// Add custom reset to default button
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="name">Button name</param>
        /// <param name="sets">array of settings to reset</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Obsolete("Use other overload instead", true)]
        public static void AddResetButton(Mod mod, string name, Settings[] sets)
        {
            //Backward compatibility
            if (sets == null) return;
            List<ModSetting> list = new List<ModSetting>();
            for (int i = 0; i < sets.Length; i++)
            {
                list.Add(sets[i].modSetting);
            }
            AddResetButton(name, list.ToArray());
        }


        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddHeader(Mod mod, string HeaderTitle) => AddHeader(mod, HeaderTitle, false);

        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddHeader(Mod mod, string HeaderTitle, bool collapsedByDefault = false) => AddHeader(mod, HeaderTitle, new Color32(95, 34, 18, 255), new Color32(236, 229, 2, 255), collapsedByDefault);

        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor) => AddHeader(mod, HeaderTitle, backgroundColor, false);

        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor, bool collapsedByDefault = false) => AddHeader(mod, HeaderTitle, backgroundColor, new Color32(236, 229, 2, 255), collapsedByDefault);

        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        /// <param name="textColor">Text Color of header</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor, Color textColor) => AddHeader(mod, HeaderTitle, backgroundColor, textColor, false);

        /// <summary>
        /// Add Header, header groups settings together
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        /// <param name="textColor">Text Color of header</param>      
        /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor, Color textColor, bool collapsedByDefault = false)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            AddHeader(HeaderTitle, backgroundColor, textColor, collapsedByDefault);
        }

        /// <summary>
        /// Add dynamic Header, same as AddHeader but returns value, you can collapse/expand/change color of it from other settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="collapsedByDefault">Header collapsed by default (optional default=false)</param>
        /// <returns>SettingsDynamicHeader</returns>
        [Obsolete("Moved to => AddHeader()", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsDynamicHeader AddDynamicHeader(Mod mod, string HeaderTitle, bool collapsedByDefault = false)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            SettingsDynamicHeader s = new SettingsDynamicHeader(HeaderTitle, new Color32(95, 34, 18, 255), new Color32(236, 229, 2, 255), collapsedByDefault);
            settingsMod.modSettingsList.Add(s);
            return s;
        }

        /// <summary>
        /// Add just a text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="text">Just a text (supports unity rich text)</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void AddText(Mod mod, string text)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            AddText(text);
        }

        /// <summary>
        /// Add dynamic text (it is not saved)
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="text">Just a text (supports unity rich text)</param>
        /// <returns>SettingsDynamicText</returns>
        [Obsolete("Moved to => AddText()", true)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static SettingsDynamicText AddDynamicText(Mod mod, string text)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            SettingsDynamicText s = new SettingsDynamicText(text);
            settingsMod.modSettingsList.Add(s);
            return s;
        }
    }
}
#endif