using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class UserLabelDTO
    {
        [JsonProperty("id")]

        public string id { get; set; }
        [JsonProperty("groupClassification")]
        public string groupClassification { get; set; }
        [JsonProperty("permissionClassification")]
        public string permissionClassification { get; set; }
        [JsonProperty("dlpUserId")]
        public string dlpUserId { get; set; }
    }
}
