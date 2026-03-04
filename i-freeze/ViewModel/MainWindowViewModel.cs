using i_freeze.Commands;
using i_freeze.Commands.MessageBoxCommands;
using i_freeze.Commands.UpgradeMessageBoxCommands;
using i_freeze.Utilities;
using IceLockWorker;
using Infrastructure;
using Infrastructure.DataContext;
using Infrastructure.Model;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        public ICommand CloseCommand { get; }
        public ICommand MinimizeCommand { get; }

        private readonly System.Timers.Timer _timer;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public MainWindowViewModel()
        {
            CloseCommand = new CloseWindowCommand();
            MinimizeCommand = new MinimizeCommand();
            _cancellationTokenSource = new CancellationTokenSource();

            CheckWindow();
        }

        void CheckWindow()
        {
            Process currentProcess = Process.GetCurrentProcess();
            var runningProcess = (from process in Process.GetProcesses()
                                  where
                                    process.Id != currentProcess.Id &&
                                    process.ProcessName.Equals(
                                      currentProcess.ProcessName,
                                      StringComparison.Ordinal)
                                  select process).FirstOrDefault();
            if (runningProcess != null)
            {
                return;
            }
            else
            {
                Activation();
            }
        }

        public async void MainWindow_Loaded()
        {
            // Use proper async task instead of Thread
             await RunFunctionAsync(_cancellationTokenSource.Token);
        }

     
        private async Task RunFunctionAsync(CancellationToken cancellationToken)
        {
            try
            {
               // await Task.Delay(TimeSpan.FromMinutes(2), cancellationToken);

                var deviceManagement = new DeviceManagement();
                await deviceManagement.PostData();

                while (!cancellationToken.IsCancellationRequested)
                {
                    TimeSpan delayBetweenRuns;

                    using (var deviceActionsContext = new DeviceContext())
                    {
                        var syncString = deviceActionsContext.DeviceActions.FirstOrDefault()?.SyncN;

                        if (double.TryParse(syncString, out double syncHours) && syncHours > 0)
                        {
                            delayBetweenRuns = TimeSpan.FromHours(syncHours);
                        }
                        else
                        {
                            delayBetweenRuns = TimeSpan.FromHours(12);
                        }
                    }

                    await Task.Delay(delayBetweenRuns, cancellationToken);

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        deviceManagement = new DeviceManagement();
                        await deviceManagement.PostData();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "MainWindow class RunFunctionAsync method");
            }
        }

        public async void Activation()
        {
            try
            {
                // Replace activation view with login view as the first page
                AppSettings.Instance.CurrentView = new LoginViewModel();
                AppSettings.Instance.SwitcherView = null;
            }
            catch (Exception ex)
            {
                // keep logging behavior you already use
                File.AppendAllText(@"C:\Users\Public\Ice Lock\Logs\i-freezeLogs.txt", ex.Message);
                // Optionally keep sending logs as before:
                _ = DeviceManagement.SendLogs(ex.Message, "MainWindow class Activation method");
            }
        }


        // Proper disposal
        public override void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
            _timer?.Dispose();
            base.Dispose();
        }
    }
}