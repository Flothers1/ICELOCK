using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class DlpUserSettingsDTO
    {
        [JsonProperty("automaticClassification")]
        public bool AutomaticClassification { get; set; }
        [JsonProperty("realtimeClassification")]

        public bool RealtimeClassification { get; set; }
        [JsonProperty("screenshotAnalyzer")]

        public bool ScreenshotAnalyzer { get; set; }
        [JsonProperty("decryptAllData")]

        public bool DecryptAllData { get; set; }
    }
}
