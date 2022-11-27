#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Text.Json.Serialization;

public class Health
{
    [JsonPropertyName("Min")]
    public double Min { get; set; }

    [JsonPropertyName("Max")]
    public double Max { get; set; }

    [JsonPropertyName("Zone")]
    public string Zone { get; set; } = "";
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
    public double Chance { get; set; } = 1.0;

    [JsonPropertyName("Quantity")]
    public Quantity Quantity { get; set; } = new Quantity {Min=0, Max=1};

    [JsonPropertyName("Health")]
    public List<Health> Health { get; set; } = new List<Health>();

    [JsonPropertyName("InventoryAttachments")]
    public List<InventoryAttachment> InventoryAttachments { get; set; } = new List<InventoryAttachment>();

    [JsonPropertyName("InventoryCargo")]
    public List<InventoryCargoModel> InventoryCargo { get; set; } = new List<InventoryCargoModel>();

    [JsonPropertyName("ConstructionPartsBuilt")]
    public List<object> ConstructionPartsBuilt { get; set; } = new List<object>();

    [JsonPropertyName("Sets")]
    public List<LoadoutSet> Sets { get; set; } = new List<LoadoutSet>();
}

public class Item
{
    [JsonPropertyName("ClassName")]
    public string ClassName { get; set; }

    [JsonPropertyName("Chance")]
    public double Chance { get; set; } = 1.0;

    [JsonPropertyName("Quantity")]
    public Quantity Quantity { get; set; } = new Quantity { Min = 0, Max = 0 };

    [JsonPropertyName("Health")]
    public List<Health> Health { get; set; } = new List<Health>();

    [JsonPropertyName("InventoryAttachments")]
    public List<InventoryAttachment> InventoryAttachments { get; set; } = new List<InventoryAttachment>();

    [JsonPropertyName("InventoryCargo")]
    public List<InventoryCargoModel> InventoryCargo { get; set; } = new List<InventoryCargoModel>();

    [JsonPropertyName("ConstructionPartsBuilt")]
    public List<object> ConstructionPartsBuilt { get; set; } = new List<object>();

    [JsonPropertyName("Sets")]
    public List<LoadoutSet> Sets { get; set; } = new List<LoadoutSet>();
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
    public List<Health> Health { get; set; } = new List<Health>();

    [JsonPropertyName("InventoryAttachments")]
    public List<InventoryAttachment> InventoryAttachments { get; set; } = new List<InventoryAttachment>();

    [JsonPropertyName("InventoryCargo")]
    public List<InventoryCargoModel> InventoryCargo { get; set; } = new List<InventoryCargoModel>();

    [JsonPropertyName("ConstructionPartsBuilt")]
    public List<object> ConstructionPartsBuilt { get; set; } = new List<object>();

    [JsonPropertyName("Sets")]
    public List<LoadoutSet> Sets { get; set; } = new List<LoadoutSet>();
}

public class LoadoutSet
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
    public List<LoadoutSet> Sets { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
