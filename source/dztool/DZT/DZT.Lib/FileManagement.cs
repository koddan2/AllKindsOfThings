using System.Text;

namespace DZT.Lib;

public static class FileManagement
{
    public static StreamWriter Writer(string path)
    {
        var writer = new StreamWriter(path, append: false, new UTF8Encoding(false));
        return writer;
    }

    public static void BackupFile(string path, bool overwrite)
    {
        var dirName = Path.GetDirectoryName(path)!;
        var fileNameNoExt = Path.GetFileNameWithoutExtension(path);
        var extWithPeriod = Path.GetExtension(path);
        var backupPath = Path.Combine(dirName, $"{fileNameNoExt}-BAK{extWithPeriod}");
        File.Copy(path, backupPath, overwrite);
    }
}
