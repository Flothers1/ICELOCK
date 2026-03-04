using i_freeze.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.Utilities
{
    public class DeviceConfigurationsDTO
    {
        public DeviceDTO Device { get; set; }
        public List<string> BlockedApps { get; set; }
        public List<string> BlockedIps { get; set; }
        public List<string> BlockedWebsites { get; set; }
        public List<string> ExceptionIps { get; set; }
        public List<string> ExceptionWifi { get; set; }
        public List<string> ExceptionApps { get; set; }
        public List<string> ExceptionWebsites { get; set; }
        public List<string> WebsiteCategories { get; set; }
        public List<string> DeviceDLPScanWord { get; set; }
        public List<string> DeviceKioskApps { get; set; }
    }
}
