using UnityEngine.UI;

namespace MSCLoader
{
    internal class SettingsElement : MonoBehaviour
    {
        public Text settingName;

        public Toggle checkBox;

        public Button button;
        public RawImage iconElement;

        public Slider slider;
        public Text value;

        public InputField textBox;
        public Text placeholder;

        public Texture2D[] iconPack;

        //Extensions
        public DropDownList dropDownList;
        public ColorPicker colorPicker;

        internal void SetupCheckbox(string name, bool value, ToggleGroup toggleGroup)
        {
            settingName.text = name;
            checkBox.isOn = value;
            if (toggleGroup != null)
                checkBox.group = toggleGroup;
        }

        internal void SetupButton(string name, Color textColor, Color btnColor)
        {
            settingName.text = name;
            settingName.color = textColor;
            button.GetComponent<Image>().color = btnColor;
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
    }
}