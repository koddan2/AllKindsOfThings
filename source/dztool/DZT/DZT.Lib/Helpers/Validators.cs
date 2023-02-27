namespace DZT.Lib.Helpers;

public static class Validators
{
    public static void ValidateDirExists(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new ApplicationException($"The directory {path} does not exist");
        }
    }

    public static void ValidateFileExists(string path)
    {
        if (!File.Exists(path))
        {
            throw new ApplicationException($"The file {path} does not exist");
        }
    }
}
