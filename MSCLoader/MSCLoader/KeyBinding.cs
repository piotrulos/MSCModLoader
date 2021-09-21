using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    internal class KeybindList
    {
        public List<Keybinds> keybinds = new List<Keybinds>();
    }
    internal class Keybinds
    {
        public string ID { get; set; }
        public KeyCode Key { get; set; }
        public KeyCode Modifier { get; set; }
    }
    internal class KeyBinding : MonoBehaviour
    {
        public Text KeybindName;

        public KeyCode modifierKey;
        public Text modifierDisplay;
        public Button modifierButton;
        public Image buttonModImage;

        public KeyCode key;
        public Text keyDisplay;
        public Button keyButton;
        public Image buttonImage;

        bool reassignKey = false;
        bool ismodifier = false;
        
        Color toggleColor = new Color32(101, 130, 18, 255);
        Color originalColor = new Color32(101, 34, 18, 255);

        public Mod mod;
        public string id;

        public void LoadBind(Keybind kb, Mod m)
        {
            mod = m;
            KeybindName.text = kb.Name;
            id = kb.ID;
            modifierKey = kb.Modifier;
            key = kb.Key;
            modifierButton.onClick.AddListener(() => ChangeKeyCode(true, true));
            keyButton.onClick.AddListener(() => ChangeKeyCode(true, false));

            modifierDisplay.text = modifierKey.ToString().ToUpper();
            keyDisplay.text = key.ToString().ToUpper();
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
            Keybind bind = mod.Keybinds.Find(x => x.ID == id);
            if (modifier)
            {
                bind.Modifier = kcode;
                modifierDisplay.text = kcode.ToString().ToUpper();
            }
            else
            {
                bind.Key = kcode;
                keyDisplay.text = kcode.ToString().ToUpper();
            }
            ModMenu.SaveModBinds(mod);
        }

    }
}
