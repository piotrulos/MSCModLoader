using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MSCLoader
{
    /// <summary>
    /// UI stuff
    /// </summary>
    public class ModUI
    {
#pragma warning disable CS1591
        internal static GameObject messageBox;
        internal static GameObject messageBoxBtn;
        private static GameObject canvasGO;

        public static void CreateCanvas()
        {
            canvasGO = new GameObject();
            canvasGO.name = "MSCLoader Canvas";
            canvasGO.layer = 5;
            canvasGO.AddComponent<Canvas>();
            canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1280f, 720f);
            canvasGO.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            canvasGO.GetComponent<CanvasScaler>().referencePixelsPerUnit = 100;
            canvasGO.AddComponent<GraphicRaycaster>();
            GameObject.DontDestroyOnLoad(canvasGO);

            //create EventSystem
            GameObject evSys = new GameObject();
            evSys.name = "EventSystem";
            evSys.AddComponent<EventSystem>();
            evSys.AddComponent<StandaloneInputModule>();
            GameObject.DontDestroyOnLoad(evSys);
        }
#pragma warning restore CS1591

        /// <summary>
        /// Get UI canvas
        /// </summary>
        /// <returns>Canvas GameObject</returns>
        public static GameObject GetCanvas() => canvasGO;
    
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
            mb.transform.SetParent(GetCanvas().transform, false);
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
        /// <summary>
        /// Show simple question message, and do something when user click yes.
        /// </summary>
        /// <param name="message">Message content</param>
        /// <param name="title">Title of message</param>
        /// <param name="ifYes">What to do when user click yes</param>
        /// <param name="ifYesUA">What to do when user click yes (UnityAction)</param>
        public static void ShowYesNoMessage(string message, string title, Action ifYes, UnityAction ifYesUA = null)
        {
            GameObject mb = GameObject.Instantiate(messageBox);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            mb.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = title.ToUpper();
            mb.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = message;
            if (ifYesUA != null)
                mb.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ifYesUA?.Invoke(); GameObject.Destroy(mb); });
            else
                mb.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ifYes.Invoke(); GameObject.Destroy(mb); });
            mb.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "YES";
            mb.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() => GameObject.Destroy(mb));
            mb.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = "NO";
            mb.transform.SetParent(GetCanvas().transform, false);
            mb.SetActive(true);
        }
        /// <summary>
        /// Show simple retry/cancel message, and do something when user click retry.
        /// </summary>
        /// <param name="message">>Message content</param>
        /// <param name="title">Title of message</param>
        /// <param name="ifRetry">What to do when user click retry</param>
        /// <param name="ifRetryUA">What to do when user click retry (UnityAction)</param>
        public static void ShowRetryCancelMessage(string message, string title, Action ifRetry, UnityAction ifRetryUA = null)
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
            mb.transform.SetParent(GetCanvas().transform, false);
            mb.SetActive(true);
        }
        /// <summary>
        /// Show simple Continue/Abort message, and do something when user click Continue.
        /// </summary>
        /// <param name="message">>Message content</param>
        /// <param name="title">Title of message</param>
        /// <param name="ifContinue">>What to do when user click Continue</param>
        /// <param name="ifContinueUA">What to do when user click Continue (UnityAction)</param>
        public static void ShowContinueAbortMessage(string message, string title, Action ifContinue, UnityAction ifContinueUA = null)
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
            mb.transform.SetParent(GetCanvas().transform, false);
            mb.SetActive(true);
        }
    }

}
