using DZT.Cli.Commands;
using SAK;
using Microsoft.Extensions.Logging;
using System.CommandLine;

RootCommand rootCommand = new(description: "Operates on DayZ xml files.");
Option<string> globalOptionDayZServerRootDir = new(
    aliases: new[]
    {
        "-r",
        "--dayz-server-root-dir",
    },
    "The root directory of the DayZServer instance")
    {
        IsRequired = true,
    };
rootCommand.Add(globalOptionDayZServerRootDir);
ManipulateTerritoryCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
UpdateAiPatrolsCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
AdjustTypesXmlCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
FixSearchForLootCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
GenerateSplattedLoadoutCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);

Console.WriteLine(rootCommand.Here("Just some output"));

////return await rootCommand.InvokeAsync(args);
return rootCommand.Invoke(args);
