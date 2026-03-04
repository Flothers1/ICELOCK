using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class SharedFilesDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; } = "";
        public string SharedWithEmail { get; set; } = "";
        public string Password { get; set; } = "";
        public string Link { get; set; } = "";
        public DateTime? ExpirationDate { get; set; }
    }
}
