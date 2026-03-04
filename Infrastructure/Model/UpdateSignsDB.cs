using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class UpdateSignsDB
    {
        [Key]
        public int Id { get; set; }

        [Range(1.0, double.MaxValue, ErrorMessage = "Version number must be greater than 1.0.")]
        public decimal VersionNumber { get; set; }
    }
}
