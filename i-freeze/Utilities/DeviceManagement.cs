using ControlzEx.Standard;
using i_freeze.Commands.MessageBoxCommands;
using i_freeze.DTOs;
using i_freeze.Services;
using i_freeze.View;
using IceLockWorker;
using Infrastructure;
using Infrastructure.DataContext;
using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using ProgressBar = i_freeze.View.ProgressBar;

namespace i_freeze.Utilities
{
    public class DeviceManagement
    {
        // to get url cloud from database
        private static string GetMainURL()
        {
            using (var baseURLContext = new BaseURLContext())
            {
                var url = baseURLContext.CloudURL.FirstOrDefault()?.URL;

                // Return the URL or a default value if it's null


                return url ?? "https://security.flothers.com:8443/api/";
                //  return url ?? "https://central.flothers.com:8443/api/";
                // return  "https://central.flothers.com:8443/api/";
            }
        }

        // Property to get the URL from the database
        public static string MainURL => GetMainURL();

        public System.Timers.Timer timer;

        private static void Cleanup(string extractPath, string zipPath)
        {
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
        }

        public async static Task<string> Token()
        {
            using (var tokenContext = new iFreezeTokenContext())
            {
                var token = tokenContext.iFreezeToken.SingleOrDefault();

                if (DateTime.TryParse(token.Expiration, out DateTime expiration) && expiration < DateTime.UtcNow.AddDays(-1))
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            var appConfigContext = new AppConfigAndLoginContext();

                            var deviceId = appConfigContext.Application_Configuration.FirstOrDefault().DeviceSN;
                            string url = $"{MainURL}Account/iFreezeLogin/{deviceId}";

                            HttpResponseMessage response = await client.GetAsync(url);

                            if (response.IsSuccessStatusCode)
                            {
                                string json = await response.Content.ReadAsStringAsync();
                                iFreezeToken apiResponse = JsonConvert.DeserializeObject<iFreezeToken>(json);

                                if (apiResponse != null)
                                {
                                    token.Expiration = apiResponse.Expiration.ToString();
                                    token.Token = apiResponse.Token;

                                    tokenContext.SaveChanges();

                                    return apiResponse.Token;
                                }

                            }
                            else
                            {
                                throw new Exception("API request to get new token failed with status code: " + response.StatusCode);
                            }
                        }
                        catch (HttpRequestException ex)
                        {
                            await SendLogs(ex.Message, "DeviceManagement class Token method");
                        }
                    }
                }



                return token.Token;
            }


        }

        public async void Runner(string exe)
        {
            //new WindowsServiceControl().FreezeWR();
            try
            {
                void run()
                {
                    ExeRunner RunProcess = new ExeRunner(exe);
                    RunProcess.runprocess();

                }
                ThreadStart thstart = run; // put function in thread to make it run in background and allow progresspar to run 
                Thread th = new Thread(thstart);
                await Task.Run(() => // Make task Schedluer to make all process run with the correct time 
                {
                    th.Start();
                    th.Join(); // Join Function it to make progress Par run until the scan finish 
                });

            }
            catch (Exception ex)
            {
                await SendLogs(ex.Message, "DeviceManagement class Runner method");
            }
        }

        public async Task<List<VersionsDTO>> GetVersionById(decimal versionNumber, Guid id)
        {

            using (HttpClient httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

                HttpResponseMessage response = await httpClient.GetAsync($"{MainURL}Versions/GetAllVersionsById?num={versionNumber}&id={id}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    APIResponse<List<VersionsDTO>> apiResponse = JsonConvert.DeserializeObject<APIResponse<List<VersionsDTO>>>(json);

                    return apiResponse.Data;
                }
                else
                {
                    throw new Exception("API request to get Version Number failed with status code: " + response.StatusCode);
                }
            }

        }


        public async static Task<bool> SendLogs(string error, string className)
        {
            //Console.WriteLine(MainDBsPath.DatabasesPath);
            //D:\\NewCode\\i-freezeGui_Kiosk-worker - i-FreezeLite - 64-32\\i-freeze\\bin\\x64\\Release\\net8.0-windows\\Databases
            var logsContext = new iFreezeLogsContext();

            try
            {
                var errorTime = $"{className} \\ {DateTime.UtcNow} \\ {error}";

                HttpClient client = new HttpClient();

                AppConfigAndLoginContext Context = new AppConfigAndLoginContext();

                iFreezeLogsDTO log = new iFreezeLogsDTO();
                log.Log = errorTime;
                log.DeviceId = Context.Application_Configuration.FirstOrDefault().DeviceSN;

                string jsoneventViewer = JsonConvert.SerializeObject(log);

                var content = new StringContent(jsoneventViewer, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"{MainURL}iFreezeLogs", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    logsContext.iFreezeLogs.Add(new iFreezeLogs { Log = errorTime });
                    logsContext.SaveChanges();

                    return false;
                }
            }
            catch
            {
                var errorTime = $"{"DeviceManagement class SendLogs method"} \\ {DateTime.UtcNow} \\ {error}";

                logsContext.iFreezeLogs.Add(new iFreezeLogs { Log = errorTime });
                logsContext.SaveChanges();
            }


            return false;

        }

        public async Task PostData()
        {
            try
            {
                using (AppConfigAndLoginContext context = new AppConfigAndLoginContext())
                {
                    //Guid DeviceSN = context.Application_Configuration.FirstOrDefault().DeviceSN;
                    //RetrieveDeviceConfigurationsMainWindow(DeviceSN);


                    //call sync method
                }
                using (ActivateDLPContext context = new ActivateDLPContext())
                {
                    string email = context.ActivateDLPEntity.FirstOrDefault().Email;
                    RetrieveDeviceConfigurationsMainWindow(email);

                }

                //Update();


            }
            catch (Exception ex)
            {
                await SendLogs(ex.Message, "DeviceManagement class PostData method");
            }


        }

        public void StopPostingData()
        {
            // Stop the timer and release resources
            timer.Stop();
            timer.Dispose();
        }

        public void UpdateDeviceActions()
        {
            string ipAddress = GetIPAddress();

            string macAddress = GetMACAddress();

            string deviceName = GetDeviceName();

            string windowsVersion = GetWindowsVersion();

            string serialNumber = GetComputerSerialNumber();

            //string serialNumber = GetMotherboardUUID();



            var deviceActionsContext = new DeviceContext();

            DeviceActions deviceActions = deviceActionsContext.DeviceActions.FirstOrDefault();
            deviceActions.DeviceIp = ipAddress;
            deviceActions.MacAddress = macAddress;
            deviceActions.SerialNumber = serialNumber;
            deviceActions.DeviceName = deviceName;
            deviceActions.OperatingSystemVersion = windowsVersion;
            deviceActionsContext.SaveChanges();


        }

        static string GetIPAddress()
        {
            string ipAddress = string.Empty;

            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                        foreach (UnicastIPAddressInformation address in ipProperties.UnicastAddresses)
                        {
                            if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ipAddress = address.Address.ToString();
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(ipAddress))
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                SendLogs(ex.Message, "DeviceManagement class GetIPAddress method");
            }

            return ipAddress;
        }

        static string GetMACAddress()
        {
            string macAddress = string.Empty;

            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface networkInterface in networkInterfaces)
                {
                    if (networkInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        macAddress = networkInterface.GetPhysicalAddress().ToString();
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                SendLogs(ex.Message, "DeviceManagement class GetMACAddress method");
            }

            return macAddress;
        }

        static string GetDeviceName()
        {
            string deviceName = Environment.MachineName;
            return deviceName;
        }

        static string GetWindowsVersion()
        {
            string windowsVersion = string.Empty;
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject os in searcher.Get())
                    {
                        windowsVersion = os["Caption"].ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                SendLogs(ex.Message, "DeviceManagement class GetWindowsVersion method");
            }

            return windowsVersion;
        }

        static string GetComputerSerialNumber()
        {
            string serialNumber = string.Empty;

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    serialNumber = obj["SerialNumber"].ToString();
                    break; // Assuming there's only one BIOS, so we break after the first result
                }
            }
            catch (Exception ex)
            {
                SendLogs(ex.Message, "DeviceManagement class GetComputerSerialNumber method");

            }

            return serialNumber;
        }

        //public string GetMotherboardUUID()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");
        //        foreach (ManagementObject obj in searcher.Get())
        //        {
        //            return obj["UUID"].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SendLogs(ex.Message, "DeviceManagement class DeviceSecurity method");
        //        return "";
        //    }

        //    return "";
        //}

        public async void DeviceSecurity()
        {
            try
            {
                using (DeviceContext context = new DeviceContext())
                {

                    // Run Performance Monitor
                    Process[] MP = Process.GetProcessesByName("i-FreezeMP");
                    if (MP.Length == 0)
                    {
                        void run()
                        {
                            var runner2 = new ExeRunner("i-FreezeMP.exe");
                            runner2.runprocess();


                        }
                        ThreadStart thstart = run; // put function in thread to make it run in background and allow progresspar to run 
                        Thread th = new Thread(thstart);
                        await Task.Run(() => // Make task Schedluer to make all process run with the correct time 
                        {
                            th.Start();
                            th.Join(); // Join Function it to make progress Par run until the scan finish 
                        });

                    }

                    DeviceActions deviceAction = context.DeviceActions.FirstOrDefault(x => x.Id == 1);

                    var NSPP = deviceAction.ActivateNetworkScan;
                    if (NSPP == "true")
                    {
                        //Process[] nn = Process.GetProcessesByName("i-FreezeNSPP");
                        //if (nn.Length == 0)
                        //{
                        //    void run()
                        //    {
                        //        ExeRunner RunProcess = new ExeRunner(@"i-FreezeNSPP.exe");
                        //        RunProcess.runprocess();


                        //    }
                        //    ThreadStart thstart = run; // put function in thread to make it run in background and allow progresspar to run 
                        //    Thread th = new Thread(thstart);
                        //    Task.Run(() => // Make task Schedluer to make all process run with the correct time 
                        //    {
                        //        th.Start();
                        //        th.Join(); // Join Function it to make progress Par run until the scan finish 
                        //    });

                        //}
                        await new WindowsServiceControl().NetworkScanToggleStartUp();

                    }

                    var KDS = deviceAction.ActivateProactiveScan;
                    if (KDS == "true")
                    {
                        await new WindowsServiceControl().StartUpFreezeWRAndFreezeNM();

                        //Process[] pname = Process.GetProcessesByName("i-FreezeWK");
                        //if (pname.Length == 0)
                        //{
                        //    void run()
                        //    {
                        //        ExeRunner runner = new ExeRunner(@"i-FreezeWK.exe");
                        //        runner.RunProcessForArabic();
                        //    }
                        //    ThreadStart thstart = run; // put function in thread to make it run in background and allow progresspar to run 
                        //    Thread th = new Thread(thstart);

                        //    Task.Run(() => // Make task Schedluer to make all process run with the correct time 
                        //    {
                        //        th.Start();
                        //        th.Join(); // Join Function it to make progress Par run until the scan finish 
                        //    });

                        //}
                    }

                    var USBS = deviceAction.EnableUSBScan;
                    if (USBS == "true")
                    {
                        Process[] pname = Process.GetProcessesByName("i-FreezeAutoUSBS");
                        if (pname.Length == 0)
                        {
                            ExeRunner Runrocess = new ExeRunner(@"i-FreezeAutoUSBS.exe");
                            Runrocess.RunProcessForArabic();
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                await SendLogs(ex.Message, "DeviceManagement class DeviceSecurity method");
            }
        }

        public class APIResponse<T>
        {
            public T Data { get; set; }
            public int Status { get; set; }
            public string Message { get; set; }
        }

        public async Task<DeviceConfigurationsDTO> GetDeviceConfigurationsFromAPI(Guid deviceId)
        {
            using (HttpClient client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await DeviceManagement.Token());

                string url = $"{MainURL}Devices/GetDeviceConfigurations?deviceId={deviceId}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    APIResponse<DeviceConfigurationsDTO> apiResponse = JsonConvert.DeserializeObject<APIResponse<DeviceConfigurationsDTO>>(json);

                    return apiResponse.Data;
                }
                else
                {
                    throw new Exception("API request to get device configrations failed with status code: " + response.StatusCode);
                }
            }
        }

        private static readonly HttpClient client = new HttpClient();

        public async void RetrieveDeviceConfigurations(string email)
        {
            try
            {
                ProgressBar win2 = new ProgressBar(); // calling progressbar to use it in this button
                win2.Show();

                await SyncFunction(email);

                win2.Close();

                new ShowMessage("Sync Completed Successfully");

            }
            catch (Exception ex)
            {
                await SendLogs(ex.Message, "DeviceManagement class RetrieveDeviceConfigurations method");

                if (ex.Message.Contains("database is locked"))
                {
                    new ShowMessage("Sync Completed Successfully");
                }
                else
                {
                    new ShowMessage("An error occurred: " + ex.Message);
                }
            }
        }

        //public async void RetrieveDeviceConfigurationsMainWindow(Guid deviceId)
        //{
        //    try
        //    {
        //        await SyncFunction(deviceId);
        //    }
        //    catch (Exception ex)
        //    {
        //        await SendLogs(ex.Message, "DeviceManagement class RetrieveDeviceConfigurationsMainWindow method");

        //    }
        //}
        public async void RetrieveDeviceConfigurationsMainWindow(string email)
        {
            try
            {
                await SyncFunction(email);
            }
            catch (Exception ex)
            {
                await SendLogs(ex.Message, "DeviceManagement class RetrieveDeviceConfigurationsMainWindow method");

            }
        }
        public async Task<List<VulnAppsDTO>> VulnApps(Guid id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

                string url = $"{MainURL}VulnApps/GetAllVulnAppsById/{id}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    APIResponse<List<VulnAppsDTO>> apiResponse = JsonConvert.DeserializeObject<APIResponse<List<VulnAppsDTO>>>(json);

                    return apiResponse.Data;
                }
                else
                {
                    throw new Exception("API request to get Vuln Apps failed with status code: " + response.StatusCode);
                }
            }
        }

        private async Task UpdateDeviceSerial(Guid deviceId)
        {
            UpdateSerialDTO updateSerial = new UpdateSerialDTO();
            updateSerial.SerialNumber = GetComputerSerialNumber();

            string jsoneventViewer = JsonConvert.SerializeObject(updateSerial);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

                var content = new StringContent(jsoneventViewer, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"{MainURL}Devices/UpdateSerial/{deviceId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    await SendLogs("Send device serial number to cloude failed", "DeviceManagement class UpdateDeviceSerial method");
                }
            }
        }

        public async Task<CloudURL> GetNewCloudURL(Guid id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

                string url = $"{MainURL}CloudURL/{id}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    APIResponse<CloudURL> apiResponse = JsonConvert.DeserializeObject<APIResponse<CloudURL>>(json);

                    return apiResponse.Data;
                }
                else
                {
                    throw new Exception("API request to get new cloud url failed with status code: " + response.StatusCode);
                }
            }
        }

        private async Task<UpdateSignsDBDTO> GetUpdateSignsDBVersion()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

                    string url = $"{MainURL}UpdateSignsDB/GetSignsDBVersionNum";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<UpdateSignsDBDTO>(json);
                    }

                    return null;

                }
            }
            catch
            {
                return null;
            }

        }

        private async Task<AddMalwareHashsFlagDTO> GetAddMalwareHashsFlag(Guid Id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

                    string url = $"{MainURL}AddMalwareHashsFlag/{Id}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        APIResponse<AddMalwareHashsFlagDTO> apiResponse = JsonConvert.DeserializeObject<APIResponse<AddMalwareHashsFlagDTO>>(json);

                        return apiResponse.Data;
                    }

                    return null;

                }
            }
            catch
            {
                return null;
            }

        }

        private async Task<List<AddMalwareHashsDTO>> GetAddMalwareHashs(Guid Id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

                    string url = $"{MainURL}AddMalwareHashs/{Id}";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        APIResponse<List<AddMalwareHashsDTO>> apiResponse = JsonConvert.DeserializeObject<APIResponse<List<AddMalwareHashsDTO>>>(json);

                        return apiResponse.Data;
                    }

                    return null;

                }
            }
            catch
            {
                return null;
            }

        }

        public async void PostAutomatedActions(BlockedProcessesDTO blockedProcesses)
        {

            string jsoneventViewer = JsonConvert.SerializeObject(blockedProcesses);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

                var content = new StringContent(jsoneventViewer, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"{MainURL}BlockedProcesses", content);

                if (!response.IsSuccessStatusCode)
                {
                    await SendLogs("Send Automated Actions to cloude failed", "DeviceManagement class PostAutomatedActions method");
                }
            }
        }



        public async Task SyncFunction(string email)
        {
            await SyncUserLabels(email);
            await SyncUserPatterns(email);
            var classificationSettings = await SyncClassificationSettings(email);
            if (classificationSettings != null)
            {
                await ApplyClassificationSettingsAsync(classificationSettings);
            }
        }
        private async Task SyncUserPatterns(string email)
        {
            try
            {
                var patternSvc = new PatternService();
                var added = await patternSvc.FetchAndAppendPatternsAsync(email);
                //new ShowMessage($"Patterns added: {added.Count}");
            }
            catch (Exception ex)
            {
                new ShowMessage("Failed to sync patterns: " + ex.Message);
            }
        }
        private async Task SyncUserLabels(string email)
        {
            try
            {
                var svc = new PolicyService();
                var saved = await svc.FetchAndSaveUserPoliciesAsync(email);
                //new ShowMessage($"Saved/updated {saved.Count} policies.");
            }
            catch (Exception ex)
            {
                new ShowMessage("Could not sync policies: " + ex.Message);
            }
        }
        private async Task<DataClassificationSettings> SyncClassificationSettings(string email)
        {
            try
            {
                var svc = new ClassificationSettingsService();
                var settings = await svc.FetchAndSaveClassificationSettingsAsync(email);
                //  new ShowMessage("Classification settings synced.");
                return settings;
            }
            catch (Exception ex)
            {
                new ShowMessage("Could not sync classification settings: " + ex.Message);
                return null;
            }
        }


        private async Task ApplyClassificationSettingsAsync(DataClassificationSettings settings)
        {
            var svc = new ClassificationSettingsService();
            await svc.ApplyClassificationSettingsAsync(settings);

        }

    }
}
