using System.IO;
using UnityEngine;

namespace MSCLoader
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Compatibility layer only
    /// </summary>
    public static class ModAssets
    {
        public static AssetBundle LoadBundle(byte[] bundleBytes) =>  AssetBundle.CreateFromMemoryImmediate(bundleBytes);
        public static AssetBundle LoadBundle(string filePath)
        {
            if (File.Exists(filePath)) 
                return AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(filePath));
            else 
                throw new FileNotFoundException($"<b>LoadBundle() Error:</b> No AssetBundle file found at path: {filePath}");
        }
        public static AssetBundle LoadBundle(Mod mod, string bundleName) => LoadAssets.LoadBundle(mod, bundleName);
        public static Texture2D LoadTexture(Mod mod, string textureName, bool normalMap = false) => LoadAssets.LoadTexture(mod, textureName, normalMap);
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

