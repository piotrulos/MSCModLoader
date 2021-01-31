using System;
using UnityEngine;
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
        public static void ShowYesNoMessage(string message, string title, Action ifYes)
        {
            GameObject mb = GameObject.Instantiate(messageBox);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            GameObject.Instantiate(messageBoxBtn).transform.SetParent(mb.transform.GetChild(0).GetChild(2), false);
            mb.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = title.ToUpper();
            mb.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = message;

            mb.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ifYes.Invoke(); GameObject.Destroy(mb); });
            mb.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = "YES";
            mb.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() =>  GameObject.Destroy(mb));
            mb.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = "NO";
            mb.transform.SetParent(GetCanvas().transform, false);
            mb.SetActive(true);
        }
    }

}
