using DZT.Lib;
using System.CommandLine;

class AdjustZombieNumbersCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("adjust-zombie-numbers");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        Option<string> sourceFile = new("--source", description: "The path to the input XML file") { IsRequired = true };
        sourceFile.AddAlias("-s");
        cmd.Add(sourceFile);

        Option<string> destFile = new("--destination", description: "The path to the output XML file") { IsRequired = true };
        sourceFile.AddAlias("-d");
        cmd.Add(destFile);

        cmd.SetHandler(
            (inputFilePath, outputFilePath, rootDir) =>
            {
                AdjustZombieNumbers impl = new(rootDir, inputFilePath, outputFilePath);
                impl.Process();
            },
            sourceFile,
            destFile,
            optionRootDir);
    }
}