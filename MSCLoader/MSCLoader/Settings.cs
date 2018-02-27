using MSCLoader;
using System;
using System.Collections.Generic;

namespace MSCLoader
{
    public class Settings
    {
        //Just testing (how to do this)

        public static List<Settings> modSettings = new List<Settings>();

        public string ID { get; set; }

        public string Name { get; set; }

        public Mod Mod { get; set; }

        public object Value { get; set; }

        public Action action { get; set; }

        public Settings(string iD, string name, Mod mod)
        {
            ID = iD;
            Name = name;
            Mod = mod;
        }

        public Settings(string iD, string name, Mod mod, Action action) : this(iD, name, mod)
        {
            this.action = action;
        }

        public static void AddCheckBox()
        {

        }

        public static void AddButton()
        {

        }

        public static object ReturnValue()
        {
            return null; //Return whatever
        }
    }
}
