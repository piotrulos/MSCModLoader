namespace MSCLoader;

internal class References
{
    #if !Mini
    public string AssemblyID;
    public string AssemblyTitle;
    public string AssemblyDescription;
    public string AssemblyAuthor;
    public string AssemblyFileVersion;
    public string FileName;
    public string Guid; //Conflict check
    public bool Invalid = false;
    public string ExMessage;
    public RefVersion UpdateInfo;
    #endif
}

