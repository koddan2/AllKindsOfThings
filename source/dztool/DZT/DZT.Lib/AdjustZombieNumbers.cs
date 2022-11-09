using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace DZT.Lib;
public class AdjustZombieNumbers
{
    private readonly string _pathToInputXmlFile;
    private readonly string _pathToOutputXmlFile;
    private readonly float _factor;

    public AdjustZombieNumbers(string rootDir, string pathToInputXmlFile, string pathToOutputXmlFile, float factor)
    {
        Validators.ValidateDirExists(rootDir);
        _factor = factor;
        _pathToInputXmlFile = Path.Combine(rootDir, pathToInputXmlFile);
        _pathToOutputXmlFile = Path.Combine(rootDir, pathToOutputXmlFile);
        Validators.ValidateFileExists(_pathToInputXmlFile);
    }

    public void Process()
    {
        var xd = XDocument.Load(_pathToInputXmlFile);
        var territories = xd.Root!.Nodes();
        foreach (XElement territory in territories.OfType<XElement>())
        {
            var zones = territory.Nodes();
            foreach (XElement zone in zones.OfType<XElement>())
            {
                var attrs = zone.Attributes();
                foreach (XAttribute attr in attrs)
                {
                    ////Console.WriteLine("{0} = {1}", attr.Name, attr.Value);
                    if (attr.Name == "dmin" || attr.Name == "dmax")
                    {
                        var val = attr.Value;
                        if (int.TryParse(val, out int intval))
                        {
                            attr.Value = ((int)Math.Ceiling(intval * _factor)).ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
        }

        FileManagement.BackupFile(_pathToInputXmlFile, true);
        using var fs = FileManagement.Writer(_pathToOutputXmlFile);
        xd.Save(fs);
    }
}
