using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    internal class ColorSliderImage : MonoBehaviour
    {
        public ColorPicker picker;
        public ColorValues type;
        public Slider.Direction direction;
        private RawImage image;

         private void Awake()
        {
            image = GetComponent<RawImage>();
            RegenerateTexture();
        }

        private void OnEnable()
        {
            if (picker != null && Application.isPlaying)
            {
                picker.onValueChanged.AddListener(ColorChanged);
            }
        }

        private void OnDisable()
        {
            if (picker != null)
            {
                picker.onValueChanged.RemoveListener(ColorChanged);
            }
        }

        private void OnDestroy()
        {
            if (image.texture != null)
                DestroyImmediate(image.texture);
        }

        private void ColorChanged(Color32 newColor)
        {
            switch (type)
            {
                case ColorValues.R:
                case ColorValues.G:
                case ColorValues.B:
                    RegenerateTexture();
                    break;
                default:
                    break;
            }
        }

        private void RegenerateTexture()
        {
            Color32 baseColor;
            if (picker != null)
                baseColor = picker.CurrentColor;
            else
                baseColor = Color.black;

            Texture2D texture;
            Color32[] colors;

            bool vertical = direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom;
            bool inverted = direction == Slider.Direction.TopToBottom || direction == Slider.Direction.RightToLeft;

            int size = 255;
            if (vertical)
                texture = new Texture2D(1, size);
            else
                texture = new Texture2D(size, 1);

            texture.hideFlags = HideFlags.DontSave;
            colors = new Color32[size];

            switch (type)
            {
                case ColorValues.R:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(i, baseColor.g, baseColor.b, baseColor.a);
                    }
                    break;
                case ColorValues.G:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(baseColor.r, i, baseColor.b, baseColor.a);
                    }
                    break;
                case ColorValues.B:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(baseColor.r, baseColor.g, i, baseColor.a);
                    }
                    break;
                case ColorValues.A:
                    for (byte i = 0; i < size; i++)
                    {
                        colors[inverted ? size - 1 - i : i] = new Color32(i, i, i, 255);
                    }
                    break;            
                default:
                    break;
            }
            texture.SetPixels32(colors);
            texture.Apply();

            if (image.texture != null)
                DestroyImmediate(image.texture);
            image.texture = texture;

            switch (direction)
            {
                case Slider.Direction.BottomToTop:
                case Slider.Direction.TopToBottom:
                    image.uvRect = new Rect(0, 0, 2, 1);
                    break;
                case Slider.Direction.LeftToRight:
                case Slider.Direction.RightToLeft:
                    image.uvRect = new Rect(0, 0, 1, 2);
                    break;
                default:
                    break;
            }
        }

    }
}