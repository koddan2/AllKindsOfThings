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
    private readonly SpawnableTypesHelper _spawnableTypesHelper;
    private readonly WeaponSetDefs _weaponSets;
    private readonly ILogger _logger;
    private readonly string _rootDir;
    private readonly string _mpMissionName;

    public GenerateSplattedLoadout(ILogger logger, string rootDir, string mpMissionName)
    {
        _logger = logger;
        _rootDir = rootDir;
        _mpMissionName = mpMissionName;
        _loadoutDir = Path.Combine(rootDir, "config/ExpansionMod/Loadouts");
        _spawnableTypesHelper = new SpawnableTypesHelper(rootDir, mpMissionName);
        _weaponSets = new WeaponSetDefs(rootDir, mpMissionName);
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
        "TTC",
        "SYGUJug",
        "Jug",
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
        ////"CONSTRUC",
        "SPOTLIGHT",
        "PLATE",
        "BARREL",
        "POWERGEN",
        "SEACHEST",
        "CRATE",
        "WOODEN",
        "TENT",
        "LONGTORCH",
        "BROOM",
        "BEARTRAP",
        "PITCHFORK",
        "SHOVEL",
        "FARMINGHOE",
        "NAILEDBASEBALLBAT",
        "BARBEDBASEBALLBAT",
        "BASEBALLBAT",
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
            || (type.Category == "clothes" && !type.NameUpper.Contains("POUCH"))
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
                Chance = x.Flags.OrFail()["crafted"] == "1" ? 0.03 : 0.01 * (((float)x.Nominal) / 7f),
                Sets = new List<LoadoutSet>(),
                Quantity = new Quantity { Min = 0, Max = 0 },
                Health = new List<Health> { new Health { Min = 0.1, Max = 0.9, Zone = "" } },
                ConstructionPartsBuilt = new List<object>(),
                InventoryAttachments = new List<InventoryAttachment>(),
                InventoryCargo = new List<InventoryCargoModel>(),
            });

        splat.InventoryCargo.AddRange(extras);
        splat.InventoryCargo = splat.InventoryCargo.DistinctBy(x => x.ClassName).ToList();

        {
            var ttcs = splat.InventoryCargo.Where(x => x.ClassName.StartsWith("TT"));
            var b = ttcs.Select(x => x.ClassName).ToArray();
            foreach (var item in ttcs)
            {
                item.Chance = 0.01;
            }
        }

        ////SplatItems("Back", splat, extras);
        ////SplatItems("Vest", splat, extras);
        ////SplatItems("Body", splat, extras);
        ////SplatItems("Legs", splat, extras);

        ////static void SplatItems(string slotName, AiLoadoutRoot splat, IEnumerable<InventoryCargoModel> extras)
        ////{
        ////    var invAttachments = splat.InventoryAttachments.FirstOrDefault(x => x.SlotName == slotName)
        ////        ?? throw new ApplicationException("Dang it.");
        ////    foreach (var invAttachment in invAttachments.Items)
        ////    {
        ////        invAttachment.InventoryCargo.AddRange(extras);
        ////        invAttachment.InventoryCargo = invAttachment.InventoryCargo.DistinctBy(x => x.ClassName).ToList();
        ////    }
        ////}
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

        var extraStuff = new Dictionary<string, string[]>
        {
            ["Vest"] = new[] {
                "MMG_JPC_Vest_black", "MMG_tt_Vest_black", "MMG_chestrig_black", "MMG_MK_III_Armor_black", "MMG_MK_V_Armor_black",
                "MMG_JPC_Vest_green", "MMG_tt_Vest_green", "MMG_chestrig_green", "MMG_MK_III_Armor_green", "MMG_MK_V_Armor_green",
                // "JuggernautLVL5_Suit",
                // "JuggernautLVL5_Tan",
                "JuggernautLVL5_Black",
                // "JuggernautLVL5_Winter",
                // "JuggernautLVL1_Suit",
                // "JuggernautLVL1_Suit_Tan",
                "JuggernautLVL1_Suit_Black",
                // "JuggernautLVL1_Suit_Winter",
            },
            ["Back"] = new[] {
                "MMG_carrier_backpack_black", "MMG_supplybag_black", "MMG_assault_pack_black", "MMG_camelback_black", "MMG_mmps_bag_black",
                "MMG_carrier_backpack_green", "MMG_supplybag_green", "MMG_assault_pack_green", "MMG_camelback_green", "MMG_mmps_bag_green",
                },
            ["Hips"] = new[] {
                "MMG_falcon_b1_belt_black",
                "MMG_falcon_b1_belt_green",
                },
            ["Headgear"] = new[] {
                "MMG_tactical_helmet_black", "MMG_striker_helmet_black", "mmg_armored_helmet_black",
                "MMG_tactical_helmet_green", "MMG_striker_helmet_green", "mmg_armored_helmet_green",
                },
            ["Body"] = new[] {
                "MMG_operatorshirt_black", "MMG_tactical_shirt_black", "MMG_combatshirt_black",
                "MMG_operatorshirt_green", "MMG_tactical_shirt_green", "MMG_combatshirt_green",
                },
            ["Legs"] = new[] {
                "MMG_combatpants_black", "mmg_tactical_pants_black",
                "MMG_combatpants_green", "mmg_tactical_pants_green",
                },
        }.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(item => new Item
            {
                Chance = 0.12,
                ClassName = item,
                Quantity = new Quantity { Min = 0, Max = 0 },
                Sets = new List<LoadoutSet>(),
                ConstructionPartsBuilt = new List<object>(),
                Health = new List<Health> { new Health { Min = 0.5, Max = 0.9 } },
                InventoryAttachments = _spawnableTypesHelper.GetAdditionalAttachments(item).ToList(),
                InventoryCargo = new List<InventoryCargoModel>(),
            }));

        foreach (var att in splat.InventoryAttachments)
        {
            att.Items = att.Items.DistinctBy(x => x.ClassName).ToList();

            if (extraStuff.ContainsKey(att.SlotName)) att.Items.AddRange(extraStuff[att.SlotName]);
            // if (att.SlotName == "Vest") att.Items.AddRange(extraStuff["Vest"]);
            // else if (att.SlotName == "Headgear") att.Items.AddRange(extraStuff["Headgear"]);
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
            .Select(set =>
            {
                // SIDE-EFFECT SELECT because I'm lazy.
                if (set.ClassName == "WEAPON")
                {
                    // set.Chance = 0.23;
                    // set.Chance = 0.11;
                    set.Chance = 0.08;
                }
                return set;
            })
            .Where(set =>
            {
                // Why is this removed?
                // It's because only NBC dudes has a Set which is CLOTHING,
                // so if this isn't removed, there'll be only NBC dudes.
                return set.ClassName != "CLOTHING";
            })
            .DistinctBy(x => x.InventoryAttachments[0]?.Items[0]?.ClassName)
            .ToList();

        // splat.Sets.Add(_weaponSets.Awm);
        // splat.Sets.Add(_weaponSets.Kiivari);
        splat.Sets.AddRange(_weaponSets.All);
    }

    private static void AssignSets(AiLoadoutRoot splat, AiLoadoutRoot item)
    {
        foreach (var set in item.Sets)
        {
            splat.Sets.Add(set);
        }
    }
}

public class SpawnableTypesHelper
{
    private readonly string _rootDir;
    private readonly string _mpMissionName;

    private Dictionary<string, XElement>? _spawnableTypes;

    public SpawnableTypesHelper(string rootDir, string mpMissionName)
    {
        _rootDir = rootDir;
        _mpMissionName = mpMissionName;
    }

    private Dictionary<string, XElement> SpawnableTypes => InitSpawnableTypes() ? _spawnableTypes.OrFail() : throw new ApplicationException("");
    private bool InitSpawnableTypes()
    {
        if (_spawnableTypes is not null) return true;

        _spawnableTypes = new Dictionary<string, XElement>();
        var sptfn = DayzFilesHelper.GetAllSpawnableTypesXmlFileNames(_rootDir.OrFail(), _mpMissionName.OrFail());
        foreach (var item in sptfn)
        {
            var xd = XDocument.Load(item);
            foreach (var xe in xd.Root.OrFail().Nodes().OfType<XElement>().Where(x => x.Name == "type"))
            {
                _spawnableTypes[xe.Attribute("name").OrFail().Value] = xe;
            }
        }

        return true;
    }

    private XElement? GetSpawnableType(string name)
    {
        if (SpawnableTypes.TryGetValue(name, out var result))
        {
            return result;
        }

        return null;
    }

    public IEnumerable<InventoryAttachment> GetAdditionalAttachments(string item)
    {
        if (item.StartsWith("Juggernaut"))
        {
            yield return new InventoryAttachment
            {
                SlotName = "",
                Items = new[] { "Juggernaut_Buttpack_Black", "Juggernaut_Pouches_Black", }.Select(className =>
                    new Item
                    {
                        Chance = 1,
                        ClassName = className,
                        ConstructionPartsBuilt = new List<object>(),
                        Health = new List<Health>
                        {
                                new Health{ Min=0.5, Max=0.9 },
                        },
                        InventoryAttachments = new List<InventoryAttachment>(),
                        InventoryCargo = new List<InventoryCargoModel>(),
                    }).ToList()
            };
        }
        else if (GetSpawnableType(item) is XElement xe)
        {
            // if (item == "MMG_JPC_Vest_black")
            if (item == "SNAFU_AWM_Gun")
            {
                var a = 1;
            }
            foreach (var attch in xe.Nodes().OfType<XElement>().Where(x => x.Name == "attachments"))
            {
                yield return new InventoryAttachment
                {
                    SlotName = "",
                    Items = (
                        from itm
                        in attch.Nodes().OfType<XElement>().Where(x => x.Name == "item")
                        select new Item
                        {
                            Chance = itm.Attribute("name")?.Value?.ToUpper()?.Contains("MAG") is true
                                ? 1
                                : (attch.Attribute("chance")?.Value.AsDouble() ?? 1d) * (itm.Attribute("chance")?.Value.AsDouble() ?? 1d),
                            ClassName = itm.Attribute("name").OrFail().Value,
                            ConstructionPartsBuilt = new List<object>(),
                            Health = new List<Health>
                            {
                                new Health{ Min=0.5, Max=0.9 },
                            },
                            InventoryAttachments = new List<InventoryAttachment>(),
                            InventoryCargo = new List<InventoryCargoModel>(),
                        }).ToList()
                };
            }
        }
    }
}

public class WeaponSetDefs
{
    private readonly SpawnableTypesHelper _spawnableTypesHelper;

    public WeaponSetDefs(string rootDir, string mpMissionName)
    {
        _spawnableTypesHelper = new SpawnableTypesHelper(rootDir, mpMissionName);
    }

    public IEnumerable<LoadoutSet> All => new []
    {
        Awm,
        Kiivari,
        // Aek545,
        S_R700,
        M_R700,
        ScarH,
        AK12,
        AK19,
        MMAKM,
    };

    public LoadoutSet MMAKM => GetLoadoutSetWeapon("TTC_AKM", 0.05, new [] {"Ammo_762x39", "Ammo_762x39", "Ammo_762x39", });
    public LoadoutSet AK19 => GetLoadoutSetWeapon("SNAFU_AK19", 0.05, new [] {"Ammo_556x45", "Ammo_556x45", "Ammo_556x45", });
    public LoadoutSet AK12 => GetLoadoutSetWeapon("SNAFU_AK12A", 0.05, new [] {"Ammo_545x39", "Ammo_545x39", "Ammo_545x39"});
    public LoadoutSet ScarH => GetLoadoutSetWeapon("Snafu_ScarH_Black_GUN", 0.05, new [] {"Ammo_308Win", "Ammo_308Win"});
    public LoadoutSet M_R700 => GetLoadoutSetWeapon("TTC_R700", 0.12, new [] {"Ammo_308Win", "Ammo_308Win"});
    public LoadoutSet S_R700 => GetLoadoutSetWeapon("GCGN_M700", 0.12, new [] {"Ammo_308Win", "Ammo_308Win"});
    public LoadoutSet Aek545 => GetLoadoutSetWeapon("SNAFU_AEK545_Gun", 0.12);
    public LoadoutSet Awm => GetLoadoutSetWeapon("SNAFU_AWM_Gun", 0.08, new [] {"SNAFU_Ammo_338", "SNAFU_Ammo_338"});
    public LoadoutSet Kiivari => GetLoadoutSetWeapon("SNAFUKivaari_Black_GUN", 0.03, new [] {"SNAFU_Ammo_338", "SNAFU_Ammo_338"});

    private LoadoutSet GetLoadoutSetWeapon(string className, double chance = 1, string[]? extraInventory = null)
    {
        extraInventory ??= Array.Empty<string>();
        return new LoadoutSet
        {
            ClassName = "WEAPON",
            Chance = chance,
            Health = new List<Health>
            {
                new Health {Min=0.4, Max=0.9},
            },
            InventoryAttachments = new List<InventoryAttachment>
            {
                new InventoryAttachment
                {
                    SlotName = "Shoulder",
                    Items = new List<Item>
                    {
                        new Item
                        {
                            Chance = 1,
                            ClassName = className,
                            Health = new List<Health> { new Health { Min = 0.5, Max = 0.9 } },
                            InventoryAttachments = _spawnableTypesHelper.GetAdditionalAttachments(className).ToList(),
                        }
                    }
                }
            },
            InventoryCargo = _spawnableTypesHelper.GetAdditionalAttachments(className)
                .SelectMany(x =>
                    x.Items
                    .Where(y => y.ClassName.ToUpper().Contains("MAG"))
                    .Select(y =>
                        new InventoryCargoModel
                        {
                            Chance = 1,
                            ClassName = y.ClassName,
                        }))
                        .Concat(extraInventory.Select(extra =>
                        new InventoryCargoModel
                        {
                            Chance = 1,
                            ClassName = extra,
                        }))
                        .ToList(),
            // InventoryCargo = new List<InventoryCargoModel>
            // {
            //     new InventoryCargoModel
            //     {
            //         Chance = 1,
            //         ClassName = "SNAFU_AWM_Mag",
            //     },
            //     new InventoryCargoModel
            //     {
            //         Chance = 1,
            //         ClassName = "SNAFU_Ammo_338",
            //         Quantity = new Quantity {Min=10, Max=20},
            //     },
            // }
            // InventoryCargo = new List<InventoryCargoModel>
            // {
            // }
        };
    }
}