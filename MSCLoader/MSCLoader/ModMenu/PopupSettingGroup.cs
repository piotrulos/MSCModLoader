using UnityEngine.UI;

namespace MSCLoader;
internal class PopupSettingGroup : MonoBehaviour
{
    public PopupSettingController popupSettingController;
    public Text PopupTitle, ConfirmButtonText;
    public Image PopupBackground;
    public GameObject PopupListView;
    public void DestroyThis() //Close
    {
#if !Mini
        popupSettingController.AfterDestroy();
        GameObject.Destroy(gameObject);
#endif
    }

    public void OnConfirm() //Submit
    {
#if !Mini

        popupSettingController.ReturnJsonString(gameObject);
        if (!popupSettingController.dontCloseOnSubmit)
            GameObject.Destroy(gameObject);
#endif
    }


}