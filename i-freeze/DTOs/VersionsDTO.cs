using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class VersionsDTO
    {
        public decimal VersionNumber { get; set; }
        public string VersionType { get; set; }
        public string VersionDescription { get; set; }
        public string ZipFilePath { get; set; }
        public string Status { get; set; }
        public Guid? DeviceId { get; set; }
    }
}
