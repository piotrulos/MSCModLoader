#if !Mini
using UnityEngine.UI;

namespace MSCLoader
{
    public partial class Settings
    {
        //List of Ancient hard obsoleted Settings functions (used only as backwards compatibility)

        internal ModSetting modSetting;

        /// <summary>
        /// Get value of setting.
        /// </summary>
        /// <returns>Raw value of setting</returns>
        [System.Obsolete("Please switch to new settings format.", true)]
        public object GetValue()
        {
            if (modSetting == null) return string.Empty;
            //Legacy Backwards compatibility with V3 settings
            switch (modSetting.SettingType)
            {
                case SettingsType.CheckBoxGroup:
                    valueName = ((SettingsCheckBoxGroup)modSetting).GetValue();
                    return ((SettingsCheckBoxGroup)modSetting).GetValue();
                case SettingsType.CheckBox:
                    valueName = ((SettingsCheckBox)modSetting).GetValue();
                    return ((SettingsCheckBox)modSetting).GetValue();
                case SettingsType.Slider:
                    valueName = ((SettingsSlider)modSetting).GetValue();
                    return ((SettingsSlider)modSetting).GetValue();
                case SettingsType.SliderInt:
                    valueName = ((SettingsSliderInt)modSetting).GetValue();
                    return ((SettingsSliderInt)modSetting).GetValue();
                case SettingsType.TextBox:
                    valueName = ((SettingsTextBox)modSetting).GetValue();
                    return ((SettingsTextBox)modSetting).GetValue();
                case SettingsType.Header:
                case SettingsType.Button:
                case SettingsType.RButton:
                case SettingsType.Text:
                case SettingsType.DropDown:
                    valueName = ((SettingsDropDownList)modSetting).GetSelectedItemIndex();
                    return ((SettingsDropDownList)modSetting).GetSelectedItemIndex();
                case SettingsType.ColorPicker:
                    valueName = ((SettingsColorPicker)modSetting).GetValue();
                    return ((SettingsColorPicker)modSetting).GetValue();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Add checkbox to settings menu (only bool Value accepted)
        /// Can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("Please switch to new settings format.", true)]
        public static void AddCheckBox(Mod mod, Settings setting)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            SettingsCheckBox s = AddCheckBox(setting.ID, setting.Name, bool.Parse(setting.Value.ToString()), setting.DoAction);
            setting.modSetting = s;
        }
        /// <summary>
        /// Add checkbox to settings menu with group, only one checkbox can be set to true in this group
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="group">Unique group name, same for all checkboxes that will be grouped</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("Please switch to new settings format.", true)]
        public static void AddCheckBox(Mod mod, Settings setting, string group)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            SettingsCheckBoxGroup s = AddCheckBoxGroup(setting.ID, setting.Name, bool.Parse(setting.Value.ToString()), group, setting.DoAction);
            setting.modSetting = s;
        }

        /// <summary>
        /// Add button that can execute function.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="description">Short optional description for this button</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("Please switch to new settings format.", true)]
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
        [System.Obsolete("Please switch to new settings format.", true)]
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
        [System.Obsolete("Please switch to new settings format.", true)]
        public static void AddButton(Mod mod, Settings setting, UnityEngine.Color normalColor, UnityEngine.Color highlightedColor, UnityEngine.Color pressedColor, UnityEngine.Color buttonTextColor, string description = null)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            AddButton(setting.Name, setting.DoAction, normalColor, buttonTextColor);
        }

        /// <summary>
        /// Add Slider, slider can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="maxValue">Max value of slider</param>
        /// <param name="minValue">Min value of slider</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("Please switch to new settings format.", true)]
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
        [System.Obsolete("Please switch to new settings format.", true)]
        public static void AddSlider(Mod mod, Settings setting, int minValue, int maxValue, string[] textValues)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            SettingsSliderInt s = AddSlider(setting.ID, setting.Name, minValue, maxValue, int.Parse(setting.Value.ToString()), setting.DoAction, textValues);
            setting.modSetting = s;
        }

        /// <summary>
        /// Add Slider, slider can execute action when its value is changed.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="maxValue">Max value of slider</param>
        /// <param name="minValue">Min value of slider</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("Please switch to new settings format.", true)]
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
        [System.Obsolete("Please switch to new settings format.", true)]
        public static void AddSlider(Mod mod, Settings setting, float minValue, float maxValue, int decimalPoints)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            SettingsSlider s = AddSlider(setting.ID, setting.Name, minValue, maxValue, float.Parse(setting.Value.ToString()), setting.DoAction, decimalPoints);
            setting.modSetting = s;
        }

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("Please switch to new settings format.", true)]
        public static void AddTextBox(Mod mod, Settings setting, string placeholderText) => AddTextBox(mod, setting, placeholderText, UnityEngine.Color.white);

        /// <summary>
        /// Add TextBox where user can type any text
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="setting">Your settings variable</param>
        /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
        /// <param name="titleTextColor">Text color of title</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("Please switch to new settings format.", true)]
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
        [System.Obsolete("Please switch to new settings format.", true)]
        public static void AddTextBox(Mod mod, Settings setting, string placeholderText, UnityEngine.Color titleTextColor, InputField.ContentType contentType)
        {
            settingsMod = mod; //Just for backward compatibility (if settings were made outside ModSettings function)
            SettingsTextBox s = AddTextBox(setting.ID, setting.Name, setting.Value.ToString(), placeholderText, contentType);
            setting.modSetting = s;
        }
    }
}
#endif