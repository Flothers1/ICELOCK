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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class SettingsAdminViewModel : ViewModelBase
    {
        public ICommand CheckUpdatesCommand { get; }
        public ICommand RequestExpertCommand { get; }
        public ICommand ReEnableUSBCommand { get; }
        public ICommand ReEnableTeatheringCommand { get; }
        public ICommand TamperProtectionToggleCommand { get; }


        public SettingsAdminViewModel()
        {
            CheckConfigAndProcess();

            CheckUpdatesCommand = new RelayCommand(CheckUpdates);
            RequestExpertCommand = new RelayCommand(RequestExpert);
         
        }

        private bool _isTamperProtection;
        public bool IsTamperProtection
        {
            get
            {
                return _isTamperProtection;
            }
            set
            {
                _isTamperProtection = value;
                OnPropertyChanged(nameof(IsTamperProtection));
            }
        }


        private void RequestExpert(object obj)
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://ifreeze.flothers.com/expert/",
                    UseShellExecute = true // Ensures the system opens it with the default browser
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                // Handle or log the exception if needed
                new ShowMessage($"Failed to open the URL. Error: {ex.Message}");
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
   

        private async void CheckConfigAndProcess()
        {
            try
            {
                DeviceContext policiesContext = new DeviceContext();
                string deviceAction = policiesContext.DeviceActions.FirstOrDefault().TamperProtection;

                bool isChecked;
                bool.TryParse(deviceAction, out isChecked);

                IsTamperProtection = isChecked;
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "SettingsAdmin class CheckConfigAndProcess");
            }
             

        }


    }
}
