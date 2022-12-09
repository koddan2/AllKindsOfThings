using SAK;
using System.Xml.Linq;

namespace DZT.Lib.Helpers;

public class MpMissionFiles
{
    private readonly string _rootDir;
    private readonly string _mpMissionName;

    public MpMissionFiles(string rootDir, string mpMissionName)
    {
        _rootDir = rootDir;
        _mpMissionName = mpMissionName;
    }

    public string PathToMpMissionDirectory => Path.Combine(_rootDir, DayzConstants.SubdirectoryNames.MpMissions, _mpMissionName);

    public XDocument CfgEconomyCoreXDocument
    {
        get
        {
            var cfgEconomyCoreXmlPath = Path.Combine(PathToMpMissionDirectory, DayzConstants.FileNames.CfgEconomyCore);
            return GetXDocumentCached(cfgEconomyCoreXmlPath);
        }
    }

    public IEnumerable<XElement> GetCentralEconomyElements()
    {
        var ceElements = CfgEconomyCoreXDocument
            .Root.OrFail()
            .Nodes()
            .OfType<XElement>()
            .Where(x => x.Name == "ce");
        return ceElements;
    }

    private Dictionary<string, XDocument> _xdocumentCache = new Dictionary<string, XDocument>();
    private XDocument GetXDocumentCached(string name)
    {
        if (_xdocumentCache.TryGetValue(name, out var result))
        {
            return result;
        }
        else
        {
            var xdoc = XDocument.Load(name);
            _xdocumentCache[name] = xdoc;
            return xdoc;
        }
    }
}

