
using Infrastructure.CustomValidations;
using System.ComponentModel.DataAnnotations;


namespace Infrastructure.Model
{
    public class DevicePY
    {
        [Key]
        public int Id { get; set; }


        [Range(0.9, double.MaxValue, ErrorMessage = "Version number must be greater than 0.")]
        public decimal VersionNumber { get; set; }

        [Required(ErrorMessage = "Device Id is required.")]
        public Guid DeviceSN { get; set; }

        [Required(ErrorMessage = "Key is required.")]
        public Guid Key { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid start date format.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid end date format.")]
        [DateGreaterThan("StartDate", ErrorMessage = "End Date must be greater than Start Date.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Check Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid check date format.")]
        public DateTime CheckDate { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = "Period must be greater than 0.")]
        public double Period { get; set; }
    }
}
