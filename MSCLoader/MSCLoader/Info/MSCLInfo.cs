namespace MSCLoader;

internal class MSCLInfo
{
#if MSC_Release
    public static readonly string BuildType = "MSC_Release";
#elif MSC_Debug
    public static readonly string BuildType = "MSC_Debug";
#elif MSC_Mini
    public static readonly string BuildType = "MSC_Mini";
#endif
    //Expand to MWC crap once ready.



}

