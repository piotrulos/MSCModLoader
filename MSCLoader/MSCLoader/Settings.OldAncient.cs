using UnityEngine.UI;

namespace MSCLoader
{
    public partial class Settings
    {
        //List of old obsolete Settings functions (used only as backwards compatibility)

        /// <summary>
        /// Get value of setting.
        /// </summary>
        /// <returns>Raw value of setting</returns>
        public object GetValue() => Value; //Return whatever is there

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
        [System.Obsolete("Please switch to new settings format.", true)]
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
            setting.Mod = mod;
            mod.modSettingsDefault.Add(new Settings(setting.ID, setting.Name, setting.Value) { Mod = mod });

            setting.Vals = new object[3];
            setting.SettingType = SettingsType.TextBox;
            setting.Vals[0] = placeholderText;
            setting.Vals[1] = titleTextColor;
            setting.Vals[2] = contentType;
            mod.modSettingsList.Add(setting);
        }

        //internal shit for pro compatibility
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
}
