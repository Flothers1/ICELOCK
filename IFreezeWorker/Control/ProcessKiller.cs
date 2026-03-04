
using Infrastructure;
using Infrastructure.DataContext;
using Infrastructure.Model;
using System.Diagnostics;
using System.Management;
using System.Text;


namespace IceLockWorker
{
    public class ProcessKiller
    {
        // i-FreezeBA
        private const string DisableExecutablePathBA = @"C:\Users\Public\Ice Lock\i-FreezeBA.exe";
            //    private static readonly string DisableExecutablePathBA =
            //Path.Combine(MainProjectPath.ProjectPath, "i-FreezeBA.exe");
        private const string DisableExecutableNameBA = "i-FreezeBA"; // Without the ".exe" part


        public async void KillAllBlockedProcesses()
        {

            Process[] PWRname = Process.GetProcessesByName(DisableExecutableNameBA);
            if (PWRname.Length == 0)
            {
                try
                {
                    ExeRunner runner = new ExeRunner(DisableExecutablePathBA);
                    runner.runprocess();

                    //await RunnerAsAdmin(DisableExecutablePathWR);

                   // await IFreezeService.Log("Run i-FreezeBA.exe_ActiveProactiveScanControl_Class.");


                }
                catch (Exception ex)
                {
                    await IFreezeService.Log($"Failed to run the Wifi executable: {ex.Message}");
                }
            }
            else
            {
               // await IFreezeService.Log(" i-FreezeBA process is already running.");
            }
        }

        public void UnBlockedProcesses()
        {
            string[] processNames = { DisableExecutableNameBA };

            foreach (string processName in processNames)
            {
                foreach (var process in Process.GetProcessesByName(processName))
                {
                    try
                    {
                       // IFreezeService.Log(" i-FreezeBA process is stopped");
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                        IFreezeService.Log($"Failed to kill process {processName}: {ex.Message}");
                    }
                }
            }
        }
        public void  BlockDLPProccess()
        {
                string[] processNames = { "DLP_CAF", "DLP_SA", "DLP_RCF", "DLP_PM" };

                foreach (string processName in processNames)
                {
                    foreach (var process in Process.GetProcessesByName(processName))
                    {
                        try
                        {
                            process.Kill();

                        }
                        catch (Exception ex)
                        {
                        
                            Debug.WriteLine($"Failed to dispose kill process {process}:" + ex.Message);
                            IFreezeService.Log($"Failed to kill process {processName}: {ex.Message}");


                        }

                    }

                }
        }
 
    }
}

