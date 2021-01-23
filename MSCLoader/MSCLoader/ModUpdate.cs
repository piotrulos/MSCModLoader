using UnityEngine;

namespace MSCLoader
{
    internal class ModUpdate : MonoBehaviour
    {
        internal ModLoader modLoader;

        private void Update() => modLoader.Mod_Update();
    }
}