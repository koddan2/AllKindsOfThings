using DZT.Lib;
using DZT.Lib.Helpers;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Linq;

namespace DZT.Cli.Commands;

internal class GenerateSplattedLoadoutCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("generate-splatted-ai-loadout");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        Option<string> mpMissionName = new("--mp-mission-name", description: "The name of the MP mission") { IsRequired = true };
        mpMissionName.AddAlias("-m");
        cmd.Add(mpMissionName);

        cmd.SetHandler(
            Handler,
            optionRootDir,
            mpMissionName);
    }

    static void Handler(string rootDir, string mpMissionName)
    {
        GenerateSplattedLoadout impl = new(Globals.DztLoggerFactory.CreateLogger<GenerateSplattedLoadout>(), rootDir, mpMissionName);
        impl.Process();
    }
}