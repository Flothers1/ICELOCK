using i_freeze.DTOs;
using IceLockWorker.Control;
using Infrastructure.DataContext;
using Infrastructure.Model;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Timers;

namespace IceLockWorker
{
    public class IFreezeService : ServiceBase
    {
        private const int DLP_PM = 129;
        private const int BlockDLPProcess = 130;

   
        private const int Enable_Event_Viewer = 142;
        private const int Disable_Event_Viewer = 143;

        //private const int Block_Untrusted_IPs = 154;
        //private const int Unblock_Untrusted_IPs = 155;
     
        //Add
        private const int FreezeWR_And_FreezeNM_StartUp = 176;
        private const int FreezeWR_And_FreezeNM_ShoutDown = 177;
        private const int Block_AppsToggle = 181;
        //private const int NSSM_IFREEZEToggle = 182;
        private const int iFreeze_Update = 185;
        private const int DLP_CAF = 186;
        private const int DLP_SA = 187;
        private const int DLP_RCF = 188;
    




        //private IHost _host;
        private Thread killProcessThread;
        private volatile bool keepRunning = false;
       // private System.Timers.Timer timer;
        protected override void OnStart(string[] args)
        {
            try
            {
                Log("10000ms");

                Thread.Sleep(10000);
                #region SyncSignsDB
                try
                {
                   // Log("SyncSignsDB region: Start");

                    using var configContext = new AppConfigAndLoginContext();

                    if (configContext.Application_Configuration == null)
                    {
                    //    Log("SyncSignsDB region: Application_Configuration DbSet is null!");
                    }
                    else
                    {
                        var config = configContext.Application_Configuration.FirstOrDefault();

                          //  Log($"SyncSignsDB region: DeviceSN = {config.DeviceSN}");

                            Guid deviceId = config.DeviceSN;
                          //  Log($"SyncSignsDB region: Device ID = {deviceId}");

                        
                    }

                    Thread.Sleep(5000);                  
                    // Log("SyncSignsDB region: Finished, waited 5 seconds");
                }
                catch (Exception ex)
                {
                    Log("SyncSignsDB region: Exception occurred - " + ex.Message);
                    Log("SyncSignsDB region: StackTrace - " + ex.StackTrace);
                }
                #endregion
                Log("Run DLP_PM");
                RunDLP_PM();

                //RunDLP_CAF();
                //Log("0");
                //RunDLP_CAF();
                //Log("1");
                //RunDLP_RCF();
                //Log("2");
                //RunDLP_SA();
                //Log("3");

                Log("OnStart invoked");

                string filePath = @"C:\Users\Public\Ice Lock\Logs\log.txt";
                //string filePath = Path.Combine(MainProjectPath.ProjectPath, "Logs", "log.txt");

                long maxSizeBytes = 5 * 1024 * 1024; // 5MB maximum size
                    if (System.IO.File.Exists(filePath))
                    {
                        FileInfo fileInfo = new FileInfo(filePath);

                        if (fileInfo.Length > maxSizeBytes)
                        {
                            // Clear file contents
                            System.IO.File.WriteAllText(filePath, string.Empty);

                        }
                       
                    }
                 string targetPath = @"C:\Windows\System32\MixSheld.ico";
                string sourcePath = @"C:\Users\Public\Ice Lock\MixSheld.ico";
              //  string sourcePath = Path.Combine(MainProjectPath.ProjectPath, "MixSheld.ico");



                if (!System.IO.File.Exists(targetPath))
                    {
                        if (System.IO.File.Exists(sourcePath))
                        {
                            System.IO.File.Copy(sourcePath, targetPath);
                        }
                }


                    //Log("Calling EnableEventViewer...");
                    EnableEventViewer();
                
                   // Log("Calling BlockedAppToogle...");
                    BlockedAppToogle();

                  // Log("Starting ifreezestart");
                   //iFreezestart();

                var RunProcessWithoutkillProcessThread = new Thread(RunInfinityExeWithoutKillProcess);
                    RunProcessWithoutkillProcessThread.Start();

                    
            }
            catch (Exception ex)
            {
                     Log("Error in OnStart: " + ex.Message);
            }
        }


        protected override async void OnStop()
        {
            //killProcessThread?.Abort();
            // await Log("Service stopped.");
            //_host?.StopAsync(TimeSpan.FromSeconds(5)).Wait();
            //_host?.Dispose(); // Cleanup resources

            //if (timer != null)
            //{
            //    timer.Stop();
            //    timer.Dispose();
            //}
        }



        protected override async void OnCustomCommand(int command)
        {
            try
            {
                switch (command)
                {
                    case DLP_CAF:
                        RunDLP_CAF();
                        break;
                    case DLP_RCF:
                        RunDLP_RCF();
                        break;
                    case DLP_SA:
                        RunDLP_SA();
                        break;
                    case DLP_PM:
                        RunDLP_PM();
                        break;
                    case FreezeWR_And_FreezeNM_StartUp:
                        FreezeWRAndFreezeNMStartUp();
                        break;
                    case FreezeWR_And_FreezeNM_ShoutDown:
                        FreezeWRAndFreezeNMShoutDown();
                        break;
              
                    case Enable_Event_Viewer:
                        EnableEventViewer();
                        break;
                    case Disable_Event_Viewer:
                        DisableEventViewer();
                        break;
                
                    case Block_AppsToggle:
                        BlockedAppToogle(); 
                        break;
                    case BlockDLPProcess:
                        BlockDLPProcesses();
                        break;
                    //case NSSM_IFREEZEToggle:
                    //    NSSMIFREEZEToggle();
                    //    break;

                    case iFreeze_Update:
                        iFreezeUpdateToggle();
                        break;
              
                        // other cases...
                }
            }
            catch (Exception ex)
            {
               await Log($"Error occurred executing command {command}: {ex.Message}");
            }
        }
        private void RunDLP_CAF()
        {
            new DLPControl().RunDLP_CAF();
        }

        private void RunDLP_PM()
        {
            new DLPControl().RunDLP_PM();
        }
        private void RunDLP_SA()
        {
            new DLPControl().RunDLP_SA();
        }
        private void RunDLP_RCF()
        {
            new DLPControl().RunDLP_RCF();
        }
        private void FreezeWRAndFreezeNMStartUp()
        {
            new ActiveProactiveScanControl().StartUpWRAndNM();
        }

        private void FreezeWRAndFreezeNMShoutDown()
        {
            new ActiveProactiveScanControl().ShutdownWRAndNM();
        }

 
        private void DisableEventViewer()
        {
            new EventViewerManager().DisableEventViewer();
        }

        private void EnableEventViewer()
        {
            new EventViewerManager().EnableEventViewer();
        }

    

        // ifreeze browser
        //public void InstallIfreezeBrowser()
        //{
        //    new IFreezeBrwoserControl().InstallIfreezeBrowserDevice();
        //}

        public void InstallLibraryBrowser()
        {
            new IFreezeBrwoserControl().InstallLibraryBrowserDevice();
        }


        //public void InstallJavaRemoteDesktopDevice()
        //{
        //    new InstallJavaRemoteDesktopControl().InstallRemoteDesktop();
        //}

        // IntalledApss Methods

        // MicrosoftEdge
      
        public void killandrestartiFreezeToggle()
        {
            new killandrestartiFreeze().killandrestart();
        }

        public void iFreezeUpdateToggle()
        {
              new iFreezeUpdateControl().StartiFreezeUpdate();
        }








        #region BlockedApp


        public async void BlockedAppToogle()
        {
            //await Log("startBlockedApp i-FreezeBA");

            try
            {
                //await Log("Creating DeviceContext...");
                DeviceContext deviceContext = new DeviceContext();

               // await Log("Querying first DeviceAction...");
                var getBlockedAppsVlue = deviceContext.DeviceActions.FirstOrDefault();

                if (getBlockedAppsVlue == null)
                {
                  //  await Log("No DeviceAction found (null).");
                    return;
                }

              //  await Log($"AppManager value: {getBlockedAppsVlue.AppManager}");

                if (getBlockedAppsVlue.AppManager == "true")
                {
                  //  await Log("AppManager is true → Unblocking & then killing blocked processes...");

                    new ProcessKiller().UnBlockedProcesses();
                   // await Log("Called UnBlockedProcesses()");

                    await Task.Delay(2000);
                   // await Log("Waited 2 seconds");

                    new ProcessKiller().KillAllBlockedProcesses();
                   // await Log("Called KillAllBlockedProcesses()");
                }
                else
                {
                   // await Log("AppManager is not true → only unblocking...");
                    new ProcessKiller().UnBlockedProcesses();
                   // await Log("Called UnBlockedProcesses()");
                }
            }
            catch (Exception ex)
            {
                await Log("Error in BlockedAppToogle: " + ex.Message);
            }
        }

        public async void BlockDLPProcesses()
        {
            Log("start BlockDLPProcesses");
            try
            {
                new ProcessKiller().BlockDLPProccess();
            }
            catch (Exception ex)
            {
                await Log("Error in KillAllBlockedProcesses: " + ex.Message);
            }
        }

        //public async void UnBlockedAppToogle()
        //{
        //    await Log("StopBlockedApp i-FreezeBA");


        //    DeviceContext deviceContext = new DeviceContext();
        //    var getBlockedAppsVlue = deviceContext.DeviceActions.FirstOrDefault();

        //    if (getBlockedAppsVlue.AppManager == "true")
        //    {
        //        new ProcessKiller().UnBlockedProcesses();

        //        await Task.Delay(2000);

        //        new ProcessKiller().KillAllBlockedProcesses();
        //    }
        //    else
        //    {
        //        getBlockedAppsVlue.AppManager = "false";
        //        deviceContext.SaveChanges();

        //        new ProcessKiller().UnBlockedProcesses();
        //    }
        //}


        //private Task<int> GetBlockedToggleValueFromDB()
        //{
        //    //DeviceStatusContext deviceStatusContext = new DeviceStatusContext();

        //    //var getBlockedAppsVlue = deviceStatusContext.DeviceAction.FirstOrDefault(a => a.DeviceName == "BlockedToggle");
        //    DeviceContext deviceContext = new DeviceContext();
        //    var getBlockedAppsVlue = deviceContext.DeviceActions.FirstOrDefault();


        //    if (getBlockedAppsVlue != null)
        //        return Task.FromResult(getBlockedAppsVlue);
        //    else
        //        return Task.FromResult(0);

        //}

        #endregion

        //private void RunInfiniteKillProcessLoop()
        //{
        //    //while (keepRunning)
        //    //{
        //        new ProcessKiller().KillAllBlockedProcesses();
        //        //Thread.Sleep(1000);
        //    //}
        //}


        private async void RunInfinityExeWithoutKillProcess()
        {
            try
            {
                //await Log("RunInfinityExeWithoutKillProcess started");

                while (true)
                {
                  

                   // await Log("Creating DeviceContext...");
                    DeviceContext deviceContext = new DeviceContext();

                   // await Log("Querying first DeviceActions record...");
                    var action = deviceContext.DeviceActions.FirstOrDefault();

       

                //    await Log("Sleeping for 100 ms...");
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                await Log("catch error: " + ex.Message);
            }
            finally
            {
               // await Log("RunInfinityExeWithoutKillProcess exited (shouldn't happen in infinite loop)");
            }
        }




        private const string DisableExecutablePath = @"C:\Users\Public\Ice Lock\i-FreezeStart.exe";
    //    private static readonly string DisableExecutablePath =
    //Path.Combine(MainProjectPath.ProjectPath, "i-FreezeStart.exe");
        private const string DisableExecutableName = "i-FreezeStart"; // Without the ".exe" part
        public async void iFreezestart()
        {
            await IFreezeService.Log($"iFreezestart");


            Process[] Kname = Process.GetProcessesByName(DisableExecutableName);

            if ( Kname.Length == 0)
            {
                try
                {

                    ExeRunner runner3 = new ExeRunner(DisableExecutablePath);
                    runner3.runprocess();
                }
                catch (Exception ex)
                {
                    await IFreezeService.Log($"Failed to run the Wifi executable: {ex.Message}");
                }
            }
            
        }



        #region SyncSignsDB

        public class APIResponse<T>
        {
            public T Data { get; set; }
            public int Status { get; set; }
            public string Message { get; set; }
        }

        public static string MainURL => GetMainURL();
        private static string GetMainURL()
        {
            using (var baseURLContext = new BaseURLContext())
            {
                var url = baseURLContext.CloudURL.FirstOrDefault()?.URL;
                // Log("MainURL: Retrieved base URL = " + (url ?? "default URL"));
                // return url ?? "https://security.flothers.com:8443/api/";
                return url ?? "https://central.flothers.com:8443/api/";
            }
        }


  
    
   
   
        #endregion




        public static async Task Log(string message)
        {
            string filePath = "C:\\Users\\Public\\Ice Lock\\Logs\\log.txt"; // Log file path

            try
            {
                using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (var sw = new StreamWriter(fs))
                {
                    await sw.WriteLineAsync($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                // Consider logging errors to a separate file to avoid recursive failure
                string errorFilePath = "C:\\Users\\Public\\Ice Lock\\Logs\\error_log.txt";
                try
                {
                    using (var fs = new FileStream(errorFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (var sw = new StreamWriter(fs))
                    {
                        await sw.WriteLineAsync($"{DateTime.Now}: Error logging message - {ex.Message}");
                    }
                }
                catch
                {
                    // If logging fails, avoid an infinite loop
                }
            }
        }


       
       




    }
}
