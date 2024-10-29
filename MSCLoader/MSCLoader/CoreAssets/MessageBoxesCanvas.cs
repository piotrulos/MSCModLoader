using UnityEngine.UI;

namespace MSCLoader;

internal class MessageBoxesCanvas : MonoBehaviour
{
    public GameObject messageBoxPrefab, messageBoxBtnPrefab;
    public GameObject changelogWindow;

    public Text changelogText;
}

internal class MessageBoxHelper : MonoBehaviour
{
    public Text messageBoxTitle, messageBoxContent;
    public GameObject btnRow1, btnRow2;
}
