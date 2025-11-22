namespace DriverWeave.Core;

public class UpdateInfo
{
    public string Title { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public bool IsDownloaded { get; set; }
    public string Description { get; set; } = string.Empty;
    public string SupportUrl { get; set; } = string.Empty;
    
    // Reference to the installed driver this update applies to
    public string MatchedHardwareId { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Title} - {Version}";
    }
}
