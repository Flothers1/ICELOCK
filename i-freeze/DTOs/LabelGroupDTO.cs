using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_freeze.DTOs
{
    public class LabelGroupDTO
    {
        public string labelName { get; set; }
        public List<string> patterns { get; set; }
    }
}
