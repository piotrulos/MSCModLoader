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
    [Obsolete("This class requires user to have 'Compatibility References' installed")]
    public class ModSave
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Obsolete("This requires user to have 'Compatibility References' installed")]
        public static void Save<T>(string fileName, T data, string encryptionKey = null) where T : class, new() => MSCLoaderHelpers.ModSave.Save<T>(fileName, data, encryptionKey);
        [Obsolete("This requires user to have 'Compatibility References' installed")]
        public static T Load<T>(string fileName, string encryptionKey = "") where T : class, new() => MSCLoaderHelpers.ModSave.Load<T>(fileName, encryptionKey);
        [Obsolete("This requires user to have 'Compatibility References' installed")]
        public static void Delete(string fileName) => MSCLoaderHelpers.ModSave.Delete(fileName);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    }
}
