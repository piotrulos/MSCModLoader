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
    }
}