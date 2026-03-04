using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class InstalledApps
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "VersionNumber is required.")]
        [Range(0.9, double.MaxValue, ErrorMessage = "Version number must be greater than 0.9.")]
        public decimal VersionNumber { get; set; }
    }
}
