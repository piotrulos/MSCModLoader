using UnityEngine.UI;

namespace MSCLoader
{
    internal class ListStuff : MonoBehaviour
    {

        public enum ListType
        {
            Mods,
            References,
            Updates,
            MainSettings,
            ModsDownload,
            ModsDownloadAll
        }

        public ModMenuView mmv;
        public ListType type;
        public GameObject listView;
        public InputField searchField;

        public static bool settingsOpened = false;
        private bool isApplicationQuitting = false;
#if !Mini
        void OnEnable()
        {
            switch (type)
            {
                case ListType.Mods:
                    mmv.modList = true;
                    mmv.modListView = listView;
                    mmv.ModList(listView, string.Empty);
                    searchField.onValueChange.RemoveAllListeners();
                    searchField.onValueChange.AddListener((string text) => mmv.ModList(listView, text));
                    break;
                case ListType.References:
                    mmv.modList = false;
                    mmv.ReferencesList(listView);
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
                case ListType.ModsDownloadAll:
                    break;
            }
        }
        void OnDisable()
        {
            if (isApplicationQuitting) return;
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
        void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }
#endif
    }
}