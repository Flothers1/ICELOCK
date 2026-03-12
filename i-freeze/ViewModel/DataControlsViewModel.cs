using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Services;
using i_freeze.Utilities;
using Infrastructure.DataContext;
using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class DataControlsViewModel : ViewModelBase, IDisposable
    {
        private bool _disposed = false;
        private DataClassificationConfigContext _context 
            = new DataClassificationConfigContext();
        public ICommand ScreenshotAnalyzerToggleCommand { get; set; }
        public ICommand RealtimeClassificationToggleCommand { get; set; }
        public ICommand AutomaticClassificationToggleCommand { get; set; }
        public ICommand DecryptAllDataToggleCommand { get; set; }
        private bool _isScreenshotAnalyzer;
        public bool IsScreenshotAnalyzer
        {
            get => _isScreenshotAnalyzer;
            set
            {
                if (_isScreenshotAnalyzer == value) return;
                _isScreenshotAnalyzer = value;
                OnPropertyChanged(nameof(IsScreenshotAnalyzer));
            }
        }

        private bool _isRealtimeClassification;
        public bool IsRealtimeClassification
        {
            get => _isRealtimeClassification;
            set
            {
                if (_isRealtimeClassification == value) return;
                _isRealtimeClassification = value;
                OnPropertyChanged(nameof(IsRealtimeClassification));
            }
        }

        private bool _isAutomaticClassification;
        public bool IsAutomaticClassification
        {
            get => _isAutomaticClassification;
            set
            {
                if (_isAutomaticClassification == value) return;
                _isAutomaticClassification = value;
                OnPropertyChanged(nameof(IsAutomaticClassification));
            }
        }

        private bool _isDecryptAllData;
        public bool IsDecryptAllData
        {
            get => _isDecryptAllData;
            set
            {
                if (_isDecryptAllData == value) return;
                _isDecryptAllData = value;
                OnPropertyChanged(nameof(IsDecryptAllData));
            }
        }
        public DataControlsViewModel()
        {
            ScreenshotAnalyzerToggleCommand = new RelayCommand(ScreenshotAnalyzerToggleClickAsync);
            RealtimeClassificationToggleCommand = new RelayCommand(RealtimeClassificationToggleClickAsync);
            AutomaticClassificationToggleCommand = new RelayCommand(AutomaticClassificationToggleClickAsync);
            DecryptAllDataToggleCommand = new RelayCommand(DecryptAllDataToggleClickAsync);
            _ = LoadSettingsAsync();
        }
        private async Task LoadSettingsAsync()
        {
            try
            {
                using (var db = new DataClassificationConfigContext())
                {
                    var settings = await db.DataClassificationSettings.FirstOrDefaultAsync();
                    if (settings == null)
                    {
                        settings = new DataClassificationSettings
                        {
                            AutomaticClassification = "false",
                            RealtimeClassification = "false",
                            ScreenshotAnalyzer = "false",
                            DecryptAllData = "false"
                        };
                        db.DataClassificationSettings.Add(settings);
                        await db.SaveChangesAsync();
                    }

                    IsAutomaticClassification = string.Equals(settings.AutomaticClassification, "true", StringComparison.OrdinalIgnoreCase);
                    IsRealtimeClassification = string.Equals(settings.RealtimeClassification, "true", StringComparison.OrdinalIgnoreCase);
                    IsScreenshotAnalyzer = string.Equals(settings.ScreenshotAnalyzer, "true", StringComparison.OrdinalIgnoreCase);
                    IsDecryptAllData = string.Equals(settings.DecryptAllData, "true", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "DataControlsViewModel.LoadSettingsAsync");
            }
        }
        private async void ScreenshotAnalyzerToggleClickAsync(object parameter)
        {
            try
            {
                await UpdateDataClassificationSettingsAsync(s => s.ScreenshotAnalyzer = IsScreenshotAnalyzer ? "true" : "false");

                if (IsScreenshotAnalyzer)
                {
                    var service = new ClassificationSettingsService();
                    // apply will start DLP_SA if enabled
                    using (var db = new DataClassificationConfigContext())
                    {
                        var settings = await db.DataClassificationSettings.FirstOrDefaultAsync();
                        await service.ApplyClassificationSettingsAsync(settings);
                    }
                }
                else
                {
                    try
                    {
                        var serviceControl = new WindowsServiceControl();
                        await serviceControl.BlockDLP_SAProcess();

                    }
                    catch (Exception ex)
                    {
                        await DeviceManagement.SendLogs(ex.Message, "BlockDLP_SAProcess");
                        new ShowMessage("Failed to Block the Process: " + ex.Message);
                    }
                    //// stop the screenshot analyzer process if running
                    //await KillProcessesSafelyAsync("DLP_SA");
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "DataControlsViewModel.ScreenshotAnalyzerToggleClickAsync");
            }
        }

        private async void RealtimeClassificationToggleClickAsync(object parameter)
        {
            try
            {
                await UpdateDataClassificationSettingsAsync(s => s.RealtimeClassification = IsRealtimeClassification ? "true" : "false");

                if (IsRealtimeClassification)
                {
                    var service = new ClassificationSettingsService();
                    using (var db = new DataClassificationConfigContext())
                    {
                        var settings = await db.DataClassificationSettings.FirstOrDefaultAsync();
                        await service.ApplyClassificationSettingsAsync(settings);
                    }
                }
                else
                {
                    try
                    {
                        var serviceControl = new WindowsServiceControl();
                        await serviceControl.BlockDLP_RCFProcess();

                    }
                    catch (Exception ex)
                    {
                        await DeviceManagement.SendLogs(ex.Message, "BlockDLP_RCFProcess");
                        new ShowMessage("Failed to Block the Process: " + ex.Message);
                    }
              //      await KillProcessesSafelyAsync("DLP_RCF");
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "DataControlsViewModel.RealtimeClassificationToggleClickAsync");
            }
        }

        private async void AutomaticClassificationToggleClickAsync(object parameter)
        {
            try
            {
                await UpdateDataClassificationSettingsAsync(s => s.AutomaticClassification = IsAutomaticClassification ? "true" : "false");

                if (IsAutomaticClassification)
                {
                    var service = new ClassificationSettingsService();
                    using (var db = new DataClassificationConfigContext())
                    {
                        var settings = await db.DataClassificationSettings.FirstOrDefaultAsync();
                        await service.ApplyClassificationSettingsAsync(settings);
                    }
                }
                else
                {
                    try
                    {
                        var serviceControl = new WindowsServiceControl();
                        await serviceControl.BlockDLP_CAFProcess();
                    }
                    catch (Exception ex)
                    {
                        await DeviceManagement.SendLogs(ex.Message, "BlockDLP_CAFProcess");
                        new ShowMessage("Failed to Block the Process: " + ex.Message);
                    }
                    //await KillProcessesSafelyAsync("DLP_CAF");
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "DataControlsViewModel.AutomaticClassificationToggleClickAsync");
            }
        }

        private async void DecryptAllDataToggleClickAsync(object parameter)
        {
            try
            {
                await UpdateDataClassificationSettingsAsync(s => s.DecryptAllData = IsDecryptAllData ? "true" : "false");

                    var serviceControl = new WindowsServiceControl();
                    if (IsDecryptAllData)
                    {
                        try
                        {
                            await serviceControl.RunDecryptAll();

                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Faild to run DecryptAll error:{ex}.");

                        }
                    }
                    else
                    {
                        try
                        {
                            await serviceControl.BlockDecrypt_AllProcess();

                        }
                        catch (Exception ex)
                        {

                        Debug.WriteLine($"Faild to Block DecryptAll error:{ex}.");

                        }
                    }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "DataControlsViewModel.DecryptAllDataToggleClickAsync");
            }
        }

        public async Task FetchAndApplySettingsFromServerAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

            try
            {
                var service = new ClassificationSettingsService();
                var settings = await service.FetchAndSaveClassificationSettingsAsync(email, cancellationToken);

                // apply the settings (this will start processes for enabled flags)
                await service.ApplyClassificationSettingsAsync(settings);

                // update view model properties to reflect newly fetched settings
                IsAutomaticClassification = string.Equals(settings.AutomaticClassification, "true", StringComparison.OrdinalIgnoreCase);
                IsRealtimeClassification = string.Equals(settings.RealtimeClassification, "true", StringComparison.OrdinalIgnoreCase);
                IsScreenshotAnalyzer = string.Equals(settings.ScreenshotAnalyzer, "true", StringComparison.OrdinalIgnoreCase);
                IsDecryptAllData = string.Equals(settings.DecryptAllData, "true", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "DataControlsViewModel.FetchAndApplySettingsFromServerAsync");
                throw;
            }
        }

        private async Task UpdateDataClassificationSettingsAsync(Action<DataClassificationSettings> updateAction)
        {
            try
            {
                using (var db = new DataClassificationConfigContext())
                {
                    var settings = await db.DataClassificationSettings.FirstOrDefaultAsync();
                    if (settings == null)
                    {
                        settings = new DataClassificationSettings();
                        db.DataClassificationSettings.Add(settings);
                    }

                    updateAction(settings);
                    db.DataClassificationSettings.Update(settings);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "DataControlsViewModel.UpdateDataClassificationSettingsAsync");
            }
        }

        #region Helper process methods
        private static bool IsProcessRunning(string processName)
        {
            try
            {
                return Process.GetProcessesByName(processName).Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private static async Task KillProcessesSafelyAsync(string processName)
        {
            await Task.Run(() =>
            {
                try
                {
                    var processes = Process.GetProcessesByName(processName);
                    foreach (var process in processes)
                    {
                        try
                        {
                            process.Kill();
                            process.WaitForExit(5000);
                        }
                        catch (Exception ex)
                        {
                            _ = DeviceManagement.SendLogs($"Failed to kill process {processName}: {ex.Message}", "DataControlsViewModel.KillProcessesSafelyAsync");
                        }
                        finally
                        {
                            process?.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = DeviceManagement.SendLogs(ex.Message, "DataControlsViewModel.KillProcessesSafelyAsync");
                }
            });
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            // nothing to dispose currently but keep pattern for future
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

