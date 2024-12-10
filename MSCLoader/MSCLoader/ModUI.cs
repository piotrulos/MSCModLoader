using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MSCLoader;

internal class MSCLCanvasManager : MonoBehaviour
{
    private bool isApplicationQuitting = false;
    private Canvas canvas;
    void Start()
    {
        gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
        canvas = GetComponent<Canvas>();
    }
#if !Mini
    void FixedUpdate()
    {
        if (!canvas.enabled) canvas.enabled = true;
    }
    void OnDisable()
    {
        if (isApplicationQuitting) return;
        ModLoader.HandleCanv(gameObject);
    }
    void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }
#endif
}
#if !Mini
/// <summary>
/// MsgBoxBtn class
/// </summary>
public class MsgBoxBtn
{
    internal Action IfClicked = null;
    internal Button button;
    internal Transform transform;
    internal bool noDestroy;

    internal MsgBoxBtn(Action ifClicked, Button btn, bool noClosing)
    {
        IfClicked = ifClicked;
        button = btn;
        transform = btn.transform;
        noDestroy = noClosing;
    }
}
/// <summary>
/// UI elements like creating Message Boxes
/// </summary>
public class ModUI
{
    internal static MessageBoxesCanvas messageBoxCv;
    internal static PopupSettingController popupSettingController;
    internal static GameObject canvasPrefab;
    private static GameObject msclCanv;

    internal static void PrepareDefaultCanvas()
    {
        msclCanv = CreateCanvas("MSCLoader Canvas", true);
        msclCanv.AddComponent<MSCLCanvasManager>();

        //create EventSystem
        GameObject evSys = new GameObject();
        evSys.name = "EventSystem";
        evSys.AddComponent<EventSystem>();
        evSys.AddComponent<StandaloneInputModule>();
        GameObject.DontDestroyOnLoad(evSys);
    }

    /// <summary>
    /// Create Canvas for your UI.
    /// </summary>
    /// <param name="name">Name for your canvas</param>
    /// <param name="dontDestroyOnLoad">Add dont destroy on load flag (optional)</param>
    /// <returns>Created canvas as GameObject</returns>
    public static GameObject CreateCanvas(string name, bool dontDestroyOnLoad = false)
    {
        GameObject go = GameObject.Instantiate(canvasPrefab);
        go.name = name;
        if (dontDestroyOnLoad)
            GameObject.DontDestroyOnLoad(go);
        return go;
    }

    internal static GameObject GetCanvas(byte type)
    {
        return msclCanv;
    }
    /// <summary>
    /// Get UI canvas
    /// </summary>
    /// <returns>Canvas GameObject</returns>
    [Obsolete("It is recommended to create your own canvas using CreateCanvas() instead.")]
    public static GameObject GetCanvas() => msclCanv;

    /// <summary>
    /// Create Message Box Button for ShowCustomMessage(...);
    /// </summary>
    /// <param name="ButtonText">Text on the Button</param>
    /// <param name="ifClicked">Action if button was clicked (leave null for just closing message box)</param>
    /// <param name="noClosing">Don't close MessageBox when action is triggered</param>
    /// <returns>MsgBoxBtn</returns>
    public static MsgBoxBtn CreateMessageBoxBtn(string ButtonText, Action ifClicked = null, bool noClosing = false)
    {
        GameObject btn = GameObject.Instantiate(messageBoxCv.messageBoxBtnPrefab);
        MsgBoxBtn msgBoxBtn = new MsgBoxBtn(ifClicked, btn.GetComponent<Button>(), noClosing);
        if (noClosing && ifClicked == null)
            ModConsole.Error("<b>CreateMessageBoxBtn()</b> - <b>ifClicked</b> cannot be null when <b>noClosing</b> is set to true");
        if (noClosing && ifClicked != null)
            msgBoxBtn.button.onClick.AddListener(() => { ifClicked(); });
        btn.GetComponentInChildren<Text>().text = ButtonText.ToUpper();
        return msgBoxBtn;
    }

    /// <summary>
    /// Create Message Box Button for ShowCustomMessage(...);
    /// </summary>
    /// <param name="ButtonText">Text on the Button</param>
    /// <param name="ifClicked">Action if button was clicked (leave null for just closing message box)</param>
    /// <param name="BackgroundColor">Button background color</param>
    /// <param name="TextColor">Text color</param>
    /// <param name="noClosing">Don't close MessageBox when action is triggered</param>
    /// <returns>MsgBoxBtn</returns>
    public static MsgBoxBtn CreateMessageBoxBtn(string ButtonText, Action ifClicked, Color32 BackgroundColor, Color32 TextColor, bool noClosing = false)
    {
        GameObject btn = GameObject.Instantiate(messageBoxCv.messageBoxBtnPrefab);
        MsgBoxBtn msgBoxBtn = new MsgBoxBtn(ifClicked, btn.GetComponent<Button>(), noClosing);
        if (noClosing && ifClicked == null)
            ModConsole.Error("<b>CreateMessageBoxBtn()</b> - <b>ifClicked</b> cannot be null when <b>noClosing</b> is set to true");
        if (noClosing && ifClicked != null)
            msgBoxBtn.button.onClick.AddListener(() => { ifClicked(); });
        btn.GetComponentInChildren<Text>().text = ButtonText.ToUpper();
        btn.GetComponentInChildren<Text>().color = TextColor;
        btn.GetComponent<Image>().color = BackgroundColor;
        return msgBoxBtn;
    }
    /// <summary>
    /// Show Message Box with simple message
    /// </summary>
    /// <param name="message">Message content</param>
    public static void ShowMessage(string message) => ShowMessage(message, "Message");

    /// <summary>
    /// Show Message Box with simple message
    /// </summary>
    /// <param name="message">Message content</param>
    /// <param name="title">Title of message</param>
    public static void ShowMessage(string message, string title)
    {
        GameObject mb = GameObject.Instantiate(messageBoxCv.messageBoxPrefab);
        MessageBoxHelper mbh = mb.GetComponent<MessageBoxHelper>();
        MsgBoxBtn btn = CreateMessageBoxBtn("OK");
        btn.transform.SetParent(mbh.btnRow1.transform, false);
        btn.button.onClick.AddListener(() => GameObject.Destroy(mb));
        mbh.messageBoxTitle.text = title.ToUpper();
        mbh.messageBoxContent.text = message;
        mb.transform.SetParent(messageBoxCv.transform, false);
        mb.SetActive(true);
    }

    /// <summary>
    /// Show simple question message, and do something when user click yes.
    /// </summary>
    /// <param name="message">Message content</param>
    /// <param name="ifYes">What to do when user click yes</param>
    public static void ShowYesNoMessage(string message, Action ifYes) => ShowYesNoMessage(message, "Question", ifYes);

    /// <summary>
    /// Show simple question message, and do something when user click yes.
    /// </summary>
    /// <param name="message">Message content</param>
    /// <param name="title">Title of message</param>
    /// <param name="ifYes">What to do when user click yes</param>
    public static void ShowYesNoMessage(string message, string title, Action ifYes) => ShowYesNoMessage(message, title, ifYes, null);

    internal static void ShowYesNoMessage(string message, string title, Action ifYes, UnityAction ifYesUA = null)
    {
        GameObject mb = GameObject.Instantiate(messageBoxCv.messageBoxPrefab);
        MessageBoxHelper mbh = mb.GetComponent<MessageBoxHelper>();
        MsgBoxBtn btnYes = CreateMessageBoxBtn("YES");
        MsgBoxBtn btnNo = CreateMessageBoxBtn("NO");
        btnYes.transform.SetParent(mbh.btnRow1.transform, false);
        btnNo.transform.SetParent(mbh.btnRow1.transform, false);
        mbh.messageBoxTitle.text = title.ToUpper();
        mbh.messageBoxContent.text = message;
        if (ifYesUA != null)
            btnYes.button.onClick.AddListener(() => { ifYesUA?.Invoke(); GameObject.Destroy(mb); });
        else
            btnYes.button.onClick.AddListener(() => { ifYes.Invoke(); GameObject.Destroy(mb); });
        btnNo.button.onClick.AddListener(() => GameObject.Destroy(mb));
        mb.transform.SetParent(messageBoxCv.transform, false);
        mb.SetActive(true);
    }

    /// <summary>
    /// Show Custom message, you can create MessageBox with custom buttons using CreateMessageBoxBtn() first
    /// </summary>
    /// <param name="message">>Message content</param>
    /// <param name="title">Title of message</param>
    /// <param name="buttons">Buttons created using CreateMessageBoxBtn()</param>
    public static void ShowCustomMessage(string message, string title, MsgBoxBtn[] buttons)
    {
        ShowCustomMessage(message, title, buttons, null);
    }
    /// <summary>
    /// Show Custom message, you can create MessageBox with custom buttons using CreateMessageBoxBtn() first
    /// </summary>
    /// <param name="message">>Message content</param>
    /// <param name="title">Title of message</param>
    /// <param name="buttons">Buttons created using CreateMessageBoxBtn()</param>
    /// <param name="buttons2">A place to add "second row" of buttons.</param>
    public static void ShowCustomMessage(string message, string title, MsgBoxBtn[] buttons, MsgBoxBtn[] buttons2)
    {
        if (buttons == null || buttons.Length == 0)
        {
            ModConsole.Error("<b>ShowCustomMessage()</b> - MessageBox requires at least one button.");
            return;
        }
        GameObject mb = GameObject.Instantiate(messageBoxCv.messageBoxPrefab);
        MessageBoxHelper mbh = mb.GetComponent<MessageBoxHelper>();
        mbh.messageBoxTitle.text = title.ToUpper();
        mbh.messageBoxContent.text = message;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].IfClicked != null)
            {
                if (!buttons[i].noDestroy)
                {
                    buttons[i].button.onClick.AddListener(() => { buttons[i].IfClicked(); GameObject.Destroy(mb); });
                }
            }
            else
                buttons[i].button.onClick.AddListener(() => { GameObject.Destroy(mb); });
            buttons[i].transform.SetParent(mbh.btnRow1.transform, false);
        }

        if (buttons2 != null)
        {
            if (buttons2.Length > 0)
            {
                for (int i = 0; i < buttons2.Length; i++)
                {
                    if (buttons2[i].IfClicked != null)
                    {
                        if (!buttons2[i].noDestroy)
                        {
                            buttons2[i].button.onClick.AddListener(() => { buttons2[i].IfClicked(); GameObject.Destroy(mb); });
                        }
                    }
                    else
                        buttons2[i].button.onClick.AddListener(() => { GameObject.Destroy(mb); });
                    buttons2[i].transform.SetParent(mbh.btnRow2.transform, false);
                }
                mbh.btnRow2.SetActive(true);
            }
        }

        mb.transform.SetParent(messageBoxCv.transform, false);
        mb.SetActive(true);
    }

    internal static void ShowChangelogWindow(string content)
    {
        messageBoxCv.changelogText.text = content;
        messageBoxCv.changelogWindow.SetActive(true);
        messageBoxCv.changelogWindow.transform.SetAsLastSibling();
    }
}
#endif