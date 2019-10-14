using System.Collections.Generic;
using UnityEngine;

// Unload All changes when back to main menu
// Just destroy everything (without this script) and load again.
// No need to reset game for some mods.
namespace MSCLoader
{
#pragma warning disable CS1591

    public class MSCUnloader : MonoBehaviour
    {
        internal bool reset;
        bool doReset = false;

        internal void MSCLoaderReset()
        {
            if (!reset) //to avoid endless loop
            {
                reset = true;
                doReset = true;
            }
        }
        void Update()
        {
            if(doReset && !Application.isLoadingLevel) //if menu is fully loaded.
            {
                foreach (GameObject o in FindObjectsOfType<GameObject>())
                {
                    if (o.name == "MSCUnloader")
                        continue;
                    Destroy(o);                    
                }
				
                ModLoader.allModsLoaded = false; // FREDTWEAK
				
                Keybind.Keybinds = new List<Keybind>();
                Keybind.DefaultKeybinds = new List<Keybind>();
                Settings.modSettings = new List<Settings>();
                PlayMakerGlobals.Instance.Variables.FindFsmBool("SongImported").Value = false; //stupid variable name.
                ModLoader.unloader = false;
                ModLoader.rtmm = true;
                Application.LoadLevel(Application.loadedLevelName);
                doReset = false;
            }
        }
    }
#pragma warning restore CS1591
}
