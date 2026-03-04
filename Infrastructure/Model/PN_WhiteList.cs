using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class PN_WhiteList
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "ProcessName is required.")]
        public string ProcessName { get; set; }
    }
}
