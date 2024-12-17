#if !Mini
using System;
using System.IO;

namespace MSCLoader;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[System.Obsolete("Only used for compatibility with ModLoaderPro", true)]
public static class ModAssets
{
    [System.Obsolete("=> LoadAssets.LoadBundle()", true)]
    public static AssetBundle LoadBundle(byte[] bundleBytes) => AssetBundle.CreateFromMemoryImmediate(bundleBytes);
    [System.Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTexture(string filePath, bool normalMap = false)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException($"<b>LoadTexture() Error:</b> File not found: {filePath}", filePath);

        string fileExtension = Path.GetExtension(filePath).ToLower();

        switch (fileExtension)
        {
            case ".jpg":
                return LoadTextureJPG(filePath, normalMap);
            case ".png":
                return LoadTexturePNG(filePath, normalMap);
            case ".dds":
                return LoadTextureDDS(filePath, normalMap);
            default:
                throw new System.NotSupportedException($"<b>LoadTexture() Error:</b> File {fileExtension} not supported as a texture: {filePath}");
        }
    }
    [System.Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTexturePNG(string filePath, bool normalMap = false)
    {
        Texture2D t2d = new Texture2D(1, 1);
        t2d.LoadImage(File.ReadAllBytes(filePath));
        return t2d;
    }
    [System.Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTextureJPG(string filePath, bool normalMap = false) => LoadTexturePNG(filePath, normalMap);
    [System.Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTextureDDS(string filePath, bool normalMap = false)
    {
        try
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);

            if (fileBytes[4] != 124) throw new Exception("Invalid DDS texture. Can't read.");

            byte DXTType = fileBytes[87];
            TextureFormat textureFormat = TextureFormat.DXT5;

            if (DXTType == 49) textureFormat = TextureFormat.DXT1;
            else if (DXTType == 53) textureFormat = TextureFormat.DXT5;
            else throw new Exception("Unsupported Texture Format. Can't load texture. Only DXT1(BC1) and DXT5(BC3) Supported.");

            int headerSize = 128;
            byte[] dxtBytes = new byte[fileBytes.Length - headerSize];
            Buffer.BlockCopy(fileBytes, headerSize, dxtBytes, 0, fileBytes.Length - headerSize);

            int height = fileBytes[13] * 256 + fileBytes[12];
            int width = fileBytes[17] * 256 + fileBytes[16];

            Texture2D texture = new Texture2D(width, height, textureFormat, false);
            texture.LoadRawTextureData(dxtBytes);
            texture.Apply();
            texture.name = Path.GetFileName(filePath);

            return texture;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            ModConsole.LogError("LoadTexture(): Can't load dds:" + filePath + "\n" + ex);
            return new Texture2D(8, 8);
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#endif