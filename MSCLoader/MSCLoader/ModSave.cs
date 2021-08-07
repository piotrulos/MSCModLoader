using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSCLoader
{
    /// <summary>
    /// Redirect Only [TO USE THIS YOU NEED TO GET REFERENCES SEPARATELY]
    /// 
    /// </summary>
    
    //TODO: Replace this shit with proper save system
    public class ModSave
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static void Save<T>(string fileName, T data, string encryptionKey = null) where T : class, new() => MSCLoaderHelpers.ModSave.Save<T>(fileName, data, encryptionKey);
        public static T Load<T>(string fileName, string encryptionKey = "") where T : class, new() => MSCLoaderHelpers.ModSave.Load<T>(fileName, encryptionKey);
        public static void Delete(string fileName) => MSCLoaderHelpers.ModSave.Delete(fileName);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    }
}
