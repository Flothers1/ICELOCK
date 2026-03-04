
using System.ComponentModel.DataAnnotations;


namespace Infrastructure.Model
{
    public class Apps_Versions
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "App Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "App Name must be between 3 and 100 characters long.")]
        public string AppName { get; set; }

        [Required(ErrorMessage = "App Name is required.")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Version must be between 1 and 30 characters long.")]
        public string Version { get; set; }
    }
}
