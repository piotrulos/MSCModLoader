using UnityEngine.UI;

namespace MSCLoader;
internal class PopupSettingGroup : MonoBehaviour
{
    public PopupController popupSettingController;
    public Text PopupTitle, ConfirmButtonText;
    public Image PopupBackground;
    public GameObject PopupListView;

    internal void Setup(PopupController psc, string title, string confirmButtonText)
    {
        popupSettingController = psc;
        PopupTitle.text = title.ToUpper();
        ConfirmButtonText.text = confirmButtonText.ToUpper();
    }
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