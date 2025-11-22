using System.Runtime.InteropServices;
using DriverWeave.Core;

namespace DriverWeave.Install;

public class SystemRestore
{
    private readonly ILogger _logger;

    public SystemRestore(ILogger logger)
    {
        _logger = logger;
    }

    [DllImport("srclient.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SRSetRestorePoint(ref RESTOREPOINTINFO pRestorePtSpec, out STATEMGRSTATUS pSMgrStatus);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct RESTOREPOINTINFO
    {
        public int dwEventType;
        public int dwRestorePtType;
        public long llSequenceNumber;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szDescription;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct STATEMGRSTATUS
    {
        public int nStatus;
        public long llSequenceNumber;
    }

    private const int BEGIN_SYSTEM_CHANGE = 100;
    private const int END_SYSTEM_CHANGE = 101;
    private const int MODIFY_SETTINGS = 12;

    public bool CreateRestorePoint(string description)
    {
        if (!OperatingSystem.IsWindows()) return false;

        _logger.Log($"Creating System Restore Point: {description}");

        try
        {
            var rpi = new RESTOREPOINTINFO
            {
                dwEventType = BEGIN_SYSTEM_CHANGE,
                dwRestorePtType = MODIFY_SETTINGS,
                llSequenceNumber = 0,
                szDescription = description
            };

            var status = new STATEMGRSTATUS();

            if (!SRSetRestorePoint(ref rpi, out status))
            {
                _logger.LogError($"Failed to start restore point creation. Status: {status.nStatus}");
                return false;
            }

            // Immediately end the system change to finalize the restore point
            EndRestorePoint(status.llSequenceNumber);
            _logger.Log("Restore point created successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating restore point", ex);
            return false;
        }
    }

    private void EndRestorePoint(long sequenceNumber)
    {
        var rpi = new RESTOREPOINTINFO
        {
            dwEventType = END_SYSTEM_CHANGE,
            llSequenceNumber = sequenceNumber
        };
        var status = new STATEMGRSTATUS();
        SRSetRestorePoint(ref rpi, out status);
    }
}
