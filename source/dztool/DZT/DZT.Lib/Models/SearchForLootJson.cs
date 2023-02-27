namespace DZT.Lib.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class SflRoot
{
    public float Rarity { get; set; }
    public int InitialCooldown { get; set; }
    public int XPGain { get; set; }
    public int SoundEnabled { get; set; }
    public int DisableNotifications { get; set; }
    public string NotificationHeading { get; set; }
    public string NotificationText { get; set; }
    public string NotificationText2 { get; set; }
    public float MaxHealthCoef { get; set; }
    public Sflbuilding[] SFLBuildings { get; set; }
    public Sfllootcategory[] SFLLootCategory { get; set; }
}

public class Sflbuilding
{
    public string name { get; set; }
    public string[] buildings { get; set; }
}

public class Sfllootcategory
{
    public string name { get; set; }
    public float rarity { get; set; }
    public string[] loot { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
