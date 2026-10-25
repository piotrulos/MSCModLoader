using UnityEngine.UI;

namespace MSCLoader;

internal class PopupWindowGroup : MonoBehaviour
{
    public PopupController popupSettingController;
    public Text PopupTitle;
    public Image PopupBackground;
    public GameObject PopupListView;

    public void DestroyThis() //Close
    {
#if !Mini
        GameObject.Destroy(gameObject);
#endif
    }

}