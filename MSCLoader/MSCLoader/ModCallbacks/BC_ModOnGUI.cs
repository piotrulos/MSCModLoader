#if !Mini
namespace MSCLoader;

internal class BC_ModOnGUI : MonoBehaviour
{
    internal ModLoader modLoader;

    private void OnGUI() => modLoader.BC_OnGUI();
}
#endif