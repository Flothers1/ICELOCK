using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class CloudURL
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "URL is required.")]
        [StringLength(100, MinimumLength = 15, ErrorMessage = "URL must be between 3 and 100 characters long.")]
        public string URL { get; set; }
    }
}
