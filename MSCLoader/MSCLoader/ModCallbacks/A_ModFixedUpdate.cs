#if !Mini

namespace MSCLoader
{
    internal class A_ModFixedUpdate : MonoBehaviour
    {
        internal ModLoader modLoader;

        private void FixedUpdate() => modLoader.A_FixedUpdate();
    }
}
#endif