using DZT.Lib;
using System.CommandLine;

class UpdateAiPatrolsCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("update-ai-patrols");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        var mpMissionName = cmd.AddOption<string>(
            name: "--mp-mission-name",
            description: "The name of the mission, e.g. dayzOffline.chernarusplus",
            isRequired: true,
            aliases: new[] { "-m" }
        );

        cmd.SetHandler(
            (rootDir, mpMissionName) =>
            {
                UpdateAiPatrols impl = new(rootDir, mpMissionName);
                impl.Process();
            },
            optionRootDir,
            mpMissionName
        );
    }
}
