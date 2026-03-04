//using i_freeze.Commands.MessageBoxCommands;
//using Infrastructure.DataContext;
//using i_freeze.DTOs;
//using Infrastructure.Model;
//using i_freeze.View;
//using Microsoft.Win32;
//using NAudio.CoreAudioApi;
//using Newtonsoft.Json;
//using System;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.IO;
//using System.IO.Compression;
//using System.Linq;
//using System.Management;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Net.NetworkInformation;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Windows;
//using System.Xml.Linq;
//using ProgressBar = i_freeze.View.ProgressBar;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Cryptography.X509Certificates;
//using Infrastructure;

//namespace i_freeze.Utilities
//{
//    public class DeviceManagement1
//    {
//        // to get url cloud from database
//        private static string GetMainURL()
//        {
//            using (var baseURLContext = new BaseURLContext())
//            {
//                var url = baseURLContext.CloudURL.FirstOrDefault()?.URL;

//                // Return the URL or a default value if it's null
                 

//                return url ?? "https://security.flothers.com:8443/api/";

//            }

//        }

//        // Property to get the URL from the database
//        public static string MainURL => GetMainURL();

//        public System.Timers.Timer timer;

//        private static void Cleanup(string extractPath, string zipPath)
//        {
//            if (Directory.Exists(extractPath))
//            {
//                Directory.Delete(extractPath, true);
//            }
//            if (File.Exists(zipPath))
//            {
//                File.Delete(zipPath);
//            }
//        }

//        public async static Task<string> Token()
//        {
//            using (var tokenContext = new iFreezeTokenContext())
//            {
//                var token = tokenContext.iFreezeToken.SingleOrDefault();

//                if (DateTime.TryParse(token.Expiration, out DateTime expiration) && expiration < DateTime.UtcNow.AddDays(-1))
//                {
//                    using (var client = new HttpClient())
//                    {
//                        try
//                        {
//                            var appConfigContext = new AppConfigAndLoginContext();

//                            var deviceId = appConfigContext.Application_Configuration.FirstOrDefault().DeviceSN;
//                            string url = $"{MainURL}Account/iFreezeLogin/{deviceId}";

//                            HttpResponseMessage response = await client.GetAsync(url);

//                            if (response.IsSuccessStatusCode)
//                            {
//                                string json = await response.Content.ReadAsStringAsync();
//                                iFreezeToken apiResponse = JsonConvert.DeserializeObject<iFreezeToken>(json);

//                                if (apiResponse != null)
//                                {
//                                    token.Expiration = apiResponse.Expiration.ToString();
//                                    token.Token = apiResponse.Token;

//                                    tokenContext.SaveChanges();

//                                    return apiResponse.Token;
//                                }

//                            }
//                            else
//                            {
//                                throw new Exception("API request to get new token failed with status code: " + response.StatusCode);
//                            }
//                        }
//                        catch (HttpRequestException ex)
//                        {
//                            await SendLogs(ex.Message, "DeviceManagement class Token method");
//                        }
                         

//                    }
//                }
                 


//                return token.Token;
//            }

//        }

//        public async void Runner(string exe)
//        {
//            //new WindowsServiceControl().FreezeWR();
//            try
//            {
//                void run()
//                {
//                    ExeRunner RunProcess = new ExeRunner(exe);
//                    RunProcess.runprocess();

//                }
//                ThreadStart thstart = run; // put function in thread to make it run in background and allow progresspar to run 
//                Thread th = new Thread(thstart);
//                await Task.Run(() => // Make task Schedluer to make all process run with the correct time 
//                {
//                    th.Start();
//                    th.Join(); // Join Function it to make progress Par run until the scan finish 
//                });

//            }
//            catch (Exception ex)
//            {
//                await SendLogs(ex.Message, "DeviceManagement class Runner method");
//            }
//        }

//        public async Task<List<VersionsDTO>> GetVersionById(decimal versionNumber, Guid id)
//        {

//            using (HttpClient httpClient = new HttpClient())
//            {

//                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                HttpResponseMessage response = await httpClient.GetAsync($"{MainURL}Versions/GetAllVersionsById?num={versionNumber}&id={id}");
//                if (response.IsSuccessStatusCode)
//                {
//                    string json = await response.Content.ReadAsStringAsync();
//                    APIResponse<List<VersionsDTO>> apiResponse = JsonConvert.DeserializeObject<APIResponse<List<VersionsDTO>>>(json);

//                    return apiResponse.Data;
//                }
//                else
//                {
//                    throw new Exception("API request to get Version Number failed with status code: " + response.StatusCode);
//                }
//            }

//        }

//        public async void Update()
//        {
//            string apiBaseUrl = $"{MainURL}Versions";
//            var zipPath = "C:\\Users\\Public\\i-Freeze\\UpdatedVersion.zip";
//            string extractPath = "C:\\Users\\Public\\i-Freeze\\UpdateFiles\\";
//            string versionType = "PremiumNewCode"; // Replace with the actual version type
//            decimal versionNumber;
//            ProgressBar win2 = null;

//            using (AppConfigAndLoginContext context = new AppConfigAndLoginContext())
//            {
//                versionNumber = context.Application_Configuration.FirstOrDefault().VersionNumber;
                 

//            }

//            using (var httpClient = new HttpClient())
//            {
//                try
//                {
//                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                    HttpResponseMessage GetLatestVersionResponse = await client.GetAsync($"{apiBaseUrl}/GetLatestVersion?versionType={versionType}");
//                    decimal latestVersion = await GetLatestVersionResponse.Content.ReadAsAsync<decimal>();

//                    if (versionNumber < latestVersion)
//                    {
//                        HttpResponseMessage response = await httpClient.GetAsync($"{apiBaseUrl}/DownloadFile?versionType={versionType}&versionNumber={versionNumber}");
//                        if (!response.IsSuccessStatusCode)
//                        {
//                            // Handle unsuccessful response
//                        }
//                        else
//                        {
//                            Cleanup(extractPath, zipPath);
//                            var bytes = await response.Content.ReadAsByteArrayAsync();
//                            File.WriteAllBytes(zipPath, bytes);

//                            ZipFile.ExtractToDirectory(zipPath, extractPath);


//                            using (AppConfigAndLoginContext updateContext = new AppConfigAndLoginContext())
//                            {
//                                var config = updateContext.Application_Configuration.FirstOrDefault(c => c.Id == 1);
//                                if (config != null)
//                                {
//                                    config.VersionNumber = latestVersion;
//                                    updateContext.SaveChanges();

//                                    var updateDeviceVesion = new VersionNumberDTO();
//                                    updateDeviceVesion.VersionNumber = latestVersion;

//                                    string json = JsonConvert.SerializeObject(updateDeviceVesion);

//                                    var content = new StringContent(json, Encoding.UTF8, "application/json");

//                                    await client.PostAsync($"{MainURL}Devices/UpdateDeviceVersion/{config.DeviceSN}", content);

//                                }
                                 

//                            }

//                            string[] processNames = { "i-FreezeWK", "i-FreezeWR", "i-FreezeNM", "i-FreezeAutoUSBS", "i-FreezeMP", "i-FreezePW", "i-FreezeSA", "i-FreezeNSPP", "i-FreezeFSS", "i-FreezeFSSBG", "i-FreezeUSBScan", "i-FreezeNSAD", "i-FreezeScanReg", "i-FreezeMPC", "i-FreezeMPM" };
//                            foreach (string processName in processNames)
//                            {
//                                foreach (var process in Process.GetProcessesByName(processName))
//                                {
//                                    process.Kill();
//                                }
//                            }


//                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
//                            {
//                                new ShowMessage("A new i-Freeze update is availabe. Please press update to install the new version.");
//                            });

//                            // to check if there is update to windows service to run i-FreezeSoftUpdate.exe
//                            string filePath = @"C:\Users\Public\i-Freeze\UpdateFiles\IceLockWorker.exe";


//                            if (File.Exists(filePath))
//                            {
//                                System.Windows.Application.Current.Dispatcher.Invoke(() =>
//                                {
//                                    if (win2 != null)
//                                    {
//                                        win2.Show();
//                                    }
//                                });

//                                Process[] pName = Process.GetProcessesByName("i-FreezeSoftUpdate");
//                                int checkIsAdminClicked = 0;
//                                if (pName.Length == 0)
//                                {
//                                    ExeRunner RunProcess = new ExeRunner("i-FreezeSoftUpdate.exe");
//                                    checkIsAdminClicked = await RunProcess.RunProcessAsAdminForSoftUpdate();
//                                }

//                                if (checkIsAdminClicked == 0)
//                                {
//                                    await Task.Delay(TimeSpan.FromSeconds(15));

//                                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
//                                    {
//                                        if (win2 != null)
//                                        {
//                                            win2.Close();
//                                        }

//                                        new ShowMessage("i-Freeze is now updated, please click Ok to restart i-Freeze.");
//                                    });

//                                    // Call updater executable
//                                    Process.Start("C:\\Users\\Public\\i-Freeze\\Update.exe");
//                                }



//                            }
//                            else
//                            {
//                                // Call updater executable
//                                Process.Start("C:\\Users\\Public\\i-Freeze\\Update.exe");
//                            }
//                        }
//                    }
//                    else
//                    {
//                        return;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    await SendLogs(ex.Message, "DeviceManagement class Update method");
//                }
//                finally
//                {
//                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
//                    {
//                        if (win2 != null)
//                        {
//                            win2.Close();
//                        }
//                    });
//                }
//            }
//        }

//        public async void UpdateWindowsServiceForSync()
//        {
//            ProgressBar win2 = null;

//            try
//            {

//                string[] processNames = { "i-FreezeWK", "i-FreezeAutoUSBS", "i-FreezeMP", "i-FreezePW", "i-FreezeSA", "i-FreezeNSPP", "i-FreezeFSS", "i-FreezeFSSBG", "i-FreezeUSBScan", "i-FreezeNSAD", "i-FreezeScanReg", "i-FreezeMPC", "i-FreezeMPM" };
//                foreach (string processName in processNames)
//                {
//                    foreach (var process in Process.GetProcessesByName(processName))
//                    {
//                        process.Kill();
//                    }
//                }

//                new ShowMessage("A new i-Freeze update is availabe. Please press update to install the new version.");


//                System.Windows.Application.Current.Dispatcher.Invoke(() =>
//                {
//                    if (win2 != null)
//                    {
//                        win2.Show();
//                    }
//                });

//                Process[] pName = Process.GetProcessesByName("i-FreezeSoftUpdate");
//                if (pName.Length == 0)
//                {
//                    ExeRunner RunProcess = new ExeRunner("i-FreezeSoftUpdate.exe");
//                    RunProcess.RunProcessAsAdmin();
//                }

//                await Task.Delay(TimeSpan.FromSeconds(15));

//                System.Windows.Application.Current.Dispatcher.Invoke(() =>
//                {
//                    if (win2 != null)
//                    {
//                        win2.Close();
//                    }
//                });

//                new ShowMessage("i-Freeze is now updated, please click Ok to restart i-Freeze.");

//                // to check if there is update to windows service to run i-FreezeSoftUpdate.exe
//                string windowsServicePath = @"C:\Users\Public\i-Freeze\UpdateFiles\IceLockWorker.exe";


//                if (File.Exists(windowsServicePath))
//                {
//                    File.Delete(windowsServicePath);
//                }

//                // Call updater executable
//                Process.Start("C:\\Users\\Public\\i-Freeze\\Update.exe");


//            }
//            catch (Exception ex)
//            {
//                await SendLogs(ex.Message, "DeviceManagement class UpdateWindowsServiceForSync method");
//            }
//            finally
//            {
//                System.Windows.Application.Current.Dispatcher.Invoke(() =>
//                {
//                    if (win2 != null)
//                    {
//                        win2.Close();
//                    }
//                });
//            }


//        }

//        public async static Task<bool> SendLogs(string error, string className)
//        {
//            var logsContext = new iFreezeLogsContext();

//            try
//            {
//                var errorTime = $"{className} \\ {DateTime.UtcNow} \\ {error}";

//                HttpClient client = new HttpClient();

//                AppConfigAndLoginContext Context = new AppConfigAndLoginContext();

//                iFreezeLogsDTO log = new iFreezeLogsDTO();
//                log.Log = errorTime;
//                log.DeviceId = Context.Application_Configuration.FirstOrDefault().DeviceSN;

//                string jsoneventViewer = JsonConvert.SerializeObject(log);

//                var content = new StringContent(jsoneventViewer, Encoding.UTF8, "application/json");

//                HttpResponseMessage response = await client.PostAsync($"{MainURL}iFreezeLogs", content);

//                if (response.IsSuccessStatusCode)
//                {
//                    return true;
//                }
//                else
//                {
//                    logsContext.iFreezeLogs.Add(new iFreezeLogs { Log = errorTime });
//                    logsContext.SaveChanges();

//                    return false;
//                }
//            }
//            catch
//            {
//                var errorTime = $"{"DeviceManagement class SendLogs method"} \\ {DateTime.UtcNow} \\ {error}";

//                logsContext.iFreezeLogs.Add(new iFreezeLogs { Log = errorTime });
//                logsContext.SaveChanges();
//            }
             


//            return false;

//        }

//        public async Task PostData()
//        {
//            try
//            {
//                await System.Windows.Application.Current.Dispatcher.InvokeAsync(async () =>
//                {
//                    List<EventViewer> eventViewer;
//                    using (var dbContext = new EventViewerContext())
//                    {
//                        eventViewer = dbContext.EventViewer.ToList();
//                    }
                     


//                    string jsoneventViewer = JsonConvert.SerializeObject(eventViewer);

//                    using (HttpClient client = new HttpClient())
//                    {
//                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                        var content = new StringContent(jsoneventViewer, Encoding.UTF8, "application/json");

//                        HttpResponseMessage response = await client.PostAsync($"{MainURL}Alerts", content);

//                        if (response.IsSuccessStatusCode)
//                        {
//                            // Display success message box
//                            //MyMessageBox myMessageBox = new MyMessageBox();
//                            //myMessageBox.ShowMessageBox("Data posted successfully!");
//                        }
//                        else
//                        {
//                            // Display error message box
//                            //MyMessageBox myMessageBox = new MyMessageBox();
//                            //myMessageBox.ShowMessageBox("Failed to post data!");
//                        }
//                    }


//                });


//                //static void TimerEventProcessor(object state)
//                //{
//                //    // Run the ShowCurrentProcess method asynchronously
//                //    Task.Run(() => ShowCurrentProcess());
//                //}

//                //// Define the initial delay time (3 hours)
//                //TimeSpan initialDelay = TimeSpan.FromSeconds(10);

//                //// Create a Timer object that triggers only once after a 3-hour delay
//                //Timer timer = new Timer(new TimerCallback(TimerEventProcessor), null, initialDelay, Timeout.InfiniteTimeSpan);


//                // To keep the main thread running (for demonstration purposes)




//                using (AppConfigAndLoginContext context = new AppConfigAndLoginContext())
//                {
//                    Guid DeviceSN = context.Application_Configuration.FirstOrDefault().DeviceSN;
//                    RetrieveDeviceConfigurationsMainWindow(DeviceSN);
                     

//                }

//                Update();


//            }
//            catch (Exception ex)
//            {
//                await SendLogs(ex.Message, "DeviceManagement class PostData method");
//            }
//        }

//        public void StopPostingData()
//        {
//            // Stop the timer and release resources
//            timer.Stop();
//            timer.Dispose();
//        }

//        public void UpdateDeviceActions()
//        {
//            string ipAddress = GetIPAddress();

//            string macAddress = GetMACAddress();

//            string deviceName = GetDeviceName();

//            string windowsVersion = GetWindowsVersion();

//            string serialNumber = GetComputerSerialNumber();

//            //string serialNumber = GetMotherboardUUID();



//            var deviceActionsContext = new DeviceContext();

//            DeviceActions deviceActions = deviceActionsContext.DeviceActions.FirstOrDefault();
//            deviceActions.DeviceIp = ipAddress;
//            deviceActions.MacAddress = macAddress;
//            deviceActions.SerialNumber = serialNumber;
//            deviceActions.DeviceName = deviceName;
//            deviceActions.OperatingSystemVersion = windowsVersion;
//            deviceActionsContext.SaveChanges();
             


//        }

//        static string GetIPAddress()
//        {
//            string ipAddress = string.Empty;

//            try
//            {
//                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
//                foreach (NetworkInterface networkInterface in networkInterfaces)
//                {
//                    if (networkInterface.OperationalStatus == OperationalStatus.Up)
//                    {
//                        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

//                        foreach (UnicastIPAddressInformation address in ipProperties.UnicastAddresses)
//                        {
//                            if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
//                            {
//                                ipAddress = address.Address.ToString();
//                                break;
//                            }
//                        }
//                        if (!string.IsNullOrEmpty(ipAddress))
//                            break;
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                SendLogs(ex.Message, "DeviceManagement class GetIPAddress method");
//            }

//            return ipAddress;
//        }

//        static string GetMACAddress()
//        {
//            string macAddress = string.Empty;

//            try
//            {
//                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

//                foreach (NetworkInterface networkInterface in networkInterfaces)
//                {
//                    if (networkInterface.OperationalStatus == OperationalStatus.Up)
//                    {
//                        macAddress = networkInterface.GetPhysicalAddress().ToString();
//                        break;
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                SendLogs(ex.Message, "DeviceManagement class GetMACAddress method");
//            }

//            return macAddress;
//        }

//        static string GetDeviceName()
//        {
//            string deviceName = Environment.MachineName;
//            return deviceName;
//        }
         
//        static string GetWindowsVersion()
//        {
//            string windowsVersion = string.Empty;
//            try
//            {
//                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
//                {
//                    foreach (ManagementObject os in searcher.Get())
//                    {
//                        windowsVersion = os["Caption"].ToString();
//                        break;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                SendLogs(ex.Message, "DeviceManagement class GetWindowsVersion method");
//            }

//            return windowsVersion;
//        }

//        static string GetComputerSerialNumber()
//        {
//            string serialNumber = string.Empty;

//            try
//            {
//                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
//                ManagementObjectCollection collection = searcher.Get();

//                foreach (ManagementObject obj in collection)
//                {
//                    serialNumber = obj["SerialNumber"].ToString();
//                    break; // Assuming there's only one BIOS, so we break after the first result
//                }
//            }
//            catch (Exception ex)
//            {
//                SendLogs(ex.Message, "DeviceManagement class GetComputerSerialNumber method");

//            }

//            return serialNumber;
//        }

//        //public string GetMotherboardUUID()
//        //{
//        //    try
//        //    {
//        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");
//        //        foreach (ManagementObject obj in searcher.Get())
//        //        {
//        //            return obj["UUID"].ToString();
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        SendLogs(ex.Message, "DeviceManagement class DeviceSecurity method");
//        //        return "";
//        //    }

//        //    return "";
//        //}

//        public async void DeviceSecurity()
//        {
//            try
//            {
//                using (DeviceContext context = new DeviceContext())
//                {

//                    // Run Performance Monitor
//                    Process[] MP = Process.GetProcessesByName("i-FreezeMP");
//                    if (MP.Length == 0)
//                    {
//                        void run()
//                        {
//                            var runner2 = new ExeRunner("i-FreezeMP.exe");
//                            runner2.runprocess();


//                        }
//                        ThreadStart thstart = run; // put function in thread to make it run in background and allow progresspar to run 
//                        Thread th = new Thread(thstart);
//                        await Task.Run(() => // Make task Schedluer to make all process run with the correct time 
//                        {
//                            th.Start();
//                            th.Join(); // Join Function it to make progress Par run until the scan finish 
//                        });

//                    }

//                    DeviceActions deviceAction = context.DeviceActions.FirstOrDefault(x => x.Id == 1);

//                    var NSPP = deviceAction.ActivateNetworkScan;
//                    if (NSPP == "true")
//                    {
//                        Process[] nn = Process.GetProcessesByName("i-FreezeNSPP");
//                        if (nn.Length == 0)
//                        {
//                            void run()
//                            {
//                                ExeRunner RunProcess = new ExeRunner(@"i-FreezeNSPP.exe");
//                                RunProcess.runprocess();


//                            }
//                            ThreadStart thstart = run; // put function in thread to make it run in background and allow progresspar to run 
//                            Thread th = new Thread(thstart);
//                            Task.Run(() => // Make task Schedluer to make all process run with the correct time 
//                            {
//                                th.Start();
//                                th.Join(); // Join Function it to make progress Par run until the scan finish 
//                            });

//                        }
//                    }

//                    var KDS = deviceAction.ActivateProactiveScan;
//                    if (KDS == "true")
//                    {
//                        await new WindowsServiceControl().StartUpFreezeWRAndFreezeNM();

//                        Process[] pname = Process.GetProcessesByName("i-FreezeWK");
//                        if (pname.Length == 0)
//                        {
//                            void run()
//                            {
//                                ExeRunner runner = new ExeRunner(@"i-FreezeWK.exe");
//                                runner.RunProcessForArabic();
//                            }
//                            ThreadStart thstart = run; // put function in thread to make it run in background and allow progresspar to run 
//                            Thread th = new Thread(thstart);

//                            Task.Run(() => // Make task Schedluer to make all process run with the correct time 
//                            {
//                                th.Start();
//                                th.Join(); // Join Function it to make progress Par run until the scan finish 
//                            });

//                        }
//                    }

//                    var USBS = deviceAction.EnableUSBScan;
//                    if (USBS == "true")
//                    {
//                        Process[] pname = Process.GetProcessesByName("i-FreezeAutoUSBS");
//                        if (pname.Length == 0)
//                        {
//                            ExeRunner Runrocess = new ExeRunner(@"i-FreezeAutoUSBS.exe", @"i-FreezeUSBScan.exe");
//                            Runrocess.RunProcessForArabic();
//                        }
//                    }

//                    context.SaveChanges();


//                }
//            }
//            catch (Exception ex)
//            {
//                await SendLogs(ex.Message, "DeviceManagement class DeviceSecurity method");
//            }
             


//        }

//        public class APIResponse<T>
//        {
//            public T Data { get; set; }
//            public int Status { get; set; }
//            public string Message { get; set; }
//        }

//        public async Task<DeviceConfigurationsDTO> GetDeviceConfigurationsFromAPI(Guid deviceId)
//        {
//            using (HttpClient client = new HttpClient())
//            {

//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await DeviceManagement.Token());

//                string url = $"{MainURL}Devices/GetDeviceConfigurations?deviceId={deviceId}";

//                HttpResponseMessage response = await client.GetAsync(url);

//                if (response.IsSuccessStatusCode)
//                {
//                    string json = await response.Content.ReadAsStringAsync();
//                    APIResponse<DeviceConfigurationsDTO> apiResponse = JsonConvert.DeserializeObject<APIResponse<DeviceConfigurationsDTO>>(json);

//                    return apiResponse.Data;
//                }
//                else
//                {
//                    throw new Exception("API request to get device configrations failed with status code: " + response.StatusCode);
//                }
//            }
//        }

//        private static readonly HttpClient client = new HttpClient();

//        public async void RetrieveDeviceConfigurations(Guid deviceId)
//        {
//            try
//            {
//                ProgressBar win2 = new ProgressBar(); // calling progressbar to use it in this button
//                win2.Show();

//                await SyncFunction(deviceId);

//                win2.Close();

//                new ShowMessage("Sync Completed Successfully");

//            }
//            catch (Exception ex)
//            {
//                await SendLogs(ex.Message, "DeviceManagement class RetrieveDeviceConfigurations method");

//                if (ex.Message.Contains("database is locked"))
//                {
//                    new ShowMessage("Sync Completed Successfully");
//                }
//                else
//                {
//                    new ShowMessage("An error occurred: " + ex.Message);
//                }
//            }
//        }

//        public async void RetrieveDeviceConfigurationsMainWindow(Guid deviceId)
//        {
//            try
//            {
//                await SyncFunction(deviceId);
//            }
//            catch (Exception ex)
//            {
//                await SendLogs(ex.Message, "DeviceManagement class RetrieveDeviceConfigurationsMainWindow method");

//            }
//        }

//        public async Task<List<VulnAppsDTO>> VulnApps(Guid id)
//        {
//            using (HttpClient client = new HttpClient())
//            {
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                string url = $"{MainURL}VulnApps/GetAllVulnAppsById/{id}";
//                HttpResponseMessage response = await client.GetAsync(url);

//                if (response.IsSuccessStatusCode)
//                {
//                    string json = await response.Content.ReadAsStringAsync();
//                    APIResponse<List<VulnAppsDTO>> apiResponse = JsonConvert.DeserializeObject<APIResponse<List<VulnAppsDTO>>>(json);

//                    return apiResponse.Data;
//                }
//                else
//                {
//                    throw new Exception("API request to get Vuln Apps failed with status code: " + response.StatusCode);
//                }
//            }
//        }

//        private async Task UpdateDeviceSerial(Guid deviceId)
//        {
//            UpdateSerialDTO updateSerial = new UpdateSerialDTO();
//            updateSerial.SerialNumber = GetComputerSerialNumber();

//            string jsoneventViewer = JsonConvert.SerializeObject(updateSerial);

//            using (HttpClient client = new HttpClient())
//            {
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                var content = new StringContent(jsoneventViewer, Encoding.UTF8, "application/json");

//                HttpResponseMessage response = await client.PostAsync($"{MainURL}Devices/UpdateSerial/{deviceId}", content);

//                if (!response.IsSuccessStatusCode)
//                {
//                    await SendLogs("Send device serial number to cloude failed", "DeviceManagement class UpdateDeviceSerial method");
//                }
//            }
//        }

//        public async Task<CloudURL> GetNewCloudURL(Guid id)
//        {
//            using (HttpClient client = new HttpClient())
//            {
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                string url = $"{MainURL}CloudURL/{id}";
//                HttpResponseMessage response = await client.GetAsync(url);

//                if (response.IsSuccessStatusCode)
//                {
//                    string json = await response.Content.ReadAsStringAsync();
//                    APIResponse<CloudURL> apiResponse = JsonConvert.DeserializeObject<APIResponse<CloudURL>>(json);

//                    return apiResponse.Data;
//                }
//                else
//                {
//                    throw new Exception("API request to get new cloud url failed with status code: " + response.StatusCode);
//                }
//            }
//        }

//        private async Task<UpdateSignsDBDTO> GetUpdateSignsDBVersion()
//        {
//            try
//            {
//                using (HttpClient client = new HttpClient())
//                {
//                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                    string url = $"{MainURL}UpdateSignsDB/GetSignsDBVersionNum";
//                    HttpResponseMessage response = await client.GetAsync(url);

//                    if (response.IsSuccessStatusCode)
//                    {
//                        string json = await response.Content.ReadAsStringAsync();

//                        return JsonConvert.DeserializeObject<UpdateSignsDBDTO>(json);
//                    }

//                    return null;

//                }
//            }
//            catch
//            {
//                return null;
//            }

//        }

//        private async Task<AddMalwareHashsFlagDTO> GetAddMalwareHashsFlag(Guid Id)
//        {
//            try
//            {
//                using (HttpClient client = new HttpClient())
//                {
//                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                    string url = $"{MainURL}AddMalwareHashsFlag/{Id}";
//                    HttpResponseMessage response = await client.GetAsync(url);

//                    if (response.IsSuccessStatusCode)
//                    {
//                        string json = await response.Content.ReadAsStringAsync();
//                        APIResponse<AddMalwareHashsFlagDTO> apiResponse = JsonConvert.DeserializeObject<APIResponse<AddMalwareHashsFlagDTO>>(json);

//                        return apiResponse.Data;
//                    }

//                    return null;

//                }
//            }
//            catch
//            {
//                return null;
//            }

//        }

//        private async Task<List<AddMalwareHashsDTO>> GetAddMalwareHashs(Guid Id)
//        {
//            try
//            {
//                using (HttpClient client = new HttpClient())
//                {
//                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                    string url = $"{MainURL}AddMalwareHashs/{Id}";
//                    HttpResponseMessage response = await client.GetAsync(url);

//                    if (response.IsSuccessStatusCode)
//                    {
//                        string json = await response.Content.ReadAsStringAsync();
//                        APIResponse<List<AddMalwareHashsDTO>> apiResponse = JsonConvert.DeserializeObject<APIResponse<List<AddMalwareHashsDTO>>>(json);

//                        return apiResponse.Data;
//                    }

//                    return null;

//                }
//            }
//            catch
//            {
//                return null;
//            }

//        }

//        public async void PostAutomatedActions(BlockedProcessesDTO blockedProcesses)
//        {

//            string jsoneventViewer = JsonConvert.SerializeObject(blockedProcesses);

//            using (HttpClient client = new HttpClient())
//            {
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                var content = new StringContent(jsoneventViewer, Encoding.UTF8, "application/json");

//                HttpResponseMessage response = await client.PostAsync($"{MainURL}BlockedProcesses", content);

//                if (!response.IsSuccessStatusCode)
//                {
//                    await SendLogs("Send Automated Actions to cloude failed", "DeviceManagement class PostAutomatedActions method");
//                }
//            }
//        }



//        public async Task SyncFunction(Guid deviceId)
//        {
//            try
//            {
//                using var Ncontext = new NetworkContext();
//                using var blocked_WebsitesSQLite = new WebsitesContext();
//                using var DeviceActionsContext = new DeviceContext();
//                using var DeviceStatusContext = new DeviceStatusContext();
//                using var blockedServiceContext = new BlockedServicesContext();
//                using var exceptionWifiContext = new Exception_WifiContext();
//                using var blockedAppsContext = new BlockedAppsContext();
//                using var AppConfigAndLoginContext = new AppConfigAndLoginContext();
//                using var AppsVersionsContext = new Apps_VersionsContext();
//                using var InstalledAppsContext = new InstalledAppsContext();
//                using var logsContext = new iFreezeLogsContext();
//                using var baseUrlContext = new BaseURLContext();
//                using var ScreenShotAnalyserPolicyContext = new ScreenShotAnalyserPolicyContext();
//                using var UpdateSignsDBContext = new UpdateSignsDBContext();
//                using var SignsContext = new SignsContext();
//                using var ApplicationInfoContext = new ApplicationInfoContext();

//                var oldValIPWhitelist = DeviceActionsContext.DeviceActions.FirstOrDefault()?.ActivateWhitelist;
//                var deviceConfigurations = await GetDeviceConfigurationsFromAPI(deviceId);
//                var device = deviceConfigurations.Device;

//                await UpdateDeviceActions(device, DeviceActionsContext);
//                await SyncExceptionWifi(device,deviceConfigurations.ExceptionWifi, exceptionWifiContext);
//                await SyncKioskMode(device, deviceConfigurations.DeviceKioskApps, ApplicationInfoContext);
//                await UpdateDeviceSecurity(device, DeviceActionsContext, DeviceStatusContext, blockedServiceContext);
//                await SyncBlockedApps(deviceConfigurations, blockedAppsContext);
//                await SyncAllowedApps(deviceConfigurations, Ncontext);
//                await ControlWhitelistAppProcess(DeviceActionsContext);
//                await SyncWhiteListIPs(deviceConfigurations, oldValIPWhitelist, Ncontext, DeviceActionsContext, DeviceStatusContext);
//                await SyncBlockedIPs(deviceConfigurations, Ncontext);
//                await SyncBlockedWebsitesAndCategories(deviceConfigurations, blocked_WebsitesSQLite);
//                await SyncPassword(device.DevicePassword, AppConfigAndLoginContext);
//                await SyncVulnApps(deviceId, AppsVersionsContext);
//                await SyncAutoScan(DeviceActionsContext);
//                await SyncScreenShotPolicy(deviceConfigurations.DeviceDLPScanWord, ScreenShotAnalyserPolicyContext);
//                await SyncSilentInstall(deviceId, InstalledAppsContext);
//                await UpdateDeviceSerial(deviceId);
//                await SyncLogs(logsContext);
//                await SyncCloudURL(deviceId, baseUrlContext);
//                await SyncSignsDB(deviceId, UpdateSignsDBContext, SignsContext);
//                await CheckWindowsServiceUpdate();

//                // Save all changes at once (performance boost)
//                await Task.WhenAll(
//                    Ncontext.SaveChangesAsync(),
//                    blocked_WebsitesSQLite.SaveChangesAsync(),
//                    DeviceActionsContext.SaveChangesAsync(),
//                    DeviceStatusContext.SaveChangesAsync(),
//                    blockedServiceContext.SaveChangesAsync(),
//                    exceptionWifiContext.SaveChangesAsync(),
//                    blockedAppsContext.SaveChangesAsync(),
//                    AppConfigAndLoginContext.SaveChangesAsync(),
//                    AppsVersionsContext.SaveChangesAsync(),
//                    InstalledAppsContext.SaveChangesAsync(),
//                    logsContext.SaveChangesAsync(),
//                    baseUrlContext.SaveChangesAsync(),
//                    ScreenShotAnalyserPolicyContext.SaveChangesAsync(),
//                    SignsContext.SaveChangesAsync()
//                );

                 
//            }
//            catch (Exception ex)
//            {
//                await SendLogs(ex.Message, "DeviceManagement class SyncFunction method");
//            }
//        }

//        private async Task UpdateDeviceActions(DeviceDTO device, DeviceContext context)
//        {
//            var entity = context.DeviceActions.FirstOrDefault(d => d.Id == 1);
//            if (entity == null) throw new Exception("Device not found.");
//            entity.DeviceName = device.DeviceName;
//            entity.IsAdminDevice = device.IsAdminDevice;
//            entity.DisableUSB = device.DisableUSB;
//            entity.BlockUntrustedIPs = device.BlockUntrustedIPs;
//            entity.DisableTethering = device.DisableTethering;
//            entity.ActivateProactiveScan = device.ActivateProactiveScan;
//            entity.ActivateNetworkScan = device.ActivateNetworkScan;
//            entity.EnableUSBScan = device.EnableUSBScan;
//            entity.MuteMicrophone = device.MuteMicrophone;
//            entity.DisableCamera = device.DisableCamera;
//            entity.IsolateDevice = device.IsolateDevice;
//            entity.BlockPowerShell = device.BlockPowerShell;
//            entity.ActivateWhitelist = device.ActivateWhitelist;
//            entity.TamperProtection = device.TamperProtection;
//            entity.WhiteListWiFi = device.WhiteListWiFi;
//            entity.ActivateWhitelistApp = device.WhiteListApps;
//            entity.AutoScan = device.AutoScan;
//            entity.ScanAtSpecificHour = device.ScanAtSpecificHour;
//            entity.AutomatedRemediation = device.AutomatedRemediation;
//            entity.IsolationStartTime = device.IsolationStartTime;
//            entity.IsolationRestoreTime = device.IsolationRestoreTime;
//            entity.FastSyncN = device.FastSyncN;
//            entity.SyncN = device.SyncN;
//            entity.VulnAppCheck = device.VulnerabilityScan;
//            entity.Kiosk = device.Kiosk;
//            entity.DLP = device.DLP;
//            if (!string.IsNullOrEmpty(device.ScanTime)) entity.ScanTime = device.ScanTime;
//            entity.WhitelistWebsite = device.ActivateWhitelistWebsite;
             

//        }

//        private async Task UpdateDeviceSecurity(DeviceDTO device, DeviceContext deviceActionsContext, DeviceStatusContext deviceStatusContext, BlockedServicesContext blockedServiceContext)
//        {
//            var action = deviceActionsContext.DeviceActions.FirstOrDefault();
//            if (action == null) return;

//            var serviceControl = new WindowsServiceControl();

//            void KillProcesses(string name)
//            {
//                foreach (var p in Process.GetProcessesByName(name)) p.Kill();
//            }

//            bool IsProcessRunning(string name) => Process.GetProcessesByName(name).Length > 0;

//            // Network Scan
//            if (action.ActivateNetworkScan == "true")
//            {
//                if (!IsProcessRunning("i-FreezeNSPP")) Runner("i-FreezeNSPP.exe");
//            }
//            else
//            {
//                KillProcesses("i-FreezeNSPP");
//            }

//            // Proactive Scan
//            if (action.ActivateProactiveScan == "true")
//            {
//                await serviceControl.StartUpFreezeWRAndFreezeNM();

//                if (!IsProcessRunning("i-FreezeWK"))
//                {
//                    await Task.Run(() =>
//                    {
//                        var runner = new ExeRunner("i-FreezeWK.exe");
//                        runner.RunProcessForArabic();
//                    });
//                }
//            }
//            else
//            {
//                await serviceControl.ShoutDownFreezeWRAndFreezeNM();
//                KillProcesses("i-FreezeWK");
//            }

//            // USB Scan
//            if (action.EnableUSBScan == "true")
//            {
//                if (!IsProcessRunning("i-FreezeAutoUSBS"))
//                {
//                    var runner = new ExeRunner("i-FreezeAutoUSBS.exe", "i-FreezeUSBScan.exe");
//                    runner.RunProcessForArabic();
//                }
//            }
//            else
//            {
//                KillProcesses("i-FreezeAutoUSBS");
//            }

//            // Isolate Device
//            var isolateStatus = deviceStatusContext.DeviceAction.FirstOrDefault(x => x.DeviceName == "IsolateDevice");
//            if (isolateStatus != null)
//            {
//                if (action.IsolateDevice == "true" && isolateStatus.Action == 0)
//                    await serviceControl.DisableNetwork();

//                else if (action.IsolateDevice == "false" && isolateStatus.Action == 1)
//                    await serviceControl.EnableNetwork();
//            }
             


//            // Camera Control
//            var cameraStatus = deviceStatusContext.DeviceAction.FirstOrDefault(x => x.DeviceName == "Camera");
//            if (cameraStatus != null)
//            {
//                if (action.DisableCamera == "true" && cameraStatus.Action == 0)
//                    await serviceControl.DisableCamera();

//                else if (action.DisableCamera == "false" && cameraStatus.Action == 1)
//                    await serviceControl.EnableCamera();
//            }
             


//            // Microphone Mute
//            try
//            {
//                var mic = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
//                mic.AudioEndpointVolume.Mute = (action.MuteMicrophone == "true");
//            }
//            catch { }

//            // Block PowerShell
//            var powershellBlocked = blockedServiceContext.Blocked_Services.Any(x => x.App_Name == "powershell");
//            if (action.BlockPowerShell == "true" && !powershellBlocked)
//            {
//                blockedServiceContext.Blocked_Services.Add(new Blocked_Services { App_Name = "powershell" });
//            }
//            else if (action.BlockPowerShell == "false" && powershellBlocked)
//            {
//                var psEntry = blockedServiceContext.Blocked_Services.FirstOrDefault(x => x.App_Name == "powershell");
//                if (psEntry != null) blockedServiceContext.Blocked_Services.Remove(psEntry);
//            }

             

//            // Disable USB
//            if (action.DisableUSB == "true")
//            {
//                KillProcesses("i-FreezeAutoUSBS");
//                if (!IsProcessRunning("Disable_Removable_Device"))
//                {
//                    try { await serviceControl.DisableUSB(); } catch { }
//                }
//            }
//            else
//            {
//                if (IsProcessRunning("Disable_Removable_Device"))
//                {
//                    try { await serviceControl.EnableUSB(); } catch { }
//                }
//            }

//            // Disable Tethering
//            if (action.DisableTethering == "true")
//            {
//                if (!IsProcessRunning("Disable_Ethernet_Tethering"))
//                {
//                    try { await serviceControl.DisableTethering(); } catch { }
//                }
//            }
//            else
//            {
//                if (IsProcessRunning("Disable_Ethernet_Tethering"))
//                {
//                    try { await serviceControl.EnableTethering(); } catch { }
//                }
//            }

//            // DLP (i-FreezeSA)
//            if (action.DLP == "true")
//            {
//                if (!IsProcessRunning("i-FreezeSA"))
//                {
//                    var runner = new ExeRunner("i-FreezeSA.exe");
//                    runner.runprocess();
//                }
//            }
//            else
//            {
//                KillProcesses("i-FreezeSA");
//            }

             

//        }

//        private async Task SyncExceptionWifi(DeviceDTO device, List<string> exceptionWifi, Exception_WifiContext context)
//        {
//            exceptionWifi ??= new List<string>();
//            var existing = context.Exception_Wifi.Select(x => x.Wifi_Name).ToHashSet();
//            foreach (var name in exceptionWifi)
//            {
//                if (!existing.Contains(name))
//                    context.Exception_Wifi.Add(new Exception_Wifi { Wifi_Name = name });
//            }
//            var toRemove = context.Exception_Wifi.Where(x => !exceptionWifi.Contains(x.Wifi_Name)).ToList();
//            context.Exception_Wifi.RemoveRange(toRemove);

//            if (device.WhiteListWiFi == "true")
//            {

//                Process[] pname = Process.GetProcessesByName("i-FreezeMacWhitelist");
//                if (pname.Length == 0)
//                {
//                    try
//                    {
//                        await new WindowsServiceControl().DisableWifi();

//                    }
//                    catch
//                    {

//                    }

//                }
//            }

//            if (device.WhiteListWiFi == "false")
//            {
//                Process[] pname = Process.GetProcessesByName("i-FreezeMacWhitelist");
//                if (pname.Length == 1)
//                {

//                    try
//                    {
//                        await new WindowsServiceControl().EnableWifi();

//                    }
//                    catch
//                    {

//                    }
//                }
//            }
             

//        }

//        private async Task SyncKioskMode(DeviceDTO device, List<string> kioskApps, ApplicationInfoContext context)
//        {
//            kioskApps ??= new List<string>();
//            var existingPaths = context.ApplicationInfo.Select(x => x.Path).ToHashSet();
//            foreach (var path in kioskApps)
//            {
//                if (!existingPaths.Contains(path))
//                {
//                    var name = Path.GetFileName(path);
//                    context.ApplicationInfo.Add(new ApplicationInfo { Path = path, Name = name, Icon = path });
//                }
//            }
//            var toRemove = context.ApplicationInfo.Where(x => !kioskApps.Contains(x.Path)).ToList();
//            context.ApplicationInfo.RemoveRange(toRemove);

//            if (device.Kiosk == "true")
//            {
//                try
//                {
//                    var mainWindow = (MainWindow)Application.Current.MainWindow;
//                    mainWindow.WindowState = WindowState.Maximized;
//                    mainWindow.Topmost = true;
//                    mainWindow.Navigate(new KioskMode());
//                }
//                catch (Exception ex)
//                {
//                    await SendLogs(ex.Message, "Kiosk mode activation failed");
//                }
//            }
//            else if (App.IsKioskModeOpen)
//            {
//                try
//                {
//                    var mainWindow = (MainWindow)Application.Current.MainWindow;
//                    mainWindow.WindowState = WindowState.Normal;
//                    mainWindow.Topmost = false;
//                    GlobalKeyboardHook.EnableTaskManager();
//                    await new KioskMode().returnClosed();
//                    App.IsKioskModeOpen = false;
//                    mainWindow.Navigate(new MainWindow());
//                }
//                catch (Exception ex)
//                {
//                    await SendLogs(ex.Message, "Kiosk mode exit failed");
//                }
//            }
//        }

//        private async Task SyncBlockedApps(DeviceConfigurationsDTO config, BlockedAppsContext context)
//        {
//            var blockedApps = config.BlockedApps ?? new List<string>();
//            var existingApps = context.Blocked_App.Select(a => a.App_Name).ToHashSet();

//            // Add new apps
//            foreach (var app in blockedApps)
//            {
//                if (!string.IsNullOrWhiteSpace(app) && !existingApps.Contains(app))
//                {
//                    context.Blocked_App.Add(new Blocked_App { App_Name = app });
//                }
//            }

//            // Remove obsolete apps
//            var appsToRemove = context.Blocked_App.Where(a => !blockedApps.Contains(a.App_Name)).ToList();
//            if (appsToRemove.Any())
//            {
//                context.Blocked_App.RemoveRange(appsToRemove);
//            }
//        }

//        private async Task SyncAllowedApps(DeviceConfigurationsDTO config, NetworkContext context)
//        {
//            var allowedApps = config.ExceptionApps ?? new List<string>();
//            var existingAllowed = context.Allowed_App.Select(a => a.App_Name).ToHashSet();

//            foreach (var app in allowedApps)
//            {
//                if (!string.IsNullOrWhiteSpace(app) && !existingAllowed.Contains(app))
//                {
//                    context.Allowed_App.Add(new Allowed_App { App_Name = app });
//                }
//            }
//        }

//        private async Task ControlWhitelistAppProcess(DeviceContext deviceActionsContext)
//        {
//            var deviceAction = deviceActionsContext.DeviceActions.FirstOrDefault();
//            if (deviceAction == null) return;

//            if (deviceAction.ActivateWhitelistApp == "true")
//            {
//                if (!Process.GetProcessesByName("i-FreezePW").Any())
//                {
//                    new ExeRunner("i-FreezePW.exe").runprocess();
//                }

//                deviceAction.ActivateWhitelistApp = "true";
//            }
//            else
//            {
//                foreach (var p in Process.GetProcessesByName("i-FreezePW"))
//                {
//                    p.Kill();
//                }

//                deviceAction.ActivateWhitelistApp = "false";
//            }
             

//        }

//        private async Task SyncWhiteListIPs(DeviceConfigurationsDTO config, string oldVal, NetworkContext netCtx, DeviceContext deviceCtx, DeviceStatusContext statusCtx)
//        {
//            var allowedIps = config.ExceptionIps ?? new List<string>();
//            var whitelistToggle = deviceCtx.DeviceActions.FirstOrDefault()?.ActivateWhitelist;
             


//            var existingAllowed = netCtx.Allowed_IP.Select(i => i.IP).ToHashSet();
//            var blockedIps = netCtx.Firewall_blacklisted_IPs.Select(i => i.IP_Name).ToHashSet();

//            foreach (var ip in allowedIps)
//            {
//                if (!IsValidIPAddress(ip)) continue;

//                if (!existingAllowed.Contains(ip) && !blockedIps.Contains(ip))
//                {
//                    netCtx.Allowed_IP.Add(new Allowed_IP { IP = ip });
//                }
//                else if (blockedIps.Contains(ip) && whitelistToggle == "true")
//                {
//                    var blocked = netCtx.Firewall_blacklisted_IPs.FirstOrDefault(b => b.IP_Name == ip);
//                    if (blocked != null)
//                    {
//                        netCtx.Firewall_blacklisted_IPs.Remove(blocked);
//                        new ExeRunner("cmd.exe", $"route delete {ip}").runprocess();
//                        netCtx.Allowed_IP.Add(new Allowed_IP { IP = ip });

//                        var control = new WindowsServiceControl();
//                        await control.WhiteIPControlDisable();
//                        await Task.Delay(TimeSpan.FromSeconds(5));
//                        await control.WhiteIPControlEnable();
//                    }
//                }
//            }

//            var toRemove = netCtx.Allowed_IP.Where(i => !allowedIps.Contains(i.IP)).ToList();
//            netCtx.Allowed_IP.RemoveRange(toRemove);

//            var whiteListStatus = statusCtx.DeviceAction.FirstOrDefault(d => d.DeviceName == "WhiteList");
             

//            var user = deviceCtx.DeviceActions.FirstOrDefault();
             


//            if (whitelistToggle == "true")
//            {
//                if (whiteListStatus?.Action == 0)
//                {
//                    await new WindowsServiceControl().WhiteIPControlInstall();
//                    user.ActivateWhitelist = "true";
//                }
//                else if (whiteListStatus?.Action == 1 && oldVal == "false")
//                {
//                    await new WindowsServiceControl().WhiteIPControlEnable();
//                    user.ActivateWhitelist = "true";
//                }
//            }
//            else if (oldVal == "true" && whitelistToggle == "false")
//            {
//                await new WindowsServiceControl().WhiteIPControlUninstall();
//                user.ActivateWhitelist = "false";
//            }

             

//        }

//        private async Task SyncBlockedIPs(DeviceConfigurationsDTO config, NetworkContext context)
//        {
//            var blockedIps = config.BlockedIps ?? new List<string>();
//            var existing = context.Blocked_Ip.ToList();

//            var existingSet = existing.Select(b => b.RemoteIP).ToHashSet();

//            foreach (var ip in blockedIps)
//            {
//                if (!IsValidIPAddress(ip)) continue;

//                var existingIp = existing.FirstOrDefault(b => b.RemoteIP == ip);
//                if (existingIp == null)
//                {
//                    context.Blocked_Ip.Add(new Blocked_Ip { RemoteIP = ip, Blocked = 0, Deleted = 0 });
//                }
//                else if (existingIp.Deleted == 1)
//                {
//                    existingIp.Deleted = 0;
//                }
//            }

//            var toRemove = existing.Where(x => !blockedIps.Contains(x.RemoteIP)).ToList();
//            foreach (var ip in toRemove)
//            {
//                ip.Deleted = 1;
//            }

//            if (blockedIps.Any())
//                await new WindowsServiceControl().BlockIP();

//            if (toRemove.Any())
//                await new WindowsServiceControl().UnblockIP();
//        }

//        private async Task SyncBlockedWebsitesAndCategories(DeviceConfigurationsDTO config, WebsitesContext context)
//        {
//            var serviceControl = new WindowsServiceControl();

//            // -------- Blocked Websites --------
//            var blockedWebsites = config.BlockedWebsites ?? new List<string>();
//            var existingBlocked = context.Blocked_Websites.ToList();
//            var existingSet = existingBlocked.Select(w => w.WebSite_name).ToHashSet();
//            bool blockNeeded = false, unblockNeeded = false;

//            foreach (var site in blockedWebsites.Distinct())
//            {
//                if (string.IsNullOrWhiteSpace(site)) continue;

//                var match = existingBlocked.FirstOrDefault(x => x.WebSite_name == site);
//                if (match == null)
//                {
//                    context.Blocked_Websites.Add(new Blocked_Websites { WebSite_name = site, Blocked = 0, Deleted = 0 });
//                    blockNeeded = true;
//                }
//                else if (match.Deleted == 1)
//                {
//                    match.Deleted = 0;
//                    blockNeeded = true;
//                }
//            }

//            var toRemove = existingBlocked.Where(w => !blockedWebsites.Contains(w.WebSite_name)).ToList();
//            foreach (var item in toRemove)
//            {
//                item.Deleted = 1;
//                unblockNeeded = true;
//            }

//            if (blockNeeded) await serviceControl.BlockWebsites();
//            if (unblockNeeded) await serviceControl.UnblockWebsites();

//            // -------- White List Websites --------
//            var allowedWebsites = config.ExceptionWebsites ?? new List<string>();
//            var existingWhite = context.White_Websites.ToList();
//            var whiteSet = existingWhite.Select(w => w.websites_url).ToHashSet();

//            foreach (var site in allowedWebsites.Distinct())
//            {
//                if (!string.IsNullOrWhiteSpace(site) && !whiteSet.Contains(site))
//                {
//                    context.White_Websites.Add(new White_Websites { websites_url = site });
//                }
//            }

//            var toRemoveWhite = existingWhite.Where(w => !allowedWebsites.Contains(w.websites_url)).ToList();
//            context.White_Websites.RemoveRange(toRemoveWhite);

//            // -------- Website Categories (DeviceAction) --------
//            var selectedCategories = config.WebsiteCategories ?? new List<string>();
//            var deviceActions = context.DeviceAction.ToList();
//            var selectedSet = selectedCategories.ToHashSet();

//            foreach (var category in selectedSet)
//            {
//                var deviceEntry = deviceActions.FirstOrDefault(x => x.DeviceName == category);
//                if (deviceEntry != null && deviceEntry.Action == 0)
//                {
//                    switch (category)
//                    {
//                        case "Social Media": await serviceControl.BlockSocialMediaGroup(); break;
//                        case "Games": await serviceControl.BlockGamesGroup(); break;
//                        case "Adult": await serviceControl.BlockAdultGroup(); break;
//                        case "News": await serviceControl.BlockNewsGroup(); break;
//                        case "VPN": await serviceControl.BlockVPNGroup(); break;
//                    }
//                    deviceEntry.Action = 1;
//                }
//            }

//            // Unblock unselected categories
//            var toUnblock = deviceActions.Where(x => !selectedSet.Contains(x.DeviceName) && x.Action == 1).ToList();
//            foreach (var action in toUnblock)
//            {
//                switch (action.DeviceName)
//                {
//                    case "Social Media": await serviceControl.UnblockSocialMediaGroup(); break;
//                    case "Games": await serviceControl.UnblockGamesGroup(); break;
//                    case "Adult": await serviceControl.UnblockAdultGroup(); break;
//                    case "News": await serviceControl.UnblockNewsGroup(); break;
//                    case "VPN": await serviceControl.UnblockVPNGroup(); break;
//                }
//                action.Action = 0;
//            }
//        }

//        private async Task SyncPassword(string newPassword, AppConfigAndLoginContext context)
//        {
//            if (string.IsNullOrWhiteSpace(newPassword)) return;

//            var loginModule = context.LoginModule.FirstOrDefault(d => d.Id == 1);
//            if (loginModule != null && loginModule.Password != newPassword)
//            {
//                loginModule.Password = newPassword;
//            }
             

//        }

//        private async Task SyncVulnApps(Guid deviceId, Apps_VersionsContext context)
//        {
//            var vulnApps = await VulnApps(deviceId);
//            if (vulnApps == null || !vulnApps.Any()) return;

//            var existing = context.Apps_Versions.ToList();
//            var existingMap = existing.ToDictionary(x => x.AppName, x => x);

//            foreach (var app in vulnApps)
//            {
//                if (existingMap.TryGetValue(app.AppName, out var existingApp))
//                {
//                    existingApp.Version = app.Version;
//                }
//                else
//                {
//                    context.Apps_Versions.Add(new Apps_Versions
//                    {
//                        AppName = app.AppName,
//                        Version = app.Version
//                    });
//                }
//            }

//            var currentAppNames = vulnApps.Select(a => a.AppName).ToHashSet();
//            var toRemove = existing.Where(x => !currentAppNames.Contains(x.AppName)).ToList();
//            context.Apps_Versions.RemoveRange(toRemove);
//        }

//        private async Task SyncAutoScan(DeviceContext context)
//        {
//            var action = context.DeviceActions.FirstOrDefault();
//            if (action?.AutoScan == "false")
//            {
//                foreach (var process in Process.GetProcessesByName("i-FreezeFSSBG"))
//                {
//                    process.Kill();
//                }

//                action.AutoScan = "false";
//            }
             

//        }

//        private async Task SyncScreenShotPolicy(List<string> keywords, ScreenShotAnalyserPolicyContext context)
//        {
//            keywords ??= new List<string>();

//            var existingItems = context.ScreenShotAnalyserPolicy.ToList();
//            var existingSet = existingItems.Select(x => x.AnalysisName).ToHashSet();

//            // Add new keywords
//            foreach (var word in keywords.Distinct())
//            {
//                if (!string.IsNullOrWhiteSpace(word) && !existingSet.Contains(word))
//                {
//                    context.ScreenShotAnalyserPolicy.Add(new ScreenShotAnalyserPolicy { AnalysisName = word });
//                }
//            }

//            // Remove outdated keywords
//            var toRemove = existingItems.Where(x => !keywords.Contains(x.AnalysisName)).ToList();
//            context.ScreenShotAnalyserPolicy.RemoveRange(toRemove);
             

//        }

//        private async Task SyncSilentInstall(Guid deviceId, InstalledAppsContext context)
//        {
//            var installedEntry = context.InstalledApps.FirstOrDefault();
//            if (installedEntry == null) return;

//            decimal oldVersion = installedEntry.VersionNumber;
//            var versions = await GetVersionById(oldVersion, deviceId);
//            if (versions == null || !versions.Any()) return;

//            string[] appList = { "MicrosoftEdge.msi", "Zoom.msi", "Chrome.msi", "MicrosoftTeams.exe", "FireFox.exe" };

//            foreach (var version in versions)
//            {
//                if (version.DeviceId != deviceId || version.VersionType != "InstalledApps") continue;

//                if (!appList.Contains(version.VersionDescription)) continue;

//                if (version.Status != "Uninstall")
//                {
//                    await UpdateInstalledApps(oldVersion, version.VersionNumber, deviceId);
//                    await Task.Delay(TimeSpan.FromSeconds(5));
//                    await InstallApp(version.VersionDescription, version.Status);

//                    var log = new BlockedProcessesDTO
//                    {
//                        AppName = version.VersionDescription,
//                        DeviceId = deviceId,
//                        Description = $"{Path.GetFileNameWithoutExtension(version.VersionDescription)} has been updated successfully"
//                    };

//                    PostAutomatedActions(log);
//                }
//                else
//                {
//                    installedEntry.VersionNumber = version.VersionNumber;

//                    string installerPath = Path.Combine(AppContext.BaseDirectory, version.VersionDescription);
//                    if (File.Exists(installerPath))
//                    {
//                        try { File.Delete(installerPath); } catch { }
//                    }
//                }
//            }

//        }

//        private async Task SyncLogs(iFreezeLogsContext context)
//        {
//            var logs = context.iFreezeLogs.ToList();
//            if (!logs.Any()) return;

//            foreach (var log in logs)
//            {
//                bool sent = await SendLogs(log.Log, "DeviceManagement class Sync Sendlog");

//                if (sent)
//                {
//                    var toDelete = context.iFreezeLogs.FirstOrDefault(l => l.Id == log.Id);
//                    if (toDelete != null)
//                        context.iFreezeLogs.Remove(toDelete);
//                }
//            }
//        }

//        private async Task SyncCloudURL(Guid deviceId, BaseURLContext context)
//        {
//            var newUrl = await GetNewCloudURL(deviceId);
//            var oldUrl = context.CloudURL.FirstOrDefault();

//            if (newUrl != null && !string.IsNullOrWhiteSpace(newUrl.URL) && oldUrl != null && newUrl.URL != oldUrl.URL)
//            {
//                oldUrl.URL = newUrl.URL;
//                context.CloudURL.Update(oldUrl);
//            }
             

//        }

//        private async Task SyncSignsDB(Guid deviceId, UpdateSignsDBContext updateContext, SignsContext signsContext)
//        {
//            // Get new version info from API
//            var newDbVersion = await GetUpdateSignsDBVersion();
//            var currentDbVersion = updateContext.UpdateSignsDB.FirstOrDefault();

//            if (newDbVersion != null && currentDbVersion != null &&
//                newDbVersion.VersionNumber > currentDbVersion.VersionNumber)
//            {
//                await DownloadSignsDB(currentDbVersion.VersionNumber, newDbVersion.VersionNumber);
//                currentDbVersion.VersionNumber = newDbVersion.VersionNumber;
//            }

//            // Get malware hashes version flag from API
//            var newHashFlag = await GetAddMalwareHashsFlag(deviceId);
//            var existingFlag = updateContext.AddMalwareHashsFlag.FirstOrDefault();

//            if (newHashFlag != null && existingFlag != null &&
//                newHashFlag.MalwareHashsVersion > existingFlag.MalwareHashsVersion)
//            {
//                var hashes = await GetAddMalwareHashs(deviceId);

//                if (hashes != null && hashes.Any())
//                {
//                    foreach (var hash in hashes)
//                    {
//                        signsContext.Malware_hashs.Add(new Malware_hashs
//                        {
//                            hash = hash.Hash,
//                            sha1 = null,
//                            sha256 = null
//                        });
//                    }


//                    // Update malware hash flag version
//                    existingFlag.MalwareHashsVersion = newHashFlag.MalwareHashsVersion;
//                    updateContext.AddMalwareHashsFlag.Update(existingFlag);
//                }
//            }
//        }

//        private async Task CheckWindowsServiceUpdate()
//        {
//            string updatePath = @"C:\Users\Public\i-Freeze\UpdateFiles\WindowsService.exe";

//            if (File.Exists(updatePath))
//            {
//                try
//                {
//                    UpdateWindowsServiceForSync();
//                }
//                catch (Exception ex)
//                {
//                    await SendLogs(ex.Message, "Failed to update Windows Service in CheckWindowsServiceUpdate");
//                }
//            }
//        }





//        //private string GetLocalWindowsName()
//        //{
//        //    try
//        //    {
//        //        using (var searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
//        //        {
//        //            foreach (var obj in searcher.Get())
//        //            {
//        //                string caption = obj["Caption"].ToString();
//        //                if (caption.Contains("Windows 11")) return "Windows 11";
//        //                if (caption.Contains("Windows 10")) return "Windows 10";
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        MessageBox.Show("Error getting Windows name: " + ex.Message);
//        //    }
//        //    return "Unknown OS";
//        //}

//        //private string GetLocalWindowsVersion()
//        //{
//        //    try
//        //    {
//        //        using (var searcher = new ManagementObjectSearcher("SELECT Version FROM Win32_OperatingSystem"))
//        //        {
//        //            foreach (var obj in searcher.Get())
//        //            {
//        //                return obj["Version"].ToString();
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        MessageBox.Show("Error getting Windows version: " + ex.Message);
//        //    }
//        //    return "0.0";
//        //}

//        //private string GetWindowsBuildVersion()
//        //{
//        //    try
//        //    {
//        //        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
//        //        {
//        //            if (key != null)
//        //            {
//        //                string build = key.GetValue("CurrentBuild")?.ToString() ?? "0";
//        //                string buildRevision = key.GetValue("UBR")?.ToString() ?? "0";
//        //                return $"{build}.{buildRevision}"; // Example: "26100.3037"
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        MessageBox.Show("Error getting Windows build version: " + ex.Message);
//        //    }
//        //    return "0.0";
//        //}

//        //private bool IsVersionLower(string localVersion, string ExternalVersion)
//        //{
//        //    // Split version into major and minor numbers
//        //    var localParts = localVersion.Split('.').Select(int.Parse).ToArray();
//        //    var apiParts = ExternalVersion.Split('.').Select(int.Parse).ToArray();

//        //    // Compare major version first
//        //    if (localParts[0] < apiParts[0]) return true;
//        //    if (localParts[0] > apiParts[0]) return false;

//        //    // If major versions are equal, compare minor version
//        //    return localParts[1] < apiParts[1];
//        //}

//        public async Task InstallApp(string app, string status)
//        {
//            try
//            {
//                switch (app)
//                {
//                    case "MicrosoftEdge.msi":
//                        switch (status)
//                        {
//                            case "Install":
//                                await new WindowsServiceControl().InstallMicrosoftEdge();
//                                break;
//                            case "Uninstall":
//                                await new WindowsServiceControl().UnInstallMicrosoftEdge();
//                                break;
//                        }

//                        break;

//                    case "Zoom.msi":
//                        switch (status)
//                        {
//                            case "Install":
//                                await new WindowsServiceControl().InstallZoom();
//                                break;
//                            case "Uninstall":
//                                await new WindowsServiceControl().UnInstallZoom();
//                                break;
//                        }

//                        break;

//                    case "Chrome.msi":
//                        switch (status)
//                        {
//                            case "Install":
//                                await new WindowsServiceControl().InstallChrome();
//                                break;
//                            case "Uninstall":
//                                await new WindowsServiceControl().UnInstallChrome();
//                                break;
//                        }

//                        break;

//                    case "MicrosoftTeams.exe":
//                        switch (status)
//                        {
//                            case "Install":
//                                await new WindowsServiceControl().InstallMicrosoftTeams();
//                                break;
//                            case "Uninstall":
//                                await new WindowsServiceControl().UnInstallMicrosoftTeams();
//                                break;
//                        }
//                        break;

//                    case "FireFox.exe":
//                        switch (status)
//                        {
//                            case "Install":
//                                await new WindowsServiceControl().InstallFireFox();
//                                break;
//                            case "Uninstall":
//                                await new WindowsServiceControl().UnInstallFireFox();
//                                break;
//                        }
//                        break;

//                }

//            }
//            catch (Exception ex)
//            {
//                await SendLogs(ex.Message, "DeviceManagement class InstallApp method");
//            }
//        }

//        public async Task UpdateInstalledApps(decimal oldeversionNumber, decimal newVersionNum, Guid deviceId)
//        {

//            string apiBaseUrl = $"{MainURL}Versions";
//            var zipPath = @"C:\Users\Public\i-Freeze\InstalledAppsVersion.zip";
//            string extractPath = @"C:\Users\Public\i-Freeze\";
//            string versionType = "InstalledApps";


//            using (var httpClient = new HttpClient())
//            {
//                try
//                {

//                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                    HttpResponseMessage response = await httpClient.GetAsync($"{apiBaseUrl}/DownloadFileInstalledApps?deviceId={deviceId}&versionType={versionType}&versionNumber={oldeversionNumber}");

//                    if (!response.IsSuccessStatusCode)
//                    {

//                    }
//                    else
//                    {

//                        if (File.Exists(zipPath))
//                        {
//                            File.Delete(zipPath);
//                        }

//                        var bytes = await response.Content.ReadAsByteArrayAsync();

//                        File.WriteAllBytes(zipPath, bytes);

//                        ZipFile.ExtractToDirectory(zipPath, extractPath);

//                        using (InstalledAppsContext updateContext = new InstalledAppsContext())
//                        {
//                            var config = updateContext.InstalledApps.FirstOrDefault();
//                            if (config != null)
//                            {
//                                config.VersionNumber = newVersionNum;
//                                updateContext.SaveChanges();
//                            }
//                        }


//                    }

//                }
//                catch (Exception ex)
//                {
//                    await SendLogs(ex.Message, "DeviceManagement class UpdateInstalledApps method");
//                }
//            }

//        }

//        public async Task DownloadSignsDB(decimal oldeversionNumber, decimal newVersionNum)
//        {

//            string apiBaseUrl = $"{MainURL}UpdateSignsDB";
//            var zipPath = @"C:\Users\Public\i-Freeze\UpdateSignsDB.zip";
//            string extractPath = @"C:\Users\Public\i-Freeze\Databases\";


//            using (var httpClient = new HttpClient())
//            {
//                try
//                {

//                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await Token());

//                    HttpResponseMessage response = await httpClient.GetAsync($"{apiBaseUrl}/DownloadFileSignsDB?versionNumber={oldeversionNumber}");

//                    if (!response.IsSuccessStatusCode)
//                    {

//                    }
//                    else
//                    {

//                        if (File.Exists(zipPath))
//                        {
//                            File.Delete(zipPath);
//                        }

//                        var bytes = await response.Content.ReadAsByteArrayAsync();

//                        File.WriteAllBytes(zipPath, bytes);

//                        string SignsDBPath = Path.Combine(Directory.GetCurrentDirectory(), "Databases", "Signs.db");

//                        if (File.Exists(SignsDBPath))
//                        {
//                            File.Delete(SignsDBPath);
//                        }

//                        ZipFile.ExtractToDirectory(zipPath, extractPath);

//                        using (UpdateSignsDBContext updateContext = new UpdateSignsDBContext())
//                        {
//                            var config = updateContext.UpdateSignsDB.FirstOrDefault();
//                            if (config != null)
//                            {
//                                config.VersionNumber = newVersionNum;
//                                updateContext.SaveChanges();
//                            }
//                        }


//                    }

//                }
//                catch (Exception ex)
//                {
//                    await SendLogs(ex.Message, "DeviceManagement class UpdateInstalledApps method");
//                }
//            }

//        }

//        private bool IsValidIPAddress(string ipString)
//        {
//            return IPAddress.TryParse(ipString, out _);
//        }

      
//    }
//}
