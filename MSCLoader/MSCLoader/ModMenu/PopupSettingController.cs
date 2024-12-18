using System;

namespace MSCLoader;
internal class PopupSettingController : MonoBehaviour
{
    public GameObject popupSettingPrefab;
    public bool alreadyCreated = false;
#if !Mini
    PopupSetting currentPopupSetting;
    Action<string> OnSubmit;
    internal bool dontCloseOnSubmit = false;
    GameObject activePopup = null;
    public void CreatePopupSetting(PopupSetting popupSetting, Action<string> onSubmit, bool dontClose)
    {
        if (!alreadyCreated)
        {
            OnSubmit = onSubmit;
            currentPopupSetting = popupSetting;
            dontCloseOnSubmit = dontClose;
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
        activePopup = null;
    }
    public void ReturnJsonString(GameObject popup)
    {
        if (currentPopupSetting == null) return;
        activePopup = popup;

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
        if(!dontCloseOnSubmit)
            AfterDestroy();
    }

    public void DestroyActivePopup()
    {
        if (activePopup != null)
        {
            GameObject.Destroy(activePopup);
            AfterDestroy();
        }
    }

#endif
}