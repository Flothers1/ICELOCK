using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class ActivateDeviceDTO
    {
        //
        public string DeviceName { get; set; }
        public string OperatingSystemVersion { get; set; }
        public string DeviceIp { get; set; }
        public string MacAddress { get; set; }
        public string SerialNumber { get; set; }
        public string TypeOfLicense { get; set; }
    }
}
