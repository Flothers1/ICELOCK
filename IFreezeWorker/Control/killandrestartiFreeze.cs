using Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceLockWorker.Control
{
    public class killandrestartiFreeze
    {

        private const string DisableExecutablePathifreeze = @"C:\Users\Public\Ice Lock\i-FreezeRestart.exe";
    //    private static readonly string DisableExecutablePathifreeze =
    //Path.Combine(MainProjectPath.ProjectPath, "i-FreezeRestart.exe");

        private const string DisableExecutableNameifreeze = "i-FreezeRestart"; // Without the ".exe" part
        public async void killandrestart()
        {
            Process[] ifreezename = Process.GetProcessesByName(DisableExecutableNameifreeze);

            if (ifreezename.Length == 0)
            {
                try
                {


                    ExeRunner runner3 = new ExeRunner(DisableExecutablePathifreeze);
                    await runner3.RunPiFreezeAsync();


                   // await IFreezeService.Log("Run i-FreezeRestart.exe.");
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

    }
}
