using MSCLoader;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    internal class UniversalView : MonoBehaviour
    {
        public ModMenuView mmv;
        public GameObject listView;
        public GameObject GoBackBtn;
        public Text Title;
        private Mod mod;
        private bool set = false;
        public void FillSettings(Mod m)
        {
            mod = m;
            set = true;
            gameObject.SetActive(true);
            mmv.ModSettingsList(listView, mod);
            GoBackBtn.SetActive(true);
            Title.text = $"{mod.Name} - Settings".ToUpper();
        }
        public void FillKeybinds(Mod m)
        {
            mod = m;
            set = false;
            gameObject.SetActive(true);
            mmv.KeyBindsList(listView, mod);
            GoBackBtn.SetActive(true);
            Title.text = $"{mod.Name} - Keybinds".ToUpper();
        }
        void OnDisable()
        {
            Title.text = "Installed mods".ToUpper();
            if (set)
                ModMenu.SaveSettings(mod);
        }
    }
}