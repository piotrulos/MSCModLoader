using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Make window draggable
/// </summary>
public class ModUIDrag : MonoBehaviour, IDragHandler
{
    RectTransform m_transform = null;

    void Start()
    {
        m_transform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
    }
}