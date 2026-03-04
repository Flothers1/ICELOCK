using Infrastructure.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class Registry_Results
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "DeviceId is required.")]
        public Guid DeviceId { get; set; }

        [Required(ErrorMessage = "ProcessName is required.")]
        public string ProcessName { get; set; }

        [Required(ErrorMessage = "Severity is required.")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Severity must be between 2 and 5 characters.")]
        public string Severity { get; set; }

        [ValidDateTime(ErrorMessage = "Time must be a valid DateTime value.")]
        public string Time { get; set; }

        [Required(ErrorMessage = "Source is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Source must be between 3 and 50 characters.")]
        public string Source { get; set; }

    }
}
