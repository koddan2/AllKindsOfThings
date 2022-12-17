using System.Text.Json;
using System.Linq;
using System.Xml.Linq;
using System.Net.Mime;
using DZT.Lib.Models;
using DZT.Lib.Helpers;

namespace DZT.Lib;

public class FixSearchForLoot
{
    private readonly string _sflJsonFilePath;
    private readonly string _typesXmlFilePath;
    private readonly string _sflJsonRelativePath;
    private readonly string _rootDir;
    private readonly string _mpMissionName = "dayzOffline.chernarusplus";
    private readonly string _profileDirectoryName = "config";

    public FixSearchForLoot(string rootDir, string mpMissionName, string profileDirectoryName)
    {
        _rootDir = rootDir;
        _mpMissionName = mpMissionName;
        _profileDirectoryName = profileDirectoryName;
        var typesXmlRelativePath = Path.Combine("mpmissions", _mpMissionName, "db", "types.xml");
        _typesXmlFilePath = Path.Combine(rootDir, typesXmlRelativePath);
        _sflJsonRelativePath = Path.Combine(_profileDirectoryName, "SearchForLoot", "SearchForLoot.json");
        _sflJsonFilePath = Path.Combine(rootDir, _sflJsonRelativePath);
    }

    public void Process()
    {
        var restore = FileManagement.TryRestoreFileV2(_rootDir, _sflJsonRelativePath);
        var backup = FileManagement.BackupFileV2(_rootDir, _sflJsonRelativePath);
        // TODO: follow cfgeconomycore.xml refs -> types files
        var xd = XDocument.Load(_typesXmlFilePath);
        var types = DzTypesXmlTypeElement.FromDocument(xd);
        var usageDict = new Dictionary<string, HashSet<string>>
        {
            { "Civilian", new HashSet<string>() },
            { "Industrial", new HashSet<string>() },
            { "Farm", new HashSet<string>() },
            { "Hunting", new HashSet<string>() },
            { "Police", new HashSet<string>() },
            { "Medical", new HashSet<string>() },
            { "Military", new HashSet<string>() },
        };
        var forbiddenSubstrings = new[]
        {
            "CHEML",
            "BEAR",
            "PEN",
            "PILEOF",
        };
        foreach (var type in types)
        {
            if (type.Category == "clothes")
            {
                continue;
            }

            if (type.Category == "containers")
            {
                continue;
            }

            if (forbiddenSubstrings.Any(x => type.NameUpper.Contains(x)))
            {
                continue;
            }

            var typeName = type.Name;

            foreach (var usage in type.Usages)
            {
                if (usage == "Town" || usage == "Village")
                {
                    usageDict["Civilian"].Add(typeName);
                }
                else if (usage.Contains("Medic"))
                {
                    usageDict["Medical"].Add(typeName);
                }
                else if (usageDict.TryGetValue(usage, out var value) && value is not null)
                {
                    value.Add(typeName);
                }
            }
        }
        var cfg = JsonSerializer.Deserialize<SflRoot>(File.ReadAllText(_sflJsonFilePath));
        if (cfg is null)
        {
            throw new ApplicationException("Fail");
        }

        var structureClassNames = DataHelper.GetStructureClassNames();
        cfg.SFLBuildings.First(b => b.name == "Civilian").buildings = structureClassNames["**Residential**"].Where(x => x != "GardenPlot").ToArray();
        cfg.SFLBuildings.First(b => b.name == "Industrial").buildings = structureClassNames["**Industrial**"].ToArray();
        cfg.SFLBuildings.First(b => b.name == "Military").buildings = (structureClassNames["**Specific**"].Concat(structureClassNames["**Military**"])).ToArray();

        var forbiddenClassNamesSubstrings = new[]
        {
            "DOOR",
            "WHEEL",
            "HOOD",
            "LEFT",
            "RIGHT",
            "TRUNK",
        };
        bool IsForbidden(string name)
        {
            foreach (var forbid in forbiddenClassNamesSubstrings)
            {
                if (name.ToUpperInvariant().Contains(forbid))
                {
                    return true;
                }
            }
            return false;
        }

        foreach (var category in cfg.SFLLootCategory)
        {
            if (usageDict.TryGetValue(category.name, out var value) && value is not null)
            {
                var list = value;
                category.loot = list
                    .Where(className => !IsForbidden(className))
                    .ToArray();
                category.rarity = 0;
            }
        }

        cfg.Rarity = 25;
        cfg.InitialCooldown = 1;
        cfg.DisableNotifications = 1;
        cfg.XPGain = 10;

        File.WriteAllText(_sflJsonFilePath, JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true, }));
    }
}

