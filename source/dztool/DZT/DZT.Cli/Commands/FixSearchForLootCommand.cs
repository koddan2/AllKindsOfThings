using DZT.Lib;
using DZT.Lib.Helpers;
using System.CommandLine;

class FixSearchForLootCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("update-searchforloot");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        Option<string> mpMissionName = new("--mp-mission-name", description: "The name of the MP mission") { IsRequired = true };
        mpMissionName.AddAlias("-m");
        cmd.Add(mpMissionName);

        Option<string> profileDirName = new("--profile-directory-name", description: "The name of the $profile directory") { IsRequired = true };
        profileDirName.AddAlias("-p");
        cmd.Add(profileDirName);

        cmd.SetHandler(
            (rootDir, mpMissionName, profileDirName) =>
            {
                FixSearchForLoot impl = new(rootDir, mpMissionName, profileDirName);
                impl.Process();
            },
            optionRootDir,
            mpMissionName,
            profileDirName);
    }
}
