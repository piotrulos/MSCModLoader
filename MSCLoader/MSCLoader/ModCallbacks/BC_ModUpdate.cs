#if !Mini

namespace MSCLoader
{
    internal class BC_ModUpdate : MonoBehaviour
    {
        internal ModLoader modLoader;

        private void Update() => modLoader.BC_Update();
    }
}
#endif