namespace MSCLoader
{
    /// <summary>
    /// The base mod class, all mods should have this class.
    /// </summary>
    /// <example><code source="Examples.cs" region="Mod" lang="C#" 
    /// title="Example Mod Class" /></example>
    public abstract class Mod
	{
        bool disabled = false;
        bool update = false;
        string compiledVer = null;
        int errorCount = 0;

        /// <summary>
        /// Number of Errors/Exceptions thrown by this mod (in Update or FixedUpdate)
        /// If there is too many errors thrown each frame, mod will be disabled to prevent FPS drop.
        /// </summary>
        public virtual int modErrors { get => errorCount; internal set => errorCount = value; }

        /// <summary>
        /// If true, mod will never call OnLoad/Update/OnGui (used only by settings)
        /// </summary>
        public virtual bool isDisabled { get => disabled; internal set => disabled = value; }

        /// <summary>
        /// If true, mod will indicate that there is update available.
        /// </summary>
        public virtual bool hasUpdate { get => update; set => update = value; }

        /// <summary>
        /// Load this mod in Main Menu.
        /// (in most cases should be false, use only if you need this).
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
        /// The name of mod that will be displayed in settings.
        /// (if not set, will be same as ID)
        /// </summary>
        public virtual string Name => ID;

        /// <summary>
        /// The current version of the mod.
        /// (prefered standard version format: 2, 3 or 4 digits)
        /// </summary>
        public abstract string Version { get; }
   
        /// <summary>
        /// Compiled MSCLoader version 
        /// (for what version this mod was compiled, visible in settings)
        /// </summary>
        public virtual string compiledVersion { get => compiledVer; set => compiledVer = value; }

        /// <summary>
        /// Author of the mod
        /// (Enter your nickname in this variable)
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// All settings should be created here.
        /// </summary>
        public virtual void ModSettings() { }

        /// <summary>
        /// Called after saved settings is loaded from file.
        /// </summary>
        public virtual void ModSettingsLoaded() { }


        /// <summary>
        /// Called once in main menu (only when LoadInMenu is true).
        /// </summary>
        public virtual void OnMenuLoad() {
            if (LoadInMenu)
                ModConsole.Error(string.Format("<b>LoadInMenu</b> is set to <b>true</b> for mod: <b>{0}</b> but <b>OnMenuLoad()</b> is empty.",ID));
        }

        /// <summary>
        /// Called once after starting "New Game"
        /// You can reset/delete your saves here
        /// </summary>
        public virtual void OnNewGame() { }

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
        /// <example>See: https://docs.unity3d.com/530/Documentation/Manual/GUIScriptingGuide.html
        /// </example>
        public virtual void OnGUI() { }

		/// <summary>
		/// Called once every frame
        /// (standard unity Update()).
		/// </summary>
		public virtual void Update() { }

        /// <summary>
        /// Called once every fixed frame 
        /// (standard unity FixedUpdate()).
        /// </summary>
        public virtual void FixedUpdate() { }
    }
}
