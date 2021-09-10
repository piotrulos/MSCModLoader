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
                    break;
            }
        }
    }
}