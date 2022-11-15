using SAK;
using System.Xml.Linq;

namespace DZT.Lib.Helpers;

public static class DayzFilesHelper
{
    public static IEnumerable<string> GetAllTypesXmlFileNames(string rootDir, string mpMissionName)
    {
        var mpMissionRoot = Path.Combine(rootDir, DayzConstants.SubdirectoryNames.MpMissions, mpMissionName);
        var baseDbTypesXmlFilePath = Path.Combine(mpMissionRoot, DayzConstants.SubdirectoryNames.Db, DayzConstants.FileNames.Types);
        yield return baseDbTypesXmlFilePath;

        var cfgEconomyCoreXmlPath = Path.Combine(mpMissionRoot, DayzConstants.FileNames.CfgEconomyCore);
        var cfgEconomyCore = XDocument.Load(cfgEconomyCoreXmlPath);
        var ceElements = cfgEconomyCore.Root.OrFail()
            .Nodes().OfType<XElement>().Where(x => x.Name == "ce");
        foreach (var ceElement in ceElements)
        {
            var ceDirName = ceElement.Attribute("folder").OrFail().Value;
            var ceDirPath = Path.Combine(mpMissionRoot, ceDirName);

            foreach (var ceElementFileElement in ceElement.Nodes().OfType<XElement>())
            {
                if (ceElementFileElement.Attribute("type")?.Value is string fileType && fileType == "types")
                {
                    var maybeFileName = ceElementFileElement.Attribute("name")?.Value;
                    if (maybeFileName is string fileName)
                    {
                        var file = Path.Combine(ceDirPath, fileName);
                        yield return file;
                    }
                }
            }
        }
    }
}

