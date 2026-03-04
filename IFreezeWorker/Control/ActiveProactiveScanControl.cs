using Infrastructure;
using System.Diagnostics;


namespace IceLockWorker
{
    public class ActiveProactiveScanControl
    {
        // i-freeze WR
        private const string DisableExecutablePathWR = @"C:\Users\Public\Ice Lock\i-FreezeWR.exe";
        private const string DisableExecutableNameWR = "i-FreezeWR"; // Without the ".exe" part

        // i-freeze NM
        private const string DisableExecutablePathNM = @"C:\Users\Public\Ice Lock\i-FreezeNM.exe";
        private const string DisableExecutableNameNM = "i-FreezeNM"; // Without the ".exe" part

        // i-freeze wk
        private const string DisableExecutablePathWK = @"C:\Users\Public\Ice Lock\i-FreezeWK.exe";
        private const string DisableExecutableNameWK = "i-FreezeWK"; // Without the ".exe" part

        //// i-freeze WR
        //private static readonly string DisableExecutablePathWR =
        //    Path.Combine(MainProjectPath.ProjectPath, "i-FreezeWR.exe");
        //private const string DisableExecutableNameWR = "i-FreezeWR"; // Without the ".exe" part

        //// i-freeze NM
        //private static readonly string DisableExecutablePathNM =
        //    Path.Combine(MainProjectPath.ProjectPath, "i-FreezeNM.exe");
        //private const string DisableExecutableNameNM = "i-FreezeNM"; // Without the ".exe" part

        //// i-freeze WK
        //private static readonly string DisableExecutablePathWK =
        //    Path.Combine(MainProjectPath.ProjectPath, "i-FreezeWK.exe");
        //private const string DisableExecutableNameWK = "i-FreezeWK"; // Without the ".exe" part

        public async void StartUpWRAndNM()
        {

            Process[] PWRname = Process.GetProcessesByName(DisableExecutableNameWR);
            Process[] PNMname = Process.GetProcessesByName(DisableExecutableNameNM);
            Process[] PWKname = Process.GetProcessesByName(DisableExecutableNameWK);

            if (PWRname.Length == 0 && PNMname.Length == 0 && PWKname.Length == 0)
            {
                try
                {
                    ExeRunner runner = new ExeRunner(DisableExecutablePathWR);
                    runner.runprocess();

                    //await RunnerAsAdmin(DisableExecutablePathWR);

                       await IFreezeService.Log("Run i-FreezeWR.exe_ActiveProactiveScanControl_Class.");

                    ExeRunner runner2 = new ExeRunner(DisableExecutablePathNM);
                    runner2.runprocess();

                   // await RunnerAsAdmin(DisableExecutablePathNM);

                     await IFreezeService.Log("Run i-FreezeNM.exe_ActiveProactiveScanControl_Class.");

                    ExeRunner runner3 = new ExeRunner(DisableExecutablePathWK);
                    runner3.runprocess();

                   // await RunnerAsAdmin(DisableExecutablePathNM);

                     await IFreezeService.Log("Run i-FreezeWK.exe_ActiveProactiveScanControl_Class.");
                }
                catch (Exception ex)
                {
                    await IFreezeService.Log($"Failed to run the Wifi executable: {ex.Message}");
                }
            }
            else
            {
                //  await IFreezeService.Log("ActiveProactiveScan process is already running.");
            }
        }

        public void ShutdownWRAndNM()
        {
            string[] processNames = { DisableExecutableNameWR, DisableExecutableNameNM, DisableExecutableNameWK };

            foreach (string processName in processNames)
            {
                foreach (var process in Process.GetProcessesByName(processName))
                {
                    process.Kill();
                }
            }
        }

        public async Task RunnerAsAdmin(string command)
        {
            try
            {
                string path = @"C:\Users\Public\Ice Lock";
                //string path = MainProjectPath.ProjectPath;


                ExeRunner runner = new ExeRunner(command);
                runner.RunCommandAsAdmin($"cd /d {path} && {command}");

                // await IFreezeService.Log("RunnerAsAdmin method command executed");

            }
            catch (Exception ex)
            {
                await IFreezeService.Log("Faild in RunnerAsAdmin method" + ex.Message);
            }
        }

    }
}
