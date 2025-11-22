namespace DriverWeave.Core;

public class DriverInfo
{
    public string DeviceName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string InfPath { get; set; } = string.Empty;
    public string HardwareId { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public bool IsSigned { get; set; }

    public override string ToString()
    {
        return $"{DeviceName} ({Version}) - {Provider}";
    }
}
