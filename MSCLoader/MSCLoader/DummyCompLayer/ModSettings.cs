#if !Mini
using UnityEngine.Events;
using UnityEngine.UI;
#pragma warning disable CS1591

//ModSettings compatibility for PRO BS mods
namespace MSCLoader;

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
        mod.proSettings = true;
        Settings.AddButton(mod, buttonText, delegate
        {
            if (action != null) action.Invoke();
        });
        GameObject d = new GameObject("zzzDummyProShitIgnoreThat");
        return d.AddComponent<SettingButton>();
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
        mod.proSettings = true;
        Settings.AddHeader(mod, text, backgroundColor, textColor);
        GameObject d = new GameObject("zzzDummyProShitIgnoreThat");
        return d.AddComponent<SettingHeader>();
    }
    [System.Obsolete("=> Keybind.Add", true)]
    public SettingKeybind AddKeybind(string id, string name, KeyCode key, params KeyCode[] modifiers)
    {
        Keybind keyb;
        if (modifiers.Length > 0)
            keyb = Keybind.Add(mod, id, name, key, modifiers[0]);
        else
            keyb = new Keybind(id, name, key, KeyCode.None);
        GameObject d = new GameObject("zzzDummyProShitIgnoreThat");
        d.AddComponent<SettingKeybind>().SettingKeybindC(keyb);
        return d.GetComponent<SettingKeybind>();
    }

    [System.Obsolete("=> Settings.AddSlider", true)]
    public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue) => AddSlider(id, name, value, minValue, maxValue, null);

    [System.Obsolete("=> Settings.AddSlider", true)]
    public SettingSlider AddSlider(string id, string name, int value, int minValue, int maxValue, UnityAction action)
    {
        mod.proSettings = true;

        SettingsSliderInt slider = Settings.AddSlider(mod, id, name, minValue, maxValue, value, delegate
        {
            if (action != null) action.Invoke();
        });
        GameObject d = new GameObject("zzzDummyProShitIgnoreThat");
        d.AddComponent<SettingSlider>().setup2(slider);
        return d.GetComponent<SettingSlider>();
    }

    [System.Obsolete("Does nothing", true)]
    public SettingSpacer AddSpacer(float height)
    {
        Settings.AddText(mod, "---");
        return new SettingSpacer();
    }
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
        return AddTextBox(id, name, value, (UnityAction)null, placeholder, inputType);
    }
    [System.Obsolete("=> Settings.AddTextBox", true)]
    public SettingTextBox AddTextBox(string id, string name, string value, UnityAction<string> action, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None)
    {
        Settings.AddText(mod, $"<color=orange>Incompatible setting action - <color=aqua><b>{id}</b></color></color>");
        return AddTextBox(id, name, value, (UnityAction)null, placeholder, inputType);
    }
    [System.Obsolete("=> Settings.AddTextBox", true)]
    public SettingTextBox AddTextBox(string id, string name, string value, UnityAction action, string placeholder = "ENTER TEXT...", InputField.CharacterValidation inputType = InputField.CharacterValidation.None)
    {
        mod.proSettings = true;
        SettingsTextBox set = Settings.AddTextBox(mod, id, name, value, placeholder);
        GameObject d = new GameObject("zzzDummyProShitIgnoreThat");
        d.AddComponent<SettingTextBox>().SettingTextBoxC(set);
        return d.GetComponent<SettingTextBox>();
    }
    [System.Obsolete("=> Settings.AddCheckBox", true)]
    public SettingToggle AddToggle(string id, string name, bool value) => AddToggle(id, name, value, (UnityAction)null);
    [System.Obsolete("=> Settings.AddCheckBox", true)]
    public SettingToggle AddToggle(string id, string name, bool value, UnityAction<bool> action)
    {
        return AddToggle(id, name, value, delegate ()
        {
            if (action != null) action.Invoke(value);
        });
    }
    [System.Obsolete("=> Settings.AddCheckBox", true)]
    public SettingToggle AddToggle(string id, string name, bool value, UnityAction action)
    {
        mod.proSettings = true;
        SettingsCheckBox set = Settings.AddCheckBox(mod, id, name, value, delegate
        {
            if (action != null) action.Invoke();
        });

        GameObject d = new GameObject("zzzDummyProShitIgnoreThat");
        d.AddComponent<SettingToggle>().SettingToggleC(set);
        return d.GetComponent<SettingToggle>();
    }
}
#pragma warning restore CS1591
#endif