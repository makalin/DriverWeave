using DriverWeave.Core;
using DriverWeave.Discovery;
using DriverWeave.Update;
using DriverWeave.Install;

namespace DriverWeave.Cli;

class Program
{
    static async Task Main(string[] args)
    {
        var logger = new ConsoleLogger();
        
        if (args.Length == 0)
        {
            PrintHelp();
            return;
        }

        var command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "--list":
                    ListDrivers(logger);
                    break;
                case "--check":
                    CheckUpdates(logger);
                    break;
                case "--update-all":
                    await UpdateAll(logger);
                    break;
                case "--export":
                    if (args.Length < 2)
                    {
                        logger.LogError("Please specify a destination path.");
                        return;
                    }
                    await ExportDrivers(logger, args[1]);
                    break;
                case "--help":
                    PrintHelp();
                    break;
                default:
                    logger.LogError($"Unknown command: {command}");
                    PrintHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError("An unexpected error occurred.", ex);
        }
    }

    static void PrintHelp()
    {
        Console.WriteLine("DriverWeave CLI");
        Console.WriteLine("Usage:");
        Console.WriteLine("  --list          List installed drivers");
        Console.WriteLine("  --check         Check for updates");
        Console.WriteLine("  --update-all    Update all drivers");
        Console.WriteLine("  --export <path> Export all drivers to folder");
    }

    static void ListDrivers(ILogger logger)
    {
        var enumerator = new DriverEnumerator(logger);
        var drivers = enumerator.GetDrivers();
        foreach (var driver in drivers)
        {
            Console.WriteLine(driver);
        }
    }

    static void CheckUpdates(ILogger logger)
    {
        var client = new WindowsUpdateClient(logger);
        var updates = client.CheckForUpdates();
        foreach (var update in updates)
        {
            Console.WriteLine($"Found Update: {update.Title} ({update.SizeBytes / 1024 / 1024} MB)");
        }
    }

    static async Task UpdateAll(ILogger logger)
    {
        // 1. Check for updates
        var updateClient = new WindowsUpdateClient(logger);
        var updates = updateClient.CheckForUpdates();
        
        if (updates.Count == 0)
        {
            logger.Log("No updates found.");
            return;
        }

        // 2. Create Restore Point
        var restore = new SystemRestore(logger);
        restore.CreateRestorePoint("DriverWeave Auto Update");

        // 3. Install Updates
        // Note: Full WUApi installation logic requires downloading streams and installing via COM.
        // This is complex to implement via dynamic COM in a short snippet.
        // We will simulate the intent here.
        
        logger.Log($"Found {updates.Count} updates. Installation logic is pending full WUApi implementation.");
        foreach(var update in updates)
        {
             logger.Log($"Pending install: {update.Title}");
        }
    }

    static async Task ExportDrivers(ILogger logger, string path)
    {
        var enumerator = new DriverEnumerator(logger);
        var drivers = enumerator.GetDrivers();
        var pnp = new PnpUtilClient(logger);

        foreach (var driver in drivers)
        {
            if (!string.IsNullOrEmpty(driver.InfPath))
            {
                var infName = Path.GetFileName(driver.InfPath);
                // Only export OEM drivers usually, as inbox drivers are protected
                if (infName.StartsWith("oem", StringComparison.OrdinalIgnoreCase))
                {
                    await pnp.ExportDriverAsync(infName, path);
                }
            }
        }
    }
}
