using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using SAK;

namespace DZT.Lib;

public static class MetaExtensions
{
    public static T Update<T>(this T obj, Action<T> updater)
    {
        updater(obj);
        return obj;
    }
}

public class ExtraSets
{
    public static void FixProps(LoadoutSet set)
    {
        set.Chance = 0.2;
    }

    public static LoadoutSet Skorpion = JsonSerializer
        .Deserialize<LoadoutSet>(File.ReadAllText(DataPath("skorpion-set.json")))!
        .Update(FixProps);
    public static LoadoutSet CzPistol = JsonSerializer
        .Deserialize<LoadoutSet>(File.ReadAllText(DataPath("czpistol-set.json")))!
        .Update(FixProps);
    public static LoadoutSet Mp5k = JsonSerializer
        .Deserialize<LoadoutSet>(File.ReadAllText(DataPath("mp5-set.json")))!
        .Update(FixProps);
    public static LoadoutSet UMP = JsonSerializer
        .Deserialize<LoadoutSet>(File.ReadAllText(DataPath("ump-set.json")))!
        .Update(FixProps);
    public static LoadoutSet Repeater = JsonSerializer
        .Deserialize<LoadoutSet>(File.ReadAllText(DataPath("repeater-set.json")))!
        .Update(FixProps);
    public static LoadoutSet Bk12 = JsonSerializer
        .Deserialize<LoadoutSet>(File.ReadAllText(DataPath("bk12-set.json")))!
        .Update(FixProps);

    public static LoadoutSet MP7 = MakeLoadoutSetFromProprietaryFormat("mp7-set.json");
    public static LoadoutSet MPX = MakeLoadoutSetFromProprietaryFormat("mpx-set.json");
    public static LoadoutSet TTCUZI = MakeLoadoutSetFromProprietaryFormat("ttcuzi-set.json");
    public static LoadoutSet SVT40 = MakeLoadoutSetFromProprietaryFormat("svt40-set.json");
    public static LoadoutSet AVS36 = MakeLoadoutSetFromProprietaryFormat("avs36-set.json");

    public static LoadoutSet PKP = MakeLoadoutSetFromProprietaryFormat("pkp-set.json");
    public static LoadoutSet SR25 = MakeLoadoutSetFromProprietaryFormat("sr25snafu-set.json");

    // public record ProprietaryFormat();
    private static LoadoutSet MakeLoadoutSetFromProprietaryFormat(string setFilePath)
    {
        var json = File.ReadAllText(DataPath(setFilePath));
        var jnode = JsonSerializer.Deserialize<JsonNode>(json)!;
        var weapon = jnode["weapon"]?["name"]?.GetValue<string>();
        var mag = jnode["mag"]?["name"]?.GetValue<string>();
        var ammo = jnode["ammo"]?[0]?.GetValue<string>();

        var chance = jnode["chance"]?.GetValue<double?>() ?? 0.2;

        return new LoadoutSet
        {
            Chance = chance,
            ClassName = "WEAPON",
            Health = new List<Health>
            {
                new Health { Min = 0.2, Max = 0.8 }
            },
            InventoryAttachments = new List<InventoryAttachment>
            {
                new InventoryAttachment
                {
                    SlotName = "Hands",
                    Items = new List<Item>
                    {
                        new Item
                        {
                            Chance = 1,
                            ClassName = weapon.OrFail(),
                            InventoryAttachments = new List<InventoryAttachment>
                            {
                                // TODO: add attachments
                                new InventoryAttachment
                                {
                                    SlotName = "",
                                    Items = mag is null
                                        ? new List<Item>()
                                        : new List<Item>
                                        {
                                            new Item { Chance = 1, ClassName = mag.OrFail() }
                                        }
                                }
                            }.Update(lst =>
                            {
                                var wepNode = jnode["weapon"]!;
                                var hasAtt = wepNode["attachments"] is not null;
                                if (!hasAtt)
                                    return;

                                var wepAttachments = jnode["weapon"]
                                    ?["attachments"]?.AsArray()
                                    .Select(attnode =>
                                    {
                                        return attnode?["name"]?.GetValue<string>();
                                    });
                                if (wepAttachments is not null)
                                {
                                    foreach (var att in wepAttachments)
                                    {
                                        if (att is null)
                                            continue;

                                        lst.Add(
                                            new InventoryAttachment
                                            {
                                                SlotName = "",
                                                Items = new List<Item>
                                                {
                                                    new Item { ClassName = att, }
                                                }
                                            }
                                        );
                                    }
                                }
                            })
                        }
                    }
                }
            },
            InventoryCargo = new List<InventoryCargoModel>
            {
                mag is null
                    ? new InventoryCargoModel { Chance = 1, ClassName = ammo.OrFail() }
                    : new InventoryCargoModel { Chance = 1, ClassName = mag.OrFail() },
                new InventoryCargoModel { Chance = 1, ClassName = ammo.OrFail() },
                new InventoryCargoModel { Chance = 0.5, ClassName = ammo.OrFail() }
            }
        };
    }

    public static string EXE => Assembly.GetExecutingAssembly().Location;
    public static string CWD => Path.GetDirectoryName(EXE)!;

    public static string DataPath(string name) => Path.Combine(CWD, "DATA", name)!;
}
