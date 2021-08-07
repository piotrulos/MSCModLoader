using UnityEngine;

namespace MSCLoader
{
    internal class ModOnGUI : MonoBehaviour
    {
        internal ModLoader modLoader;

        private void OnGUI() => modLoader.Mod_OnGUI();
    }
}