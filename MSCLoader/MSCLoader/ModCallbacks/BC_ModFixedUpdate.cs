#if !Mini
namespace MSCLoader;

internal class BC_ModFixedUpdate : MonoBehaviour
{
    internal ModLoader modLoader;

    private void FixedUpdate() => modLoader.BC_FixedUpdate();
}
#endif