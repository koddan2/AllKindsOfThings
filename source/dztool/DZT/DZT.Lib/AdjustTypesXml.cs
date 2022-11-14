using System.Xml.Linq;
using DZT.Lib.Helpers;
using DZT.Lib.Models;
using Microsoft.Extensions.Logging;
using SAK;

namespace DZT.Lib;

public class AdjustTypesXml
{
    private readonly string _mpMissionDir;
    private readonly XDocument _cfgEconomyCore;
    private readonly ILogger<AdjustTypesXml> _logger;
    private readonly string _rootDir;
    private readonly string _baseDbTypesXmlFilePath;

    public AdjustTypesXml(ILogger<AdjustTypesXml> logger, string rootDir, string mpMissionName)
    {
        _mpMissionDir = Path.Combine(rootDir, DayzConstants.SubdirectoryNames.MpMissions, mpMissionName);
        _baseDbTypesXmlFilePath = Path.Combine(_mpMissionDir,DayzConstants.SubdirectoryNames.Db, DayzConstants.FileNames.Types);
        var cfgEconomyCoreXmlPath = Path.Combine(_mpMissionDir, DayzConstants.FileNames.CfgEconomyCore);
        _cfgEconomyCore = XDocument.Load(cfgEconomyCoreXmlPath);
        _logger = logger;
        _rootDir = rootDir;
    }

    public void Process()
    {
        ProcessSingle(_baseDbTypesXmlFilePath);

        var ceElements = _cfgEconomyCore.Root.OrFail()
            .Nodes().OfType<XElement>().Where(x => x.Name == "ce");
        foreach (var ceElement in ceElements)
        {
            var ceDirName = ceElement.Attribute("folder").OrFail().Value;
            var ceDirPath = Path.Combine(_mpMissionDir, ceDirName);
            foreach (var file in Directory.GetFiles(ceDirPath))
            {
                ProcessSingle(file);
            }
        }
    }

    private void ProcessSingle(string inputFilePath)
    {
        var relativePath = Path.GetRelativePath(_rootDir, inputFilePath);
        var restore = FileManagement.TryRestoreFileV2(_rootDir, relativePath);
        if (File.Exists(restore.BackupFilePath))
        {
            FileManagement.FormatXmlFileInPlace(restore.BackupFilePath);
        }
        var backupResult = FileManagement.BackupFileV2(_rootDir, relativePath);
        if (backupResult.FileOperationCommitted)
        {
            _logger.LogInformation("Backed up file {file}", backupResult.BackupFilePath);
        }
        else
        {
            _logger.LogInformation("File already backed up {file}", backupResult.BackupFilePath);
        }
        XDocument xd = XDocument.Load(inputFilePath);

        var types = xd.Root.OrFail().Nodes().OfType<XElement>().Select(DzTypesXmlTypeElement.FromElement);

        foreach (var type in types)
        {
            GeneralAdjustments(type);
            ProcessBaseDayzItems(type);
            ProcessExpansion(type);
            ProcessAdvancedWeaponScopes(type);
            ProcessSnafu(type);
            ProcessDzn(type);
        }

        using var fs = FileManagement.Utf8WithoutBomWriter(inputFilePath);
        xd.Save(fs);
    }

    private static void GeneralAdjustments(DzTypesXmlTypeElement type)
    {
        var keepLifetime = type.Category == "containers";
        if (!keepLifetime)
        {
            type.Lifetime = Math.Max((int)TimeSpan.FromMinutes(15).TotalSeconds, (int)type.Lifetime / 10);
        }
        type.Restock = 0;
    }

    private static void ProcessBaseDayzItems(DzTypesXmlTypeElement type)
    {
        if (type.Name == "WeaponCleaningKit")
        {
            type.Nominal = 80;
            type.Min = 70;
        }
    }

    private static void ProcessAdvancedWeaponScopes(DzTypesXmlTypeElement type)
    {
        if (!type.Name.StartsWith("AD_"))
        {
            return;
        }

        if (type.Nominal == 0)
        {
            type.Nominal = 2;
            type.Min = 1;
        }
    }

    private static void ProcessExpansion(DzTypesXmlTypeElement type)
    {
        if (!type.Name.StartsWith("EXPANSION_"))
        {
            return;
        }

        var toLowerOccurenceSubstrings = new[]
        {
            "DOOR",
            "WHEEL",
            "HOOD",
            "LEFT",
            "RIGHT",
            "TRUNK",
        };

        if (toLowerOccurenceSubstrings.Any(x => type.Name.Contains(x)))
        {
            type.Nominal = 2;
            type.Min = 0;
        }
    }

    private static void ProcessSnafu(DzTypesXmlTypeElement type)
    {
        if (!type.Name.StartsWith("SNAFU"))
        {
            return;
        }

        type.Nominal = 1;
        type.Min = 1;
        type.Restock = 0;
        type.Flags = type.Flags?.ToDictionary(x => x.Key, y =>
        {
            if (y.Key == "count_in_map")
            {
                return "1";
            }
            else
            {
                return "0";
            }
        });
    }

    private static void ProcessDzn(DzTypesXmlTypeElement type)
    {
        if (!type.Name.StartsWith("dzn"))
        {
            return;
        }

        type.Tags = Array.Empty<string>();
        if (type.Name.Contains("platecarriervest"))
        {
            type.Values = Array.Empty<string>();
            type.Category = "weapons";
        }
        else
        {
            type.Values = new[]
            {
                    "Tier1",
                    "Tier2",
                    "Tier3",
                    "Tier4",
                };
            type.Category = "clothes";
        }
        type.Usages = new[]
        {
                "Military"
            };
        type.Restock = 0;
        type.Nominal = 5;
        type.Min = 3;
    }
}
