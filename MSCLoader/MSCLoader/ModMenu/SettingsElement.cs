extern alias Unity; //Workaround for TG's BS where he put outline in Assembly-CSharp for no reason.
using UnityEngine.UI;
using Outline = Unity.UnityEngine.UI.Outline;

namespace MSCLoader;

internal class SettingsElement : MonoBehaviour
{
    public Text settingName;
    [Header("LayoutStuff")]
    public LayoutElement layoutElement;
    public Image BackgroundColor;
    public Outline OutlineColor;

    [Header("Checkbox")]
    public Toggle checkBox;

    [Header("Button")]
    public Button button;
    public RawImage iconElement;
    public Texture2D[] iconPack;

    [Header("Slider")]
    public Slider slider;
    public Text value;

    [Header("TextBox")]
    public InputField textBox;
    public Text placeholder;

    [Header("DropDown")]
    public DropDownList dropDownList;
    [Header("Color Picker")]
    public ColorPicker colorPicker;

#if !Mini
    internal void SetupCheckbox(string name, bool value, ToggleGroup toggleGroup)
    {
        settingName.text = name;
        checkBox.isOn = value;
        if (toggleGroup != null)
            checkBox.group = toggleGroup;
    }

    internal void SetupButton(SettingsButton btn)
    {
        settingName.text = btn.Name.ToUpper();
        settingName.color = btn.TextColor;
        button.GetComponent<Image>().color = btn.BackgroundColor;
    }

    internal void SetupButton(string name, Color textColor, Color btnColor)
    {
        settingName.text = name.ToUpper();
        settingName.color = textColor;
        button.GetComponent<Image>().color = btnColor;
    }

    internal void SetupButtonWithIcon(SettingsButton btn)
    {
        settingName.text = btn.Name.ToUpper();
        settingName.color = btn.TextColor;
        button.GetComponent<Image>().color = btn.BackgroundColor;
        button.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
        if (btn.PredefinedIcon == SettingsButton.ButtonIcon.Custom)
        {
            if (btn.CustomIcon == null)
            {
                ModConsole.Error($"Custom icon for Button {btn.Name} is null.");
            }
            else
            {
                iconElement.texture = btn.CustomIcon;
            }
        }
        else
        {
            iconElement.texture = iconPack[(int)btn.PredefinedIcon];
        }
        iconElement.gameObject.SetActive(true);
    }

    internal void SetupButtonWithIcon(string name, Color textColor, Color btnColor, SettingsButton.ButtonIcon icon, Texture2D customIcon = null, bool iconOnRight = false)
    {
        settingName.text = name.ToUpper();
        settingName.color = textColor;
        button.GetComponent<Image>().color = btnColor;
        button.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;

        iconElement.texture = iconPack[(int)icon];
        iconElement.gameObject.SetActive(true);
    }
    internal void SetupSliderInt(string name, int val, int min, int max, string[] textValues)
    {
        value.text = textValues != null ? textValues[val] : val.ToString();
        settingName.text = name;
        slider.wholeNumbers = true;
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = val;
    }

    internal void SetupSlider(string name, float val, float min, float max)
    {
        settingName.text = name;
        value.text = val.ToString();
        slider.wholeNumbers = false;
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = val;
    }

    internal void SetupTextBox(string name, string val, string plholder, InputField.ContentType contentType)
    {
        settingName.text = name;
        settingName.color = Color.white;
        placeholder.text = plholder;
        textBox.contentType = contentType;
        textBox.text = val;
    }

    internal void SetupTextArea(string name, string val, string plholder)
    {
        settingName.text = name;
        settingName.color = Color.white;
        placeholder.text = plholder;
        textBox.text = val;
    }

    internal void StretchElement()
    {
        if(layoutElement == null) return;
        layoutElement.flexibleWidth = 1;
    }
#endif
}
