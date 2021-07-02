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
        string filePath = null;
        int errorCount = 0;
        ModsManifest modMetadata = null;
        ModsManifest rmodMetadata = null;

        internal virtual int modErrors { get => errorCount; set => errorCount = value; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public virtual bool isDisabled { get => disabled; internal set => disabled = value; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        internal virtual bool hasUpdate { get => update; set => update = value; }

        internal virtual string compiledVersion { get => compiledVer; set => compiledVer = value; }

        internal virtual string fileName { get => filePath; set => filePath = value; }

        internal virtual ModsManifest metadata { get => modMetadata; set => modMetadata = value; }
        internal virtual ModsManifest RemMetadata { get => rmodMetadata; set => rmodMetadata = value; }

        /// <summary>
        /// Load this mod in Main Menu.
        /// (in most cases should be false, use only if you need this).
        /// </summary>
        public virtual bool LoadInMenu => false;

        /// <summary>
        /// Set this to true if you want load custom files from Assets folder
        /// (This will create a subfolder for your mod)
        /// </summary>
        public virtual bool UseAssetsFolder { get; set; } = false;

        /// <summary>
        /// Enable SecondPassOnLoad() that will execute after all mods have been loaded.
        /// </summary>
        public virtual bool SecondPass => false;

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
        /// All settings should be created here.
        /// </summary>
        public virtual void ModSettings() { }

        /// <summary>
        /// Called after saved settings is loaded from file.
        /// </summary>
        public virtual void ModSettingsLoaded() { }

        /// <summary>
        /// Called once after starting "New Game"
        /// You can reset/delete your saves here
        /// </summary>
        public virtual void OnNewGame() { }

        /// <summary>
        /// Called once in main menu (only when LoadInMenu is true).
        /// </summary>
        public virtual void OnMenuLoad()
        {
            if (LoadInMenu)
                ModConsole.Error(string.Format("<b>LoadInMenu</b> is set to <b>true</b> for mod: <b>{0}</b> but <b>OnMenuLoad()</b> is empty.", ID));
         //   MenuOnLoad();
        }

        /// <summary>
        /// Called once as soon as GAME scene is loaded.
        /// </summary>
        public virtual void PreLoad() { }

        /// <summary>
        /// Called once, after GAME scene is fully loaded.
        /// </summary>
        public virtual void OnLoad() { }

        /// <summary>
        /// Called once, after ALL mods has finished OnLoad() and when SecondPass is set to true
        /// (Executed still before first pass of Update(), but NOT exectued if OnLoad() failed with error)
        /// </summary>
        public virtual void SecondPassOnLoad() { PostLoad(); }

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

        /// <summary>
        /// Called once, when save and quit.
        /// </summary>
        public virtual void OnSave() { }

        /// <summary>
        /// Standard unity OnGUI().
        /// </summary>
        /// <example>See: https://docs.unity3d.com/500/Documentation/Manual/GUIScriptingGuide.html
        /// </example>
        public virtual void OnGUI() { MenuOnGUI(); }

        /// <summary>
        /// Called once every frame
        /// (standard unity Update()).
        /// </summary>
        public virtual void Update() { MenuUpdate(); }

        /// <summary>
        /// Called once every fixed frame 
        /// (standard unity FixedUpdate()).
        /// </summary>
        public virtual void FixedUpdate() { MenuFixedUpdate(); }

        #region ProRedirectsAndDummys
        /// <summary>
        /// CompLayer DON'T USE
        /// </summary>
        /// 
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ModSettings modSettings;
        /// <summary>
        /// Constructor DON'T USE
        /// </summary>
        /// 
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool proSettings;
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

        /// <summary>
        /// resources Icon.
        /// </summary>
        public virtual byte[] Icon { get; set; } = null;
        /// <summary>
        /// Compatibility only: does nothing
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual string UpdateLink { get; } = "";

        /// <summary>
        /// Compatibility only: same as OnMenuLoad()
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual void MenuOnLoad() { }

        /// <summary>
        /// Compatibility only: same as OnGUI()
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual void MenuOnGUI() { }

        /// <summary>
        /// Compatibility only: same as Update()
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual void MenuUpdate() { }

        /// <summary>
        /// Compatibility only: same as FixedUpdate()
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual void MenuFixedUpdate() { }

        /// <summary>
        /// Compatibility only: same as SecondPassOnLoad()
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public virtual void PostLoad() { }
#endregion
    }
}
