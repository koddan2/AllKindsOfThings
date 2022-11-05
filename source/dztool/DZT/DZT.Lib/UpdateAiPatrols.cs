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

        _settings.AccuracyMin = 0.20;
        _settings.AccuracyMax = 0.72;
        _settings.DespawnRadius = 3000;
        _settings.DespawnTime = 40;
        _settings.MinDistRadius = 350;
        _settings.MaxDistRadius = 2200;

        AddObjectPatrol("East", "Land_Village_PoliceStation", p =>
        {
            p.LoadoutFile = "PoliceLoadout";
            p.Chance = 0.1;
        });
        AddObjectPatrol("East", new[] { "Land_City_Hospital", "Land_Village_HealthCare" }, p =>
        {
            p.Faction = "Raiders";
            p.LoadoutFile = "BanditLoadout";
            p.Chance = 0.1;
        });

        AddObjectPatrol("East", "Land_City_PoliceStation", p =>
        {
            p.LoadoutFile = "PoliceLoadout";
            p.Chance = 0.1;
        });
        AddObjectPatrol("East", new[] { "Land_City_Hospital", "Land_Village_HealthCare" }, p =>
        {
            p.Faction = "Raiders";
            p.LoadoutFile = "BanditLoadout";
            p.Chance = 0.1;
        });

        AddObjectPatrol("East", new[] { "Land_Mil_Airfield_HQ", "Land_Mil_ATC_Small", "Land_Mil_ATC_Big" }, p =>
        {
            p.Faction = "Raiders";
            p.LoadoutFile = "GorkaLoadout";
            p.Chance = 0.1;
        });

        AddObjectPatrol("East", new[] { "Land_City_FireStation" }, p =>
        {
            p.Faction = "Raiders";
            p.LoadoutFile = "FireFighterLoadout";
            p.Chance = 0.3;
        });

        _settings.ObjectPatrols.ToList().ForEach(p =>
        {
            p.NumberOfAI = -5;
            p.UnlimitedReload = 1;
            ////if (p.LoadoutFile == "" && p.ClassName != "Wreck_UH1Y" && p.ClassName != "Wreck_Mi8_Crashed")
            if (p.LoadoutFile == "")
            {
                //p.LoadoutFile = "PoliceLoadout";
                p.LoadoutFile = GetRandomLoadout();
            }
        });

        _settings.Patrols.ToList().ForEach(p =>
        {
            p.NumberOfAI = -5;
            p.UnlimitedReload = 1;
            if (p.LoadoutFile == "")
            {
                //p.LoadoutFile = "PoliceLoadout";
                p.LoadoutFile = GetRandomLoadout();
            }
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

