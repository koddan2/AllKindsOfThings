using DZT.Lib;
using DZT.Lib.Helpers;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Text.Json;

namespace DZT.Cli.Commands;
class AdjustTypesXmlCommand
{
    internal static void AddToCommand(Command addTo, Option<string> optionRootDir)
    {
        Command cmd = new("adjust-types");
        addTo.Add(cmd);

        cmd.Add(optionRootDir);

        Option<string> mpMissionName = new("--mp-mission-name", description: "The name of the MP mission") { IsRequired = true };
        mpMissionName.AddAlias("-m");
        cmd.Add(mpMissionName);

        Option<string> configurationFilePath = new("--configuration", description: "The path to the configuration file") { IsRequired = false };
        configurationFilePath.AddAlias("-c");
        cmd.Add(configurationFilePath);

        cmd.SetHandler(
            (rootDir, mpMissionName, cfgFile) =>
            {
                var logger = Globals.DztLoggerFactory.CreateLogger<AdjustTypesXml>();
                AdjustTypesXmlConfiguration cfg = new();
                if (cfgFile is not null)
                {
                    if (File.Exists(cfgFile))
                    {
                        var ext = Path.GetExtension(cfgFile);
                        logger.LogInformation("Configuration file supplied exists. File type is {ext}.", ext);
                        var cfgText = File.ReadAllText(cfgFile);
                        if (ext == ".toml")
                        {
                            var model = Tomlyn.Toml.ToModel(cfgText);
                            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
                            logger.LogInformation("Configuration file is TOML - normalized configuration: {cfg}", json);
                            cfg = Tomlet.TomletMain.To<AdjustTypesXmlConfiguration>(cfgText);
                            cfg = Tomlyn.Toml.ToModel<AdjustTypesXmlConfiguration>(cfgText);
                        }
                        else if (ext == ".json")
                        {
                            if (JsonSerializer.Deserialize<AdjustTypesXmlConfiguration>(cfgText) is AdjustTypesXmlConfiguration notNull)
                            {
                                cfg = notNull;
                            }
                        }
                        else
                        {
                            logger.LogError("Unknown file type - skipping configuration file");
                        }
                    }
                    else
                    {
                        logger.LogWarning("Configuration file was supplied, but the file ({}) does not exist", Path.GetFullPath(cfgFile));
                    }
                }
                AdjustTypesXml impl = new(logger, cfg, rootDir, mpMissionName);
                impl.Process();
            },
            optionRootDir,
            mpMissionName,
            configurationFilePath);
    }
}
