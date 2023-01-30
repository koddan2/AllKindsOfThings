using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAK.Files
{
	public class FileSystemWalker
	{
		public static string? FindFirstDirectoryContainingFileUpwards(string startDirectory, Func<FileInfo, bool> test, uint maxLevels = 32)
		{
			var testDir = Path.GetFullPath(Path.Combine(startDirectory, ".."));
			var counter = maxLevels;
			while (counter-- > 0)
			{
				var di = new DirectoryInfo(testDir);
				var match = di.EnumerateFiles().FirstOrDefault(test);
				if (match is not null)
				{
					return testDir;
				}
				testDir = Path.GetFullPath(Path.Combine(testDir, ".."));
			}

			return default;
		}
	}
}
