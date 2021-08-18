using System;

namespace MSCLoader
{
    public partial class Mod
    {
        //Here is old pre 1.2 functions used here only for backwards compatibility

        /// <summary>
        /// [OBSOLETE] This is no longer in use, you can safely remove that line.
        /// </summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal virtual bool LoadInMenu => false;

        /// <summary>
        /// [OBSOLETE] This is no longer in use, you can safely remove that line.
        /// </summary>        
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal virtual bool UseAssetsFolder { get; set; } = false;

        /// <summary>
        /// [OBSOLETE] This is no longer in use, you can safely remove that line.
        /// </summary>     
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        internal virtual bool SecondPass => false;

        /// <summary>
        /// Called once after starting "New Game"
        /// You can reset/delete your saves here
        /// </summary>
        public virtual void OnNewGame() { }
      
        /// <summary>
        /// [DON'T USE] It's useless
        /// </summary>
        /// 
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [Obsolete("This doesn't do anything")]
        public ModSettings modSettings;
        /// <summary>
        /// Constructor DON'T USE
        /// </summary>
        /// 
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool proSettings;

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

        /// <summary>
        /// Called once in main menu (only when LoadInMenu is true).
        /// </summary>
        public virtual void OnMenuLoad()
        {
            if (LoadInMenu)
                ModConsole.Error(string.Format("<b>LoadInMenu</b> is set to <b>true</b> for mod: <b>{0}</b> but <b>OnMenuLoad()</b> is empty.", ID));
        }

        /// <summary>
        /// Called once as soon as GAME scene is loaded.
        /// </summary>
        public virtual void PreLoad() { }

        /// <summary>
        /// Called once, after GAME scene is fully loaded.
        /// </summary>
        internal virtual void OnLoad() { }

        /// <summary>
        /// Called once, after ALL mods has finished OnLoad() and when SecondPass is set to true
        /// (Executed still before first pass of Update(), but NOT exectued if OnLoad() failed with error)
        /// </summary>
        public virtual void SecondPassOnLoad() { PostLoad(); }

    }
}
