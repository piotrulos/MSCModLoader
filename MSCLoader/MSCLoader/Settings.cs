using MSCLoader;
using System;
using System.Collections.Generic;

namespace MSCLoader
{
    public class SettingsList
    {
        public bool isDisabled { get; set; }
        public List<Setting> settings = new List<Setting>();
    }
    public class Setting
    {
        public string ID { get; set; }
        public object Value { get; set; }
    }
    public enum SettingsType
    {
        CheckBox = 1,
        Button = 2,
        Slider = 3
    }
    public class Settings
    {
        public static List<Settings> modSettings = new List<Settings>();

        public string ID { get; set; }

        public string Name { get; set; }

        public Mod Mod { get; set; }

        public object Value { get; set; }

        public Action DoAction { get; set; }

        public SettingsType type { get; set; }

        public object[] Vals { get; set; }
        //constructors

        public Settings(string iD, string name, object value)
        {
            ID = iD;
            Name = name;
            Value = value;
            DoAction = null;
        }

        public Settings(string iD, string name, Action doAction)
        {
            ID = iD;
            Name = name;
            Value = "DoAction";
            DoAction = doAction;
        }

        public Settings(string iD, string name, object value, Action doAction)
        {
            ID = iD;
            Name = name;
            Value = value;
            DoAction = doAction;
        }

        /// <summary>
        /// Return all settings for mod.
        /// </summary>
        /// <param name="mod">The mod to get the settings for.</param>
        /// <returns>List of Settings for the mod.</returns>
        public static List<Settings> Get(Mod mod) => modSettings.FindAll(x => x.Mod == mod);

        public static void AddCheckBox(Mod mod, Settings setting)
        {
            setting.Mod = mod;
            if(setting.Value is bool)
            {
                setting.type = SettingsType.CheckBox;
                modSettings.Add(setting);
            }
            else
            {
                ModConsole.Error("AddCheckBox: non-bool value.");
            }
        }

        public static void AddButton(Mod mod, Settings setting, string description = null)
        {
            setting.Mod = mod;
            setting.Vals = new object[1];
            if (description == null)
                description = string.Empty;

            if (setting.DoAction != null)
            {
                setting.type = SettingsType.Button;
                setting.Vals[0] = description;
                modSettings.Add(setting);
            }
            else
            {
                ModConsole.Error("AddButton: Action cannot be null.");
            }
        }

        public static void AddSlider(Mod mod, Settings setting, int minValue, int maxValue)
        {
            setting.Mod = mod;
            setting.Vals = new object[3];

            if(setting.Value is int)
            {
                setting.type = SettingsType.Slider;
                setting.Vals[0] = minValue;
                setting.Vals[1] = maxValue;
                setting.Vals[2] = true;
                modSettings.Add(setting);
            }
            else
            {
                ModConsole.Error("AddSlider: only int allowed here");
            }
        }

        public static void AddSlider(Mod mod, Settings setting, float minValue, float maxValue)
        {
            setting.Mod = mod;
            setting.Vals = new object[3];

            if (setting.Value is float)
            {
                setting.type = SettingsType.Slider;
                setting.Vals[0] = minValue;
                setting.Vals[1] = maxValue;
                setting.Vals[2] = false;
                modSettings.Add(setting);
            }
            else
            {
                ModConsole.Error("AddSlider: only float allowed here");
            }
        }

        public object GetValue() => Value; //Return whatever is there
    }
}
