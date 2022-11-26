using System.Xml.Linq;
using DZT.Lib.Helpers;
using DZT.Lib.Models;
using Microsoft.Extensions.Logging;
using SAK;

namespace DZT.Lib;

public class AdjustTypesXml
{
    private readonly ILogger _logger;
    private readonly AdjustTypesXmlConfiguration _configuration;
    private readonly string _rootDir;
    private readonly string _mpMissionName;

    public AdjustTypesXml(ILogger logger, AdjustTypesXmlConfiguration configuration, string rootDir, string mpMissionName)
    {
        _logger = logger;
        _configuration = configuration;
        _rootDir = rootDir;
        _mpMissionName = mpMissionName;
    }

    public void Process()
    {
        foreach (var file in DayzFilesHelper.GetAllTypesXmlFileNames(_rootDir, _mpMissionName))
        {
            ProcessSingle(file);
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

        FileManagement.FormatXmlFileInPlace(inputFilePath);
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

        var extraFromQuad = new[] {
            // "tacticalbelt_quad_black",
            // "tacticalbelt_quad_green",
            // "tacticalbelt_quad_tan",
            "riflesling_quad_black",
            // "riflesling_quad_green",
            // "riflesling_quad_tan",
            // "riflesling_quad_winter",
            "rifleslingfront_quad_black",
            // "rifleslingfront_quad_green",
            // "rifleslingfront_quad_tan",
            // "rifleslingfront_quad_winter",
            "rifleslingback_quad_black",
            // "rifleslingback_quad_green",
            // "rifleslingback_quad_tan",
            // "rifleslingback_quad_winter",
            // "JuggernautLVL5_Suit",
            "JuggernautLVL5_Tan",
            "JuggernautLVL5_Black",
            // "JuggernautLVL5_Winter",
            // "JuggernautLVL1_Suit",
            "JuggernautLVL1_Suit_Tan",
            "JuggernautLVL1_Suit_Black",
            // "JuggernautLVL1_Suit_Winter",
            // "Juggernaut_Suit_Buttpack",
            "Juggernaut_Buttpack_Tan",
            "Juggernaut_Buttpack_Black",
            // "Juggernaut_Buttpack_Winter",
            // "Juggernaut_Suit_Pouches",
            "Juggernaut_Pouches_Tan",
            "Juggernaut_Pouches_Black",
            // "Juggernaut_Pouches_Winter",
        };

        if (Path.GetFileName(inputFilePath) == "generated_types.xml")
            foreach (var item in extraFromQuad)
            {
                using var tempStream = StreamHelper.GenerateStreamFromString($@"
                <type name=""{item}"">
                    <nominal>4</nominal>
                    <lifetime>1440</lifetime>
                    <restock>0</restock>
                    <min>1</min>
                    <quantmin>-1</quantmin>
                    <quantmax>-1</quantmax>
                    <cost>100</cost>
                    <flags count_in_map=""1"" count_in_hoarder=""0"" count_in_cargo=""0"" ount_in_player=""0"" crafted=""0"" deloot=""1"" />
                </type>");
                var tempXd = XDocument.Load(tempStream);
                var tempType = DzTypesXmlTypeElement.FromElement(tempXd.Root!);
                tempType.Category = "clothes";
                xd.Root!.Add(tempXd.Root);
            }

        var types = DzTypesXmlTypeElement.FromDocument(xd);

        foreach (var type in types)
        {
            ProcessByConfiguration(type);
            GeneralAdjustments(type);
            ProcessBaseDayzItems(type);
            //ProcessExpansion(type);
            //ProcessAdvancedWeaponScopes(type);
            //ProcessSnafu(type);
            //ProcessDzn(type);
        }

        using var fs = FileManagement.Utf8WithoutBomWriter(inputFilePath);
        xd.Save(fs);
    }


    private void ProcessByConfiguration(DzTypesXmlTypeElement type)
    {
        foreach (var rule in _configuration.Rules)
        {
            if (rule.Matches(type))
            {
                rule.Apply(type);
            }
        }
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
            return y.Key == "count_in_map" ? "1" : "0";
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

public class AdjustTypesXmlConfigurationRuleFlags
{
    public string CountInMap { get; set; } = "0";
    public string CountInHoarder { get; set; } = "0";
    public string CountInCargo { get; set; } = "0";
    public string CountInPlayer { get; set; } = "0";
    public string Crafted { get; set; } = "0";
    public string Deloot { get; set; } = "0";
}
public enum AdjustTypesXmlConfigurationRuleAction
{
    Remove = 1,
}
public class AdjustTypesXmlConfigurationRule
{
    public AdjustTypesXmlConfigurationRuleAction? Action { get; set; }
    public uint? Nominal { get; set; }
    public uint? Min { get; set; }
    public uint? Lifetime { get; set; }
    public uint? Restock { get; set; }
    public string? Category { get; set; }

    public AdjustTypesXmlConfigurationRuleFlags? Flags { get; set; }

    public List<string> Values { get; set; } = new List<string>();
    public List<string> Usages { get; set; } = new List<string>();
    public List<string> Tags { get; set; } = new List<string>();

    public string? StartsWith { get; set; }
    public List<string> NameEqualsCaseInsensitive { get; set; } = new List<string>();
    public List<string> NameContainsSubstringCaseInsensitive { get; set; } = new List<string>();
    public List<string> NotNameContainsSubstringCaseInsensitive { get; set; } = new List<string>();


    public override string ToString()
    {
        return $"SW:{StartsWith}|EQ:({string.Join(":", NameEqualsCaseInsensitive)})|CON:{string.Join(":", NameContainsSubstringCaseInsensitive)}";
    }
}

public class AdjustTypesXmlConfiguration
{
    public string Title { get; set; } = "";
    public List<AdjustTypesXmlConfigurationRule> Rules { get; set; } = new List<AdjustTypesXmlConfigurationRule>();
}

public static class AdjustTypesXmlConfigurationExtensions
{
    public static void Apply(this AdjustTypesXmlConfigurationRule rule, DzTypesXmlTypeElement type)
    {
        if (rule.Action == AdjustTypesXmlConfigurationRuleAction.Remove)
        {
            type.Element.Remove();
        }

        if (rule.Nominal is uint nominal)
        {
            type.Nominal = Convert.ToInt32(nominal);
        }

        if (rule.Min is uint min)
        {
            type.Min = Convert.ToInt32(min);
        }

        if (rule.Lifetime is uint lifetime)
        {
            type.Lifetime = Convert.ToInt32(lifetime);
        }

        if (rule.Restock is uint restock)
        {
            type.Restock = Convert.ToInt32(restock);
        }

        type.Values = rule.Values.ToArray();
        type.Usages = rule.Usages.ToArray();
        type.Tags = rule.Tags.ToArray();
        type.Category = rule.Category;

        if (rule.Flags is AdjustTypesXmlConfigurationRuleFlags flags)
        {
            type.Flags = new Dictionary<string, string>
            {
                ["count_in_map"] = flags.CountInMap,
                ["count_in_hoarder"] = flags.CountInHoarder,
                ["count_in_cargo"] = flags.CountInCargo,
                ["ount_in_player"] = flags.CountInPlayer,
                ["crafted"] = flags.Crafted,
                ["deloot"] = flags.Deloot,
            };
        }
    }

    public static bool Matches(this AdjustTypesXmlConfigurationRule rule, DzTypesXmlTypeElement type)
    {
        ////if (rule.NameEqualsCaseInsensitive.Count > 0)
        ////{
        ////    var a = 1;
        ////}
        var match = true;
        if (rule.NameEqualsCaseInsensitive.Count > 0)
        {
            var equals = rule.NameEqualsCaseInsensitive.Any(x => x.ToUpperInvariant() == type.NameUpper);
            if (!equals)
            {
                match = false;
                return match;
            }
        }
        else if (rule.StartsWith is string startsWith)
        {
            if (!type.Name.StartsWith(startsWith))
            {
                match = false;
                return match;
            }
        }

        if (rule.NotNameContainsSubstringCaseInsensitive.Count > 0)
        {
            match = match && rule.NotNameContainsSubstringCaseInsensitive.All(substr =>
            {
                var nameUpper = type.Name.ToUpperInvariant();
                return !nameUpper.Contains(substr.ToUpperInvariant());
            });
        }

        if (rule.NameContainsSubstringCaseInsensitive.Count > 0)
        {
            match = match && rule.NameContainsSubstringCaseInsensitive.Any(substr =>
            {
                var nameUpper = type.Name.ToUpperInvariant();
                return nameUpper.Contains(substr.ToUpperInvariant());
            });
        }

        return match;
    }
}
