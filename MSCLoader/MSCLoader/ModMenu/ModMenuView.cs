using MSCLoader;
using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    internal class ModMenuView : MonoBehaviour
    {
        public Text ModTab, ReferenceTab, UpdateTab;

        public GameObject ModElementPrefab, ReferenceElementPrefab, UpdateElementPrefab;
        public GameObject HeaderGroupPrefab;
        public GameObject ButtonPrefab, CheckBoxPrefab, KeyBindPrefab, LabelPrefab, SliderPrefab, TextBoxPrefab;

        public bool modList = false;
        public GameObject modListView;
        public void ModMenuOpened()
        {
            ModTab.text = $"Mods ({ModLoader.Instance.actualModList.Length})";
            ReferenceTab.text = $"References (0)";
            UpdateTab.text = $"Updates ({ModLoader.Instance.HasUpdateModList.Count})";
            if (modList) ModList(modListView);
        }

        public void ModList(GameObject listView)
        {
            RemoveChildren(listView.transform);
            for (int i = 0; i < ModLoader.Instance.actualModList.Length; i++)
            {
                GameObject mod = GameObject.Instantiate(ModElementPrefab);
                mod.GetComponent<MenuElementList>().mod = ModLoader.Instance.actualModList[i];
                mod.GetComponent<MenuElementList>().ModInfoFill();
                mod.transform.SetParent(listView.transform, false);
            }
            for(int i = 0; i < ModLoader.InvalidMods.Count; i++)
            {
                GameObject mod = GameObject.Instantiate(ModElementPrefab);
                mod.GetComponent<MenuElementList>().InvalidMod(ModLoader.InvalidMods[i]);
                mod.transform.SetParent(listView.transform, false);
            }
        }

        public void UpdateList(GameObject listView)
        {
            RemoveChildren(listView.transform);
            for (int i = 0; i < ModLoader.Instance.HasUpdateModList.Count; i++)
            {
                GameObject mod = GameObject.Instantiate(UpdateElementPrefab);
                mod.GetComponent<MenuElementList>().mod = ModLoader.Instance.HasUpdateModList[i];
                mod.GetComponent<MenuElementList>().UpdateInfoFill();
                mod.transform.SetParent(listView.transform, false);
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
    }
}