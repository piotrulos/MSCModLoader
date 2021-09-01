using System;
using System.Collections.Generic;
using UnityEngine;

namespace MSCLoader
{
	/// <summary>
	/// Add easily rebindable keybinds.
	/// </summary>
	public class Keybind
	{
        /// <summary>
        /// List of Keybinds
        /// </summary>
        public static List<Keybind> Keybinds = new List<Keybind>();
        /// <summary>
        /// List of Default Keybinds
        /// </summary>
        public static List<Keybind> DefaultKeybinds = new List<Keybind>();

		/// <summary>
		/// The ID of the keybind (Should only be used once in your mod).
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// The name that will be displayed in settings
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The KeyCode the user will have to press.
		/// </summary>
		public KeyCode Key { get; set; }

		/// <summary>
		/// The modifier KeyCode the user will have to press with the Key.
		/// </summary>
		public KeyCode Modifier { get; set; }

		/// <summary>
		/// The Mod this Keybind belongs to (This is set when using Keybind.Add).
		/// </summary>
		public Mod Mod { get; set; }

        /// <summary>
        /// Helpful additional variables.
        /// </summary>
        public object[] Vals { get; set; }

        /// <summary>
        /// Add a keybind.
        /// </summary>
        /// <param name="mod">The instance of your mod.</param>
        /// <param name="key">The Keybind to add.</param>
        /// <example><code source="Examples.cs" region="KeyBindAdd" lang="C#" 
        /// title="Keybind Add" /></example>
        public static void Add(Mod mod, Keybind key)
        {
            key.Mod = mod;
            Keybinds.Add(key);
            DefaultKeybinds.Add(new Keybind(key.ID, key.Name, key.Key, key.Modifier) { Mod = mod });
        }
        /// <summary>
        /// Add a keybind.
        /// </summary>
        /// <param name="mod">The instance of your mod.</param>
        /// <param name="id">The ID of the Keybind (Used only once in your mod).</param>
        /// <param name="name">The name of the Keybind that will be displayed.</param>
        /// <param name="key">The KeyCode the user will press.</param>
        /// <returns>Keybind</returns>
        public static Keybind Add(Mod mod, string id, string name, KeyCode key)
        {
            return Add(mod, id, name, key, KeyCode.None);

        }
        /// <summary>
        /// Add a keybind.
        /// </summary>
        /// <param name="mod">The instance of your mod.</param>
        /// <param name="id">The ID of the Keybind (Used only once in your mod).</param>
        /// <param name="name">The name of the Keybind that will be displayed.</param>
        /// <param name="key">The KeyCode the user will press.</param>
        /// <param name="modifier">The modifier KeyCode the user will have to press.</param>
        /// <returns>Keybind</returns>
        public static Keybind Add(Mod mod, string id, string name, KeyCode key, KeyCode modifier)
		{
            Keybind keyb = new Keybind(id, name, key, modifier) { Mod = mod };
            Keybinds.Add(keyb);
			DefaultKeybinds.Add(new Keybind(id, name, key, modifier) { Mod = mod });
            return keyb;

        }
        /// <summary>
        /// Add Header, blue title bar that can be used to separate settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        public static void AddHeader(Mod mod, string HeaderTitle) => AddHeader(mod, HeaderTitle, Color.blue, Color.white);

        /// <summary>
        /// Add Header, blue title bar that can be used to separate settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor) => AddHeader(mod, HeaderTitle, backgroundColor, Color.white);

        /// <summary>
        /// Add Header, blue title bar that can be used to separate settings.
        /// </summary>
        /// <param name="mod">Your mod instance</param>
        /// <param name="HeaderTitle">Title of your header</param>
        /// <param name="backgroundColor">Background color of header</param>
        /// <param name="textColor">Text Color of header</param>
        public static void AddHeader(Mod mod, string HeaderTitle, Color backgroundColor, Color textColor)
        {
            Keybind kb = new Keybind(null, HeaderTitle, KeyCode.None, KeyCode.None);
            kb.Mod = mod;
            kb.Vals = new object[3];
            kb.Vals[0] = HeaderTitle;
            kb.Vals[1] = backgroundColor;
            kb.Vals[2] = textColor;
            Keybinds.Add(kb);
        }
        /// <summary>
        /// Return all keybinds for mod.
        /// </summary>
        /// <param name="mod">The mod to get the Keybinds for.</param>
        /// <returns>List of Keybinds for the mod.</returns>
        public static List<Keybind> Get(Mod mod) => Keybinds.FindAll(x => x.Mod == mod);

        /// <summary>
        /// Return all default keybinds for mod.
        /// </summary>
        /// <param name="mod">The mod to get the keybinds for.</param>
        /// <returns>List of default Keybinds for the mod.</returns>
        public static List<Keybind> GetDefault(Mod mod) => DefaultKeybinds.FindAll(x => x.Mod == mod);

        /// <summary>
        /// Constructor for Keybind without modifier
        /// </summary>
        /// <param name="id">The ID of the Keybind.</param>
        /// <param name="name">The name of the Keybind.</param>
        /// <param name="key">The KeyCode the user will press.</param>
        /// <example><code source="Examples.cs" region="KeyBind1" lang="C#" 
        /// title="Keybind without modifier" /></example>
        public Keybind(string id, string name, KeyCode key)
		{
			ID = id;
			Name = name;
			Key = key;
			Modifier = KeyCode.None;
		}

        /// <summary>
        /// Constructor for Keybind
        /// </summary>
        /// <param name="id">The ID of the Keybind (Used only once in your mod).</param>
        /// <param name="name">The name of the Keybind that will be displayed.</param>
        /// <param name="key">The KeyCode the user will press.</param>
        /// <param name="modifier">The modifier KeyCode the user will have to press.</param>
        /// <example><code source="Examples.cs" region="KeyBind2" lang="C#" 
        /// title="Keybind and modifier" /></example>
        public Keybind(string id, string name, KeyCode key, KeyCode modifier)
		{
			ID = id;
			Name = name;
			Key = key;
			Modifier = modifier;
		}

        /// <summary>
        /// Check if keybind is being hold down. (Same behaviour as GetKey)
        /// </summary>
        /// <returns>true, if the keybind is being hold down.</returns>
        /// <example><code source="Examples.cs" region="GetKeybind" lang="C#" 
        /// title="Keybind and modifier" /></example>
        public bool GetKeybind()
        {
            if (Modifier != KeyCode.None)
            {
                return Input.GetKey(Modifier) && Input.GetKey(Key);
            }

            return Input.GetKey(Key);
        }

        /// <summary>
        /// Check if the keybind was just pressed once. (Same behaviour as GetKeyDown)
        /// </summary>
        /// <returns>true, Check if the keybind was just pressed.</returns>
        /// <example><code source="Examples.cs" region="GetKeybindDown" lang="C#" 
        /// title="Keybind and modifier" /></example>
        public bool GetKeybindDown()
        {
            if (Modifier != KeyCode.None)
            {
                return Input.GetKey(Modifier) && Input.GetKeyDown(Key);
            }

            return Input.GetKeyDown(Key);
        }

        /// <summary>
        /// Check if the keybind was just released. (Same behaviour as GetKeyUp)
        /// </summary>
        /// <returns>true, Check if the keybind was just released.</returns>
        /// <example><code source="Examples.cs" region="GetKeybindUp" lang="C#" 
        /// title="Keybind and modifier" /></example>
        public bool GetKeybindUp()
        {
            if (Modifier != KeyCode.None)
            {
                return Input.GetKey(Modifier) && Input.GetKeyUp(Key);
            }

            return Input.GetKeyUp(Key);
        }

        /// <summary>
        /// [DEPRECATED] Checks if the Keybind is being held down.
        /// </summary>
        /// <returns>true, if the Keybind is being held down.</returns>
        /// <example><code source="Examples.cs" region="KeyBindPress" lang="C#" 
        /// title="Keybind and modifier" /></example>
        [Obsolete("IsPressed() is deprecated, just rename it to GetKeybind()", true)]
        public bool IsPressed()
		{
            return GetKeybind();
        }

        /// <summary>
        /// [DEPRECATED] Checks if the Keybind was just pressed.
        /// </summary>
        /// <returns>true, if the Keybind is being pressed.</returns>
        /// <example><code source="Examples.cs" region="KeyBindDown" lang="C#" 
        /// title="Keybind and modifier" /></example>
        [Obsolete("IsDown() is deprecated, just rename it to GetKeybindDown()", true)]
        public bool IsDown()
		{
            return GetKeybindDown();
        }   
        
	}
}
