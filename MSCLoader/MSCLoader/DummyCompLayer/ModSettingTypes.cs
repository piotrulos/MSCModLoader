using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MSCLoader
{
    //[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    /* public class ModSetting : MonoBehaviour
     {

     }*/
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class SettingButton
    {
        Settings setting;
        public bool Enabled;
        public string ID { get => setting.ID; set => setting.ID = value; }
        public string Name { get => setting.Vals[0].ToString(); set => setting.Vals[0] = value; }
        public string ButtonText { get => setting.Name; set => setting.Name = value; }
        public void AddAction(UnityAction action, bool ignoreSuspendActions = false)
        {
            setting.DoUnityAction += action;
        }
        public SettingButton(Settings set)
        {
            setting = set;
        }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class SettingHeader 
    {
        Settings setting;

        public bool Enabled;

        public float Height;

        public Color BackgroundColor { get => (Color)setting.Vals[1]; set => setting.Vals[1] = value; }
        /// <summary>The Outline color for the header.</summary>
        public Color OutlineColor { get => Color.white; set => _ = value; }
        /// <summary>Text displayed on the header.</summary>
        public string Text { get => setting.Name; set => setting.Name = value; }
        public SettingHeader(Settings set)
        {
            setting = set;
        }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class SettingKeybind 
    {
        Keybind keyb;
        public bool GetKey() => keyb.GetKeybind();
        public bool GetKeyDown() => keyb.GetKeybindDown();
        public bool GetKeyUp() => keyb.GetKeybindUp();
        public SettingKeybind(Keybind key)
        {
            keyb = key;
        }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class SettingRadioButtons 
    {
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

        public SettingRadioButtons(Settings[] set)
        {
            settings = set;
        }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class RadioButton
    {

    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class SettingSlider 
    {
        Settings setting;
        public bool Enabled;
        public string ID { get => setting.ID; set => setting.ID = value; }
        public string Name { get => setting.Name; set => setting.Name = value; }
        public float Value { get => float.Parse(setting.Value.ToString()); set => setting.Value = value; }
        public int ValueInt { get => int.Parse(setting.Value.ToString()); set => setting.Value = value; }

        public SettingSlider(Settings set)
        {
            setting = set;
        }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class SettingSpacer 
    {

    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class SettingText 
    {
        public Text text;
        public Shadow textShadow;

        public Image background;
        public string Text { get => text.text; set => text.text = value; }
        public Color TextColor { get => text.color; set => text.color = value; }
        public Color BackgroundColor { get => background.color; set => background.color = value; }
        public SettingText()
        {
            text = new GameObject("z").AddComponent<Text>();
        }
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class SettingTextBox 
    {
        Settings setting;
        public Text nameText;
        public Shadow nameShadow;
        public GameObject gameObject;
        public InputField inputField;
        public Image inputImage;
        public Text inputPlaceholderText;
        public bool Enabled;
        public string ID { get => setting.ID; set => setting.ID = value; }
        public string Name { get => setting.Name; set => setting.Name = value; }
        public string Value { get => setting.Value.ToString(); set => setting.Value = value; }
        public string Placeholder { get => setting.Vals[0].ToString(); set => setting.Vals[0] = value; }
        public string defaultValue;
        public SettingTextBox(Settings set)
        {
            gameObject = new GameObject();
            setting = set;
        }

    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class SettingToggle 
    {

    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]

    public class SettingBoolean 
    {

    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class SettingNumber 
    {
 
    }
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class SettingString 
    {
  
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
