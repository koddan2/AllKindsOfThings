using DZT.Lib;
using System.CommandLine;

class UpdateAiPatrolsCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("update-ai-patrols");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        Option<string> sourceFile = new("--source", description: "The path to the input JSON file") { IsRequired = true };
        sourceFile.AddAlias("-s");
        cmd.Add(sourceFile);

        Option<string> destFile = new("--destination", description: "The path to the output JSON file") { IsRequired = true };
        sourceFile.AddAlias("-d");
        cmd.Add(destFile);

        cmd.SetHandler(
            (inputFilePath, outputFilePath, rootDir) =>
            {
                UpdateAiPatrols impl = new(rootDir, inputFilePath, outputFilePath);
                impl.Process();
            },
            sourceFile,
            destFile,
            optionRootDir);
    }
}