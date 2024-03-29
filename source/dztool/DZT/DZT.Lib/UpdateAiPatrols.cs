﻿using DZT.Lib.Helpers;
using DZT.Lib.Models;
using System.Linq;
using System.Text.Json;

namespace DZT.Lib;

public class UpdateAiPatrols
{
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
    private readonly ExtraPatrols _extraPatrols = new();

    public CategorizedDouble CategorizedDouble { get; set; } =
        new CategorizedDouble(
            /// high
            // minimal: 0.004,
            // small: 0.07,
            // medium: 0.45,
            // large: 0.7);
            /// lagom
            minimal: 0.005,
            small: 0.025,
            medium: 0.125,
            large: 0.400
        );

    double Chance(CategoryValue category, double modifier = 0d)
    {
        return CategorizedDouble.Get(category, modifier);
    }

    public UpdateAiPatrols(string rootDir, string mpMissionName)
    {
        Validators.ValidateDirExists(rootDir);
        var relativePathToFile = Path.Combine(
            "mpmissions",
            mpMissionName,
            @"expansion\settings\AIPatrolSettings.json"
        );
        _aiPatrolSettingsJsonFile = Path.Combine(rootDir, relativePathToFile);
        Validators.ValidateFileExists(_aiPatrolSettingsJsonFile);

        FileManagement.TryRestoreFileV2(rootDir, relativePathToFile);
        var result = FileManagement.BackupFileV2(rootDir, relativePathToFile);
        if (result.FileOperationCommitted)
        {
            Console.WriteLine("File backed up: {0}", result.BackupFilePath);
        }
        _json = File.ReadAllText(_aiPatrolSettingsJsonFile);
        _settings = JsonSerializer.Deserialize<AiPatrolSettingsRoot>(_json)!;

        var extraPatrolsFilePath = FileManagement.GetWorkspaceFilePath(
            rootDir,
            "chernarusplus.AIPatrolSettings.patch.Patrols.json"
        );
        if (File.Exists(extraPatrolsFilePath))
        {
            _extraPatrols =
                JsonSerializer.Deserialize<ExtraPatrols>(File.ReadAllText(extraPatrolsFilePath))
                ?? new ExtraPatrols();
        }
    }

    class ExtraPatrols
    {
        public Patrol[] Patrols { get; set; } = Array.Empty<Patrol>();
    }

    public void Process()
    {
        _settings.MVersion = 12;
        _settings.Enabled = 1;
        _settings.AccuracyMin = 0.50;
        _settings.AccuracyMax = 0.85;
        _settings.DespawnRadius = 900;
        _settings.DespawnTime = 20;
        _settings.RespawnTime = 700;
        _settings.MinDistRadius = 60;
        _settings.MaxDistRadius = 650;
        _settings.ThreatDistanceLimit = 120;
        _settings.DamageMultiplier = 1.0;

        AddObjectPatrol(
            "East",
            new[]
            {
                // police
                "Land_City_PoliceStation",
                "Land_Village_PoliceStation",
                // super markets
                "Land_City_Store",
                "Land_City_Store_WithStairs",
                // health care
                "Land_City_Hospital",
                "Land_Village_HealthCare",
                // construction
                "Land_Construction_Crane",
            },
            p =>
            {
                p.LoadoutFile = "SplattedLoadout";
                p.Chance = Chance(CategoryValue.Large, 0);
            }
        );
        AddObjectPatrol(
            "East",
            new[]
            {
                "Land_Village_store",
                "Land_Power_Station",
                "Land_Power_Transformer_Build",
                // rail
                "Land_Rail_Station_Big",
                "Land_Rail_Station_Small",
                "Land_Rail_Warehouse_Small"
            },
            p =>
            {
                p.NumberOfAI = -4;
                p.Faction = "Raiders";
                p.LoadoutFile = "SplattedLoadout";
                p.Chance = Chance(CategoryValue.Medium, 0);
            }
        );

        AddObjectPatrol(
            "East",
            new[] { "Land_Mil_Airfield_HQ", "Land_Mil_ATC_Small", "Land_Mil_ATC_Big" },
            p =>
            {
                p.Faction = "Raiders";
                p.LoadoutFile = "SplattedLoadout";
                p.Chance = Chance(CategoryValue.Medium, 0.2);
                p.NumberOfAI = -4;
            }
        );

        AddObjectPatrol(
            "East",
            new[] { "Land_City_FireStation" },
            p =>
            {
                p.Chance = Chance(CategoryValue.Medium, 0.2);
                p.Faction = "Raiders";
                p.LoadoutFile = "SplattedLoadout";
                p.NumberOfAI = -4;
            }
        );

        AddObjectPatrol(
            "East",
            new[] { "Land_City_Stand_Grocery", "Land_House_1B01_Pub", },
            p =>
            {
                p.Faction = "Raiders";
                p.LoadoutFile = "SplattedLoadout";
                p.NumberOfAI = -4;
                p.Chance = Chance(CategoryValue.Small, 0.1);
            }
        );

        var structureClassNames = DataHelper.GetStructureClassNames();
        var objectsForPatrolCity = new[] { structureClassNames["**Residential**"], }
            .SelectMany(x => x)
            .Where(x => !_forbiddenObjects.Contains(x))
            .ToArray();

        AddObjectPatrol(
            "East",
            objectsForPatrolCity,
            p =>
            {
                p.Faction = "Mercenaries";
                p.LoadoutFile = "SplattedLoadout";
                p.Chance = Chance(CategoryValue.Minimal);
                p.NumberOfAI = -2;
            }
        );

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
                p.Chance = Chance(CategoryValue.Small);
                p.NumberOfAI = -3;
            }
        );

        AddObjectPatrol(
            "East",
            new[]
            {
                "Land_Church1_Yellow",
                "Land_Chapel",
                "Land_Church2_2",
                "Land_Church3",
                "Land_Misc_FeedShack",
                "Land_Misc_DeerStand2",
                "Land_Misc_DeerStand1",
            },
            p =>
            {
                p.Faction = "Mercenaries";
                p.LoadoutFile = "SplattedLoadout";
                p.Chance = Chance(CategoryValue.Max);
                p.NumberOfAI = -2;
            }
        );

        AddObjectPatrol(
            "East",
            structureClassNames["**Military**"].ToArray(),
            p =>
            {
                p.Faction = "East";
                p.LoadoutFile = "SplattedLoadout";
                p.Chance = Chance(CategoryValue.Small);
                p.NumberOfAI = -3;
            }
        );

        AddObjectPatrol(
            "West",
            structureClassNames["**Military**"].ToArray(),
            p =>
            {
                p.Faction = "West";
                p.LoadoutFile = "SplattedLoadout";
                p.Chance = Chance(CategoryValue.Small);
                p.NumberOfAI = -3;
            }
        );

        _settings.ObjectPatrols
            .ToList()
            .ForEach(p =>
            {
                p.Faction = "Mercenaries";
                p.UnlimitedReload = 1;
                p.Behaviour = "ALTERNATE";
                p.Speed = "RANDOM";
                p.Formation = "RANDOM";
                p.FormationLooseness = 0.2d;
                p.LoadoutFile = "SplattedLoadout";
                p.ThreatDistanceLimit = -1;
                p.DamageMultiplier = -1;
                p.RespawnTime = -2;
                // object patrols must be more restricted in area where they spawn
                p.MinDistRadius = 50;
                p.MaxDistRadius = 350;
                p.DespawnRadius = 500;
            });

        _settings.Patrols.AddRange(
            _extraPatrols.Patrols.SideEffect(x =>
            {
                ////x.Chance = 0.25;
            })
        );

        _settings.Patrols
            .ToList()
            .ForEach(p =>
            {
                p.Chance = 1.0;
                p.NumberOfAI = -4;

                p.Speed = "RANDOM";
                p.Formation = "RANDOM";
                p.FormationLooseness = 0.2d;
                p.Faction = "Mercenaries";
                p.UnlimitedReload = 1;
                p.Behaviour = "ALTERNATE";
                p.LoadoutFile = "SplattedLoadout";
                p.ThreatDistanceLimit = -1;
                p.DamageMultiplier = -1;
                p.RespawnTime = -2;
            });

        using var fs = FileManagement.Utf8WithoutBomWriter(_aiPatrolSettingsJsonFile);
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true, };
        JsonSerializer.Serialize(fs.BaseStream, _settings, jsonOptions);
    }

    private ObjectPatrol GetObjectPatrol(string faction)
    {
        var settings = JsonSerializer.Deserialize<AiPatrolSettingsRoot>(_json)!;
        return settings.ObjectPatrols.First(x => x.Faction == faction);
    }

    private void AddObjectPatrol(
        string faction,
        string className,
        Action<ObjectPatrol>? mutate = null
    )
    {
        var extraPatrol = GetObjectPatrol(faction);
        extraPatrol.ClassName = className;
        mutate?.Invoke(extraPatrol);
        _settings.ObjectPatrols.Add(extraPatrol);
    }

    private void AddObjectPatrol(
        string faction,
        string[] classNames,
        Action<ObjectPatrol>? mutate = null
    )
    {
        foreach (var className in classNames)
        {
            AddObjectPatrol(faction, className, mutate);
        }
    }
}
