#if !Mini
using System;
using System.Collections.Generic;
using System.IO;


namespace MSCLoader;

/// <summary>
/// Class for Loading custom assets from Assets folder
/// </summary>
public static class LoadAssets
{
    internal static List<string> assetNames = new List<string>();
    /// <summary>
    /// Make GameObject Pickable, make sure your GameObject has Rigidbody and colliders attached.
    /// </summary>
    /// <param name="go">Your GameObject</param>
    public static void MakeGameObjectPickable(GameObject go)
    {
        go.layer = LayerMask.NameToLayer("Parts");
        go.tag = "PART";
    }

    /// <summary>
    /// Load texture (*.dds, *.jpg, *.png, *.tga) from mod assets folder
    /// </summary>
    /// <param name="mod">Mod instance.</param>
    /// <param name="fileName">File name to load from assets folder (for example "texture.dds")</param>
    /// <param name="normalMap">Normal mapping (default false)</param>
    /// <returns>Returns unity Texture2D</returns>
    public static Texture2D LoadTexture(Mod mod, string fileName, bool normalMap = false)
    {
        string fn = Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName);

        if (!File.Exists(fn))
        {
            throw new FileNotFoundException($"<b>LoadTexture() Error:</b> File not found: {fn}{Environment.NewLine}", fn);
        }
        string ext = Path.GetExtension(fn).ToLower();
        if (ext == ".png" || ext == ".jpg")
        {
            Texture2D t2d = new Texture2D(1, 1);
            t2d.LoadImage(File.ReadAllBytes(fn));
            return t2d;
        }
        else if (ext == ".dds")
        {
            Texture2D returnTex = LoadDDS(fn);
            return returnTex;
        }
        else if (ext == ".tga")
        {
            Texture2D returnTex = LoadTGA(fn);
            return returnTex;
        }
        else
        {
            throw new NotSupportedException($"<b>LoadTexture() Error:</b> Texture not supported: {fileName}{Environment.NewLine}");
        }
    }

    /// <summary>
    /// Load (*.obj) file from mod assets folder and return as GameObject
    /// </summary>
    /// <param name="mod">Mod instance.</param>
    /// <param name="fileName">File name to load from assets folder (for example "beer.obj")</param>
    /// <param name="collider">Apply mesh collider to object</param>
    /// <param name="rigidbody">Apply rigidbody to object to affect gravity (don't do it without collider)</param>
    /// <returns>Returns unity GameObject</returns>
    [Obsolete("LoadOBJ is deprecated, please use AssetBundles instead.", true)]
    public static GameObject LoadOBJ(Mod mod, string fileName, bool collider = true, bool rigidbody = false)
    {
        Mesh mesh = LoadOBJMesh(mod, fileName);
        if (mesh != null)
        {
            GameObject obj = new GameObject();
            obj.AddComponent<MeshFilter>().mesh = mesh;
            obj.AddComponent<MeshRenderer>();
            if (rigidbody)
                obj.AddComponent<Rigidbody>();
            if (collider)
            {
                if (rigidbody)
                    obj.AddComponent<MeshCollider>().convex = true;
                else
                    obj.AddComponent<MeshCollider>();
            }
            return obj;
        }
        else
            return null;
    }

    /// <summary>
    /// Load (*.obj) file from mod assets folder and return as Mesh
    /// </summary>
    /// <param name="mod">Mod instance.</param>
    /// <param name="fileName">File name to load from assets folder (for example "beer.obj")</param>
    /// <returns>Returns unity Mesh</returns>
    [Obsolete("LoadOBJMesh is deprecated, please use AssetBundles instead.", true)]
    public static Mesh LoadOBJMesh(Mod mod, string fileName)
    {
        string fn = Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName);
        if (!File.Exists(fn))
        {
            throw new FileNotFoundException($"<b>LoadOBJ() Error:</b> File not found: {fn}{Environment.NewLine}", fn);
        }
        string ext = Path.GetExtension(fn).ToLower();
        if (ext == ".obj")
        {
            OBJLoader obj = new OBJLoader();
            Mesh mesh = obj.ImportFile(Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName));
            mesh.name = Path.GetFileNameWithoutExtension(fn);
            return mesh;
        }
        else
            throw new NotSupportedException($"<b>LoadOBJ() Error:</b> Only (*.obj) files are supported{Environment.NewLine}");
    }


    /// <summary>
    /// Loads assetbundle from Assets folder
    /// </summary>
    /// <param name="mod">Mod instance.</param>
    /// <param name="bundleName">File name to load (for example "something.unity3d")</param>
    /// <returns>Unity AssetBundle</returns>
    public static AssetBundle LoadBundle(Mod mod, string bundleName)
    {
        string bundle = Path.Combine(ModLoader.GetModAssetsFolder(mod), bundleName);
        if (File.Exists(bundle))
        {
            ModConsole.Print($"Loading Asset: {bundleName}...");
            AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(bundle));
            string[] array = ab.GetAllAssetNames();
            for (int i = 0; i < array.Length; i++)
            {
                assetNames.Add(Path.GetFileNameWithoutExtension(array[i]));
            }
            return ab;
        }
        else
        {
            ModUI.ShowMessage($"Asset files for <color=orange>{mod.Name}</color> not found!{Environment.NewLine} Make sure you unpacked ALL files from zip into mods folder.", $"{mod.Name} - Fatal Error");
            throw new FileNotFoundException($"<b>LoadBundle() Error:</b> File not found: <b>{bundle}</b>{Environment.NewLine}", bundleName);
        }
    }

    /// <summary>
    /// Loads assetbundle from Resources
    /// </summary>
    /// <param name="assetBundleFromResources">Resource path</param>
    /// <returns>Unity AssetBundle</returns>
    public static AssetBundle LoadBundle(byte[] assetBundleFromResources)
    {
        if (assetBundleFromResources != null)
        {
            AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(assetBundleFromResources);
            string[] array = ab.GetAllAssetNames();
            for (int i = 0; i < array.Length; i++)
            {
                assetNames.Add(Path.GetFileNameWithoutExtension(array[i]));
            }
            return ab;
        }
        else
            throw new Exception($"<b>LoadBundle() Error:</b> Resource doesn't exists{Environment.NewLine}");
    }

    /// <summary>
    /// Loads assetbundle from Embedded Resources
    /// </summary>
    /// <param name="assetBundleEmbeddedResources">Resource path namespace.folder.file.extension</param>
    /// <returns>Unity AssetBundle</returns>
    public static AssetBundle LoadBundle(string assetBundleEmbeddedResources)
    {
        System.Reflection.Assembly a = System.Reflection.Assembly.GetCallingAssembly();
        using (Stream resFilestream = a.GetManifestResourceStream(assetBundleEmbeddedResources))
        {
            if (resFilestream == null)
            {
                throw new Exception($"<b>LoadBundle() Error:</b> Resource doesn't exists{Environment.NewLine}");
            }
            else
            {
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(ba);
                string[] array = ab.GetAllAssetNames();
                for (int i = 0; i < array.Length; i++)
                {
                    assetNames.Add(Path.GetFileNameWithoutExtension(array[i]));
                }
                return ab;
            }
        }
    }

    // TGALoader by https://gist.github.com/mikezila/10557162
    internal static Texture2D LoadTGA(string fileName)
    {
        using (FileStream imageFile = File.OpenRead(fileName))
        {
            return LoadTGA(imageFile);
        }
    }

    //DDS loader based on https://raw.githubusercontent.com/hobbitinisengard/crashday-3d-editor/7e7c6c78c9f67588156787af1af92cfad1019de9/Assets/IO/DDSDecoder.cs
    internal static Texture2D LoadDDS(string ddsPath)
    {
        try
        {
            byte[] ddsBytes = File.ReadAllBytes(ddsPath);

            byte ddsSizeCheck = ddsBytes[4];
            if (ddsSizeCheck != 124)
                throw new Exception("Invalid DDS DXTn texture. Unable to read"); //header byte should be 124 for DDS image files

            int height = ddsBytes[13] * 256 + ddsBytes[12];
            int width = ddsBytes[17] * 256 + ddsBytes[16];

            byte DXTType = ddsBytes[87];
            TextureFormat textureFormat = TextureFormat.DXT5;
            if (DXTType == 49)
            {
                textureFormat = TextureFormat.DXT1;
            }

            if (DXTType == 53)
            {
                textureFormat = TextureFormat.DXT5;
            }
            int DDS_HEADER_SIZE = 128;
            byte[] dxtBytes = new byte[ddsBytes.Length - DDS_HEADER_SIZE];
            Buffer.BlockCopy(ddsBytes, DDS_HEADER_SIZE, dxtBytes, 0, ddsBytes.Length - DDS_HEADER_SIZE);

            FileInfo finf = new FileInfo(ddsPath);
            Texture2D texture = new Texture2D(width, height, textureFormat, false);
            texture.LoadRawTextureData(dxtBytes);
            texture.Apply();
            texture.name = finf.Name;

            return texture;
        }
        catch (Exception ex)
        {
            ModConsole.Error($"<b>LoadTexture() Error:</b>{Environment.NewLine}Error: Could not load DDS texture");
            if (ModLoader.devMode)
                ModConsole.Error(ex.ToString());
            System.Console.WriteLine(ex);
            return new Texture2D(8, 8);
        }
    }

    // TGALoader by https://gist.github.com/mikezila/10557162
    static Texture2D LoadTGA(Stream TGAStream)
    {

        using (BinaryReader r = new BinaryReader(TGAStream))
        {
            r.BaseStream.Seek(12, SeekOrigin.Begin);

            short width = r.ReadInt16();
            short height = r.ReadInt16();
            int bitDepth = r.ReadByte();
            r.BaseStream.Seek(1, SeekOrigin.Current);

            Texture2D tex = new Texture2D(width, height);
            Color32[] pulledColors = new Color32[width * height];

            if (bitDepth == 32)
            {
                for (int i = 0; i < width * height; i++)
                {
                    byte red = r.ReadByte();
                    byte green = r.ReadByte();
                    byte blue = r.ReadByte();
                    byte alpha = r.ReadByte();

                    pulledColors[i] = new Color32(blue, green, red, alpha);
                }
            }
            else if (bitDepth == 24)
            {
                for (int i = 0; i < width * height; i++)
                {
                    byte red = r.ReadByte();
                    byte green = r.ReadByte();
                    byte blue = r.ReadByte();

                    pulledColors[i] = new Color32(blue, green, red, 1);
                }
            }
            else
            {
                throw new Exception($"<b>LoadTexture() Error:</b> TGA texture is not 32 or 24 bit depth.{Environment.NewLine}");
            }

            tex.SetPixels32(pulledColors);
            tex.Apply();
            return tex;

        }
    }
}
#endif