#if !Mini
using System;

namespace MSCLoader;

public partial class Mod
{
    //Here is old pre 1.2 functions used here only for backwards compatibility
    //Obsoleted (internal doesn't break old code)

    internal virtual bool LoadInMenu => false;
    internal virtual void OnNewGame() { }

    /// <summary>
    /// [DON'T USE] 
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [Obsolete("This doesn't do anything")]
    public ModSettings modSettings;

    internal bool proSettings;
    internal virtual void MenuOnLoad() { }
    internal virtual void PostLoad() { }
    internal virtual void OnSave() { }
    internal virtual void OnGUI() { }
    internal virtual void Update() { }
    internal virtual void FixedUpdate() { }
    internal virtual void OnMenuLoad()
    {
        if (LoadInMenu)
            ModConsole.Error($"<b>LoadInMenu</b> is set to <b>true</b> for mod: <b>{ID}</b> but <b>OnMenuLoad()</b> is empty.");
    }
    internal virtual void PreLoad() { }
    internal virtual void OnLoad() { }
    internal virtual void SecondPassOnLoad() { PostLoad(); }
    internal virtual void OnModEnabled() { }
    internal virtual void OnModDisabled() { }
    internal virtual void ModSettings() { }
    internal virtual void ModSettingsLoaded() { }

    /// <summary>
    /// Constructor only for compatibiltiy for pro settings.
    /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
    public Mod() => modSettings = new ModSettings(this);
#pragma warning restore CS0618 // Type or member is obsolete
}
#endif