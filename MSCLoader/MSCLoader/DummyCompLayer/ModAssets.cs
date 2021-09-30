using System.IO;
using UnityEngine;

namespace MSCLoader
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Compatibility layer only
    /// </summary>
    [System.Obsolete("Same exact shit as LoadAssets")]
    public static class ModAssets
    {
        [System.Obsolete("=> LoadAssets.LoadBundle()",true)]
        public static AssetBundle LoadBundle(byte[] bundleBytes) =>  AssetBundle.CreateFromMemoryImmediate(bundleBytes);
        [System.Obsolete("=> LoadAssets.LoadBundle()", true)]
        public static AssetBundle LoadBundle(string filePath)
        {
            if (File.Exists(filePath)) 
                return AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(filePath));
            else 
                throw new FileNotFoundException($"<b>LoadBundle() Error:</b> No AssetBundle file found at path: {filePath}");
        }
        [System.Obsolete("=> LoadAssets.LoadBundle()", true)]
        public static AssetBundle LoadBundle(Mod mod, string bundleName) => LoadAssets.LoadBundle(mod, bundleName);
        [System.Obsolete("=> LoadAssets.LoadTexture()", true)]
        public static Texture2D LoadTexture(Mod mod, string textureName, bool normalMap = false) => LoadAssets.LoadTexture(mod, textureName, normalMap);
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

