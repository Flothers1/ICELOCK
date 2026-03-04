using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class SharedFile
    {
            [Key]
            public int Id { get; set; }

            public string FileName { get; set; }

            // email of the user it was shared with
            public string SharedWithEmail { get; set; }

            // optional password
            public string Password { get; set; }

            // link to the shared file
            public string Link { get; set; }

            // store expiration as a string (text in SQLite)
            public string?  ExpirationDate { get; set; }

            // renamed and switched to string so DB Browser will show it as TEXT
            // use ISO 8601 when setting this: DateTime.UtcNow.ToString("o")
            public string SharedAt { get; set; } = DateTime.UtcNow.ToString("o");
    }
}
