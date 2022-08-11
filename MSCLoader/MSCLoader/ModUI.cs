using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MSCLoader
{
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
        internal static GameObject canvasPrefab;
        private static GameObject msclCanv, modMenuCanv, dwnlMenuCanv, consoleCanv;

        internal static void CreateCanvases()
        {
            msclCanv = CreateCanvas("MSCLoader Canvas", true);
            modMenuCanv = CreateCanvas("MSCLoader Canvas menu", true);
            dwnlMenuCanv = CreateCanvas("MSCLoader Canvas Download Menu", false);
            consoleCanv = CreateCanvas("MSCLoader Canvas console", true);

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
            switch (type)
            {
                case 0:
                    return msclCanv;
                case 1:
                    return modMenuCanv;
                case 2:
                    return consoleCanv;
                case 5:
                    return dwnlMenuCanv;
                default:
                    return msclCanv;
            }
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
        /// <param name="ifClicked">Action if button was clicked (leave null for just closing meesage box)</param>
        /// <param name="noClosing">Don't close MessageBox when action is triggered</param>
        /// <returns>MsgBoxBtn</returns>
        public static MsgBoxBtn CreateMessageBoxBtn(string ButtonText, Action ifClicked = null, bool noClosing = false)
        {
            GameObject btn = GameObject.Instantiate(messageBoxCv.messageBoxBtnPrefab);
            MsgBoxBtn msgBoxBtn = new MsgBoxBtn(ifClicked, btn.GetComponent<Button>(), noClosing);
            if (noClosing && ifClicked == null)
                ModConsole.Error("<b>CreateMessageBoxBtn()</b> - <b>ifClicked</b> cannot be null when <b>noClosing</b> is set to true");
            if(noClosing && ifClicked != null)
                msgBoxBtn.button.onClick.AddListener(() => { ifClicked(); });
            btn.GetComponentInChildren<Text>().text = ButtonText.ToUpper();
            return msgBoxBtn;
        }

        /// <summary>
        /// Create Message Box Button for ShowCustomMessage(...);
        /// </summary>
        /// <param name="ButtonText">Text on the Button</param>
        /// <param name="ifClicked">Action if button was clicked (leave null for just closing meesage box)</param>
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
        /// Show simple retry/cancel message, and do something when user click retry.
        /// </summary>
        /// <param name="message">>Message content</param>
        /// <param name="title">Title of message</param>
        /// <param name="ifRetry">What to do when user click retry</param>
        public static void ShowRetryCancelMessage(string message, string title, Action ifRetry) => ShowRetryCancelMessage(message, title, ifRetry, null);

        internal static void ShowRetryCancelMessage(string message, string title, Action ifRetry, UnityAction ifRetryUA = null)
        {
            GameObject mb = GameObject.Instantiate(messageBoxCv.messageBoxPrefab);
            MessageBoxHelper mbh = mb.GetComponent<MessageBoxHelper>();
            MsgBoxBtn btnRt = CreateMessageBoxBtn("RETRY");
            MsgBoxBtn btnCn = CreateMessageBoxBtn("CANCEL");
            btnRt.transform.SetParent(mbh.btnRow1.transform, false);
            btnCn.transform.SetParent(mbh.btnRow1.transform, false);
            mbh.messageBoxTitle.text = title.ToUpper();
            mbh.messageBoxContent.text = message;
            if (ifRetryUA != null)
                btnRt.button.onClick.AddListener(() => { ifRetryUA?.Invoke(); GameObject.Destroy(mb); });
            else
                btnRt.button.onClick.AddListener(() => { ifRetry.Invoke(); GameObject.Destroy(mb); });
            btnCn.button.onClick.AddListener(() => GameObject.Destroy(mb));
            mb.transform.SetParent(messageBoxCv.transform, false);
            mb.SetActive(true);
        }
        /// <summary>
        /// Show simple Continue/Abort message, and do something when user click Continue.
        /// </summary>
        /// <param name="message">>Message content</param>
        /// <param name="title">Title of message</param>
        /// <param name="ifContinue">>What to do when user click Continue</param>
        public static void ShowContinueAbortMessage(string message, string title, Action ifContinue) => ShowContinueAbortMessage(message, title, ifContinue, null);

        internal static void ShowContinueAbortMessage(string message, string title, Action ifContinue, UnityAction ifContinueUA = null)
        {
            GameObject mb = GameObject.Instantiate(messageBoxCv.messageBoxPrefab);
            MessageBoxHelper mbh = mb.GetComponent<MessageBoxHelper>();
            MsgBoxBtn btnCont = CreateMessageBoxBtn("CONTINUE");
            MsgBoxBtn btnAb = CreateMessageBoxBtn("ABORT");
            btnCont.transform.SetParent(mbh.btnRow1.transform, false);
            btnAb.transform.SetParent(mbh.btnRow1.transform, false);
            mbh.messageBoxTitle.text = title.ToUpper();
            mbh.messageBoxContent.text = message;
            if (ifContinueUA != null)
                btnCont.button.onClick.AddListener(() => { ifContinueUA?.Invoke(); GameObject.Destroy(mb); });
            else
                btnCont.button.onClick.AddListener(() => { ifContinue.Invoke(); GameObject.Destroy(mb); });
            btnAb.button.onClick.AddListener(() => GameObject.Destroy(mb));
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
            if(buttons == null)
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
                buttons[i].button.onClick.AddListener(() => { buttons[i].IfClicked(); GameObject.Destroy(mb); });
                buttons[i].transform.SetParent(mbh.btnRow1.transform, false);
            }
            mb.transform.SetParent(messageBoxCv.transform, false);
            mb.SetActive(true);
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
                        Action act = buttons[i].IfClicked;
                        buttons[i].button.onClick.AddListener(() => { act(); GameObject.Destroy(mb); });
                    }
                }
                else
                    buttons[i].button.onClick.AddListener(() => { GameObject.Destroy(mb); });
                buttons[i].transform.SetParent(mbh.btnRow1.transform, false);
            }
            if (buttons2.Length > 0)
            {
                for (int i = 0; i < buttons2.Length; i++)
                {
                    if (buttons2[i].IfClicked != null)
                    {
                        if (!buttons2[i].noDestroy)
                        {
                            Action act = buttons2[i].IfClicked;
                            buttons2[i].button.onClick.AddListener(() => { act(); GameObject.Destroy(mb); });
                        }
                    }
                    else
                        buttons2[i].button.onClick.AddListener(() => { GameObject.Destroy(mb); });
                    buttons2[i].transform.SetParent(mbh.btnRow2.transform, false);
                }
                mbh.btnRow2.SetActive(true);
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

}
