namespace MSCLoader
{
    /// <summary>
    /// The base mod class, all mods should have this class.
    /// </summary>
    /// <example><code source="Examples.cs" region="Mod" lang="C#" 
    /// title="Example Mod Class" /></example>
    public abstract partial class Mod
    {
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
        /// Author of the mod
        /// (Enter your nickname in this variable)
        /// </summary>
        public abstract string Author { get; }

        /// <summary>
        /// resources Icon.
        /// </summary>
        public virtual byte[] Icon { get; set; } = null;

        /// <summary>
        /// Setup your mod.
        /// </summary>
        public virtual void ModSetup() { }

        /// <summary>
        /// All settings should be created here.
        /// </summary>
        public virtual void ModSettings() { }

        /// <summary>
        /// Called after saved settings is loaded from file.
        /// </summary>
        public virtual void ModSettingsLoaded() { }

        /// <summary>
        /// Called once when user enable this mod in settings.
        /// </summary>
        public virtual void OnModEnabled() { }

        /// <summary>
        /// Called once when user disable this mod in settings.
        /// </summary>
        public virtual void OnModDisabled() { }

        /// <summary>
        /// Method called whenever mod settings are open.
        /// </summary>
        public virtual void ModSettingsOpen() { }

        /// <summary>
        /// Method called whenever mod settings are closed.
        /// </summary>
        public virtual void ModSettingsClose() { }



        #region ProRedirectsAndDummys
 
        /// <summary>
        /// Constructor DON'T USE
        /// </summary>      
        public Mod()
        {
            modSettings = new ModSettings(this);
        }
        /// <summary>
        /// Compatibility only: does nothing
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual string Description { get; set; } = "";

#endregion
    }
}
