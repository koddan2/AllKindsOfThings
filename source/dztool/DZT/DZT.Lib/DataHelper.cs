using System.Reflection;

namespace DZT.Lib;
class DataHelper
{
    public static IDictionary<string, IEnumerable<string>> GetStructureClassNames()
    {
        Assembly asm = Assembly.GetExecutingAssembly();
        var rootDir = Path.GetDirectoryName(asm.Location)!;
        var dataDir = Path.Combine(rootDir, "DATA");
        var structureTextFile = Path.Combine(dataDir, "structureRelatedClassNames.txt");

        var result = new Dictionary<string, IEnumerable<string>>();
        using var reader = new StreamReader(structureTextFile);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine()!;
            var category = "";
            if (line.StartsWith("**"))
            {
                category = line;
            }

            ////var lastLine = "";
            var acc = new List<string>();
            while (!reader.EndOfStream && line.Length > 0)
            {
                line = reader.ReadLine()!;
                line = line.Replace("\"", "").Replace(",", "");
                if (line.Length > 0)
                {
                    acc.Add(line);
                    ////lastLine = line;
                }
            }
            result[category] = acc;
        }

        return result;
    }
}
