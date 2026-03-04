using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class ActivateDlpResponse
    {
        public string  License { get; set; }
        public string IsValid { get; set; }
        public string ExpirationDate { get; set; }
        public string DeviceId { get; set; }
    }
}
