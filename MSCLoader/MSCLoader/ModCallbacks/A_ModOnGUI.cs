#if !Mini

namespace MSCLoader
{
    internal class A_ModOnGUI : MonoBehaviour
    {
        internal ModLoader modLoader;

        private void OnGUI() => modLoader.A_OnGUI();
    }
}
#endif