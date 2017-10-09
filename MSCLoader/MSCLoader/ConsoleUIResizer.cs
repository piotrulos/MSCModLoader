using System;
using UnityEngine;
using UnityEngine.EventSystems;

//Standard unity MonoBehaviour class
namespace MSCLoader
{
    /// <summary>
    /// Resize console window by mouse
    /// </summary>
    public class ConsoleUIResizer : MonoBehaviour, IDragHandler
    {
#pragma warning disable CS1591
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
                /*m_transform.anchoredPosition = new Vector2(0f, -60f);
                m_transform.position = m_transform.position;
                m_logview.position = m_logview.position;
                m_scrollbar.position = m_scrollbar.position;
                m_logview.sizeDelta = m_logview.sizeDelta;
                m_scrollbar.sizeDelta = m_scrollbar.sizeDelta;*/
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
            //Debug.Log(m_logview.anchoredPosition.y);
            //m_logview.sizeDelta.Set(0, m_logview.sizeDelta.y + eventData.delta.y);
        }
        public void OnMouseEnter()
        {
            Cursor.SetCursor(cursor, new Vector2(24, 24), CursorMode.Auto);
        }

        public void OnMouseExit()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
#pragma warning restore CS1591
    }
}
