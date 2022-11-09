using DZT.Lib;
using System.CommandLine;
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
                FixExpansionTypesXml impl = new(rootDir, inputFilePath);
                impl.Process();
            },
            sourceFile,
            optionRootDir);
    }
}
