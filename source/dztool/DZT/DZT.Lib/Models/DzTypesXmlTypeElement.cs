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
    public static DzTypesXmlTypeElement FromElement(XElement element) => new(element);
    public static IEnumerable<DzTypesXmlTypeElement> FromDocument(XDocument doc)
    {
        var types = doc.Root!.Nodes();
        var result = types.OfType<XElement>().Select(x => new DzTypesXmlTypeElement(x));
        return result;
    }

    public override string ToString()
    {
        return $"<{Name}>";
    }

    private IEnumerable<XElement>? _nodes = null;
    public IEnumerable<XElement> Nodes
    {
        get
        {
            _nodes ??= Element.Nodes().OfType<XElement>().ToArray();
            return _nodes;
        }
    }
    private XElement? GetNode(string name, Func<XElement, bool>? pred = null) =>
        pred is null
        ? Nodes.FirstOrDefault(x => x.Name == name)
        : Nodes.Where(pred).FirstOrDefault(x => x.Name == name)
        ;

    public string Name => Element.Attribute("name")?.Value ?? throw new ApplicationException("Invalid <type>: missing attribute(name)");
    public int Nominal
    {
        get => GetNode("nominal")?.Value.ParseInt() ?? 0;
        set => GetNode("nominal")?.SetValue(value);
    }
    public int Lifetime
    {
        get => GetNode("lifetime")?.Value.ParseInt() ?? 0;
        set => GetNode("lifetime")?.SetValue(value);
    }
    public int Restock
    {
        get => GetNode("restock")?.Value.ParseInt() ?? 0;
        set => GetNode("restock")?.SetValue(value);
    }
    public int Min
    {
        get => GetNode("min")?.Value.ParseInt() ?? 0;
        set => GetNode("min")?.SetValue(value);
    }
    public int QuantMin
    {
        get => GetNode("quantmin")?.Value.ParseInt() ?? 0;
        set => GetNode("quantmin")?.SetValue(value);
    }
    public int QuantMax
    {
        get => GetNode("quantmax")?.Value.ParseInt() ?? 0;
        set => GetNode("quantmax")?.SetValue(value);
    }
    public int Cost
    {
        get => GetNode("cost")?.Value.ParseInt() ?? 0;
        set => GetNode("cost")?.SetValue(value);
    }
    public IDictionary<string, string>? Flags
    {
        get => Nodes.FirstOrDefault(x => x.Name == "flags")?.Attributes()
            .ToDictionary(x => x.Name.ToString(), x => x.Value);
        set
        {
            Nodes.FirstOrDefault(x => x.Name == "flags")?.Remove();
            var attributes = value?.Select(kvp => new XAttribute(kvp.Key, kvp.Value));
            var newEl = new XElement("flags", attributes);
            Element.Add(newEl);
        }
    }
    public string? Category
    {
        get => GetNode("category")?.Attribute("name")?.Value;
        set
        {
            if (value is null)
            {
                if (GetNode("category") is XElement categoryEl)
                {
                    categoryEl.Remove();
                }
            }
            else if (GetNode("category") is null)
            {
                Element.Add(new XElement("category", new XAttribute("name", value)));
            }
            else
            {
                GetNode("category")?.Attribute("name")?.SetValue(value);
            }
        }
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
            Nodes
                .Where(x => x.Name == "usage")
                .Remove();
            _nodes = null;
            foreach (var usageName in value)
            {
                var node = new XElement("usage", new XAttribute("name", usageName));
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
            Nodes
                .Where(x => x.Name == "value")
                .Remove();
            _nodes = null;
            foreach (var valueName in value)
            {
                var node = new XElement("value", new XAttribute("name", valueName));
                Element.Add(node);
            }
            var b = Element;
        }
    }
    public string[] Tags
    {
        get => Nodes
            .Where(x => x.Name == "tag")
            .Select(x => x.Attribute("name")?.Value)
            .OfType<string>()
            .ToArray();
        set
        {
            Nodes
                .Where(x => x.Name == "tag")
                .Remove();
            _nodes = null;
            foreach (var tagName in value)
            {
                var node = new XElement("tag", new XAttribute("name", tagName));
                Element.Add(node);
            }
            var b = Element;
        }
    }
}
