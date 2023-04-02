using System.Reflection;
using System.Text.Json;

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

    public static string EXE => Assembly.GetExecutingAssembly().Location;
    public static string CWD => Path.GetDirectoryName(EXE)!;

    public static string DataPath(string name) => Path.Combine(CWD, "DATA", name)!;
}
