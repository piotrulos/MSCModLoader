using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MSCLoader
{
    /// <summary>
    /// Collection of utility functions for retrieving info from the game.
    /// </summary>
    public static class GameUtil
	{
		/// <summary>
		/// Return private field from class.
		/// </summary>
		/// <typeparam name="T">The type of field to get.</typeparam>
		/// <param name="instance">The instance of the class to get the field from.</param>
		/// <param name="name">The name of the private field.</param>
		/// <returns>Private field from class.</returns>
		public static T GetPrivateField<T>(object instance, string name)
		{
			FieldInfo var = instance.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
			return (T)var.GetValue(instance);
		}


        /// <summary>
        /// Returns all monobehaviours (casted to T)
        /// </summary>
        /// <typeparam name="T">interface type</typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T[] GetInterfaces<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            var mObjs = gObj.GetComponents<MonoBehaviour>();

            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }

        /// <summary>
        /// Returns the first monobehaviour that is of the interface type (casted to T)
        /// </summary>
        /// <typeparam name="T">Interface type</typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T GetInterface<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            return gObj.GetInterfaces<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the first instance of the monobehaviour that is of the interface type T (casted to T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T GetInterfaceInChildren<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            return gObj.GetInterfacesInChildren<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets all monobehaviours in children that implement the interface of type T (casted to T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T[] GetInterfacesInChildren<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");

            var mObjs = gObj.GetComponentsInChildren<MonoBehaviour>();

            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }
    }
}
