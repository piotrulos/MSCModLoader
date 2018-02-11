using MSCLoader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MSCLoader
{
    public class SaveData
    {
        public List<SaveDataList> save = new List<SaveDataList>();
    }
    public class SaveDataList
    {
        public Vector3 pos;
        public float rotX, rotY, rotZ;
    }

    public class SaveLoad
    {

        //save position and rotation of one gameobject
        public static void SaveGameObject(Mod mod, GameObject g, string fileName)
        {
            string path = Path.Combine(ModLoader.GetModConfigFolder(mod), fileName);
            SaveData save = new SaveData();
            SaveDataList s = new SaveDataList
            {
                pos = g.transform.position,
                rotX = g.transform.localEulerAngles.x,
                rotY = g.transform.localEulerAngles.y,
                rotZ = g.transform.localEulerAngles.z
            };
            save.save.Add(s);
            string serializedData = JsonConvert.SerializeObject(save);
            File.WriteAllText(path, serializedData);
           
        }

        //save position and rotation of list of gameobjects
        public static void SaveGameObjects(Mod mod, List<GameObject> go, string fileName)
        {
            string path = Path.Combine(ModLoader.GetModConfigFolder(mod), fileName);
            SaveData save = new SaveData();
            foreach(var g in go)
            {
                SaveDataList s = new SaveDataList
                {
                    pos = g.transform.position,
                    rotX = g.transform.localEulerAngles.x,
                    rotY = g.transform.localEulerAngles.y,
                    rotZ = g.transform.localEulerAngles.z
                };
                save.save.Add(s);
            }
            string serializedData = JsonConvert.SerializeObject(save);
            File.WriteAllText(path, serializedData);
        }

        //serialize custom save class
        public static void SerializeSaveFile<T>(Mod mod, T saveDataClass, string fileName)
        {
            var config = new JsonSerializerSettings();
            config.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            string path = Path.Combine(ModLoader.GetModConfigFolder(mod), fileName);
            string serializedData = JsonConvert.SerializeObject(saveDataClass, config);
            File.WriteAllText(path, serializedData);
        }

        //deserialize custom save class
        public static T DeserializeSaveFile<T>(Mod mod, string fileName) where T : new()
        {
            string path = Path.Combine(ModLoader.GetModConfigFolder(mod), fileName);
            string serializedData = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(serializedData);
        }
    }
}
