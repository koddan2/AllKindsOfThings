using System.Xml.Linq;
using DZT.Lib.Helpers;

namespace DZT.Lib.Models;

/** EXAMPLE
  <type name="ACOGOptic_6x">
    <nominal>5</nominal>
    <lifetime>14400</lifetime>
    <restock>0</restock>
    <min>2</min>
    <quantmin>-1</quantmin>
    <quantmax>-1</quantmax>
    <cost>100</cost>
    <flags count_in_cargo="0" count_in_hoarder="0" count_in_map="1" count_in_player="0" crafted="0" deloot="0" />
    <category name="weapons" />
    <usage name="Military" />
    <usage name="Police" />
    <value name="Tier3" />
    <value name="Tier4" />
  </type>
*/
public record DzTypesXmlTypeElement(XElement Element)
{
    private IEnumerable<XElement>? _nodes = null;
    public IEnumerable<XElement> Nodes
    {
        get
        {
            _nodes ??= Element.Nodes().OfType<XElement>().ToArray();
            return _nodes;
        }
    }
    private XElement GetNode(string name, Func<XElement, bool>? pred = null) =>
        pred is null
        ? Nodes.First(x => x.Name == name)
        : Nodes.Where(pred).First(x => x.Name == name)
        ;

    public string Name => Element.Attribute("name")?.Value ?? throw new ApplicationException("Invalid <type>: missing attribute(name)");
    public int Nominal
    {
        get => GetNode("nominal").Value.ParseInt() ?? 0;
        set => GetNode("nominal").SetValue(value);
    }
    public int Lifetime
    {
        get => GetNode("lifetime").Value.ParseInt() ?? 0;
        set => GetNode("lifetime").SetValue(value);
    }
    public int Restock
    {
        get => GetNode("restock").Value.ParseInt() ?? 0;
        set => GetNode("restock").SetValue(value);
    }
    public int Min
    {
        get => GetNode("min").Value.ParseInt() ?? 0;
        set => GetNode("min").SetValue(value);
    }
    public int QuantMin
    {
        get => GetNode("quantmin").Value.ParseInt() ?? 0;
        set => GetNode("quantmin").SetValue(value);
    }
    public int QuantMax
    {
        get => GetNode("quantmax").Value.ParseInt() ?? 0;
        set => GetNode("quantmax").SetValue(value);
    }
    public int Cost
    {
        get => GetNode("cost").Value.ParseInt() ?? 0;
        set => GetNode("cost").SetValue(value);
    }
    public XElement Flags
    {
        get => GetNode("flags");
    }
    public string? Category
    {
        get => GetNode("category").Attribute("name")?.Value;
        set => GetNode("category").Attribute("name")?.SetValue(value ?? "");
    }
    public string[] Usages
    {
        get => Nodes
            .Where(x => x.Name == "usage")
            .Select(x => x.Attribute("name")?.Value)
            .OfType<string>()
            .ToArray();
        set
        {
            Nodes.Remove();
            _nodes = null;
            foreach (var usage in value)
            {
                var node = new XElement("usage", new XAttribute("name", usage));
                Element.Add(node);
            }
        }
    }
    public string[] Values
    {
        get => Nodes
            .Where(x => x.Name == "value")
            .Select(x => x.Attribute("name")?.Value)
            .OfType<string>()
            .ToArray();
        set
        {
            Nodes.Remove();
            _nodes = null;
            foreach (var usage in value)
            {
                var node = new XElement("value", new XAttribute("name", usage));
                Element.Add(node);
            }
        }
    }
}
