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

        var types = xd.Root.OrFail().Nodes().OfType<XElement>().Select(DzTypesXmlTypeElement.FromElement);

        foreach (var type in types)
        {
            ProcessByConfiguration(type);
            GeneralAdjustments(type);
            ProcessBaseDayzItems(type);
            ProcessExpansion(type);
            ProcessAdvancedWeaponScopes(type);
            ProcessSnafu(type);
            ProcessDzn(type);
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

public class AdjustTypesXmlConfigurationRule
{
    public uint? Nominal { get; set; }
    public uint? Min { get; set; }
    public uint? Lifetime { get; set; }
    public uint? Restock { get; set; }
    public string? Category { get; set; }

    public List<string> Values { get; set; } = new List<string>();
    public List<string> Usages { get; set; } = new List<string>();
    public List<string> Tags { get; set; } = new List<string>();

    public string? StartsWith { get; set; }
    public string? NameEqualsCaseInsensitive { get; set; }
    public List<string> NameContainsSubstringCaseInsensitive { get; set; } = new List<string>();

    public override string ToString()
    {
        return $"SW:{StartsWith}|CON:{string.Join(":", NameContainsSubstringCaseInsensitive)}";
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
    }

    public static bool Matches(this AdjustTypesXmlConfigurationRule rule, DzTypesXmlTypeElement type)
    {
        var match = true;
        if (rule.NameEqualsCaseInsensitive is string nameEquals)
        {
            if (type.Name.ToUpperInvariant() != nameEquals.ToUpperInvariant())
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
