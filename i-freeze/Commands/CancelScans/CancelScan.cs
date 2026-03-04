using i_freeze.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.Commands.CancelScans
{
    class CancelScan : CommandBase
    {
        public override void Execute(object parameter)
        {
            foreach (var process in Process.GetProcessesByName("Standard_Scan_OfflineEnc.exe"))
            {
                process.Kill(); //Kill Process of Scan to stop it in background 
            }
            foreach (var process in Process.GetProcessesByName("DeepScanEnc.exe"))
            {
                process.Kill(); //Kill Process of Scan to stop it in background 
            }
            foreach (var process in Process.GetProcessesByName("Standard_Scan_OnlineEnc.exe"))
            {
                process.Kill(); //Kill Process of Scan to stop it in background 
            }
            // Environment.Exit(Environment.ExitCode);
            CloseWindowCommand.CloseWindow(); //Close this Messagebox
        }

    }
}
