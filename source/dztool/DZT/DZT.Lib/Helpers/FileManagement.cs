using System.Text;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace DZT.Lib.Helpers;

public static class FileManagement
{
    private readonly static Random _Rng = new();
    private readonly static HashAlgorithm _HashAlgorithm = SHA1.Create();
    private readonly static string _BackupSubDirectoryName = "BACKUP";

    public static StreamWriter Utf8WithoutBomWriter(string path, bool append = false)
    {
        var writer = new StreamWriter(path, append: append, new UTF8Encoding(false));
        return writer;
    }

    public record RestoreFileV2Result(string BackupFilePath, bool FileOperationCommitted);
    internal static RestoreFileV2Result TryRestoreFileV2(string rootDir, string relativePath)
    {
        var backupRootDir = Path.Combine(rootDir, GeneralInvariants.ApplicationSubDirectoryName, _BackupSubDirectoryName);
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
        var backupRootDir = Path.Combine(rootDir, GeneralInvariants.ApplicationSubDirectoryName, "BACKUP");
        var pathToBackupFile = Path.Combine(backupRootDir, relativePath);
        var srcFile = Path.Combine(rootDir, relativePath);

        if (!HasSameContent(pathToBackupFile, srcFile))
        {
            BackupBackupFile(pathToBackupFile);
        }

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

    public static void FormatXmlFileInPlace(string filePath)
    {
        var xd = XDocument.Load(filePath);
        using var tempfs = FileManagement.Utf8WithoutBomWriter(filePath);
        xd.Save(tempfs);
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

    private static void BackupBackupFile(string filePath)
    {
        var dirName = Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException($"Could not get directory name of {filePath}");
        var fileNameNoExt = Path.GetFileNameWithoutExtension(filePath);
        var extension = Path.GetExtension(filePath);

        var backupBackupFile = Path.Combine(dirName, fileNameNoExt, $"{DateTimeOffset.Now:O}", extension);
        File.Copy(filePath, backupBackupFile, true);
    }

    private static bool HasSameContent(string file1, string file2)
    {
        using var fileStream1 = File.OpenRead(file1);
        using var fileStream2 = File.OpenRead(file2);
        var hash1 = BitConverter.ToString(_HashAlgorithm.ComputeHash(fileStream1));
        var hash2 = BitConverter.ToString(_HashAlgorithm.ComputeHash(fileStream2));

        return hash1.Equals(hash2);
    }
}
