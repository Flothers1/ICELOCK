using Infrastructure;
using System.Diagnostics;


namespace IceLockWorker
{
    public class DLPControl
    {
                string path = @"C:\Users\Public\Ice Lock";


        private const string DLP_CAFPath = @"C:\Users\Public\Ice Lock\DLP_CAF.exe";
        private const string DLP_CAFName = "DLP_CAF"; // Without the ".exe" part

        // DLP_SA
        private const string DLP_SAPath = @"C:\Users\Public\Ice Lock\DLP_SA.exe";
        private const string DLP_SAName = "DLP_SA"; // Without the ".exe" part

        // DLP_RCF
        private const string DLP_RCFPath = @"C:\Users\Public\Ice Lock\DLP_RCF.exe";
        private const string DLP_RCFName = "DLP_RCF"; // Without the ".exe" part
        // DLP_PM
        private const string DLP_PMPath = @"C:\Users\Public\Ice Lock\DLP_PM.exe";
        private const string DLP_PMName = "DLP_PM"; // Without the ".exe" part

        public async void RunDLP_CAF()
        {

            Process[] dLP_CAFName = Process.GetProcessesByName(DLP_CAFName);

            if (dLP_CAFName.Length == 0)
            {
                try
                {

                    await IFreezeService.Log($"DLP_CAF will run");
                    //ExeRunner runner = new ExeRunner("DLP_CAF.exe");
                    //runner.RunCommandAsAdmin($"cd /d {path} && DLP_CAF.exe");
                    ExeRunner runner = new ExeRunner(DLP_CAFPath);
                    runner.runprocess();
                    await IFreezeService.Log($"DLP_CAF is runnig");

                }
                catch (Exception ex)
                {
                    await IFreezeService.Log($"Failed to run  DLP_CAF: {ex.Message}");
                }
            }
            else
            {
                 await IFreezeService.Log("ActiveProactiveScan process is already running.");
            }
        }
        public async void RunDLP_SA()
        {
            Process[] dLP_SAName = Process.GetProcessesByName(DLP_SAName);

            if (dLP_SAName.Length == 0)
            {
                try
                {
                    await IFreezeService.Log($"DLP_SA will run");

                    //ExeRunner runner = new ExeRunner("DLP_SA.exe");
                    //runner.RunCommandAsAdmin($"cd /d {path} && DLP_SA.exe");

                    ExeRunner runner = new ExeRunner(DLP_SAPath);
                    runner.runprocess();
                    await IFreezeService.Log($"DLP_SA is runnig");

                }
                catch (Exception ex)
                {
                    await IFreezeService.Log($"Failed to run DLP_SA: {ex.Message}");
                }
            }
            else
            {
                //  await IFreezeService.Log("ActiveProactiveScan process is already running.");
            }
        }
        public async void RunDLP_RCF()
        {
            Process[] dLP_RCFName = Process.GetProcessesByName(DLP_RCFName);

            if (dLP_RCFName.Length == 0)
            {
                try
                {
                    await IFreezeService.Log($"DLP_RCF will run");

                    ExeRunner runner = new ExeRunner(DLP_RCFPath);
                    runner.runprocess();
                    await IFreezeService.Log($"DLP_RCF is runnig");

                }
                catch (Exception ex)
                {
                    await IFreezeService.Log($"Failed to run DLP_RCF: {ex.Message}");
                }
            }
            else
            {
                //  await IFreezeService.Log("ActiveProactiveScan process is already running.");
            }
        }
        public async void RunDLP_PM()
        {
            Process[] dLP_PMName = Process.GetProcessesByName(DLP_PMName);

            if (dLP_PMName.Length == 0)
            {
                try
                {
                    await IFreezeService.Log($"DLP_PM will run");

                    //ExeRunner runner = new ExeRunner("DLP_RCF.exe");
                    //runner.RunCommandAsAdmin($"cd /d {path} && DLP_RCF.exe");
                    ExeRunner runner = new ExeRunner(DLP_PMPath);
                    runner.runprocess();
                    await IFreezeService.Log($"DLP_PM is runnig");

                }
                catch (Exception ex)
                {
                    await IFreezeService.Log($"Failed to run DLP_PM: {ex.Message}");
                }
            }
            else
            {
                //  await IFreezeService.Log("ActiveProactiveScan process is already running.");
            }
        }
    }
}
