using DZT.Lib.Helpers;
using DZT.Lib.Models;
using System.Linq;
using System.Text.Json;

namespace DZT.Lib;
public class UpdateAiPatrols
{
    private readonly Random _rng = new();
    private readonly string _json;
    private readonly string _aiPatrolSettingsJsonFile;
    private readonly AiPatrolSettingsRoot _settings;

    private readonly string[] _forbiddenObjects = new[]
    {
        "GardenPlot",
        "Polytunnel",
        "Land_Dam_Concrete_20_Floodgate",
        "Land_Train_Wagon_Tanker",
    };

    public CategorizedDouble CategorizedDouble { get; set; } = new CategorizedDouble(
        minimal: 0.004,
        small: 0.07,
        medium: 0.45,
        large: 0.7);

    double Chance(CategoryValue category, double modifier = 0d)
    {
        return CategorizedDouble.Get(category, modifier);
    }

    public UpdateAiPatrols(string rootDir, string mpMissionName)
    {
        Validators.ValidateDirExists(rootDir);
        var relativePathToFile = Path.Combine("mpmissions", mpMissionName, @"expansion\settings\AIPatrolSettings.json");
        _aiPatrolSettingsJsonFile = Path.Combine(rootDir, relativePathToFile);
        Validators.ValidateFileExists(_aiPatrolSettingsJsonFile);

        FileManagement.TryRestoreFileV2(rootDir, relativePathToFile);
        var result = FileManagement.BackupFileV2(rootDir, relativePathToFile);
        _json = File.ReadAllText(_aiPatrolSettingsJsonFile);
        _settings = JsonSerializer.Deserialize<AiPatrolSettingsRoot>(_json)!;
    }

    public void Process()
    {
        _settings.MVersion = 11;
        _settings.Enabled = 1;
        _settings.AccuracyMin = 0.52;
        _settings.AccuracyMax = 0.82;
        _settings.DespawnRadius = 1000;
        _settings.DespawnTime = 20;
        _settings.MinDistRadius = 80;
        _settings.MaxDistRadius = 950;
        _settings.ThreatDistanceLimit = 400;
        _settings.DamageMultiplier = 1.0;

        AddObjectPatrol("East", new[] { "Land_City_PoliceStation", "Land_Village_PoliceStation" }, p =>
        {
            p.LoadoutFile = "SplattedLoadout";
            // p.Chance = Math.Min(1.0, BaseSpawnChanceMedium + 0.1);
            p.Chance = Chance(CategoryValue.Medium, 0.1);
        });
        AddObjectPatrol("East", new[]
        {
            "Land_City_Hospital",
            "Land_Village_HealthCare",
            "Land_Village_store",
            "Land_Power_Station",
            "Land_City_Store",
            "Land_City_Store_WithStairs",
        },
        p =>
        {
            p.NumberOfAI = -6;
            p.Faction = "Raiders";
            p.LoadoutFile = "SplattedLoadout";
            // p.Chance = Math.Min(1.0, BaseSpawnChanceMedium + 0.1);
            p.Chance = Chance(CategoryValue.Medium, 0.1);
        });

        AddObjectPatrol("East", new[] { "Land_Mil_Airfield_HQ", "Land_Mil_ATC_Small", "Land_Mil_ATC_Big" }, p =>
        {
            p.Faction = "Raiders";
            p.LoadoutFile = "SplattedLoadout";
            // p.Chance = BaseSpawnChanceMedium;
            p.Chance = Chance(CategoryValue.Medium);
            p.NumberOfAI = -4;
        });

        AddObjectPatrol("East", new[] { "Land_City_FireStation" }, p =>
        {
            // p.Chance = Math.Min(1.0, BaseSpawnChanceMedium + 0.0);
            p.Chance = Chance(CategoryValue.Medium);
            p.Faction = "Raiders";
            p.LoadoutFile = "SplattedLoadout";
            p.NumberOfAI = -4;
        });

        AddObjectPatrol("East", new[] {
            "Land_City_Stand_Grocery",
            "Land_House_1B01_Pub",
            }, p =>
        {
            p.Faction = "Raiders";
            p.LoadoutFile = "SplattedLoadout";
            p.NumberOfAI = -4;
            // p.Chance = Math.Min(1.0, BaseSpawnChanceMedium - 0.0);
            p.Chance = Chance(CategoryValue.Medium);
        });

        var structureClassNames = DataHelper.GetStructureClassNames();
        var objectsForPatrolCity = new[]
            {
                structureClassNames["**Residential**"],
            }
            .SelectMany(x => x)
            .Where(x => !_forbiddenObjects.Contains(x))
            .ToArray();

        AddObjectPatrol(
            "East",
            objectsForPatrolCity,
            p =>
            {
                p.Faction = "Mercenaries";
                // p.Faction = "Raiders";
                p.LoadoutFile = "SplattedLoadout";
                // p.Chance = 0.002;
                p.Chance = Chance(CategoryValue.Minimal);
                p.NumberOfAI = -2;
            });

        var objectsForPatrolRest = new[]
            {
                structureClassNames["**Industrial**"],
                structureClassNames["**Specific**"],
                structureClassNames["**Military**"],
            }
            .SelectMany(x => x)
            .Where(x => !_forbiddenObjects.Contains(x))
            .ToArray();

        AddObjectPatrol(
            "East",
            objectsForPatrolRest,
            p =>
            {
                p.Faction = "Mercenaries";
                p.LoadoutFile = "SplattedLoadout";
                // p.Chance = 0.05;
                p.Chance = Chance(CategoryValue.Small);
                p.NumberOfAI = -3;
            });

        AddObjectPatrol("East", structureClassNames["**Military**"].ToArray(), p =>
        {
            p.Faction = "East";
            p.LoadoutFile = "SplattedLoadout";
            // p.Chance = 0.005;
            p.Chance = Chance(CategoryValue.Minimal);
            p.NumberOfAI = -3;
        });

        AddObjectPatrol("West", structureClassNames["**Military**"].ToArray(), p =>
        {
            p.Faction = "West";
            ////p.LoadoutFile = "WestLoadout";
            p.LoadoutFile = "SplattedLoadout";
            // p.Chance = 0.005;
            p.Chance = Chance(CategoryValue.Minimal);
            p.NumberOfAI = -3;
        });

        _settings.ObjectPatrols.ToList().ForEach(p =>
        {
            p.Faction = "Mercenaries";
            p.UnlimitedReload = 1;
            p.LoadoutFile = "SplattedLoadout";
            p.ThreatDistanceLimit = -1;
            p.DamageMultiplier = -1;
        });

        _settings.Patrols.ToList().ForEach(p =>
        {
            p.Faction = "Mercenaries";
            p.UnlimitedReload = 1;
            p.LoadoutFile = "SplattedLoadout";
            p.ThreatDistanceLimit = -1;
            p.DamageMultiplier = -1;
        });

        using var fs = FileManagement.Utf8WithoutBomWriter(_aiPatrolSettingsJsonFile);
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        JsonSerializer.Serialize(fs.BaseStream, _settings, jsonOptions);
    }

    private ObjectPatrol GetObjectPatrol(string faction)
    {
        var settings = JsonSerializer.Deserialize<AiPatrolSettingsRoot>(_json)!;
        return settings.ObjectPatrols.First(x => x.Faction == faction);
    }

    private void AddObjectPatrol(string faction, string className, Action<ObjectPatrol>? mutate = null)
    {
        var extraPatrol = GetObjectPatrol(faction);
        extraPatrol.ClassName = className;
        mutate?.Invoke(extraPatrol);
        _settings.ObjectPatrols.Add(extraPatrol);
    }

    private void AddObjectPatrol(string faction, string[] classNames, Action<ObjectPatrol>? mutate = null)
    {
        foreach (var className in classNames)
        {
            AddObjectPatrol(faction, className, mutate);
        }
    }

    public string GetRandomLoadout()
    {
        var candidates = new[]
        {
            "BanditLoadout",
            "EastLoadout",
            "FireFighterLoadout",
            "GorkaLoadout",
            "HumanLoadout",
            "NBCLoadout",
            "PlayerFemaleSuitLoadout",
            "PlayerMaleSuitLoadout",
            "PlayerSurvivorLoadout",
            "PoliceLoadout",
            "SurvivorLoadout",
            "TTSKOLoadout",
            "WestLoadout",
        };

        return candidates[_rng.Next(candidates.Length)];
    }
}

