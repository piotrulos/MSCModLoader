using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MSCLoader
{
    internal class ModMenuButton : MonoBehaviour
    {
        public Texture2D arrowLeft, arrowRight;
        public RawImage image;
        public Animation btnAnim;
        Animation anim;
        internal bool opened = false;
        public GameObject TitleText, SearchField;

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
            TitleText.SetActive(true);
            SearchField.SetActive(false);
            if (!opened)
            {
                gameObject.GetComponent<ModMenuView>().ModMenuOpened();
                opened = true;
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

        }
    }
}