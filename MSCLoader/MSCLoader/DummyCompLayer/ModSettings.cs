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
            Settings.AddButton(mod, new Settings(id, buttonText, action));
            return null;
        }
        public SettingHeader AddHeader(string text) => AddHeader(text, Color.green, Color.white);

        public SettingHeader AddHeader(string text, Color backgroundColor) => AddHeader(text, backgroundColor, Color.white);

        public SettingHeader AddHeader(string text, Color backgroundColor, Color textColor) => AddHeader(text, backgroundColor, textColor);

        public SettingHeader AddHeader(string text, Color backgroundColor, Color textColor, Color outlineColor)
        {
            Settings.AddHeader(mod, text, backgroundColor, textColor);
            return null;
        }
        public SettingKeybind AddKeybind(string id, string name, KeyCode key, params KeyCode[] modifiers) => null;
        public SettingRadioButtons AddRadioButtons(string id, string name, int value, params string[] options) => null;
        public SettingRadioButtons AddRadioButtons(string id, string name, int value, UnityAction<int> action, params string[] options) => null;
        public SettingRadioButtons AddRadioButtons(string id, string name, int value, UnityAction action, params string[] options) => null;

        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, int roundDigits = 2) => null;
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, int roundDigits = 2, UnityAction<float> action = null) => null;
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, int roundDigits = 2, UnityAction action = null) => null;

        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, UnityAction<float> action = null) =>AddSlider(id, name, value, minValue, maxValue, 2, action);
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, UnityAction action = null) => AddSlider(id, name, value, minValue, maxValue, 2, action);
        public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue) => null;
        public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue, UnityAction<float> action) => null;
        public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue, UnityAction action) => null;

        public SettingSpacer AddSpacer(float height) => null;
        public SettingText AddText(string text, Color backgroundColor) => AddText(text);
        public SettingText AddText(string text, Color backgroundColor, Color textColor) => AddText(text);
        public SettingText AddText(string text, Color backgroundColor, Color textColor, Color outlineColor) => AddText(text);
        public SettingText AddText(string text)
        {
            Settings.AddText(mod, text);
            return null;
        }

        public SettingTextBox AddTextBox(string id, string name, string value, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None) => null;
        public SettingTextBox AddTextBox(string id, string name, string value, UnityAction<string> action, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None) => null;
        public SettingTextBox AddTextBox(string id, string name, string value, UnityAction action, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None) => null;
        public SettingToggle AddToggle(string id, string name, bool value) => null;
        public SettingToggle AddToggle(string id, string name, bool value, UnityAction<bool> action) => null;
        public SettingToggle AddToggle(string id, string name, bool value, UnityAction action) => null;
        public SettingBoolean AddBoolean(string id, bool value) => null;
        public SettingNumber AddNumber(string id, float value) => null;
        public SettingNumber AddNumber(string id, int value) => null;

        public SettingString AddString(string id, string value) => null;
    }
}
#pragma warning restore CS1591 
