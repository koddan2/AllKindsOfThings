using System.Text;

namespace DZT.Lib;

public static class FileManagement
{
    private readonly static Random _Rng = new Random();

    public static StreamWriter Writer(string path)
    {
        var writer = new StreamWriter(path, append: false, new UTF8Encoding(false));
        return writer;
    }

    public static void BackupFile(string path, bool overwrite, bool appendRandomString = false)
    {
        var dirName = Path.GetDirectoryName(path)!;
        var fileNameNoExt = Path.GetFileNameWithoutExtension(path);
        var extWithPeriod = Path.GetExtension(path);
        var backupPath = appendRandomString ?
            Path.Combine(dirName, $"{fileNameNoExt}-BAK{_Rng.Next(1000, 9999)}{extWithPeriod}")
            :
            Path.Combine(dirName, $"{fileNameNoExt}-BAK{extWithPeriod}");
        File.Copy(path, backupPath, overwrite);
    }
}
