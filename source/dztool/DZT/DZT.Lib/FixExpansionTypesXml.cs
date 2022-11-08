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
        foreach (XElement type in types.OfType<XElement>())
        {
            var attrName = type.Attribute("name")!.Value;
            if (attrName.StartsWith("Expansion"))
            {
                if (attrName.Contains("Wheel") || attrName.Contains("Door"))
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

        using var fs = FileManagement.Writer(inputFilePath + "-NEW");
        xd.Save(fs);
    }
}

