using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class Exception_Wifi
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Wifi Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Wifi Name must be between 3 and 100 characters long.")]
        public string Wifi_Name { get; set; }
    }
}
