using UnityEngine.UI;

namespace MSCLoader;

internal class MSCLoaderCanvasLoading : MonoBehaviour
{
    public GameObject modLoadingUI, modUpdateUI;
    public Text lHeader, lTitle, lMod, uTitle, uStatus;
    public Slider lProgress, uProgress;

    void Awake()
    {
        modLoadingUI.SetActive(false);
        modUpdateUI.SetActive(false);
    }
    public void ToggleUpdateUI(bool toggle)
    {
        modUpdateUI.SetActive(toggle);
        modUpdateUI.transform.SetAsLastSibling(); //Always on top
        ModConsole.Warning("Update UI " + toggle);

    }
    public void ToggleLoadingUI(bool toggle)
    {
        modLoadingUI.transform.SetAsLastSibling(); //Always on top
        modLoadingUI.SetActive(toggle);
    }
    public void SetUpdate(string title, int progress, int maxProgress, string status)
    {
        SetUpdateTitle(title);
        SetUpdateProgress(progress, maxProgress);
        SetUpdateStatus(status);
        ToggleUpdateUI(true);
    }
    public void SetUpdateTitle(string title) => uTitle.text = title.ToUpper();
    public void SetUpdateStatus(string status) => uStatus.text = status;
    public void SetLoadingTitle(string title) => lHeader.text = title.ToUpper();
    public void SetLoadingMod(string mod) => lMod.text = mod;
    public void SetUpdateProgress(int progress, int maxValue)
    {
        uProgress.value = progress;
        uProgress.maxValue = maxValue;
    }
    public void SetUpdateProgress(int progress, string status)
    {
        uProgress.value = progress;
        SetUpdateStatus(status);

    }
    public void SetLoadingProgress(int progress, int maxValue)
    {
        lProgress.value = progress;
        lProgress.maxValue = maxValue;
    }

}
