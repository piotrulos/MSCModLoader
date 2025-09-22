using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MSCLoader;

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
    public Text KeybindName, KeybindText;
    public GameObject Buttons, ButtonsR;

    bool reassignKey = false;
    bool ismodifier = false;

    public Mod mod;
#if !Mini
    private SettingsKeybind keyb;
#endif
    public void ChangeKeybind(bool modifier)
    {
        #if !Mini
        ChangeKeyCode(true, modifier);
        #endif
    }

    public void ResetToDefault()
    {
#if !Mini
        ModUI.ShowYesNoMessage($"This will reset keybind to default{Environment.NewLine}Default keybind is: <color=yellow>{(keyb.DefaultKeybModif == KeyCode.None ? keyb.DefaultKeybKey.ToString().ToUpper() : $"{keyb.DefaultKeybModif.ToString().ToUpper()} + {FriendlyBindName(keyb.DefaultKeybKey.ToString()).ToUpper()}")}</color>{Environment.NewLine}Do you want to continue?", "Reset Keybind", delegate
        {
            keyb.ResetToDefault();
            ModMenu.SaveModBinds(mod);
            ChangeKeyCode(false, ismodifier);
        });
#endif
    }
    public void CancelReassign()
    {
        #if !Mini
        ChangeKeyCode(false, ismodifier);
        #endif
    }

    public void SetToNone()
    {
        #if !Mini
        if (keyb.KeybModif != KeyCode.None && !ismodifier)
        {
            KeybindError(false);
            return;
        }
        UpdateKeyCode(KeyCode.None, ismodifier);
        #endif
    }

#if !Mini
    public void LoadBind(SettingsKeybind kb, Mod m)
    {
        mod = m;
        keyb = kb;
        KeybindName.text = kb.Name;
        KeybindText.text = kb.KeybModif == KeyCode.None ? FriendlyBindName(kb.KeybKey.ToString()).ToUpper() : $"{FriendlyBindName(kb.KeybModif.ToString()).ToUpper()} + {FriendlyBindName(kb.KeybKey.ToString()).ToUpper()}";

    }
    private string FriendlyBindName(string name)
    {
        if (name.StartsWith("Keypad"))
        {
            switch (name)
            {
                case "KeypadDivide":
                    return "Num /";
                case "KeypadMultiply":
                    return "Num *";
                case "KeypadMinus":
                    return "Num -";
                case "KeypadPlus":
                    return "Num +";
                case "KeypadEnter":
                    return "Num Enter";
                case "KeypadEquals":
                    return "Num =";
                case "KeypadPeriod":
                    return "Num .";
            }
            return name.Replace("Keypad", "Num ");
        }
        return name;
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
                        if (keyCodes[i] == KeyCode.Mouse0) //LMB = skip
                        {
                            continue;
                        }
                        if (keyCodes[i] == KeyCode.Mouse1) //RMB = sets to none
                        {
                            SetToNone();
                            break;
                        }
                        UpdateKeyCode(keyCodes[i], ismodifier);
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
            KeybindText.text = "PRESS A KEY...";
            Buttons.SetActive(false);
            ButtonsR.SetActive(true);
        }
        else
        {
            KeybindText.text = keyb.KeybModif == KeyCode.None ? FriendlyBindName(keyb.KeybKey.ToString()).ToUpper() : $"{FriendlyBindName(keyb.KeybModif.ToString()).ToUpper()} + {keyb.KeybKey.ToString().ToUpper()}";
            Buttons.SetActive(true);
            ButtonsR.SetActive(false);
        }
    }
    void UpdateKeyCode(KeyCode kcode, bool modifier)
    {
        if (modifier)
        {
            if (keyb.KeybKey == kcode && kcode != KeyCode.None)
            {
                KeybindError(true);
                return;
            }
            if (keyb.KeybKey == KeyCode.None && kcode != KeyCode.None)
            {
                KeybindError(false);
                return;
            }
            keyb.KeybModif = kcode;
        }
        else
        {
            if (keyb.KeybModif == kcode && kcode != KeyCode.None)
            {
                KeybindError(true);
                return;
            }
            keyb.KeybKey = kcode;
        }
        ModMenu.SaveModBinds(mod);
        ChangeKeyCode(false, ismodifier);
    }
    void KeybindError(bool sameKey)
    {
        if (sameKey)
        {
            ModUI.ShowMessage("You cannot use the same key for both key and modifier!", "Keybind Error");
        }
        else
        {
            ModUI.ShowMessage("You cannot set key to none if there is a modifier!", "Keybind Error");
        }
        ChangeKeyCode(false, ismodifier);
    }
#endif
}

