using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class FastSyncDTO
    {
        public int Id { get; set; }
        public bool Action { get; set; } = false;
        public Guid DeviceId { get; set; }
    }
}
