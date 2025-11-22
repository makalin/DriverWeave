using DriverWeave.Core;
using System.Runtime.Versioning;

namespace DriverWeave.Update;

[SupportedOSPlatform("windows")]
public class WindowsUpdateClient
{
    private readonly ILogger _logger;

    public WindowsUpdateClient(ILogger logger)
    {
        _logger = logger;
    }

    public List<UpdateInfo> CheckForUpdates()
    {
        _logger.Log("Checking for updates via Windows Update API...");
        var updates = new List<UpdateInfo>();

        try
        {
            // Use dynamic COM interop to avoid adding a reference to WUApiLib.dll
            Type? sessionType = Type.GetTypeFromProgID("Microsoft.Update.Session");
            if (sessionType == null)
            {
                _logger.LogError("Windows Update Session COM object not found.");
                return updates;
            }

            dynamic session = Activator.CreateInstance(sessionType)!;
            dynamic searcher = session.CreateUpdateSearcher();
            searcher.Online = true; // Check online
            
            // Search for drivers that are not installed
            _logger.Log("Searching for available driver updates...");
            // Criteria: IsInstalled=0 and Type='Driver'
            dynamic searchResult = searcher.Search("IsInstalled=0 and Type='Driver'");

            _logger.Log($"Search completed. Found {searchResult.Updates.Count} potential updates.");

            foreach (dynamic update in searchResult.Updates)
            {
                try
                {
                    var info = new UpdateInfo
                    {
                        Title = update.Title,
                        Description = update.Description,
                        // MaxDownloadSize is decimal, cast to long
                        SizeBytes = (long)update.MaxDownloadSize,
                        SupportUrl = update.SupportUrl,
                        IsDownloaded = update.IsDownloaded
                    };
                    
                    // Try to extract version from title if possible, as WUApi doesn't always expose it directly on the top object
                    // Example title: "Intel - System - 10.1.1.42"
                    // We can leave Version empty for now or try to parse it.
                    
                    updates.Add(info);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error parsing update object", ex);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error querying Windows Update", ex);
        }

        return updates;
    }
}
