using System.Xml.Linq;

namespace DZT.Lib;

public class FixExpansionTypesXml
{
    private readonly string inputFilePath;

    public FixExpansionTypesXml(string rootDir, string inputFilePath)
    {
        this.inputFilePath = Path.Combine(rootDir, inputFilePath);
    }

    public void Process()
    {
        FileManagement.BackupFile(inputFilePath, overwrite: false, appendRandomString: true);
        XDocument xd = XDocument.Load(inputFilePath);
        var types = xd.Root!.Nodes();

        var toLowerOccurenceSubstrings = new[]
        {
            "DOOR",
            "WHEEL",
            "HOOD",
            "LEFT",
            "RIGHT",
            "TRUNK",
        };
        foreach (XElement type in types.OfType<XElement>())
        {
            var attrName = type.Attribute("name")!.Value?.ToUpperInvariant() ?? "";
            if (attrName.StartsWith("EXPANSION_"))
            {
                if (toLowerOccurenceSubstrings.Any(x=>attrName.Contains(x)))
                {
                    var typeNodes = type.Nodes();
                    foreach (XElement typeNode in typeNodes.OfType<XElement>())
                    {
                        if (typeNode.Name == "min")
                        {
                            typeNode.SetValue("1");
                        }
                        if (typeNode.Name == "nominal")
                        {
                            typeNode.SetValue("2");
                        }
                    }
                }
            }
        }

        using var fs = FileManagement.Utf8BomWriter(inputFilePath);
        xd.Save(fs);
    }
}

