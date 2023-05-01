using DZT.Lib.Helpers;
using DZT.Lib.Models;
using Microsoft.Extensions.Logging;
using SAK;
using System.Text.Json;
using System.Xml.Linq;

namespace DZT.Lib;

public class GenerateSplattedLoadout
{
    internal static Quantity DefaultQuantity = new Quantity { Min = 0.01, Max = 0.8 };
    private readonly string _loadoutDir;
    private readonly SpawnableTypesHelper _spawnableTypesHelper;
    private readonly WeaponSetDefs _weaponSets;
    private readonly ILogger _logger;
    private readonly string _rootDir;
    private readonly string _mpMissionName;
    private readonly string _profileDirectoryName = "config";

    private readonly CategorizedDouble _weaponChanceCategories =
        new(minimal: 0.01, small: 0.03, medium: 0.07, large: 0.15);

    public GenerateSplattedLoadout(
        ILogger logger,
        string rootDir,
        string mpMissionName,
        string profileDirectoryName
    )
    {
        _logger = logger;
        _rootDir = rootDir;
        _mpMissionName = mpMissionName;
        _profileDirectoryName = profileDirectoryName;
        _loadoutDir = Path.Combine(_rootDir, _profileDirectoryName, "ExpansionMod/Loadouts");
        _spawnableTypesHelper = new SpawnableTypesHelper(_rootDir, _mpMissionName);
        _weaponSets = new WeaponSetDefs(_rootDir, _mpMissionName, _weaponChanceCategories);
    }

    public string OutputFileName { get; set; } = "SplattedLoadout.json";

    public void Process()
    {
        var loadoutFiles = Directory
            .EnumerateFiles(_loadoutDir)
            .Where(file => Path.GetFileName(file) != OutputFileName);
        _logger.LogInformation(
            "Found {} loadout files ({@list})",
            loadoutFiles.Count(),
            loadoutFiles.Select(x => Path.GetFileName(x))
        );
        var model = loadoutFiles
            .Select(file => JsonSerializer.Deserialize<AiLoadoutRoot>(File.ReadAllText(file)))
            .ToList();

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
            Health =
            {
                new Health { Min = 0.3, Max = 0.9 },
            },
        };

        AssignInventoryAttachments(splat, model);
        AssignInventoryCargo(splat, model);
        AssignSets(splat, model);

        splat.Sets.Add(ExtraSets.Skorpion);
        splat.Sets.Add(ExtraSets.UMP);
        splat.Sets.Add(ExtraSets.Mp5k);
        splat.Sets.Add(ExtraSets.Repeater);
        splat.Sets.Add(ExtraSets.Bk12);
        splat.Sets.Add(ExtraSets.CzPistol);
        splat.Sets.Add(ExtraSets.MP7);
        splat.Sets.Add(ExtraSets.MPX);
        splat.Sets.Add(ExtraSets.TTCUZI);
        splat.Sets.Add(ExtraSets.SVT40);
        splat.Sets.Add(ExtraSets.AVS36);
        splat.Sets.Add(ExtraSets.Marlin);
        splat.Sets.Add(ExtraSets.PKP);
        splat.Sets.Add(ExtraSets.SR25);

        var ij = splat.Sets.First(IsIj);
        splat.Sets.Remove(ij);
        splat.Sets.Add(ij);

        var outputPath = Path.Combine(_loadoutDir, OutputFileName);
        File.WriteAllText(
            outputPath,
            JsonSerializer.Serialize(splat, DefaultSettings.JsonSerializerOptions)
        );
    }

    private static readonly string[] _ForbiddenClassNamesStartsWith = new[]
    {
        // "TTC",
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
        "PICKAXE",
        "CANISTERGASOLINE",
        "GARDENLIME",
    };

    private static readonly string[] _ExemptedFromExclusion = new[]
    {
        "BOTTLE",
        "SUPPRESS",
        "_SIL",
        "ARMBAND",
    };

    static bool IsForbidden(DzTypesXmlTypeElement type)
    {
        if (_ExemptedFromExclusion.Any(type.NameUpper.Contains))
        {
            return false;
        }

        if (
            _ForbiddenClassNamesStartsWith.Any(x => type.NameUpper.StartsWith(x.ToUpperInvariant()))
        )
        {
            return true;
        }

        var forbid0 =
            type.Category == "containers"
            || type.Category == "bags"
            || type.Usages.Any(usage => usage == "ContaminatedArea")
            || (type.Category == "clothes" && !type.NameUpper.Contains("POUCH"))
            || type.Tags?.Contains("floor") is true
            || type.Tags?.Contains("skip-ai") is true
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
        var hasTags = type.Tags?.Length > 0;
        var hasNominal = type.Nominal > 0;

        if (hasCategory || hasUsage || hasTags || hasNominal)
        {
            return false;
        }

        return true;
    }

    private IEnumerable<DzTypesXmlTypeElement> GetCargoCandidates()
    {
        var seen = new Dictionary<string, bool>();
        foreach (
            var typesXmlFile in DayzFilesHelper.GetAllTypesXmlFileNames(_rootDir, _mpMissionName)
        )
        {
            _logger.LogInformation("Processing {file}", typesXmlFile);
            var xd = XDocument.Load(typesXmlFile);
            var types = DzTypesXmlTypeElement.FromDocument(xd);
            foreach (var type in types)
            {
                if (IsForbidden(type))
                {
                    continue;
                }
                else if (type.Nominal == 0)
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

    private bool IsSuppressor(string className)
    {
        return className.ToUpper().Contains("SIL") || className.ToUpper().Contains("SUPP");
    }

    private bool IsSuppressor(DzTypesXmlTypeElement typeElement)
    {
        return typeElement.NameUpper.Contains("SIL") || typeElement.NameUpper.Contains("SUPP");
    }

    private void AssignInventoryCargo(AiLoadoutRoot splat, List<AiLoadoutRoot?> _)
    {
        var cargoCandidates = GetCargoCandidates();

        ////foreach (var item in model)
        ////{
        ////    if (item?.InventoryCargo is not null)
        ////    {
        ////        splat.InventoryCargo.AddRange(item.InventoryCargo);
        ////    }
        ////}

        double GetChance(DzTypesXmlTypeElement x)
        {
            // var thoseWithMoreChance = new[]
            // {
            //     "50CAL",
            //     "AMMO",
            //     "SNAFU_AMMO_338",
            //     "GCGN_AMMO_408CHEY",
            //     "TTC_AMMOBOX_50BMG_10RND",
            //     "TTC_AMMO_50BMG",
            //     "TTC_AMMO_338",
            //     "TTC_AMMOBOX_338MM_10RND"
            // };

            double result = 0;
            if (x.Flags.OrFail()["crafted"] == "1")
            {
                result = 0.05;
            }
            // else if (thoseWithMoreChance.Any(s => x.NameUpper.Contains(s)))
            // {
            //     result = 0.1;
            // }
            else if (IsSuppressor(x))
            {
                result = 0.003;
            }
            else if (x.NameUpper.Contains("TTC") || x.NameUpper.Contains("SNAFU"))
            {
                if (x.Nominal == 0)
                {
                    result = 0;
                }
                else
                {
                    result = 0.01 * (((float)x.Nominal) / 7f);
                    result = Math.Clamp(result, 0.001, 0.05);
                }
            }
            else
            {
                result = 0.01 * (((float)x.Nominal) / 4f);
            }

            return Math.Clamp(result, 0.001, 0.08);
        }

        var extras = cargoCandidates.Select(
            x =>
                new InventoryCargoModel
                {
                    ClassName = x.Name,
                    Chance = GetChance(x),
                    Sets = new List<LoadoutSet>(),
                    Quantity = DefaultQuantity,
                    Health = new List<Health>
                    {
                        new Health
                        {
                            Min = 0.1,
                            Max = 0.9,
                            Zone = ""
                        }
                    },
                    ConstructionPartsBuilt = new List<object>(),
                    // InventoryAttachments = new List<InventoryAttachment>(),
                    InventoryAttachments = _spawnableTypesHelper
                        .GetAdditionalAttachments(x.Name)
                        .ToList()
                        .SideEffect(x =>
                        {
                            x.Items.ForEach(y =>
                            {
                                if (!IsSuppressor(y.ClassName))
                                {
                                    y.Chance = 0.2;
                                }
                            });
                        })
                        .ToList(),
                    InventoryCargo = new List<InventoryCargoModel>(),
                }
        );

        splat.InventoryCargo.AddRange(extras);
        splat.InventoryCargo = splat.InventoryCargo.DistinctBy(x => x.ClassName).ToList();

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
            ["Vest"] = new[]
            {
                "MMG_JPC_Vest_black",
                "MMG_tt_Vest_black",
                "MMG_chestrig_black",
                "MMG_MK_III_Armor_black",
                "MMG_MK_V_Armor_black",
                "MMG_JPC_Vest_green",
                "MMG_tt_Vest_green",
                "MMG_chestrig_green",
                "MMG_MK_III_Armor_green",
                "MMG_MK_V_Armor_green",
                "MMG_tt_Vest_police",
                "JuggernautLVL5_Suit",
                "JuggernautLVL5_Tan",
                "JuggernautLVL5_Black",
                "JuggernautLVL5_Winter",
                "JuggernautLVL1_Suit",
                "JuggernautLVL1_Suit_Tan",
                "JuggernautLVL1_Suit_Black",
                "JuggernautLVL1_Suit_Winter",
                "quad_gen2_Suit",
                "quad_Gen2_Black",
                "quad_Gen2_Green",
                "6b13Vest_FloraBrown",
                "6b13Vest_PixelGreen",
                "6b13Vest_PixelBlack",
            },
            ["Back"] = new[]
            {
                "MMG_carrier_backpack_black",
                "MMG_supplybag_black",
                "MMG_assault_pack_black",
                "MMG_camelback_black",
                "MMG_mmps_bag_black",
                "MMG_carrier_backpack_green",
                "MMG_supplybag_green",
                "MMG_assault_pack_green",
                "MMG_camelback_green",
                "MMG_mmps_bag_green",
                "bag_6B38_black_mung",
                "bag_6B38_LETO_mung",
                "rifleholster_black_mung",
                "rifleholster_TTsKO_mung",
            },
            ["Hips"] = new[]
            {
                "MMG_falcon_b1_belt_black",
                "MMG_falcon_b1_belt_green",
                "mmg_cargobelt_black",
                "mmg_cargobelt_green",
                "mmg_cargobelt_tan",
                "tacticalbelt_quad_black",
                "tacticalbelt_quad_green",
                "tacticalbelt_quad_tan",
            },
            ["Headgear"] = new[]
            {
                "MMG_tactical_helmet_black",
                "MMG_striker_helmet_black",
                "mmg_armored_helmet_black",
                "MMG_tactical_helmet_green",
                "MMG_striker_helmet_green",
                "mmg_armored_helmet_green",
                "MMG_tactical_helmet_police",
                "quad_Ronin_Helmet",
                "quad_Ronin_Gold",
                "Quad_ArmourPlate",
            },
            ["Body"] = new[]
            {
                "MMG_operatorshirt_black",
                "MMG_tactical_shirt_black",
                "MMG_combatshirt_black",
                "MMG_operatorshirt_green",
                "MMG_tactical_shirt_green",
                "MMG_combatshirt_green",
                "MMG_combatshirt_police",
            },
            ["Legs"] = new[]
            {
                "MMG_combatpants_black",
                "mmg_tactical_pants_black",
                "MMG_combatpants_green",
                "mmg_tactical_pants_green",
                "MMG_combatpants_police",
            },
        }.ToDictionary(
            kvp => kvp.Key,
            kvp =>
                kvp.Value.Select(
                    item =>
                        new Item
                        {
                            Chance = _weaponChanceCategories.Get(CategoryValue.Medium),
                            ClassName = item,
                            Quantity = new Quantity { Min = 0, Max = 0 },
                            Sets = new List<LoadoutSet>(),
                            ConstructionPartsBuilt = new List<object>(),
                            Health = new List<Health>
                            {
                                new Health { Min = 0.5, Max = 0.9 }
                            },
                            InventoryAttachments = _spawnableTypesHelper
                                .GetAdditionalAttachments(item)
                                .ToList(),
                            InventoryCargo = new List<InventoryCargoModel>(),
                        }
                )
        );

        foreach (var att in splat.InventoryAttachments)
        {
            att.Items = att.Items.DistinctBy(x => x.ClassName).ToList();

            if (extraStuff.TryGetValue(att.SlotName, out var value) && value is not null)
            {
                att.Items.AddRange(value);
            }
            // if (att.SlotName == "Vest") att.Items.AddRange(extraStuff["Vest"]);
            // else if (att.SlotName == "Headgear") att.Items.AddRange(extraStuff["Headgear"]);
        }

        splat.InventoryAttachments = splat.InventoryAttachments
            .Where(x => x.SlotName != "Shoulder")
            .ToList();
    }

    private static void AssignInventoryAttachments(AiLoadoutRoot splat, AiLoadoutRoot item)
    {
        foreach (var attachment in item.InventoryAttachments)
        {
            var toAssign = splat.InventoryAttachments.FirstOrDefault(
                x => x.SlotName == attachment.SlotName
            );
            if (toAssign is null)
            {
                splat.InventoryAttachments.Add(attachment);
            }
            else if (attachment.Items is not null)
            {
                if (attachment.SlotName == "Melee")
                {
                    if (attachment.Items.Any(xx => xx.ClassName == "StunBaton"))
                    {
                        attachment.Items.First(xx => xx.ClassName == "StunBaton").Chance = 0.1;
                    }
                }
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
            .SideEffect(set =>
            {
                if (set.ClassName == "WEAPON")
                {
                    if (IsIj(set))
                    {
                        set.Chance = 0.25;
                    }
                    else
                    {
                        // set.Chance = 0.23;
                        // set.Chance = 0.11;
                        set.Chance = _weaponChanceCategories.Get(CategoryValue.Minimal);
                    }
                }
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

        splat.Sets.AddRange(_weaponSets.All);
    }

    private static bool IsIj(LoadoutSet x) =>
        x.InventoryAttachments?.FirstOrDefault()?.Items?.FirstOrDefault()?.ClassName.Contains("IJ")
            is true;

    private static void AssignSets(AiLoadoutRoot splat, AiLoadoutRoot item)
    {
        foreach (var set in item.Sets)
        {
            splat.Sets.Add(set);
        }
    }
}

public class WeaponSetDefs
{
    private readonly SpawnableTypesHelper _spawnableTypesHelper;
    private readonly CategorizedDouble _weaponChanceCategories;

    public WeaponSetDefs(
        string rootDir,
        string mpMissionName,
        CategorizedDouble weaponChanceCategories
    )
    {
        _spawnableTypesHelper = new SpawnableTypesHelper(rootDir, mpMissionName);
        _weaponChanceCategories = weaponChanceCategories;
    }

    public IEnumerable<LoadoutSet> All =>
        new[]
        {
            // Awm,
            // Kiivari,
            // Aek545,
            // S_R700,
            M_R700,
            // ScarH,
            // AK12,
            // AK19,
            MMAKM,
            MMMk47,
        };

    public LoadoutSet SNAFUSR25 =>
        GetLoadoutSetWeapon(
            "SNAFU_SR25_Gun",
            _weaponChanceCategories.Get(CategoryValue.Minimal),
            new[] { "Ammo_308Win", "Ammo_308Win", "Ammo_308Win", }
        );

    public LoadoutSet MMMk47 =>
        GetLoadoutSetWeapon(
            "TTC_Mk47",
            _weaponChanceCategories.Get(CategoryValue.Minimal),
            new[] { "Ammo_762x39", "Ammo_762x39", "Ammo_762x39", }
        );
    public LoadoutSet MMAKM =>
        GetLoadoutSetWeapon(
            "TTC_AKM",
            _weaponChanceCategories.Get(CategoryValue.Minimal),
            new[] { "Ammo_762x39", "Ammo_762x39", "Ammo_762x39", }
        );
    public LoadoutSet AK19 =>
        GetLoadoutSetWeapon(
            "SNAFU_AK19",
            _weaponChanceCategories.Get(CategoryValue.Minimal),
            new[] { "Ammo_556x45", "Ammo_556x45", "Ammo_556x45", }
        );
    public LoadoutSet AK12 =>
        GetLoadoutSetWeapon(
            "SNAFU_AK12A",
            _weaponChanceCategories.Get(CategoryValue.Minimal),
            new[] { "Ammo_545x39", "Ammo_545x39", "Ammo_545x39" }
        );
    public LoadoutSet ScarH =>
        GetLoadoutSetWeapon(
            "Snafu_ScarH_Black_GUN",
            _weaponChanceCategories.Get(CategoryValue.Minimal),
            new[] { "Ammo_308Win", "Ammo_308Win" }
        );
    public LoadoutSet M_R700 =>
        GetLoadoutSetWeapon(
            "TTC_R700",
            _weaponChanceCategories.Get(CategoryValue.Medium),
            new[] { "Ammo_308Win", "Ammo_308Win" }
        );
    public LoadoutSet S_R700 =>
        GetLoadoutSetWeapon(
            "GCGN_M700",
            _weaponChanceCategories.Get(CategoryValue.Medium),
            new[] { "Ammo_308Win", "Ammo_308Win" }
        );
    public LoadoutSet Aek545 =>
        GetLoadoutSetWeapon("SNAFU_AEK545_Gun", _weaponChanceCategories.Get(CategoryValue.Minimal));
    public LoadoutSet Awm =>
        GetLoadoutSetWeapon(
            "SNAFU_AWM_Gun",
            _weaponChanceCategories.Get(CategoryValue.Minimal),
            new[] { "SNAFU_Ammo_338", "SNAFU_Ammo_338" }
        );
    public LoadoutSet Kiivari =>
        GetLoadoutSetWeapon(
            "SNAFUKivaari_Black_GUN",
            _weaponChanceCategories.Get(CategoryValue.Minimal),
            new[] { "SNAFU_Ammo_338", "SNAFU_Ammo_338" }
        );

    private LoadoutSet GetLoadoutSetWeapon(
        string className,
        double chance = 1,
        IEnumerable<string>? extraInventory = null
    )
    {
        extraInventory ??= new List<string>();
        var possibleAttachments = _spawnableTypesHelper
            .GetAdditionalAttachments(className)
            .SelectMany(x => x.Items);
        var additionalMags = possibleAttachments
            .Where(y => y.ClassName.ToUpper().Contains("MAG"))
            .Select(
                y =>
                    new InventoryCargoModel
                    {
                        Chance = 1,
                        ClassName = y.ClassName,
                        Quantity = GenerateSplattedLoadout.DefaultQuantity
                    }
            )
            .ToList();

        if (additionalMags.Count == 1)
        {
            additionalMags.Add(additionalMags.Single());
        }

        var extraInventoryCargo = extraInventory.Select(
            extra =>
                new InventoryCargoModel
                {
                    Chance = 1,
                    ClassName = extra,
                    Quantity = GenerateSplattedLoadout.DefaultQuantity
                }
        );
        var inventoryCargo = additionalMags.Concat(extraInventoryCargo).ToList();
        return new LoadoutSet
        {
            ClassName = "WEAPON",
            Chance = chance,
            Health = new List<Health>
            {
                new Health { Min = 0.4, Max = 0.9 },
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
                            Health = new List<Health>
                            {
                                new Health { Min = 0.5, Max = 0.9 }
                            },
                            InventoryAttachments = _spawnableTypesHelper
                                .GetAdditionalAttachments(className)
                                .ToList(),
                        }
                    }
                }
            },
            InventoryCargo = inventoryCargo,
        };
    }
}
