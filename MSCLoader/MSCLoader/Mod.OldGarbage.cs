#if !Mini
using System;

namespace MSCLoader;

public partial class Mod
{
    //Here is old pre 1.2 functions used here only for backwards compatibility

    /// <summary>
    /// Load this mod in Main Menu.
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
            ModConsole.Error($"<b>LoadInMenu</b> is set to <b>true</b> for mod: <b>{ID}</b> but <b>OnMenuLoad()</b> is empty.");
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
    /// Called once when mod has been enabled in settings
    /// </summary>
    public virtual void OnModEnabled() { }

    /// <summary>
    /// Called once when mod has been disabled in settings
    /// </summary>
    public virtual void OnModDisabled() { }

    /// <summary>
    /// All settings should be created here.
    /// </summary>
    public virtual void ModSettings() { }

    /// <summary>
    /// Called after saved settings habe been loaded from file.
    /// </summary>
    public virtual void ModSettingsLoaded() { }

    /// <summary>
    /// pro BS
    /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
    public Mod() => modSettings = new ModSettings(this);
#pragma warning restore CS0618 // Type or member is obsolete
}
#endif