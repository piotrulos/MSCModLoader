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
        public static string mscPath;
        public static string modsPath;
        public static Game selectedGame = Game.MSC;
        public static bool is64 = true;
        public static bool dbgPack = false;
        public static string[] packFiles = null;
        public static SelectedAction selectedAction = SelectedAction.Nothing;
    }
}
