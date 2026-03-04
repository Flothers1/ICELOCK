using Infrastructure.CustomValidations;
using System.ComponentModel.DataAnnotations;


namespace Infrastructure.Model
{

    public class Configurations
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Device ID is required.")]
        public Guid DeviceSN { get; set; }

    }

}