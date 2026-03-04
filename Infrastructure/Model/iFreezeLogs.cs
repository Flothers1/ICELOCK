using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class iFreezeLogs
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Log is required.")]
        public string Log { get; set; }
    }
}
