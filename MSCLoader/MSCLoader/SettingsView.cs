using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MSCLoader
{
    public class SettingsView : MonoBehaviour
    {
        public GameObject settingView;
        public GameObject settingViewContainer;
        public GameObject modList;
        public GameObject modView;
        public GameObject modSettings;
        public GameObject ModSettingsView;
        public GameObject goBackBtn;
        public GameObject keybindsList;

        public Text IDtxt;
        public Text Nametxt;
        public Text Versiontxt;
        public Text Authortxt;

        Mod selected;

        public void modButton(string name,string version,string author, Mod mod)
        {
            GameObject modButton = ModUI.CreateUIBase("ModButton", modView);
            modButton.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            modButton.AddComponent<Image>().color = Color.white;
            modButton.AddComponent<LayoutElement>().minHeight = 50;
            modButton.AddComponent<Button>().targetGraphic = modButton.GetComponent<Image>();
            ColorBlock cb = modButton.GetComponent<Button>().colors;
            cb.normalColor = new Color32(0x00, 0xB4, 0xFF, 0x64); //00B4FF64
            cb.highlightedColor = new Color32(0x00, 0xFF, 0x2B, 0x64); //00FF2B64
            modButton.GetComponent<Button>().colors = cb;
            modButton.AddComponent<ModInfo>().mod = mod;

            GameObject modName = ModUI.CreateTextBlock("Name",name, modButton, TextAnchor.UpperLeft,Color.green);
            modName.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            modName.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            modName.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            modName.GetComponent<RectTransform>().anchoredPosition = new Vector2(2, 0);
            modName.GetComponent<Text>().fontStyle = FontStyle.Bold;
            modName.GetComponent<Text>().fontSize = 18;
            modName.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            modName.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            GameObject modVersion = ModUI.CreateTextBlock("Version", string.Format("v{0}",version), modButton, TextAnchor.LowerLeft, Color.yellow);
            modVersion.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            modVersion.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            modVersion.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            modVersion.GetComponent<RectTransform>().anchoredPosition = new Vector2(2, 0);
            modVersion.GetComponent<Text>().fontSize = 16;
            modVersion.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            modVersion.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            GameObject modAuthor = ModUI.CreateTextBlock("Author", author, modButton, TextAnchor.LowerRight, Color.yellow);
            modAuthor.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
            modAuthor.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
            modAuthor.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
            modAuthor.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2, 0);
            modAuthor.GetComponent<Text>().fontSize = 16;
            modAuthor.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            modAuthor.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            modButton.GetComponent<Button>().onClick.AddListener(() => settingView.GetComponent<SettingsView>().selectMod());

        }

        public void KeyBindsList(string name, KeyCode modifier, KeyCode key, string ID)
        {
            
            GameObject keyBind = ModUI.CreateUIBase("KeyBind", keybindsList);
            keyBind.AddComponent<LayoutElement>().preferredHeight = 35;
            keyBind.AddComponent<KeyBinding>();
            GameObject keyName = ModUI.CreateTextBlock("Key Name", name, keyBind,TextAnchor.MiddleLeft);
            keyName.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
            keyName.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
            keyName.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
            keyName.GetComponent<RectTransform>().sizeDelta = new Vector2(290, 17);

            GameObject keyButton = ModUI.CreateUIBase("Modifier Button", keyBind);
            keyButton.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            keyButton.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
            keyButton.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            keyButton.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 17);
            keyButton.AddComponent<Image>();
            keyButton.AddComponent<Button>().targetGraphic = keyButton.GetComponent<Image>();
            ColorBlock cb = keyButton.GetComponent<Button>().colors;
            cb.normalColor = new Color32(0x14, 0x14, 0x14, 0xFF);
            cb.highlightedColor = new Color32(0x50, 0x50, 0x50, 0xFF);
            cb.pressedColor = new Color32(0x64, 0x64, 0x64, 0xFF);
            keyButton.GetComponent<Button>().colors = cb;
            keyButton.GetComponent<Button>().targetGraphic = keyButton.GetComponent<Image>();

            GameObject BtnTxt = ModUI.CreateTextBlock("Text", "", keyButton, TextAnchor.MiddleCenter, Color.white);
            BtnTxt.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            BtnTxt.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            BtnTxt.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            BtnTxt.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 17);

            keyBind.GetComponent<KeyBinding>().modifierButton = keyButton;
            keyBind.GetComponent<KeyBinding>().modifierDisplay = BtnTxt.GetComponent<Text>();

            GameObject plus = ModUI.CreateTextBlock("Text", "+", keyBind, TextAnchor.MiddleCenter, Color.white);
            plus.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
            plus.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
            plus.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            plus.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 17);
            plus.GetComponent<Text>().fontStyle = FontStyle.Bold;
            plus.GetComponent<Text>().resizeTextForBestFit = true;

            GameObject keyButton2 = ModUI.CreateUIBase("Keybind Button", keyBind);
            keyButton2.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
            keyButton2.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
            keyButton2.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
            keyButton2.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 17);
            keyButton2.AddComponent<Image>();
            keyButton2.AddComponent<Button>().targetGraphic = keyButton2.GetComponent<Image>();
            keyButton2.GetComponent<Button>().colors = cb;
            keyButton2.GetComponent<Button>().targetGraphic = keyButton2.GetComponent<Image>();

            GameObject BtnTxt2 = ModUI.CreateTextBlock("Text", "", keyButton2, TextAnchor.MiddleCenter, Color.white);
            BtnTxt2.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            BtnTxt2.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            BtnTxt2.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            BtnTxt2.GetComponent<RectTransform>().sizeDelta = new Vector2(140, 17);

            keyBind.GetComponent<KeyBinding>().keyButton = keyButton2;
            keyBind.GetComponent<KeyBinding>().keyDisplay = BtnTxt2.GetComponent<Text>();
            keyBind.GetComponent<KeyBinding>().key = key;
            keyBind.GetComponent<KeyBinding>().modifierKey = modifier;
            keyBind.GetComponent<KeyBinding>().mod = selected;
            keyBind.GetComponent<KeyBinding>().id = ID ;
            keyBind.GetComponent<KeyBinding>().LoadBind();

        }
        public void RemoveChildren(Transform parent) //clear 
        {
            foreach (Transform child in parent)
                Destroy(child.gameObject);
        }
        public void goBack()
        {
            modList.SetActive(true);
            modSettings.SetActive(false);
            goBackBtn.SetActive(false);
        }

        public void selectMod()
        {
            selected = EventSystem.current.currentSelectedGameObject.GetComponent<ModInfo>().mod;
            goBackBtn.SetActive(true);
            modList.SetActive(false);
            modSettings.SetActive(true);
            IDtxt.text = string.Format("ID: <b>{0}</b>", selected.ID);
            Nametxt.text = string.Format("Name: <b>{0}</b>", selected.Name);
            Versiontxt.text = string.Format("Version: <b>{0}</b>", selected.Version);
            Authortxt.text = string.Format("Author: <b>{0}</b>", selected.Author);
            RemoveChildren(keybindsList.transform);
            foreach (Keybind key in Keybind.Keybinds)
            {
                if (key.Mod == selected)
                {
                    KeyBindsList(key.Name, key.Modifier, key.Key, key.ID);
                }
                else
                {
                    //no keybinds
                }
            }
            //ModConsole.Print(EventSystem.current.currentSelectedGameObject.GetComponent<ModInfo>().mod.ID); //debug
        }
        public void toggleVisibility()
        {
            if (!settingViewContainer.activeSelf)
            {
                RemoveChildren(modView.transform);
                setVisibility(!settingViewContainer.activeSelf);
                foreach (Mod mod in ModLoader.LoadedMods)
                {
                    modButton(mod.Name, mod.Version, mod.Author, mod);
                }
            }
            else
            {
                setVisibility(!settingViewContainer.activeSelf);
            }
        }

        public void setVisibility(bool visible)
        {
            settingViewContainer.SetActive(visible);
        }
    }
}
