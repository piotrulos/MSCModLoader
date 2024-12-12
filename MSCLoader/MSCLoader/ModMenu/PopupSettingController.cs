using System;

namespace MSCLoader;
internal class PopupSettingController : MonoBehaviour
{
    public GameObject popupSettingPrefab;
    public bool alreadyCreated = false;
#if !Mini
    PopupSetting currentPopupSetting;
    Action<string> OnSubmit;
    public void CreatePopupSetting(PopupSetting popupSetting, Action<string> onSubmit)
    {
        if (!alreadyCreated)
        {
            OnSubmit = onSubmit;
            currentPopupSetting = popupSetting;
            alreadyCreated = true;
            GameObject mWindow = Instantiate(popupSettingPrefab);
            mWindow.transform.SetParent(transform, false);
            PopupSettingGroup popupSettingGroup = mWindow.GetComponent<PopupSettingGroup>();
            popupSettingGroup.popupSettingController = this;
            ModMenuView mmv = GameObject.Find("MSCLoader Mod Menu").GetComponentInChildren<ModMenuView>();
            popupSettingGroup.PopupTitle.text = popupSetting.WindowTitle.ToUpper();
            popupSettingGroup.ConfirmButtonText.text = popupSetting.SubmitButtonText.ToUpper();
            for (int i = 0; i < popupSetting.settingElements.Count; i++)
            {
                mmv.SettingsList(popupSetting.settingElements[i], popupSettingGroup.PopupListView.transform);
            }
        }
    }

    public void AfterDestroy()
    {
        alreadyCreated = false;
        currentPopupSetting = null;
        OnSubmit = null;
    }
    public void ReturnJsonString()
    {
        if (currentPopupSetting == null) return;
        string result = currentPopupSetting.ReturnJsonString();
        if (OnSubmit != null)
        {
            try
            {
                OnSubmit(result);
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to execute OnSubmit: {e.Message}");
                Console.WriteLine(e);
            }
        }
        AfterDestroy();
    }
#endif
}