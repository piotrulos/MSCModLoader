namespace MSCLInstaller
{
    enum SelectedAction
    {
        Nothing,
        ChangeModsFolder,
        InstallMSCLoader,
        UpdateMSCLoader,
        ReinstallMSCLoader,
        UninstallMSCLoader,
        AdvancedOptions
    }
    internal enum ModsFolder
    {
        GameFolder,
        MyDocuments,
        Appdata
    }
    internal class Storage
    {
        public static string currentPath;
        public static string gamePath;
        public static string modsPath;
        public static Game selectedGame = Game.MSC;
        public static bool is64 = true; //MSC only
        public static bool dbgPack = false;
        public static string[] packFiles = null;
        public static SelectedAction selectedAction = SelectedAction.Nothing;
        public static bool skipIntroCfg = false;
        public static bool skipConfigScreenCfg = false;
        public static ModsFolder modsFolderCfg = ModsFolder.GameFolder;

        public static readonly string[] requiredDlls =
        {
            "INIFileParser.dll",
            "Ionic.Zip.Reduced.dll"
        };
    }
}
