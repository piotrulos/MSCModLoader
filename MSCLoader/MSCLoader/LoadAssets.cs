using System;
using System.Collections;
using System.IO;
using UnityEngine;

//TODO: test assetbundles on external mods (and do tutorial)

namespace MSCLoader
{
    public class LoadAssets : MonoBehaviour
    {
        //Load texture from mod assets folder
        public static Texture2D LoadTexture(Mod mod, string fileName, bool normalMap = false)
        {
            string fn = Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName);

            if (!File.Exists(fn))
            {
                ModConsole.Error(string.Format("<b>LoadTexture() Error:</b>{1}File not found: {0}", fn, Environment.NewLine));
                return null;
            }
            string ext = Path.GetExtension(fn).ToLower();
            if (ext == ".png" || ext == ".jpg")
            {
                Texture2D t2d = new Texture2D(1, 1);
                t2d.LoadImage(File.ReadAllBytes(fn));
                if (normalMap)
                    SetNormalMap(ref t2d);
                return t2d;
            }
            else if (ext == ".dds")
            {
                Texture2D returnTex = LoadDDSManual(fn);
                if (normalMap)
                    SetNormalMap(ref returnTex);
                return returnTex;
            }
            else if (ext == ".tga")
            {
                Texture2D returnTex = LoadTGA(fn);
                if (normalMap)
                    SetNormalMap(ref returnTex);
                return returnTex;
            }
            else
            {
                ModConsole.Error(string.Format("<b>LoadTexture() Error:</b>{1}Texture not supported: {0}", fileName, Environment.NewLine));
            }
            return null;
        }

        //Load (*.obj) file
        public static GameObject LoadOBJ(Mod mod, string fileName)
        {
            string fn = Path.Combine(ModLoader.GetModAssetsFolder(mod), fileName);
            if (!File.Exists(fn))
            {
                ModConsole.Error(string.Format("<b>LoadOBJ() Error:</b>{1}File not found: {0}", fn, Environment.NewLine));
                return null;
            }
            string ext = Path.GetExtension(fn).ToLower();
            if (ext == ".obj")
                return OBJLoader.LoadOBJFile(mod, fn);
            else
                ModConsole.Error(string.Format("<b>LoadOBJ() Error:</b>{0}Only (*.obj) files are supported", Environment.NewLine));
            return null;
        }


        /* IEnumerator Testing(Mod mod, string bundleName)
         {
             AssetBundle ab = new AssetBundle();
             yield return StartCoroutine(LoadBundle(mod, bundleName, value => ab = value));

         }*/

        public IEnumerator LoadBundle(Mod mod, string bundleName, Action<AssetBundle> ab)
        {
            using (WWW www = new WWW("file:///" + Path.Combine(ModLoader.GetModAssetsFolder(mod), bundleName)))
            {
                while (www.progress < 1)
                {
                    ModConsole.Print(string.Format("Progress - {0}%.", www.progress * 100));//replace 
                    yield return new WaitForSeconds(.1f);
                }
                yield return www;
                if (www.error != null)
                {
                    ModConsole.Error(www.error);
                    yield break;
                }
                else
                {
                    ab(www.assetBundle);
                    yield return www.assetBundle;
                }
            }
        }

        static Texture2D LoadTGA(string fileName)
        {
            using (var imageFile = File.OpenRead(fileName))
            {
                return LoadTGA(imageFile);
            }
        }

        static Texture2D LoadDDSManual(string ddsPath)
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

                return (texture);
            }
            catch (Exception ex)
            {
                ModConsole.Error(string.Format("<b>LoadTexture() Error:</b>{0}Error: Could not load DDS texture", Environment.NewLine,ex.Message));
                return new Texture2D(8, 8);
            }
        }

        static void SetNormalMap(ref Texture2D tex)
        {
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                Color temp = pixels[i];
                temp.r = pixels[i].g;
                temp.a = pixels[i].r;
                pixels[i] = temp;
            }
            tex.SetPixels(pixels);
        }

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
                    ModConsole.Error(string.Format("<b>LoadTexture() Error:</b>{0}TGA texture is not 32 or 24 bit depth.", Environment.NewLine));
                }

                tex.SetPixels32(pulledColors);
                tex.Apply();
                return tex;

            }
        }
    }
}
