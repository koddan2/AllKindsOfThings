﻿using DZT.Lib;
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
                FixSearchForLoot impl = new(rootDir);
                impl.Process();
            },
            optionRootDir);
    }
}
