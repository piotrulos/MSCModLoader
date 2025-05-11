using System.Reflection;

namespace MSCLoader;

internal class MSCLInfo
{
    public static readonly string Version = $"{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}{(Assembly.GetExecutingAssembly().GetName().Version.Build == 0 ? "" : $".{Assembly.GetExecutingAssembly().GetName().Version.Build}")}";
    public static readonly int Build = Assembly.GetExecutingAssembly().GetName().Version.Revision;
#if MSC_Release
    public static readonly string BuildType = "MSC_Release";
#elif MSC_Debug
    public static readonly string BuildType = "MSC_Debug";
#elif MSC_Mini
    public static readonly string BuildType = "MSC_Mini";
#endif
    //Expand to MWC crap once ready.


}

