using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class PatternEntity
    {
        [Key]
        public int Id { get; set; }
        public string Pattern { get; set; }

        public string Label { get; set; }
    }
}
