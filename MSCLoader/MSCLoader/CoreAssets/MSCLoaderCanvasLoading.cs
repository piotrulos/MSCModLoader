using System.Collections;
using UnityEngine.UI;

namespace MSCLoader;

internal class MSCLoaderCanvasLoading : MonoBehaviour
{
    public GameObject modLoadingUI, modUpdateUI, lContainer;
    public Text lHeader, lTitle, lMod, uTitle, uStatus;
    public Slider lProgress, uProgress;
    public Image lBackFade;

    private Coroutine updateUIAnim;

    void Awake()
    {
        modLoadingUI.SetActive(false);
        modUpdateUI.SetActive(false);
    }
    public void ToggleUpdateUI(bool toggle)
    {
        if (modUpdateUI.activeSelf == toggle) return;
        if (updateUIAnim != null) StopCoroutine(updateUIAnim);
        if (toggle)
        {
            modUpdateUI.transform.localScale = new Vector3(1, 0, 1);
        }
        else
        {
            modUpdateUI.transform.localScale = new Vector3(1, 1, 1);
        }
        updateUIAnim = StartCoroutine(UpdateUIAnim(toggle));
    }
    public void ToggleLoadingUI(bool toggle)
    {
        if (modLoadingUI.activeSelf == toggle) return;
        if (toggle)
        {
            lBackFade.color = new Color32(0, 0, 0, 245);
            lContainer.transform.localScale = new Vector3(1, 1, 1);
            modLoadingUI.SetActive(true);
        }
        else
        {
            lContainer.transform.localScale = new Vector3(1, 1, 1);
            StartCoroutine(LoadingUIAnimClose());
        }
        //   modLoadingUI.SetActive(toggle);
    }
    public void SetUpdate(string title, int progress, int maxProgress, string status)
    {
        SetUpdateTitle(title);
        SetUpdateProgress(progress, maxProgress);
        SetUpdateStatus(status);
        ToggleUpdateUI(true);
    }
    public void SetLoading(string title, int progress, int maxProgress, string status)
    {
        SetLoadingTitle(title);
        SetLoadingProgress(progress, maxProgress);
        SetLoadingStatus(status);
        ToggleLoadingUI(true);
    }
    public void SetUpdateTitle(string title) => uTitle.text = title.ToUpper();
    public void SetUpdateStatus(string status) => uStatus.text = status;
    public void SetLoadingTitle(string title) => lTitle.text = title.ToUpper();
    public void SetLoadingHeader(string header) => lHeader.text = header.ToUpper();
    public void SetLoadingStatus(string mod) => lMod.text = mod;
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
    public void SetLoadingProgress(string status)
    {
        lProgress.value++;
        SetLoadingStatus(status);
    }
    IEnumerator UpdateUIAnim(bool open)
    {
        if (open)
        {
            modUpdateUI.SetActive(open);
            modUpdateUI.transform.SetAsLastSibling(); //Always on top
            while (modUpdateUI.transform.localScale.y < 1)
            {
                modUpdateUI.transform.localScale = new Vector3(1, (float)System.Math.Round(modUpdateUI.transform.localScale.y + 0.1f, 1), 1);
                yield return null;
            }
        }
        else
        {
            while (modUpdateUI.transform.localScale.y > 0)
            {
                modUpdateUI.transform.localScale = new Vector3(1, (float)System.Math.Round(modUpdateUI.transform.localScale.y - 0.1f, 1), 1);
                yield return null;
            }
            modUpdateUI.SetActive(open);
        }
        updateUIAnim = null;
    }

    IEnumerator LoadingUIAnimClose()
    {
        bool anim = true;
        while (anim)
        {
            if (lBackFade.color.a < 1)
            {
                lBackFade.color = new Color(0, 0, 0, (float)System.Math.Round(lBackFade.color.a - 0.2f, 1));
            }
            if (lContainer.transform.localScale.y > 0)
            {
                lContainer.transform.localScale = new Vector3(1, (float)System.Math.Round(lContainer.transform.localScale.y - 0.2f, 1), 1);
            }
            if (lContainer.transform.localScale.y <= 0 && lBackFade.color.a <= 0)
            {
                anim = false;
            }
            yield return null;
        }
        modLoadingUI.SetActive(false);
    }
}
