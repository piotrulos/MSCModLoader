using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace MSCLoader
{
    public class LoadAssets : MonoBehaviour
    {
        /*public static LoadAssets instance;

        LoadAssets() => instance = new LoadAssets();*/

     /*   public AssetBundle GetBundle(Mod mod, string bundleName)
        {
            AssetBundle ab = null;
            StartCoroutine(LoadBundle(mod,bundleName, (abr) => {
                ModConsole.Print(abr.ToString());
                ab = abr;
            }));
            ModConsole.Print("returned");
            return ab;//test
        }*/
        IEnumerator Testing(Mod mod, string bundleName)
        {
            AssetBundle ab = new AssetBundle();
            yield return StartCoroutine(LoadBundle(mod, bundleName, value => ab = value));

        }
        public IEnumerator LoadBundle(Mod mod, string bundleName, Action<AssetBundle> ab)
        {
            using (WWW www = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(mod), bundleName)))
            {
                while (www.progress < 1)
                {
                    ModConsole.Print(string.Format("Progress - {0}%.", www.progress * 100));
                    yield return new WaitForSeconds(.1f);
                }
                yield return www;
                if (www.error != null)
                {
                    ModConsole.Error(www.error);
                    yield break;
                }
                else
                {
                    ab(www.assetBundle);
                    //ModConsole.Print(www.assetBundle.name);
                    //ModConsole.Print(www.assetBundle.LoadAsset("MSCLoader.guiskin"));
                    //GUI.skin = www.assetBundle.LoadAsset("MSCLoader.guiskin") as GUISkin;
                    //test = www.assetBundle.LoadAsset("MSCLoader.guiskin") as GUISkin;
                    yield return www.assetBundle;
                    ModConsole.Print("Load Complete 1"); //test
                    //www.assetBundle.Unload(false); //freeup memory
                }
            }
        }
    }
}
