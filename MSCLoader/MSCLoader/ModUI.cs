using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public static void ShowYesNoMessage(string message, string title, Action ifYes)
    {
        GameObject mb = GameObject.Instantiate(messageBoxCv.messageBoxPrefab);
        MessageBoxHelper mbh = mb.GetComponent<MessageBoxHelper>();
        MsgBoxBtn btnYes = CreateMessageBoxBtn("YES");
        MsgBoxBtn btnNo = CreateMessageBoxBtn("NO");
        btnYes.transform.SetParent(mbh.btnRow1.transform, false);
        btnNo.transform.SetParent(mbh.btnRow1.transform, false);
        mbh.messageBoxTitle.text = title.ToUpper();
        mbh.messageBoxContent.text = message;
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
    /// <summary>
    /// Create Popup Setting Window
    /// </summary>
    /// <param name="title">Window title</param>
    /// <param name="submitButtonText">Submit button text</param>
    /// <returns>PopupSetting</returns>
    public static PopupSetting CreatePopupSetting(string title, string submitButtonText) => new PopupSetting(title, submitButtonText);

    /// <summary>
    /// Parse popup response by deserializing it.
    /// </summary>
    /// <typeparam name="T">Your class</typeparam>
    /// <param name="response">Popup response</param>
    /// <returns>Deserialized response</returns>
    public static T ParsePopupResponse<T>(string response) where T : new()
    {
        return JsonConvert.DeserializeObject<T>(response);
    }

}

/// <summary>
/// Popup Setting Window
/// </summary>
public class PopupSetting
{
    internal string WindowTitle = "Popup Setting".ToUpper();
    internal string SubmitButtonText = "Confirm".ToUpper();
    internal List<ModSetting> settingElements = new List<ModSetting>();
    internal PopupSetting(string title, string buttonText)
    {
        WindowTitle = title;
        SubmitButtonText = buttonText;
    }
    internal string ReturnJsonString()
    {
        List<string> json = new List<string>();
        foreach (ModSetting s in settingElements)
        {
            switch (s.SettingType)
            {
                case SettingsType.CheckBox:
                    SettingsCheckBox ss = (SettingsCheckBox)s;
                    json.Add($"\"{ss.ID}\":{ss.GetValue()}");
                    break;
                case SettingsType.Slider:
                    SettingsSlider ss2 = (SettingsSlider)s;
                    json.Add($"\"{ss2.ID}\":{ss2.GetValue()}");
                    break;
                case SettingsType.SliderInt:
                    SettingsSliderInt ss3 = (SettingsSliderInt)s;
                    json.Add($"\"{ss3.ID}\":{ss3.GetValue()}");
                    break;
                case SettingsType.TextBox:
                    SettingsTextBox ss4 = (SettingsTextBox)s;
                    json.Add($"\"{ss4.ID}\":\"{ss4.GetValue()}\"");
                    break;
                case SettingsType.DropDown:
                    SettingsDropDownList ss5 = (SettingsDropDownList)s;
                    json.Add($"\"{ss5.ID}\":{ss5.GetSelectedItemIndex()}");
                    break;
                default:
                    break;
            }
        }

        return "{" + $"{string.Join(",", json.ToArray())}" + "}";
    }
    /// <summary>
    /// Show popup window
    /// </summary>
    /// <param name="OnSubmit">This will be called once confirm button is pressed to submit result</param>
    public void ShowPopup(Action<string> OnSubmit) => ModUI.popupSettingController.CreatePopupSetting(this, OnSubmit);

    /// <summary>
    /// Add just a text (doesn't return anything on confirm)
    /// </summary>
    /// <param name="text">Just a text (supports unity rich text)</param>
    public void AddText(string text)
    {
        SettingsText s = new SettingsText(text, true);
        settingElements.Add(s);
    }

    /// <summary>
    /// Add button to popup menu (doesn't return anything on confirm)
    /// </summary>
    /// <param name="name">Button name</param>
    /// <param name="onClick">Do something when button is clicked</param>
    /// <param name="predefinedIcon">Add optional icon</param>
    public void AddButton(string name, Action onClick, SettingsButton.ButtonIcon predefinedIcon = SettingsButton.ButtonIcon.None)
    {
        SettingsButton s = new SettingsButton(name, onClick, Color.black, Color.white, true, predefinedIcon, null);
        settingElements.Add(s);
    }

    /// <summary>
    /// Add checkbox to popup menu (bool value returns on confirm)
    /// </summary>
    /// <param name="settingID">Unique settings ID for your mod</param>
    /// <param name="name">Name of the setting</param>
    /// <param name="value">Default Value for this setting</param>
    public void AddCheckBox(string settingID, string name, bool value = false)
    {
        SettingsCheckBox s = new SettingsCheckBox(settingID, name, value, null, true);
        settingElements.Add(s);
    }

    /// <summary>
    /// Add Integer Slider to popup menu (int value returns on confirm)
    /// </summary>
    /// <param name="settingID">Unique settings ID for your mod</param>
    /// <param name="name">Name of the setting</param>
    /// <param name="minValue">minimum int value</param>
    /// <param name="maxValue">maximum int value</param>
    /// <param name="value">Default Value for this setting</param>
    /// <param name="textValues">Optional text values array (array index = slider value)</param>
    public void AddSlider(string settingID, string name, int minValue, int maxValue, int value = 0, string[] textValues = null)
    {

        if (textValues != null && textValues.Length <= (maxValue - minValue))
        {
            ModConsole.Error($"[<b>{settingID}</b>] AddSlider() on popup menu error: textValues array is smaller than slider range (min to max).");
        }
        SettingsSliderInt s = new SettingsSliderInt(settingID, name, value, minValue, maxValue, null, textValues, true);
        settingElements.Add(s);
    }

    /// <summary>
    /// Add Slider to popup menu (float value returns on confirm)
    /// </summary>
    /// <param name="settingID">Unique settings ID for your mod</param>
    /// <param name="name">Name of the setting</param>
    /// <param name="minValue">minimum float value</param>
    /// <param name="maxValue">maximum float value</param>
    /// <param name="value">Default Value for this setting</param>
    /// <param name="decimalPoints">Round value to number of decimal points</param>
    public void AddSlider(string settingID, string name, float minValue, float maxValue, float value = 0f, int decimalPoints = 2)
    {

        if (decimalPoints < 0)
        {
            ModConsole.Error($"[<b>{settingID}</b>] AddSlider()  on popup menu error: decimalPoints cannot be negative (defaulting to 2)");
            decimalPoints = 2;
        }
        SettingsSlider s = new SettingsSlider(settingID, name, value, minValue, maxValue, null, decimalPoints, true);
        settingElements.Add(s);
    }

    /// <summary>
    /// Add TextBox where user can type any text (string value returns on confirm)
    /// </summary>
    /// <param name="settingID">Your unique settings ID</param>
    /// <param name="name">Name of text box</param>
    /// <param name="value">Default TextBox value</param>
    /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
    public void AddTextBox(string settingID, string name, string value, string placeholderText) => AddTextBox(settingID, name, value, placeholderText, InputField.ContentType.Standard);

    /// <summary>
    /// Add TextBox where user can type any text (string value returns on confirm)
    /// </summary>
    /// <param name="settingID">Your unique settings ID</param>
    /// <param name="name">Name of text box</param>
    /// <param name="value">Default TextBox value</param>
    /// <param name="placeholderText">Placeholder text (like "Enter text...")</param>
    /// <param name="contentType">InputField content type</param>
    public void AddTextBox(string settingID, string name, string value, string placeholderText, InputField.ContentType contentType)
    {
        SettingsTextBox s = new SettingsTextBox(settingID, name, value, placeholderText, contentType, true);
        settingElements.Add(s);
    }

    /// <summary>
    /// Add DropDown List to popup menu (int [array index] value returns on confirm)
    /// </summary>
    /// <param name="settingID">unique settings ID</param>
    /// <param name="name">Name of the dropdown list</param>
    /// <param name="arrayOfItems">array of items that will be displayed in list</param>
    /// <param name="defaultSelected">default selected Index ID (default 0)</param>
    /// <param name="OnSelectionChanged">Action when item is selected</param>
    /// <param name="visibleByDefault">Visible by default (default=true)</param>
    /// <returns>SettingsDropDownList</returns>
    public void AddDropDownList(string settingID, string name, string[] arrayOfItems, int defaultSelected = 0)
    {
        SettingsDropDownList s = new SettingsDropDownList(settingID, name, arrayOfItems, defaultSelected, null, true);
        settingElements.Add(s);
    }

}
#endif