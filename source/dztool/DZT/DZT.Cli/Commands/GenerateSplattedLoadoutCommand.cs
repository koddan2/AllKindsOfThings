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

        cmd.SetHandler(
            Handler,
            optionRootDir);
    }

    static void Handler(string rootDir)
    {
        GeneralSetup.Initialize(rootDir);
        GenerateSplattedLoadout impl = new(Globals.DztLoggerFactory.CreateLogger<GenerateSplattedLoadout>(), rootDir);
        impl.Process();
    }
}