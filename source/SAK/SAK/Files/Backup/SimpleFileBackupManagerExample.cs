namespace SAK.Files.Backup;

public interface IPossibleSideEffectFileResult
{
    bool Performed { get; }
}

public interface IBackupFileResult : IPossibleSideEffectFileResult { }
public record SuccessfulBackupFileResult(string BackedUpFilePath, bool Overwritten) : IBackupFileResult
{
    public bool Performed => true;
}
public enum BackupFailedReason
{
    SourceFileDoesNotExist = 1,
    Unknown = 2,
    Aborted = 3,
}
public record FailedBackupFileResult(BackupFailedReason Reason, string Message) : IBackupFileResult
{
    public bool Performed => false;
}

public record RestoreFileResult(bool Performed, string BackedUpFilePath, string RestoredFilePath);

public enum OnConflict
{
    Abort = 1,
    Overwrite,
}

#if DEBUG
public class SimpleFileBackupManagerExample
{
    public static void Example1()
    {
        var sfbm = new SimpleFileBackupManager(@"C:\my-dir");
        var result = sfbm.BackupFile("sub/directory/file.txt", OnConflict.Overwrite);
        if (result is SuccessfulBackupFileResult successful)
        {
            Console.WriteLine("{0} - was overwritten? -> {1}", successful.BackedUpFilePath, successful.Overwritten);
        }
        else if (result is FailedBackupFileResult failed)
        {
            Console.WriteLine("ERROR ({0}): {1}", failed.Reason, failed.Message);
        }
    }
}
#endif

public class SimpleFileBackupManager
{
    public SimpleFileBackupManager(string rootDirectory)
    {
        RootDirectory = rootDirectory;
    }

    public string RootDirectory { get; }
    public string BackupDirectoryName { get; set; } = "backup";
    public string BackupDirectoryPath => Path.Combine(RootDirectory, BackupDirectoryName);

    public IBackupFileResult BackupFile(string relativePath, OnConflict onConflict)
    {
        var sourceFilePath = Path.Combine(RootDirectory, relativePath);
        if (!File.Exists(sourceFilePath))
        {
            return new FailedBackupFileResult(BackupFailedReason.SourceFileDoesNotExist, "The supplied path does not resolve to an existing file.");
        }

        EnsureBackupDirectory();
        var backupFilePath = GetBackupFilePath(relativePath);
        if (File.Exists(backupFilePath) && onConflict == OnConflict.Abort)
        {
            return new FailedBackupFileResult(BackupFailedReason.Aborted, $"A file already exists at {backupFilePath} and onConflict is set to {onConflict}");
        }

        try
        {
            var exists = File.Exists(backupFilePath);
            File.Copy(sourceFilePath, backupFilePath, onConflict == OnConflict.Overwrite);
            return new SuccessfulBackupFileResult(backupFilePath, exists);
        }
        catch (Exception e)
        {
            return new FailedBackupFileResult(BackupFailedReason.Unknown, e.Message);
        }
    }

    private string GetBackupFilePath(string relativePath)
    {
        return Path.Combine(BackupDirectoryPath, relativePath);
    }

    private void EnsureBackupDirectory()
    {
        if (!Directory.Exists(BackupDirectoryPath))
        {
            Directory.CreateDirectory(BackupDirectoryPath);
        }
    }
}
