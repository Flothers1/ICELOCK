using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class FilesProtections // Make Class to identify Cloumns of Blocked_IP
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Path is required.")]
        [MinLength(3, ErrorMessage = "Path Field must be greater than 3 char")]
        public string Path { get; set; }
    }
}
