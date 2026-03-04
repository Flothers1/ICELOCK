using Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IceLockWorker.Control
{
    public class NSSMIFREEZEControl
    {
        private const string ServiceName = "Wfreeze";
        private const string ExecutablePath = @"C:\Users\Public\Ice Lock\Disable_Removable_Device.exe";
        //    private static readonly string ExecutablePath =
        //Path.Combine(MainProjectPath.ProjectPath, "Disable_Removable_Device.exe");

        public async void NSSMIFREEZE()
        {
          //  await IFreezeService.Log("IsServiceInstalled: " + IsServiceInstalled(ServiceName).ToString());

          //  await IFreezeService.Log("IsServiceRunning: " + IsServiceRunning(ServiceName).ToString());

            if (!IsServiceInstalled(ServiceName))
            {
                await InstallService();
            }

            if (!IsServiceRunning(ServiceName))
            {
                await StartService();
            }

         //   await IFreezeService.Log("Ensure NSSM service is installed and running.");
        }

        private async Task InstallService()
        {
            ExeRunner runner = new ExeRunner("nssm", $"install {ServiceName} \"{ExecutablePath}\"");
            runner.runprocess();


            //var installInfo = new ProcessStartInfo
            //{
            //    FileName = "nssm",
            //    Arguments = $"install {ServiceName} \"{ExecutablePath}\"",
            //    UseShellExecute = false,
            //    Verb = "runas",
            //    CreateNoWindow = true,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true
            //};

            //using var process = Process.Start(installInfo);

         //   await IFreezeService.Log("start InstallService");

            //if (process != null)
            //    await process.WaitForExitAsync();
        }

        private async Task StartService()
        {
            ExeRunner runner = new ExeRunner("nssm", $"start {ServiceName}");
            runner.runprocess();

            //var startInfo = new ProcessStartInfo
            //{
            //    FileName = "nssm",
            //    Arguments = $"start {ServiceName}",
            //    UseShellExecute = false,
            //    Verb = "runas",
            //    CreateNoWindow = true,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true
            //};

            //using var process = Process.Start(startInfo);

          //  await IFreezeService.Log("start StartService");

            //if (process != null)
            //    await process.WaitForExitAsync();
        }

        private bool IsServiceInstalled(string serviceName)
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == serviceName);
        }

        private bool IsServiceRunning(string serviceName)
        {
            try
            {
                var service = new ServiceController(serviceName);
                return service.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                return false;
            }
        }

    }
    }

