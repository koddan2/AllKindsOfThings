using DZT.Lib;
using System.CommandLine;

class AdjustZombieNumbersCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("adjust-zombie-numbers");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        var sourceFile = cmd.AddOption<string>(
            name: "--source",
            description: "The path to the input XML file",
            isRequired: true,
            aliases: new[] { "-s" });

        var destFile = cmd.AddOption<string>(
            name: "--destination",
            description: "The path to the output XML file",
            isRequired: true,
            aliases: new[] { "-d" });

        var factor = cmd.AddOption<float>(
            name: "--factor",
            description: "The factor to multiply with",
            isRequired: false,
            getDefaultValue: () => 2f,
            aliases: new[] { "-f" });

        cmd.SetHandler(
            Handler,
            sourceFile,
            destFile,
            optionRootDir,
            factor);
    }

    static void Handler(string inputFilePath, string outputFilePath, string rootDir, float factor)
    {
        AdjustZombieNumbers impl = new(rootDir, inputFilePath, outputFilePath, factor);
        impl.Process();
    }
}