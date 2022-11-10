using System.Text.Json;
using System.Linq;
using System.Xml.Linq;
using System.Net.Mime;
using DZT.Lib.Models;

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
        var usageDict = new Dictionary<string, List<string>>
        {
            { "Civilian", new List<string>() },
            { "Industrial", new List<string>() },
            { "Farm", new List<string>() },
            { "Hunting", new List<string>() },
            { "Police", new List<string>() },
            { "Medical", new List<string>() },
            { "Military", new List<string>() },
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
                    else if (nameAttr is not null && usageDict.ContainsKey(nameAttr))
                    {
                        usageDict[nameAttr].Add(typeName);
                    }
                }
            }
        }
        var cfg = JsonSerializer.Deserialize<SflRoot>(File.ReadAllText(_sflJsonFilePath));
        if (cfg is null) throw new ApplicationException("Fail");
        foreach (var category in cfg.SFLLootCategory)
        {
            if (usageDict.ContainsKey(category.name))
            {
                var list = usageDict[category.name];
                category.loot = list.ToArray();
                category.rarity = 0;
            }
        }

        cfg.Rarity = 50;
        cfg.InitialCooldown = 1;
        cfg.DisableNotifications = 1;

        File.WriteAllText(_sflJsonFilePath, JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true, }));
    }
}

