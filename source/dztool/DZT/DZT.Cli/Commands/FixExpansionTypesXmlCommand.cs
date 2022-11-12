using DZT.Lib;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace DZT.Cli.Commands;
class FixExpansionTypesXmlCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("fix-expansion-types");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        Option<string> sourceFile = new("--file", description: "The path to the input file") { IsRequired = true };
        sourceFile.AddAlias("-f");
        cmd.Add(sourceFile);

        cmd.SetHandler(
            (inputFilePath, rootDir) =>
            {
                FixExpansionTypesXml impl = new(Globals.DztLoggerFactory.CreateLogger<FixExpansionTypesXml>(), rootDir, inputFilePath);
                impl.Process();
            },
            sourceFile,
            optionRootDir);
    }
}
