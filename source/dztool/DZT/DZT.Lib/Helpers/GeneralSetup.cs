namespace DZT.Lib.Helpers
{
    public static class GeneralSetup
    {
        internal static string ApplicationDirPath = "";

        public static void Initialize(string rootDir)
        {
            Validators.ValidateDirExists(rootDir);
            const string applicationFolderName = ".dzt";
            ApplicationDirPath = Path.Combine(rootDir, applicationFolderName);
            if (!Directory.Exists(ApplicationDirPath))
            {
                Directory.CreateDirectory(ApplicationDirPath);
            }
        }
    }
}
