using DZT.Lib.Helpers;
using DZT.Lib.Models;
using Microsoft.Extensions.Logging;
using SAK;
using System.Text.Json;
using System.Xml.Linq;

namespace DZT.Lib;
public class GenerateSplattedLoadout
{
    private readonly string _loadoutDir;
    private readonly ILogger _logger;
    private readonly string _rootDir;
    private readonly string _mpMissionName;

    public GenerateSplattedLoadout(ILogger logger, string rootDir, string mpMissionName)
    {
        _logger = logger;
        _rootDir = rootDir;
        _mpMissionName = mpMissionName;
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
            ////ConstructionPartsBuilt = new List<object>(),
            Health = ////new List<Health>
            {
                new Health {Min=0.3, Max=0.9},
            },
            ////InventoryAttachments = invAttachmentsSlotNames.Select(x => new InventoryAttachment { SlotName = x, Items=new List<Item>() }).ToList(),
            ////InventoryAttachments = new List<InventoryAttachment>(),
            ////InventoryCargo = new List<InventoryCargoModel>(),
            ////Sets = new List<Set>(),
        };

        AssignInventoryAttachments(splat, model);
        AssignInventoryCargo(splat, model);
        AssignSets(splat, model);

        var outputPath = Path.Combine(_loadoutDir, OutputFileName);
        File.WriteAllText(outputPath, JsonSerializer.Serialize(splat, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static readonly string[] _ForbiddenClassNamesStartsWith = new[]
    {
        "TTC_",
        "SYGUJug",
    };

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
        "POWERGEN",
        "SEACHEST",
        "CRATE",
        "WOODEN",
        "TENT",
    };

    private static readonly string[] _ExemptedFromExclusion = new[]
    {
        "BOTTLE",
        "SUPPRESS",
        "ARMBAND",
    };

    static bool IsForbidden(DzTypesXmlTypeElement type)
    {
        if (_ExemptedFromExclusion.Any(type.NameUpper.Contains))
        {
            return false;
        }

        if (_ForbiddenClassNamesStartsWith.Any(x => type.NameUpper.StartsWith(x.ToUpperInvariant())))
        {
            return true;
        }

        var forbid0 = type.Category == "containers"
            || type.Category == "clothes"
            || type.Tags?.Contains("floor") is true
            || type.Flags?["crafted"] == "1"
            ;
        if (forbid0)
        {
            return true;
        }

        foreach (var forbid in _ForbiddenClassNamesSubstrings)
        {
            if (type.NameUpper.Contains(forbid))
            {
                return true;
            }
        }

        var hasCategory = !type.Category.IsNullOrEmpty();
        var hasUsage = type.Usages?.Length > 0;
        var hasNominal = type.Nominal > 0;

        if (hasCategory || (hasUsage || hasNominal))
        {
            return false;
        }

        return true;
    }

    private IEnumerable<DzTypesXmlTypeElement> GetCargoCandidates()
    {
        var seen = new Dictionary<string, bool>();
        foreach (var typesXmlFile in DayzFilesHelper.GetAllTypesXmlFileNames(_rootDir, _mpMissionName))
        {
            var xd = XDocument.Load(typesXmlFile);
            var types = DzTypesXmlTypeElement.FromDocument(xd);
            foreach (var type in types)
            {
                if (IsForbidden(type))
                {
                    continue;
                }
                else if (seen.ContainsKey(type.Name))
                {
                    continue;
                }
                seen[type.Name] = true;

                yield return type;
            }
        }
    }

    private void AssignInventoryCargo(AiLoadoutRoot splat, List<AiLoadoutRoot?> model)
    {
        var cargoCandidates = GetCargoCandidates();

        ////foreach (var item in model)
        ////{
        ////    if (item?.InventoryCargo is not null)
        ////    {
        ////        splat.InventoryCargo.AddRange(item.InventoryCargo);
        ////    }
        ////}

        var extras = cargoCandidates
            .Select(x => new InventoryCargoModel
            {
                ClassName = x.Name,
                Chance = 0.03,
                Sets = new List<Set>(),
                Quantity = new Quantity { Min = 0, Max = 0 },
                Health = new List<Health> { new Health { Min = 0.1, Max = 0.9, Zone = "" } },
                ConstructionPartsBuilt = new List<object>(),
                InventoryAttachments = new List<InventoryAttachment>(),
                InventoryCargo = new List<InventoryCargoModel>(),
            });

        splat.InventoryCargo.AddRange(extras);
        splat.InventoryCargo = splat.InventoryCargo.DistinctBy(x => x.ClassName).ToList();

        ////SplatItems("Back", splat, extras);
        ////SplatItems("Vest", splat, extras);
        ////SplatItems("Body", splat, extras);
        ////SplatItems("Legs", splat, extras);

        static void SplatItems(string slotName, AiLoadoutRoot splat, IEnumerable<InventoryCargoModel> extras)
        {
            var invAttachments = splat.InventoryAttachments.FirstOrDefault(x => x.SlotName == slotName)
                ?? throw new ApplicationException("Dang it.");
            foreach (var invAttachment in invAttachments.Items)
            {
                invAttachment.InventoryCargo.AddRange(extras);
                invAttachment.InventoryCargo = invAttachment.InventoryCargo.DistinctBy(x => x.ClassName).ToList();
            }
        }
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

