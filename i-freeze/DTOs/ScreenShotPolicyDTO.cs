using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class ScreenShotPolicyDTO
    {
        public string id { get; set; }
        public string groupClassification { get; set; }
        public string permissionClassification { get; set; }
        public string dlpUserId { get; set; }
    }
}
