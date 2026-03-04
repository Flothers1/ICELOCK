using i_freeze.Commands.MessageBoxCommands;
using i_freeze.DTOs;
using i_freeze.Utilities;
using i_freeze.View;
using Infrastructure;
using Infrastructure.DataContext;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public ICommand CheckUpdatesCommand { get; }
        public ICommand RequestExpertCommand { get; }
        public ICommand SyncCommand { get; }
        public ICommand MonitorPerformanceCommand { get; }
        public ICommand OpenLinkCommand { get; }

        // Reuse HttpClient instance for better performance
        private static readonly HttpClient _httpClient = new HttpClient();

        public SettingsViewModel()
        {
            CheckUpdatesCommand = new RelayCommand(CheckUpdates);
            RequestExpertCommand = new RelayCommand(RequestExpert);
            SyncCommand = new RelayCommand(Sync);
            MonitorPerformanceCommand = new RelayCommand(MonitorPerformance);
            OpenLinkCommand = new RelayCommand(OpenLink);
        }

        private void RequestExpert(object obj)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "https://ifreeze.flothers.com/expert/",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                new ShowMessage($"Failed to open the URL. Error: {ex.Message}");
            }
        }

        private async void Sync(object obj)
        {
            try
            {
                Process[] NetD = Process.GetProcessesByName("i-FreezeNetD");
                if (NetD.Length == 0)
                {
                    await Task.Run(() =>
                    {
                        var runner2 = new ExeRunner(@"i-FreezeNetD.exe");
                        runner2.RunToEnd2();
                    });
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "Settings class Sync method");
            }
        }

        private async void MonitorPerformance(object obj)
        {
            try
            {
                Process[] MP = Process.GetProcessesByName("i-FreezeMP");
                if (MP.Length == 0)
                {
                    await Task.Run(() =>
                    {
                        var runner2 = new ExeRunner("i-FreezeMP.exe");
                        runner2.runprocess();
                    });
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "Settings class MonitorPerformance method");
            }
        }

       
        private static readonly HttpClient httpClient = new HttpClient();

        private async void CheckUpdates(object obj)
        {
            ProgressBar win2 = new ProgressBar();
            win2.Show();

            try
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", await DeviceManagement.Token());

                HttpResponseMessage response = await httpClient.GetAsync(
                    $"{DeviceManagement.MainURL}Versions/GetLatestifreezeVersion?versionType=premium"
                );

                if (!response.IsSuccessStatusCode)
                {
                    new ShowMessage("Failed to check for updates.");
                    return;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);

                decimal latestVersion = doc.RootElement.GetProperty("version").GetDecimal();

                using var appConfigContext = new AppConfigAndLoginContext();
                var appConfig = appConfigContext.Application_Configuration.FirstOrDefault();

                if (appConfig == null)
                {
                    new ShowMessage("No configuration found in database.");
                    return;
                }

                //if (latestVersion > appConfig.VersionNumber)
                //{
                //    new ShowMessage($"VERSION {latestVersion} AVAILABLE");

                //    // check if updater already running
                //    Process[] existing = Process.GetProcessesByName("i-FreezeUpdate2");
                //    if (existing.Length == 0)
                //    {
                //        // run updater and WAIT until it finishes
                //        await Task.Run(() =>
                //        {
                //            var runnerFU = new ExeRunner("i-FreezeUpdate2.exe");
                //            runnerFU.runprocess();
                //        });
                //    }
                //    else
                //    {
                //        new ShowMessage("i-Freeze not updated .");
                //    }
                //}
                //else
                //{
                //    new ShowMessage("i-Freeze is up to date");
                //}
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.ToString(), "Settings class CheckUpdates method");
            }
            finally
            {
                win2.Close();
            }
        }


        private void OpenLink(object parameter)
        {
            if (parameter is string path)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
    }
}
