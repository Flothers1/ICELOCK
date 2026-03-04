using Infrastructure.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class EventViewer // Make Class to identify Cloumns of Blocked_APP
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "DeviceId is required.")]
        public Guid DeviceId { get; set; }

        [ValidDateTime(ErrorMessage = "Time must be a valid DateTime value.")]
        public string Time { get; set; }

        [Required(ErrorMessage = "LogName is required.")]
        public string LogName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Source is required.")]
        public string Source { get; set; }

        [Required(ErrorMessage = "Level is required.")]
        public string Level { get; set; }
    }
}
