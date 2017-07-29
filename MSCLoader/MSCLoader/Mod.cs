namespace MSCLoader
{
    /// <summary>
    /// The base mod class, all mods should derive this.
    /// </summary>
    public abstract class Mod
	{
        /// <summary>
        /// Load this mod in Main Menu (in most cases should be false).
        /// </summary>
        public virtual bool LoadInMenu => false;   
      
        /// <summary>
        /// The ID for your mod (This should be unique).
        /// </summary>
        public abstract string ID { get; }

        /// <summary>
        /// The name that will be displayed.
        /// </summary>
        public virtual string Name => ID;

        /// <summary>
        /// The current version of the mod.
        /// </summary>
        public abstract string Version { get; }

		/// <summary>
		/// The name of the author.
		/// </summary>
		public abstract string Author { get; }


		/// <summary>
		/// Called when the mod is loaded.
		/// </summary>
		public virtual void OnLoad() { }

		/// <summary>
		/// Called to draw the obsolete GUI.
		/// </summary>
		public virtual void OnGUI() { }

		/// <summary>
		/// Called every tick.
		/// </summary>
		public virtual void Update() { }
	}
}
