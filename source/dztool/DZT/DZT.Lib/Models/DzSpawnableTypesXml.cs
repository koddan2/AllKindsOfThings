// using System.Xml.Serialization;
// XmlSerializer serializer = new XmlSerializer(typeof(Spawnabletypes));
// using (StringReader reader = new StringReader(xml))
// {
//    var test = (Spawnabletypes)serializer.Deserialize(reader);
// }

using System.Xml.Serialization;

namespace DZT.Lib.Models;

[XmlRoot(ElementName = "item")]
public class Item
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }

    [XmlAttribute(AttributeName = "chance")]
    public double Chance { get; set; }
}

[XmlRoot(ElementName = "attachments")]
public class Attachment
{
    [XmlElement(ElementName = "item")]
    public List<Item> Item { get; set; } = new List<Item>();

    [XmlAttribute(AttributeName = "chance")]
    public double Chance { get; set; }
}

[XmlRoot(ElementName = "type")]
public class Type
{
    [XmlElement(ElementName = "attachments")]
    public List<Attachment> Attachments { get; set; } = new List<Attachment>();

    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }
}

[XmlRoot(ElementName = "spawnabletypes")]
public class Spawnabletypes
{
    [XmlElement(ElementName = "type")]
    public List<Type> Type { get; set; } = new List<Type>();
}
