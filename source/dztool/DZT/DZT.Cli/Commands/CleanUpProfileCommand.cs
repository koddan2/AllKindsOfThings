using DZT.Lib;
using DZT.Lib.Helpers;
using System.CommandLine;

class CleanUpProfileCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("clean-up-profile");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        Option<string> profileDirName = new("--profile-directory-name", description: "The name of the $profile directory") { IsRequired = true };
        profileDirName.AddAlias("-p");
        cmd.Add(profileDirName);

        cmd.SetHandler(
            (rootDir, profileDirName) =>
            {
                CleanUpProfile impl = new(rootDir, profileDirName);
                impl.Process();
            },
            optionRootDir,
            profileDirName);
    }
}
