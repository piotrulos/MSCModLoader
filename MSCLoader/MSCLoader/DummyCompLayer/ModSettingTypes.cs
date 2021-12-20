using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MSCLoader;

//[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

/* public class ModSetting : MonoBehaviour
 {

 }*/
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[System.Obsolete("Useless", true)]
public class SettingButton : MonoBehaviour
{
    Settings setting;
    public bool Enabled { get; set; }
    public string ID { get => setting.ID; set => setting.ID = value; }
    public string Name { get => setting.Vals[0].ToString(); set => setting.Vals[0] = value; }
    public string ButtonText { get => setting.Name; set => setting.Name = value; }
    public void AddAction(UnityAction action, bool ignoreSuspendActions = false)
    {
        setting.DoUnityAction += action;
    }
    public void SettingButtonC(Settings set)
    {
        setting = set;
        nameText = set.NameText;
    }
    public bool suspendActions = false;
    public Button.ButtonClickedEvent OnClick { get => button.onClick; set => button.onClick = value; }
    public Text nameText;
    public Shadow nameShadow;

    public Button button;
    public Image buttonImage;
    public Text buttonText;
    public Shadow buttonTextShadow;
}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

[System.Obsolete("Useless", true)]
public class SettingHeader : MonoBehaviour
{
    Settings setting;

    public bool Enabled { get; set; }

    public float Height { get; set; }

    public Color BackgroundColor { get => (Color)setting.Vals[1]; set => setting.Vals[1] = value; }
    /// <summary>The Outline color for the header.</summary>
    public Color OutlineColor { get => Color.white; set => _ = value; }
    /// <summary>Text displayed on the header.</summary>
    public string Text { get => setting.Name; set => setting.Name = value; }
    public void SettingHeaderC(Settings set)
    {
        setting = set;
    }
    public LayoutElement layoutElement;
    public Image background;

    public Text text;
    public Shadow textShadow;
}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

[System.Obsolete("Useless", true)]
public class SettingKeybind : MonoBehaviour
{
    Keybind keyb;
    public bool GetKey() => keyb.GetKeybind();
    public bool GetKeyDown() => keyb.GetKeybindDown();
    public bool GetKeyUp() => keyb.GetKeybindUp();
    public UnityEvent OnKeyDown = new UnityEvent();
    public UnityEvent OnKey = new UnityEvent();
    public UnityEvent OnKeyUp = new UnityEvent();
    public void SettingKeybindC(Keybind key)
    {
        keyb = key;
    }
}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

[System.Obsolete("==> SettigsCheckBoxGroup", true)]
public class SettingRadioButtons : MonoBehaviour
{
    public class RadioEvent : UnityEvent<int> { }

    internal int radioValue;
    public List<RadioButton> buttons = new List<RadioButton>();
    Settings[] settings;
    public int Value
    {
        get
        {
            for (int i = 0; i < settings.Length; i++)
            {
                if ((bool)settings[i].GetValue())
                {
                    return i;
                }
            }
            return 0;
        }
        set
        {
            for (int i = 0; i < settings.Length; i++)
            {
                settings[i].Value = false;
            }
            settings[value].Value = true;

        }
    }

    public void SettingRadioButtonsC(Settings[] set)
    {
        settings = set;
    }
}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

[System.Obsolete("==> SettigsCheckBoxGroup", true)]
public class RadioButton : MonoBehaviour
{
    public SettingRadioButtons settingRadioButtons;
    public int radioID = 0;

    public Toggle toggle;
    public Image offImage, onImage;

    public Text labelText;
    public Shadow labelShadow;

    public bool suspendSetValue = false;
    public void SetSettingValue()
    {
    }
}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

[System.Obsolete("==> SettigsSlider", true)]
public class SettingSlider : MonoBehaviour
{
    Settings setting;
    public bool Enabled { get; set; }
    public string ID { get => setting.ID; set => setting.ID = value; }
    public string Name { get => setting.Name; set => setting.Name = value; }
    public float Value { get => float.Parse(setting.Value.ToString()); set => setting.Value = value; }
    public int ValueInt { get => int.Parse(setting.Value.ToString()); set => setting.Value = value; }

    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public bool WholeNumbers { get; set; }
    public int RoundDigits { get; set; }
    public Slider.SliderEvent OnValueChanged { get; set; }
    public string[] TextValues
    {
        get;
        set;
    }
    public string ValuePrefix
    {
        get;
        set;
    }
    public string ValueSuffix
    {
        get;
        set;
    }
    public void SettingSliderC(Settings set)
    {
        setting = set;
    }

    public Text nameText;
    public Shadow nameShadow;

    public Text valueText;
    public Shadow valueShadow;

    public Slider slider;
    public Image backgroundImage, handleImage;
}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

[System.Obsolete("Useless", true)]
public class SettingSpacer : MonoBehaviour
{
    public LayoutElement layoutElement;
    public bool Enabled { get; set; }
    public float Height { get; set; }
}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

[System.Obsolete("Useless", true)]
public class SettingText : MonoBehaviour
{
    public Text text;
    public Shadow textShadow;

    public Image background;
    public bool Enabled { get; set; }
    public string Text { get => text.text; set => text.text = value; }
    public Color TextColor { get => text.color; set => text.color = value; }
    public Color BackgroundColor { get => background.color; set => background.color = value; }
    public void SettingTextC()
    {
        text = new GameObject("z").AddComponent<Text>();
    }
}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[System.Obsolete("==> SettigsTextBox", true)]
public class SettingTextBox : MonoBehaviour
{
    Settings setting;
    public Text nameText;
    public Shadow nameShadow;
    public InputField inputField;
    public Image inputImage;
    public Text inputPlaceholderText;
    public bool Enabled { get; set; }
    public string ID { get => setting.ID; set => setting.ID = value; }
    public string Name { get => setting.Name; set => setting.Name = value; }
    public string Value { get => setting.Value.ToString(); set => setting.Value = value; }
    public string Placeholder { get => setting.Vals[0].ToString(); set => setting.Vals[0] = value; }
    public string defaultValue;
    public void SettingTextBoxC(Settings set)
    {
        setting = set;
    }

}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

[System.Obsolete("==> SettigsCheckBox", true)]
public class SettingToggle : MonoBehaviour
{
    public Text nameText;
    public Shadow nameShadow;

    public Toggle toggle;
    public Image offImage, onImage;

    public bool Enabled { get; set; }
    public string ID { get => setting.ID; set => setting.ID = value; }
    public string Name { get => setting.Name; set => setting.Name = value; }
    public bool Value { get => bool.Parse(setting.Value.ToString()); set => setting.Value = value; }
    public Toggle.ToggleEvent OnValueChanged { get => toggle.onValueChanged; }
    Settings setting;
    public void SettingToggleC(Settings set)
    {
        setting = set;
    }


}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

[System.Obsolete("Useless", true)]
public class SettingBoolean : MonoBehaviour
{
    public string ID { get; set; }
    public bool Value;
    public SettingBoolean()
    {

    }
}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[System.Obsolete("Useless", true)]
public class SettingNumber : MonoBehaviour
{
    public string ID { get; set; }
    public float Value;
    public int ValueInt { get => (int)Value; set => Value = value; }
    public SettingNumber()
    {

    }
}
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[System.Obsolete("Useless", true)]
public class SettingString : MonoBehaviour
{
    public string ID { get; set; }
    public string Value;
    public SettingString()
    {

    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
