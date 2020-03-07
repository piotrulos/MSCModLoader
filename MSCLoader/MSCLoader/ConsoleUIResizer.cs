using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MSCLoader
{

#pragma warning disable CS1591
    //resize console UI by mouse
    public class ConsoleUIResizer : MonoBehaviour, IDragHandler
    {
        RectTransform m_transform = null;
        RectTransform m_logview = null;
        RectTransform m_scrollbar = null;
        public GameObject logview;
        public GameObject scrollbar;
        public Texture2D cursor;

        void Start()
        {
            m_transform = GetComponent<RectTransform>();
            m_logview = logview.GetComponent<RectTransform>();
            m_scrollbar = scrollbar.GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_transform.anchoredPosition.y < -60)
            {
                if (eventData.delta.y > 0)
                {
                    m_transform.position += new Vector3(0f, eventData.delta.y);
                    m_logview.position += new Vector3(0f, eventData.delta.y);
                    m_scrollbar.position += new Vector3(0f, eventData.delta.y);
                    m_logview.sizeDelta = new Vector2(333f, 120 + m_logview.anchoredPosition.y);
                    m_scrollbar.sizeDelta = new Vector2(13f, 120 + m_scrollbar.anchoredPosition.y);
                }

            }
            else
            {
                m_transform.position += new Vector3(0f, eventData.delta.y);
                m_logview.position += new Vector3(0f, eventData.delta.y);
                m_scrollbar.position += new Vector3(0f, eventData.delta.y);
                m_logview.sizeDelta = new Vector2(333f, 120 + m_logview.anchoredPosition.y);
                m_scrollbar.sizeDelta = new Vector2(13f, 120 + m_scrollbar.anchoredPosition.y);
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
            string path = ModLoader.GetModSettingsFolder(new ModConsole());
            if (File.Exists(Path.Combine(path, "console.data")))
            {
                try
                {
                    m_transform = GetComponent<RectTransform>();
                    m_logview = logview.GetComponent<RectTransform>();
                    m_scrollbar = scrollbar.GetComponent<RectTransform>();
                    string data = File.ReadAllText(Path.Combine(path, "console.data"));
                    string[] values = data.Trim().Split(',');
                    if (float.Parse(values[0], CultureInfo.InvariantCulture) <= -60) //don't go offscreen
                    {
                        values[0] = "-60"; values[1] = "-60"; values[2] = "-60"; values[3] = "60"; values[4] = "60";
                    }
                    m_transform.anchoredPosition = new Vector3(0f, float.Parse(values[0], CultureInfo.InvariantCulture));
                    m_logview.anchoredPosition = new Vector3(0f, float.Parse(values[1], CultureInfo.InvariantCulture));
                    m_scrollbar.anchoredPosition = new Vector3(0f, float.Parse(values[2], CultureInfo.InvariantCulture));
                    m_logview.sizeDelta = new Vector2(333f, float.Parse(values[3], CultureInfo.InvariantCulture));
                    m_scrollbar.sizeDelta = new Vector2(13f, float.Parse(values[4], CultureInfo.InvariantCulture));
                }
                catch (Exception e)
                {
                    if (ModLoader.devMode)
                        ModConsole.Error(e.ToString());
                    Debug.Log(e);
                    File.Delete(Path.Combine(path, "console.data"));
                }
            }
        }
        public void SaveConsoleSize()
        {
            string path = ModLoader.GetModSettingsFolder(new ModConsole());
            string data = string.Format("{0},{1},{2},{3},{4}", m_transform.anchoredPosition.y, m_logview.anchoredPosition.y, m_scrollbar.anchoredPosition.y, m_logview.sizeDelta.y, m_scrollbar.sizeDelta.y);
            File.WriteAllText(Path.Combine(path, "console.data"), data);
        }
#pragma warning restore CS1591
    }
}
