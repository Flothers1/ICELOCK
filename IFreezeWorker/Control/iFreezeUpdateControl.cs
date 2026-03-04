using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceLockWorker.Control
{
    public class iFreezeUpdateControl
    {
        // i-FreezeUpdate
        private const string DisableExecutablePathiFreeze = @"C:\Users\Public\Ice Lock\i-FreezeUpdate2.exe";
        
        private const string DisableExecutableNameiFreeze = "i-FreezeUpdate2"; // Without the ".exe" part
        public async void StartiFreezeUpdate()
        {
            Process[] iFreezename = Process.GetProcessesByName(DisableExecutableNameiFreeze);

            if (iFreezename.Length == 0)
            {
                try
                {

                    ExeRunner runner3 = new ExeRunner(DisableExecutablePathiFreeze);
                    runner3.runprocess();


                    await IFreezeService.Log("Run i-FreezeUpdate.exe_ActiveProactiveScanControl_Class.");
                }
                catch (Exception ex)
                {
                    await IFreezeService.Log($"Failed to run the Wifi executable: {ex.Message}");
                }
            }
            else
            {
               await IFreezeService.Log("i-FreezeUpdate process is already running.");
            }
        }
    }
}
