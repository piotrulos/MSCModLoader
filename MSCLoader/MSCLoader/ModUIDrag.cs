using UnityEngine.EventSystems;

namespace MSCLoader;

/// <summary>
/// Make Unity.UI element draggable, attachable to UI gameobject 
/// </summary>
public class ModUIDrag : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    /// <summary>
    /// RectTransform to move (recommended to be root of the window)
    /// </summary>
    public RectTransform rectTransform = null;

    void Start()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        rectTransform.position += new Vector3(eventData.delta.x, eventData.delta.y);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
    }
}
