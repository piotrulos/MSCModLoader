using UnityEngine.UI;

namespace MSCLoader;
internal class PopupSettingGroup : MonoBehaviour
{
    public PopupSettingController popupSettingController;
    public Text PopupTitle, ConfirmButtonText;
    public Image PopupBackground;
    public GameObject PopupListView;
    public void DestroyThis()
    {
#if !Mini
        popupSettingController.AfterDestroy();
        GameObject.Destroy(gameObject);
#endif
    }

    public void OnConfirm()
    {
#if !Mini

        popupSettingController.ReturnJsonString();
        GameObject.Destroy(gameObject);
#endif
    }


}