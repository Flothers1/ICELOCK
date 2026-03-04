using Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IceLockWorker.Services
{
    public class ProcessMonitoringService : BackgroundService
    {
        private readonly ILogger<ProcessMonitoringService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

        public ProcessMonitoringService(ILogger<ProcessMonitoringService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Process Monitoring Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await MonitorProcesses();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Process Monitoring Service is stopping");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in process monitoring");
                    // Wait longer on error to prevent rapid failure loops
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        private async Task MonitorProcesses()
        {
            try
            {
                // Monitor critical processes
                var criticalProcesses = new[]
                {
                    "i-FreezeWK", "i-FreezeAutoUSBS", "i-FreezeMP",
                    "i-FreezeNSPP", "i-FreezeFSS", "i-FreezeUSBScan",
                    "i-FreezeNSAD", "i-FreezeScanReg", "i-FreezeMPC", "i-FreezeMPM"
                };

                foreach (var processName in criticalProcesses)
                {
                    var processes = Process.GetProcessesByName(processName);
                    if (processes.Length == 0)
                    {
                        _logger.LogWarning($"Critical process {processName} is not running");
                        // Optionally restart the process here
                        await RestartProcess(processName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error monitoring processes");
            }
        }

        private async Task RestartProcess(string processName)
        {
            try
            {
                // Add logic to restart specific processes if needed
                var exePath = $@"C:\Users\Public\Ice Lock\{processName}.exe";
                //var exePath = Path.Combine(MainProjectPath.ProjectPath, $"{processName}.exe");

                if (File.Exists(exePath))
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = exePath,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(startInfo);
                    _logger.LogInformation($"Restarted process: {processName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to restart process: {processName}");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Process Monitoring Service is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}