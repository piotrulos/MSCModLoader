using UnityEngine;
using System.Collections;

namespace MSCLoader
{
    internal class ListStuff : MonoBehaviour
    {

        public enum ListType
        {
            Mods,
            References,
            Updates,
            MainSettings
        }

        public ModMenuView mmv;
        public ListType type;
        public GameObject listView;

        public static bool settingsOpened = false;

        void OnEnable()
        {
            switch (type)
            {
                case ListType.Mods:
                    mmv.modList = true;
                    mmv.modListView = listView;
                    mmv.ModList(listView);
                    break;
                case ListType.References:
                    mmv.modList = false;
                    break;
                case ListType.Updates:
                    mmv.modList = false;
                    mmv.UpdateList(listView);
                    break;
                case ListType.MainSettings:
                    mmv.modList = false;
                    settingsOpened = true;
                    mmv.MainSettingsList(listView);
                    break;
            }
        }
        void OnDisable()
        {
            switch (type)
            {
                case ListType.Mods:
                    break;
                case ListType.References:
                    break;
                case ListType.Updates:
                    break;
                case ListType.MainSettings:
                    ModMenu.SaveSettings(ModLoader.LoadedMods[0]);
                    ModMenu.SaveSettings(ModLoader.LoadedMods[1]);
                    break;
            }
        }

    }
}