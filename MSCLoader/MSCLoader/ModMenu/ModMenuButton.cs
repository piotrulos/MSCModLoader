using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MSCLoader
{
    internal class ModMenuButton : MonoBehaviour
    {
        public bool menu2 = false;
        public Texture2D arrowLeft, arrowRight;
        public RawImage image;
        public Animation btnAnim; 
        public GameObject TitleText, SearchField;
        Animation anim;
        internal bool opened = false;


        void Start()
        {
            anim = gameObject.GetComponent<Animation>();
        }

        public void PlanAnimationClip(string clip)
        {
            if (opened) return;
            //	anim.Stop();
            btnAnim.Play(clip);
        }
        public void ButtonClicked()
        {
#if !Mini
            TitleText.SetActive(true);
            SearchField.SetActive(false);
            if (!opened)
            {
                if (menu2)
                    gameObject.GetComponent<DownloadMenuView>().DownloadMenuOpened();
                else
                    gameObject.GetComponent<ModMenuView>().ModMenuOpened();
                opened = true;
                if (menu2)
                    btnAnim.Play("hover_over_end 1");
                else
                    btnAnim.Play("hover_over_end");

                anim.Play("menu_open");
                image.texture = arrowRight;
            }
            else
            {
                anim.Play("menu_close");
                image.texture = arrowLeft;
                opened = false;
                if (ListStuff.settingsOpened)
                {
                    ModMenu.SaveSettings(ModLoader.LoadedMods[0]);
                    ModMenu.SaveSettings(ModLoader.LoadedMods[1]);
                }
            }
#endif
        }
    }
}