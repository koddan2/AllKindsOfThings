using DZT.Cli.Commands;
using Microsoft.Extensions.Logging;
using System.CommandLine;

RootCommand rootCommand = new(description: "Operates on DayZ xml files.");
Option<string> globalOptionDayZServerRootDir = new(name: "--dayz-server-root-dir", "The root directory of the DayZServer instance")
{
    IsRequired = true,
};
rootCommand.Add(globalOptionDayZServerRootDir);
ManipulateTerritoryCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
UpdateAiPatrolsCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
FixExpansionTypesXmlCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
FixSearchForLootCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
GenerateSplattedLoadoutCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);

////return await rootCommand.InvokeAsync(args);
return rootCommand.Invoke(args);
