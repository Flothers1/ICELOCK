using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class PowerShellCommandDTO
    {
        public int Id { get; set; }
        public string CommandName { get; set; }
        public Guid DeviceId { get; set; }
    }
}
