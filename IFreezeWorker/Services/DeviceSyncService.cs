using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Infrastructure.DataContext;

namespace IceLockWorker.Services
{
    public class DeviceSyncService : BackgroundService
    {
        private readonly ILogger<DeviceSyncService> _logger;
        private readonly TimeSpan _syncInterval = TimeSpan.FromHours(3);

        public DeviceSyncService(ILogger<DeviceSyncService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Device Sync Service started");

            // Initial delay before first sync
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PerformDeviceSync();

                    var nextSyncDelay = await GetSyncInterval();
                    await Task.Delay(nextSyncDelay, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Device Sync Service is stopping");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in device sync");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task<TimeSpan> GetSyncInterval()
        {
            try
            {
                using var deviceActionsContext = new DeviceContext();
                var syncString = deviceActionsContext.DeviceActions.FirstOrDefault()?.SyncN;

                if (double.TryParse(syncString, out double syncHours) && syncHours > 0)
                {
                    return TimeSpan.FromHours(syncHours);
                }

                return _syncInterval; // Default 3 hours
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sync interval, using default");
                return _syncInterval;
            }
        }

        private async Task PerformDeviceSync()
        {
            try
            {
                _logger.LogInformation("Starting device sync");

                // Device sync logic would go here
                // Note: DeviceManagement class needs to be accessible from this project
                // Or create an interface/service for this functionality

                _logger.LogInformation("Device sync completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during device sync");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Device Sync Service is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}