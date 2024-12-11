using UnityEngine.UI;

namespace MSCLoader;
internal class PopupSettingController : MonoBehaviour
{
    public GameObject popupSettingPrefab;
    public bool alreadyCreated = false;
    PopupSetting currentPopupSetting;
    public void CreatePopupSetting(PopupSetting popupSetting)
    {
        if (!alreadyCreated)
        {
            currentPopupSetting = popupSetting;
            alreadyCreated = true;
            GameObject mWindow = Instantiate(popupSettingPrefab);
            mWindow.transform.SetParent(transform, false);
            PopupSettingGroup popupSettingGroup = mWindow.GetComponent<PopupSettingGroup>();
            popupSettingGroup.popupSettingController = this;
            ModMenuView mmv = GameObject.Find("MSCLoader Mod Menu").GetComponentInChildren<ModMenuView>();
            popupSettingGroup.PopupTitle.text = popupSetting.WindowTitle.ToUpper();
            for (int i = 0; i < popupSetting.settingElements.Count; i++)
            {
                mmv.SettingsList(popupSetting.settingElements[i], popupSettingGroup.PopupListView.transform);
            }
        }
    }
    public void ReturnJsonString()
    {
        if(currentPopupSetting != null) currentPopupSetting.ReturnJsonString();
    }
}