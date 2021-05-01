using UnityEngine;

namespace MSCLoader.Helper
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Dummy namespace compatibility layer
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class ModHelper
    {
        //TODO: Redirect some other shit if possible

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void MakePickable(this GameObject gameObject, bool includeTag = true) => LoadAssets.MakeGameObjectPickable(gameObject);
     
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void MakePickable(this Transform transform, bool includeTag = true) => transform.gameObject.MakePickable(includeTag);

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void OpenFolder(string path) { }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void OpenWebsite(string url) { }

    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
