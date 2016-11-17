using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MSCLoader
{
	/// <summary>
	/// Collection of utility functions for retrieving info from the game.
	/// </summary>
	public class GameUtil
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
	}
}
