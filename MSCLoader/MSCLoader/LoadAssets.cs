using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace MSCLoader
{
    /// <summary>
    /// Class for Loading custom assets from Assets folder
    /// </summary>
    public static class LoadAssets
    {
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
        /// <example>
        /// You need to enter file name from your mod's asset folder.<code source="Examples.cs" region="LoadTexture" lang="C#" 
        /// title="Example for change texture when we press key" /></example>
        /// <param name="mod">Mod instance.</param>
        /// <param name="fileName">File name to load from assets folder (for example "texture.dds")</param>
        /// <param name="normalMap">Normal mapping (default false)</param>
        /// <returns>Returns unity Texture2D</returns>
        public static Texture2D LoadTexture(Mod mod, string fileName, bool normalMap = false)
        {
            string fn = Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName);

            if (!File.Exists(fn))
            {
                throw new FileNotFoundException(string.Format("<b>LoadTexture() Error:</b> File not found: {0}{1}", fn, Environment.NewLine), fn);
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
                throw new NotSupportedException(string.Format("<b>LoadTexture() Error:</b> Texture not supported: {0}{1}", fileName, Environment.NewLine));
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
        /// <example>Example Code
        /// <code source="Examples.cs" region="LoadOBJ" lang="C#" />
        /// </example>
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
        /// <example>Example Code
        /// <code source="Examples.cs" region="LoadOBJMesh" lang="C#" />
        /// </example>
        [Obsolete("LoadOBJMesh is deprecated, please use AssetBundles instead.", true)]
        public static Mesh LoadOBJMesh(Mod mod, string fileName)
        {
            string fn = Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName);
            if (!File.Exists(fn))
            {
                throw new FileNotFoundException(string.Format("<b>LoadOBJ() Error:</b> File not found: {0}{1}", fn, Environment.NewLine), fn);
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
                throw new NotSupportedException(string.Format("<b>LoadOBJ() Error:</b> Only (*.obj) files are supported{0}", Environment.NewLine));
        }


        /// <summary>
        /// Loads assetbundle from Assets folder
        /// </summary>
        /// <example> Example based on loading settings assets.
        /// <code source="Examples.cs" region="LoadBundle" lang="C#"/></example>
        /// <param name="mod">Mod instance.</param>
        /// <param name="bundleName">File name to load (for example "something.unity3d")</param>
        /// <returns>Unity AssetBundle</returns>
        public static AssetBundle LoadBundle(Mod mod, string bundleName)
        {
            string bundle = Path.Combine(ModLoader.GetModAssetsFolder(mod), bundleName);
            if(File.Exists(bundle))
            {
                ModConsole.Print(string.Format("Loading Asset: {0}...", bundleName));
                return AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(bundle));
            }
            else
            {
                throw new FileNotFoundException(string.Format("<b>LoadBundle() Error:</b> File not found: <b>{0}</b>{1}", bundle, Environment.NewLine), bundleName);
            }
        }

        /// <summary>
        /// Loads assetbundle from Resources
        /// </summary>
        /// <param name="assetBundleFromResources">Resource path</param>
        /// <returns>Unity AssetBundle</returns>
        public static AssetBundle LoadBundle(byte[] assetBundleFromResources)
        {
            if(assetBundleFromResources != null)
                return AssetBundle.CreateFromMemoryImmediate(assetBundleFromResources);
            else
                throw new Exception(string.Format("<b>LoadBundle() Error:</b> Resource doesn't exists{0}", Environment.NewLine));
        }

        /// <summary>
        /// Loads assetbundle from Embedded Resources
        /// </summary>
        /// <param name="assetBundleEmbeddedResources">Resource path namespace.folder.file.extension</param>
        /// <returns>Unity AssetBundle</returns>
        public static AssetBundle LoadBundle(string assetBundleEmbeddedResources)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream(assetBundleEmbeddedResources))
            {
                if (resFilestream == null)
                {

                    throw new Exception(string.Format("<b>LoadBundle() Error:</b> Resource doesn't exists{0}", Environment.NewLine));
                }
                else
                {
                    byte[] ba = new byte[resFilestream.Length];
                    resFilestream.Read(ba, 0, ba.Length);
                    return AssetBundle.CreateFromMemoryImmediate(ba);
                }
            }                             
        }

        // TGALoader by https://gist.github.com/mikezila/10557162
        static Texture2D LoadTGA(string fileName)
        {
            using (var imageFile = File.OpenRead(fileName))
            {
                return LoadTGA(imageFile);
            }
        }

        //DDS loader by https://gist.github.com/tomazsaraiva/8be91104fa0f4a52e0a7629eb1e59844#file-loaddds-cs
        static Texture2D LoadDDS(string ddsPath)
        {
            try
            {
                byte[] data = File.ReadAllBytes(ddsPath);
                FileInfo finf = new FileInfo(ddsPath);
                TextureFormat textureFormat = TextureFormat.ARGB32;
                byte DXTType = data[87];
                if (DXTType == 49)
                     textureFormat = TextureFormat.DXT1;
                if (DXTType == 53)               
                    textureFormat = TextureFormat.DXT5;                
                if (textureFormat != TextureFormat.DXT1 && textureFormat != TextureFormat.DXT5)               
                    throw new Exception("Invalid TextureFormat. Only DXT1 and DXT5 formats are supported by this method.");              

                byte ddsSizeCheck = data[4];
                if (ddsSizeCheck != 124)                
                    throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files              

                int height = data[13] * 256 + data[12];
                int width = data[17] * 256 + data[16];

                int DDS_HEADER_SIZE = 128;
                byte[] dxtBytes = new byte[data.Length - DDS_HEADER_SIZE];
                Buffer.BlockCopy(data, DDS_HEADER_SIZE, dxtBytes, 0, data.Length - DDS_HEADER_SIZE);

                Texture2D texture = new Texture2D(width, height, textureFormat, false);
                texture.LoadRawTextureData(dxtBytes);
                texture.Apply();
                texture.name = finf.Name;
                return texture;
            }
            catch (Exception ex)
            {
                ModConsole.Error(string.Format("<b>LoadTexture() Error:</b>{0}Error: Could not load DDS texture", Environment.NewLine,ex.Message));
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
                    throw new Exception(string.Format("<b>LoadTexture() Error:</b> TGA texture is not 32 or 24 bit depth.{0}", Environment.NewLine));
                }

                tex.SetPixels32(pulledColors);
                tex.Apply();
                return tex;

            }
        }
    }
}
