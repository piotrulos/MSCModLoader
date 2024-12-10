using System.Collections;
using UnityEngine.UI;

namespace MSCLoader;
internal class PopupSettingGroup : MonoBehaviour
{

    public RectTransform contents;
    public LayoutElement conentsElement;

    public Text HeaderTitle;
    public Image HeaderBackground;
    public GameObject HeaderListView;
    public void DestroyThis()
    {
        GameObject.Destroy(gameObject);
    }


}