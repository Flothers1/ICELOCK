using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class ActivateDLPEntity
    {
        [Key]
        public int Id { get; set; }

        // the license GUID you used for activation (stored as string)
        public string License { get; set; }

        // store everything as strings (your DB browser-friendly style)
        public string IsValid { get; set; }          // "true" / "false"
        public string ExpirationDate { get; set; }   // ISO string or null
        public string DeviceId { get; set; }         // GUID string or null

        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("O");
        public string Email { get; set; }
        public string Password { get; set; }


    }
}
