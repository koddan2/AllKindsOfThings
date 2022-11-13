using DZT.Lib.Helpers;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Xml.Linq;

namespace DZT.Lib;

public class GenerateSplattedLoadout
{
    private readonly string _loadoutDir;
    private readonly ILogger<GenerateSplattedLoadout> _logger;
    private readonly string _rootDir;

    public GenerateSplattedLoadout(ILogger<GenerateSplattedLoadout> logger, string rootDir)
    {
        _logger = logger;
        _rootDir = rootDir;
        _loadoutDir = Path.Combine(rootDir, "config/ExpansionMod/Loadouts");
    }

    public string OutputFileName { get; set; } = "SplattedLoadout.json";

    public void Process()
    {
        var loadoutFiles = Directory.EnumerateFiles(_loadoutDir).Where(file => Path.GetFileName(file) != OutputFileName);
        _logger.LogInformation("Found {} loadout files ({@list})", loadoutFiles.Count(), loadoutFiles.Select(x => Path.GetFileName(x)));
        var model = loadoutFiles.Select(file => JsonSerializer.Deserialize<AiLoadoutRoot>(File.ReadAllText(file))).ToList();

        ////var invAttachmentsSlotNames = new[]
        ////{
        ////    "Body",
        ////    "Legs",
        ////    "Feet",
        ////    "Back",
        ////    "Vest",
        ////    "Headgear",
        ////    "Gloves",
        ////    "Hips",
        ////    "MASK",
        ////    "Melee",
        ////};
        var splat = new AiLoadoutRoot
        {
            Chance = 1,
            Quantity = new Quantity { Min = 0, Max = 0 },
            ClassName = "",
            ConstructionPartsBuilt = new List<object>(),
            Health = new List<Health>(),
            ////InventoryAttachments = invAttachmentsSlotNames.Select(x => new InventoryAttachment { SlotName = x, Items=new List<Item>() }).ToList(),
            InventoryAttachments = new List<InventoryAttachment>(),
            InventoryCargo = new List<InventoryCargoModel>(),
            Sets = new List<Set>(),
        };

        AssignInventoryAttachments(splat, model);
        AssignInventoryCargo(splat, model);
        AssignSets(splat, model);

        var outputPath = Path.Combine(_loadoutDir, OutputFileName);
        File.WriteAllText(outputPath, JsonSerializer.Serialize(splat, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static readonly string[] _ForbiddenClassNamesSubstrings = new[]
    {
        "DOOR",
        "WHEEL",
        "HOOD",
        "LEFT",
        "RIGHT",
        "TRUNK",
        "ANIMAL",
        "EXPANSION",
        "ZMB",
        "CONSTRUC",
        "PLATE",
        "BARREL",
        "POWERG",
        "SEACHEST",
        "CRATE",
        "WOODEN",
        "TENT",
    };

    static bool IsForbidden(string name, XElement typ)
    {
        // exempt armbands
        if (name.Contains("Armband_"))
        {
            return false;
        }

        foreach (var forbid in _ForbiddenClassNamesSubstrings)
        {
            if (name.ToUpperInvariant().Contains(forbid))
            {
                return true;
            }
        }

        var hasCategory = false;
        var hasUsage = false;
        var hasNominal = false;
        foreach (var typEl in typ.Nodes().OfType<XElement>())
        {
            if (typEl.Name == "nominal" && typEl.Value != "0")
            {
                hasNominal = true;
            }

            if (typEl.Name == "category")
            {
                var catName = typEl.Attribute("name")?.Value;
                hasCategory = catName != "vehiclesparts" && catName != "weapons";
            }
            else if (typEl.Name == "usage")
            {
                hasUsage = true;
            }
        }

        if (hasCategory && (hasUsage || hasNominal))
        {
            return false;
        }

        return true;
    }

    private record TypeElement(string Name);
    private HashSet<TypeElement> GetCargoCandidates()
    {
        var xd = DataHelper.GetTypesXml(_rootDir, "dayzOffline.chernarusplus");
        var accum = new HashSet<TypeElement>();
        var forbiddens = new HashSet<string>();
        foreach (var typ in xd.Root!.Nodes().OfType<XElement>())
        {
            var nameAttr = typ.Attribute("name")!.Value;
            if (IsForbidden(nameAttr, typ))
            {
                forbiddens.Add(nameAttr);
                continue;
            }

            accum.Add(new TypeElement(nameAttr));
        }

        return accum;
    }

    private void AssignInventoryCargo(AiLoadoutRoot splat, List<AiLoadoutRoot?> model)
    {
        var cargoCandidates = GetCargoCandidates();

        foreach (var item in model)
        {
            if (item?.InventoryCargo is not null)
            {
                splat.InventoryCargo.AddRange(item.InventoryCargo);
            }
        }

        var extras = cargoCandidates
            .Select(x => new InventoryCargoModel
            {
                ClassName = x.Name,
                Chance = 0.023,
                Sets = new List<Set>(),
                Quantity = new Quantity { Min = 0, Max = 0 },
                Health = new List<Health> { new Health { Min = 0.1, Max = 0.9, Zone = "" } },
                ConstructionPartsBuilt = new List<object>(),
                InventoryAttachments = new List<InventoryAttachment>(),
                InventoryCargo = new List<InventoryCargoModel>(),
            });

        splat.InventoryCargo.AddRange(extras);

        splat.InventoryCargo = splat.InventoryCargo.DistinctBy(x => x.ClassName).ToList();
    }

    private void AssignInventoryAttachments(AiLoadoutRoot splat, List<AiLoadoutRoot?> model)
    {
        var index = 0;
        foreach (var item in model)
        {
            if (item is AiLoadoutRoot notNull)
            {
                AssignInventoryAttachments(splat, notNull);
            }
            else
            {
                _logger.LogWarning("Encountered <null> AiLoadoutRoot (index={index})", index);
            }
            ++index;
        }

        foreach (var att in splat.InventoryAttachments)
        {
            att.Items = att.Items.DistinctBy(x => x.ClassName).ToList();
        }

        splat.InventoryAttachments = splat.InventoryAttachments.Where(x => x.SlotName != "Shoulder").ToList();
    }

    private static void AssignInventoryAttachments(AiLoadoutRoot splat, AiLoadoutRoot item)
    {
        foreach (var attachment in item.InventoryAttachments)
        {
            var toAssign = splat.InventoryAttachments.FirstOrDefault(x => x.SlotName == attachment.SlotName);
            if (toAssign is null)
            {
                splat.InventoryAttachments.Add(attachment);
            }
            else if (attachment.Items is not null)
            {
                toAssign.Items.AddRange(attachment.Items);
            }
        }
    }

    private void AssignSets(AiLoadoutRoot splat, List<AiLoadoutRoot?> model)
    {
        var index = 0;
        foreach (var item in model)
        {
            if (item is AiLoadoutRoot notNull)
            {
                AssignSets(splat, notNull);
            }
            else
            {
                _logger.LogWarning("Encountered <null> AiLoadoutRoot (index={index})", index);
            }
            ++index;
        }

        splat.Sets = splat.Sets
            //// .Where(s => s.ClassName != "CLOTHING")
            .Select(set =>
            {
                if (set.ClassName == "WEAPON")
                {
                    set.Chance = 0.23;
                }
                return set;
            })
            .Where(set =>
            {
                return set.ClassName != "CLOTHING";
            })
            .DistinctBy(x => x.InventoryAttachments[0]?.Items[0]?.ClassName)
            .ToList();
    }

    private static void AssignSets(AiLoadoutRoot splat, AiLoadoutRoot item)
    {
        foreach (var set in item.Sets)
        {
            splat.Sets.Add(set);
        }
    }
}

