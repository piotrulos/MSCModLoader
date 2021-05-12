using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#pragma warning disable CS1591

//ModSettings pseudo redirect from pro (just for avoid exceptions)
namespace MSCLoader
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class ModSettings
    {
        Mod mod;
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ModSettings(Mod mod)
        {
            this.mod = mod;
        }
        public SettingButton AddButton(string id, string buttonText, UnityAction action = null, bool blockSuspension = false) => AddButton(id, buttonText, "", action, blockSuspension);
        public SettingButton AddButton(string id, string buttonText, string name = "", UnityAction action = null, bool blockSuspension = false)
        {
            Settings set = new Settings(id, buttonText, action, blockSuspension);
            Settings.AddButton(mod, set, name);
            mod.proSettings = true;
            return new SettingButton(set);
        }
        public SettingHeader AddHeader(string text) => AddHeader(text, new Color32(0, 128, 0, 255));

        public SettingHeader AddHeader(string text, Color backgroundColor) => AddHeader(text, backgroundColor, Color.white);

        public SettingHeader AddHeader(string text, Color backgroundColor, Color textColor) => AddHeader(text, backgroundColor, textColor, Color.white);

        public SettingHeader AddHeader(string text, Color backgroundColor, Color textColor, Color outlineColor)
        {
            Settings set = new Settings(null, text, null);
            mod.proSettings = true;
            Settings.AddHeader(mod, text, backgroundColor, textColor, set);
            return new SettingHeader(set);
        }
        public SettingKeybind AddKeybind(string id, string name, KeyCode key, params KeyCode[] modifiers)
        {
            Keybind keyb = new Keybind(id, name, key, modifiers[0]);
            Keybind.Add(mod, keyb);
            return new SettingKeybind(keyb);
        }
        public SettingRadioButtons AddRadioButtons(string id, string name, int value, params string[] options) => AddRadioButtons(id,name,value,(UnityAction)null, options);
        public SettingRadioButtons AddRadioButtons(string id, string name, int value, UnityAction<int> action, params string[] options) => null;
        public SettingRadioButtons AddRadioButtons(string id, string name, int value, UnityAction action, params string[] options) 
        {
            mod.proSettings = true;
            Settings[] set = new Settings[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                set[i] = new Settings(id, options[i], false, action, false);
                Settings.AddCheckBox(mod, set[i], "radioGroup");
            }
            return new SettingRadioButtons(set);
        }

        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, int roundDigits = 2) => AddSlider(id, name, value, minValue, maxValue, roundDigits, (UnityAction)null);       
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, int roundDigits = 2, UnityAction<float> action = null) => null;
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, int roundDigits = 2, UnityAction action = null)
        {
            mod.proSettings = true;
            Settings set = new Settings(id, name, value);
            Settings.AddSlider(mod, set, minValue, maxValue, roundDigits);
            return new SettingSlider(set);
        }

        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, UnityAction<float> action = null) => null;
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, UnityAction action = null)
        {
            mod.proSettings = true;
            Settings set = new Settings(id, name, value);
            Settings.AddSlider(mod, set, minValue, maxValue);
            return new SettingSlider(set);
        }
        public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue) => AddSlider(id, name, value, minValue, maxValue, (UnityAction)null);
        public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue, UnityAction<float> action) => null;
        public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue, UnityAction action)
        {
            mod.proSettings = true;
            Settings set = new Settings(id, name, value);

            Settings.AddSlider(mod, set, minValue, maxValue);
            return new SettingSlider(set);
        }
      
        public SettingSpacer AddSpacer(float height) => null;
        public SettingText AddText(string text, Color backgroundColor) => AddText(text);
        public SettingText AddText(string text, Color backgroundColor, Color textColor) => AddText(text);
        public SettingText AddText(string text, Color backgroundColor, Color textColor, Color outlineColor) => AddText(text);
        public SettingText AddText(string text)
        {
            mod.proSettings = true;
            Settings.AddText(mod, text);
            return new SettingText();
        }

        public SettingTextBox AddTextBox(string id, string name, string value, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None) => null;
        public SettingTextBox AddTextBox(string id, string name, string value, UnityAction<string> action, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None) => null;
        public SettingTextBox AddTextBox(string id, string name, string value, UnityAction action, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None) 
        {
            mod.proSettings = true;
            Settings set = new Settings(id, name, value, action, false);
            Settings.AddTextBox(mod, set, placeholder);
            return new SettingTextBox(set);
        }
        public SettingToggle AddToggle(string id, string name, bool value) => AddToggle(id,name,value,(UnityAction)null);
        public SettingToggle AddToggle(string id, string name, bool value, UnityAction<bool> action) => null;
        public SettingToggle AddToggle(string id, string name, bool value, UnityAction action)
        {
            mod.proSettings = true;
            Settings.AddCheckBox(mod, new Settings(id, name, value, action, false));
            return new SettingToggle();
        }
        public SettingBoolean AddBoolean(string id, bool value) => null;
        public SettingNumber AddNumber(string id, float value) => null;
        public SettingNumber AddNumber(string id, int value) => null;

        public SettingString AddString(string id, string value) => null;
    }
}
#pragma warning restore CS1591 
