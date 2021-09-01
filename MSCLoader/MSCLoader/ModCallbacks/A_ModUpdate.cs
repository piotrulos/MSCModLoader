using UnityEngine;

namespace MSCLoader
{
    internal class A_ModUpdate : MonoBehaviour
    {
        internal ModLoader modLoader;

        private void Update() => modLoader.A_Update();
    }
}