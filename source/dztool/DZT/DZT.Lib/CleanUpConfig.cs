namespace DZT.Lib;

public class CleanUpProfile
{
    private readonly string _rootDir;
    private readonly string _profileDirectoryName = "config";
    private readonly string _profileDirectory;

    private long _totalRem = 0;

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
        Console.WriteLine(
            "Deleted a total of {0} kibibytes",
            _totalRem == 0 ? 0 : _totalRem / 1024
        );
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
            var ephemeralFile = ext is ".rpt" || ext is ".log" || ext is ".adm" || ext is ".mdmp";
            if (ephemeralFile)
            {
                DoDelete(fileName);
            }
        }
    }

    private void DoDelete(string fileName)
    {
        var fi = new FileInfo(fileName);
        var size = fi.Length;
        _totalRem += size;
        File.Delete(fileName);
        Console.WriteLine("Deleted file: {0}", fileName);
    }
}
