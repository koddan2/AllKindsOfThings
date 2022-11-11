using DZT.Lib;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Linq;

namespace DZT.Cli.Commands;

internal class ManipulateTerritoryCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("manipulate-territory");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        var mpMissionName = cmd.AddOption<string>(
            name: "--mp-mission",
            description: "The name of the mpmission, e.g. 'dayzOffline.chernarusplus'",
            isRequired: true,
            aliases: new[] { "-m" });

        var entityName = cmd.AddOption<string>(
            name: "--entity",
            description: "The name of the entity to manipulate, e.g. 'zombie' or 'wolf'",
            isRequired: true,
            aliases: new[] { "-e" });

        var multiplyByFactor = cmd.AddOption<float?>(
            name: "--multiply-by",
            description: "The factor to multiply 'dmin' and 'dmax' by",
            isRequired: false);

        var setMinValue = cmd.AddOption<uint?>(
            name: "--set-dmin",
            description: "The value to set in 'dmin'. Must be 0 or greater.",
            isRequired: false);

        var setMaxValue = cmd.AddOption<uint?>(
            name: "--set-dmax",
            description: "The value to set in 'dmax'. Must be equal to, or greater than, [--set-dmin].",
            isRequired: false);

        setMaxValue.AddValidator((result) =>
        {
            var minVal = result.GetValueForOption(setMinValue);
            var maxVal = result.GetValueForOption(setMaxValue);
            if (maxVal < minVal)
            {
                result.ErrorMessage = "[--set-dmin] must be <= [--set-dmax]";
            }
        });

        var restoreBackup = cmd.AddOption<bool?>(
            name: "--restore",
            description: "Restores the file from the backup - if one exists.",
            isRequired: false);

        cmd.SetHandler(
            Handler,
            optionRootDir,
            mpMissionName,
            entityName,
            multiplyByFactor,
            setMinValue,
            setMaxValue,
            restoreBackup);
    }

    static void Handler(
        string rootDir,
        string mpMissionName,
        string entityName,
        float? multiplyByFactor,
        uint? setMin,
        uint? setMax,
        bool? restoreBackup)
    {
        GeneralSetup.Initialize(rootDir);
        ManipulateTerritory impl = new(Globals.DztLoggerFactory.CreateLogger<ManipulateTerritory>(), rootDir, mpMissionName, entityName, multiplyByFactor, setMin, setMax);
        if (restoreBackup is true)
        {
            impl.RestoreBackup();
        }
        else
        {
            impl.Process();
        }
    }
}