
using System.ComponentModel.DataAnnotations;


namespace Infrastructure.Model
{
    public class DeviceActionWebsites
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Device Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "App Name must be between 3 and 100 characters long.")]
        public string DeviceName { get; set; }

        [Required(ErrorMessage = "Action is required.")]
        [Range(0, 1)]
        public int Action { get; set; }
    }
}
