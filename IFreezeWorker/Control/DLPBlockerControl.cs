using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceLockWorker.Control
{
    public class DLPBlockerControl
    {
        // public entry that blocks all DLP processes
        public void BlockAllDlpProcesses()
        {
            BlockDlpCaf();
            BlockDlpSa();
            BlockDlpRcf();
            BlockDlpPm();
            BlockDecryptAll();
        }
        // Individual public methods — one per DLP process as you requested
        public void BlockDlpCaf() => KillProcessesByName("DLP_CAF");
        public void BlockDlpSa() => KillProcessesByName("DLP_SA");
        public void BlockDlpRcf() => KillProcessesByName("DLP_RCF");
        public void BlockDlpPm() => KillProcessesByName("DLP_PM");
        public void BlockDecryptAll() => KillProcessesByName("decryptor_all_dlp");
        private void KillProcessesByName(string processName)
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                try
                {
                    // optionally attempt graceful close first:
                    // if (!process.CloseMainWindow()) { process.Kill(); }
                    process.Kill();
                    // Optionally wait for exit (non-blocking if you prefer)
                    // process.WaitForExit(2000);

                    Debug.WriteLine($"Killed process {process.ProcessName} (Id={process.Id}).");
                    IFreezeService.Log($"Killed process {processName} (Id={process.Id}).");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to kill process {process.ProcessName} (Id={process.Id}): {ex.Message}");
                    IFreezeService.Log($"Failed to kill process {processName} (Id={process.Id}): {ex.Message}");
                }
            }
        }
    }
}
