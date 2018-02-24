namespace MSCLoader
{
    /// <summary>
    /// The base mod class, all mods should derive this.
    /// </summary>
    /// <example><code source="Examples.cs" region="Mod" lang="C#" 
    /// title="Example Mod Class" /></example>
    public abstract class Mod
	{
        bool disabled = false;
        bool update = false;
        string compiledVer = null;

        /// <summary>
        /// If true, mod will never call OnLoad (used only by settings)
        /// </summary>
        public virtual bool isDisabled { get => disabled; set => disabled = value; }

        /// <summary>
        /// If true, mod will indicate that there is update available.
        /// </summary>
        public virtual bool hasUpdate { get => update; set => update = value; }

        /// <summary>
        /// Load this mod in Main Menu (in most cases should be false).
        /// </summary>
        public virtual bool LoadInMenu => false;

        /// <summary>
        /// Set this to true if you want load custom files from Assets folder
        /// (This will create a subfolder for your mod)
        /// </summary>
        public virtual bool UseAssetsFolder => false;

        /// <summary>
        /// The ID for your mod (This should be unique).
        /// </summary>
        public abstract string ID { get; }

        /// <summary>
        /// The name that will be displayed.
        /// (if not set, will be same as ID)
        /// </summary>
        public virtual string Name => ID;

        /// <summary>
        /// The current version of the mod.
        /// (prefered standard version format 2, 3 or 4 digit)
        /// </summary>
        public abstract string Version { get; }
   
        /// <summary>
        /// Compiled MSCLoader version 
        /// </summary>
        public virtual string compiledVersion { get => compiledVer; set => compiledVer = value; }

        /// <summary>
        /// Author of the mod
        /// (Enter your nickname in this variable)
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// Called once, when the mod is loaded in MainMenu scene (only when LoadInMenu is true).
        /// </summary>
        public virtual void OnMenuLoad() {
            if (LoadInMenu)
                ModConsole.Error(string.Format("<b>LoadInMenu</b> is set to <b>true</b> for mod: <b>{0}</b> but <b>OnMenuLoad()</b> is empty.",ID));
        }

        /// <summary>
        /// Called once, after GAME scene is fully loaded.
        /// </summary>
        public virtual void OnLoad() { }

        /// <summary>
        /// Called once, when save and quit.
        /// </summary>
        public virtual void OnSave() { }

        /// <summary>
        /// Standard unity OnGUI().
        /// </summary>
        /// <example>see https://docs.unity3d.com/500/Documentation/Manual/GUIScriptingGuide.html</example>
        public virtual void OnGUI() { }

		/// <summary>
		/// Called once every frame (standard unity Update()).
		/// </summary>
		public virtual void Update() { }

        /// <summary>
        /// Called once every fixed frame (standard unity FixedUpdate()).
        /// </summary>
        public virtual void FixedUpdate() { }
    }
}
