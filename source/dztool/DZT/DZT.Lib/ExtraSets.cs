using System.Reflection;
using System.Text.Json;

namespace DZT.Lib;

public class ExtraSets
{
    public static LoadoutSet Skorpion = JsonSerializer.Deserialize<LoadoutSet>(
        File.ReadAllText(DataPath("skorpion-set.json"))
    )!;
    public static LoadoutSet Mp5k = JsonSerializer.Deserialize<LoadoutSet>(
        File.ReadAllText(DataPath("mp5-set.json"))
    )!;
    public static LoadoutSet Repeater = JsonSerializer.Deserialize<LoadoutSet>(
        File.ReadAllText(DataPath("repeater-set.json"))
    )!;

    public static string EXE => Assembly.GetExecutingAssembly().Location;
    public static string CWD => Path.GetDirectoryName(EXE)!;

    public static string DataPath(string name) => Path.Combine(CWD, "DATA", name)!;
}
