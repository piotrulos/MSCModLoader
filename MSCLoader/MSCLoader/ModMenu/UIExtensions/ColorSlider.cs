using UnityEngine;
using UnityEngine.UI;
using System;

namespace MSCLoader
{  
    internal class ColorSlider : MonoBehaviour
    {
        public ColorPicker hsvpicker;
        public ColorValues type;
        public Text valueTxt;
        private Slider slider;

        private bool listen = true;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            hsvpicker.onValueChanged.AddListener(ColorChanged);
            slider.onValueChanged.AddListener(SliderChanged);
        }

        private void OnDestroy()
        {
            hsvpicker.onValueChanged.RemoveListener(ColorChanged);
            slider.onValueChanged.RemoveListener(SliderChanged);
        }

        private void ColorChanged(Color32 newColor)
        {
            listen = false;
            slider.onValueChanged.RemoveListener(SliderChanged);
            switch (type)
            {
                case ColorValues.R:
                    slider.value = newColor.r;
                    break;
                case ColorValues.G:
                    slider.value = newColor.g;
                    break;
                case ColorValues.B:
                    slider.value = newColor.b;
                    break;
                case ColorValues.A:
                    slider.value = newColor.a;
                    break;
                default:
                    break;
            }
            valueTxt.text = slider.value.ToString();
            slider.onValueChanged.AddListener(SliderChanged);

            listen = true;

        }

        private void SliderChanged(float newValue)
        {
            if (listen)
            {
                valueTxt.text = slider.value.ToString();
                hsvpicker.AssignColor(type, (byte)Math.Round(newValue, 0));

            }
        }
    }
}