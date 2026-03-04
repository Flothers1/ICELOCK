using Infrastructure.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    [Table("DeviceActions")]

    public class DeviceActions
    {

        [Key]
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "DeviceName must be between 3 and 100 characters long.")]
        public string? DeviceName { get; set; }

        public string? IsAdminDevice { get; set; }


        [StringLength(50, MinimumLength = 3, ErrorMessage = "OperatingSystemVersion must be between 3 and 50 characters long.")]
        public string? OperatingSystemVersion { get; set; }


        [ValidIPAddress(ErrorMessage = "DeviceIp must be a valid IP address.")]
        public string? DeviceIp { get; set; }


        [StringLength(20, MinimumLength = 3, ErrorMessage = "MacAddress must be between 3 and 20 characters long.")]
        public string? MacAddress { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "SerialNumber must be between 3 and 20 characters long.")]
        public string? SerialNumber { get; set; }


        [StringLength(5, MinimumLength = 4, ErrorMessage = "Disable USB must be between 4 and 5 characters long.")]
        public string? DisableUSB { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "ActivateProactiveScan must be between 4 and 5 characters long.")]
        public string? ActivateProactiveScan { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "Disable USB must be between 4 and 5 characters long.")]
        public string? ActivateNetworkScan { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "EnableUSBScan must be between 4 and 5 characters long.")]
        public string? EnableUSBScan { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "MuteMicrophone must be between 4 and 5 characters long.")]
        public string? MuteMicrophone { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "DisableCamera must be between 4 and 5 characters long.")]
        public string? DisableCamera { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "IsolateDevice must be between 4 and 5 characters long.")]
        public string? IsolateDevice { get; set; }
        [StringLength(5, MinimumLength = 4, ErrorMessage = "Lockduration must be between 4 and 5 characters long.")]
        public string? Lockduration { get; set; }
      

        [StringLength(5, MinimumLength = 4, ErrorMessage = "BlockPowerShell must be between 4 and 5 characters long.")]
        public string? BlockPowerShell { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "DisableTethering must be between 4 and 5 characters long.")]
        public string? DisableTethering { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "BlockUntrustedIPs must be between 4 and 5 characters long.")]
        public string? BlockUntrustedIPs { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "ActivateWhitelist must be between 4 and 5 characters long.")]
        public string? ActivateWhitelist { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "ActivateWhitelistApp must be between 4 and 5 characters long.")]
        public string? ActivateWhitelistApp { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "TamperProtection must be between 4 and 5 characters long.")]
        public string? TamperProtection { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "WhiteListWiFi must be between 4 and 5 characters long.")]
        public string? WhiteListWiFi { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "AutoScan must be between 4 and 5 characters long.")]
        public string? AutoScan { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "ScanTime must be between 4 and 5 characters long.")]
        public string? ScanTime { get; set; }

        public string? ScanAtSpecificHour { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "WhitelistWebsite must be between 4 and 5 characters long.")]
        public string? WhitelistWebsite { get; set; }

        [StringLength(5, MinimumLength = 4, ErrorMessage = "DLP must be between 4 and 5 characters long.")]
        public string? DLP { get; set; }

        public string? AdvancedRemediation { get; set; }
        public string? BasicRemediation { get; set; }

        //public string? AutomatedRemediation { get; set; }

        public string? performancemonitor { get; set; }

        public string? AppManager { get; set; }

        public string? Kiosk { get; set; }
        public string? IsolationStartTime { get; set; }
        public string? IsolationRestoreTime { get; set; }
        public string? SyncN { get; set; }
        public string? FastSyncN { get; set; }
        public string? VulnAppCheck { get; set; }

        public string? FastSync { get; set; }
        public string? PerformanceMonitor { get; set; }





    }
}
