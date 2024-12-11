using System.Collections;
using UnityEngine.UI;

namespace MSCLoader;
internal class PopupSettingGroup : MonoBehaviour
{
    public PopupSettingController popupSettingController;
    public Text PopupTitle;
    public Image PopupBackground;
    public GameObject PopupListView;
    public void DestroyThis()
    {
        popupSettingController.alreadyCreated = false;
        GameObject.Destroy(gameObject);        
    }

    public void OnConfirm()
    {
        popupSettingController.ReturnJsonString();
        popupSettingController.alreadyCreated = false;
        GameObject.Destroy(gameObject);
    }


}