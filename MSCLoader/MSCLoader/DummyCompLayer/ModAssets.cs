#if !Mini
using System.IO;

namespace MSCLoader;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[System.Obsolete("Same exact shit as LoadAssets")]
public static class ModAssets
{
    [System.Obsolete("=> LoadAssets.LoadBundle()", true)]
    public static AssetBundle LoadBundle(byte[] bundleBytes) => AssetBundle.CreateFromMemoryImmediate(bundleBytes);
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
            case ".tga":
                return LoadTextureTGA(filePath, normalMap);
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
    public static Texture2D LoadTextureDDS(string filePath, bool normalMap = false) => LoadAssets.LoadDDS(filePath);
    [System.Obsolete("=> LoadAssets.LoadTexture()", true)]
    public static Texture2D LoadTextureTGA(string filePath, bool normalMap = false) => LoadAssets.LoadTGA(filePath);
    [System.Obsolete("=> LoadAssets.LoadOBjMesh()", true)]
    public static Mesh LoadMeshOBJ(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"<b>LoadMeshOBJ() Error:</b> File not found: {filePath}{System.Environment.NewLine}", filePath);
        }
        string ext = Path.GetExtension(filePath).ToLower();
        if (ext == ".obj")
        {
            OBJLoader obj = new OBJLoader();
            Mesh mesh = obj.ImportFile(filePath);
            mesh.name = Path.GetFileNameWithoutExtension(filePath);
            return mesh;
        }
        else
            throw new System.NotSupportedException($"<b>LoadMeshOBJ() Error:</b> Only (*.obj) files are supported{System.Environment.NewLine}");
    }

}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#endif