using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MSCLoader
{
    public class ModUI : MonoBehaviour
    {
        /// <summary>
        /// Create canvas for UI
        /// </summary>
        public static void CreateCanvas()
        {
            GameObject canvasGO = new GameObject();
            canvasGO.name = "MSCLoader Canvas";
            canvasGO.layer = 5;
            canvasGO.AddComponent<Canvas>();
            canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1280f, 720f);
            canvasGO.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            canvasGO.GetComponent<CanvasScaler>().referencePixelsPerUnit = 100;
            canvasGO.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasGO);

            //create EventSystem
            GameObject evSys = new GameObject();
            evSys.name = "EventSystem";
            evSys.AddComponent<EventSystem>();
            evSys.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(evSys);
        }
        /// <summary>
        /// Create base gameObject for your UI.
        /// </summary>
        /// <param name="name">GameObject name.</param>
        /// <param name="parent">Parent object for this base.</param>
        public static GameObject CreateUIBase(string name, GameObject parent)
        {
            GameObject createWindow = new GameObject();
            createWindow.name = name;
            createWindow.layer = 5;
            createWindow.transform.SetParent(parent.transform,false);
            createWindow.AddComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            return createWindow;
        }

        /// <summary>
        /// Create parent gameObject for UI on canvas.
        /// </summary>
        /// <param name="name">GameObject name.</param>
        /// <param name="draggable">Can drag this object by mouse.</param>
        public static GameObject CreateParent(string name, bool draggable)
        {
            GameObject createWindow = CreateUIBase(name, GameObject.Find("MSCLoader Canvas"));
            if (draggable)
                createWindow.AddComponent<ModUIDrag>();
            return createWindow;
        }

        public static void Separator(GameObject parent, string text = null)
        {

            GameObject separator = CreateUIBase("Separator", parent);
            separator.AddComponent<Image>().color = Color.white;
            separator.AddComponent<LayoutElement>().minHeight = 1;
            if (text != null)
            {
                GameObject keyBindsTitle =CreateTextBlock("SeparatorText", text, parent, TextAnchor.MiddleCenter);
                keyBindsTitle.GetComponent<Text>().fontStyle = FontStyle.Bold;
                GameObject separator2 = CreateUIBase("Separator", parent);
                separator2.AddComponent<Image>().color = Color.white;
                separator2.AddComponent<LayoutElement>().minHeight = 1;

            }
        }

        /// <summary>
        /// Creates Text on canvas or any other selected parent
        /// </summary>
        /// <param name="name">GameObject name.</param>
        /// <param name="text">Text you want to display.</param>
        /// <param name="parent">Where to put GameObject.</param>
        /// <param name="color">Font color (default white)</param>
        /// <param name="outline">A small outline around text for clear visibility on any surface. (default true)</param>
        public static GameObject CreateTextBlock(string name, string text, GameObject parent, TextAnchor alignment = TextAnchor.MiddleLeft, Color? color =  null, bool outline = false)
        {
            GameObject textBlock = CreateUIBase(name, parent);
            textBlock.AddComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textBlock.GetComponent<Text>().alignment = alignment;
            textBlock.GetComponent<Text>().color = color.GetValueOrDefault(Color.white);
            textBlock.GetComponent<Text>().text = text;
            if(outline)
            {
                textBlock.AddComponent<Outline>();
            }
            return textBlock;
        }

        /// <summary>
        /// Creates Input Field
        /// </summary>
        /// <param name="name">GameObject name.</param>
        /// <param name="placeholderText">Text you want to display as placeholder.</param>
        /// <param name="parent">Where to put GameObject.</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        public static GameObject CreateInputField(string name, string placeholderText, GameObject parent, float width, float height)
        {
            GameObject inputField = CreateUIBase(name, parent);
            inputField.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

            GameObject inputFieldp = CreateTextBlock("Placeholder", placeholderText, inputField, TextAnchor.MiddleLeft, new Color(1, 1, 1, 0.5f));
            inputFieldp.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            inputFieldp.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            inputFieldp.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            inputFieldp.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 20, height - 12);

            GameObject inputFieldt = CreateTextBlock("InputText", string.Empty, inputField, TextAnchor.MiddleLeft);
            inputFieldt.GetComponent<Text>().supportRichText = false;
            inputFieldt.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            inputFieldt.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            inputFieldt.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            inputFieldt.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 20, height - 12);

            inputField.AddComponent<Image>().color = Color.gray;
            inputField.AddComponent<InputField>().targetGraphic = inputField.GetComponent<Image>();
            inputField.GetComponent<InputField>().textComponent = inputFieldt.GetComponent<Text>();
            inputField.GetComponent<InputField>().placeholder = inputFieldp.GetComponent<Text>();

            return inputField;
        }
        /// <summary>
        /// Creates Button
        /// </summary>
        /// <param name="name">GameObject name.</param>
        /// <param name="text">Text you want to display on this button.</param>
        /// <param name="parent">Where to put GameObject.</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        public static GameObject CreateButton(string name, string text, GameObject parent, float width, float height)
        {
            GameObject Btn = CreateUIBase(name, parent);
            Btn.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            Btn.AddComponent<Image>();
            Btn.AddComponent<Button>().targetGraphic = Btn.GetComponent<Image>();

            GameObject BtnTxt = CreateTextBlock("InputText", text, Btn, TextAnchor.MiddleCenter, Color.black);
            BtnTxt.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            BtnTxt.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            BtnTxt.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            BtnTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

            return Btn;
        }
        public static GameObject CreateScrollbar(GameObject parent, float width, float height,float rotate)
        {
            GameObject scrollbar = CreateUIBase("Scrollbar", parent);
            scrollbar.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            scrollbar.GetComponent<RectTransform>().Rotate(0, 0, rotate);
            scrollbar.AddComponent<Image>().color = Color.black;

            GameObject scrollbarsa = CreateUIBase("Sliding Area", scrollbar);
            scrollbarsa.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            scrollbarsa.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            scrollbarsa.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            scrollbarsa.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            scrollbarsa.GetComponent<RectTransform>().Rotate(0, 0, rotate);

            GameObject scrollbarha = CreateUIBase("Handle", scrollbarsa);
            scrollbarha.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            scrollbarha.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            scrollbarha.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            scrollbarha.GetComponent<RectTransform>().Rotate(0, 0, rotate);
            scrollbarha.AddComponent<Image>().color = Color.white;
            scrollbar.AddComponent<Scrollbar>().handleRect = scrollbarha.GetComponent<RectTransform>();
            scrollbar.GetComponent<Scrollbar>().targetGraphic = scrollbarha.GetComponent<Image>();
            return scrollbar;
        }

    }
}
