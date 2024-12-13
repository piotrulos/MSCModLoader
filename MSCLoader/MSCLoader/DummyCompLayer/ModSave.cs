#if !Mini
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MSCLoader;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Obsolete("This is only compatibility layer for ModLoaderPro, please use more efficient save system", true)]
public class ModSave
{
    [Obsolete("This is only compatibility layer for ModLoaderPro, please use more efficient save system", true)]
    public static void Save<T>(string fileName, T data, string encryptionKey = null) where T : class, new()
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, $"{fileName}.xml");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces xmlNamespace = new XmlSerializerNamespaces();
            xmlNamespace.Add("", "");
            StreamWriter output = new StreamWriter(filePath);
            XmlWriterSettings xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    ",
                NewLineOnAttributes = false,
                OmitXmlDeclaration = true
            };
            XmlWriter xmlWriter = XmlWriter.Create(output, xmlSettings);
            xmlSerializer.Serialize(xmlWriter, data, xmlNamespace);

            xmlWriter.Close();
            output.Close();

            if (!string.IsNullOrEmpty(encryptionKey))
            {
                string clearText = File.ReadAllText(filePath);
                byte[] clearBytes = Encoding.Unicode.GetBytes(File.ReadAllText(Path.Combine(Application.persistentDataPath, $"{fileName}.xml")));
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                File.WriteAllText(filePath, clearText);
            }

            ModConsole.Log($"MODSAVE: File {fileName} successfully saved!");
        }
        catch (Exception exception)
        {
            ModConsole.LogError($"MODSAVE: File {fileName} couldn't be saved.\n{exception}");
        }
    }
    [Obsolete("This is only compatibility layer for ModLoaderPro, please use more efficient save system", true)]
    public static T Load<T>(string fileName, string encryptionKey = "") where T : class, new()
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, $"{fileName}.xml");

            if (!File.Exists(path)) return new T();

            StreamReader input = new StreamReader(path);
            MemoryStream memoryInput = null;

            if (File.Exists(path))
            {
                if (!string.IsNullOrEmpty(encryptionKey))
                {
                    string cipherText = input.ReadToEnd().Replace(" ", "+");
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    using (Aes encryptor = Aes.Create())
                    {
                        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(cipherBytes, 0, cipherBytes.Length);
                                cs.Close();
                            }
                            cipherText = Encoding.Unicode.GetString(ms.ToArray());
                        }
                    }

                    //File.WriteAllText(path, cipherText);
                    memoryInput = new MemoryStream(Encoding.UTF8.GetBytes(cipherText));
                }

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                XmlReader xmlReader;
                if (memoryInput != null)
                {
                    xmlReader = XmlReader.Create(memoryInput);
                }
                else
                {
                    xmlReader = XmlReader.Create(input);
                }
                T t = xmlSerializer.Deserialize(xmlReader) as T;
                input.Close();

                ModConsole.Log($"MODSAVE: File {fileName} successfully loaded!");

                return t;
            }
        }
        catch (Exception exception)
        {
            ModConsole.LogError($"MODSAVE: File {fileName} couldn't be loaded.\n{exception}");
        }

        return new T();
    }
    [Obsolete("This is only compatibility layer for ModLoaderPro, please use more efficient save system", true)]
    public static void Delete(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, $"{fileName}.xml");
        if (File.Exists(path))
        {
            File.Delete(path);
            ModConsole.Log($"MODSAVE: File {fileName} found and deleted, mod is reset.");
        }
        else ModConsole.Log($"MODSAVE: File {fileName} not found, save is already reset.");
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
#endif