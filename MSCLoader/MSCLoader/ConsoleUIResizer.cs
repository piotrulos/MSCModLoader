using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MSCLoader
{

#pragma warning disable CS1591
    //resize console UI by mouse
    public class ConsoleUIResizer : MonoBehaviour, IDragHandler
    {
        private RectTransform canvasRectTransform;
        private RectTransform panelRectTransform;
        private RectTransform m_otherResizer;
        private RectTransform m_transform;
        private RectTransform m_logview;
        private RectTransform m_scrollbar;
        private RectTransform m_inputField;
        private RectTransform m_submitBtn;
        private bool clampedToRight;
        private bool clampedToTop;

        public GameObject otherResizer;
        public GameObject logview;
        public GameObject scrollbar;
        public GameObject inputField;
        public GameObject submitBtn;
        public Texture2D cursor;
        public bool Xresizer = false;
        class ConsoleSizeSave
        {
            public float[] otherResizerPos = new float[2];
            public float[] otherResizerSize = new float[2];
            public float[] ResizerPos = new float[2];
            public float[] ResizerSize = new float[2];
            public float[] ScrollbarPos = new float[2];
            public float[] ScrollbarSize = new float[2];
            public float[] LogviewPos = new float[2];
            public float[] LogviewSize = new float[2];
            public float[] InputFieldSize = new float[2];
            public float[] SubmitBtnPos = new float[2];
        }
        void Start()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvasRectTransform = canvas.transform as RectTransform;
                panelRectTransform = transform as RectTransform;
            }
            m_otherResizer = otherResizer.GetComponent<RectTransform>();
            m_transform = GetComponent<RectTransform>();
            m_logview = logview.GetComponent<RectTransform>();
            m_scrollbar = scrollbar.GetComponent<RectTransform>();
            if (Xresizer)
            {
                m_submitBtn = submitBtn.GetComponent<RectTransform>();
                m_inputField = inputField.GetComponent<RectTransform>();
            }
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            panelRectTransform.SetAsLastSibling();
        }
        public void OnDrag(PointerEventData eventData)
        {
            ClampToBorder();
            if (Xresizer)
            {
                if (clampedToRight && eventData.delta.x > 0)
                    return;
                m_transform.position += new Vector3(eventData.delta.x, 0f);
                if (m_transform.anchoredPosition.x <= 0)
                    m_transform.anchoredPosition = new Vector2(0f, m_transform.anchoredPosition.y);
                m_scrollbar.anchoredPosition = new Vector2(m_transform.anchoredPosition.x, m_scrollbar.anchoredPosition.y);
                m_submitBtn.anchoredPosition = new Vector2(m_transform.anchoredPosition.x, 0f);
                m_logview.sizeDelta = new Vector2(336f + m_transform.anchoredPosition.x, 120f + m_logview.anchoredPosition.y);
                m_inputField.sizeDelta = new Vector2(326f + m_transform.anchoredPosition.x, 30f);
                m_otherResizer.sizeDelta = new Vector2(346f + m_transform.anchoredPosition.x, -8f);
            }
            else
            {
                if (clampedToTop && eventData.delta.y > 0)
                    return;
                m_transform.position += new Vector3(0f, eventData.delta.y);
                if (m_transform.anchoredPosition.y <= -60f)
                    m_transform.anchoredPosition = new Vector2(m_transform.anchoredPosition.x, -60f);
                m_logview.anchoredPosition = new Vector2(0f, m_transform.anchoredPosition.y);
                m_scrollbar.anchoredPosition = new Vector2(m_scrollbar.anchoredPosition.x, m_transform.anchoredPosition.y);
                m_otherResizer.anchoredPosition = new Vector2(m_otherResizer.anchoredPosition.x, m_transform.anchoredPosition.y);
                m_logview.sizeDelta = new Vector2(336f + m_otherResizer.anchoredPosition.x, 120f + m_logview.anchoredPosition.y);
                m_scrollbar.sizeDelta = new Vector2(10f, 120f + m_scrollbar.anchoredPosition.y);
                m_otherResizer.sizeDelta = new Vector2(-8f, 150f + m_otherResizer.anchoredPosition.y);
            }
        }
        public void OnMouseEnter()
        {
            Cursor.SetCursor(cursor, new Vector2(24, 24), CursorMode.Auto);
        }

        public void OnMouseExit()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        public void LoadConsoleSize()
        {
            if (!Xresizer) return;
            Start();
            string path = ModLoader.GetModSettingsFolder(new ModConsole());
            if (File.Exists(Path.Combine(path, "consoleSize.data")))
            {
                try
                {
                    ConsoleSizeSave css = JsonConvert.DeserializeObject<ConsoleSizeSave>(File.ReadAllText(Path.Combine(path, "consoleSize.data")));
                    m_transform.anchoredPosition = new Vector2( css.ResizerPos[0], css.ResizerPos[1]);
                    m_transform.sizeDelta = new Vector2(css.ResizerSize[0], css.ResizerSize[1]);
                    m_otherResizer.anchoredPosition = new Vector2(css.otherResizerPos[0], css.otherResizerPos[1]);
                    m_otherResizer.sizeDelta = new Vector2(css.otherResizerSize[0], css.otherResizerSize[1]);
                    m_scrollbar.anchoredPosition = new Vector2(css.ScrollbarPos[0], css.ScrollbarPos[1]);
                    m_scrollbar.sizeDelta = new Vector2(css.ScrollbarSize[0], css.ScrollbarSize[1]);
                    m_logview.anchoredPosition = new Vector2(css.LogviewPos[0], css.LogviewPos[1]);
                    m_logview.sizeDelta = new Vector2(css.LogviewSize[0], css.LogviewSize[1]);
                    m_inputField.sizeDelta = new Vector2(css.InputFieldSize[0], css.InputFieldSize[1]);
                    m_submitBtn.anchoredPosition = new Vector2(css.SubmitBtnPos[0], css.SubmitBtnPos[1]);
                }
                catch (Exception e)
                {
                    if (ModLoader.devMode)
                        ModConsole.Error(e.ToString());
                    System.Console.WriteLine(e);
                    File.Delete(Path.Combine(path, "consoleSize.data"));
                }
            }
        }
        public void SaveConsoleSize()
        {
            string path = ModLoader.GetModSettingsFolder(new ModConsole());
            if (Xresizer)
            {
                ConsoleSizeSave css = new ConsoleSizeSave()
                {
                    otherResizerPos = new float[] { m_otherResizer.anchoredPosition.x, m_otherResizer.anchoredPosition.y },
                    otherResizerSize = new float[] { m_otherResizer.sizeDelta.x, m_otherResizer.sizeDelta.y },
                    ResizerPos = new float[] { m_transform.anchoredPosition.x, m_transform.anchoredPosition.y },
                    ResizerSize = new float[] { m_transform.sizeDelta.x, m_transform.sizeDelta.y },
                    ScrollbarPos = new float[] { m_scrollbar.anchoredPosition.x, m_scrollbar.anchoredPosition.y },
                    ScrollbarSize = new float[] { m_scrollbar.sizeDelta.x, m_scrollbar.sizeDelta.y },
                    LogviewPos = new float[] { m_logview.anchoredPosition.x, m_logview.anchoredPosition.y },
                    LogviewSize = new float[] { m_logview.sizeDelta.x, m_logview.sizeDelta.y },
                    InputFieldSize = new float[] { m_inputField.sizeDelta.x, m_inputField.sizeDelta.y },
                    SubmitBtnPos = new float[] { m_submitBtn.anchoredPosition.x, m_submitBtn.anchoredPosition.y }
                };
                string serializedData = JsonConvert.SerializeObject(css, Formatting.Indented);
                File.WriteAllText(Path.Combine(path, "consoleSize.data"), serializedData);
            }
        }
        private void ClampToBorder()
        {
            Vector3[] canvasCorners = new Vector3[4];
            Vector3[] panelRectCorners = new Vector3[4];
            canvasRectTransform.GetWorldCorners(canvasCorners);
            panelRectTransform.GetWorldCorners(panelRectCorners);

            if (panelRectCorners[2].x > canvasCorners[2].x - 5)
            {
                if (!clampedToRight)
                {
                    clampedToRight = true;
                }
            }
            else if (clampedToRight)
            {
                clampedToRight = false;
            }

            if (panelRectCorners[2].y > canvasCorners[2].y - 5)
            {

                if (!clampedToTop)
                {
                    clampedToTop = true;
                }
            }
            else if (clampedToTop)
            {
                clampedToTop = false;
            }
        }
#pragma warning restore CS1591
    }
}
