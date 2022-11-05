#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System.Text.Json.Serialization;

namespace DZT.Lib.Models;

public class ObjectPatrol
{
    [JsonPropertyName("Faction")]
    public string Faction { get; set; }

    [JsonPropertyName("Formation")]
    public string Formation { get; set; }

    [JsonPropertyName("LoadoutFile")]
    public string LoadoutFile { get; set; }

    [JsonPropertyName("NumberOfAI")]
    public int NumberOfAI { get; set; }

    [JsonPropertyName("Behaviour")]
    public string Behaviour { get; set; }

    [JsonPropertyName("Speed")]
    public string Speed { get; set; }

    [JsonPropertyName("UnderThreatSpeed")]
    public string UnderThreatSpeed { get; set; }

    [JsonPropertyName("CanBeLooted")]
    public int CanBeLooted { get; set; }

    [JsonPropertyName("UnlimitedReload")]
    public int UnlimitedReload { get; set; }

    [JsonPropertyName("AccuracyMin")]
    public double AccuracyMin { get; set; }

    [JsonPropertyName("AccuracyMax")]
    public double AccuracyMax { get; set; }

    [JsonPropertyName("MinDistRadius")]
    public double MinDistRadius { get; set; }

    [JsonPropertyName("MaxDistRadius")]
    public double MaxDistRadius { get; set; }

    [JsonPropertyName("DespawnRadius")]
    public double DespawnRadius { get; set; }

    [JsonPropertyName("MinSpreadRadius")]
    public double MinSpreadRadius { get; set; }

    [JsonPropertyName("MaxSpreadRadius")]
    public double MaxSpreadRadius { get; set; }

    [JsonPropertyName("Chance")]
    public double Chance { get; set; }

    [JsonPropertyName("WaypointInterpolation")]
    public string WaypointInterpolation { get; set; }

    [JsonPropertyName("DespawnTime")]
    public double DespawnTime { get; set; }

    [JsonPropertyName("RespawnTime")]
    public double RespawnTime { get; set; }

    [JsonPropertyName("ClassName")]
    public string ClassName { get; set; }
}

public class Patrol
{
    [JsonPropertyName("Faction")]
    public string Faction { get; set; }

    [JsonPropertyName("Formation")]
    public string Formation { get; set; }

    [JsonPropertyName("LoadoutFile")]
    public string LoadoutFile { get; set; }

    [JsonPropertyName("NumberOfAI")]
    public int NumberOfAI { get; set; }

    [JsonPropertyName("Behaviour")]
    public string Behaviour { get; set; }

    [JsonPropertyName("Speed")]
    public string Speed { get; set; }

    [JsonPropertyName("UnderThreatSpeed")]
    public string UnderThreatSpeed { get; set; }

    [JsonPropertyName("CanBeLooted")]
    public int CanBeLooted { get; set; }

    [JsonPropertyName("UnlimitedReload")]
    public int UnlimitedReload { get; set; }

    [JsonPropertyName("AccuracyMin")]
    public double AccuracyMin { get; set; }

    [JsonPropertyName("AccuracyMax")]
    public double AccuracyMax { get; set; }

    [JsonPropertyName("MinDistRadius")]
    public double MinDistRadius { get; set; }

    [JsonPropertyName("MaxDistRadius")]
    public double MaxDistRadius { get; set; }

    [JsonPropertyName("DespawnRadius")]
    public double DespawnRadius { get; set; }

    [JsonPropertyName("MinSpreadRadius")]
    public double MinSpreadRadius { get; set; }

    [JsonPropertyName("MaxSpreadRadius")]
    public double MaxSpreadRadius { get; set; }

    [JsonPropertyName("Chance")]
    public double Chance { get; set; }

    [JsonPropertyName("WaypointInterpolation")]
    public string WaypointInterpolation { get; set; }

    [JsonPropertyName("DespawnTime")]
    public double DespawnTime { get; set; }

    [JsonPropertyName("RespawnTime")]
    public double RespawnTime { get; set; }

    [JsonPropertyName("UseRandomWaypointAsStartPoint")]
    public int UseRandomWaypointAsStartPoint { get; set; }

    [JsonPropertyName("Waypoints")]
    public List<List<double>> Waypoints { get; set; }
}

public class AiPatrolSettingsRoot
{
    [JsonPropertyName("m_Version")]
    public int MVersion { get; set; }

    [JsonPropertyName("Enabled")]
    public int Enabled { get; set; }

    [JsonPropertyName("DespawnTime")]
    public double DespawnTime { get; set; }

    [JsonPropertyName("RespawnTime")]
    public double RespawnTime { get; set; }

    [JsonPropertyName("MinDistRadius")]
    public double MinDistRadius { get; set; }

    [JsonPropertyName("MaxDistRadius")]
    public double MaxDistRadius { get; set; }

    [JsonPropertyName("DespawnRadius")]
    public double DespawnRadius { get; set; }

    [JsonPropertyName("AccuracyMin")]
    public double AccuracyMin { get; set; }

    [JsonPropertyName("AccuracyMax")]
    public double AccuracyMax { get; set; }

    [JsonPropertyName("ObjectPatrols")]
    public List<ObjectPatrol> ObjectPatrols { get; set; }

    [JsonPropertyName("Patrols")]
    public List<Patrol> Patrols { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
