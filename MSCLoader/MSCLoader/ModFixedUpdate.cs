using UnityEngine;

namespace MSCLoader
{
    internal class ModFixedUpdate : MonoBehaviour
    {
        internal ModLoader modLoader;

        private void FixedUpdate() => modLoader.Mod_FixedUpdate();
    }
}