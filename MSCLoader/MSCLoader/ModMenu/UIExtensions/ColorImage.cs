using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    internal class ColorImage : MonoBehaviour
    {
        public ColorPicker picker;

        public Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
            picker.onValueChanged.AddListener(ColorChanged);
        }

        private void OnDestroy()
        {
            picker.onValueChanged.RemoveListener(ColorChanged);
        }

        private void ColorChanged(Color32 newColor)
        {
            image.color = newColor;
        }
    }
}