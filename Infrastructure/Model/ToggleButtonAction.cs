using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Model
{
    public class ToggleButtonAction
    {
        [Key]
        public string Toggle_Name { get; set; }

        [Required(ErrorMessage = "Severity is required.")]
        [Range(0,1)]
        public int Action { get; set; }
    }
}