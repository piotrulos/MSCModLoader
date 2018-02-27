//Replace this with Asset Bundles for simplicity

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MSCLoader
{
#pragma warning disable CS1591
    public class ModUI
    {
        public static GameObject messageBox;
        
        //canvas for UI
        public static void CreateCanvas()
        {
            GameObject canvasGO = new GameObject();
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

        public static void ShowMessage(string message) => ShowMessage(message, "Message");

        public static void ShowMessage(string message, string title)
        {
            GameObject mb = GameObject.Instantiate(messageBox);
            mb.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = title;
            mb.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = message;
            mb.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
            mb.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Button>().onClick.AddListener(() => GameObject.Destroy(mb));
            mb.transform.SetParent(GameObject.Find("MSCLoader Canvas").transform, false);
            mb.SetActive(true);
        }

        public static void ShowYesNoMessage(string message, Action ifYes) => ShowYesNoMessage(message, "Message", ifYes);

        public static void ShowYesNoMessage(string message, string title, Action ifYes)
        {
            GameObject mb = GameObject.Instantiate(messageBox); 
            mb.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = title;
            mb.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = message;
            mb.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
            mb.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<Button>().onClick.AddListener(() => { ifYes.Invoke(); GameObject.Destroy(mb); });
            mb.transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<Button>().onClick.AddListener(() =>  GameObject.Destroy(mb));
            mb.transform.SetParent(GameObject.Find("MSCLoader Canvas").transform, false);
            mb.SetActive(true);
        }
    }
#pragma warning restore CS1591
}
