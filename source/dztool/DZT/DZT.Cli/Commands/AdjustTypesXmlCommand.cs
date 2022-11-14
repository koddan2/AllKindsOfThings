using DZT.Lib;
using DZT.Lib.Helpers;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace DZT.Cli.Commands;
class AdjustTypesXmlCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("adjust-types");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        Option<string> mpMissionName = new("--mp-mission-name", description: "The name of the MP mission") { IsRequired = true };
        mpMissionName.AddAlias("-m");
        cmd.Add(mpMissionName);

        cmd.SetHandler(
            (rootDir, mpMissionName) =>
            {
                AdjustTypesXml impl = new(Globals.DztLoggerFactory.CreateLogger<AdjustTypesXml>(), rootDir, mpMissionName);
                impl.Process();
            },
            optionRootDir,
            mpMissionName);
    }
}
