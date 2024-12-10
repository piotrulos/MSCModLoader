using System.Collections;
using UnityEngine.UI;

namespace MSCLoader;
internal class SettingsGroup : MonoBehaviour
{

    public RawImage headerButtonImg;
    public RectTransform contents;
    public LayoutElement conentsElement;
    public Texture2D upArrow, downArrow;
    private bool collapsed;
    private bool anim;

    public Text HeaderTitle;
    public Image HeaderBackground;
    public GameObject HeaderListView;
    public void Toggle()
    {
        if (anim) return;
        if (collapsed)
        {
            SetHeader(true);
        }
        else
        {
            SetHeader(false);
        }
    }

    public void SetHeader(bool expand)
    {
        if (collapsed)
        {
            if (expand)
            {
                StartCoroutine(Anim(true));
                headerButtonImg.texture = upArrow;
            }
        }
        else
        {
            if (!expand)
            {
                StartCoroutine(Anim(false));
                headerButtonImg.texture = downArrow;
            }
        }
    }

    public void SetHeaderNoAnim(bool expand)
    {
        if (collapsed)
        {
            if (expand)
            {
                headerButtonImg.texture = upArrow;
                conentsElement.ignoreLayout = false;
                contents.localScale = new Vector3(1, 1, 1);
                collapsed = false;
            }
        }
        else
        {
            if (!expand)
            {
                headerButtonImg.texture = downArrow;
                contents.localScale = new Vector3(1, 0, 1);
                conentsElement.ignoreLayout = true;
                collapsed = true;
            }
        }
    }
    IEnumerator Anim(bool expand)
    {

        anim = true;
        if (expand)
        {
            conentsElement.ignoreLayout = false;

            for (int i = 0; i < 50; i++)
            {
                contents.localScale = new Vector3(1, (float)System.Math.Round(contents.localScale.y + 0.02f, 2), 1);
                yield return null;
            }
            collapsed = false;
        }
        else
        {

            for (int i = 0; i < 50; i++)
            {
                contents.localScale = new Vector3(1, (float)System.Math.Round(contents.localScale.y - 0.02f, 2), 1);
                yield return null;
            }
            conentsElement.ignoreLayout = true;
            collapsed = true;

        }
        anim = false;
    }
}