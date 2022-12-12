using DZT.Lib.Helpers;
using SAK;
using System.Xml.Linq;

namespace DZT.Lib;

public class SpawnableTypesHelper
{
    private readonly string _rootDir;
    private readonly string _mpMissionName;

    private Dictionary<string, XElement>? _spawnableTypes;

    public SpawnableTypesHelper(string rootDir, string mpMissionName)
    {
        _rootDir = rootDir;
        _mpMissionName = mpMissionName;
    }

    private Dictionary<string, XElement> SpawnableTypes => InitSpawnableTypes() ? _spawnableTypes.OrFail() : throw new ApplicationException("");
    private bool InitSpawnableTypes()
    {
        if (_spawnableTypes is not null) return true;

        _spawnableTypes = new Dictionary<string, XElement>();
        var sptfn = DayzFilesHelper.GetAllSpawnableTypesXmlFileNames(_rootDir.OrFail(), _mpMissionName.OrFail());
        foreach (var item in sptfn)
        {
            var xd = XDocument.Load(item);
            foreach (var xe in xd.Root.OrFail().Nodes().OfType<XElement>().Where(x => x.Name == "type"))
            {
                _spawnableTypes[xe.Attribute("name").OrFail().Value] = xe;
            }
        }

        return true;
    }

    private XElement? GetSpawnableType(string name)
    {
        if (SpawnableTypes.TryGetValue(name, out var result))
        {
            return result;
        }

        return null;
    }

    public IEnumerable<InventoryAttachment> GetAdditionalAttachments(string item)
    {
        if (item.StartsWith("Juggernaut"))
        {
            yield return new InventoryAttachment
            {
                SlotName = "",
                Items = new[] { "Juggernaut_Buttpack_Black", "Juggernaut_Pouches_Black", }.Select(className =>
                    new Item
                    {
                        Chance = 1,
                        ClassName = className,
                        ConstructionPartsBuilt = new List<object>(),
                        Health = new List<Health>
                        {
                                new Health{ Min = 0.5, Max = 0.9 },
                        },
                        InventoryAttachments = new List<InventoryAttachment>(),
                        InventoryCargo = new List<InventoryCargoModel>(),
                    }).ToList()
            };
        }
        else if (GetSpawnableType(item) is XElement xe)
        {
            // if (item == "MMG_JPC_Vest_black")
            // if (item == "SNAFU_AWM_Gun")
            // {
            //     var a = 1;
            // }
            foreach (var attch in xe.Nodes().OfType<XElement>().Where(x => x.Name == "attachments"))
            {
                yield return new InventoryAttachment
                {
                    SlotName = "",
                    Items = (
                        from itm
                        in attch.Nodes().OfType<XElement>().Where(x => x.Name == "item")
                        select new Item
                        {
                            // Chance = itm.Attribute("name")?.Value?.ToUpper()?.Contains("MAG") is true
                            //     ? 1
                            //     : (attch.Attribute("chance")?.Value.AsDouble() ?? 1d) * (itm.Attribute("chance")?.Value.AsDouble() ?? 1d),
                            // Chance = 1,
                            Chance = itm.Attribute("name")?.Value?.ToUpper()?.Contains("SUPP") is true
                                ? 0.02
                                : 1,
                            ClassName = itm.Attribute("name").OrFail().Value,
                            ConstructionPartsBuilt = new List<object>(),
                            Health = new List<Health>
                            {
                                new Health{ Min = 0.5, Max = 0.9 },
                            },
                            InventoryAttachments = new List<InventoryAttachment>(),
                            InventoryCargo = new List<InventoryCargoModel>(),
                        }).ToList()
                };
            }
        }
    }
}
