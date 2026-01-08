namespace MSCLoader;

/// <summary>
/// The base mod class, all mods should have this class.
/// </summary>
public abstract partial class Mod
{
#if !Mini
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
    /// List of games supported by this mod
    /// </summary>
    public virtual Game SupportedGames => Game.MySummerCar;

    /// <summary>
    /// Setup your mod. IMPORTANT! Only SetupFunction() is allowed here
    /// Please disclose AI generated code in Description and AssemblyInfo inside AssemblyTrademark field
    /// </summary>
    public virtual void ModSetup() { }

#endif

}
