using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class UserLabel
    {
        [Key]
        public int  Id { get; set; }
        public string GroupClassification { get; set; }
        public string PermissionClassification { get; set; }
        public string DlpUserId { get; set; }
    }
}
