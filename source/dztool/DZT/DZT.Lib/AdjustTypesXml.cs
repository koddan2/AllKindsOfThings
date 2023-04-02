using System.Text.Json;
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

    public AdjustTypesXml(
        ILogger logger,
        AdjustTypesXmlConfiguration configuration,
        string rootDir,
        string mpMissionName
    )
    {
        _logger = logger;
        _configuration = configuration;
        _rootDir = rootDir;
        _mpMissionName = mpMissionName;
    }

    public string GeneratedTypesXmlFileName { get; set; } = "generated_types.xml";

    public bool IsGeneratedTypesXmlFile(string filePath) =>
        Path.GetFileName(filePath) == GeneratedTypesXmlFileName;

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

        var extraFromQuad = new[]
        {
            // "tacticalbelt_quad_black",
            // "tacticalbelt_quad_green",
            // "tacticalbelt_quad_tan",
            // "riflesling_quad_black",
            // "riflesling_quad_green",
            // "riflesling_quad_tan",
            // "riflesling_quad_winter",
            // "rifleslingfront_quad_black",
            // "rifleslingfront_quad_green",
            // "rifleslingfront_quad_tan",
            // "rifleslingfront_quad_winter",
            // "rifleslingback_quad_black",
            // "rifleslingback_quad_green",
            // "rifleslingback_quad_tan",
            // "rifleslingback_quad_winter",
            "JuggernautLVL5_Suit",
            "JuggernautLVL5_Tan",
            "JuggernautLVL5_Black",
            "JuggernautLVL5_Winter",
            "JuggernautLVL1_Suit",
            "JuggernautLVL1_Suit_Tan",
            "JuggernautLVL1_Suit_Black",
            "JuggernautLVL1_Suit_Winter",
            "Juggernaut_Suit_Buttpack",
            "Juggernaut_Buttpack_Tan",
            "Juggernaut_Buttpack_Black",
            "Juggernaut_Buttpack_Winter",
            "Juggernaut_Suit_Pouches",
            "Juggernaut_Pouches_Tan",
            "Juggernaut_Pouches_Black",
            "Juggernaut_Pouches_Winter",
        };

        if (IsGeneratedTypesXmlFile(inputFilePath))
        {
            foreach (var item in extraFromQuad)
            {
                var xml = DzTypesXmlTypeElement.XmlTemplate(item);
                using var tempStream = StreamHelper.GenerateStreamFromString(xml);
                var tempXd = XDocument.Load(tempStream);
                var tempType = DzTypesXmlTypeElement.FromElement(tempXd.Root!);
                tempType.Category = "clothes";
                xd.Root!.Add(tempXd.Root);
            }
        }

        var types = DzTypesXmlTypeElement.FromDocument(xd);

        foreach (var type in types)
        {
            ProcessByConfiguration(type, xd, inputFilePath);
            GeneralAdjustments(type);
            // ProcessBaseDayzItems(type);
            //ProcessExpansion(type);
            //ProcessAdvancedWeaponScopes(type);
            //ProcessSnafu(type);
            //ProcessDzn(type);
        }

        ProcessInsertionsByConfiguration(xd, inputFilePath);

        using var fs = FileManagement.Utf8WithoutBomWriter(inputFilePath);
        xd.Save(fs);
    }

    private void ProcessInsertionsByConfiguration(XDocument xd, string pathToFile)
    {
        foreach (var rule in _configuration.Rules)
        {
            if (
                rule.Action is AdjustTypesXmlConfigurationRuleAction.Insert
                && IsGeneratedTypesXmlFile(pathToFile)
            )
            {
                foreach (var nameStr in rule.Names)
                {
                    var xe = new XElement("type", new XAttribute("name", nameStr));
                    xd.Root!.Add(xe);
                    rule.Apply(DzTypesXmlTypeElement.FromElement(xe));
                }
            }
        }
    }

    private void ProcessByConfiguration(DzTypesXmlTypeElement type, XDocument _, string pathToFile)
    {
        foreach (var rule in _configuration.Rules)
        {
            if (rule.Action == AdjustTypesXmlConfigurationRuleAction.Insert)
            {
                continue;
            }
            else if ( /**/
                rule.Action is AdjustTypesXmlConfigurationRuleAction.Update
                || rule.Action is AdjustTypesXmlConfigurationRuleAction.Remove
            )
            {
                if (rule.Matches(type, pathToFile))
                {
                    if (
                        rule.Action == AdjustTypesXmlConfigurationRuleAction.Remove
                        && type.Element.Parent is null
                    )
                    {
                        continue;
                    }

                    rule.Apply(type);
                }
            }
        }
    }

    private static void GeneralAdjustments(DzTypesXmlTypeElement type)
    {
        return;
        var keepLifetime = type.Category == "containers";
        if (!keepLifetime)
        {
            type.Lifetime = Math.Max(
                (int)TimeSpan.FromMinutes(15).TotalSeconds,
                (int)type.Lifetime / 2
            );
        }
        type.Restock = 0;
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

    Insert = 2,

    Update = 3,
}

public class AdjustTypesXmlConfigurationRule
{
    public AdjustTypesXmlConfigurationRuleAction? Action { get; set; } =
        AdjustTypesXmlConfigurationRuleAction.Update;
    public uint? Nominal { get; set; }
    public uint? NominalModifyDiv { get; set; }
    public uint? Min { get; set; }
    public uint? MinModifyDiv { get; set; }
    public uint? Lifetime { get; set; }
    public uint? Restock { get; set; }
    public string? Category { get; set; }

    public AdjustTypesXmlConfigurationRuleFlags? Flags { get; set; }

    public List<string>? Values { get; set; } = null;
    public List<string>? Usages { get; set; } = null;
    public List<string>? Tags { get; set; } = null;

    /// Names for insertion
    public List<string> Names { get; set; } = new();
    public string? InFile { get; set; }
    public List<string> InFiles { get; set; } = new();
    public string? StartsWith { get; set; }
    public string? StartsWithCaseInsensitive { get; set; }
    public List<string> NameEqualsCaseInsensitive { get; set; } = new();
    public List<string> NameContainsSubstringCaseInsensitive { get; set; } = new();
    public List<string> NotNameContainsSubstringCaseInsensitive { get; set; } = new();

    public override string ToString()
    {
        // return $"SW:{StartsWith}|EQ:({string.Join(":", NameEqualsCaseInsensitive)})|CON:{string.Join(":", NameContainsSubstringCaseInsensitive)}";
        return JsonSerializer.Serialize(this, DefaultSettings.JsonSerializerOptions);
    }
}

public class AdjustTypesXmlConfiguration
{
    public string Title { get; set; } = "";
    public List<AdjustTypesXmlConfigurationRule> Rules { get; set; } =
        new List<AdjustTypesXmlConfigurationRule>();
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
        if (rule.NominalModifyDiv is uint nominalModifyDiv)
        {
            type.Nominal = Math.Max(0, Convert.ToInt32(type.Nominal / nominalModifyDiv));
        }

        if (rule.Min is uint min)
        {
            type.Min = Convert.ToInt32(min);
        }
        if (rule.MinModifyDiv is uint minModifyDiv)
        {
            type.Min = Math.Max(0, Convert.ToInt32(type.Min / minModifyDiv));
        }

        if (rule.Lifetime is uint lifetime)
        {
            type.Lifetime = Convert.ToInt32(lifetime);
        }

        if (rule.Restock is uint restock)
        {
            type.Restock = Convert.ToInt32(restock);
        }

        if (rule.Values is List<string> values)
            type.Values = values.ToArray();
        if (rule.Usages is List<string> usages)
            type.Usages = usages.ToArray();
        if (rule.Tags is List<string> tags)
            type.Tags = tags.ToArray();
        if (rule.Category is string category)
            type.Category = category;

        if (rule.Flags is AdjustTypesXmlConfigurationRuleFlags flags)
        {
            type.Flags = new Dictionary<string, string>
            {
                ["count_in_map"] = flags.CountInMap,
                ["count_in_hoarder"] = flags.CountInHoarder,
                ["count_in_cargo"] = flags.CountInCargo,
                ["count_in_player"] = flags.CountInPlayer,
                ["crafted"] = flags.Crafted,
                ["deloot"] = flags.Deloot,
            };
        }
    }

    public static bool Matches(
        this AdjustTypesXmlConfigurationRule rule,
        DzTypesXmlTypeElement type,
        string pathToFile
    )
    {
        ////if (rule.NameEqualsCaseInsensitive.Count > 0)
        ////{
        ////    var a = 1;
        ////}
        var match = true;

        if (rule.InFile is string inFile)
        {
            if (Path.GetFileName(pathToFile) == inFile)
            {
                match = true;
            }
            else
            {
                match = false;
            }
        }
        if (rule.InFiles.Any())
        {
            if (rule.InFiles.Contains(Path.GetFileName(pathToFile)))
            {
                match = true;
            }
            else
            {
                match = false;
            }
        }

        if (rule.NameEqualsCaseInsensitive.Count > 0)
        {
            var equals = rule.NameEqualsCaseInsensitive.Any(
                x => x.ToUpperInvariant() == type.NameUpper
            );
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
        else if (rule.StartsWithCaseInsensitive is string startsWithCaseInsensitive)
        {
            if (!type.NameUpper.StartsWith(startsWithCaseInsensitive.ToUpperInvariant()))
            {
                match = false;
                return match;
            }
        }

        if (rule.NotNameContainsSubstringCaseInsensitive.Count > 0)
        {
            match =
                match
                && rule.NotNameContainsSubstringCaseInsensitive.All(substr =>
                {
                    var nameUpper = type.Name.ToUpperInvariant();
                    return !nameUpper.Contains(substr.ToUpperInvariant());
                });
        }

        if (rule.NameContainsSubstringCaseInsensitive.Count > 0)
        {
            match =
                match
                && rule.NameContainsSubstringCaseInsensitive.Any(substr =>
                {
                    var nameUpper = type.Name.ToUpperInvariant();
                    return nameUpper.Contains(substr.ToUpperInvariant());
                });
        }

        return match;
    }
}
