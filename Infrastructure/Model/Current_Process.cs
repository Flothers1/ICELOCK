using Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class Current_Process
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Device Id is required.")]
        public Guid DeviceId { get; set; }


        [Required(ErrorMessage = "Process Name is required.")]
        public string ProcessName { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Invalid Time format.")]
        public DateTime Time { get; set; }

        public Current_Process(AppConfigAndLoginContext context)
        {
            var deviceId = context.Application_Configuration
                         .Select(config => config.DeviceSN)
                         .FirstOrDefault();

            DeviceId = deviceId;
            Time = DateTime.Now;
            Id = Guid.NewGuid();
        }
    }
}
