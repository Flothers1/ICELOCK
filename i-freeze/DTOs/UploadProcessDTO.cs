using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class UploadProcessDTO
    {
        public int Id { get; set; }
        public string PathName { get; set; }
        public string Action { get; set; }
        public Guid DeviceId { get; set; }
    }
}
