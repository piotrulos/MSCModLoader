using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#pragma warning disable CS1591

//ModSettings pseudo redirect from pro (just for avoid exceptions)
namespace MSCLoader
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [System.Obsolete("=> Settings")]
    public class ModSettings
    {
        Mod mod;
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ModSettings(Mod mod)
        {
            this.mod = mod;
        }
        [System.Obsolete("=> Settings.AddButton", true)]

        public SettingButton AddButton(string id, string buttonText, UnityAction action = null, bool blockSuspension = false) => AddButton(id, buttonText, "", action, blockSuspension);
        [System.Obsolete("=> Settings.AddButton", true)]
        public SettingButton AddButton(string id, string buttonText, string name = "", UnityAction action = null, bool blockSuspension = false)
        {
            Settings set = new Settings(id, buttonText, action, blockSuspension);
            set.SettingType = SettingsType.Button;
            Settings.AddButton(mod, set, name);
            mod.proSettings = true;
            return new SettingButton(set);
        }
        [System.Obsolete("=> Settings.AddHeader", true)]
        public SettingHeader AddHeader(string text) => AddHeader(text, new Color32(0, 128, 0, 255));
        [System.Obsolete("=> Settings.AddHeader", true)]

        public SettingHeader AddHeader(string text, Color backgroundColor) => AddHeader(text, backgroundColor, Color.white);
        [System.Obsolete("=> Settings.AddHeader", true)]

        public SettingHeader AddHeader(string text, Color backgroundColor, Color textColor) => AddHeader(text, backgroundColor, textColor, Color.white);
        [System.Obsolete("=> Settings.AddHeader", true)]

        public SettingHeader AddHeader(string text, Color backgroundColor, Color textColor, Color outlineColor)
        {
            Settings set = new Settings(null, text, null);
            set.SettingType = SettingsType.Header;
            mod.proSettings = true;
            Settings.AddHeader(mod, text, backgroundColor, textColor, set);
            return new SettingHeader(set);
        }
        [System.Obsolete("=> Keybind.Add", true)]
        public SettingKeybind AddKeybind(string id, string name, KeyCode key, params KeyCode[] modifiers)
        {
            Keybind keyb;
            if (modifiers.Length > 0)
                keyb = Keybind.Add(mod, id, name, key, modifiers[0]);
            else
                keyb = new Keybind(id, name, key, KeyCode.None);
            return new SettingKeybind(keyb);
        }
        [System.Obsolete("=> Settings.AddCheckBox (group)", true)]
        public SettingRadioButtons AddRadioButtons(string id, string name, int value, params string[] options) => AddRadioButtons(id,name,value,(UnityAction)null, options);
        [System.Obsolete("=> Settings.AddCheckBox (group)", true)]
        public SettingRadioButtons AddRadioButtons(string id, string name, int value, UnityAction<int> action, params string[] options)
        {
            Settings.AddText(mod, $"<color=orange>Incompatible setting - <color=aqua><b>{id}</b></color></color>");
            Settings[] set = new Settings[options.Length];
            return new SettingRadioButtons(set);
        }
        [System.Obsolete("=> Settings.AddCheckBox (group)", true)]
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

        [System.Obsolete("=> Settings.AddSlider", true)]
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, int roundDigits = 2) => AddSlider(id, name, value, minValue, maxValue, roundDigits, (UnityAction)null);
        [System.Obsolete("=> Settings.AddSlider", true)]
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, int roundDigits = 2, UnityAction<float> action = null)
        {
            Settings.AddText(mod, $"<color=orange>Incompatible setting - <color=aqua><b>{id}</b></color></color>");
            return new SettingSlider(new Settings(id, name, value));
        }
        [System.Obsolete("=> Settings.AddSlider", true)]
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, int roundDigits = 2, UnityAction action = null)
        {
            mod.proSettings = true;
            Settings set = new Settings(id, name, value);
            Settings.AddSlider(mod, set, minValue, maxValue, roundDigits);
            return new SettingSlider(set);
        }

        [System.Obsolete("=> Settings.AddSlider", true)]
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, UnityAction<float> action = null)
        {
            Settings.AddText(mod, $"<color=orange>Incompatible setting - <color=aqua><b>{id}</b></color></color>");
            return new SettingSlider(new Settings(id, name, value));
        }
        [System.Obsolete("=> Settings.AddSlider", true)]
        public SettingSlider AddSlider(string id, string name, float value, float minValue, float maxValue, UnityAction action = null)
        {
            mod.proSettings = true;
            Settings set = new Settings(id, name, value);
            Settings.AddSlider(mod, set, minValue, maxValue);
            return new SettingSlider(set);
        }
        [System.Obsolete("=> Settings.AddSlider", true)]
        public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue) => AddSlider(id, name, value, minValue, maxValue, (UnityAction)null);
        [System.Obsolete("=> Settings.AddSlider", true)]
        public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue, UnityAction<float> action)
        {
            Settings.AddText(mod, $"<color=orange>Incompatible setting - <color=aqua><b>{id}</b></color></color>");
            return new SettingSlider(new Settings(id, name, value));
        }
        [System.Obsolete("=> Settings.AddSlider", true)]
        public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue, UnityAction action)
        {
            mod.proSettings = true;
            Settings set = new Settings(id, name, value);

            Settings.AddSlider(mod, set, minValue, maxValue);
            return new SettingSlider(set);
        }

        [System.Obsolete("Does nothing", true)]
        public SettingSpacer AddSpacer(float height)
        {
            Settings.AddText(mod, $"<color=orange>Incompatible setting - <color=aqua><b>AddSpacer</b></color></color>");
            return new SettingSpacer();
        }
        [System.Obsolete("=> Settings.AddText", true)]
        public SettingText AddText(string text, Color backgroundColor) => AddText(text);
        [System.Obsolete("=> Settings.AddText", true)]
        public SettingText AddText(string text, Color backgroundColor, Color textColor) => AddText(text);
        [System.Obsolete("=> Settings.AddText", true)]
        public SettingText AddText(string text, Color backgroundColor, Color textColor, Color outlineColor) => AddText(text);
        [System.Obsolete("=> Settings.AddText", true)]
        public SettingText AddText(string text)
        {
            mod.proSettings = true;
            Settings.AddText(mod, text);
            return new SettingText();
        }

        [System.Obsolete("=> Settings.AddTextBox", true)]
        public SettingTextBox AddTextBox(string id, string name, string value, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None)
        {
            Settings.AddText(mod, $"<color=orange>Incompatible setting - <color=aqua><b>{id}</b></color></color>");
            return new SettingTextBox(new Settings(id, name, value));
        }
        [System.Obsolete("=> Settings.AddTextBox", true)]
        public SettingTextBox AddTextBox(string id, string name, string value, UnityAction<string> action, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None)
        {
            Settings.AddText(mod, $"<color=orange>Incompatible setting - <color=aqua><b>{id}</b></color></color>");
            return new SettingTextBox(new Settings(id, name, value));
        }
        [System.Obsolete("=> Settings.AddTextBox", true)]
        public SettingTextBox AddTextBox(string id, string name, string value, UnityAction action, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None) 
        {
            mod.proSettings = true;
            Settings set = new Settings(id, name, value, action, false);
            Settings.AddTextBox(mod, set, placeholder);
            return new SettingTextBox(set);
        }
        [System.Obsolete("=> Settings.AddCheckBox", true)]
        public SettingToggle AddToggle(string id, string name, bool value) => AddToggle(id,name,value,(UnityAction)null);
        [System.Obsolete("=> Settings.AddCheckBox", true)]
        public SettingToggle AddToggle(string id, string name, bool value, UnityAction<bool> action)
        {
            Settings.AddText(mod, $"<color=orange>Incompatible setting - <color=aqua><b>{id}</b></color></color>");
            return new SettingToggle(new Settings(id, name, value));
        }
        [System.Obsolete("=> Settings.AddCheckBox", true)]
        public SettingToggle AddToggle(string id, string name, bool value, UnityAction action)
        {
            mod.proSettings = true;
            Settings set = new Settings(id, name, value, action, false);
            Settings.AddCheckBox(mod, set);
            return new SettingToggle(set);
        }
        [System.Obsolete("Useless", true)]
        public SettingBoolean AddBoolean(string id, bool value) => new SettingBoolean();
        [System.Obsolete("Useless", true)]
        public SettingNumber AddNumber(string id, float value) => new SettingNumber();
        [System.Obsolete("Useless", true)]
        public SettingNumber AddNumber(string id, int value) => new SettingNumber();

        [System.Obsolete("Useless", true)]
        public SettingString AddString(string id, string value) => new SettingString();
    }
}
#pragma warning restore CS1591 
