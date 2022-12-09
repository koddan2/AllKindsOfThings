using SAK;
using System.Xml.Linq;

namespace DZT.Lib.Helpers;

public static class DayzFilesHelper
{
    public static IEnumerable<string> GetAllSpawnableTypesXmlFileNames(string rootDir, string mpMissionName)
    {
        var files = new MpMissionFiles(rootDir, mpMissionName);
        var baseXmlFilePath = Path.Combine(files.PathToMpMissionDirectory, DayzConstants.SubdirectoryNames.Db, DayzConstants.FileNames.CfgSpawnableTypes);
        yield return baseXmlFilePath;

        foreach (var also in GetCentralEconomyXmlFileNames(rootDir, mpMissionName, "spawnabletypes"))
        {
            yield return also;
        }
    }

    public static IEnumerable<string> GetAllTypesXmlFileNames(string rootDir, string mpMissionName)
    {
        var files = new MpMissionFiles(rootDir, mpMissionName);
        var mpMissionRoot = files.PathToMpMissionDirectory;
        var baseDbTypesXmlFilePath = Path.Combine(mpMissionRoot, DayzConstants.SubdirectoryNames.Db, DayzConstants.FileNames.Types);
        yield return baseDbTypesXmlFilePath;

        foreach (var also in GetCentralEconomyXmlFileNames(rootDir, mpMissionName, "types"))
        {
            yield return also;
        }
    }

    public static IEnumerable<string> GetCentralEconomyXmlFileNames(string rootDir, string mpMissionName, string ceType = "types")
    {
        var files = new MpMissionFiles(rootDir, mpMissionName);
        var ceElements = files.GetCentralEconomyElements();
        foreach (var ceElement in ceElements)
        {
            var ceDirName = ceElement.Attribute("folder").OrFail().Value;
            var ceDirPath = Path.Combine(files.PathToMpMissionDirectory, ceDirName);

            foreach (var ceElementFileElement in ceElement.Nodes().OfType<XElement>())
            {
                if (ceElementFileElement.Attribute("type")?.Value is string fileType && fileType == ceType)
                {
                    var maybeFileName = ceElementFileElement.Attribute("name")?.Value;
                    if (maybeFileName is string fileName)
                    {
                        var file = Path.Combine(ceDirPath, fileName);
                        if (!File.Exists(file))
                        {
                            throw new ApplicationException($"File {maybeFileName} does not exist.");
                        }

                        yield return file;
                    }
                }
            }
        }
    }
}

