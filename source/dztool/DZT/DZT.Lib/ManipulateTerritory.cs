using DZT.Lib.Helpers;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace DZT.Lib;
public class ManipulateTerritory
{
    private readonly ILogger<ManipulateTerritory> _logger;
    private readonly string _mpMissionName;
    private readonly string _entityName;
    private readonly string _rootDir;
    private readonly float? _multiplyByFactor;
    private readonly uint? _setMin;
    private readonly uint? _setMax;

    public ManipulateTerritory(
        ILogger<ManipulateTerritory> logger,
        string rootDir,
        string mpMissionName,
        string entityName,
        float? multiplyByFactor,
        uint? setMin,
        uint? setMax)
    {
        _logger = logger;
        _rootDir = rootDir;
        _mpMissionName = mpMissionName;
        _entityName = entityName;
        _multiplyByFactor = multiplyByFactor;
        _setMin = setMin;
        _setMax = setMax;
    }

    public void RestoreBackup()
    {
        var territoryFilePathRelative = Path.Combine("mpmissions", _mpMissionName, "env", $"{_entityName}_territories.xml");
        _logger.LogInformation("Restoring backup: {}", territoryFilePathRelative);
        try
        {
            FileManagement.TryRestoreFileV2(_rootDir, territoryFilePathRelative);
        }
        catch (Exception exn)
        {
            _logger.LogWarning("Unable to restore file: {}", exn.Message);
        }
    }

    public void Process()
    {
        var territoryFilePathRelative = Path.Combine("mpmissions", _mpMissionName, "env", $"{_entityName}_territories.xml");
        var territoryFilePath = Path.Combine(_rootDir, territoryFilePathRelative);
        var backupResult = FileManagement.BackupFileV2(_rootDir, territoryFilePathRelative);
        if (backupResult.FileOperationCommitted)
        {
            _logger.LogInformation("File backed up as {}", backupResult.BackupFilePath);
        }
        else
        {
            _logger.LogInformation("File already backed up {}", territoryFilePath);
        }

        var xd = XDocument.Load(territoryFilePath);
        var territories = xd.Root!.Nodes();
        foreach (XElement territory in territories.OfType<XElement>())
        {
            var zones = territory.Nodes();
            foreach (XElement zone in zones.OfType<XElement>())
            {
                var attrs = zone.Attributes();
                foreach (XAttribute attr in attrs)
                {
                    ProcessMinMax(attr);
                    ProcessMultiplyBy(attr);
                }
            }
        }

        using var fs = FileManagement.Utf8WithoutBomWriter(territoryFilePath);
        xd.Save(fs);
        _logger.LogInformation("File {} updated", territoryFilePath);
    }

    private void ProcessMinMax(XAttribute attr)
    {
        if (attr.Name == "dmin" && _setMin is uint setMin)
        {
            attr.Value = setMin.ToString(CultureInfo.InvariantCulture);
        }
        if (attr.Name == "dmax" && _setMax is uint setMax)
        {
            attr.Value = setMax.ToString(CultureInfo.InvariantCulture);
        }
    }

    private void ProcessMultiplyBy(XAttribute attr)
    {
        if (_multiplyByFactor is float factor)
        {
            if (attr.Name == "dmin" || attr.Name == "dmax")
            {
                var val = attr.Value;
                if (int.TryParse(val, out int intval))
                {
                    attr.Value = ((int)Math.Ceiling(intval * factor)).ToString(CultureInfo.InvariantCulture);
                }
            }
        }
    }
}
