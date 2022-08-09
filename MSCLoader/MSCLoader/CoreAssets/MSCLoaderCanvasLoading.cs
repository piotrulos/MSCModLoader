using UnityEngine;
using UnityEngine.UI;

namespace MSCLoader
{
    //Helper class UI
    internal class MSCLoaderCanvasLoading : MonoBehaviour
    {
        [SerializeField]
        internal GameObject modLoadingUI, modUpdateUI;
        [SerializeField]
        internal Text lHeader, lTitle, lMod, uTitle, uStatus;
        [SerializeField]
        internal Slider lProgress, uProgress;
    }
}