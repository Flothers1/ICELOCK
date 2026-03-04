using i_freeze.Commands.MessageBoxCommands;
using i_freeze.DTOs;
using i_freeze.Utilities;
using i_freeze.View;
using Infrastructure;
using Infrastructure.DataContext;
using Infrastructure.Model;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace i_freeze.ViewModel
{
    public class ActivationViewModel : ViewModelBase, IDisposable
    {
        private const string BaseApiUrl = "http://184.174.37.115:44500/api/Licenses/";

        // Remove class-level contexts - CRITICAL MEMORY LEAK FIX
        // private AppConfigAndLoginContext context = new AppConfigAndLoginContext(); // DELETED
        // private AppConfigPYContext contextPY = new AppConfigPYContext(); // DELETED

        // Reuse HttpClient for better performance
        private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });

        private bool _disposed = false;

        static ActivationViewModel()
        {
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        private string _liscence;
        public string Liscence
        {
            get { return _liscence; }
            set
            {
                _liscence = value;
                OnPropertyChanged();
            }
        }

        public ICommand ActivateCommand { get; }

        public ActivationViewModel()
        {
         //   ActivateCommand = new RelayCommand(Activate);
        }



        //private async void Activate(object obj)
        //{
        //    try
        //    {
        //        if (Guid.TryParse(Liscence, out var code))
        //        {
        //            var deviceSerial = await ActivateLicenseAsync(code);
        //            if (deviceSerial != Guid.Empty)
        //            {
        //                await UpdateConfigurationAfterActivationAsync(code, deviceSerial);
        //            }
        //        }
        //        else
        //        {
        //            new ShowMessage("Invalid activation key format.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        new ShowMessage($"Error: {ex.Message}");
        //    }
        //}

        private async Task<Guid> ActivateLicenseAsync(Guid activationKey)
        {
            try
            {
                await UpdateDeviceActionsAsync();

                DeviceActions deviceActions;
                using (var deviceContext = new DeviceContext())
                {
                    deviceActions = deviceContext.DeviceActions.FirstOrDefault();
                    if (deviceActions == null)
                    {
                        throw new InvalidOperationException("Device actions not found");
                    }
                }

                var deviceDto = new ActivateDeviceDTO
                {
                    DeviceIp = deviceActions.DeviceIp,
                    MacAddress = deviceActions.MacAddress,
                    SerialNumber = deviceActions.SerialNumber,
                    OperatingSystemVersion = deviceActions.OperatingSystemVersion,
                    DeviceName = deviceActions.DeviceName,
                    TypeOfLicense = "Premium"
                };

                var response = await _httpClient.PostAsync(
                    $"{DeviceManagement.MainURL}Licenses/ActivateDevice/{activationKey}",
                    new StringContent(JsonConvert.SerializeObject(deviceDto), Encoding.UTF8, "application/json")
                );

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Guid>();
                }
                else
                {
                    await HandleActivationErrorAsync(response);
                    return Guid.Empty;
                }
            }
            catch (HttpRequestException ex)
            {
                HandleNetworkError(ex);
                return Guid.Empty;
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "ActivationViewModel ActivateLicenseAsync");
                return Guid.Empty;
            }
        }

        private async Task HandleActivationErrorAsync(HttpResponseMessage response
        )
        {
            string errorMessage = await response.Content.ReadAsStringAsync();

            if (errorMessage != "Invalid license")
            {
                try
                {
                    var invalidFields = JsonConvert.DeserializeObject<HandleActivationInvalidDataDTO>(errorMessage);
                    if (invalidFields?.title == "One or more validation errors occurred.")
                    {
                        var errorProperties = typeof(errors).GetProperties();
                        foreach (var property in errorProperties)
                        {
                            if (property.GetValue(invalidFields.errors) is List<string> value && value.Any())
                            {
                                new ShowMessage(value.FirstOrDefault());
                                return;
                            }
                        }
                    }
                }
                catch
                {
                    // If JSON parsing fails, use the raw error message
                }
            }

            new ShowMessage(errorMessage);
        }

        private void HandleNetworkError(HttpRequestException ex)
        {
            string message = ex.InnerException?.Message ?? ex.Message;

            if (message.Contains("operation was canceled"))
            {
                new ShowMessage("There is a problem in i-Freeze server");
            }
            else if (message.Contains("No such host is known"))
            {
                new ShowMessage("There is a problem in the network");
            }
            else
            {
                new ShowMessage(message);
            }
        }

      

        private async Task UpdateDeviceActionsAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    var deviceManagement = new DeviceManagement();
                    deviceManagement.UpdateDeviceActions();
                });
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "ActivationViewModel UpdateDeviceActionsAsync");
            }
        }

        #region IDisposable Implementation
        public override void Dispose()
        {
            if (!_disposed)
            {
                // Note: Don't dispose static HttpClient as it's shared
                _disposed = true;
            }
            base.Dispose();
        }
        #endregion
    }
}