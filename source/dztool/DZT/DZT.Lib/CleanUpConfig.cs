namespace DZT.Lib;

public class CleanUpProfile
{
    private readonly string _rootDir;
    private readonly string _profileDirectoryName = "config";
    private readonly string _profileDirectory;

    public CleanUpProfile(string rootDir, string profileDirectoryName)
    {
        _rootDir = rootDir;
        _profileDirectoryName = profileDirectoryName;
        _profileDirectory = Path.Combine(_rootDir, _profileDirectoryName);
    }

    public void Process()
    {
        CleanUpCoreDayZFiles();
        CleanUpAllLogs();
    }

    private void CleanUpAllLogs()
    {
        var allLogFiles = Directory.GetFiles(
            _profileDirectory,
            "*.log",
            SearchOption.AllDirectories
        );
        foreach (var fileName in allLogFiles)
        {
            if (Path.GetExtension(fileName).ToLower() is ".log")
            {
                DoDelete(fileName);
            }
        }
    }

    private void CleanUpCoreDayZFiles()
    {
        var fileNames = Directory.EnumerateFiles(_profileDirectory);
        foreach (var fileName in fileNames.ToArray())
        {
            var ext = Path.GetExtension(fileName).ToLower();
            var ephemeralFile = ext is ".rpt" || ext is ".log" || ext is ".adm";
            if (ephemeralFile)
            {
                DoDelete(fileName);
            }
        }
    }

    private static void DoDelete(string fileName)
    {
        File.Delete(fileName);
        Console.WriteLine("Deleted file: {0}", fileName);
    }
}
