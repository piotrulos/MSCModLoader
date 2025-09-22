using System;
using System.Collections;
using UnityEngine.UI;

namespace MSCLoader;

internal class MessageBoxesCanvas : MonoBehaviour
{
    public GameObject messageBoxPrefab, messageBoxScrollablePrefab, messageBoxBtnPrefab;
}

internal class MessageBoxHelper : MonoBehaviour
{
    public Text messageBoxTitle, messageBoxContent;
    public GameObject btnRow1, btnRow2;
    public RectTransform messageBoxContainer;
    public bool isScrollable = false;

    internal Action convertToScrollable;
    void Start()
    {
        if (isScrollable) return;
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        yield return null; //Wait a signle frame to get true rect values
        if (messageBoxContainer.rect.height > (GetComponent<RectTransform>().rect.height - 150))
        {
            convertToScrollable?.Invoke();
        }
    }
}
