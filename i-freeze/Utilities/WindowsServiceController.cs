using ControlzEx.Standard;
using i_freeze.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace i_freeze.Utilities
{
    class WindowsServiceControl
    {
        string serviceName;
        private const string ServiceName = "IceLock"; // Your service's name
        public enum Command
        {
            DLP_PM = 129,

            BlockProcess = 130,
            BlockDLP_CAF = 131,
            BlockDLP_SA = 132,
            BlockDLP_RCF = 133,
            BlockDecrypt_All = 134,
        //Add
        FreezeWRAndFreezeNMStartUp = 176,
            FreezeWRAndFreezeNMShoutDown = 177,
            BlockVPNGroup = 178,
            UnblockVPNGroup = 179,
            WindowsUpdate = 180,
            BlockedAppsToggle = 181,
            //NSSMIFREEZE = 182
            NetworkScanToggleStartUp = 182,
            NetworkScanToggleShoutDown = 183,
            killandrestartiFreeze = 184,
            iFreezeUpdate = 185,
            DLP_CAF = 186,
            DLP_SA = 187,
            DLP_RCF = 188,
            Decrypt_All = 189



        }


        public WindowsServiceControl(string serviceName)
        {
            this.serviceName = serviceName;
        }
        public WindowsServiceControl()
        {

        }
        public async Task StartWindowsServiceAdminAsync()
        {
            await RunServiceCommandAsync("start");
        }

        public async Task StopWindowsServiceAdminAsync()
        {
            await RunServiceCommandAsync("stop");
        }

        public async Task RestartWindowsServiceAsync()
        {
            try
            {
                await StopWindowsServiceAdminAsync();
                await Task.Delay(5000); // Delay for 5 seconds
                await StartWindowsServiceAdminAsync();
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "WindowsServiceControl class");
            }
        }
        private async Task RunServiceCommandAsync(string command)
        {
            try
            {
                string cwd = Directory.GetCurrentDirectory();
                var p = new ProcessStartInfo("net.exe", $"{command} {ServiceName}")
                {
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    WorkingDirectory = cwd
                };
                using (var process = Process.Start(p))
                {
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                await DeviceManagement.SendLogs(ex.Message, "WindowsServiceControl class");
            }
        }
   
     
        public async Task BlockVPNGroup()
        {
            await SendCommandToService(Command.BlockVPNGroup);
        }

        public async Task UnblockVPNGroup()
        {
            await SendCommandToService(Command.UnblockVPNGroup);
        }

    
        //--------------------------------------------------------------------------------
        //Add
        public async Task StartUpFreezeWRAndFreezeNM()
        {
            await SendCommandToService(Command.FreezeWRAndFreezeNMStartUp);
        }
        public async Task ShoutDownFreezeWRAndFreezeNM()
        {
            await SendCommandToService(Command.FreezeWRAndFreezeNMShoutDown);
        }

        public async Task WindowsUpdate_FreezeWU()
        {
            await SendCommandToService(Command.WindowsUpdate);
        }


        public async Task BlockedAppToggle()
        {
            await SendCommandToService(Command.BlockedAppsToggle);
        }

        //public async Task NSSMIFREEZE()
        //{
        //    await SendCommandToService(Command.NSSMIFREEZE);
        //}

        public async Task NetworkScanToggleStartUp()
        {
            await SendCommandToService(Command.NetworkScanToggleStartUp);
        }

        public async Task NetworkScanToggleShoutDown()
        {
            await SendCommandToService(Command.NetworkScanToggleShoutDown);
        }
        public async Task killandrestartiFreezeToggle()
        {
            await SendCommandToService(Command.killandrestartiFreeze);
        }

        public async Task iFreezeUpdateToggle()
        {
            await SendCommandToService(Command.iFreezeUpdate);
        }
     
        public async Task RunDLP_CAF()
        {
            await SendCommandToService(Command.DLP_CAF);
        }
        public async Task RunDLP_SA()
        {
            await SendCommandToService(Command.DLP_SA);
        }
        public async Task RunDLP_RCF()
        {
            await SendCommandToService(Command.DLP_RCF);
        }
        public async Task RunDLP_PM()
        {
            await SendCommandToService(Command.DLP_PM);
        }
        public async Task RunDecryptAll()
        {
            await SendCommandToService(Command.Decrypt_All);
        }
        public async Task BlockDLPProcess()
        {
            await SendCommandToService(Command.BlockProcess);
        }
        public async Task BlockDLP_CAFProcess()
        {
            await SendCommandToService(Command.BlockDLP_CAF);
        }
        public async Task BlockDLP_SAProcess()
        {
            await SendCommandToService(Command.BlockDLP_SA);
        }
        public async Task BlockDecrypt_AllProcess()
        {
            await SendCommandToService(Command.BlockDecrypt_All);
        }
        public async Task BlockDLP_RCFProcess()
        {
            await SendCommandToService(Command.BlockDLP_RCF);
        }

        // Helper method to send a command to the service
        private async Task SendCommandToService(Command command)
        {
            string targetService = string.IsNullOrWhiteSpace(this.serviceName) ? ServiceName : this.serviceName;
            try
            {
                using (var service = new ServiceController(targetService))
                {
                    service.Refresh();
                    if (service.Status == ServiceControllerStatus.Stopped ||
                        service.Status == ServiceControllerStatus.Paused)
                    {
                        try
                        {
                            service.Start();
                        }
                        catch (Exception startEx)
                        {
                            await DeviceManagement.SendLogs($"Failed to start service '{targetService}': {startEx.Message}", "WindowsServiceControl");
                            throw;
                        }
                    
                        // Wait for it to become Running (await Task.Run to avoid blocking threadpool)
                        try
                        {
                            await Task.Run(() => service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30)));
                            service.Refresh();
                        }
                        catch (System.ServiceProcess.TimeoutException)
                        {
                            await DeviceManagement.SendLogs($"Timeout waiting for service '{targetService}' to start.", "WindowsServiceControl");
                            throw;
                        }
                    }
                    else if (service.Status == ServiceControllerStatus.StartPending ||
                     service.Status == ServiceControllerStatus.StopPending)
                    {
                        // Wait for the service to settle to a stable state
                        try
                        {
                            await Task.Run(() => service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30)));
                            service.Refresh();
                        }
                        catch (System.ServiceProcess.TimeoutException)
                        {
                            await DeviceManagement.SendLogs($"Timeout waiting for service '{targetService}' to settle.", "WindowsServiceControl");
                            throw;
                        }
                    }
                    // final check
                    if (service.Status != ServiceControllerStatus.Running)
                    {
                        var msg = $"Service '{targetService}' is not running (status={service.Status}). Cannot send custom command.";
                        await DeviceManagement.SendLogs(msg, "WindowsServiceControl");
                        throw new InvalidOperationException(msg);
                    }
                    // Execute the custom command
                    try
                    {
                        service.ExecuteCommand((int)command);
                    }
                    catch (System.ComponentModel.Win32Exception w32)
                    {
                        await DeviceManagement.SendLogs($"Win32Exception when sending command {(int)command} to '{targetService}': {w32.Message}", "WindowsServiceControl");
                        throw;
                    }
                    catch (InvalidOperationException ioex)
                    {
                        await DeviceManagement.SendLogs($"InvalidOperationException when sending command {(int)command} to '{targetService}': {ioex.Message}", "WindowsServiceControl");
                        throw;
                    }
                    // small delay to avoid overlapping commands (optional)
                    await Task.Delay(500);
                    service.ExecuteCommand((int)command);

                    await Task.Delay(1000); // Ensure commands do not overlap

                    await Task.Run(() => service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30)));
                }
            }
            catch (Exception ex)
            {
                // top-level logging (already logged above where relevant)
                await DeviceManagement.SendLogs($"SendCommandToService failed for {targetService}: {ex.Message}", "WindowsServiceControl");
                throw;
            }
        }

        //private async Task SendCommandToService(Command command)
        //{
        //    try
        //    {
        //        using (var service = new ServiceController(ServiceName))
        //        {
        //            service.ExecuteCommand((int)command);

        //            await Task.Run(() => service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30)));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        await DeviceManagement.SendLogs(ex.Message, "WindowsServiceControl class");
        //    }

        //}

    }
}
