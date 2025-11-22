using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DriverWeave.Core;
using DriverWeave.Discovery;
using DriverWeave.Update;
using DriverWeave.Install;

namespace DriverWeave.App;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ILogger _logger;
    
    public ObservableCollection<DriverInfo> Drivers { get; } = new();
    public ObservableCollection<string> Logs { get; } = new();

    private string _status = "Ready";
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    public RelayCommand ScanCommand { get; }
    public RelayCommand CheckUpdatesCommand { get; }

    public MainViewModel()
    {
        _logger = new AppLogger(this);
        ScanCommand = new RelayCommand(async _ => await ScanDrivers());
        CheckUpdatesCommand = new RelayCommand(async _ => await CheckUpdates());
    }

    private async Task ScanDrivers()
    {
        Status = "Scanning drivers...";
        Drivers.Clear();
        Logs.Clear();
        
        await Task.Run(() =>
        {
            var enumerator = new DriverEnumerator(_logger);
            var drivers = enumerator.GetDrivers();
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var d in drivers) Drivers.Add(d);
            });
        });

        Status = $"Found {Drivers.Count} drivers.";
    }

    private async Task CheckUpdates()
    {
        Status = "Checking for updates...";
        
        await Task.Run(() =>
        {
            var client = new WindowsUpdateClient(_logger);
            var updates = client.CheckForUpdates();
            
            if (updates.Count == 0)
            {
                _logger.Log("No updates found.");
            }
            else
            {
                foreach(var u in updates)
                {
                    _logger.Log($"Update available: {u.Title} ({u.SizeBytes / 1024 / 1024} MB)");
                }
            }
        });

        Status = "Update check complete.";
    }

    public void AddLog(string message)
    {
        Application.Current.Dispatcher.Invoke(() => Logs.Add(message));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class AppLogger : ILogger
{
    private readonly MainViewModel _vm;
    public AppLogger(MainViewModel vm) => _vm = vm;
    public void Log(string message) => _vm.AddLog($"[{DateTime.Now:HH:mm:ss} INFO] {message}");
    public void LogError(string message, Exception? ex = null) => _vm.AddLog($"[{DateTime.Now:HH:mm:ss} ERROR] {message} {ex?.Message}");
}
