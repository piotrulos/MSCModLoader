using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MSCLoader
{
#pragma warning disable CS1591 
    public class SaveData
    {
        public List<SaveDataList> save = new List<SaveDataList>();
    }
    public class SaveDataList
    {
        public string name;
        public Vector3 pos;
        public float rotX, rotY, rotZ;
    }
#pragma warning restore CS1591

    /// <summary>
    /// Save and Load Class for gameobject and custom class
    /// </summary>
    public class SaveLoad
    {

        /// <summary>
        /// Save position and rotation of single gameobject to file (DO NOT loop this for multiple gameobjects)
        /// Call this in <see cref="Mod.OnSave"/>  function
        /// </summary>
        /// <param name="mod">Mod instance</param>
        /// <param name="g">Your GameObject to save</param>
        /// <param name="fileName">Name of the save file</param>
        public static void SaveGameObject(Mod mod, GameObject g, string fileName)
        {
            string path = Path.Combine(ModLoader.GetModConfigFolder(mod), fileName);
            SaveData save = new SaveData();
            SaveDataList s = new SaveDataList
            {
                name = g.name,
                pos = g.transform.position,
                rotX = g.transform.localEulerAngles.x,
                rotY = g.transform.localEulerAngles.y,
                rotZ = g.transform.localEulerAngles.z
            };
            save.save.Add(s);
            string serializedData = JsonConvert.SerializeObject(save, Formatting.Indented);
            File.WriteAllText(path, serializedData);
           
        }

        /// <summary>
        /// Load position and rotation of single gameobject from file
        /// Call this AFTER you load your gameobject
        /// </summary>
        /// <param name="mod">Mod instance</param>
        /// <param name="fileName">Name of the save file</param>
        public static void LoadGameObject(Mod mod, string fileName)
        {
            SaveData data = DeserializeSaveFile<SaveData>(mod, fileName);
            GameObject go = GameObject.Find(data.save[0].name);
            go.transform.position = data.save[0].pos;
            go.transform.rotation = Quaternion.Euler(data.save[0].rotX, data.save[0].rotY, data.save[0].rotZ);
        }

        /// <summary>
        /// Serialize custom save class (see example)
        /// Call Only in <see cref="Mod.OnSave"/>
        /// </summary>
        /// <typeparam name="T">Your class</typeparam>
        /// <param name="mod">Mod Instance</param>
        /// <param name="saveDataClass">Your class</param>
        /// <param name="fileName">Name of the save file</param>
        /// <example><code source="SaveExamples.cs" region="Serializer" lang="C#" 
        /// title="Example of save class" /></example>
        public static void SerializeSaveFile<T>(Mod mod, T saveDataClass, string fileName)
        {
            var config = new JsonSerializerSettings();
            config.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatting = Formatting.Indented;
            string path = Path.Combine(ModLoader.GetModConfigFolder(mod), fileName);
            string serializedData = JsonConvert.SerializeObject(saveDataClass, config);
            File.WriteAllText(path, serializedData);
        }

        /// <summary>
        /// Deserialize custom save class (see example)
        /// </summary>
        /// <typeparam name="T">Your save class</typeparam>
        /// <param name="mod">Mod Instance</param>
        /// <param name="fileName">Name of the save file</param>
        /// <returns>Deserialized class</returns>
        /// <example><code source="SaveExamples.cs" region="Deserializer" lang="C#" 
        /// title="Example of loading class" /></example>
        public static T DeserializeSaveFile<T>(Mod mod, string fileName) where T : new()
        {
            string path = Path.Combine(ModLoader.GetModConfigFolder(mod), fileName);
            if (File.Exists(path))
            {
                string serializedData = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(serializedData);
            }
            return default(T);
        }
    }
}
