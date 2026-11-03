using System;

namespace MSCLoader;
internal class PopupController : MonoBehaviour
{
    public GameObject popupSettingPrefab, popupWindowPrefab, popupCanvasPrefab;
    public ModMenuView mmv;
    public bool alreadyCreated = false;
#if !Mini
    PopupSetting currentPopupSetting;
    Action<string> OnSubmit;

    internal bool dontCloseOnSubmit = false;
    GameObject activePopup = null;

    public GameObject CreatePopupWindow(PopupWindow popupWindow)
    {
        GameObject popup = Instantiate(popupWindowPrefab);
        popup.transform.SetParent(transform, false);
        PopupWindowGroup popupWindowGroup = popup.GetComponent<PopupWindowGroup>();
        popupWindowGroup.popupSettingController = this;
        popupWindowGroup.PopupTitle.text = popupWindow.WindowTitle.ToUpper();
        Transform currentTransform = popupWindowGroup.PopupListView.transform;

        for (int i = 0; i < popupWindow.settingElements.Count; i++)
        {
            switch (popupWindow.settingElements[i].SettingType)
            {
                case SettingsType.LayoutGroup:
                    currentTransform = mmv.SettingsLayoutGroup(popupWindow.settingElements[i], currentTransform);
                    break;
                case SettingsType.LayoutGroupEnd:
                    currentTransform = popupWindowGroup.PopupListView.transform;
                    break;
                default:
                    mmv.SettingsList(popupWindow.settingElements[i], currentTransform);
                    break;
            }
        }
        return popup;
    }
    public void CreatePopupSetting(PopupSetting popupSetting, Action<string> onSubmit, bool dontClose)
    {
        if (alreadyCreated)
        {
            ModUI.ShowMessage("Another popup setting is already open! Please close the current popup and try again.", "Error");
            return;
        }
        OnSubmit = onSubmit;
        currentPopupSetting = popupSetting;
        dontCloseOnSubmit = dontClose;
        alreadyCreated = true;
        GameObject mWindow = Instantiate(popupSettingPrefab);
        mWindow.transform.SetParent(transform, false);
        PopupSettingGroup popupSettingGroup = mWindow.GetComponent<PopupSettingGroup>();
        popupSettingGroup.Setup(this, popupSetting.WindowTitle, popupSetting.SubmitButtonText);
        Transform currentTransform = popupSettingGroup.PopupListView.transform;

        for (int i = 0; i < popupSetting.settingElements.Count; i++)
        {
            switch (popupSetting.settingElements[i].SettingType)
            {
                case SettingsType.LayoutGroup:
                    currentTransform = mmv.SettingsLayoutGroup(popupSetting.settingElements[i], currentTransform);
                    break;
                case SettingsType.LayoutGroupEnd:
                    currentTransform = popupSettingGroup.PopupListView.transform;
                    break;
                default:
                    mmv.SettingsList(popupSetting.settingElements[i], currentTransform);
                    break;
            }
        }
        activePopup = mWindow;

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
        if (!dontCloseOnSubmit)
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