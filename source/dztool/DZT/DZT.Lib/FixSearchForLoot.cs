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

    public FixSearchForLoot(string rootDir)
    {
        var typesXmlRelativePath = "mpmissions\\dayzOffline.chernarusplus\\db\\types.xml";
        _typesXmlFilePath = Path.Combine(rootDir, typesXmlRelativePath);
        var sflJsonRelativePath = "config\\SearchForLoot\\SearchForLoot.json";
        _sflJsonFilePath = Path.Combine(rootDir, sflJsonRelativePath);
    }

    public void Process()
    {
        FileManagement.BackupFile(_sflJsonFilePath, overwrite: true);
        var types = XDocument.Load(_typesXmlFilePath);
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
        foreach (var type in types.Root!.Nodes().OfType<XElement>())
        {
            var typeName = type.Attribute("name")?.Value;
            if (typeName is null) continue;

            foreach (var typeNode in type.Nodes().OfType<XElement>())
            {
                if (typeNode.Name == "usage")
                {
                    var nameAttr = typeNode.Attribute("name")?.Value;
                    if (nameAttr == "Town" || nameAttr == "Village")
                    {
                        usageDict["Civilian"].Add(typeName);
                    }
                    else if (nameAttr == "Medic")
                    {
                        usageDict["Medical"].Add(typeName);
                    }
                    else if (nameAttr is not null && usageDict.ContainsKey(nameAttr))
                    {
                        usageDict[nameAttr].Add(typeName);
                    }
                }
            }
        }
        var cfg = JsonSerializer.Deserialize<SflRoot>(File.ReadAllText(_sflJsonFilePath));
        if (cfg is null) throw new ApplicationException("Fail");

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
                if (name.ToUpperInvariant().Contains(forbid)) return true;
            }
            return false;
        }

        foreach (var category in cfg.SFLLootCategory)
        {
            if (usageDict.ContainsKey(category.name))
            {
                var list = usageDict[category.name];
                category.loot = list
                    .Where(className => !IsForbidden(className))
                    .ToArray();
                category.rarity = 0;
            }
        }

        cfg.Rarity = 99;
        cfg.InitialCooldown = 1;
        cfg.DisableNotifications = 1;
        cfg.XPGain = 10;

        File.WriteAllText(_sflJsonFilePath, JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true, }));
    }
}

