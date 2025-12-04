using System.Reflection;

namespace MSCLoader;

internal class MSCLInfo
{
    public static readonly string Version = $"{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}{(Assembly.GetExecutingAssembly().GetName().Version.Build == 0 ? "" : $".{Assembly.GetExecutingAssembly().GetName().Version.Build}")}";
    public static readonly int Build = Assembly.GetExecutingAssembly().GetName().Version.Revision;
#if MSC
    public static readonly string menuAssetVersion = "a565b90a";
    public static readonly string consoleAssetVersion = "d3819c23";
#endif
#if MWC
    public static readonly string menuAssetVersion = "tbd";
    public static readonly string consoleAssetVersion = "tbd";
#endif
#if MSC_Release
    public static readonly string BuildType = "MSC_Release";
    public static readonly string TargetGame = "My Summer Car";
#elif MSC_Debug
    public static readonly string BuildType = "MSC_Debug";
    public static readonly string TargetGame = "My Summer Car (<color=magenta>Debug Build</color>)";
#elif MSC_Mini
    public static readonly string BuildType = "MSC_Mini";
    public static readonly string TargetGame = "MSC (Assets Only)"; //Unusable in game
#elif MWC_Debug
    public static readonly string BuildType = "MWC_Debug";
    public static readonly string TargetGame = "My Winter Car (<color=magenta>Debug Build</color>)";
#elif MWC_Release
    public static readonly string BuildType = "MWC_Release";
    public static readonly string TargetGame = "My Winter Car";
#elif MWC_Mini
    public static readonly string BuildType = "MWC_Mini";
    public static readonly string TargetGame = "MWC (Assets Only)"; //Unusable in game
#endif

}

