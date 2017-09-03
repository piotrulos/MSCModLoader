using UnityEngine;
using UnityEngine.EventSystems;

namespace MSCLoader
{
    /// <summary>
    /// Make window draggable, attach to UI gameobject 
    /// </summary>
    public class ModUIDrag : MonoBehaviour, IDragHandler
    {

        RectTransform m_transform = null;

        void Start()
        {
            m_transform = GetComponent<RectTransform>();
        }
#pragma warning disable CS1591
        public void OnDrag(PointerEventData eventData)
        {
            m_transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
        }
#pragma warning restore CS1591
    }
}