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
        public GameObject SearchBtn;
        public GameObject SearchField;
        private Mod mod;
        private bool set = false;
        private string previousText = "INSTALLED MODS";
        private bool isApplicationQuitting = false;
#if !Mini
        public void FillMetadataInfo(Mod m)
        {
            mod = m;
            set = false;
            gameObject.SetActive(true);
            mmv.MetadataInfoList(listView, mod);
            GoBackBtn.SetActive(true);
            SearchBtn.SetActive(false);
            SearchField.SetActive(false);
            Title.gameObject.SetActive(true);
            Title.text = $"{mod.Name} - Details".ToUpper();

        }
        public void FillSettings(Mod m)
        {
            mod = m;
            set = true;
            gameObject.SetActive(true);
            try
            {
                mmv.ModSettingsList(listView, mod);
            }
            catch (System.Exception e)
            {
                ModConsole.Error(e.Message);
                System.Console.WriteLine(e);
            }
            GoBackBtn.SetActive(true);
            SearchBtn.SetActive(false);
            SearchField.SetActive(false);
            Title.gameObject.SetActive(true);
            Title.text = $"{mod.Name} - Settings".ToUpper();
        }
        public void FillKeybinds(Mod m)
        {
            mod = m;
            set = false;
            gameObject.SetActive(true);
            mmv.KeyBindsList(listView, mod);
            GoBackBtn.SetActive(true);
            SearchBtn.SetActive(false);
            SearchField.SetActive(false);
            Title.gameObject.SetActive(true);
            Title.text = $"{mod.Name} - Keybinds".ToUpper();
        }
        public void FillUpdate(Mod m)
        {
            mod = m;
            set = false;
            gameObject.SetActive(true);
            mmv.MetadataUploadForm(listView, mod);
            GoBackBtn.SetActive(true);
            SearchBtn.SetActive(false);
            SearchField.SetActive(false);
            Title.gameObject.SetActive(true);
            Title.text = $"{mod.Name} - Update File".ToUpper();                
        }
#endif
        public void CloseView()
        {
            if (!gameObject.activeSelf) return;
            GoBackBtn.SetActive(false);
            Title.text = previousText;
            gameObject.SetActive(false);
            SearchBtn.SetActive(true);
        }
#if !Mini
        void OnEnable()
        {
            previousText = Title.text;
        }
        void OnDisable()
        {
            if (isApplicationQuitting) return;
            if (set)
                ModMenu.SaveSettings(mod);
        }
        void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }
#endif
    }
}