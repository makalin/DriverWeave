using System.Diagnostics;
using DriverWeave.Core;

namespace DriverWeave.Install;

public class PnpUtilClient
{
    private readonly ILogger _logger;

    public PnpUtilClient(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> InstallDriverAsync(string infPath)
    {
        _logger.Log($"Installing driver: {infPath}");
        // /install flag installs the driver on matching devices
        var result = await RunPnpUtilAsync($"/add-driver \"{infPath}\" /install");
        return result.ExitCode == 0;
    }

    public async Task<bool> DeleteDriverAsync(string oemInfName, bool force = false)
    {
        _logger.Log($"Deleting driver: {oemInfName}");
        var args = $"/delete-driver {oemInfName}";
        if (force) args += " /force";
        var result = await RunPnpUtilAsync(args);
        return result.ExitCode == 0;
    }

    public async Task<bool> ExportDriverAsync(string oemInfName, string destinationDir)
    {
        _logger.Log($"Exporting driver {oemInfName} to {destinationDir}");
        Directory.CreateDirectory(destinationDir);
        var result = await RunPnpUtilAsync($"/export-driver {oemInfName} \"{destinationDir}\"");
        return result.ExitCode == 0;
    }

    private async Task<(int ExitCode, string Output)> RunPnpUtilAsync(string args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "pnputil.exe",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
            // Note: pnputil requires admin rights. The app itself should be run as admin.
        };

        try
        {
            using var process = Process.Start(psi);
            if (process == null) return (-1, "Failed to start pnputil");

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (!string.IsNullOrWhiteSpace(output)) _logger.Log(output);
            if (!string.IsNullOrWhiteSpace(error)) _logger.LogError(error);

            return (process.ExitCode, output);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error running pnputil", ex);
            return (-1, ex.Message);
        }
    }
}
