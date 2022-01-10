using UnityEngine;
using UnityEngine.Events;
using System;

namespace MSCLoader
{
    internal enum ColorValues
    {
        R,
        G,
        B,
        A
    }
    internal class ColorPicker : MonoBehaviour
    {
        public class ColorChangedEvent : UnityEvent<Color32> { }
        private byte _red = 0;
        private byte _green = 0;
        private byte _blue = 0;

        private byte _alpha = 255;

        public ColorChangedEvent onValueChanged = new ColorChangedEvent();
        public GameObject AlphaSlider;

        public Color32 CurrentColor
        {
            get => new Color32(_red, _green, _blue, _alpha);
            set
            {
                _red = value.r;
                _green = value.g;
                _blue = value.b;
                _alpha = value.a;
                SendChangedEvent();
            }
        }

        private void Start()
        {
            SendChangedEvent();
        }

        public byte R
        {
            get => _red;
            set
            {
                if (_red == value)
                    return;

                _red = value;
                SendChangedEvent();
            }
        }

        public byte G
        {
            get => _green;
            set
            {
                if (_green == value)
                    return;

                _green = value;
                SendChangedEvent();
            }
        }

        public byte B
        {
            get => _blue;
            set
            {
                if (_blue == value)
                    return;

                _blue = value;
                SendChangedEvent();
            }
        }

        private byte A
        {
            get => _alpha;
            set
            {
                if (_alpha == value)
                    return;

                _alpha = value;

                SendChangedEvent();
            }
        }
        private void SendChangedEvent()
        {
            onValueChanged.Invoke(CurrentColor);
        }

        public void AssignColor(ColorValues type, byte value)
        {
            switch (type)
            {
                case ColorValues.R:
                    R = value;
                    break;
                case ColorValues.G:
                    G = value;
                    break;
                case ColorValues.B:
                    B = value;
                    break;
                case ColorValues.A:
                    A = value;
                    break;
                default:
                    break;
            }
        }

        public byte GetValue(ColorValues type)
        {
            switch (type)
            {
                case ColorValues.R:
                    return R;
                case ColorValues.G:
                    return G;
                case ColorValues.B:
                    return B;
                case ColorValues.A:
                    return A;
                default:
                    return 0;
            }
        }
    }

}