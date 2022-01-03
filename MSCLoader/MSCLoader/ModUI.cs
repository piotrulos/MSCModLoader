using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MSCLoader
{
    /// <summary>
    /// UI elements like creating Message Boxes
    /// </summary>
    public class ModUI
    {
        internal static GameObject messageBox;
        internal static GameObject messageBoxBtn;
        internal static GameObject canvasPrefab;
        private static GameObject msclCanv, msgboxCanv, lodadingCanv, modMenuCanv, dwnlMenuCanv, consoleCanv;

        internal static void CreateCanvases()
        {
            msclCanv = CreateCanvas("MSCLoader Canvas", true);
            modMenuCanv = CreateCanvas("MSCLoader Canvas menu", true);
            dwnlMenuCanv = CreateCanvas("MSCLoader Canvas Download Menu", false);
            consoleCanv = CreateCanvas("MSCLoader Canvas console", true);
            msgboxCanv = CreateCanvas("MSCLoader Canvas msgbox", true);
            msgboxCanv.GetComponent<Canvas>().sortingOrder = 1;
            lodadingCanv = CreateCanvas("MSCLoader Canvas loading", true);
            lodadingCanv.GetComponent<Canvas>().sortingOrder = 2;

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
                case 3:
                    return lodadingCanv;
                case 4:
                    return msgboxCanv;
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
            GameObject mb = GameObject.Instantiate(messageBox);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            mb.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = title.ToUpper();
            mb.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = message;            
            mb.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => GameObject.Destroy(mb));
            mb.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "OK";
            mb.transform.SetParent(GetCanvas(4).transform, false);
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
            GameObject mb = GameObject.Instantiate(messageBox);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            mb.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = title.ToUpper();
            mb.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = message;
        //    mb.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
            if (ifYesUA != null)
                mb.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ifYesUA?.Invoke(); GameObject.Destroy(mb); });
            else
                mb.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ifYes.Invoke(); GameObject.Destroy(mb); });
            mb.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "YES";
            mb.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => GameObject.Destroy(mb));
            mb.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = "NO";
            mb.transform.SetParent(GetCanvas(4).transform, false);
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
            GameObject mb = GameObject.Instantiate(messageBox);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            mb.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = title.ToUpper();
            mb.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = message;
            if (ifRetryUA != null)
                mb.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ifRetryUA?.Invoke(); GameObject.Destroy(mb); });
            else
                mb.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ifRetry.Invoke(); GameObject.Destroy(mb); });
            mb.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "RETRY";
            mb.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => GameObject.Destroy(mb));
            mb.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = "CANCEL";
            mb.transform.SetParent(GetCanvas(4).transform, false);
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
            GameObject mb = GameObject.Instantiate(messageBox);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            mb.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = title.ToUpper();
            mb.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = message;
            if (ifContinueUA != null)
                mb.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ifContinueUA?.Invoke(); GameObject.Destroy(mb); });
            else
                mb.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ifContinue.Invoke(); GameObject.Destroy(mb); });
            mb.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "CONTINUE";
            mb.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => GameObject.Destroy(mb));
            mb.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = "ABORT";
            mb.transform.SetParent(GetCanvas(4).transform, false);
            mb.SetActive(true);
        }
    }

}
