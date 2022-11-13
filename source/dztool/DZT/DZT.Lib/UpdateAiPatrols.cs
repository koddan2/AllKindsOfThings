using DZT.Lib.Helpers;
using DZT.Lib.Models;
using System.Linq;
using System.Text.Json;

namespace DZT.Lib;
public class UpdateAiPatrols
{
    private readonly Random _rng = new();
    private readonly string _inputFilePath;
    private readonly string _outputFilePath;
    private readonly string _json;
    private readonly AiPatrolSettingsRoot _settings;

    private readonly string[] _forbiddenObjects = new[]
    {
        "GardenPlot",
        "Polytunnel",
        "Land_Dam_Concrete_20_Floodgate",
        "Land_Train_Wagon_Tanker",
    };

    public double BaseExtraSpawnChance { get; set; } = 0.25;

    public UpdateAiPatrols(string rootDir, string inputFilePath, string outputFilePath)
    {
        Validators.ValidateDirExists(rootDir);
        _inputFilePath = Path.Combine(rootDir, inputFilePath);
        _outputFilePath = Path.Combine(rootDir, outputFilePath);
        Validators.ValidateFileExists(_inputFilePath);

        FileManagement.BackupFile(_inputFilePath, overwrite: true);
        _json = File.ReadAllText(_inputFilePath);
        _settings = JsonSerializer.Deserialize<AiPatrolSettingsRoot>(_json)!;
    }

    public void Process()
    {
        _settings.MVersion = 11;
        _settings.Enabled = 1;
        _settings.AccuracyMin = 0.42;
        _settings.AccuracyMax = 0.77;
        _settings.DespawnRadius = 1700;
        _settings.DespawnTime = 30;
        _settings.MinDistRadius = 200;
        _settings.MaxDistRadius = 1400;
        _settings.ThreatDistanceLimit = 400;
        _settings.DamageMultiplier = 1.0;

        AddObjectPatrol("East", new[] { "Land_City_PoliceStation", "Land_Village_PoliceStation" }, p =>
        {
            p.LoadoutFile = "PoliceLoadout";
            p.Chance = Math.Min(1.0, BaseExtraSpawnChance + 0.1);
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
            p.NumberOfAI = -7;
            p.Faction = "Raiders";
            p.LoadoutFile = "BanditLoadout";
            p.Chance = Math.Min(1.0, BaseExtraSpawnChance + 0.1);
        });

        AddObjectPatrol("East", new[] { "Land_Mil_Airfield_HQ", "Land_Mil_ATC_Small", "Land_Mil_ATC_Big" }, p =>
        {
            p.Faction = "Raiders";
            p.LoadoutFile = "GorkaLoadout";
            p.Chance = BaseExtraSpawnChance;
        });

        AddObjectPatrol("East", new[] { "Land_City_FireStation" }, p =>
        {
            p.Faction = "Raiders";
            p.LoadoutFile = "FireFighterLoadout";
            p.Chance = Math.Min(1.0, BaseExtraSpawnChance + 0.3);
        });

        AddObjectPatrol("East", new[] { "Land_City_Stand_Grocery", "Land_House_1B01_Pub" }, p =>
        {
            p.Faction = "Raiders";
            ////p.LoadoutFile = "SurvivorLoadout";
            p.LoadoutFile = "SplattedLoadout";
            p.Chance = Math.Max(BaseExtraSpawnChance - 0.15, 0.1);
        });

        var structureClassNames = DataHelper.GetStructureClassNames();
        var objectsForPatrol = new[]
            {
                structureClassNames["**Residential**"],
                structureClassNames["**Industrial**"],
                structureClassNames["**Specific**"],
                ////structureClassNames["**Wrecks**"],
                structureClassNames["**Military**"],
            }
            .SelectMany(x => x)
            .Where(x => !_forbiddenObjects.Contains(x))
            .ToArray();

        AddObjectPatrol(
            "East",
            objectsForPatrol,
            p =>
            {
                p.Faction = "Raiders";
                ////p.LoadoutFile = "SurvivorLoadout";
                p.LoadoutFile = "SplattedLoadout";
                p.Chance = 0.01;
                p.NumberOfAI = -3;
            });

        AddObjectPatrol("East", structureClassNames["**Military**"].ToArray(), p =>
        {
            p.Faction = "East";
            p.LoadoutFile = "EastLoadout";
            p.Chance = 0.005;
            p.NumberOfAI = 2;
        });

        AddObjectPatrol("West", structureClassNames["**Military**"].ToArray(), p =>
        {
            p.Faction = "West";
            p.LoadoutFile = "WestLoadout";
            p.Chance = 0.005;
            p.NumberOfAI = 2;
        });

        _settings.ObjectPatrols.ToList().ForEach(p =>
        {
            ////p.NumberOfAI = -5;
            p.UnlimitedReload = 1;
            ////if (p.LoadoutFile == "" && p.ClassName != "Wreck_UH1Y" && p.ClassName != "Wreck_Mi8_Crashed") // if (p.LoadoutFile == "")
            ////{
            ////    p.LoadoutFile = "PoliceLoadout";
            ////    p.LoadoutFile = GetRandomLoadout();
            ////}
            ////p.LoadoutFile = "SplattedLoadout";
            p.ThreatDistanceLimit = -1;
            p.DamageMultiplier = -1;
        });

        _settings.Patrols.ToList().ForEach(p =>
        {
            ////p.NumberOfAI = -5;
            p.UnlimitedReload = 1;
            ////if (p.LoadoutFile == "")
            ////{
            ////    p.LoadoutFile = "PoliceLoadout";
            ////    p.LoadoutFile = GetRandomLoadout();
            ////}
            p.ThreatDistanceLimit = -1;
            p.DamageMultiplier = -1;
        });

        using var fs = FileManagement.Utf8BomWriter(_outputFilePath);
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

