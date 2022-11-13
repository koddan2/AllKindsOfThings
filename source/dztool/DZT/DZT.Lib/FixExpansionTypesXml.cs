using System.ComponentModel.Design.Serialization;
using System.Xml.Linq;
using DZT.Lib.Helpers;
using DZT.Lib.Models;
using Microsoft.Extensions.Logging;

namespace DZT.Lib;

public class FixExpansionTypesXml
{
    private readonly string _inputFilePath;
    private readonly ILogger<FixExpansionTypesXml> _logger;
    private readonly string _rootDir;

    public FixExpansionTypesXml(ILogger<FixExpansionTypesXml> logger, string rootDir, string inputFilePath)
    {
        _inputFilePath = Path.Combine(rootDir, inputFilePath);
        _logger = logger;
        _rootDir = rootDir;
    }

    public void ProcessDzn()
    {
        var relativePath = "mpmissions/dayzOffline.chernarusplus/_mixins/br_nam_types_dzn.xml";
        _ = FileManagement.TryRestoreFileV2(_rootDir, relativePath);
        var backupResult = FileManagement.BackupFileV2(_rootDir, relativePath);

        var pathToFile = Path.Combine(_rootDir, relativePath);
        XDocument xd = XDocument.Load(pathToFile);
        var dzTypes = DzTypesXmlTypeElement.FromDocument(xd);
        foreach (var typ in dzTypes)
        {
            typ.Tags = Array.Empty<string>();
            if (typ.Name.Contains("platecarriervest"))
            {
                typ.Values = Array.Empty<string>();
                typ.Category = "weapons";
            }
            else
            {
                typ.Values = new[]
                {
                    "Tier1",
                    "Tier2",
                    "Tier3",
                    "Tier4",
                };
            typ.Category = "clothes";
            }
            typ.Usages = new[]
            {
                "Military"
            };
            typ.Restock = 0;
            typ.Nominal = 5;
            typ.Min = 3;
        }

        using var fs = FileManagement.Utf8WithoutBomWriter(pathToFile);
        xd.Save(fs);
    }

    public void Process()
    {
        var relativePath = Path.GetRelativePath(_rootDir, _inputFilePath);
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
        XDocument xd = XDocument.Load(_inputFilePath);
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
        var toKeepLifetimeSubstrings = new[]
        {
            "BARREL",
            "TENT",
            "CRATE",
            "SEACHEST",
        };

        var typesApi = types.OfType<XElement>().Select(x => new DzTypesXmlTypeElement(x));
        foreach (var type in typesApi)
        {
            var keepLifetime = toKeepLifetimeSubstrings.Any(x => type.Name.ToUpperInvariant().Contains(x));
            if (!keepLifetime)
            {
                type.Lifetime = (int)type.Lifetime / 10;
            }
            ////else
            ////{
            ////    var a = 1;
            ////}
            type.Restock = 0;
            // fix GCK
            // AD_
            if (type.Name == "WeaponCleaningKit")
            {
                type.Nominal = 80;
                type.Min = 70;
            }
            else if (type.Name.StartsWith("AD_"))
            {
                if (type.Nominal == 0)
                {
                    type.Nominal = 4;
                    type.Min = 2;
                }
            }
            else if (type.Name.StartsWith("EXPANSION_"))
            {
                if (toLowerOccurenceSubstrings.Any(x => type.Name.Contains(x)))
                {
                    type.Nominal = 2;
                    type.Min = 0;
                }
            }
        }

        using var fs = FileManagement.Utf8WithoutBomWriter(_inputFilePath);
        xd.Save(fs);
    }
}
