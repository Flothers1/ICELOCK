using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class ScreenShotAnalyserPolicy
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "AnalysisName is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "AnalysisName must be between 3 and 50 characters.")]
        public string AnalysisName { get; set; }
    }
}
