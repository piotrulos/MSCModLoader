///Heavily modified Unity UI extensions (old ass version) (BSD3 license)
using UnityEngine.UI;

namespace MSCLoader
{
    [RequireComponent(typeof(RectTransform), typeof(Button))]
    internal class DropDownListButton
    {
        public RectTransform rectTransform;
        public Button btn;
        public Text txt;
        public Image btnImg;
        public Image img;
        public GameObject gameobject;

        public DropDownListButton(GameObject btnObj)
        {
            gameobject = btnObj;
            rectTransform = btnObj.GetComponent<RectTransform>();
            btnImg = btnObj.GetComponent<Image>();
            btn = btnObj.GetComponent<Button>();
            txt = rectTransform.FindChild("Text").GetComponent<Text>();
            img = rectTransform.FindChild("Image").GetComponent<Image>();
        }
    }
}