using System.Reflection;

namespace MSCLoader;

internal class MSCLInfo
{
    public static readonly string Version = $"{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}{(Assembly.GetExecutingAssembly().GetName().Version.Build == 0 ? "" : $".{Assembly.GetExecutingAssembly().GetName().Version.Build}")}";
    public static readonly int Build = Assembly.GetExecutingAssembly().GetName().Version.Revision;
#if MSC
    public static readonly string menuAssetVersion = "9854c1c0";
    public static readonly string consoleAssetVersion = "d3819c23";
    public static readonly string coreAssetsPath = "MSCLoader.CoreAssets.core_msc.unity3d";
#endif
#if MWC
    public static readonly string menuAssetVersion = "ced97cbb";
    public static readonly string consoleAssetVersion = "7099769d";
    public static readonly string coreAssetsPath = "MSCLoader.CoreAssets.core_mwc.unity3d";
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

