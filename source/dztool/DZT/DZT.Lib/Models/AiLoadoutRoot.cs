#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Text.Json.Serialization;

public class Health
{
    [JsonPropertyName("Min")]
    public double Min { get; set; }

    [JsonPropertyName("Max")]
    public double Max { get; set; }

    [JsonPropertyName("Zone")]
    public string Zone { get; set; }
}

public class InventoryAttachment
{
    [JsonPropertyName("SlotName")]
    public string SlotName { get; set; }

    [JsonPropertyName("Items")]
    public List<Item> Items { get; set; }
}

public class InventoryCargoModel
{
    [JsonPropertyName("ClassName")]
    public string ClassName { get; set; }

    [JsonPropertyName("Chance")]
    public double Chance { get; set; }

    [JsonPropertyName("Quantity")]
    public Quantity Quantity { get; set; }

    [JsonPropertyName("Health")]
    public List<Health> Health { get; set; }

    [JsonPropertyName("InventoryAttachments")]
    public List<InventoryAttachment> InventoryAttachments { get; set; }

    [JsonPropertyName("InventoryCargo")]
    public List<InventoryCargoModel> InventoryCargo { get; set; }

    [JsonPropertyName("ConstructionPartsBuilt")]
    public List<object> ConstructionPartsBuilt { get; set; }

    [JsonPropertyName("Sets")]
    public List<Set> Sets { get; set; }
}

public class Item
{
    [JsonPropertyName("ClassName")]
    public string ClassName { get; set; }

    [JsonPropertyName("Chance")]
    public double Chance { get; set; }

    [JsonPropertyName("Quantity")]
    public Quantity Quantity { get; set; }

    [JsonPropertyName("Health")]
    public List<Health> Health { get; set; }

    [JsonPropertyName("InventoryAttachments")]
    public List<InventoryAttachment> InventoryAttachments { get; set; }

    [JsonPropertyName("InventoryCargo")]
    public List<InventoryCargoModel> InventoryCargo { get; set; }

    [JsonPropertyName("ConstructionPartsBuilt")]
    public List<object> ConstructionPartsBuilt { get; set; }

    [JsonPropertyName("Sets")]
    public List<Set> Sets { get; set; }
}

public class Quantity
{
    [JsonPropertyName("Min")]
    public double Min { get; set; }

    [JsonPropertyName("Max")]
    public double Max { get; set; }
}

public class AiLoadoutRoot
{
    [JsonPropertyName("ClassName")]
    public string ClassName { get; set; }

    [JsonPropertyName("Chance")]
    public double Chance { get; set; }

    [JsonPropertyName("Quantity")]
    public Quantity Quantity { get; set; }

    [JsonPropertyName("Health")]
    public List<Health> Health { get; set; }

    [JsonPropertyName("InventoryAttachments")]
    public List<InventoryAttachment> InventoryAttachments { get; set; }

    [JsonPropertyName("InventoryCargo")]
    public List<InventoryCargoModel> InventoryCargo { get; set; }

    [JsonPropertyName("ConstructionPartsBuilt")]
    public List<object> ConstructionPartsBuilt { get; set; }

    [JsonPropertyName("Sets")]
    public List<Set> Sets { get; set; }
}

public class Set
{
    [JsonPropertyName("ClassName")]
    public string ClassName { get; set; }

    [JsonPropertyName("Chance")]
    public double Chance { get; set; }

    [JsonPropertyName("Quantity")]
    public Quantity Quantity { get; set; }

    [JsonPropertyName("Health")]
    public List<Health> Health { get; set; }

    [JsonPropertyName("InventoryAttachments")]
    public List<InventoryAttachment> InventoryAttachments { get; set; }

    [JsonPropertyName("InventoryCargo")]
    public List<InventoryCargoModel> InventoryCargo { get; set; }

    [JsonPropertyName("ConstructionPartsBuilt")]
    public List<object> ConstructionPartsBuilt { get; set; }

    [JsonPropertyName("Sets")]
    public List<Set> Sets { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
