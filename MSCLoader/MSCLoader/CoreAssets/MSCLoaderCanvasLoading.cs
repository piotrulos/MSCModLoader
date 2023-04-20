using UnityEngine.UI;

namespace MSCLoader
{
    //Helper class UI
    internal class MSCLoaderCanvasLoading : MonoBehaviour
    {
        public GameObject modLoadingUI, modUpdateUI;
        public Text lHeader, lTitle, lMod, uTitle, uStatus;
        public Slider lProgress, uProgress;
        
        void Awake()
        {
            modLoadingUI.SetActive(false);
            modUpdateUI.SetActive(false);
        }
    }
}