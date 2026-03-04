using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class DeviceDTO
    {
        public string DeviceName { get; set; }
        public string IsAdminDevice { get; set; }
        public string OperatingSystemVersion { get; set; }
        public string DeviceIp { get; set; }
        public string MacAddress { get; set; }
        public string SerialNumber { get; set; }
        public string DisableUSB { get; set; }
        public string DisableTethering { get; set; }
        public string ActivateProactiveScan { get; set; }
        public string ActivateNetworkScan { get; set; }
        public string EnableUSBScan { get; set; }
        public string MuteMicrophone { get; set; }
        public string DisableCamera { get; set; }
        public string IsolateDevice { get; set; }
        public string Lockduration { get; set; }
      


        public string BlockPowerShell { get; set; }
        public string BlockUntrustedIPs { get; set; }
        public string ActivateWhitelist { get; set; }
        public string TamperProtection { get; set; }
        public string WhiteListWiFi { get; set; }
        public string WhiteListApps { get; set; }
        public string AutoScan { get; set; }
        public string ScanTime { get; set; }
        public string? ScanAtSpecificHour { get; set; }
        public string DevicePassword { get; set; }
        public string ActivateWhitelistWebsite { get; set; }
        public string DLP { get; set; }
        //public string ?AutomatedRemediation { get; set; }
        public string? performancemonitor { get; set; }

        public string ?AppManager { get; set; }

        public string? IsolationStartTime { get; set; }
        public string? IsolationRestoreTime { get; set; }
        public string ?Kiosk { get; set; }
        public string? SyncN { get; set; }
        public string? FastSyncN { get; set; }
        public string? FastSync { get; set; }
        public string? AdvancedRemediation { get; set; }
        public string? BasicRemediation { get; set; }

        public string? VulnerabilityScan { get; set; }

    }
}
