using DZT.Lib;
using System.CommandLine;
class UpdateAiPatrolsCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("update-ai-patrols");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        var sourceFile = cmd.AddOption<string>(
            name: "--source",
            description: "The path to the input JSON file",
            isRequired: true,
            aliases: new[] { "-s" });

        var destFile = cmd.AddOption<string>(
            name: "--destination",
            description: "The path to the output JSON file",
            isRequired: true,
            aliases: new[] { "-d" });

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