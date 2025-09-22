using System.Reflection;

namespace MSCLoader;

internal class MSCLInfo
{
    public static readonly string Version = $"{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}{(Assembly.GetExecutingAssembly().GetName().Version.Build == 0 ? "" : $".{Assembly.GetExecutingAssembly().GetName().Version.Build}")}";
    public static readonly int Build = Assembly.GetExecutingAssembly().GetName().Version.Revision;
#if MSC_Release
    public static readonly string BuildType = "MSC_Release";
    public static readonly string TargetGame = "My Summer Car";
#elif MSC_Debug
    public static readonly string BuildType = "MSC_Debug";
    public static readonly string TargetGame = "My Summer Car (<color=magenta>Debug Build</color>)";
#elif MSC_Mini
    public static readonly string BuildType = "MSC_Mini";
    public static readonly string TargetGame = "MSC (Assets Only)"; //Unusable in game
#endif
    //Expand to MWC crap once ready.


}

