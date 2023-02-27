using DZT.Cli.Commands;
using SAK;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Diagnostics;

namespace DZT.Cli;

internal class Application
{
    internal static int Run(string[] args)
    {
        try
        {
            RootCommand rootCommand = new(description: "Operates on DayZ xml files.");
            Option<string> globalOptionDayZServerRootDir =
                new(
                    aliases: new[] { "-r", "--dayz-server-root-dir", },
                    "The root directory of the DayZServer instance"
                )
                {
                    IsRequired = true,
                };
            rootCommand.Add(globalOptionDayZServerRootDir);
            ManipulateTerritoryCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
            UpdateAiPatrolsCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
            AdjustTypesXmlCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
            FixSearchForLootCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
            GenerateSplattedLoadoutCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);
            CleanUpProfileCommand.AddToCommand(rootCommand, globalOptionDayZServerRootDir);

            Console.WriteLine(rootCommand.Here("Just some output"));

            ////return await rootCommand.InvokeAsync(args);
            return rootCommand.Invoke(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            var stack = e.Demystify();
            Console.WriteLine(stack);
            return -1;
        }
    }
}
