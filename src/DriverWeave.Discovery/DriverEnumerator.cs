using System.Management;
using DriverWeave.Core;
using System.Runtime.Versioning;

namespace DriverWeave.Discovery;

[SupportedOSPlatform("windows")]
public class DriverEnumerator
{
    private readonly ILogger _logger;

    public DriverEnumerator(ILogger logger)
    {
        _logger = logger;
    }

    public IEnumerable<DriverInfo> GetDrivers()
    {
        _logger.Log("Scanning for drivers using WMI...");
        var drivers = new List<DriverInfo>();

        try
        {
            // Win32_PnPSignedDriver is the standard WMI class for drivers
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPSignedDriver");
            foreach (var obj in searcher.Get())
            {
                try
                {
                    var driver = new DriverInfo
                    {
                        DeviceName = obj["DeviceName"]?.ToString() ?? "Unknown Device",
                        Provider = obj["Manufacturer"]?.ToString() ?? "Unknown",
                        Version = obj["DriverVersion"]?.ToString() ?? "0.0.0.0",
                        Date = ParseDate(obj["DriverDate"]?.ToString()),
                        InfPath = obj["InfName"]?.ToString() ?? string.Empty,
                        HardwareId = obj["HardWareID"]?.ToString() ?? string.Empty,
                        DeviceId = obj["DeviceID"]?.ToString() ?? string.Empty,
                        IsSigned = (bool)(obj["IsSigned"] ?? false),
                        Class = obj["DeviceClass"]?.ToString() ?? "Unknown"
                    };
                    drivers.Add(driver);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error parsing driver object", ex);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error querying WMI", ex);
        }

        _logger.Log($"Found {drivers.Count} drivers.");
        return drivers;
    }

    private DateTime ParseDate(string? wmiDate)
    {
        if (string.IsNullOrEmpty(wmiDate)) return DateTime.MinValue;
        try
        {
            return ManagementDateTimeConverter.ToDateTime(wmiDate);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }
}
