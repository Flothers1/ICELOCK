using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class i_Freeze_Window
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Flag is required.")]
        [Range(0,1)]
        public int Flag { get; set; }
    }
}
