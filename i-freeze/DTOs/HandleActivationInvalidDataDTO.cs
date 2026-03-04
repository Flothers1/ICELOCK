using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    internal class HandleActivationInvalidDataDTO
    {
        public string title { get; set; }
        public errors? errors { get; set; }
       
    }

    internal class errors
    {
        public List<string>? DeviceIp { get; set; }
        public List<string>? MacAddress { get; set; }
        public List<string>? SerialNumber { get; set; }
        public List<string>? OperatingSystemVersion { get; set; }
        public List<string>? DeviceName { get; set; }
        public List<string>? TypeOfLicense { get; set; }
    }
}
