using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class ActivateDeviceDlpDTO
    {
        [JsonProperty("deviceName")] public string DeviceName { get; set; }
        [JsonProperty("operatingSystemVersion")] public string OperatingSystemVersion { get; set; }
        [JsonProperty("deviceIp")] public string DeviceIp { get; set; }
        [JsonProperty("macAddress")] public string MacAddress { get; set; }
        [JsonProperty("serialNumber")] public string SerialNumber { get; set; }
        [JsonProperty("typeOfLicense")] public string TypeOfLicense { get; set; } = "DLP";
    }
}
