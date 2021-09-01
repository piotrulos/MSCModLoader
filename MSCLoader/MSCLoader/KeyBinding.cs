using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
#pragma warning disable CS1591
    public class KeybindList
    {
        public List<Keybinds> keybinds = new List<Keybinds>();
    }
    public class Keybinds
    {
        public string ID { get; set; }
        public KeyCode Key { get; set; }
        public KeyCode Modifier { get; set; }
    }
    public class KeyBinding : MonoBehaviour
    {
        public KeyCode modifierKey;
        public Text modifierDisplay;
        public GameObject modifierButton;
        Image buttonModImage;

        public KeyCode key;
        public Text keyDisplay;
        public GameObject keyButton;
        Image buttonImage;

        bool reassignKey = false;
        bool ismodifier = false;
        public Color toggleColor = new Color32(0xFF, 0xFF, 0x00, 0xFF);

        Color originalColor = Color.white;

        public Mod mod;
        public string id;

        public void LoadBind()
        {
            buttonModImage = modifierButton.GetComponent<Image>();
            buttonImage = keyButton.GetComponent<Image>();

            modifierButton.GetComponent<Button>().onClick.AddListener(() => ChangeKeyCode(true, true));
            keyButton.GetComponent<Button>().onClick.AddListener(() => ChangeKeyCode(true, false));

            modifierDisplay.text = modifierKey.ToString();
            keyDisplay.text = key.ToString();
        }

        void Update()
        {
            if (reassignKey)
            {
                //Checks if key is pressed and if button has been pressed indicating wanting to re-assign
                if (Input.anyKeyDown)
                {
                    KeyCode[] keyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
                    for (int i = 0; i < keyCodes.Length; i++)
                    {
                        if (Input.GetKeyDown(keyCodes[i]))
                        {
                            if (keyCodes[i] != KeyCode.Mouse0 && keyCodes[i] != KeyCode.Mouse1) //LMB = cancel
                            {
                                UpdateKeyCode(keyCodes[i], ismodifier);
                            }
                            if (keyCodes[i] == KeyCode.Mouse1) //RMB = sets to none
                            {
                                UpdateKeyCode(KeyCode.None, ismodifier);
                            }
                            ChangeKeyCode(false, ismodifier);
                            break;
                        }
                    }

                }
            }
        }

        public void ChangeKeyCode(bool toggle, bool modifier)
        {
            reassignKey = toggle;
            ismodifier = modifier;
            if (toggle)
            {
                if (modifier)
                    buttonModImage.color = toggleColor;
                else
                    buttonImage.color = toggleColor;
            }
            else
            {
                if (modifier)
                    buttonModImage.color = originalColor;
                else
                    buttonImage.color = originalColor;
            }
        }
        void UpdateKeyCode(KeyCode kcode, bool modifier)
        {
            Keybind bind = Keybind.Keybinds.Find(x => x.Mod == mod && x.ID == id);
            if (modifier)
            {
                bind.Modifier = kcode;
                modifierDisplay.text = kcode.ToString();
            }
            else
            {
                bind.Key = kcode;
                keyDisplay.text = kcode.ToString();
            }
            ModMenu.SaveModBinds(mod);
        }

    }
#pragma warning restore CS1591
}
