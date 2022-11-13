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

        cmd.SetHandler(
            (rootDir) =>
            {
                GeneralSetup.Initialize(rootDir);
                FixSearchForLoot impl = new(rootDir);
                impl.Process();
            },
            optionRootDir);
    }
}
