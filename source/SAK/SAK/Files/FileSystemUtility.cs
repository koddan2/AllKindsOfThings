
namespace SAK.Files;

public static class FileSystemUtility
{
    public static List<string> GetFilesRecursive(this DirectoryInfo di)
    {
        return GetFilesRecursive(di.FullName);
    }

    /// <summary>
    /// https://stackoverflow.com/a/4195909
    /// </summary>
    /// <param name="directoryPath"></param>
    /// <returns>The list of all files.</returns>
    public static List<string> GetFilesRecursive(string directoryPath)
    {
        // 1.
        // Store results in the file results list.
        List<string> result = new();

        // 2.
        // Store a stack of our directories.
        Stack<string> stack = new();

        // 3.
        // Add initial directory.
        stack.Push(directoryPath);

        // 4.
        // Continue while there are directories to process
        while (stack.Count > 0)
        {
            // A.
            // Get top directory
            string currentDirectory = stack.Pop();

            try
            {
                if (!Directory.Exists(currentDirectory))
                {
                    continue;
                }

                // B
                // Add all files at this directory to the result List.
                result.AddRange(Directory.GetFiles(currentDirectory, "*.*"));

                // C
                // Add all directories at this directory.
                foreach (string subDirectories in Directory.GetDirectories(currentDirectory))
                {
                    stack.Push(subDirectories);
                }
            }
            catch
            {
                // D
                // Could not open the directory - we just ignore this exception here, by design.
            }
        }

        return result;
    }
}
