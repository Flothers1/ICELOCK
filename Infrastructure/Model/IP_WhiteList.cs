using Infrastructure.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Model
{
    public class IP_WhiteList // Make Class to identify Cloumns of Blocked_APP
    {
        [Key]
        public int Id { get; set; }
        public string? ProcessName { get; set; }

        [Required(ErrorMessage = "RemoteIP is required.")]
        [ValidIPAddress(ErrorMessage = "Remote IP must be a valid IP address.")]
        public string RemoteIP { get; set; }
        public string? RemotePort { get; set; }
    }
}