using System.Text;

namespace DZT.Lib.Helpers;

public static class FileManagement
{
    private readonly static Random _Rng = new();

    public static StreamWriter Utf8BomWriter(string path)
    {
        var writer = new StreamWriter(path, append: false, new UTF8Encoding(false));
        return writer;
    }

    public record RestoreFileV2Result(string BackupFilePath, bool FileOperationCommitted);
    internal static RestoreFileV2Result TryRestoreFileV2(string rootDir, string relativePath)
    {
        var backupRootDir = Path.Combine(GeneralSetup.ApplicationDirPath, "BACKUP");
        var pathToBackupFile = Path.Combine(backupRootDir, relativePath);
        var destFile = Path.Combine(rootDir, relativePath);

        if (File.Exists(pathToBackupFile))
        {
            File.Copy(pathToBackupFile, destFile, true);
            return new RestoreFileV2Result(pathToBackupFile, true);
        }
        else
        {
            return new RestoreFileV2Result(pathToBackupFile, false);
        }
    }

    public record BackupFileV2Result(string BackupFilePath, bool FileOperationCommitted);
    public static BackupFileV2Result BackupFileV2(string rootDir, string relativePath)
    {
        var backupRootDir = Path.Combine(GeneralSetup.ApplicationDirPath, "BACKUP");
        var pathToBackupFile = Path.Combine(backupRootDir, relativePath);
        var srcFile = Path.Combine(rootDir, relativePath);

        if (File.Exists(pathToBackupFile))
        {
            return new BackupFileV2Result(pathToBackupFile, false);
        }

        if (!Directory.Exists(backupRootDir))
        {
            Directory.CreateDirectory(backupRootDir);
        }
        var pathToBackupFileDir = Path.GetDirectoryName(pathToBackupFile);

        if (pathToBackupFileDir is null)
        {
            throw new ApplicationException($"Unable to determine directory of {pathToBackupFile}");
        }

        if (!Directory.Exists(pathToBackupFileDir))
        {
            Directory.CreateDirectory(pathToBackupFileDir);
        }

        File.Copy(srcFile, pathToBackupFile, false);
        return new BackupFileV2Result(pathToBackupFile, true);
    }

    public static void BackupFile(string path, bool overwrite = false, bool appendRandomString = false)
    {
        var dirName = Path.GetDirectoryName(path);

        if (dirName is null)
        {
            throw new ApplicationException($"Unable to determine directory of {path}");
        }

        var fileNameNoExt = Path.GetFileNameWithoutExtension(path);
        var extWithPeriod = Path.GetExtension(path);
        var backupPath = appendRandomString ?
            Path.Combine(dirName, $"{fileNameNoExt}-BAK{_Rng.Next(1000, 9999)}{extWithPeriod}")
            :
            Path.Combine(dirName, $"{fileNameNoExt}-BAK{extWithPeriod}");
        File.Copy(path, backupPath, overwrite);
    }
}
