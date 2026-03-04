using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class file_rules
    {
        public int Id { get; set; }
        public string file_path { get; set; } = string.Empty;
        public string label { get; set; } = string.Empty;
        public int Encrypt { get; set; }
    }
}
