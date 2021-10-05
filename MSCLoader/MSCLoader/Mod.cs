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
        /// Mod Icon from Resources or Embedded Resources.
        /// </summary>
        public virtual byte[] Icon { get; set; } = null;

        /// <summary>
        /// Short Description of your mod
        /// </summary>
        public virtual string Description { get; set; } = null;

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
        /// Called once when mod has been enabled
        /// </summary>
        public virtual void OnModEnabled() { }

        /// <summary>
        /// Called once when mod has been disabled
        /// </summary>
        public virtual void OnModDisabled() { }

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
        public Mod() => modSettings = new ModSettings(this);
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
