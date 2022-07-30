#if !Mini
using Newtonsoft.Json;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    internal class DownloadableModList
    {
        public string mod_id;
        public string mod_version;
        public string mod_description;
        public int mod_type;
    }

    internal class DwnlModList
    {
        public List<DownloadableModList> DownloadableModList;
    }
    internal class DownloadMenuView : MonoBehaviour
    {
        public Text Tab1, Tab2;

        public GameObject DownloadElementPrefab;
        public GameObject ButtonPrefab, LabelPrefab;
        //     public UniversalView universalView;

        public bool modList = false;
        public GameObject modListView;
        DwnlModList dml;
        public void RefreshTabs()
        {

            if (dml != null) return;
            string dwl = string.Empty;
            WebClient getdwl = new WebClient();
            getdwl.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
            try
            {
#if !Mini
                dwl = getdwl.DownloadString($"{ModLoader.serverURL}/mods_list.php?mods={0}");
                dml = JsonConvert.DeserializeObject<DwnlModList>(dwl);
#endif
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed");
                Console.WriteLine(e);
            }
            Tab1.text = $"Download Mods (<color=lime>{dml.DownloadableModList.Count}</color>)";
            /*ModTab.text = $"Mods (<color=lime>{ModLoader.Instance.actualModList.Length}</color>/<color=magenta>{ModLoader.InvalidMods.Count}</color>)";
             ReferenceTab.text = $"References (<color=aqua>{ModLoader.Instance.ReferencesList.Count}</color>)";
             UpdateTab.text = $"Updates (<color=yellow>{ModLoader.HasUpdateModList.Count + ModLoader.HasUpdateRefList.Count}</color>)";
        */

        }
        public void DownloadMenuOpened()
        {
            RefreshTabs();
            if (modList) ModList(modListView, string.Empty);
        }
        System.Collections.IEnumerator ModListAsync(GameObject listView, string search)
        {
            DownloadableModList[] filteredList = new DownloadableModList[0];
            filteredList = dml.DownloadableModList.ToArray();

            /*  if (search == string.Empty)
              {
                  filteredList = dml.DownloadableModList.ToArray();
              }
              else
              {
                  filteredList = ModLoader.Instance.actualModList.Where(x => x.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || x.ID.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
              }*/
            //  ModConsole.Warning(filteredList.Length.ToString());
            for (int i = 0; i < filteredList.Length; i++)
            {
                GameObject mod = GameObject.Instantiate(DownloadElementPrefab);
                // mod.GetComponent<MenuElementList>().mod = filteredList[i];
                mod.GetComponent<MenuElementList>().DownloadItemFill(filteredList[i]);
                //    mod.GetComponent<MenuElementList>().ModButtonsPrep(universalView);
                mod.transform.SetParent(listView.transform, false);
                mod.SetActive(true);
                yield return null;
            }
            /*   for (int i = 0; i < filteredInvalidList.Length; i++)
               {
                   GameObject mod = GameObject.Instantiate(DownloadElementPrefab);
                   mod.GetComponent<MenuElementList>().InvalidMod(filteredInvalidList[i]);
                   mod.transform.SetParent(listView.transform, false);
                   mod.SetActive(true);
                   yield return null;
               }
               if (filteredList.Length == 0 && filteredInvalidList.Length == 0)
               {
                   SettingsElement tx = CreateText(listView.transform, $"<color=aqua>Nothing found</color>");
                   tx.value.alignment = TextAnchor.MiddleCenter;
               }*/
        }

        public void ModList(GameObject listView, string search)
        {
            if (dml == null) return;
            StopAllCoroutines();
            RemoveChildren(listView.transform);
            if (ModLoader.Instance.actualModList.Length == 0 && search == string.Empty)
            {
                SettingsElement tx = CreateText(listView.transform, $"<color=aqua>A little empty here, seems like there is no mods installed.{Environment.NewLine}If you think that you installed mods, check if you put mods in correct folder.{Environment.NewLine}Current Mod folder is: <color=yellow>{ModLoader.ModsFolder}</color></color>");
                tx.settingName.alignment = TextAnchor.MiddleCenter;
            }
            StartCoroutine(ModListAsync(listView, search));
            return;
        }
        internal static void OpenModLink(string url)
        {
            if (ModMenu.openLinksOverlay.GetValue())
            {
                //try opening in steam overlay
                try
                {
#if !Mini
                    Steamworks.SteamFriends.ActivateGameOverlayToWebPage(url);
#endif
                }
                catch (Exception e)
                {
                    ModConsole.Error(e.Message);
                    System.Console.WriteLine(e);
                    Application.OpenURL(url);
                    System.Console.WriteLine(url);
                }
            }
            else
            {
                Application.OpenURL(url);
                System.Console.WriteLine(url);
            }
        }

        public void RemoveChildren(Transform parent)
        {
            if (parent.childCount > 0)
            {
                for (int i = 0; i < parent.childCount; i++)
                    Destroy(parent.GetChild(i).gameObject);
            }
        }

        public SettingsElement CreateButton(Transform listView, string text, Color textColor, Color btnColor)
        {
            GameObject btnP = Instantiate(ButtonPrefab);
            SettingsElement btn = btnP.GetComponent<SettingsElement>();
            btn.settingName.text = text.ToUpper();
            btn.settingName.color = textColor;
            btn.button.GetComponent<Image>().color = btnColor;
            btn.transform.SetParent(listView.transform, false);
            return btn;
        }
        public SettingsElement CreateText(Transform listView, string text)
        {
            GameObject tx = Instantiate(LabelPrefab);
            SettingsElement txt = tx.GetComponent<SettingsElement>();
            txt.value.text = text;
            tx.transform.SetParent(listView.transform, false);
            return txt;
        }
    }
}