using System.Reflection;
using System.Xml.Linq;

namespace DZT.Lib.Helpers;

class DataHelper
{
    public static XDocument GetTypesXml(string rootDir, string mpMissionName)
    {
        var path = Path.Combine(rootDir, "mpmissions", mpMissionName, "db", "types.xml");
        return XDocument.Load(path);
    }

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
