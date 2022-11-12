namespace DZT.Lib;
public class CleanUpConfig
{
    private readonly string _rootDir;

    public CleanUpConfig(string rootDir)
    {
        _rootDir = rootDir;
    }

    public void Process()
    {
        var configDir = Path.Combine(_rootDir, "config");
    }
}