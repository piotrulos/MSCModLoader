using MSCLoader;
using System;
using System.Collections.Generic;

namespace MSCLoader
{
    public enum SettingsType
    {
        CheckBox = 1,
        Button = 2,
        Slider = 3
    }
    public class Settings
    {
        //Just testing (how to do this)

        public static List<Settings> modSettings = new List<Settings>();

        public string ID { get; set; }

        public string Name { get; set; }

        public Mod Mod { get; set; }

        public object Value { get; set; }

        public Action DoAction { get; set; }

        public SettingsType type { get; set; }

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

        public static void AddButton()
        {

        }

        public object ReturnValue() => Value; //Return whatever
    }
}
