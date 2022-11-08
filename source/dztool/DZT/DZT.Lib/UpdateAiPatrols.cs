using DZT.Lib.Models;
using System.Text.Json;

namespace DZT.Lib;
public class UpdateAiPatrols
{
    private readonly Random _rng = new Random();
    private readonly string _inputFilePath;
    private readonly string _outputFilePath;
    private readonly string _json;
    private readonly AiPatrolSettingsRoot _settings;
    private readonly string _rootDir;

    public double BaseExtraSpawnChance { get; set; } = 0.5;

    public UpdateAiPatrols(string rootDir, string inputFilePath, string outputFilePath)
    {
        Validators.ValidateDirExists(rootDir);
        _inputFilePath = Path.Combine(rootDir, inputFilePath);
        _outputFilePath = Path.Combine(rootDir, outputFilePath);
        Validators.ValidateFileExists(_inputFilePath);

        FileManagement.BackupFile(_inputFilePath, overwrite: true);
        _json = File.ReadAllText(_inputFilePath);
        _settings = JsonSerializer.Deserialize<AiPatrolSettingsRoot>(_json)!;
        _rootDir = rootDir;
    }

    public void Process()
    {
        ////AddObjectPatrol("East", "StaticObj_Wreck_BRDM_DE");
        ////AddObjectPatrol("East", "Land_wreck_truck01_aban1_green_DE");
        ////AddObjectPatrol("East", "Land_Wreck_offroad02_aban2");
        ////AddObjectPatrol("East", "StaticObj_Wreck_Decal_Small2", p =>
        ////{
        ////    p.Faction = "Raiders";
        ////});
        ////AddObjectPatrol("East", "Land_Guardhouse", p =>
        ////{
        ////    p.Faction = "Raiders";
        ////});

        _settings.AccuracyMin = 0.27;
        _settings.AccuracyMax = 0.72;
        _settings.DespawnRadius = 2500;
        _settings.DespawnTime = 120;
        _settings.MinDistRadius = 350;
        _settings.MaxDistRadius = 1700;

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
            p.Faction = "East";
            p.LoadoutFile = "SurvivorLoadout";
            p.Chance = Math.Max(BaseExtraSpawnChance - 0.15, 0.1);
        });

        AddObjectPatrol("East", new[]
        {
            "Land_Mil_Tent_Big1_1",
            "Land_Mil_Tent_Big1_2",
            "Land_Mil_Tent_Big1_3",
            "Land_Mil_Tent_Big1_4",
            "Land_Mil_Tent_Big1_5",
            "Land_Mil_Tent_Big2_1",
            "Land_Mil_Tent_Big2_2",
            "Land_Mil_Tent_Big2_3",
            "Land_Mil_Tent_Big2_4",
            "Land_Mil_Tent_Big2_5",
            "Land_Mil_Tent_Big3",
            "Land_Mil_Tent_Big4",
        }, p =>
        {
            p.Faction = "East";
            p.LoadoutFile = "EastLoadout";
            p.Chance = 0.01;
        });

        AddObjectPatrol("West", new[]
        {
            "Land_Mil_Tent_Big1_1",
            "Land_Mil_Tent_Big1_2",
            "Land_Mil_Tent_Big1_3",
            "Land_Mil_Tent_Big1_4",
            "Land_Mil_Tent_Big1_5",
            "Land_Mil_Tent_Big2_1",
            "Land_Mil_Tent_Big2_2",
            "Land_Mil_Tent_Big2_3",
            "Land_Mil_Tent_Big2_4",
            "Land_Mil_Tent_Big2_5",
            "Land_Mil_Tent_Big3",
            "Land_Mil_Tent_Big4",
        }, p =>
        {
            p.Faction = "West";
            p.LoadoutFile = "WestLoadout";
            p.Chance = 0.01;
        });

        _settings.ObjectPatrols.ToList().ForEach(p =>
        {
            p.NumberOfAI = -5;
            p.UnlimitedReload = 1;
            ////if (p.LoadoutFile == "" && p.ClassName != "Wreck_UH1Y" && p.ClassName != "Wreck_Mi8_Crashed") // if (p.LoadoutFile == "")
            ////{
            ////    p.LoadoutFile = "PoliceLoadout";
            ////    p.LoadoutFile = GetRandomLoadout();
            ////}
        });

        _settings.Patrols.ToList().ForEach(p =>
        {
            p.NumberOfAI = -5;
            p.UnlimitedReload = 1;
            ////if (p.LoadoutFile == "")
            ////{
            ////    p.LoadoutFile = "PoliceLoadout";
            ////    p.LoadoutFile = GetRandomLoadout();
            ////}
        });

        using var fs = FileManagement.Writer(_outputFilePath);
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
        AddObjectPatrol(faction, new string[] { className }, mutate);
    }

    private void AddObjectPatrol(string faction, string[] classNames, Action<ObjectPatrol>? mutate = null)
    {
        foreach (var className in classNames)
        {
            var extraPatrol = GetObjectPatrol(faction);
            extraPatrol.ClassName = className;
            mutate?.Invoke(extraPatrol);
            _settings.ObjectPatrols.Add(extraPatrol);
        }
    }

    private string GetRandomLoadout()
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

